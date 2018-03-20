using System;
using System.IO;
using System.Xml;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    public partial class ImportText : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();
            titleControl.Controls.Add(new HyperLink { NavigateUrl = "~/Settings/Manage.aspx", Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title") });
            titleControl.Controls.Add(new Label { Text = " - " + WebTextManager.GetText("/pageText/settings/importText.aspx/title") });
            _fileUploader.OnFileUploaded += new Checkbox.Web.UI.Controls.UFrameFileUploadControl.FileUploadedDelegate(_fileUploader_OnFileUploaded);

            RegisterClientScriptInclude("UploadHandler", ResolveUrl("~/Resources/FileUploadHandler.js"));

            Master.SetTitleControl(titleControl);

            Master.HideDialogButtons();
        }

        /// <summary>
        /// File was uploaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void _fileUploader_OnFileUploaded(object sender, Checkbox.Web.UI.Controls.UFrameFileUploadControl.UFrameFileUploadEventArgs args)
        {
            //Verify a file has been selected
            if (args.PostedFile == null || args.PostedFile.ContentLength == 0)
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/importText.aspx/selectFile"), StatusMessageType.Warning);
                return;
            }

            //Make sure this file has text for this form
            XmlDocument doc = new XmlDocument();

            try
            {
                MemoryStream ms = new MemoryStream(args.Buffer);
                doc.Load(ms);
            }
            catch (Exception ex)
            {
                Master.ShowStatusMessage(string.Format("{0}<br>Details: {1}", WebTextManager.GetText("/pageText/settings/importText.aspx/loadFileError"), ex.Message), StatusMessageType.Warning);
                return;
            }
            //Now try to import
            try
            {
                TextManager.ImportTexts(new StringReader(doc.OuterXml));
            }
            catch
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/importText.aspx/importProviderError"), StatusMessageType.Warning);
                return;
            }

            //If we get this far, report success
            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/importText.aspx/importSuccessful"), StatusMessageType.Success);
        }
    }
}
