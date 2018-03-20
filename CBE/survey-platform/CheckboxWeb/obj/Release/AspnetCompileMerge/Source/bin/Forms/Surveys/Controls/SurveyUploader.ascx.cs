using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;
using Checkbox;
using Prezza.Framework.Common;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Control to handle uploading surveys to Checkbox
    /// </summary>
    public partial class SurveyUploader : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _extensionValidationError.Visible = false;
            _invalidXmlError.Visible = false;
            _formatValidationError.Visible = false;
            _xmlfromPreviousVersion.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string SurveyName
        {
            get 
            {
                if(string.IsNullOrEmpty(_parsedNameTxt.Value) && GetUploadedFileDocument()!= null)
                {
                    _parsedNameTxt.Value = LoadSurveyNameFromXml();
                }

                return _parsedNameTxt.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UploadedTempFilePath { get { return _uploadedFilePathTxt.Text; } }

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
        /// Load survey from xml
        /// </summary>
        private string LoadSurveyNameFromXml()
        {
            var xmlDoc = GetUploadedFileDocument();

            if (xmlDoc == null || xmlDoc.DocumentElement == null)
            {
                return string.Empty;
            }

            return XmlUtility.GetNodeText(
                xmlDoc.DocumentElement.SelectSingleNode("TemplateData/TemplateName")
                ?? xmlDoc.DocumentElement.SelectSingleNode("TemplateName"));
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

            //Check if this xml contains information about response template.
            if (xmlDoc.DocumentElement.Name != "ResponseTemplate")
            {
                _formatValidationError.Visible = true;
                return false;
            }

            //If the next section is xml scheme, it's xml created by previous Checkbox version.
            if(xmlDoc.FirstChild != null && xmlDoc.FirstChild.Name == "ConfigurationData")
            {
                _xmlfromPreviousVersion.Visible = true;
                return false;
            }

            return true;
        }
    }
}