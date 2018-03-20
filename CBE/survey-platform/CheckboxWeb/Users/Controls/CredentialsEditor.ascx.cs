using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;

namespace CheckboxWeb.Users.Controls
{
    public partial class CredentialsEditor : Checkbox.Web.Common.UserControlBase
    {
        private const string HIDDEN_PASSWORD = "**********";

        public CheckboxPrincipal UserToEdit;
        public bool IsUserReadOnly;
        public bool IsExternalUser;
        public bool IsCheckboxNetworkUser;
        public string ExternalUserId;
        public IPrincipal CurrentUser;

        public void Initialize(CheckboxPrincipal userToEdit, bool isUserReadOnly, bool isExternalUser,
            bool isCheckboxNetworkUser, string externalUserId)
        {
            UserToEdit = userToEdit;
            IsUserReadOnly = isUserReadOnly;
            IsExternalUser = isExternalUser;
            IsCheckboxNetworkUser = isCheckboxNetworkUser;
            ExternalUserId = externalUserId;
            CurrentUser = UserManager.GetCurrentPrincipal();

            _password.Text = HIDDEN_PASSWORD;
            _passwordConfirm.Text = HIDDEN_PASSWORD;
            _password.Attributes["value"] = HIDDEN_PASSWORD;
            _passwordConfirm.Attributes["value"] = HIDDEN_PASSWORD;

            //Parse user name and domain
            _username.Text = Server.HtmlDecode(UserManager.ParseUserName(UserToEdit.Identity.Name));
            _domain.Text = UserManager.GetDomain(UserToEdit.Identity.Name);
            _email.Text = UserManager.GetUserEmail(UserToEdit.Identity.Name);

            if (IsCheckboxNetworkUser || IsExternalUser)
            {
                _loginTypeList.SelectedValue = "External";
            }

            if (IsUserReadOnly)
            {
                _readOnlyUserWarningPanel.Visible = true;

                SetReadOnlyUI();
            }
            else if (IsExternalUser)
            {
                _externalUserWarningPanel.Visible = true;

                SetReadOnlyUI();
            }

            _loginTypePanel.Visible = ApplicationManager.AppSettings.AllowNetworkUsers &&
                                      !ApplicationManager.AppSettings.EnableMultiDatabase;


        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_loginTypeList.SelectedValue == "Checkbox")
            {
                _passwordPlace.Visible = true;
                _domainPlace.Visible = false;
                _descriptionLabel.Text =
                    WebTextManager.GetText("/pageText/users/add.aspx/loginType/checkbox/explanation");
            }
            else
            {
                _passwordPlace.Visible = false;
                _domainPlace.Visible = true;
                _descriptionLabel.Text =
                    WebTextManager.GetText("/pageText/users/add.aspx/loginType/external/explanation");
            }
        }

        public void SaveUserInfoButton()
        {
            _passwordError.Visible = false;
            bool passwordUpdated = _password.Text != HIDDEN_PASSWORD;

            string cleanName = ApplicationManager.AppSettings.AllowHTMLNames
                ? _username.Text.Trim()
                : Server.HtmlEncode(_username.Text.Trim());

            //Update user properties
            if (!_loginTypePanel.Visible || _loginTypeList.SelectedValue == "Checkbox")
            {
                string status;

                string pw;

                if (passwordUpdated)
                {
                    pw = _password.Text == string.Empty ? null : _password.Text;
                }
                else
                {
                    //Passing NULL causes password to not be updated.  If any value is passed, the password will be
                    //update to that value...which is bad in cases where the password is encrypted, since the encrypted value
                    //will be re-encrypted if it is passed to update user, which expects plain-text passwords.
                    pw = null;
                }

                CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName, null,
                    pw, _email.Text.Trim(),
                    ((CheckboxPrincipal) HttpContext.Current.User).Identity.Name, out status);
         
                Session["EditUser"] = updatedUser;
            }
            else
            {
                //Make sure a domain is specified
                if (string.IsNullOrEmpty(_domain.Text))
                {
                    _domainError.Visible = true;
                    return;
                }

                string status;

                CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName,
                    _domain.Text, String.Empty, _email.Text.Trim(),
                    ((CheckboxPrincipal) HttpContext.Current.User).Identity.Name, out status);
            }

            //If user has modified his username he will be logged out 
            var user = UserManager.GetCurrentPrincipal();
            bool needToLogout = (user.Identity.Name == UserToEdit.Identity.Name) && (_username.Text != user.Identity.Name || passwordUpdated);

            if (needToLogout)
            {
                FormsAuthentication.SignOut();
                Response.Redirect(FormsAuthentication.LoginUrl, false);
            }
            else
            {
                UserToEdit = null;
            }

        }

        protected void Username_ValidateFormat(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (!UserManager.ValidateUniqueIdentifierFormat(args.Value))
            {
                args.IsValid = false;
            }
        }

        private void SetReadOnlyUI()
        {
            _username.Enabled = false;
            _email.Enabled = false;
            _domain.Enabled = false;

            _passwordLabel.Visible = false;
            _password.Visible = false;

            _passwordConfirmLabel.Visible = false;
            _passwordConfirm.Visible = false;
        }

        public CredentialEditorErrorCode Validate()
        {
            CredentialEditorErrorCode validationCode = CredentialEditorErrorCode.None;

            //if username contains @ char is should be an email
            if (_username.Text.Contains("@"))
            {
                var validator = new EmailValidator();

                if (!validator.Validate(_username.Text.Trim()))
                    return  CredentialEditorErrorCode.UserNameEmailRulesViolated;
            }

            //Is email valid?     
            if (!String.IsNullOrEmpty(_email.Text))
            {
                var validator = new EmailValidator();

                if (!validator.Validate(_email.Text.Trim()))
                {
                    _emailFormatInvalidLabel.Visible = true;

                    return CredentialEditorErrorCode.EmailIsNotValid;
                }

                _emailFormatInvalidLabel.Visible = false;
            }
            else
                _emailFormatInvalidLabel.Visible = false;

            //Is username in use (if it's being changed)?
            string cleanName = ApplicationManager.AppSettings.AllowHTMLNames
                ? _username.Text.Trim()
                : Server.HtmlEncode(_username.Text.Trim());
            if (String.Compare(cleanName, UserToEdit.Identity.Name, StringComparison.InvariantCultureIgnoreCase) !=
                0)
            {
                if (UserManager.UserExists(cleanName))
                {
                    _usernameInUseLabel.Visible = true;
                    return CredentialEditorErrorCode.UserExists;
                }
                _usernameInUseLabel.Visible = false;
            }

            return validationCode;
        }
    }

    public enum CredentialEditorErrorCode
    {
        None = 1,
        UserNameEmailRulesViolated = 2,
        EmailIsNotValid = 3,
        UserExists = 4,
        UpdateUserIsNull = 5,
        NeedToLogout =  6 
    }
}