using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.SystemMode;
using Checkbox.Users;
using Checkbox.Web.Page;
using CheckboxWeb.Services;

namespace CheckboxWeb.Settings
{
    public partial class SystemMode : SettingsPage
    {
        /// <summary>
        ///     The system mode database key
        /// </summary>
        private const string SystemModeKey = "SystemMode";

        private SurveyManagementService _surveyManagementService;

        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += SaveBtn_Click;

            _surveyManagementService = new SurveyManagementService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var systemMode = ApplicationManager.AppSettings.GetValue(SystemModeKey, string.Empty);

                if (string.IsNullOrEmpty(systemMode))
                {
                    throw new Exception("Can not define system mode, pick one and click Save button");
                }

                _systemModeOptions.SelectedValue = systemMode;

                BindAdminUsersList();
            }

            _timeline.VisibleEvents = "PREPMODE_ON,PREPMODE_OFF";

            if (_systemModeOptions.SelectedValue.Equals("ProdMode") && !IsPrepModelAllowed())
            {
                //disable prep mode option
                _systemModeOptions.Items[0].Enabled = false;
                _prepModeWarningMsg.Visible = true;
            }

            _availableUsers.Enabled = _systemModeOptions.SelectedValue == "PrepMode";
        }

        private void BindAdminUsersList()
        {
            var userNameList = GetAdminUsers();

            Dictionary<string,string> users = new Dictionary<string, string>();

            foreach (var userName in userNameList)
            {
                var email = UserManager.GetUserEmail(userName);
                if (!string.IsNullOrEmpty(email) && !users.ContainsKey(userName))
                    users.Add(userName, email);
            }

            //adding all users in system administratore role to list
            foreach (var user in users)
            {
                var guid = UserManager.GetUserPrincipal(user.Key).UserGuid;

                _availableUsers.Items.Add(new ListItem
                {
                    Text = $"{user.Key ?? "userName"} ({user.Value ?? "userEmail"})",
                    Value = user.Key,
                    Selected = SystemModeManager.IsPrepModeUser(guid)
                });
            }
        }

        /// <summary>
        /// Determines whether [is prep model allowed].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is prep model allowed]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPrepModelAllowed()
        {
            bool isPrepModeAllowed = true; 
                                                                                          
            var avaliableSurveys = _surveyManagementService.ListAvailableSurveys(string.Empty, 1, int.MaxValue, string.Empty, false,
                string.Empty, string.Empty);

            // if any survey is active , it is not allow to use prep mode
            if (avaliableSurveys?.ResultData?.ResultPage != null)
            {
                isPrepModeAllowed = !avaliableSurveys.ResultData.ResultPage.Any(survey => survey.IsActive);

                if (isPrepModeAllowed)
                {
                    if (avaliableSurveys.ResultData.ResultPage.Any(survey => ResponseTemplateManager.GetTemplateResponseCount(survey.ID, true, false) > 0))
                        isPrepModeAllowed = false;
                }
            }
            
            return isPrepModeAllowed;
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.SetValue(SystemModeKey, _systemModeOptions.SelectedItem.Value);

            foreach (ListItem user in _availableUsers.Items)
            {
                var guid = UserManager.GetUserPrincipal(user.Value).UserGuid;

                if (!Guid.Empty.Equals(guid))
                    SystemModeManager.AddPrepModeUser(guid, user.Selected);
            }
        }

        protected void SystemModeOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            SystemModeManager.LogEvent(new SystemModeEvent()
            {
                EventType = _systemModeOptions.SelectedValue == "PrepMode" ? SystemModeEventType.PREPMODE_ON : SystemModeEventType.PREPMODE_OFF
            });
        }

        private List<string> GetAdminUsers()
        {
            var roles = RoleManager.ListRoles(true).Where(x => x.Contains("Administrator"));
            var userNameList = new List<string>();

            foreach (var role in roles)
            {
                try
                {
                    userNameList = userNameList.Concat(UserManager.ListUsersInRole(UserManager.GetCurrentPrincipal(),
                        role,
                        new PaginationContext())).ToList();
                }
                catch (ArgumentNullException ex)
                {
                    HandleException(ex, $"User with {role} can not be found");
                }
            }

            return userNameList;
        }
    }
}