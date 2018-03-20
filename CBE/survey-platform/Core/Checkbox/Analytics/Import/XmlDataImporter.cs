using System;
using System.Collections.Generic;
using System.Xml;
using System.Data;

using Checkbox.Forms;
using Checkbox.Analytics.Data;
using Checkbox.Progress;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items;

using Prezza.Framework.Data;
using Prezza.Framework.Security.Principal;
using Checkbox.Security.Principal;

namespace Checkbox.Analytics.Import
{
    /// <summary>
    /// XmlDataImporter class
    /// </summary>
    public static class XmlDataImporter
    {

        private static string ReadElementValue(this XmlTextReader reader, string element)
        {
            if (!reader.Name.Equals(element) || reader.NodeType != XmlNodeType.Element)
            {
                if (!reader.MoveToNextElement(element))
                    throw new InvalidOperationException("Can not import Survey (" + element + "). XML file is not valid");
            }

            if (!reader.Read())
                throw new InvalidOperationException("Can not import Survey (" + element + "). XML node is empty");

            if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.Whitespace)
                return null;

            return reader.Value;
        }

        /// <summary>
        /// Parse XML and validate responses and survey items
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="survey"></param>
        /// <param name="items"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        public static List<string> ParseAndValidate(XmlTextReader reader, ResponseTemplate survey, out List<ItemInfo> items, out List<ResponseInfo> responses)
        {
            items = null;
            responses = null;

            List<string> errors = new List<string>();

            if (!reader.MoveToNextElement("CheckboxResponseExport"))
            {
                errors.Add("Can not import response without CheckboxResponseExport XML element");

                return errors;
            }

            int surveyId = int.Parse(reader.ReadElementValue("SurveyId"));

            string surveyName = reader.ReadElementValue("SurveyName");

            items = ParseItems(reader);

            if (items == null || items.Count == 0)
            {
                errors.Add("Source survey has no items to import");

                return errors;
            }

            responses = ParseResponses(reader);

            if (responses == null || responses.Count == 0)
            {
                errors.Add("Source survey has no responses to import");

                return errors;
            }

			int[] itemIdArr = survey.ListTemplateItemIds();
            // match items
			foreach (int targetItemId in itemIdArr)
            {
				ItemData targetItem = survey.GetItem(targetItemId);

                MatrixItemData matrix = targetItem as MatrixItemData;

                if (matrix != null)
                    ValidateMatrixItem(survey, items, matrix, errors);
                else
                    ValidateItem(survey, items, targetItem, errors);
            }

            // match responses
            foreach (ResponseInfo resp in responses)
            {
                Guid guid = new Guid(resp.responseGuid);
                DataRow dr = ResponseManager.GetResponseDataRow(guid);

                if (dr != null)
                {
                    bool deleted = dr["Deleted"] is bool ? (bool)dr["Deleted"] : false;

                    if (deleted)
                        errors.Add("Unable to restore deleted response. " + resp.responseGuid + ". Will be skipped.");
                }
            }

            return errors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="survey"></param>
        /// <param name="progress"></param>
        public static List<string> Import(ResponseTemplate survey, List<ItemInfo> srcItems, List<ResponseInfo> responses, string progress, StateImportInfo stateImportInfo)
        {
            List<string> errors = new List<string>();

            float curResponseIndex = -1;
            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                foreach (ResponseInfo resp in responses)
                {
                    // If user click "Cancel", stop the process
                    if (stateImportInfo.IsCanceled)
                        break;
                    curResponseIndex++;

                    int percent = (int)((curResponseIndex / responses.Count) * 70); // 0-70

                    int r = percent % 10;

                    if (r == 0)
                        SetProgress(ProgressStatus.Running, 20 + percent, "Importing survey responses.", progress);

                    Guid guid = new Guid(resp.responseGuid);
                    DataRow dr = ResponseManager.GetResponseDataRow(guid);

                    if (dr != null)
                    {
                        DateTime lastEdit = (DateTime)dr["LastEdit"];
                        bool deleted = dr["Deleted"] is bool ? (bool)dr["Deleted"] : false;

                        if (deleted)
                            continue;

                        if (lastEdit >= resp.lastEdit)
                            continue;// we have this response already
                    }

                    Response newResponse = null;
					int[] itemIdArr = survey.ListTemplateItemIds();
					foreach (int targetItemId in itemIdArr)
                    {
						ItemData targetItem = survey.GetItem(targetItemId);
                        MatrixItemData matrix = targetItem as MatrixItemData;

                        if (matrix != null)
                            ImportMatrixItem(survey, srcItems, resp, matrix, ref newResponse, transaction);
                        else
                            ImportItem(survey, srcItems, resp, targetItem, ref newResponse, transaction);
                    }

                    if (newResponse != null)
                    {
                        newResponse.SaveImportedState(resp.started, resp.ended, resp.lastEdit, resp.isComplete);
                    }
                }
                if (stateImportInfo.IsCanceled)
                    transaction.Rollback();
                else
                    transaction.Commit();
            }

            return errors;
        }

