using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// Abstract base class for data exporters (CSV, SPSS CSV, SPSS Native, etc.) for survey
    /// data.  This base class assumes the use of an analysis to load answer data, generate column
    /// headers, etc.  In the future this may not be the case and some refactoring will be done as
    /// necessary.
    /// </summary>
    public abstract class SurveyDataExporter
    {
        /// <summary>
        /// Max export columns per file when splitting files
        /// </summary>
        public const int MAX_COLUMN_COUNT_PER_FILE = 255;

        /// <summary>
        /// Backing field for analysis used by exporter
        /// </summary>
        private Analysis _analysis;

        /// <summary>
        /// Backing field for response template
        /// </summary>
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get/set options to use
        /// </summary>
        protected ExportOptions Options { get; set; }

        /// <summary>
        /// Get/set response template id
        /// </summary>
        protected int ResponseTemplateId { get; set; }

        /// <summary>
        /// Get/set progress key
        /// </summary>
        protected string ProgressKey { get; set; }

        /// <summary>
        /// Language code for export
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Get/set list of all data field names
        /// </summary>
        protected List<string> AllFieldNames { get; set; }

        /// <summary>
        /// Get/set list of response data field names
        /// </summary>
        protected List<string> ResponseFieldNames { get; set; }

        /// <summary>
        /// Get/set list of user data data field names
        /// </summary>
        protected List<string> UserFieldNames { get; set; }

        /// <summary>
        /// Get/set list of question data field names
        /// </summary>
        protected List<string> QuestionFieldNames { get; set; }

        /// <summary>
        /// Get/set whether analysis data is loaded or not.
        /// </summary>
        protected bool AnalysisDataLoaded { get; set; }

        /// <summary>
        /// List all field names for the item.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> ListAllFieldNames()
        {
            if (AllFieldNames == null || AllFieldNames.Count == 0)
            {
                PopulateFieldNames(false);
            }

            return new ReadOnlyCollection<string>(AllFieldNames);
        }

        /// <summary>
        /// Get the analysis used as the basis for the export.
        /// </summary>
        protected Analysis Analysis
        {
            get
            {
                if (_analysis == null)
                {
                    //Create analysis
                    AnalysisTemplate template = CreateAnalysisTemplate();

                    //Create analysis
                    _analysis = template.CreateAnalysis(LanguageCode, Options.IncludeIncomplete, Options.IncludeTestResponses);
                }

                return _analysis;
            }
        }

        /// <summary>
        /// Get the response template
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get 
            {
                return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId));
            }
        }

        /// <summary>
        /// Create template for the analysis to use for export.
        /// </summary>
        /// <returns>Analysis template configured with an export item suitable for type of export to create.</returns>
        protected abstract AnalysisTemplate CreateAnalysisTemplate();

        /// <summary>
        /// Initialize the exporter class.
        /// </summary>
        /// <param name="responseTemplateId">ID of response template to export data for.</param>
        /// <param name="languageCode">Language code to use for quesiton and option texts.</param>
        /// <param name="options">Additional options for the export.</param>
        /// <param name="progressKey">Key to use for tracking progress when working asynchronously.</param>
        /// <remarks>When using progress tracking, the export will only set progress to 99%.  It's up to the caller
        /// to mark the progress as complete.</remarks>
        public virtual void Initialize(int responseTemplateId, string languageCode, ExportOptions options, string progressKey)
        {
            ResponseTemplateId = responseTemplateId;
            Options = options;
            ProgressKey = progressKey;
            LanguageCode = languageCode;
        }

        /// <summary>
        /// Write the export data to the specified file path.
        /// </summary>
        /// <param name="filePath">File path to write export to.</param>
        public virtual void WriteToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Options.OutputEncoding))
            {
                try
                {
                    WriteToTextWriter(writer);
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
        public virtual void WriteToTextWriter(TextWriter writer)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        TotalItemCount = 100,
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

            //Write export
            WriteExportData(writer);

            //Set completed progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                //Set progress to 99% so caller can finally
                // mark progress complete.
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 99,
                        Status = ProgressStatus.Pending,
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
        protected abstract void WriteExportData(TextWriter writer);

        /// <summary>
        /// Populate names of fields
        /// </summary>
        /// <param name="forceColumnReload">Force reload of column names.</param>
        protected virtual void PopulateFieldNames(bool forceColumnReload)
        {
            ResponseFieldNames = ListAllResponseFieldNames();
            UserFieldNames = ListAllUserFieldNames();

            if (forceColumnReload)
            {
                foreach (Item item in Analysis.Items)
                {
                    if (item is ExportItem)
                    {
                        ((ExportItem)item).ClearColumns();
                    }
                }
            }

            QuestionFieldNames = ListAllQuestionFieldNames();

            AllFieldNames = new List<string>();
            AllFieldNames.AddRange(ListAllResponseFieldNames());
            if (Options.IncludeDetailedUserInfo)
            {
                AllFieldNames.AddRange(ListAllUserFieldNames());
            }
            AllFieldNames.AddRange(ListAllQuestionFieldNames());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        protected IEnumerable<ResponseProperties> GetIncompleteResponses(int responseTemplateId, DateTime? startDate, DateTime? endDate)
        {
            IList<ResponseProperties> result = new List<ResponseProperties>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetIncompleteResponseData");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateId);
            command.AddInParameter("StartDate", DbType.DateTime, startDate);
            command.AddInParameter("EndDate", DbType.DateTime, endDate);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        ResponseProperties properties = new ResponseProperties();

                        properties["ResponseID"] = DbUtility.GetValueFromDataReader(reader, "ResponseID", default(long));
                        properties["IP"] = DbUtility.GetValueFromDataReader(reader, "IP", string.Empty);
                        properties["NetworkUser"] = DbUtility.GetValueFromDataReader(reader, "NetworkUser", string.Empty);
                        properties["Language"] = DbUtility.GetValueFromDataReader(reader, "Language", string.Empty);
                        properties["IsTest"] = DbUtility.GetValueFromDataReader(reader, "IsTest", false);
                        properties["Invitee"] = DbUtility.GetValueFromDataReader(reader, "Invitee", string.Empty);
                        properties["SessionId"] = DbUtility.GetValueFromDataReader(reader, "SessionId", default(Guid?));
                        properties["Started"] = DbUtility.GetValueFromDataReader(reader, "Started", default(DateTime?));
                        properties["Ended"] = DbUtility.GetValueFromDataReader(reader, "Ended", default(DateTime?));
                        properties["LastEdit"] = DbUtility.GetValueFromDataReader(reader, "LastEdit", default(DateTime?));
                        properties["UniqueIdentifier"] = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);
                        properties["ResponseGuid"] = DbUtility.GetValueFromDataReader(reader, "GUID", default(Guid?));

                        result.Add(properties);
                    }
                }
                finally
                {
                    //Close the reader and rethrow the exception
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Get list of all export fields that are included when "Include Detailed
        /// Response Information" option is used.
        /// </summary>
        /// <returns>List of response field names.</returns>
        protected virtual List<string> ListAllResponseFieldNames()
        {
            var headers = new List<string>();

            if (Options.IncludeResponseId
                || (Options.FileSet.HasValue && Options.FileSet > 0))
            {
                headers.Add("ResponseID");
            }

            if (Options.IncludeDetailedResponseInfo)
            {
                headers.Add("ResponseGuid");
                headers.Add("Started");
                headers.Add("Ended");
                headers.Add("TotalTime");
                headers.Add("LastEdit");

                if (ApplicationManager.AppSettings.LogIpAddresses)
                {
                    headers.Add("IP");
                }

                headers.Add("Language");
                headers.Add("UniqueIdentifier");
                headers.Add("Invitee");

                if (ApplicationManager.AppSettings.LogNetworkUser)
                {
                    headers.Add("NetworkUser");
                }
            }

            if (Options.IncludeScore)
            {
                if (Options.IncludeDetailedScoreInfo)
                {
                    for (int i = 1; i < ResponseTemplate.PageCount - 1; i++)
                    {
                        headers.Add("Score_Page_" + i);
                    
                        if (Options.IncludePossibleScore)
                            headers.Add("Possible_Score_Page_" + i);
                    }
                    headers.Add("Score_Total");

                    if (Options.IncludePossibleScore)
                        headers.Add("Possible_Score_Total");
                }
                else 
                    headers.Add("Score");
            }

            return headers;
        }

        /// <summary>
        /// List all user field names to include when "Include Detailed User
        /// Info" options is enabled.
        /// </summary>
        /// <returns>List of user fields to include.</returns>
        protected virtual List<string> ListAllUserFieldNames()
        {
            List<string> fields = new List<string>();

            fields.Add(TextManager.GetText("/pageText/forms/surveys/responses/export.aspx/email"));
            fields.AddRange(ProfileManager.ListPropertyNames());

            return fields;
        }

        /// <summary>
        /// List all answer field names to include.  Specific list and values are
        /// affected by whether hidden items and open-ended items are included, whether
        /// checkbox answers are merged, aliases are used, etc.
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> ListAllQuestionFieldNames()
        {
            foreach (Item item in Analysis.Items)
            {
                if (item is ExportItem)
                {
                    return ((ExportItem)item).GetColumnNames(ProgressKey, LanguageCode, 25, 50, null , null);
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Format a specific response property field value for the export.
        /// </summary>
        /// <param name="headerValue">Response property to format.</param>
        /// <param name="responseProperties">Collection of response property values for a response.</param>
        /// <remarks>Checks for DBNull values and calculates TotalTime field.</remarks>
        protected static string FormatResponseData(string headerValue, ResponseProperties responseProperties)
        {
            if (headerValue.Equals("TotalTime", StringComparison.InvariantCultureIgnoreCase))
            {
                if (responseProperties["Started"] != null && responseProperties["Ended"] != null)
                {
                    var start = (DateTime)responseProperties["Started"];
                    var end = (DateTime)responseProperties["Ended"];
                    TimeSpan time = end.Subtract(start);

                    return time.ToString();
                }
            }
            else
            {
                object value = responseProperties[headerValue];

                if (value != null &&
                    value != DBNull.Value)
                {
                    return value.ToString();
                }

                return string.Empty;
            }

            //Default value
            return string.Empty;
        }
    }
}
