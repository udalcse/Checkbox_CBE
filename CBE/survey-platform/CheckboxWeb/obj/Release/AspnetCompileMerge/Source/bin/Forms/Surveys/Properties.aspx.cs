using System;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Survey status page
    /// </summary>
    public partial class Properties : ResponseTemplatePage
    {
        private string _guidUrl;

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Form.Administer"; } }

        /// <summary>
        /// The fully qualified url which uses a GUID to identify a survey.
        /// It is used as the key when looking up custom urls
        /// </summary>
        private string GuidUrl
        {
            get
            {
                return _guidUrl ?? (_guidUrl = string.Format("{0}/Survey.aspx?s={1}",
                                                             ApplicationManager.ApplicationRoot,
                                                             ResponseTemplate.GUID.ToString().Replace("-", String.Empty)));
            }
        }

        /// <summary>
        /// Set initial values
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Used in name validator
            _properties.CurrentSurveyName = ResponseTemplate.Name;
            _properties.EnableScoring = ResponseTemplate.BehaviorSettings.EnableScoring;
            _properties.ActivateSurvey = ResponseTemplate.BehaviorSettings.IsActive;
            _properties.SurveyName = ResponseTemplate.Name;

            ConfigureCustomUrl();

            Master.OkClick += OkBtn_Click;

            Master.Title = WebTextManager.GetText("/pageMenu/survey_editor/_properties") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64);

        }

        /// <summary>
        /// 
        /// </summary>
        private void ConfigureCustomUrl()
        {
            //Parse out the extension
            string customUrl = UrlMapper.GetSource(GuidUrl);
            int extensionStart = customUrl.LastIndexOf(".");

            if (extensionStart > 0)
            {
                string extension = customUrl.Substring(extensionStart + 1);
                string url = customUrl.Substring(0, extensionStart);

                if (_properties.ExtensionExists(extension))
                {
                    _properties.CustomUrlExtension = extension;
                    _properties.CustomUrl = url;
                }
            }
        }

        /// <summary>
        /// Get title for page
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && _properties.CustomUrlIsValid())
            {
                //Apply and save changes
                ResponseTemplate.Name = _properties.SurveyName;
                ResponseTemplate.BehaviorSettings.EnableScoring = _properties.EnableScoring;
                ResponseTemplate.BehaviorSettings.IsActive = _properties.ActivateSurvey;

                ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ResponseTemplate.Save();

                ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);

                //Set the short URL Mapping
                if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
                {
                    _properties.SaveCustomUrl(GuidUrl);
                }

                //Close window
                Master.CloseDialog("properties", false);
            }
        }
    }
}