        private static void ImportMatrixItem(ResponseTemplate survey, List<ItemInfo> srcItems, ResponseInfo resp, MatrixItemData matrix, ref Response newResponse, IDbTransaction transaction)
        {
            if (resp.answers == null || resp.answers.Count == 0)
                return;

			//ItemData[] children = null;// matrix.GetChildItemArray();

            for (int col = 1; col <= matrix.ColumnCount; col++)
            {
                for (int row = 1; row <= matrix.RowCount; row++)
                {
                    ItemData targetItem = matrix.GetItemAt(row, col);
                    string alias = matrix.GetRowColumnAlias(row, col);

                    LightweightItemMetaData lightSrcItem = matrix.GetLightweightItem(targetItem.ID.Value);
                    ItemInfo srcItem = MatchItem(lightSrcItem, alias, targetItem.ItemTypeID, srcItems, resp.language);

                    if(srcItem.alias != alias)
                        srcItem = MatchItem(lightSrcItem, alias, targetItem.ItemTypeID, srcItems, resp.language);

                    if (srcItem == null)
                        continue;

                    if (newResponse == null)
                    {
                        object princ = Users.UserManager.GetCurrentPrincipal();
                        newResponse = survey.CreateResponse(resp.language);
                        newResponse.Initialize(resp.ip, resp.networkUser, resp.language, false, resp.Invitee, princ as CheckboxPrincipal, null, resp.started);
                    }

                    Item si = newResponse.GetItem(targetItem.ID.Value);

                    si.ImportAnswers(srcItem, resp.answers);
                }
            }
        }

        private static void ImportItem(ResponseTemplate survey, List<ItemInfo> srcItems, ResponseInfo resp, ItemData targetItem, ref Response newResponse, IDbTransaction transaction)
        {
            if (resp.answers == null || resp.answers.Count == 0)
                return;

			LightweightItemMetaData lightSrcItem = SurveyMetaDataProxy.GetItemData(targetItem.ID.Value, true);// survey.GetLightweightItem(targetItem.ID.Value, true);
            ItemInfo srcItem = MatchItem(lightSrcItem, targetItem.Alias, targetItem.ItemTypeID, srcItems, resp.language);

            if (srcItem == null)
                return;

            if (newResponse == null)
            {                
                object princ = Guid.Empty.ToString().Equals(resp.respondentGuid) ? Users.UserManager.GetUserPrincipal(resp.uniqueIdentifier) : new Checkbox.Forms.Security.Principal.AnonymousRespondent(Guid.Parse(resp.respondentGuid));
                newResponse = survey.CreateResponse(resp.language);
                newResponse.AnonymizeResponses = resp.isAnonymized;
                newResponse.Initialize(resp.ip, resp.networkUser, resp.language, resp.isTest, resp.Invitee, princ as CheckboxPrincipal, null, resp.started);
            }

            Item si = newResponse.GetItem(targetItem.ID.Value);

            si.ImportAnswers(srcItem, resp.answers);
        }

