using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.IO;
using System.Xml;
using Checkbox.Management;
using Checkbox.LicenseLibrary;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Edit template language settings
    /// </summary>
    public partial class Languages : ResponseTemplatePage, IStatusPage
    {
        /// <summary>
        /// Get page specific title for survey
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return WebTextManager.GetText("/pageText/surveyLanguage.aspx/import"); }
        }

        /// <summary>
        /// Initialize screen
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnPageInit()
        {
            base.OnPageInit();

            Master.SetTitle(PageSpecificTitle);

            Master.OkClick += _okBtn_Click;
            Master.OkTextId = "/common/close";
            Master.CancelVisible = false;

            _import.Click += _import_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            _extensionValidationError.Visible = false;
            _invalidXmlError.Visible = false;
            _formatValidationError.Visible = false;

            if (!ApplicationManager.AppSettings.AllowMultiLanguage)
            {
                Response.Redirect(UserDefaultRedirectUrl, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoExport()
        {

            Response.Expires = -1;
            Response.Buffer = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename="+ Utilities.StripHtml(ResponseTemplate.Name.Replace(" ", string.Empty), null) + "_TextExport.xml");
            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            TextManager.ExportFilteredTexts(Response.Output, ResponseTemplate.LanguageSettings.SupportedLanguages.ToArray(), GetTextIdsForExport().ToArray());
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetTextIdsForExport()
        {
            var textIdList = new List<string> {"/templateData/" + ResponseTemplateId};

            foreach (var pageId in ResponseTemplate.ListTemplatePageIds())
            {
                var templatePage = ResponseTemplate.GetPage(pageId);

                if (templatePage == null)
                {
                    continue;
                }

                foreach (var itemId in templatePage.ListItemIds())
                {
                    textIdList.AddRange(GetTextIdsForItemData(ResponseTemplate.GetItem(itemId)));
                }
            }

            return textIdList.Distinct(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetTextIdsForItemData(ItemData itemData)
        {
            var textIds = new List<string>();

            if (itemData == null)
            {
                return textIds;
            }


            //Item text/description
            if (!string.IsNullOrEmpty(itemData.TextIdPrefix))
            {
                textIds.Add("/" + itemData.TextIdPrefix + "/" + itemData.ID + "/");
            }
            
            //Option texts
            if (itemData is SelectItemData)
            {
                textIds.AddRange(((SelectItemData) itemData).Options.Select(itemOption => itemOption.TextID));
            }
            //Matrix children -- Row Text items and colum items
            else if (itemData is MatrixItemData)
            {
                var matrixItemData = (MatrixItemData) itemData;

                //Row text items
                for (int rowNumber = 1; rowNumber <= matrixItemData.RowCount; rowNumber++)
                {
                    var rowItemId = matrixItemData.GetItemIdAt(rowNumber, matrixItemData.PrimaryKeyColumnIndex);

                    if (rowItemId.HasValue)
                    {
                        textIds.AddRange(GetTextIdsForItemData(ItemConfigurationManager.GetConfigurationData(rowItemId.Value)));
                    }
                }

                //Column text items
                //Row text items
                for (int columnNumber = 1; columnNumber <= matrixItemData.ColumnCount; columnNumber++)
                {
                    if (columnNumber == matrixItemData.PrimaryKeyColumnIndex)
                    {
                        continue;
                    }

                    textIds.AddRange(
                        GetTextIdsForItemData(
                            ItemConfigurationManager.GetConfigurationData(
                                matrixItemData.GetColumnPrototypeId(columnNumber))));
                }
            }
            //Other composite children
            else if (itemData is ICompositeItemData)
            {
                var childItemIds = ((ICompositeItemData) itemData).GetChildItemDataIDs();

                foreach (var childItemId in childItemIds)
                {
                    textIds.AddRange(GetTextIdsForItemData(ItemConfigurationManager.GetConfigurationData(childItemId)));
                }
            }
            
            return textIds;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoImport()
        {
            if (!ApplicationManager.AppSettings.AllowMultiLanguage)
                return;

            if (!ValidateFile())
            {
                return;
            }

            //Flag refresh required
            _refreshRequired.Text = "yes";

            ////Make sure this file has text for this form
            var doc = GetUploadedFileDocument();

            //Now try to import
            try
            {
                TextManager.ImportTexts(new StringReader(doc.OuterXml));
            }
            catch
            {
                ShowStatusMessage(WebTextManager.GetText("/pageText/settings/importText.aspx/importProviderError"), StatusMessageType.Warning);
                return;
            }

            //If we get this far, report success
            ShowStatusMessage(WebTextManager.GetText("/pageText/settings/importText.aspx/importSuccessful"), StatusMessageType.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ValidateFile()
        {
            string fileName = _uploadedFileNameTxt.Text;

            //Check extension.
            if (!(".xml".Equals(Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase)))
            {
                _extensionValidationError.Visible = true;
                return false;
            }

            var xmlDoc = GetUploadedFileDocument();

            if (xmlDoc == null || xmlDoc.DocumentElement == null)
            {
                _invalidXmlError.Visible = true;
                return false;
            }

            //Ensure file is a text export
            if (!xmlDoc.HasChildNodes)
            {
                _formatValidationError.Visible = true;
                return false;
            }

            //Check if this xml contains information about response template.
            if (xmlDoc.DocumentElement.Name != "textExport")
            {
                _formatValidationError.Visible = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetUploadedFileDocument()
        {
            //Get file data
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return null;
            }

            //Attempt to read & parse file as xml
            try
            {
                var ms = new MemoryStream(fileBytes);
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(ms);

                return xmlDocument;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _import_Click(object sender, EventArgs e)
        {            
            EventHandlerWrapper(DoImport);
        }

        /// <summary>
        /// Handle ok button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            Master.CloseDialog("{op: 'editLanguages', result: 'ok', refresh:'no'}", true);
        }

        #region IStatusPage Members

        public void WireStatusControl(System.Web.UI.Control sourceControl)
        {
         
        }

        public void WireUndoControl(System.Web.UI.Control sourceControl)
        {
            throw new NotImplementedException();
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _importExportStatus.Message = message;
            _importExportStatus.MessageType = messageType;
            //_importExportStatus.ActionText = actionText;
            //_importExportStatus.ActionArgument = actionArgument;
            _importExportStatus.ShowStatus();
        }

        #endregion
    }
}
