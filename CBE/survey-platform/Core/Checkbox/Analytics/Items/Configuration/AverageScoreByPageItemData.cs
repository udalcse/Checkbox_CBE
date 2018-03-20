using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AverageScoreByPageItemData : AverageScoreItemData
    {
        private List<int> _sourcePages;

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new AverageScoreByPageItem();
        }

        /// <summary>
        /// List source item ids
        /// </summary>
        public List<int> SourcePageIds
        {
            get { return _sourcePages ?? (_sourcePages = new List<int>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override AverageScoreCalculation DefaultAverageScoreCalculationValue
        {
            get { return AverageScoreCalculation.PageAverages; }
        }

        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ItemMetaData)
            {
                ((ItemMetaData)itemDto).TextData =
                    ResponseTemplateManager.ActiveSurveyLanguages.Select(
                        language => new TextData { LanguageCode = language, TextValues = new SimpleNameValueCollection() })
                        .ToArray();

                //Pick first source item to use for text
                var textDataList = ((ItemMetaData)itemDto).TextData;

                //
                foreach (var textData in textDataList)
                {
                    if (SourcePageIds.Count > 0)
                    {
                        textData.TextValues["NavText"] = TextManager.GetText("/controlText/averageScoreByPageItem/pageSource") 
                            + " " + (ResponseTemplateManager.GetPagePositionById(SourcePageIds.First()) - 1);
                    }
                    else if (AverageScoreCalculation == AverageScoreCalculation.PageAveragesWithTotalScore)
                    {
                        textData.TextValues["NavText"] = TextManager.GetText("/controlText/averageScoreByPageItem/totalSurveySource");
                    }
                }
            }
        }

        #region Load

        /// <summary>
        /// Load the item from the database
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            if (data == null)
            {
                throw new Exception("Unable to load analysis item from NULL data.");
            }

            base.LoadAdditionalData(data);

            SourcePageIds.Clear();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetAIPageSourceIds");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var pageId = DbUtility.GetValueFromDataReader(reader, "SourcePageID", default(int?));
                        if (pageId.HasValue)
                            SourcePageIds.Add(pageId.Value);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Update source item tables
        /// </summary>
        /// <param name="t"></param>
        protected override void UpdateSourceItemTables(IDbTransaction t)
        {
            base.UpdateSourceItemTables(t);

            Database db = DatabaseFactory.CreateDatabase();
            
            //Delete source item mappings
            DBCommandWrapper deleteSourcePageCommand = GetDeleteSourcePageCommand(db);
            db.ExecuteNonQuery(deleteSourcePageCommand, t);

            //Add mappings
            foreach (int sourcePageId in SourcePageIds)
            {
                DBCommandWrapper insertSourcePageCommand = GetInsertSourcePageCommand(db);
                insertSourcePageCommand.AddInParameter("SourcePageID", DbType.Int32, sourcePageId);
                db.ExecuteNonQuery(insertSourcePageCommand, t);
            }
        }

        /// <summary>
        /// Get a command to insert a source page association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertSourcePageCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertAIPageSource");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        /// Get a command to delete a source page association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeleteSourcePageCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeleteAIPageSource");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        #endregion

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("SourcePageIds", String.Join(",", SourcePageIds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var pages = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("SourcePageIds"));

            if (!String.IsNullOrEmpty(pages))
                _sourcePages = pages.Split(',').Select(int.Parse).ToList();
        }

        #endregion
    }
}
