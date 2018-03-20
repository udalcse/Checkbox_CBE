using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Configuration;
using Checkbox.Security.Principal;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Users;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users.Groups
{
    public partial class Add : SecuredPage
    {
        private string _exitRedirectDestination;

        /// Specify whether grids should be automatically loaded or not.  If delay is set to true, grids
        /// will not be loaded until reload grids method are called. Default value is true.
        public bool DelayLoad
        {
            get
            {
                bool temp;
                return bool.TryParse(_delayLoad.Text, out temp) && temp;
            }
            set { _delayLoad.Text = value.ToString(); }
        }

        protected override void OnPageInit()
        {
            base.OnPageInit();


            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _addGroupWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/users/groups/add.aspx/wizardStepTitle/{0}", step.ID));
            }

            SetupPageTitle();

            _exitRedirectDestination = ResolveUrl("~/Users/Groups/Members.aspx");

            //Validators initialization.
            _groupNameRequired.ErrorMessage = WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameRequired");
            _groupNameLength.ErrorMessage = WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameLength");
            _groupExistValidator.ErrorMessage = WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameExists");

            Master.HideDialogButtons();

            //Initialize grid
            _availableGrid.ItemClickCallback =  "onAvailableItemSelect";
            _availableGrid.ListTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListTemplate.html");
            _availableGrid.ListItemTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListItemTemplate.html");
            _availableGrid.LoadDataCallback = ID + "loadAvailableGridAjax";
            _availableGrid.EmptyGridText = "No available users";
            _availableGrid.DelayLoad = DelayLoad;
            _availableGrid.IsAjaxScrollModeEnabled = true;
            _availableGrid.InitialFilterField = "UniqueIdentifier";
            _availableGrid.FilterItemType = "available";

            _currentGrid.ItemClickCallback =  "onCurrentItemSelect";
            _currentGrid.ListTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListTemplate.html");
            _currentGrid.ListItemTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListItemTemplate.html");
            _currentGrid.LoadDataCallback = ID + "loadCurrentGridAjax";
            _currentGrid.EmptyGridText = "No users";
            _currentGrid.DelayLoad = DelayLoad;
            _currentGrid.IsAjaxScrollModeEnabled = true;
            _currentGrid.InitialFilterField = "UniqueIdentifier";
            _currentGrid.FilterItemType = "current";

            _reviewGroupUsersGrid.ListTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListTemplate.html");
            _reviewGroupUsersGrid.ListItemTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/userNewGroupListItemTemplate.html");
            _reviewGroupUsersGrid.LoadDataCallback = ID + "loadCurrentGridAjax";
            _reviewGroupUsersGrid.EmptyGridText = "No users";
            _reviewGroupUsersGrid.DelayLoad = DelayLoad;
            _reviewGroupUsersGrid.IsAjaxScrollModeEnabled = true;
            _reviewGroupUsersGrid.InitialFilterField = "UniqueIdentifier";
            _reviewGroupUsersGrid.FilterItemType = "current";

            _userStoreWrapper.Visible = !StaticConfiguration.DisableForeighMembershipProviders;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                //clean session
                GroupManager.CleanCurrentUsersForNewGroup();
            }
            //Service helper
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            //Survey management
            RegisterClientScriptInclude(
                "svcUserManagement.js",
                ResolveUrl("~/Services/js/svcUserManagement.js"));

            //Highlight
            RegisterClientScriptInclude(
                "jquery.highlight-3.yui.js",
                ResolveUrl("~/Resources/jquery.highlight-3.yui.js"));

            //Text helper
            RegisterClientScriptInclude(
                "textHelper.js",
                ResolveUrl("~/Resources/textHelper.js"));

            //Status control
            RegisterClientScriptInclude(
                "statusControl.js",
                ResolveUrl("~/Resources/statusControl.js"));
        }
        

        private void SetupPageTitle()
        {   
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Users/Groups/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/users/groups/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = string.Format(" - {0}", WebTextManager.GetText("/pageText/users/groups/add.aspx/title"));
            
            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);
            Master.SetTitleControl(titleControl);

        }

        #region Properties

        /// <summary>
        /// Get required permission for the page
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Group.Edit"; }
        }

        /// <summary>
        /// Get the required permission for the specified access controllable entity
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Group.Edit"; }
        }

        #endregion

        #region Wizard event handlers

        /// <summary>
        /// Handles the next button click of the wizard
        /// - validates inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddGroupWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            string cleanName = _groupName.Text.Trim();
            string cleanDescription = _groupDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            if (String.Compare(_addGroupWizard.WizardSteps[e.CurrentStepIndex].ID, "GroupNameStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                //Set up the review screen
                _groupNameReview.Text = cleanName;
                _groupDescriptionReview.Text = cleanDescription;
            }
        }

        /// <summary>
        /// Handles the finsh button click of the wizard
        /// - Creates the new group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddGroupWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            string cleanName = _groupName.Text.Trim();
            string cleanDescription = _groupDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            Group newGroup = null;

            try
            {          
                newGroup = GroupManager.CreateGroup(cleanName, cleanDescription);
                newGroup.ModifiedBy = HttpContext.Current.User.Identity.Name;
                newGroup.Save();

            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                _completionTitle.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/errorTitle");
                _createGroupError.Text = String.Format(WebTextManager.GetText("/pageText/users/groups/add.aspx/errorMessage"), err.Message);
                _createGroupError.Visible = true;
                _exitRedirectDestination = ResolveUrl("~/Users/Groups/Manage.aspx");
                _editUserGroupButton.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/exitButtonError");
                return;
            }

            if (newGroup != null)
            {
                if (Checkbox.Management.ApplicationManager.UseSimpleSecurity)
                {
                    Checkbox.Security.AccessManager.UpdatePolicy(newGroup.DefaultPolicyID.Value, new Prezza.Framework.Security.Policy(new string[] { "Group.View", "Group.Edit", "Group.Delete", "Group.ManageUsers" }));
                    newGroup.ReloadDefaultPolicy();
                }

                Session["CurrentGroup"] = newGroup;

                //save group members
                var newGroupMembers = GroupManager.ListCurrentUsersForNewGroup(HttpContext.Current.User as CheckboxPrincipal, "");
                if (newGroupMembers.Count > 0)
                {
                    foreach (var userUniqueIdentifier in newGroupMembers)
                    {
                        newGroup.AddUser(userUniqueIdentifier);
                        GroupManager.InvalidateUserMemberships(userUniqueIdentifier);
                    }
                    newGroup.Modifier = HttpContext.Current.User.Identity.Name;
                    newGroup.Save();
                }

                //clean session
                GroupManager.CleanCurrentUsersForNewGroup();
            }
            else
            {
                _completionTitle.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/errorTitle");
                _createGroupError.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/errorMessageGeneric");
                _createGroupError.Visible = true;
                _exitRedirectDestination = ResolveUrl("~/Users/Groups/Manage.aspx");
                _editUserGroupButton.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/exitButtonError");
            }
        }

        /// <summary>
        /// Handles the cancel button click of the wizard
        /// - returns the user to the group manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddGroupWizard_CancelButtonClick(object sender, EventArgs e)
        {
            //clean session
            GroupManager.CleanCurrentUsersForNewGroup();
            
            if (Request["reloadOnCancel"]=="true")
            {
                Group currentGroup = Session["CurrentGroup"] as Group;

                var args = new Dictionary<string, string>();
                args.Add("page", "addGroup");
                args.Add("newGroupId", currentGroup.ID.ToString());

                Master.CloseDialog("reloadGroupList", args);
            }
            else
            {
                Master.CloseDialog(null);                
            }
        }

        #endregion

        #region Control event handlers

        protected void _close_OnClick(object sender, EventArgs e)
        {
            //clean session
            GroupManager.CleanCurrentUsersForNewGroup();

           //add null args to prevent js error
            var args = new Dictionary<string, string>();
            args.Add("arg", null);
            Master.CloseDialog("reloadGroupList", args);
            
        }

        protected void _editUserGroupButton_OnClick(object sender, EventArgs e)
        {
            var currentGroup = Session["CurrentGroup"] as Group;

            if (currentGroup != null)
            {
                var args = new Dictionary<string, string> { {"page", "addGroup"}, { "newGroupId", currentGroup.ID.ToString() } };

                Master.CloseDialog("reloadGroupList", args);
            }
            else{
            Master.CloseDialog(null);}
        }

        /// <summary>
        /// Checking for an existing group name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void _groupExistValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string cleanName = _groupName.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
            }

            args.IsValid = !GroupManager.IsDuplicateName(null, cleanName);
        }

        #endregion
    }
}
