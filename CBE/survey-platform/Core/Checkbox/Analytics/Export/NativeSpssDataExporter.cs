using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Progress;
using Spss;
using System.Text.RegularExpressions;
using Checkbox.Forms.Items;
using Checkbox.Analytics.Items;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// Exporter for writing survey results data to native SPSS format.
    /// </summary>
    public class NativeSpssDataExporter : SpssCompatibleCsvDataExporter
    {
        private DataTable _resultTable;
        private List<long> _responseIds;

        /// <summary>
        /// Write SPSS data to file.
        /// </summary>
        /// <param name="filePath">File to write SPSS data to.</param>
        public override void WriteToFile(string filePath)
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

            //Build a datatable to pass to convert method, which uses schema to generate file.
            QuestionFieldNames = ListAllQuestionFieldNames();

            _resultTable = new DataTable();

            if (Options.IncludeResponseId)
            {
                _resultTable.Columns.Add("ResponseID", typeof(Int64));
            }
            else
            {
                _resultTable.Columns.Add("RowNumber", typeof(int));
            }

            foreach (string t in QuestionFieldNames)
            {
                //Set initial column type
                Type columnType = typeof(int);

                //When including open-ended results, there may be non-numeric values, so figure out
                // if that is the case.
                if (Options.IncludeOpenEnded)
                {
                    //Find out type, but use knowledge that select many questions
                    // have underscores in names to prevent unnecessary item loading
                    if (!t.Contains("_"))
                    {
                        int questionItemId = GetItemIdForField(t);

                        if (questionItemId > 0)
                        {
                            ItemData itemData = ItemConfigurationManager.GetConfigurationData(questionItemId);

                            if (itemData != null && !(itemData is SelectItemData))
                            {
                                //If item is not a select item, set column type to string
                                columnType = typeof(string);
                            }
                        }
                    }
                }

                //prepare column name to meet SPSS standarts
                var columnName = Regex.Replace(t, @"[^a-zA-Z0-9]", "_");
                if (columnName.Length > 8)
                    columnName = columnName.Remove(8);

                _resultTable.Columns.Add(columnName, columnType);
            }

            //Populate response answers
            _responseIds = Analysis.Data.ListResponseIds();

            if (Options.IncludeIncomplete && Options.IncludeResponseId)
            {
                //List response ids to export
                var incompleteResponses = GetIncompleteResponses(ResponseTemplateId, Analysis.MinResponseDate, Analysis.MaxResponseDate);
                //add incomplete rersponses
                foreach (var response in incompleteResponses)
                {
                    if (!_responseIds.Contains((long)response["ResponseId"]))
                        _responseIds.Add((long)response["ResponseId"]);
                }
            }

            //Call convert, and pass delegates to get metadata and to get next data row
            SpssConvert.ToFile(
                _resultTable,
                filePath,
                DataRowCallback,
                MetaDataCallback);

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
        /// Callback for getting a specific row of results.
        /// </summary>
        /// <param name="rowNumber">Row of results to list data for.</param>
        /// <returns>Answers for a specific row of the export.</returns>
        private object[] DataRowCallback(int rowNumber)
        {
            int responseIndex = rowNumber - 1;

            if (_responseIds == null || responseIndex >= _responseIds.Count)
            {
                return null;
            }

            //Get answers to questions
            List<string> tempAnswers = GetAnswerData(QuestionFieldNames.Count, _responseIds[responseIndex], Analysis, false, ResponseTemplate);

            //Get output array to contain massaged data that includes extra field and dbnulls when 
            // appropriate.
            var returnAnswers = new List<object>(tempAnswers.Count + 1);

            //Add response id, if necessary
            returnAnswers.Add(Options.IncludeResponseId ? _responseIds[responseIndex] : rowNumber);


            //Now replace any empty strings with DBNull, which is expected by SPSS for int columns
            //Because of manually added response id / row number field, table has one more
            // column then returned temp answers list has items, so skip first column of table.
            for (int i = 1; i < _resultTable.Columns.Count; i++)
            {
                //Get value to add, checking for null
                object valueToAdd = Utilities.IsNotNullOrEmpty(tempAnswers[i - 1])
                    ? tempAnswers[i - 1]
                    : string.Empty;

                //If expecting an int, ensure value is not null and can be parsed as a number, which is
                // what spss will do later
                int dummyResult;

                if (_resultTable.Columns[i].DataType == typeof(int)
                    && !int.TryParse(valueToAdd.ToString(), out dummyResult))
                {
                    valueToAdd = DBNull.Value;
                }
                else
                {
                    valueToAdd = Utilities.StripHtml(valueToAdd.ToString(), 255);
                }

                returnAnswers.Add(valueToAdd);
            }

            //Update status
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey) && rowNumber % 25 == 0)
            {
                //Do a little math, since we want progress on this step to go
                // from 70% to 100%.  Essentially we're doing a range compression
                // of 0.3 
                double currentPercent = 100 * (0.7 + (0.3 * ((double)rowNumber / _responseIds.Count)));

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

            return returnAnswers.ToArray();
        }

        /// <summary>
        /// Callback for populating metadata for a specific question or options.
        /// </summary>
        /// <param name="metaData"></param>
        private void MetaDataCallback(VarMetaData metaData)
        {
            //Set label to be question text, 40 char max in SPSS?
            metaData.Label = Utilities.StripHtml(GetActualTextForField(metaData.Name), SpssThinWrapper.SPSS_MAX_VARLABEL);


            if (metaData.HasValueLabels)
            {
                List<string> labels = GetOptionTextsForField(metaData.Name);

                for (int i = 1; i <= labels.Count; i++)
                {
                    metaData[i] = labels[i - 1];
                }
            }
        }

        /// <summary>
        /// Get actual text name for field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private List<string> GetOptionTextsForField(string fieldName)
        {
            var texts = new List<string>();

            //Avoid select many items, which have underscores in the name
            if (fieldName.Contains("_"))
            {
                return texts;
            }

            int itemId = GetItemIdForField(fieldName);

            if (itemId > 0)
            {
                var itemData = SurveyMetaDataProxy.GetItemData(itemId, false);

                if (itemData.Options.Count > 0)
                {
                    texts.AddRange(itemData.Options.Select(optionId => Utilities.StripHtml(SurveyMetaDataProxy.GetOptionText(itemId, optionId, LanguageCode, false, false), SpssThinWrapper.SPSS_MAX_VALLABEL)));
                }
            }

            return texts;
        }

        /// <summary>
        /// List all answer field names to include.  Specific list and values are
        /// affected by whether hidden items and open-ended items are included, whether
        /// checkbox answers are merged, aliases are used, etc.
        /// </summary>
        /// <returns></returns>
        protected override List<string> ListAllQuestionFieldNames()
        {
            foreach (Item item in Analysis.Items)
            {
                if (item is ExportItem)
                {
                    return ((ExportItem)item).GetColumnNames(ProgressKey, LanguageCode, 25, 50);
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Write export data to the specified writer.  Not supported for SPSS
        /// native format which writes directly to file as required by the underlying
        /// COM component supplied by SPSS.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteExportData(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}