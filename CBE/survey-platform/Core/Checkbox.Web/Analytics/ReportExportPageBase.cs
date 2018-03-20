using System;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Web.Page;

namespace Checkbox.Web.Analytics
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportExportPageBase : SecuredPage
    {
        private AnalysisTemplate _analysisTemplate;

        /// <summary>
        /// ID of report template
        /// </summary>
        [QueryParameter("r")]
        public int ReportTemplateId { get; set; }

        /// <summary>
        /// Guid of selected responses which exists in Session
        /// </summary>
        [QueryParameter]
        public Guid BulkPDF { get; set; }

        /// <summary>
        /// Guid of selected users from User list
        /// </summary>
        [QueryParameter]
        public Guid UserList { get; set; }

        /// <summary>
        /// Check if Bulk export should run
        /// </summary>
        public bool IsBulkExport
        {
            get { return BulkPDF != null && BulkPDF != Guid.Empty; }
        }
        /// <summary>
        /// Get/set current export file path
        /// </summary>
        protected string ExportFilePath
        {
            get { return GetSessionValue("ExportFilePath", false, string.Empty); }
            set { Session["ExportFilePath"] = value; }
        }

        /// <summary>
        /// Get a reference to the analysis template.
        /// </summary>
        protected virtual AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_analysisTemplate == null && ReportTemplateId > 0)
                {
                    _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportTemplateId);
                }

                return _analysisTemplate;
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
            get { return Session["PdfExportSettings"] as PdfExportSettings; }
            set { Session["PdfExportSettings"] = value; }
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
            const string sessionKey = "ExportReportProgressKey";

            string progressKey = GetSessionValue(sessionKey, false, string.Empty);
            if (!string.IsNullOrEmpty(progressKey))
                return progressKey;

            if (IsBulkExport)
            {
                progressKey = "ExportReport" + BulkPDF.ToString() + Session.SessionID;
            }
            else
            {
                progressKey = "ExportReport" + ReportTemplateId + Session.SessionID;
            }

            //Set progress key
            Session[sessionKey] = progressKey;

            return progressKey;
        }
    }
}
