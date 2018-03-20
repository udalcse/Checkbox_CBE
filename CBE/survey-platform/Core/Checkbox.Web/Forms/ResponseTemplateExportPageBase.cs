using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Web.Page;

namespace Checkbox.Web.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseTemplateExportPageBase : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// ID of survey template
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("mode")]
        public string PreviewMode { get; set; }


        /// <summary>
        /// Gets or sets the survey unique identifier.
        /// </summary>
        /// <value>
        /// The survey unique identifier.
        /// </value>
        [QueryParameter("surveyGuid")]
        public string SurveyGuid { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [print client PDF].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [print client PDF]; otherwise, <c>false</c>.
        /// </value>
        [QueryParameter("printClientPdf")]
        public bool PrintClientPdf { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("loc")]
        public string Locale { get; set; }

        /// <summary>
        /// Get/set current export file path
        /// </summary>
        protected string ExportFilePath
        {
            get { return GetSessionValue("ExportFilePath", false, string.Empty); }
            set { Session["ExportFilePath"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the export file.
        /// </summary>
        /// <value>
        /// The name of the export file.
        /// </value>
        protected string ExportFileName
        {
            get { return GetSessionValue("ExportFileName", false, string.Empty); }
            set { Session["ExportFileName"] = value; }
        }

        /// <summary>
        /// Get a reference to the analysis template.
        /// </summary>
        protected virtual ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null && ResponseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        protected void WriteTemporaryFile(string path, byte[] data)
        {
            FileUtilities.SaveFile(path, data);
        }

        /// <summary>
        /// Get/set export settings
        /// </summary>
        protected PdfExportSettings ExportSettings
        {
            get { return Session["PdfExportSettingsForSurvey"] as PdfExportSettings; }
            set { Session["PdfExportSettingsForSurvey"] = value; }
        }

        /// <summary>
        /// Get path to temp folder for exports
        /// </summary>
        protected virtual string TempFolderPath
        {
            get { return Server.MapPath(ApplicationManager.ApplicationRoot + "/Temp/"); }
        }

        /// <summary>
        /// Instead of generating a key, read it from the session
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            const string sessionKey = "ExportSurveyProgressKey";

            string progressKey = GetSessionValue(sessionKey, false, string.Empty);
            if (!string.IsNullOrEmpty(progressKey))
                return progressKey;

            progressKey = "ExportSurvey" + ResponseTemplateId + Session.SessionID;

            //Set progress key
            Session[sessionKey] = progressKey;

            return progressKey;
        }
    }
}
