using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Security.Principal;
using Checkbox.Security;
using Prezza.Framework.ExceptionHandling;
using System.Web;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Properties : EditUserPage, IStatusPage
    {
        private const string HIDDEN_PASSWORD = "**********";

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set the user to edit on the user controls
            _profileEditor.Initialize(UserToEdit, IsUserReadOnly);
            _groupSelector.Initialize(UserToEdit, IsUserReadOnly);
            _roleSelector.Initialize(UserToEdit, IsUserReadOnly);


            string title = WebTextManager.GetText("/pageText/users/properties.aspx/title");
                title += " - " + UserToEdit.Identity.Name;

            Master.SetTitle(title);

            Master.OkClick += OnOkClick;

            _password.Text = HIDDEN_PASSWORD;
            _passwordConfirm.Text = HIDDEN_PASSWORD;
            _password.Attributes["value"] = HIDDEN_PASSWORD;
            _passwordConfirm.Attributes["value"] = HIDDEN_PASSWORD;

            _username.Text = UserToEdit.Identity.Name;
            _domain.Text = UserManager.GetDomain(UserToEdit.Identity.Name);
            _email.Text = UserManager.GetUserEmail(UserToEdit.Identity.Name);

            //
            if (IsUserReadOnly)
            {
                _readOnlyUserWarningPanelLoginInfo.Visible = true;
                _readOnlyUserWarningPanelProfile.Visible = true;
                _readOnlyUserWarningPanelRoles.Visible = true;
                _readOnlyUserWarningPanelGroups.Visible = true;
            }

            //Show/hide inputs for external users
            if (UserManager.EXTERNAL_USER_AUTHENTICATION_TYPE.Equals(UserToEdit.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase))
            {
                //Hide read-only panel to prevent duplicate warnings
                _readOnlyUserWarningPanelLoginInfo.Visible = false;

                _username.Enabled = false;
                _email.Enabled = false;

                _passwordLabel.Visible = false;
                _password.Visible = false;

                _passwordConfirmLabel.Visible = false;
                _passwordConfirm.Visible = false;

                _externalUserWarningPanel.Visible = true;

                Master.OkEnable = false;
            }
        }

        /// <summary>
        /// Register hide status script
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        #region Control event handlers

        /// <summary>
        /// Handles the change of the login type radio button; changes the appearance of the login info panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoginType_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        }

        /// <summary>
        /// Check tab validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnOkClick(object sender, EventArgs e)
        {
            //Validate
            Page.Validate();

            if (!Page.IsValid)
            {
                _currentTabIndex.Text = "0";
            }

            if (SaveUserInfo()
                && SaveProfileInfo()
                && SaveRolesInfo()
                && SaveGroupsInfo())
            {
                //Close and reload
                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "closeWindow",
                    "closeWindow('userUpdated')",
                    true);
            }
        }

        /// <summary>
        /// Handles the click event of the save button on the user info panel
        /// - Saves changes to the user
        /// </summary>
        protected bool SaveUserInfo()
        {
            if (IsUserReadOnly)
            {
                return false;
            }

            try
            {
                //Is email valid?     
                if (!String.IsNullOrEmpty(_email.Text))
                {
                    var validator = new Checkbox.Forms.Validation.EmailValidator();
                    
					if (!validator.Validate(_email.Text.Trim()))
                    {
                        _emailFormatInvalidLabel.Visible = true;
						ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), "E-Mail is not valid"), StatusMessageType.Error);

                        return false;
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
                        return false;
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
                        return false;
                    }

                    if (_passwordConfirm.Text.Length <= 0)
                    {
                        _confirmPasswordError.Text = WebTextManager.GetText("/pageText/users/properties.aspx/passwordConfirmRequired");
                        _confirmPasswordError.Visible = true;
                        return false;
                    }

                    if (!String.Equals(_password.Text.Trim(), _passwordConfirm.Text.Trim()))
                    {
                        _passwordError.Text = WebTextManager.GetText("/pageText/users/properties.aspx/passwordMatch");
                        _passwordError.Visible = true;
                        return false;
                    }
				
				}

				_passwordError.Visible = false;

                //Update user properties
                if (!_loginTypePanel.Visible || _loginTypeList.SelectedValue == "Password")
                {
                    bool passwordUpdated = (_password.Text != HIDDEN_PASSWORD);

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

                    CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName, null, pw, (_email.Text ?? string.Empty).Trim(), ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);

                    if (updatedUser == null)
                    {
                        ShowStatusMessage(String.Format(
                            WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), 
                            status == UserManager.ERROR_USER_NOT_UNIQUE 
                                ? WebTextManager.GetText("/pageText/users/properties.aspx/updateErrorUnique") 
                                : status), 
                            StatusMessageType.Error);
                        return false;
                    }

                    //Cause user to be reloaded on next access
                    ReloadUser();
                }
                else
                {
                    //Make sure a domain is specified
                    if (String.IsNullOrEmpty(_domain.Text))
                    {
                        _domainError.Visible = true;
                        return false;
                    }

                    string status;

                    CheckboxPrincipal updatedUser = UserManager.UpdateUser(UserToEdit.Identity.Name, cleanName, _domain.Text, String.Empty, _email.Text.Trim(),
                        ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);

                    if (updatedUser == null)
                    {
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), status), StatusMessageType.Error);
                        return false;
                    }

                    //Cause user to be reloaded on next access
                    ReloadUser();
                }

                return true;
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return false;
            }
        }

        /// <summary>
        /// Handles the click event of the save button on the profile properties panel
        /// - Saves changes to the user
        /// </summary>
        protected bool SaveProfileInfo()
        {
            try
            {
                if (_profileEditor.ProfileProperties.Keys.Count > 0)
                {
                    ProfileManager.StoreProfile(UserToEdit.Identity.Name, _profileEditor.ProfileProperties);
                }

                return true;
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return false;
            }
        }

        /// <summary>
        /// Handles the click event of the save button on the roles panel
        /// - Saves changes to the user
        /// </summary>
        protected bool SaveRolesInfo()
        {
            try
            {
                //Remove user from all roles
                List<string> selectedRoles = _roleSelector.SelectedRoles;
                string[] currentRoles = RoleManager.ListRolesForUser(UserToEdit.Identity.Name);

                foreach (string currentRole in currentRoles)
                {
                    //If current role not selected, remove
                    if (selectedRoles.FirstOrDefault(role => role.Equals(currentRole, StringComparison.InvariantCultureIgnoreCase)) == null)
                    {
                        RoleManager.RemoveUserFromRoles(UserToEdit.Identity.Name, new[] {currentRole});
                    }
                }

                //Now add to selected
                if (selectedRoles.Count > 0)
                {
                    RoleManager.AddUserToRoles(UserToEdit.Identity.Name, selectedRoles.ToArray());
                }

                return true;
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return false;
            }
        }

        /// <summary>
        /// Handles the click event of the save button on the profile properties panel
        /// - Saves changes to the user
        /// </summary>
        protected bool SaveGroupsInfo()
        {
            try
            {
                List<int> currentMemberships = GroupManager.ListGroupMembershipIds(UserToEdit.Identity.Name);
                List<int> selectedGroups = _groupSelector.SelectedGroupIDs;

                bool needToInvalidateCache = false;

                foreach (int currentGroupId in currentMemberships)
                {
                    if (!selectedGroups.Contains(currentGroupId))
                    {
                        Group g = GroupManager.GetGroup(currentGroupId);

                        if (g != null)
                        {
                            g.RemoveUser(UserToEdit.Identity.Name);
                            g.Save();
                            needToInvalidateCache = true;
                        }
                    }
                }
                if (selectedGroups.Count > 0)
                {
                    foreach (Int32 groupID in selectedGroups)
                    {
                        Group group = GroupManager.GetGroup(groupID);

                        if (group != null)
                        {
                            group.AddUser(UserToEdit.Identity);
                            group.Save();
                            needToInvalidateCache = true;
                        }
                    }
                }

                if (needToInvalidateCache)
                {
                    GroupManager.InvalidateUserMemberships(UserToEdit.Identity.Name);
                }

                return true;
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return false;
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
            //_statusControl.ActionText = actionText;
            //_statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        #endregion
    }
}
