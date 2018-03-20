using System;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Users;

namespace Checkbox.Web.Forms.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoginBase : Common.UserControlBase
    {
        #region Abstract Members

        protected abstract Panel LoginFailedWrapperPanel { get; }
        protected abstract Button LoginButton { get; }
        protected abstract TextBox UserNameTextBox { get; }
        protected abstract TextBox PasswordTextBox { get; }
        protected abstract string LoginButtonSurveyText { get; }

        #endregion

        protected string InviteeGuid
        {
            get
            {
                Guid g;
                if (Guid.TryParse(Request.QueryString["i"], out g))
                    return g.ToString().ToLower().Replace("-", "");

                return null;
            }
        }

        /// <summary>
        /// Language code
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Initialize with language code.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="responseTemplateId"></param>
        public virtual void Initialize(string languageCode, int responseTemplateId)
        {
            //Store language code
            LanguageCode = languageCode;
            ResponseTemplateId = responseTemplateId;

            if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
                LoginButton.OnClientClick = "return false;";

            LoginButton.Attributes["data-action"] = "login";
            LoginButton.Text = LoginButtonSurveyText;

            if (string.IsNullOrEmpty(LoginButton.Text))
                TextManager.GetText("/pageText/login.aspx/loginButton");
        }

        /// <summary>
        /// Get name of successfully authenticated user
        /// </summary>
        public string GetAuthenticatedUserName()
        {
            var userName = Request[UserNameTextBox.UniqueID];
            if (userName != null)
                userName = Server.HtmlEncode(userName.Trim());

            var password = Request[PasswordTextBox.UniqueID];
            if (password != null)
                password = Server.HtmlEncode(password.Trim());

            //Validate credentials rather than logging user in to avoid overwriting any login created by
            // main application login page.
            if (!UserManager.ValidateLoginCredentials(userName, password))
            {
                LoginFailedWrapperPanel.Visible = true;
                return null;
            }

            return userName;
        }        
    }
}
