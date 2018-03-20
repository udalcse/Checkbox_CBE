using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Forms.Validation;
using System.Web;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Credentials : EditUserPage, IStatusPage
    {
        private const string HIDDEN_PASSWORD = "**********";

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            string title = IsUserReadOnly || IsExternalUser
                ? WebTextManager.GetText("/pageText/users/properties.aspx/titleView")
                : WebTextManager.GetText("/pageText/users/properties.aspx/basicInfoTab");

            if (UserToEdit != null)
            {
                title += " - " + UserToEdit.Identity.Name;
            }


            Master.SetTitle(title);

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

            Master.OkClick += SaveUserInfoButton_Click;
            

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

            _loginTypePanel.Visible = ApplicationManager.AppSettings.AllowNetworkUsers && !ApplicationManager.AppSettings.EnableMultiDatabase;
        }



        /// <summary>
        /// 
        /// </summary>
        private void SetReadOnlyUI()
        {
            _username.Enabled = false;
            _email.Enabled = false;
            _domain.Enabled = false;

            _passwordLabel.Visible = false;
            _password.Visible = false;

            _passwordConfirmLabel.Visible = false;
            _passwordConfirm.Visible = false;

            Master.OkVisible = false;
        }

        /// <summary>
        /// Register hide status script
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (_loginTypeList.SelectedValue == "Checkbox")
            {
                _passwordPlace.Visible = true;
                _domainPlace.Visible = false;
                _descriptionLabel.Text = WebTextManager.GetText("/pageText/users/add.aspx/loginType/checkbox/explanation");
            }
            else
            {
                _passwordPlace.Visible = false;
                _domainPlace.Visible = true;
                _descriptionLabel.Text = WebTextManager.GetText("/pageText/users/add.aspx/loginType/external/explanation");
            }

            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));

            if (User.Identity.Name == UserToEdit.Identity.Name)
            {
                if (!IsPostBack)
                {
                    String scriptText =
                        "alert('" + WebTextManager.GetText("/pageText/users/Credentials.aspx/EditThemselvesAlert") + "');";
                    ClientScript.RegisterStartupScript(this.GetType(), "EditThemselves", scriptText, true);
                }
            }

            
        }

        #region Control event handlers

        /// <summary>
        /// Handles the click event of the save button on the user info panel
        /// - Saves changes to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveUserInfoButton_Click(object sender, EventArgs e)
        {
            if (IsUserReadOnly)
            {
                return;
            }

            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                //if username contains @ char is should be an email
                if (_username.Text.Contains("@"))
                {
                    var validator = new Checkbox.Forms.Validation.EmailValidator();

                    if (!validator.Validate(_username.Text.Trim()))
                    {
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"),
                            WebTextManager.GetText("/pageText/users/properties.aspx/userNameEmailRulesViolated")), StatusMessageType.Error);
                        return;
                    }
                }

                //Is email valid?     
                if (!String.IsNullOrEmpty(_email.Text))
                {
                    var validator = new Checkbox.Forms.Validation.EmailValidator();

                    if (!validator.Validate(_email.Text.Trim()))
                    {
                        _emailFormatInvalidLabel.Visible = true;
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"),
                            WebTextManager.GetText("/pageText/users/properties.aspx/emailInvalid")), StatusMessageType.Error);

                        return;
                    }
                    _emailFormatInvalidLabel.Visible = false;
                }
                else
                    _emailFormatInvalidLabel.Visible = false;

                //Is username in use (if it's being changed)?
                string cleanName = ApplicationManager.AppSettings.AllowHTMLNames ? _username.Text.Trim() : Server.HtmlEncode(_username.Text.Trim());
                if (String.Compare(cleanName, UserToEdit.Identity.Name, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    if (UserManager.UserExists(cleanName))
                    {
                        _usernameInUseLabel.Visible = true;
                        return;
                    }
                    _usernameInUseLabel.Visible = false;
                }

                //If the password is being changed, are the two versions present and the same?
                if (String.Compare(_password.Text.Trim(), HIDDEN_PASSWORD) != 0)
                {
                    if (String.IsNullOrEmpty(_password.Text))
                    {
                        _passwordError.Text = WebTextManager.GetText("/pageText/users/properties.aspx/passwordRequired");
                        _passwordError.Visible = true;
                        return;
                    }

                    if (_passwordConfirm.Text.Length <= 0)
                    {
                        _confirmPasswordError.Text = WebTextManager.GetText("/pageText/users/properties.aspx/passwordConfirmRequired");
                        _confirmPasswordError.Visible = true;
                        return;
                    }

                    if (!String.Equals(_password.Text.Trim(), _passwordConfirm.Text.Trim()))
                    {
                        _passwordError.Text = WebTextManager.GetText("/pageText/users/properties.aspx/passwordMatch");
                        _passwordError.Visible = true;
                        return;
                    }

                    //Make sure password is valid in case of EnforcePasswordLimitsGlobally set to "true".
                    if (ApplicationManager.AppSettings.EnforcePasswordLimitsGlobally)
                    {
                        PasswordValidator passwordValidator = new PasswordValidator();

                        if (!passwordValidator.Validate(_password.Text.Trim()))
                        {
                            _passwordError.Text = passwordValidator.GetMessage(WebTextManager.GetUserLanguage());
                            _passwordError.Visible = true;
                            return;
                        }
                     }
                }

                _passwordError.Visible = false;
                bool passwordUpdated = (_password.Text != HIDDEN_PASSWORD);

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

                    CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName, null, pw, _email.Text.Trim(),
                        ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);

                    if (updatedUser == null)
                    {
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"),
                            status == UserManager.ERROR_USER_NOT_UNIQUE
                                ? WebTextManager.GetText("/pageText/users/properties.aspx/updateErrorUnique")
                                : status), StatusMessageType.Error);
                        return;
                    }

                    UserGuid = updatedUser.UserGuid;

                    Session["EditUser"] = updatedUser;
                }
                else
                {
                    //Make sure a domain is specified
                    if (String.IsNullOrEmpty(_domain.Text))
                    {
                        _domainError.Visible = true;
                        return;
                    }

                    string status;

                    CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName, _domain.Text, String.Empty, _email.Text.Trim(),
                        ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);

                    if (updatedUser == null)
                    {
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), status), StatusMessageType.Error);
                        return;
                    }

                    ExternalUserId = updatedUser.Identity.Name;

                    Session["EditUser"] = updatedUser;
                }

               
                //If user has modified his username he will be logged out 
                bool needToLogout = (User.Identity.Name == UserToEdit.Identity.Name) && (_username.Text != User.Identity.Name || passwordUpdated);
                if (needToLogout)
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect(FormsAuthentication.LoginUrl, false);
                }
                else
                {
                    ReloadUser();
                }

                var args = new Dictionary<string, string>
                               {
                                   {"page", "credentials"},
                                   {"newUserName", UserToEdit.Identity.Name}
                               };
                Master.CloseDialog(args);
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return;
            }
        }

        /// <summary>
        /// Validates the format of the username
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void Username_ValidateFormat(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (!UserManager.ValidateUniqueIdentifierFormat(args.Value))
            {
                args.IsValid = false;
            }
        }

        #endregion

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
        }

        public void WireUndoControl(Control sourceControl)
        {
            throw new NotImplementedException();
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _statusControl.Message = message;
            _statusControl.MessageType = messageType;
            _statusControl.ShowStatus();
        }

        #endregion

    }
}