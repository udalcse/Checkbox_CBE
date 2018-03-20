using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class ExternalUsers : SettingsPage
    {
        /// <summary>
        /// Page load
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
          
            //enable/disable page controls
            ConfigurePageControls();

        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            Response.ClearContent();
            writer.Write("<b>External users functionality is not supported in versions 6.4 or higher</b>");
            Response.Flush();
        }

        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/externalUsers.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);
            Master.OkClick += new EventHandler(Master_OkClick);
            

            if (ApplicationManager.UseSimpleSecurity)
            {
                _standardRolesPanel.Visible = false;
                _simpleRolesPanel.Visible = true;
            }
            else
            {
                _standardRolesPanel.Visible = true;
                _simpleRolesPanel.Visible = false;
            }

            _allowExternalUsers.Checked = ApplicationManager.AppSettings.NTAuthentication;
            _requireRegistration.Checked = ApplicationManager.AppSettings.RequireRegisteredUsers;
            _authenticationVariableName.Text = ApplicationManager.AppSettings.NTAuthenticationVariableName;

            LoadRolesData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadRolesData()
        {
            //Load list of available roles
            _roles.DataSource = RoleManager.ListRoles(false);
            _roles.DataBind();

            string[] currentRoles = ApplicationManager.AppSettings.DefaultRolesForUnAuthenticatedNetworkUsers;

            foreach (string role in currentRoles)
            {
                ListItem option = _roles.Items.FindByText(role);
                
                if (option != null)
                    option.Selected = true;
            }
        }

        /// <summary>
        /// Enables/Disables page controls depending on selected options.
        /// </summary>
        private void ConfigurePageControls()
        {
            //   1. If Allow network login is disabled all other pages controls are disabled.
            //   2. If require registration is enabled role controls are disabled.
            _requireRegistration.Enabled = _allowExternalUsers.Checked;
            _authenticationVariableName.Enabled = _allowExternalUsers.Checked;

            _roles.Enabled = _allowExternalUsers.Checked && !_requireRegistration.Checked;
            _rolesLabel.Enabled = _allowExternalUsers.Checked && !_requireRegistration.Checked;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        void Master_OkClick(object sender, EventArgs e)
        {
            //Set the properties
            ApplicationManager.AppSettings.NTAuthentication = _allowExternalUsers.Checked;
            ApplicationManager.AppSettings.RequireRegisteredUsers = _requireRegistration.Checked;
            ApplicationManager.AppSettings.NTAuthenticationVariableName = _authenticationVariableName.Text;
            ApplicationManager.AppSettings.DefaultRolesForUnAuthenticatedNetworkUsers = GetSelectedRoles();

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }

        private string[] GetSelectedRoles()
        {
            var roles = new List<string>();

//            if (ApplicationManager.UseSimpleSecurity)
//            {
//                roles.AddRange(_simpleRolesList.SelectedValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
//            }

            for (int i = 0; i < _roles.Items.Count; i++)
            {
                if (_roles.Items[i].Selected)
                    roles.Add(_roles.Items[i].Text);
            }

            return roles.ToArray();
        }
    }
}
