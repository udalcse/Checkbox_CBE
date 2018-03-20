using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Users;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// Survey data exporter that writes results data to a CSV file.
    /// </summary>
    public class CsvDataExporter : SurveyDataExporter
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
        /// Get/set chunk size (# line between buffer flushes) for export.
        /// </summary>
        protected int ChunkSize { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CsvDataExporter()
        {
            ReplaceNewLines = ApplicationManager.AppSettings.ReplaceNewLine;
            NewLineReplacement = ApplicationManager.AppSettings.NewLineReplacement;
            ChunkSize = ApplicationManager.AppSettings.ResponseDataExportChunkSize;
        }

        /// <summary>
        /// Create the analysis template to use for the exporter
        /// </summary>
        /// <returns></returns>
        protected override AnalysisTemplate CreateAnalysisTemplate()
        {
            return AnalysisTemplateManager.GenerateCSVExportTemplate(ResponseTemplate, Options);
        }

        /// <summary>
        /// Populate field names.
        /// </summary>
        /// <remarks>This override figures out which columns to include when splitting the
        /// export across files.</remarks>
        protected override void PopulateFieldNames(bool forceQuestionReload)
        {
            base.PopulateFieldNames(forceQuestionReload);

            CalculateStartAndEndIndices();
        }

        /// <summary>
        /// Calculate indices of first and last fields for writing data.
        /// </summary>
        protected virtual void CalculateStartAndEndIndices()
        {
            //Set initial values
            StartFieldIndex = 0;
            EndFieldIndex = AllFieldNames.Count - 1;

            //If splitting a file, determine what field names to include, otherwise list
            //  all fields.
            if (Options.FileSet.HasValue
                && Options.FileSet > 0)
            {
                int fileNumber = Options.FileSet.Value;

                //Figure out first column to display
                int numberOfColumnsToDisplay = MAX_COLUMN_COUNT_PER_FILE;

                //Step 2a: If this is the first page of results, the response id will be
                // added to the first column and this additional column IS NOT in the headers list for subsequent pages
                // so we need to adjust the first column accordingly.  To adjust the column, we
                // need to account for the fact that for the 2nd and subsequent pages, only 
                // MAX_COLUMN_COUNT_PER_FILE - 1 columns are displayed.  That makes the 
                // total number of columns (excluding the added response id) added in the
                // previous fileNumber pages equal to:
                // x * (MAX_COLUMN_COUNT_PER_FILE) - (x - 1) where
                // x = the number of PREVIOUS pages (i.e. fileNumber - 1) and x > 0
                // So total number of columns used from headers array for the PREVIOUS
                // x pages is...
                // x = 1 --> MAX_COLUMN_COUNT_PER_FILE of data from headers list
                // x = 2 --> 2 * MAX_COLUMN_COUNT_PER_FILE - 1
                // x = 3 --> 3 * MAX_COLUMN_COUNT_PER_FILE - 2
                // x = 4 --> 4 * MAX_COLUMN_COUNT_PER_FILE - 3
                // etc.

                //So, we need to adjust the first column to pick from the headers and answers
                // lists according to number of columns from previous (fileNumber - 1) pages.
                // The total number of columns must also be revised down for the 3rd and subsequent pages.
                if (fileNumber > 1)
                {
                    numberOfColumnsToDisplay -= 1;          //Subtract 1 since we manually add response id as first column on 2nd and subsequent files.
                }

                //Columns are 0-based indexes and fileNumbers are 1-based, so account 
                // for that by subtracting 1 from file number to find first column.
                int firstColumn = (fileNumber - 1) * MAX_COLUMN_COUNT_PER_FILE;

                //Now, adjust for issue mentioned in gigantic comment above.  We only need to do this
                // for the 3rd and subsequent pages, because the 1st page is expected to have a response 
                // id and that is in the headers list so the firstColumn calculated for the second page
                // will be correct using the above calculation.  For others, we have to account for the
                // bonus response id column added by subtracting one column for each page after the 
                // 2nd.
                // i.e. for the Nth page of the export, there are N-1 extra ResponseId columns added
                //Account for the fact for manually added columns.
                if (fileNumber > 2)
                {
                    firstColumn -= (fileNumber - 2);
                }

                //Now calculate the last column based on the first column and the total number of columns
                // to display.  Don't forget to subtract one due to 0-based indexing.
                int lastColumn = firstColumn + numberOfColumnsToDisplay - 1;

                ////Revise last column further downward in case last page of results does not have full number
                //// of columns.
                if (lastColumn >= AllFieldNames.Count)
                {
                    lastColumn = AllFieldNames.Count - 1;
                }

                //Store values
                StartFieldIndex = firstColumn;
                EndFieldIndex = lastColumn;
            }
        }

        /// <summary>
        /// Write exported data to the specified text writer
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteExportData(TextWriter writer)
        {
            //Write headers
            WriteHeaders(writer);

            //Write newline
            writer.Write(Environment.NewLine);

            //Write responses
            WriteResponses(writer);
        }

        /// <summary>
        /// Write headers to the writer
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteHeaders(TextWriter writer)
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

            //If splitting file, add response id header in addition to specfied range.
            if (Options.FileSet.HasValue && Options.FileSet > 1)
            {
                WriteValue("ResponseId", writer);
                writer.Write(",");
            }

            for (int fieldIndex = StartFieldIndex; fieldIndex <= EndFieldIndex; fieldIndex++)
            {
                string header = Utilities.DecodeAndStripHtml(AllFieldNames[fieldIndex]);

                if (!string.IsNullOrEmpty(header))
                    header = Regex.Replace(header, "(\n|\r|\r\n)", " ");

                //Strip HTML from headers
                WriteValue(
                    Utilities.DecodeAndStripHtml(header),
                    writer);

                if (fieldIndex < EndFieldIndex)
                {
                    writer.Write(",");
                }
            }
        }

        /// <summary>
        /// Write responses
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteResponses(TextWriter writer)
        {
            //Create a list to store all columns.  We will then selectively
            // write based on start/end index
            var allColumnData = new List<string>();

            List<long> responseIds = Analysis.Data.ListResponseIds();

            IEnumerable<ResponseProperties> incompleteResponses = new List<ResponseProperties>();

            if (Options.IncludeIncomplete)
            {
                //List response ids to export
                incompleteResponses = GetIncompleteResponses(ResponseTemplateId, Analysis.MinResponseDate, Analysis.MaxResponseDate);
                //add incomplete rersponses
                foreach (var response in incompleteResponses)
                {
                    if (!responseIds.Contains((long) response["ResponseId"]))
                        responseIds.Add((long) response["ResponseId"]);
                }
            }
            
            int numRows = responseIds.Count;

            var trackingText = TextManager.GetText("/controlText/exportManager/exportingAnswers", LanguageCode);

            //Create a fake response. Then we can load answers here and run its rules
            //to calculate possible score counting excluded items 
            var r = ResponseTemplate.CreateResponse(LanguageCode, Guid.NewGuid());

            //find all IScored items and store their ids
            var answerableItemsByPage = new Dictionary<int, List<Item>>();
            var allAnswerableItemIds = new List<Item>();
            foreach (var page in r.GetResponsePages())
            {
                if (page.PageType == TemplatePageType.ContentPage)
                {
                    answerableItemsByPage[page.Position - 1] = page.Items.Where(i => i is IScored).ToList();
                    allAnswerableItemIds.AddRange(answerableItemsByPage[page.Position - 1]);
                }
            }

            for (int rowNumber = 0; rowNumber < numRows; rowNumber++)
            {
                long responseId = responseIds[rowNumber];
                var responseProperties = Analysis.Data.GetResponseProperties(responseId) ??
                    incompleteResponses.FirstOrDefault(ir => (long) ir["ResponseId"] == responseId);

                //If scoring, manually get the score
                if (Options.IncludeScore)
                {
                    if (Options.IncludeDetailedScoreInfo)
                    {
                        if (Options.IncludePossibleScore)
                        {
                            var detailedScoreData = Analysis.Data.CalculateResponseDetailedScoreData(r, responseId, ResponseTemplate);
                            if (detailedScoreData != null)
                            {
                                foreach (var pageScoreData in detailedScoreData.PageScores)
                                {
                                    responseProperties["Score_Page_" + pageScoreData.Position] = pageScoreData.CurrentScore;
                                    responseProperties["Possible_Score_Page_" + pageScoreData.Position] = pageScoreData.MaxPossibleScore;
                                }

                                responseProperties["Score_Total"] = detailedScoreData.CurrentSurveyScore;
                                responseProperties["Possible_Score_Total"] = detailedScoreData.PossibleSurveyMaxScore;
                            }
                        }
                        else
                        {
                            foreach (var page in answerableItemsByPage)
                            {
                                responseProperties["Score_Page_" + page.Key] = Analysis.Data.CalculateResponseScore(responseId, page.Value);
                            }
                            responseProperties["Score"] = Analysis.Data.CalculateResponseScore(responseId, allAnswerableItemIds);
                        }
                    }
                    else
                        responseProperties["Score"] = Analysis.Data.CalculateResponseScore(responseId, allAnswerableItemIds);
                }

                allColumnData.AddRange(GetResponseData(ResponseFieldNames, responseProperties));
                
                if (Options.IncludeDetailedUserInfo)
                {
                    allColumnData.AddRange(GetUserData(UserFieldNames, (string)responseProperties["UniqueIdentifier"]));
                }
                var answers = GetAnswerData(QuestionFieldNames.Count, responseId, Analysis, Options.StripHtmlTags, ResponseTemplate);
                allColumnData.AddRange(answers);

                //Ensure that ResponseID is the first column of all sub files.  Since it exists as a
                // header, no need to manually insert it into first file.
                if (Options.FileSet.HasValue && Options.FileSet.Value > 1)
                {
                    WriteValue(allColumnData[0], writer);
                    writer.Write(",");
                }

                for (int i = StartFieldIndex; i <= EndFieldIndex; i++)
                {
                    WriteValue(allColumnData[i], writer);
                    
                    if (i < EndFieldIndex)
                    {
                        writer.Write(",");
                    }
                }

                writer.Write(Environment.NewLine);
                allColumnData.Clear();

                // Let the Response buffer fill up before flushing it to the client
                if (rowNumber % ChunkSize == 0)
                {
                    writer.Flush();
                }

                 //Update status -- Only update every 25 row
                if (Utilities.IsNotNullOrEmpty(ProgressKey) && rowNumber % 25 == 0)
                {
                    ProgressProvider.SetProgressCounter(
                        ProgressKey,
                        trackingText,
                        rowNumber,
                        numRows,
                        30,
                        100);
                }
            }

            writer.Flush();
        }

        /// <summary>
        /// Get a list of response property values converted to string format.
        /// </summary>
        /// <param name="responseHeaders">List of response property headers for the export.</param>
        /// <param name="props">Response property values for a response.</param>
        /// <returns>List of property values.</returns>
        protected static List<string> GetResponseData(IList<string> responseHeaders, ResponseProperties props)
        {
            return responseHeaders.Select(t => FormatResponseData(t, props)).ToList();
        }

        /// <summary>
        /// List user field values for a response.
        /// </summary>
        /// <param name="profileHeaders">List of user fields.</param>
        /// <param name="uniqueIdentifier">Unique identifier of user that completed the response.</param>
        public static List<string> GetUserData(IList<string> profileHeaders, string uniqueIdentifier)
        {
            var data = new List<string>();

            if (profileHeaders.Count == 0)
            {
                return data;
            }

            List<ProfileProperty> profile = null;

            //Attempt to get the unique identifier
            if (Utilities.IsNotNullOrEmpty(uniqueIdentifier))
            {
                profile = ProfileManager.GetProfileProperties(uniqueIdentifier, true, false);

                string emailKey = TextManager.GetText("/pageText/forms/surveys/responses/export.aspx/email");
                string emailValue = UserManager.GetUserEmail(uniqueIdentifier);

                if (!string.IsNullOrEmpty(emailKey) && !string.IsNullOrEmpty(emailValue))
                {
                    if (!profile.Any(item => item.Name.Equals(emailKey)))
                        profile.Add(new ProfileProperty()
                        {
                            FieldType = CustomFieldType.SingleLine,
                            Name = emailKey,
                            Value = emailValue
                        });
                    else
                    {
                        profile.FirstOrDefault(item => item.Name.Equals(emailKey)).Value = emailValue;
                    }
                }
            }

            var matrixFields = profile.Where(item => item.FieldType == CustomFieldType.Matrix).ToList();

            var userGuid = UserManager.GetUserPrincipal(uniqueIdentifier).UserGuid;
            foreach (var field in matrixFields)
            {
                var matrix = ProfileManager.GetMatrixField(field.Name, userGuid);
                field.Value = matrix.ConvertMatrixToCsvState();
            }

            var radioButtonFields = profile.Where(item => item.FieldType == CustomFieldType.RadioButton).ToList();

            foreach (var field in radioButtonFields)
            {
                var radioButton = ProfileManager.GetRadioButtonField(field.Name, userGuid);
                var selectedOption = radioButton.Options.FirstOrDefault(s => s.IsSelected);
                if (selectedOption != null)
                {
                    field.Value = selectedOption.Name;
                }
                else
                {
                    field.Value = string.Empty;
                }
            }


            foreach (string t in profileHeaders)
            {
                if (profile != null && profile.Any(item => item.Name.Equals(t)))
                {
                    data.Add(profile.FirstOrDefault(item => item.Name.Equals(t)).Value);
                }
                else
                {
                    data.Add(string.Empty);
                }
            }

            return data;
        }


        /// <summary>
        /// List answer values for a specific response.
        /// </summary>
        /// <param name="expectedAnswerCount">Expected number of values to return.</param>
        /// <param name="responseId">ID of response to list answer values for.</param>
        /// <param name="analysis">Analysis containing export item object and response answer data.</param>
        /// <param name="stripHtmlTags">Are we need to strip all html tags in answers?</param>
        /// <param name="rt"></param>
        protected static List<string> GetAnswerData(int expectedAnswerCount, long responseId, Analysis analysis, bool stripHtmlTags, ResponseTemplate rt)
        {
            var responseAnswers = new List<string>();

            var item = analysis.Items.OfType<ExportItem>().FirstOrDefault();
            if (item != null)
            {
                var answers = (item).GetRowAnswers(responseId, stripHtmlTags, analysis.IncludeIncompleteResponses, analysis.IncludeTestResponses, rt);
                responseAnswers.AddRange(answers.Select(answer => Regex.Replace(answer, "(\n|\r|\r\n)", " ")));
            }

            //NOTE: Code below added as precaution only.  It was not added in response to a bug during testing, etc.
            // but since the calling code expects that the length of the answer list matches the length of the 
            // column headers list, we'll do some work to make sure the lengths match just in case

            //Ensure answers are of expected length.  This doesn't address the underlying problem which is that
            // there is a column count mismatch, but this will hopefully prevent the export from getting messed up
            if (responseAnswers.Count < expectedAnswerCount)
            {
                int numToAdd = expectedAnswerCount - responseAnswers.Count;

                for (int i = 0; i < numToAdd; i++)
                {
                    responseAnswers.Add(string.Empty);
                }
            }

            if (responseAnswers.Count > expectedAnswerCount)
            {
                int numToRemove = responseAnswers.Count - expectedAnswerCount;

                responseAnswers.RemoveRange(expectedAnswerCount, numToRemove);
            }

            return responseAnswers;
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
