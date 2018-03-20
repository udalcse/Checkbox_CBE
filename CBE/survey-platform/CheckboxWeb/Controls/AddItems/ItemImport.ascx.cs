using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;

namespace CheckboxWeb.Controls.AddItems
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ItemImport : Checkbox.Web.Common.UserControlBase
    {
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
                _extensionValidationError.Visible = true;
                return false;
            }

            //Ensure file is a survey
            if (!xmlDoc.HasChildNodes)
            {
                _extensionValidationError.Visible = true;
                return false;
            }

            return true;
        }
    }
}