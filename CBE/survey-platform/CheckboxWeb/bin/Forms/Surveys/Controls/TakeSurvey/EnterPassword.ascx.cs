using System.Web.UI.WebControls;
using Checkbox.Wcf.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EnterPassword : Checkbox.Web.Forms.TakeSurvey.EnterPasswordBase
    {
        protected override Label WrongPasswordLbl
        {
            get { return _wrongPasswordLbl; }
        }

        protected override Label PasswordLbl
        {
            get { return _passwordLbl; }
        }

        protected override Button PasswordBtn
        {
            get { return _passwordBtn; }
        }

        protected override RequiredFieldValidator PasswordRequiredValidator
        {
            get { return _passwordRequiredValidator; }
        }

        protected override TextBox PasswordTxt
        {
            get { return _passwordTxt; }
        }

        protected override string EnterPasswordSurveyText
        {
            get
            {
                return SurveyEditorServiceImplementation.GetSurveyText("LOGIN_ENTERPASSWORD", ResponseTemplateId, LanguageCode, "en-US");
            }
        }

        protected override string MiscContinueSurveyText
        {
            get
            {
                return SurveyEditorServiceImplementation.GetSurveyText("MISC_CONTINUE", ResponseTemplateId, LanguageCode, "en-US");
            }
        }

        protected override string InvalidPasswordSurveyText
        {
            get
            {
                return SurveyEditorServiceImplementation.GetSurveyText("LOGIN_INVALIDPASSWORD", ResponseTemplateId, LanguageCode, "en-US");
            }
        }
    }
}