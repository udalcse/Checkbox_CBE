using System;
using System.IO;

using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;

using Prezza.Framework.Caching;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    /// <summary>
    /// Show progress and download links
    /// </summary>
    public partial class ExportProgress : ExportResultsPage
    {
        [QueryParameter]
        public bool ZipParts { get; set; }

        /// <summary>
        /// Get worker url
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
            return ResolveUrl("~/Forms/Surveys/Responses/DoExport.aspx");
        }

        /// <summary>
        /// Get worker url params
        /// </summary>
        /// <returns></returns>
        protected override object GetWorkerUrlData()
        {
            return new {ZipParts = ZipParts, s = ResponseTemplate.ID};
        }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();

            _dlFileButton.Click += _downloadBtn_Click;

            _backToExportLink.NavigateUrl = "Export.aspx?s=" + ResponseTemplate.ID;
        }

        /// <summary>
        /// Start the process when page is loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            //Seed progress cache so it's not empty on first request to get progress
            ProgressKey = GenerateProgressKey();

            //Ensure appropriate links will be visible
            _s3ExportDiv.Visible = UploadItemManager.UseS3ForTempFiles;
            _directDlDiv.Visible = !_s3ExportDiv.Visible;

            SetProgressStatus(
                ProgressStatus.Pending,
                0,
                100,
                WebTextManager.GetText("/pageText/doExport.aspx/exportPending"));

            //Base method starts progress
            base.OnPageLoad();
       
        }


        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _downloadBtn_Click(object sender, EventArgs e)
        {
            EventHandlerWrapper(sender, e, DownloadFile);
        }

        /// <summary>
        /// Do work to download file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFile(object sender, EventArgs e)
        {
            if (ZipParts)
            {
                DownloadZipFromDisk(
                    GetSplitZipArchivePath(Session.SessionID),
                    GetAttachmentFileName() + ".zip");
            }
            else
            {
                PrepareResponse();
                if (ApplicationManager.AppSettings.WebFarm)
                {
                    var cacheManager = CacheFactory.GetCacheManager();
                    var exportBytes = cacheManager[ProgressKey] as byte[] ?? new byte[] { };

                    Response.AddHeader("Content-Length", exportBytes.Length.ToString());

                    Response.BinaryWrite(exportBytes);
                }
                else
                {
                    //Get file size
                    var info = new FileInfo(ExportFilePath);

                    Response.AddHeader("Content-Length", info.Length.ToString());

                    Response.TransmitFile(ExportFilePath);
                }

                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest(); 
            }
        }
    }
}