        private static void ValidateMatrixItem(ResponseTemplate survey, List<ItemInfo> srcItems, MatrixItemData matrix, List<string> errors)
        {
			//ItemData[] children = null;// matrix.GetChildItemArray();

            for (int col = 1; col <= matrix.ColumnCount; col++)
            {
                for (int row = 1; row <= matrix.RowCount; row++)
                {
                    ItemData targetItem = matrix.GetItemAt(row, col);
                    string alias = matrix.GetRowColumnAlias(row, col);

                    LightweightItemMetaData lightSrcItem = matrix.GetLightweightItem(targetItem.ID.Value);
                    ItemInfo srcItem = MatchItem(lightSrcItem, alias, targetItem.ItemTypeID, srcItems, survey.LanguageSettings.DefaultLanguage);

                    if (srcItem == null)
                        errors.Add(string.Format("Survey '{0}' has question item that will be skipped. {1}", survey.Name, lightSrcItem));
                }
            }
        }

        private static void ValidateItem(ResponseTemplate survey, List<ItemInfo> srcItems, ItemData targetItem, List<string> errors)
        {
			LightweightItemMetaData lightSrcItem = SurveyMetaDataProxy.GetItemData(targetItem.ID.Value, true);// survey.GetLightweightItem(targetItem.ID.Value, true);
            lightSrcItem.PagePosition = survey.GetPagePositionForItem(targetItem.ID.Value).Value;
            ItemInfo srcItem = MatchItem(lightSrcItem, targetItem.Alias, targetItem.ItemTypeID, srcItems, survey.LanguageSettings.DefaultLanguage);

            if (srcItem == null)
                errors.Add(string.Format("Survey '{0}' has question item that will be skipped. {1}", survey.Name, lightSrcItem));
        }

        private static ItemInfo MatchItem(LightweightItemMetaData targetItem, string targetAlias, int itemTypeId, List<ItemInfo> srcItems, string lang)
        {
            for (int i = 0; i < srcItems.Count; i++)
            {
                if (srcItems[i].MatchAlias(targetItem, targetAlias, itemTypeId, lang))
                    return srcItems[i];
            }

            for (int i = 0; i < srcItems.Count; i++)
            {
                if (srcItems[i].Match(targetItem, targetAlias, itemTypeId, lang))
                    return srcItems[i];
            }

            return null;
        }

        private static void SetProgress(ProgressStatus status, int value, string message, string pKey)
        {
            ProgressProvider.SetProgress(pKey,
                new ProgressData
                {
                    CurrentItem = value,
                    Status = status,
                    Message = message,
                    TotalItemCount = 100
                });
        }

        private static List<ResponseInfo> ParseResponses(XmlTextReader reader)
        {
            if (reader.Name != "Responses")
                if (!reader.MoveToNextElement("Responses"))
                    return null;

            if (reader.IsEmptyElement)
                return null;

            List<ResponseInfo> responseList = new List<ResponseInfo>();
            ResponseInfo resp = ParseResponse(reader);

            while (resp != null)
            {
                if (resp.answers != null && resp.answers.Count > 0)
                    responseList.Add(resp);

                resp = ParseResponse(reader);
            }

            return responseList;
        }

        private static ResponseInfo ParseResponse(XmlTextReader reader)
        {
            if (reader.Name != "Response")
                if (!reader.MoveToNextElement("Response"))
                    return null;

            ResponseInfo resp = new ResponseInfo();

            resp.responseId = int.Parse(reader.ReadElementValue("ResponseId"));
            resp.responseGuid = reader.ReadElementValue("GUID");
            int templateId = int.Parse(reader.ReadElementValue("ResponseTemplateId"));
            resp.isComplete = bool.Parse(reader.ReadElementValue("IsComplete"));
            resp.lastPageViewed = int.Parse(reader.ReadElementValue("LastPageViewed"));
            resp.started = DateTime.Parse(reader.ReadElementValue("Started"));
            string ended = reader.ReadElementValue("Ended");

            if (!string.IsNullOrEmpty(ended))
                resp.ended = DateTime.Parse(ended);

            resp.ip = reader.ReadElementValue("IP");
            string lastEdit = reader.ReadElementValue("LastEdit");
            if (!string.IsNullOrEmpty(lastEdit))
                resp.lastEdit = DateTime.Parse(lastEdit);
            resp.networkUser = reader.ReadElementValue("NetworkUser");
            resp.language = reader.ReadElementValue("Language");
            resp.uniqueIdentifier = reader.ReadElementValue("UniqueIdentifier");
            bool isDeleted = bool.Parse(reader.ReadElementValue("Deleted"));
            resp.respondentGuid = reader.ReadElementValue("RespondentGUID");
            resp.isTest = bool.Parse(reader.ReadElementValue("IsTest"));
            resp.isAnonymized = bool.Parse(reader.ReadElementValue("IsAnonymized"));            
            resp.Invitee = reader.ReadElementValue("Invitee");
            resp.answers = ParseAnswers(reader);

            return resp;
        }

