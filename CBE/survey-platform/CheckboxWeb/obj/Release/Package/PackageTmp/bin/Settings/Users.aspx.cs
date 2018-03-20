using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class Users : SettingsPage
    {
       /// <summary>
       /// 
       /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            if (!Page.IsPostBack)
            {
                _allowPublicRegistration.Checked = ApplicationManager.AppSettings.AllowPublicRegistration;
                _requireEmailAddress.Checked = ApplicationManager.AppSettings.RequireEmailAddressOnRegistration;
                _allowUsersToEditOwnInfo.Checked = ApplicationManager.AppSettings.AllowEditSelf;
                _allowUsersToResetPassword.Checked = ApplicationManager.AppSettings.AllowPasswordReset;
            }

            LoadRolesData();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/users.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            Master.OkClick += new EventHandler(Master_OkClick);

            //enable/disable page controls
            ConfigurePageControls();
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.AllowPublicRegistration = _allowPublicRegistration.Checked;
            if (_allowPublicRegistration.Checked)
            {
                ApplicationManager.AppSettings.RequireEmailAddressOnRegistration = _requireEmailAddress.Checked;
                ApplicationManager.AppSettings.DefaultUserRoles = GetSelectedRoles();
            }

            ApplicationManager.AppSettings.AllowEditSelf = _allowUsersToEditOwnInfo.Checked;
            ApplicationManager.AppSettings.AllowPasswordReset = _allowUsersToResetPassword.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }

        /// <summary>
        /// Enables/Disables page controls depending on selected options.
        /// </summary>
        private void ConfigurePageControls()
        {
            // If public registration is enabled require email and role controls are enabled.
            _requireEmailAddress.Enabled = _allowPublicRegistration.Checked;
            _roles.Enabled = _allowPublicRegistration.Checked;
        }

        private string[] GetSelectedRoles()
        {
            var roles = new List<string>();

            for (int i = 0; i < _roles.Items.Count; i++)
            {
                if (_roles.Items[i].Selected)
                    roles.Add(_roles.Items[i].Text);
            }

            return roles.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadRolesData()
        {
            //Load list of available roles
            _roles.DataSource = RoleManager.ListRoles(false);
            _roles.DataBind();

            string[] currentRoles = ApplicationManager.AppSettings.DefaultUserRoles;

            foreach (string role in currentRoles)
            {
                ListItem option = _roles.Items.FindByText(role);

                if (option != null)
                    option.Selected = true;
            }
        }
    }
}
