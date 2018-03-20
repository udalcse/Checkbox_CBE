using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Forms;
using static System.Web.HttpContext;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class Export : ResponseTemplateExportPageBase
    {
        protected override void OnPageInit()
        {
            Master.OkTextId = "/pageText/forms/surveys/export.aspx/export";
            Master.OkClick += ExportBtn_Click;

            Master.CancelTextId = "/common/close";

            PopulateExportOptionDefaults();

            var surveyLanguages = WebTextManager.GetSurveyLanguagesDictionary();

            foreach (var languageCode in ResponseTemplate.LanguageSettings.SupportedLanguages)
            {
                var localizedLanguageName = surveyLanguages.ContainsKey(languageCode)
                    ? surveyLanguages[languageCode]
                    : languageCode;

                _languageList.Items.Add(new ListItem(
                    localizedLanguageName,
                    languageCode));
            }

            if (_languageList.Items.FindByValue(Locale) != null)
                _languageList.SelectedValue = Locale;
            else if (_languageList.Items.FindByValue(ResponseTemplate.LanguageSettings.DefaultLanguage) != null)
                _languageList.SelectedValue = ResponseTemplate.LanguageSettings.DefaultLanguage;

            //Check for multiLanguage Support
            string errorMsg;
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                if (_languageList.Items.FindByValue(TextManager.DefaultLanguage) != null)
                {
                    _languageList.SelectedValue = TextManager.DefaultLanguage;
                }
                _languageList.Enabled = false;
            }

            _languageListPanel.Visible = _languageList.Items.Count > 1;
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
            _documentOrientation.Items.Add(new ListItem(TextManager.GetText("/pageText/forms/surveys/export.aspx/orientationPortrait"), PdfExportOrientation.Portrait.ToString()));
            _documentOrientation.Items.Add(new ListItem(TextManager.GetText("/pageText/forms/surveys/export.aspx/orientationLandscape"), PdfExportOrientation.Landscape.ToString()));
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


                ExportFileName = $"_{DateTime.Now.Ticks}_{GetAttachmentFileName()}";

                //Set file path.  This will be used by native spss export and by
                // ajax export page.
                ExportFilePath = $@"{TempFolderPath}\{ApplicationManager.ApplicationDataContext}{ExportFileName}";

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
        /// <returns></returns>
        private PdfExportSettings GetExportOptions()
        {
            PdfExportOrientation orientation;
            if (!Enum.TryParse(_documentOrientation.SelectedValue, out orientation))
                orientation = PdfExportOrientation.Landscape;

            string language = !string.IsNullOrEmpty(_languageList.SelectedValue)
                ? _languageList.SelectedValue
                : Locale;

            PdfExportSettings result = new PdfExportSettings
            {
                Orientation = orientation,
                Language = language,
                ApplicationPath = ApplicationManager.ApplicationPath,
                Name = ResponseTemplate.Name,
                SurveyTemplateGUID = this.SurveyGuid
            };

            if (this.PrintClientPdf)
            {
                var sessionCookie = Current.Request.Cookies.Get("ASP.NET_SessionId");
                var authentificationCookie = Request.Cookies.Get(".ASPXAUTH");
                //var responseCookie = Request.Cookies.Get("Response");
                result.PrintClientPdf = true;
                result.InvitationId = Request.QueryString["i"];
                result.ResumeId = Request.QueryString["iid"];
                result.AuthenticationCookie = authentificationCookie?.Value;
                result.Session = sessionCookie?.Value;
            } 

            return result;
        }

        /// <summary>
        /// Get the name of the file to put in the attachment response header
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAttachmentFileName()
        {
            return FileUtilities.FixFileName(ResponseTemplate.Name, ".", string.Empty) + "_export.pdf";
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
                Response.Redirect(pageName + "?s=" + ResponseTemplateId + "&mode=" + PreviewMode, false);

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
                ResponseTemplate, "Form.Edit"))
            {
                throw new Exception("The provided user context does not have the necessary authorization for the requested operation.");
            }

            // inform the browser about the binary data format
            Current.Response.AddHeader("Content-Type", "application/pdf");

            // let the browser know how to open the PDF document, attachment or inline, and the file name
            Current.Response.AddHeader("Content-Disposition", string.Format(
                                                    "attachment; filename={0}.pdf;",
                                                    Utilities.StripHtml(ResponseTemplate.Name.Replace(" ", string.Empty))));

            //See if temp file can be used for better performance
            bool writeToTempFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

            if (writeToTempFile)
            {
                var result = PdfExportManager.ExportSurvey(ResponseTemplateId, GetExportOptions());

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
                    ResponseTemplateId,
                    GetExportOptions());
            }

            Response.Flush();
            Response.End();
        }


    }
}