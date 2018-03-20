using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Displays a button Share via Facebook and allows to share the survey
    /// </summary>
    public partial class FacebookButton : Checkbox.Web.Common.UserControlBase
    {
        public string ResponseTemplateName { get; set; }
        public string ResponseTemplateTitleTextID { get; set; }

        public bool HideButton { set; get; }

        public bool HideShareButton { set; get; }

        protected string ActionLinkText
        {
            get
            {
                return string.Format(WebTextManager.GetText("/facebookButton/actionLinkTextTemplate"), string.IsNullOrEmpty(WebTextManager.GetText(ResponseTemplateTitleTextID)) ? ResponseTemplateName : WebTextManager.GetText(ResponseTemplateTitleTextID));
            }
        }

        protected string FacebookAppID
        {
            get
            {
                return ApplicationManager.AppSettings.FacebookAppID;
            }
        }

        protected bool FacebookSettingsExist
        {
            get
            {
                return !string.IsNullOrEmpty(FacebookAppID);
            }
        }

        public void InitSurveyText()
        {
            _description.Text = string.Format(WebTextManager.GetText("/facebookButton/descriptionTemplate"), string.IsNullOrEmpty(WebTextManager.GetText(ResponseTemplateTitleTextID)) ? ResponseTemplateName : WebTextManager.GetText(ResponseTemplateTitleTextID));
            _message.Text = string.Format(WebTextManager.GetText("/facebookButton/messageTemplate"), ResponseTemplateName);
            _userPrompt.Text = WebTextManager.GetText("/facebookButton/userMessagePrompt");
            _caption.Text = string.Format(WebTextManager.GetText("/facebookButton/messageCaption"), string.IsNullOrEmpty(WebTextManager.GetText(ResponseTemplateTitleTextID)) ? ResponseTemplateName : WebTextManager.GetText(ResponseTemplateTitleTextID));
        }

        public void InitReportText()
        {
            _description.Text = string.Format(WebTextManager.GetText("/facebookButton/report/descriptionTemplate"), string.IsNullOrEmpty(WebTextManager.GetText(ResponseTemplateTitleTextID)) ? ResponseTemplateName : WebTextManager.GetText(ResponseTemplateTitleTextID));
            _message.Text = string.Format(WebTextManager.GetText("/facebookButton/report/messageTemplate"), ResponseTemplateName);
            _userPrompt.Text = WebTextManager.GetText("/facebookButton/report/userMessagePrompt");
            _caption.Text = string.Format(WebTextManager.GetText("/facebookButton/report/messageCaption"), string.IsNullOrEmpty(WebTextManager.GetText(ResponseTemplateTitleTextID)) ? ResponseTemplateName : WebTextManager.GetText(ResponseTemplateTitleTextID));
        }
    }
}