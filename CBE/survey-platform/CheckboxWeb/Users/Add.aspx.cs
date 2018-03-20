using System;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Security.Principal;
using Prezza.Framework.Security;
using Checkbox.Security;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Common;
using System.Collections.Generic;
using Checkbox.Forms.Validation;

namespace CheckboxWeb.Users
{
    public partial class Add : SecuredPage
    {
        //set max length for fields in "User information" block
        const int UserInformationFieldMaxLength = 24;
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Initilize the role selector so that it can determine if the editor limit has been reached
            _roleSelector.Initialize(null, false);
            _groupSelector.Initialize(null, false);
            _profileEditor.Initialize(null, false);

            if (_profileEditor.ProfileProperties.Count == 0)
                _profileInstructions.TextId = "/pageText/users/add.aspx/noProfileProperties";

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _addUserWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/users/add.aspx/wizardStepTitle/{0}", step.ID));
            }

            if (ApplicationManager.AppSettings.NTAuthentication)
            {
                _descriptionLabel.Text = WebTextManager.GetText("/pageText/users/add.aspx/loginType/checkbox/explanation");
                _loginInfoPanel.CssClass = "right fixed_500";
                _loginTypePanel.Visible = true;
            }

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                Password = null;
            }

            /*Need additional time to test this feature
            var currentUser = HttpContext.Current.User as CheckboxPrincipal;

            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentUser, GroupManager.GetEveryoneGroup(), "Group.ManageUsers") && _groupSelector.AvailableGroupsCount == 0)
            {
                Response.Redirect(ResolveUrl("~/Users/Groups/AddGroupDialog.aspx"));
            }
            */

            Master.SetTitle(WebTextManager.GetText("/pageText/users/add.aspx/title"));

            //Change the instructional text depending on whether or not email address is required

            //if (ApplicationManager.AppSettings.RequireEmailAddressOnRegistration)
            //{
            //    _loginInfoInstructions.Text = WebTextManager.GetText("/pageText/users/add.aspx/loginInfoInstructionsEmailRequired");
            //    _emailRequired.Enabled = true;
            //}
            //else
            //{
                _loginInfoInstructions.Text = WebTextManager.GetText("/pageText/users/add.aspx/loginInfoInstructions");
                _emailRequired.Enabled = false;
            //}

            Master.HideDialogButtons();
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RegisterClientScriptInclude("jQueryValidate", ResolveUrl("~/Resources/jquery.validate.min.js"));
        }

        #region Properties

        /// <summary>
        /// Get the required role permissions for the page
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Group.ManageUsers"; }
        }

        private string Password
        {
            get { return Session["upp"].ToString(); }
            set { Session["upp"] = value; }
        }
        
        #endregion

        #region Wizard event handlers

        /// <summary>
        /// Handles the finish event of the wizard; saves the new user to the configured provider,
        /// then redirects back to the user manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddUserWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                //Create the user
                string username = ApplicationManager.AppSettings.AllowHTMLNames ? _username.Text.Trim() : Server.HtmlEncode(_username.Text.Trim());
                string userCreateStatus = String.Empty;
                CheckboxPrincipal newUser = UserManager.CreateUser(username, Password, _domain.Text.Trim(), _email.Text.Trim(), ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out userCreateStatus);
                Password = null;

                //Now set the profile properties, roles, and groups
                if (_profileEditor.ProfileProperties.Keys.Count > 0)
                {
                    ProfileManager.StoreProfile(newUser.Identity.Name, _profileEditor.ProfileProperties);
                }

                if(_profileEditor.RadioFields.Keys.Count > 0)
                {
                    _profileEditor.StoreRadioButtons(UserManager.GetUserPrincipal(newUser.Identity.Name).UserGuid);
                }

                if (_roleSelector.SelectedRoles.Count > 0)
                {
                    RoleManager.AddUserToRoles(newUser.Identity.Name, _roleSelector.SelectedRoles.ToArray());
                }

                if (_groupSelector.SelectedGroupIDs.Count > 0)
                {
                    foreach (Int32 groupID in _groupSelector.SelectedGroupIDs)
                    {
                        Group group = GroupManager.GetGroup(groupID);
                        group.AddUser(newUser);
                        group.Save();
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                _completionTitle.Text = WebTextManager.GetText("/pageText/users/add.aspx/errorTitle");
                _createUserError.Text = String.Format(WebTextManager.GetText("/pageText/users/add.aspx/errorMessage"), err.Message);
                _createUserError.Visible = true;
            }

        }

        /// <summary>
        /// Handles the next button click of the wizard; validates inputs and saves data required to create a user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddUserWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            _emailFormatInvalidLabel.Visible = false;
            _usernameInUseLabel.Visible = false;
            _roleRequiredError.Visible = false;
            _passwordValidationErrorLabel.Visible = false;

            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            //Login information
            if (String.Compare(_addUserWizard.WizardSteps[e.CurrentStepIndex].ID, "UsernameStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                //Is email valid?     
                if (!String.IsNullOrEmpty(_email.Text))
                {
                    var validator = new Checkbox.Forms.Validation.EmailValidator();
                    if (!validator.Validate(_email.Text.Trim()))
                    {
                        _emailFormatInvalidLabel.Visible = true;
                        e.Cancel = true;
                        return;
                    }
                }

                //Is username in use?
                string cleanName = ApplicationManager.AppSettings.AllowHTMLNames ? _username.Text.Trim() : Server.HtmlEncode(_username.Text.Trim());
                if (UserManager.UserExists(cleanName))
                {
                    _usernameInUseLabel.Visible = true;
                    e.Cancel = true;
                    return;
                }

                PasswordValidator passwordValidator = new PasswordValidator();

                if (!passwordValidator.Validate(_password.Text.Trim()))
                {
                    _passwordValidationErrorLabel.Text = passwordValidator.GetMessage(WebTextManager.GetUserLanguage());
                    _passwordValidationErrorLabel.Visible = true;
                    e.Cancel = true;
                    return;
                }

                //Store the password so it's available when creating the user in the finish step (using the session so it's not sent back to the browser)
                Password = _password.Text.Trim();
            }
            //else if (String.Compare(_addUserWizard.WizardSteps[e.CurrentStepIndex].ID, "ProfileStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            //{
            //    foreach (string propertyName in _profileEditor.ProfileProperties.Keys)
            //    {
            //        ProfileProperties[propertyName] = _profileEditor.ProfileProperties[propertyName];
            //    }
            //}
            else if (String.Compare(_addUserWizard.WizardSteps[e.CurrentStepIndex].ID, "RoleStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (_roleSelector.SelectedRoles.Count == 0)
                {
                    _roleRequiredError.Visible = true;
                    e.Cancel = true;
                    return;
                }

                //foreach (string roleName in _roleSelector.SelectedRoles)
                //{
                //    Roles.Add(roleName);
                //}
            }
            else if (String.Compare(_addUserWizard.WizardSteps[e.CurrentStepIndex].ID, "GroupStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                // If the current principal is not an administrator and simple security is not enabled
                // then a user group must be selected
                var currentUser = HttpContext.Current.User as CheckboxPrincipal;
                if (!currentUser.IsInRole("System Administrator") && !ApplicationManager.UseSimpleSecurity && _groupSelector.AvailableGroupsCount > 0)
                {
                    if (_groupSelector.SelectedGroupIDs.Count <= 0
                        && !AuthorizationFactory.GetAuthorizationProvider().Authorize(currentUser, GroupManager.GetEveryoneGroup(), "Group.ManageUsers"))
                    {
                        _groupRequiredError.Visible = true;
                        e.Cancel = true;
                        return;
                    }
                }

                //foreach (Int32 groupID in _groupSelector.SelectedGroupIDs)
                //{
                //    Groups.Add(groupID);
                //}

                //Set up the review screen
                _usernameReview.ToolTip = _username.Text;
                _usernameReview.Text = Utilities.TruncateText(_username.Text, UserInformationFieldMaxLength);
                if (!String.IsNullOrEmpty(_email.Text))
                {
                    _emailReview.ToolTip = _email.Text;
                    _emailReview.Text = Utilities.TruncateText(_email.Text, UserInformationFieldMaxLength);
                }
                if (!String.IsNullOrEmpty(_domain.Text))
                {
                    _domainReview.Text = _domain.Text;
                }
                else
                {
                    _domainReview.Text = "<i>[None]</i>";
                }
                
                _profileReviewList.DataSource = _profileEditor.ProfileProperties.Keys;
                _profileReviewList.DataBind();

                _rolesReviewList.DataSource = _roleSelector.SelectedRoles;
                _rolesReviewList.DataBind();

                _groupsReviewList.DataSource = _groupSelector.SelectedGroupIDs;
                _groupsReviewList.DataBind();
            }
        }

        /// <summary>
        /// Handles the cancel event of the wizard;
        /// - redirects back to the user manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddUserWizard_CancelButtonClick(object sender, EventArgs e)
        {
            if (Request["reloadOnCancel"] == "true")
            {
                var args = new Dictionary<string, string> { { "page", "addUser" }, { "newUserName", Request["lastAddedUserName"] } };

                Master.CloseDialog("reloadUserList", args);
            }
            else
                Master.CloseDialog(null);
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _closeWizardButton_Click(object sender, EventArgs e)
        {
            string username = ApplicationManager.AppSettings.AllowHTMLNames ? _username.Text.Trim() : Server.HtmlEncode(_username.Text.Trim());

            var args = new Dictionary<string, string> { { "page", "addUser" }, { "newUserName", username } };

            Master.CloseDialog("onDialogClosed",args);
        }

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
        /// Handles the item created event of the profile review list
        /// - Populates this user's values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileReviewList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem && ((ListViewDataItem)e.Item).DataItem != null)
            {
                var labelText = _profileEditor.ProfileProperties[((ListViewDataItem)e.Item).DataItem.ToString()];
                ((Label)e.Item.FindControl("_profileReview")).ToolTip = labelText;
                ((Label)e.Item.FindControl("_profileReview")).Text = Utilities.TruncateText(labelText, UserInformationFieldMaxLength);
            }
        }

        /// <summary>
        /// Handles the item created event of the group review list
        /// - Sets the group name based on the selected group id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GroupsReviewList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem && ((ListViewDataItem)e.Item).DataItem != null)
            {
                ((Label)e.Item.FindControl("_groupReviewLabel")).Text = Utilities.TruncateText(GroupManager.GetGroup(Convert.ToInt32(((ListViewDataItem)e.Item).DataItem)).Name, 50);
            }
        }


        protected void RestartButton_OnClick(object sender, EventArgs e)
        {
            string username = ApplicationManager.AppSettings.AllowHTMLNames ? _username.Text.Trim() : Server.HtmlEncode(_username.Text.Trim());
            Response.Redirect(ResolveUrl("~/Users/Add.aspx?reloadOnCancel=true&lastAddedUserName=" + username));
        }

        //if username contains @ char is should be an email
        protected void Username_ValidateEmailFormat(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (!_username.Text.Contains("@"))
                return;
            var validator = new EmailValidator();
            if (!validator.Validate(_username.Text.Trim()))
            {
                args.IsValid = false;
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

        protected void Username_ValidateLength(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (1 > args.Value.Length || args.Value.Length > 255)
            {
                args.IsValid = false;
            }
        }

        #endregion
    }
}
