using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Checkbox.Web.Forms.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EnterPasswordBase : Common.UserControlBase
    {
        #region Abstract Members

        protected abstract Label WrongPasswordLbl { get; }
        protected abstract Label PasswordLbl { get; }
        protected abstract Button PasswordBtn { get; }
        protected abstract RequiredFieldValidator PasswordRequiredValidator { get; }
        protected abstract TextBox PasswordTxt { get; }

        protected abstract string EnterPasswordSurveyText { get; }
        protected abstract string MiscContinueSurveyText { get; }
        protected abstract string InvalidPasswordSurveyText { get; }

        #endregion

        private const string PASSWORD_ENTERED_FLAG = "PasswordEnteredFlag";

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Visible)
                ViewState[PASSWORD_ENTERED_FLAG] = null;
            else if (ViewState[PASSWORD_ENTERED_FLAG] != null)
                WrongPasswordLbl.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="responseTemplateId"></param>
        public void Initialize(string languageCode, int responseTemplateId)
        {
            LanguageCode = languageCode;
            ResponseTemplateId = responseTemplateId;

            PasswordLbl.Text = EnterPasswordSurveyText;

         //   PasswordRequiredValidator.Text = InvalidPasswordSurveyText;
            PasswordRequiredValidator.ToolTip = PasswordRequiredValidator.Text;
            WrongPasswordLbl.Text = InvalidPasswordSurveyText;

            if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
                PasswordBtn.OnClientClick = "return false;";

            PasswordBtn.Text = MiscContinueSurveyText;
            PasswordBtn.Attributes["data-action"] = "password";
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetPasswordEntered()
        {
            ViewState[PASSWORD_ENTERED_FLAG] = 1;

            string password = Request[PasswordTxt.UniqueID];
            
            return password != null ? password.Trim() : string.Empty;
        }
    }
}
