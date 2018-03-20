using System;
using System.IO;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Analytics;
using Prezza.Framework.Caching;
using ProgressProvider = Checkbox.Progress.DatabaseProvider.ProgressProvider;

namespace CheckboxWeb.Users.CsvExport
{
    public partial class ExportProgress : ReportExportPageBase
    {
        /// <summary>
        /// Get worker url
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
             return ResolveUrl("~/Users/CsvExport/DoExport.aspx?UserList=" + UserList);
        }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();

            _dlFileButton.Click += _downloadBtn_Click;
        }

        /// <summary>
        /// Start the process when page is loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            //Seed progress cache so it's not empty on first request to get progress
            ProgressKey = GenerateProgressKey();

            ProgressProvider.SetProgress(ProgressKey, new ProgressData
            {
                Status = ProgressStatus.Pending,
                CurrentItem = 0,
                TotalItemCount = 100,
                Message = WebTextManager.GetText("/controlText/analysisData/pdfExport/pending")
            }, null);

            //Base method starts progress
            base.OnPageLoad();
        }

        /// <summary>
        /// Get URL to call to start work
        /// </summary>
        /// <returns></returns>
        protected override string GetProgressProviderType()
        {
            return "'database'";
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
            if (ApplicationManager.AppSettings.WebFarm)
            {
                var cacheManager = CacheFactory.GetCacheManager();
                var exportBytes = cacheManager[ProgressKey] as byte[] ?? new byte[] { };

                Response.AddHeader("Content-Length", exportBytes.Length.ToString());
                Response.BinaryWrite(exportBytes);
            }
            else
            {
                Response.Clear();
                HttpContext.Current.Response.AddHeader("Content-Type", "text/csv");
                HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format(
                                                            "attachment; filename={0}.csv;",
                                                            FileUtilities.FixFileName("UserProfilesList", ".", string.Empty)));

                var info = new FileInfo(ExportFilePath);

                Response.AddHeader("Content-Length", info.Length.ToString());
                Response.TransmitFile(ExportFilePath);
            }

            Response.Flush();
            Response.End();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}