        private static List<ItemAnswer> ParseAnswers(XmlTextReader reader)
        {
            if (reader.Name != "Answers")
                if (!reader.MoveToNextElement("Answers"))
                    return null;

            if (reader.IsEmptyElement)
                return null;

            ItemAnswer answer = ParseAnswer(reader);
            List<ItemAnswer> answers = new List<ItemAnswer>();

            while (answer != null)
            {
                answers.Add(answer);
                answer = ParseAnswer(reader);
            }

            return answers;
        }

        private static ItemAnswer ParseAnswer(XmlTextReader reader)
        {
            if (reader.Name != "Answer" || reader.NodeType != XmlNodeType.Element)
                if (!reader.MoveToNextElement("Answer"))
                    return null;

            ItemAnswer answer = new ItemAnswer();

            answer.AnswerId = long.Parse(reader.ReadElementValue("AnswerId"));
            answer.ItemId = int.Parse(reader.ReadElementValue("ItemId"));
            answer.AnswerText = reader.ReadElementValue("AnswerText");
            string optionId = reader.ReadElementValue("OptionId");
            string points = reader.ReadElementValue("Points");

            if (!string.IsNullOrEmpty(optionId))
                answer.OptionId = int.Parse(optionId);

            if (!string.IsNullOrEmpty(points))
                answer.Points = double.Parse(points);

            //answer.CreatedDate = DateTime.Parse(reader.ReadElementValue("DateCreated"));

			//string cDate = reader.ReadElementValue("DateCreated");
			//string modDate = reader.ReadElementValue("ModifiedDate");

            //if (modDate != null)
            //    answer.LastEdit = DateTime.Parse(modDate);)

            return answer;
        }

        private static List<ItemInfo> ParseItems(XmlTextReader reader)
        {
            if (reader.Name != "Items")
                if (!reader.MoveToNextElement("Items"))
                    return null;

            if (reader.IsEmptyElement)
                return null;

            List<ItemInfo> items = new List<ItemInfo>();
            ItemInfo item = ParseItem(reader);

            while (item != null)
            {
                items.Add(item);
                item = ParseItem(reader);
            }

            return items;
        }

        private static ItemInfo ParseItem(XmlTextReader reader)
        {
            if (reader.Name != "Item")
                if (!reader.MoveToNextElement("Item"))
                    return null;

            ItemInfo item = new ItemInfo();

            item.itemId = int.Parse(reader.ReadElementValue("ItemId"));
            item.itemTypeId = int.Parse(reader.ReadElementValue("ItemType"));
            item.pagePos = int.Parse(reader.ReadElementValue("PagePosition"));
            item.itemPos = int.Parse(reader.ReadElementValue("ItemPosition"));
            item.alias = reader.ReadElementValue("Alias");
            item.text = reader.ReadElementValue("Text");

            item.parentItemId = reader.ReadElementValue("ParentItemId");

            string row = reader.ReadElementValue("Row");

            if (row != null)
            {
                item.row = int.Parse(row);

                string column = reader.ReadElementValue("Column");

                if (column != null)
                    item.column = int.Parse(column);
            }

            item.options = ParseOptions(reader);

            return item;
        }

        private static List<OptionInfo> ParseOptions(XmlTextReader reader)
        {
            if (!reader.MoveToNextElement("Options"))
                return null;

            if (reader.IsEmptyElement)
                return null;

            List<OptionInfo> options = new List<OptionInfo>();
            OptionInfo opt = ParseOption(reader);

            while (opt != null)
            {
                options.Add(opt);
                opt = ParseOption(reader);
            }

            return options;
        }

