using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Security.Principal;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web.Analytics;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Export : ReportExportPageBase
    {
        CheckboxPrincipal _principal = null;

        protected override void OnPageInit()
        {
            Master.OkTextId = "/pageText/forms/surveys/reports/export.aspx/export";
            Master.OkClick += ExportBtn_Click;

            Master.CancelTextId = "/common/close";

            PopulateExportOptionDefaults();
        }

        /// <summary>
        /// Export!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportBtn_Click(object sender, EventArgs e)
        {
            EventHandlerWrapper(sender, e, DoExport);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateExportOptionDefaults()
        {
            _documentOrientation.Items.Add(new ListItem(TextManager.GetText("/pageText/forms/surveys/reports/export.aspx/orientationPortrait"), PdfExportOrientation.Portrait.ToString()));
            _documentOrientation.Items.Add(new ListItem(TextManager.GetText("/pageText/forms/surveys/reports/export.aspx/orientationLandscape"), PdfExportOrientation.Landscape.ToString()));
        }

        /// <summary>
        /// Do the export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoExport(object sender, EventArgs e)
        {
            try
            {
                //Store options
                ExportSettings = GetExportOptions();

                //Set file path.  This will be used by native spss export and by
                // ajax export page.
                ExportFilePath = string.Format(
                    @"{0}\{1}_{2}_{3}",
                    TempFolderPath,
                    ApplicationManager.ApplicationDataContext.Replace(":", string.Empty),
                    DateTime.Now.Ticks,
                    GetAttachmentFileName());

                //See if redirect possible then do it.
                if (DoRedirectToAjaxEnabledPage("ExportProgress.aspx"))
                {
                    return;
                }

                //No ajax, so use default method
                DoCommonExport();
            }
            catch (ThreadAbortException) { }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override CheckboxPrincipal CurrentPrincipal
        {
            get
            {
                if (_principal != null)
                    return _principal;

                if (HttpContext.Current != null)
                {
                    _principal = HttpContext.Current.User as CheckboxPrincipal;
                }

                return _principal ?? (_principal = new AnonymousRespondent(Guid.NewGuid()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PdfExportSettings GetExportOptions()
        {
            PdfExportOrientation orientation;
            if (!Enum.TryParse(_documentOrientation.SelectedValue, out orientation))
                orientation = PdfExportOrientation.Landscape;

            string encryptedAuthTicket;
            UserManager.GenerateAuthenticationTicket(CurrentPrincipal, out encryptedAuthTicket);

            PdfExportSettings result = new PdfExportSettings
            {
                Orientation = orientation,
                ProgressKey = GenerateProgressKey(),
                ApplicationPath = ApplicationManager.ApplicationPath,
                Name = ReportTemplate.Name,
                AuthTicket = encryptedAuthTicket
            };
            return result;
        }

        /// <summary>
        /// Get the name of the file to put in the attachment response header
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAttachmentFileName()
        {
            return FileUtilities.FixFileName(ReportTemplate.Name, ".", string.Empty) + "_export.pdf";
        }

        /// <summary>
        /// Check to see if Ajax enabled page is possible and if possible, set up redirect.  To avoid spurious
        /// errors, end response parameter is passed as false, so calling method should read return value and
        /// act accordingly.  Value of true means redirect was set.
        /// </summary>
        /// <returns></returns>
        private bool DoRedirectToAjaxEnabledPage(string pageName)
        {
            //Ensure output directory can be written to.
            //If directory can't be written, use default output method.
            //Check to see if file can be created. If it can, redirect to Ajaxy page to write data to file. If not, write export directly to response
            // using existing mechanism.
            bool writeToTempFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

            //Permissions check out, so store some state information and redirect to ajax page
            if (writeToTempFile)
            {
                //Redirect
                Response.Redirect(pageName + "?r=" + ReportTemplateId, false);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoCommonExport()
        {
            //Check user rights
            if (!AuthorizationProvider.Authorize(UserManager.GetCurrentPrincipal(),
                ReportTemplate, "Analysis.Run"))
            {
                throw new Exception("The provided user context does not have the necessary authorization for the requested operation.");
            }

            // inform the browser about the binary data format
            HttpContext.Current.Response.AddHeader("Content-Type", "application/pdf");

            // let the browser know how to open the PDF document, attachment or inline, and the file name
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format(
                                                    "attachment; filename={0}.pdf;",
                                                    Utilities.StripHtml(ReportTemplate.Name.Replace(" ", string.Empty))));

            //See if temp file can be used for better performance
            bool writeToTempFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

            if (writeToTempFile)
            {
                var result = PdfExportManager.ExportReport(ReportTemplateId, GetExportOptions());

                WriteTemporaryFile(ExportFilePath, result.Data);

                //Get file size
                var info = new FileInfo(ExportFilePath);

                Response.AddHeader("Content-Length", info.Length.ToString());

                Response.TransmitFile(ExportFilePath);
            }
            else
            {
                PdfExportManager.CommonReportExport(
                    Response.OutputStream,
                    ReportTemplateId,
                    GetExportOptions());
            }

            Response.Flush();
            Response.End();
        }


    }
}