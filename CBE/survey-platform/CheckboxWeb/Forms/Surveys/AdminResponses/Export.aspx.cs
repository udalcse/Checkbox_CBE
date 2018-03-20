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
using Checkbox.Web;
using Checkbox.Web.Analytics;
using static System.Web.HttpContext;

namespace CheckboxWeb.Forms.Surveys.AdminResponses
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
                ExportSettings = GetBulkExportOptions();

                ExportFilePath = string.Format(
                   @"{0}\{1}_{2}_{3}",
                   TempFolderPath,
                   ApplicationManager.ApplicationDataContext.Replace(":", string.Empty),
                   DateTime.Now.Ticks,
                   GetBulkAttachmentFileName());


                //See if redirect possible then do it.
                if (DoRedirectToAjaxEnabledPage("ExportProgress.aspx"))
                {
                    return;
                }
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

        private PdfExportSettings GetBulkExportOptions()
        {
            PdfExportOrientation orientation;
            if (!Enum.TryParse(_documentOrientation.SelectedValue, out orientation))
                orientation = PdfExportOrientation.Landscape;

            PdfExportSettings result = new PdfExportSettings
            {
                Orientation = orientation,
                ProgressKey = GenerateProgressKey(),
                ApplicationPath = ApplicationManager.ApplicationPath,
                Name = BulkPDF.ToString(),
                AuthenticationCookie = Current.Request.Cookies.Get(".ASPXAUTH").Value,
                Session = Current.Request.Cookies.Get("ASP.NET_SessionId").Value
            };
            return result;
        }

        /// <summary>
        /// Get the name of the file to put in the attachment response header
        /// </summary>
        /// <returns></returns>
        protected virtual string GetBulkAttachmentFileName()
        {
            return FileUtilities.FixFileName(BulkPDF.ToString(), ".", string.Empty) + "_export.zip";
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
                Response.Redirect(pageName + "?bulkPDF=" + BulkPDF.ToString(), false);

                return true;
            }

            return false;
        }
    }
}