        private static OptionInfo ParseOption(XmlTextReader reader)
        {
            if (!reader.MoveToNextElement(null))
                return null;

            if (reader.Name != "Option")
                return null;

            OptionInfo option = new OptionInfo();

            option.optionId = int.Parse(reader.ReadElementValue("OptionId"));
            option.position = int.Parse(reader.ReadElementValue("Position"));
            option.alias = reader.ReadElementValue("Alias");
            option.text = reader.ReadElementValue("Text");

            return option;
        }
    }

	/// <summary>
	/// 
	/// </summary>
    public class ResponseInfo
    {
        public DateTime ended;
        public string ip;
        public bool isComplete;
        public string language;
        public DateTime lastEdit;
        public int lastPageViewed;
        public string networkUser;
        public string Invitee;
        public int responseId;
        public DateTime started;
        public string uniqueIdentifier;
        public string responseGuid;
	    public string ResumeKey;
	    public string sessionGuid;
        public bool isTest;
        public bool isAnonymized;
        public string respondentGuid;

        public List<ItemAnswer> answers;
    }

	/// <summary>
	/// 
	/// </summary>
    public class OptionInfo
    {
        public int optionId;
        public int position;
        public string alias;
        public string text;
    }

	/// <summary>
	/// 
	/// </summary>
    public class StateImportInfo
    {
		/// <summary>
		/// 
		/// </summary>
        public bool IsCanceled
        {
            get;
            set;
        }
    }

	/// <summary>
	/// 
	/// </summary>
    public class ItemInfo
    {
        public int itemId;
        public int itemTypeId;
        public int pagePos;
        public int itemPos;
        public string alias;
        public string text;
        public string parentItemId;
        public int row = -1;
        public int column = -1;
        public List<OptionInfo> options;

        internal bool Match(LightweightItemMetaData targetItem, string targetAlias, int itemType, string lang)
        {
            if (alias == null)
            {
                return MatchPosition(targetItem) && itemTypeId == itemType;
            }

            // item sith same alias exists?
            if (alias != targetAlias)
            {
                return MatchByText(targetItem, itemType, lang);
            }
            else
            {
                // item is correct type?
                if (itemTypeId == itemType)
                    return true;
                else
                    return MatchByText(targetItem, itemType, lang);
            }
        }

        internal bool MatchAlias(LightweightItemMetaData targetItem, string targetAlias, int itemType, string lang)
        {
            if (string.IsNullOrEmpty(alias))
                return false;

            // item sith same alias exists?
            if (alias != targetAlias)
            {
                return false;
            }
            else
            {
                // item is correct type?
                if (itemTypeId == itemType)
                    return true;
                else
                    return false;
            }
        }

        private bool MatchByText(LightweightItemMetaData targetItem, int itemType, string lang)
        {
            // item with same text exists?
            if (text != targetItem.GetText(false, lang))
            {
                // item atsame position exists?
                if (!MatchPosition(targetItem))
                    return false;

                if (itemTypeId != itemType)
                    return false;

                return true;
            }
            else
            {
                // item is correct type?
                if (itemTypeId == itemType)
                    return true;
                else
                {
                    // item atsame position exists?
                    if (MatchPosition(targetItem))
                        return true;

                    return false;
                }
            }
        }

        private bool MatchPosition(LightweightItemMetaData targetItem)
        {
            if (row >= 0 && targetItem.Coordinate != null)
                return targetItem.Coordinate.X == row && targetItem.Coordinate.Y == column;

            if (itemPos != targetItem.ItemPosition)
                return false;

            if (pagePos != targetItem.PagePosition)
                return false;

            return true;
        }

        internal OptionInfo FindOption(int? optId, out int optIndex)
        {
            optIndex = -1;

            if (options == null || !optId.HasValue)
                return null;

            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].optionId == optId.Value)
                {
                    optIndex = i;
                    return options[i];
                }
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})[{2},{3}] Type:{4}", text, alias, row, column, itemTypeId);
        }
    }

}
