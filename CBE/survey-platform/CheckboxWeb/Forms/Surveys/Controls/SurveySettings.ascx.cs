using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Security.Principal;
using Checkbox.Styles;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class SurveySettings : Checkbox.Web.Common.UserControlBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _exportText.Click += _exportText_Click;
            /*
            _extensionValidationError.Visible = false;
            _invalidXmlError.Visible = false;
            _formatValidationError.Visible = false;*/

        }

        private ResponseTemplate _rt;

        public int ResponseTemplateId { set; get; }

        public ResponseTemplate ResponseTemplate
        {
            get
            {
                return _rt ?? (_rt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId));
            }
        }

        protected IDictionary<int, string> PcStyles
        {
            get { return GetStylesPC(); }
        }

        protected IDictionary<int, string> MobileStyles
        {
            get { return GetStylesMobile(); }
        }

        protected IDictionary<int, string> ProgressBarOrientationOptions
        {
            get { return GetProgressBarOrientationOptions(); }
        }

        private IDictionary<int, string> GetStylesPC()
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            var styles = StyleTemplateManager.ListStyleTemplates(HttpContext.Current.User as CheckboxPrincipal, StyleTemplateType.PC);

            //set default option 
            result.Add(0, TextManager.GetText("/controlText/forms/surveys/infoWidgets/appearance/noStyle"));

            foreach (var styleTemplate in styles)
            {
                result.Add(styleTemplate.TemplateId, styleTemplate.Name);
            }
            return result;
        }

        private IDictionary<int, string> GetStylesMobile()
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            var styles = MobileStyleManager.GetAllStyles(TextManager.DefaultLanguage).OrderByDescending(s => s.IsDefault);

            foreach (var style in styles)
            {
                result.Add(style.StyleId, style.Name);
            }

            return result;
        }

        private IDictionary<int, string> GetProgressBarOrientationOptions()
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            var options = Enum.GetValues(typeof (ProgressBarOrientation));

            foreach (var option in options)
            {
                result.Add((int)(ProgressBarOrientation)option, 
                    TextManager.GetText("/enum/progressBarOrientation/"+ option.ToString().ToLower()));
            }

            return result;
        }

        #region Export

        void _exportText_Click(object sender, EventArgs e)
        {
            Response.Expires = -1;
            Response.Buffer = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Utilities.StripHtml(ResponseTemplate.Name.Replace(" ", string.Empty), null) + "_TextExport.xml");
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
            var textIdList = new List<string> { "/templateData/" + ResponseTemplateId };

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
                textIds.AddRange(((SelectItemData)itemData).Options.Select(itemOption => itemOption.TextID));
            }
            //Matrix children -- Row Text items and colum items
            else if (itemData is MatrixItemData)
            {
                var matrixItemData = (MatrixItemData)itemData;

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
                var childItemIds = ((ICompositeItemData)itemData).GetChildItemDataIDs();

                foreach (var childItemId in childItemIds)
                {
                    textIds.AddRange(GetTextIdsForItemData(ItemConfigurationManager.GetConfigurationData(childItemId)));
                }
            }

            return textIds;
        }

        #endregion

        #region Import



        #endregion
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetUploadedFileDocument()
        {
            //Get file data
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.FileName);

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
        }*/
    }
}