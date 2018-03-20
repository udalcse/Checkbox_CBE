using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Security.Principal;
using Checkbox.Globalization.Text;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Management;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Web.Page;

using Prezza.Framework.Security;
using Checkbox.LicenseLibrary;
using Checkbox.Users;
using Checkbox.Web;
using Prezza.Framework.ExceptionHandling;

//using Xheo.Licensing;

namespace CheckboxWeb.Users.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RoleSelector : Checkbox.Web.Common.UserControlBase
    {
        //private ExtendedLicense _license;
        private bool? _isSurveyEditor = null;
        private bool? _isSurveyEditorAllowed = null;

        //
        protected bool IsReadOnly { get; set; }

        //
        public IPrincipal UserToEdit { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToEdit"></param>
        /// <param name="isReadOnly"></param>
        public void Initialize(IPrincipal userToEdit, bool isReadOnly)
        {
            //_license = license;
            UserToEdit = userToEdit;
            IsReadOnly = isReadOnly;

            //Get the list of assignable roles
            BindRoles();
            SelectAssignedRoles();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindRoles()
        {
            List<string> roles = ApplicationManager.UseSimpleSecurity ? RoleManager.ListSimpleSecurityRoles() : AvailableRoles(RoleManager.ListRoles());

            var currentUser = HttpContext.Current.User as CheckboxPrincipal;
            if (!currentUser.IsInRole("System Administrator") && roles.Contains("System Administrator"))
            {
                //Remove system admin from the list of assignable roles
                roles.Remove("System Administrator");
            }

            if (ApplicationManager.UseSimpleSecurity)
            {
                _normalRoleSelector.Visible = false;
                _simpleRoleSelector.Visible = true;

                _simpleRolesList.DataSource = RoleManager.ListSimpleSecurityRoles();
                _simpleRolesList.DataBind();
            }
            else
            {
                _normalRoleSelector.Visible = true;
                _simpleRoleSelector.Visible = false;

                _roleList.DataSource = roles;

                _roleList.DataBind();

                _roleList.Enabled = !IsReadOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _roleWarningPanel.Visible = !CanCreateSurveyEditor;
        }


        public void SaveRoles()
        {
            //Remove user from all roles
            string[] currentRoles = RoleManager.ListRolesForUser(UserToEdit.Identity.Name);

            foreach (string currentRole in currentRoles)
            {
                //If current role not selected, remove
                if (this.SelectedRoles.FirstOrDefault(role => role.Equals(currentRole, StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                    RoleManager.RemoveUserFromRoles(UserToEdit.Identity.Name, new[] { currentRole });
                }
            }

            //Now add to selected
            RoleManager.AddUserToRoles(UserToEdit.Identity.Name, this.SelectedRoles.ToArray());


            //User roles have been changed - so the user must be reloaded
            UserManager.ExpireCachedPrincipal(UserToEdit.Identity.Name);
        }

        /// <summary>
       /// 
       /// </summary>
       /// <param name="allRoles"></param>
       /// <returns></returns>
        private List<string> AvailableRoles(List<string> allRoles)
        {
            if (IsSurveyEditorAllowed || IsSurveyEditor)
                return allRoles;

           return (from roleName in allRoles
                   where ! roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase)
                   let role = RoleManager.GetRole(roleName)
                   where role != null && !role.HasPermission("Form.Edit") && !role.HasPermission("Analysis.Administer")
                   select roleName).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectAssignedRoles()
        {
            if (UserToEdit != null)
            {
                string[] userRoles = RoleManager.ListRolesForUser(UserToEdit.Identity.Name);

                if (ApplicationManager.UseSimpleSecurity)
                {
                    ListItem li = null;
                    
                    if (userRoles.Contains(TextManager.GetText("/simpleSecurity/systemAdministrator")))
                        li = _simpleRolesList.Items.FindByText(TextManager.GetText("/simpleSecurity/systemAdministrator"));
                    
                    else if (userRoles.Contains(TextManager.GetText("/simpleSecurity/surveyAdministrator")))
                        li = _simpleRolesList.Items.FindByText(TextManager.GetText("/simpleSecurity/surveyAdministrator"));
                    
                    else if (userRoles.Contains(TextManager.GetText("/simpleSecurity/respondent")))
                        li = _simpleRolesList.Items.FindByText(TextManager.GetText("/simpleSecurity/respondent"));
                    
                    if (li != null)
                        li.Selected = true;
                }
                else
                {
                    foreach (string roleName in userRoles)
                    {
                        ListItem checkbox = _roleList.Items.FindByText(roleName);

                        if (checkbox != null)
                        {
                            checkbox.Selected = true;
                        }
                    }
                }
            }
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private bool CanCreateSurveyEditor
        {
            get
            {
                return (IsSurveyEditorAllowed || IsSurveyEditor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsSurveyEditor
        {
            get
            {
                if (_isSurveyEditor == null)
                {
                    if (UserToEdit == null)
                        return false;

                    bool isAdmin = false;
                    string[] userRoles = RoleManager.ListRolesForUser(UserToEdit.Identity.Name);

                    foreach (string roleName in userRoles)
                    {
                        if (roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isAdmin = true;
                            break;
                        }

                        Role role = RoleManager.GetRole(roleName);

                        if (role != null && (role.HasPermission("Form.Edit") || role.HasPermission("Analysis.Administer")))
                        {
                            isAdmin = true;
                            break;
                        }
                    }

                    _isSurveyEditor = isAdmin;
                }

                return _isSurveyEditor.Value;
            }
        }

        /// <summary>
        /// Get whether a survey editor can be specified
        /// </summary>
        private bool IsSurveyEditorAllowed
        {
            get
            {
				if (_isSurveyEditorAllowed == null)
				{
					SurveyEditorLimit limit = (Page as LicenseProtectedPage).ActiveLicense.SurveyEditorLimit;

					if (limit == null)
					{
						_isSurveyEditorAllowed = true;

						return _isSurveyEditorAllowed.Value;
					}

					string message;
					LimitValidationResult result = limit.Validate(out message);

					_isSurveyEditorAllowed = (result == LimitValidationResult.LimitNotReached);
				}

				return _isSurveyEditorAllowed.Value;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public List<string> SelectedRoles
        {
            get
            {
                var output = new List<string>();
                if (ApplicationManager.UseSimpleSecurity)
                {
                    output.Add(_simpleRolesList.SelectedItem.Text);
                }
                else
                {
                    output.AddRange(from ListItem checkbox in _roleList.Items
                                    where checkbox.Selected
                                    select checkbox.Text);
                }

                return output;
            }
        }

        public bool IsValid => this.SelectedRoles.Any();

        #endregion
    }
}
