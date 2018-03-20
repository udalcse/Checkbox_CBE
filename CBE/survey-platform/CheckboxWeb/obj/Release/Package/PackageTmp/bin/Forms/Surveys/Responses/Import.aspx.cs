using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;

using Checkbox.Web.Page;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Analytics.Import;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    public partial class Import : SecuredPage
    {
        [QueryParameter("s")]
        public int ResponseId;

        [QueryParameter("ReturnPage")]
        public string ReturnPage;

        private bool _errorShown;

        ResponseTemplate _rt;

        ResponseTemplate Survey
        {
            get { return _rt ?? (_rt = ResponseTemplateManager.GetResponseTemplate(ResponseId)); }
        }

        protected string CancelRedirectUrl
        {
            get
            {
                return ReturnPage;
            }
        }

        /// <summary>
        /// Require form create permission
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Create"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();            
            _inputFile.Attributes.Add("onchange", String.Format("OnFileChoose({0},{1});return false", Master.OkClientID, _inputFile.ClientID));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_inputFile.PostedFile != null)
            {
                if (ReadXmlFile())
                {
                    ProcessImport();
                }
            }

            Master.OkEnable = false;
            Master.OkTextId = "/controlText/wizard/next";
            Master.CancelTextId = "/common/close";
        }

        /// <summary>
        /// Read XML
        /// </summary>
        /// <returns>true - if file is correct, another - false</returns>
        private bool ReadXmlFile()
        {
            HttpPostedFile file = _inputFile.PostedFile;
            string fileExtension = Path.GetExtension(file.FileName);

            if (fileExtension.ToLower() != ".xml")
            {
                Master.ShowError(WebTextManager.GetText("/pageText/surveyImport.aspx/errorMessage") + WebTextManager.GetText("/pageText/surveyImport.aspx/fileTypeError"), null); 
                _errorShown = true;
                return false;
            }

            //Instantiate the XML objects
            Stream xmlStream = file.InputStream;
            StreamReader xmlReader = new StreamReader(xmlStream);
            XmlTextReader reader = new XmlTextReader(xmlStream);

            try
            {
                XmlDataValidator.Validate(reader, true);

                xmlStream.Position = 0;
                reader = new XmlTextReader(xmlStream);

                List<ItemInfo> srcItems;
                List<ResponseInfo> srcResponses;
                List<string> errors = XmlDataImporter.ParseAndValidate(reader, Survey, out srcItems, out srcResponses);

                int count = srcResponses == null ? 0 : srcResponses.Count;

                if (count != 0)
                {
                    Session["ItemInfos"] = srcItems;
                    Session["ResponseInfos"] = srcResponses;

                    if (errors.Count > 0)
                    {
                        StringBuilder sb = GetErrorText(errors);

                        InfoPanel.Visible = true;
                        WarningLabel.Text = sb.ToString();
                        WarningLabel.Visible = true;

                        return false;
                    }

                    return true;
                }

                if (errors.Count > 0)
                {
                    StringBuilder sb = GetErrorText(errors);

                    InfoPanel.Visible = true;
                    WarningLabel.Text = sb.ToString();
                    WarningLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Master.ShowError(ex.Message, ex);
                return false;
            }

            return false;
        }

        private static StringBuilder GetErrorText(List<string> errors)
        {
            var sb = new StringBuilder();

            foreach (var t in errors)
            {
                sb.Append(t);
                sb.AppendLine("<br />");
            }
            return sb;
        }

        /// <summary>
        /// Xml validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ValidationHandler(object sender, ValidationEventArgs e)
        {
			Master.ShowError(e.Message, e.Exception);
            _errorShown = true;
        }

		private void ProcessImport()
		{
			//Set progress key
			Session["ImportResultsProgressKey"] = "ImportResults_" + ResponseId + "_" + Session.SessionID;

			//Redirect
			Response.Redirect("ImportProgress.aspx?SurveyId=" + ResponseId + "&ReturnPage=" + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery), true);
		}
    }
}
