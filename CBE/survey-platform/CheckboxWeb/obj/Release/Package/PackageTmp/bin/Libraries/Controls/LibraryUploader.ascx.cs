using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Prezza.Framework.Common;

namespace CheckboxWeb.Libraries.Controls
{
    public partial class LibraryUploader : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _extensionValidationError.Visible = false;
            _invalidXmlError.Visible = false;
            _formatValidationError.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LibraryName
        {
            get
            {
                if (string.IsNullOrEmpty(_parsedNameTxt.Value) && GetUploadedFileDocument() != null)
                {
                    _parsedNameTxt.Value = LoadLibraryNameFromXml();
                }

                return _parsedNameTxt.Value;
            }
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
            var ms = new MemoryStream(fileBytes);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(ms);

            return xmlDocument;
        }


        /// <summary>
        /// Load library name from xml
        /// </summary>
        private string LoadLibraryNameFromXml()
        {
            var xmlDoc = GetUploadedFileDocument();

            if (xmlDoc == null || xmlDoc.DocumentElement == null)
            {
                return string.Empty;
            }

            return XmlUtility.GetNodeText(
                xmlDoc.DocumentElement.SelectSingleNode("TemplateName"));
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

            //Ensure file is a survey
            if (!xmlDoc.HasChildNodes)
            {
                _formatValidationError.Visible = true;
                return false;
            }

            //Check if this xml contains information about style
            if (xmlDoc.DocumentElement.Name != "CssDocument")
            {
                _formatValidationError.Visible = true;
                return false;
            }

            return true;
        }
    }
}