using System.Web.UI.WebControls;
using Checkbox.Wcf.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Login : Checkbox.Web.Forms.TakeSurvey.LoginBase
    {
        protected override Panel LoginFailedWrapperPanel
        {
            get { return _loginFailedWrapper; }
        }

        protected override Button LoginButton
        {
            get { return _loginButton; }
        }

        protected override TextBox UserNameTextBox
        {
            get { return _userName; }
        }

        protected override TextBox PasswordTextBox
        {
            get { return _password; }
        }

        protected override string LoginButtonSurveyText
        {
            get { return SurveyEditorServiceImplementation.GetSurveyText("LOGIN_LOGINBUTTON", ResponseTemplateId, LanguageCode, "en-US"); }
        }
    }

}