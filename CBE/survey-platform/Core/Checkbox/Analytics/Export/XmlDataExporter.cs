using System;
using System.IO;
using System.Xml;
using System.Data;

using Checkbox.Common;
using Checkbox.Pagination;
using Checkbox.Progress;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Security;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// Survey data exporter that writes results data to a XML file.
    /// </summary>
    public class XmlDataExporter : SurveyDataExporter
    {
        /// <summary>
        /// Get/set field index to start with when writing a row of data
        /// </summary>
        protected int StartFieldIndex { get; set; }

        /// <summary>
        /// Get/set field index to end with when writing a row of data.
        /// </summary>
        protected int EndFieldIndex { get; set; }

        /// <summary>
        /// Get/set newline replacement character
        /// </summary>
        protected string NewLineReplacement { get; set; }

        /// <summary>
        /// Get/set whether to replace newlines
        /// </summary>
        protected bool ReplaceNewLines { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public XmlDataExporter()
        {
            NewLineReplacement = ApplicationManager.AppSettings.NewLineReplacement;

            //Make sure we don't want to actually preserve new lines in CSV
            ReplaceNewLines = !NewLineReplacement.Equals("NEW_LINE", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Create the analysis template to use for the exporter
        /// </summary>
        /// <returns></returns>
        protected override AnalysisTemplate CreateAnalysisTemplate()
        {
            return AnalysisTemplateManager.GenerateXMLExportTemplate(ResponseTemplate, Options);
        }

        /// <summary>
        /// Populate field names.
        /// </summary>
        /// <remarks>This override figures out which columns to include when splitting the
        /// export across files.</remarks>
        protected override void PopulateFieldNames(bool forceQuestionReload)
        {
            base.PopulateFieldNames(forceQuestionReload);

            //CalculateStartAndEndIndices();
        }

        /// <summary>
        /// Write the export data to the specified file path.
        /// </summary>
        /// <param name="filePath">File path to write export to.</param>
        public override void WriteToFile(string filePath)
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                try
                {
                    var xmlWriter = new XmlTextWriter(writer) {Formatting = Formatting.Indented};

                    WriteToXmlWriter(xmlWriter);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "UIProcess");
                    throw;
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Write export to text writer.
        /// </summary>
        /// <param name="writer">Text writer to write export data to.</param>
        public override void WriteToTextWriter(TextWriter writer)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 0,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/analyzingSurveyStructure", LanguageCode)
                    }
                );
            }

            //Get field names
            PopulateFieldNames(true);

            //Pre-Load profile information in bulk if necessary.  Greatly speeds export
            // when user options are included.
            if (Options.IncludeDetailedUserInfo)
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        TotalItemCount = 100,
                        CurrentItem = 0,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/loadingUserProfileData", LanguageCode)
                    }
                );

                //Do the load
                ProfileManager.CheckboxProvider.PreLoadProfilesForTemplateResponses(ResponseTemplateId);
            }

            var xmlWriter = new XmlTextWriter(writer);

            //Write export
            WriteExportData(xmlWriter);

            //Set completed progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/exportManager/completed", LanguageCode),
                        TotalItemCount = 100
                    }
                );
            }
        }

        /// <summary>
        /// Write export to XML text writer.
        /// </summary>
        /// <param name="writer">Text writer to write export data to.</param>
        public void WriteToXmlWriter(XmlTextWriter writer)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 0,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/analyzingSurveyStructure", LanguageCode)
                    }
                );
            }

            //Get field names
            //PopulateFieldNames(true);

            //Write export
            WriteExportData(writer);

            //Set completed progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/exportManager/completed", LanguageCode),
                        TotalItemCount = 100
                    }
                );
            }
        }

        /// <summary>
        /// Perform the actual working of writing export data to the specified text
        /// writer.
        /// </summary>
        /// <param name="writer">Text writer to write dtata to.</param>
        protected override void WriteExportData(TextWriter writer)
        {
            throw new NotImplementedException("Use WriteExportData(XmlTextWriter writer) method instead");
        }

        /// <summary>
        /// Write exported data to the specified text writer
        /// </summary>
        /// <param name="writer"></param>
        private void WriteExportData(XmlTextWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("CheckboxResponseExport");

            //Write headers
            WriteHeaders(writer, ResponseTemplate, ResponseTemplate.Name);

            //Write responses
            writer.WriteStartElement("Responses");
            WriteResponses(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();

        }

        /// <summary>
        /// Write headers to the writer
        /// </summary>
        /// <param name="writer"></param>
        public void WriteHeaders(XmlTextWriter writer, Template survey, string name)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                //Start progress at 70, since we'll consider 70% of the work to be loading survey & items,
                // and answer data from database by analysis
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 70,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/writingHeaders", LanguageCode)
                    }
                );
            }

            writer.WriteElementString("SurveyId", survey.ID.ToString());
            WriteCData(writer, "SurveyName", name);

            writer.WriteStartElement("Items");
			int[] pageIdArr = survey.ListTemplatePageIds();
            
			foreach (int tPageId in pageIdArr)
            {
				TemplatePage tPage = survey.GetPage(tPageId);

				int[] itemIdList = tPage.ListItemIds();
				int itemIdx = 0;

				foreach (int itemId in itemIdList)
                {
					ItemData item = survey.GetItem(itemId);
					int? itemPos = survey.GetItemPositionWithinPage(itemId);

                    WriteItem(writer, tPage, item, itemPos.HasValue ? itemPos.Value : itemIdx);

					itemIdx++;

                    var matrix = item as MatrixItemData;

                    if (matrix == null)
                    {
                        continue;
                    }

                    for (int col = 1; col <= matrix.ColumnCount; col++)
                    {
                        for (int row = 1; row <= matrix.RowCount; row++)
                        {
                            ItemData iData = matrix.GetItemAt(row, col);

                            WriteMatrixItem(writer, tPage, matrix, iData, row, col);
                        }
                    }
                }
            }
            
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="page"></param>
        /// <param name="matrix"></param>
        /// <param name="item"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void WriteMatrixItem(XmlTextWriter writer, TemplatePage page, MatrixItemData matrix, ItemData item, int row, int column)
        {
            writer.WriteStartElement("Item");
            writer.WriteElementString("ItemId", item.ID.ToString());
            writer.WriteElementString("ItemType", item.ItemTypeID.ToString());
            writer.WriteElementString("PagePosition", page.Position.ToString());
            writer.WriteElementString("ItemPosition", "0");

            string alias = matrix.GetRowColumnAlias(row, column);

            writer.WriteElementString("Alias", alias);

            LightweightItemMetaData lwmd = matrix.GetLightweightItem(item.ID.Value);
            string text = lwmd.GetText(false, LanguageCode);
            
            writer.WriteElementString("Text", text);

            writer.WriteElementString("ParentItemId", matrix.ID.Value.ToString());

            writer.WriteStartElement("Row");

            if (lwmd.Coordinate != null)
                writer.WriteString(lwmd.Coordinate.X.ToString());

            writer.WriteEndElement();

            writer.WriteStartElement("Column");

            if (lwmd.Coordinate != null)
                writer.WriteString(lwmd.Coordinate.Y.ToString());

            writer.WriteEndElement();

            //get item options from the prototype
            int prototypeID = matrix.GetColumnPrototypeId(column);
            if (prototypeID > 0)
            {
                item = ItemConfigurationManager.GetConfigurationData(prototypeID);
            }

            WriteItemOptions(writer, item); 

            //ItemAnswer ia;

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="page"></param>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        private void WriteItem(XmlTextWriter writer, TemplatePage page, ItemData item, int pos)
        {
            writer.WriteStartElement("Item");
            writer.WriteElementString("ItemId", item.ID.ToString());
            writer.WriteElementString("ItemType", item.ItemTypeID.ToString());
            writer.WriteElementString("PagePosition", page.Position.ToString());
            writer.WriteElementString("ItemPosition", pos.ToString());

            WriteCData(writer, "Alias", item.Alias);
            
			LightweightItemMetaData lwmd = SurveyMetaDataProxy.GetItemData(item.ID.Value, true);
			
            string text = lwmd.GetText(false, LanguageCode);
            WriteCData(writer, "Text", text);

            writer.WriteElementString("ParentItemId", lwmd.AncestorId.ToString());

            writer.WriteStartElement("Row");

            if (lwmd.Coordinate != null)
                writer.WriteString(lwmd.Coordinate.Y.ToString());

            writer.WriteEndElement();

            writer.WriteStartElement("Column");

            if (lwmd.Coordinate != null)
                writer.WriteString(lwmd.Coordinate.X.ToString());

            writer.WriteEndElement();

            WriteItemOptions(writer, item);

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item"></param>
        private void WriteItemOptions(XmlTextWriter writer, ItemData item)
        {
            writer.WriteStartElement("Options");

            var sid = item as SelectItemData;

            if (sid != null)
            {
                foreach (ListOptionData lod in sid.Options)
                {
                    writer.WriteStartElement("Option");

                    writer.WriteElementString("OptionId", lod.OptionID.ToString());
                    writer.WriteElementString("Position", lod.Position.ToString());
                    WriteCData(writer, "Alias", lod.Alias);

                    string otext = TextManager.GetText(lod.TextID, LanguageCode);

                    WriteCData(writer, "Text", otext);
                    
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write responses
        /// </summary>
        /// <param name="writer"></param>
        private void WriteResponses(XmlTextWriter writer)
        {
			var context = new PaginationContext();

			var responseDataList = ResponseManager.GetResponseList(ResponseTemplateId, Options.IncludeIncomplete, true, true, context, Options.StartDate, Options.EndDate);

            int rowNumber = 0;

            foreach(var responseData in responseDataList)
            {
                writer.WriteStartElement("Response");

                writer.WriteElementString("ResponseId", responseData.Id.ToString());
                writer.WriteElementString("GUID", responseData.Guid.ToString());
                writer.WriteElementString("ResponseTemplateId", ResponseTemplateId.ToString());
                writer.WriteElementString("IsComplete", (responseData.CompletionDate.HasValue).ToString());
                writer.WriteElementString("LastPageViewed", responseData.LastPageViewed.ToString());
                writer.WriteElementString("Started", responseData.Started.ToString());
                writer.WriteElementString("Ended", responseData.CompletionDate.ToString());

                if (ApplicationManager.AppSettings.LogIpAddresses)
                {
                    writer.WriteElementString("IP", responseData.RespondentIp);
                }

                writer.WriteElementString("LastEdit", responseData.LastEditDate.ToString());
                writer.WriteElementString("NetworkUser", responseData.NetworkUser);
                writer.WriteElementString("Language", responseData.ResponseLanguage);
                writer.WriteElementString("UniqueIdentifier", responseData.UserIdentifier);
                writer.WriteElementString("Deleted", false.ToString());
                writer.WriteElementString("RespondentGUID", responseData.AnonymousRespondentGuid.ToString());
                writer.WriteElementString("IsTest", responseData.IsTest.ToString());
                writer.WriteElementString("IsAnonymized", responseData.IsAnonymized.ToString()); 
                writer.WriteElementString("Invitee", responseData.Invitee);

                writer.WriteStartElement("Answers");

                DataSet respAnswerData = ResponseManager.ListResponseAnswers(responseData.Id);

                if (respAnswerData != null && respAnswerData.Tables.Count > 0)
                {

                    foreach (DataRow answerRow in respAnswerData.Tables[0].Rows)
                    {
                        writer.WriteStartElement("Answer");

                        writer.WriteElementString("AnswerId", DbUtility.GetValueFromDataRow(answerRow, "AnswerId", -1).ToString());
                        writer.WriteElementString("ItemId", DbUtility.GetValueFromDataRow(answerRow, "ItemId", -1).ToString());

                        WriteCData(writer, "AnswerText", DbUtility.GetValueFromDataRow(answerRow, "AnswerText", string.Empty));

                        var optionId = DbUtility.GetValueFromDataRow(answerRow, "OptionId", (int?) null);
                        var optionIdAsString = optionId.HasValue ? optionId.ToString() : string.Empty;

                        writer.WriteElementString("OptionId", optionIdAsString);

                        var points = DbUtility.GetValueFromDataRow(answerRow, "Points", (decimal?)null);
                        var pointsIdAsString = points.HasValue ? points.ToString() : string.Empty;

                        writer.WriteElementString("Points", pointsIdAsString);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();

                // Let the Response buffer fill up before flushing it to the client
                if (rowNumber % ApplicationManager.AppSettings.ResponseDataExportChunkSize == 0)
                {
                    writer.Flush();
                }

                //Update status
                //Set initial progress
                if (Utilities.IsNotNullOrEmpty(ProgressKey))
                {
                    //Do a little math, since we want progress on this step to go
                    // from 70% to 100%.  Essentially we're doing a range compression
                    // of 0.3 
                    double currentPercent = 100 * (0.7 + (0.3 * ((double)rowNumber / responseDataList.Count)));

                    //Sanity check:  When row number = 0, current percent = 70, which
                    //                 is desired starting point
                    //               When row number = response count, current percent = 100, which
                    //                 is desired end point.  Yay.
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        new ProgressData
                        {
                            CurrentItem = (int)currentPercent,
                            Status = ProgressStatus.Running,
                            Message = TextManager.GetText("/controlText/exportManager/exportingAnswers", LanguageCode),
                            TotalItemCount = 100
                        }
                    );
                }

                writer.WriteEndElement();

                rowNumber++;
            }

            writer.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        protected void WriteCData(XmlWriter writer, string elementName, string value)
        {
            if(string.IsNullOrEmpty(elementName) || writer == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteCData(value);
            writer.WriteEndElement();
        }


        /// <summary>
        /// Write a value to the writer with property quote escaping.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tw"></param>
        protected void WriteValue(string value, TextWriter tw)
        {
            value = value.Replace("\"", "\"\"");

            if (ReplaceNewLines)
            {
                value = value.Replace(Environment.NewLine, NewLineReplacement);
            }

            value = string.Format("\"{0}\"", value);

            tw.Write(value);
        }
    }
}
