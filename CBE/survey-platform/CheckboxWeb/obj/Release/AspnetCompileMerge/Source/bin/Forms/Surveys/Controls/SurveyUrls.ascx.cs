using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class SurveyUrls : Checkbox.Web.Common.UserControlBase
    {
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// TextID for the greeting being displayed 
        /// "/pageText/forms/surveys/launch.aspx/summary"
        /// </summary>
        public string MessageTextID
        {
            get;
            set;
        }

        /// <summary>
        /// TextID for the message being displayed when the survey has GUID link and custom URLs
        /// "/controlText/forms/surveys/controls/surveyUrls.ascx/guidAndCustom"
        /// </summary>
        public string GUIDAndCustomTextID
        {
            get;
            set;
        }


        /// <summary>
        /// TextID for the message being displayed when the survey has GUID link only
        /// "/controlText/forms/surveys/controls/surveyUrls.ascx/guidOnly"
        /// </summary>
        public string GUIDOnlyTextID
        {
            get;
            set;
        }

        /// <summary>
        /// Class for the container
        /// </summary>
        public string ContainerCSSClass
        {
            get;
            set;
        }

        /// <summary>
        /// Determine if it's a test response.
        /// </summary>
        public bool IsTestResponse { get; set; }

        public string GuidURL
        {
            get
            {
                return _guidUrl.Text;
            }
        }

        public string CustomURL
        {
            get
            {
                return _customSurveyUrl.Text;
            }
        }

        public string AdditionalURLParams
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                _customSurveyUrl.ToolTip = WebTextManager.GetText("/common/newWindow");
                _guidUrlImage.ToolTip = WebTextManager.GetText("/common/newWindow");
            }

            var surveyUrlWithGuid = ResolveUrl("~/Survey.aspx") + "?s=" + ResponseTemplateManager.GetResponseTemplateGUID(ResponseTemplateId).ToString().Replace("-", String.Empty);
            var customUrl = UrlMapper.GetSource(surveyUrlWithGuid);

            string url = ApplicationManager.ApplicationURL + surveyUrlWithGuid + "&" + AdditionalURLParams + (IsTestResponse ? "&test=true" : String.Empty);

            _guidUrl.Text = ApplicationManager.ApplicationURL + surveyUrlWithGuid + (IsTestResponse ? "&forceNew=true&test=true" : String.Empty);
            _guidUrl.NavigateUrl = url;

            _htmlCode.Text = string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\">\n</iframe>", url);
            _htmlCode.Attributes["style"] = "margin: 10px;";
            
            if (IsTestResponse)
                _htmlCode.Visible = false;

            if (Utilities.IsNotNullOrEmpty(customUrl) && ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                _guidUrlLabel.Text = WebTextManager.GetText(GUIDAndCustomTextID);

                _customSurveyUrl.Text = ApplicationManager.ApplicationURL + customUrl + (IsTestResponse ? "?forceNew=true&test=true" : String.Empty);
                _customSurveyUrl.NavigateUrl = ApplicationManager.ApplicationURL + customUrl + "?" + AdditionalURLParams + (IsTestResponse ? "&test=true" : String.Empty);

                _customUrlPanel.Visible = true;
            }
            else
            {
                _guidUrlLabel.Text = WebTextManager.GetText(GUIDOnlyTextID);
                _customUrlPanel.Visible = false;
            }
        }
    }
}