using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Users
{
    public partial class ImportWorker : ProgressReportingEnabledPage
    {
        [QueryParameter]
        public bool UpdateExisting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_ImportUsers";
        }


        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Various counters for progress tracking
            int userCount = UsersToLoad.Count;
            int createdCount = 0;
            int updatedCount = 0;

            try
            {
                var progressBaseMessage = WebTextManager.GetText("/pageText/users/import.aspx/importing");

                //Start progress tracking
                ProgressProvider.SetProgress(ProgressKey,
                                             WebTextManager.GetText("/pageText/users/import.aspx/preparing"),
                                             string.Empty, ProgressStatus.Pending, 0, userCount);

                //Set up some variables to start the process
                var selectedFields = SelectedUserFields;
                var selectedRoles = SelectedUserRoles;
                var selectedGroups = SelectedUserGroups;

                int domainColumnIndex =
                    selectedFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/domain"));
                int userNameColumnIndex =
                    selectedFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/username"));
                int emailColumnIndex =
                    selectedFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/email"));
                int passwordIndex =
                    selectedFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/password"));

                //Figure out if profile updates are required
                var profileUpdateRequired = IsProfileUpdateRequired(selectedFields);

                //Get limit for number of survey editors
                var surveyEditorLimit = SurveyEditorLimit;
                var currentEditorCount = StartingSurveyEditorCount;
                var editorRoleSelected = IsSurveyEditorRoleSelected(selectedRoles);

                //Iterate through users and create them
                foreach (List<string> userToLoad in UsersToLoad)
                {
                    string domain = string.Empty;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string email = string.Empty;
                    bool updatePassword = true;

                    //Figure out user name
                    if (userNameColumnIndex >= 0)
                        userName = userToLoad[userNameColumnIndex];
                    else if (emailColumnIndex >= 0)
                        userName = userToLoad[emailColumnIndex];

                    //If no user name or email (shouldn't happen due to prior error checking), move on
                    if (Utilities.IsNullOrEmpty(userName))
                        continue;

                    //Determine domain
                    if (domainColumnIndex >= 0 && userNameColumnIndex >= 0)
                        domain = userToLoad[domainColumnIndex];

                    //Figure out password
                    if (passwordIndex >= 0)
                        password = userToLoad[passwordIndex];

                    //Figure out email
                    if (emailColumnIndex >= 0)
                        email = userToLoad[emailColumnIndex];

                    // If no password is provided the CreateUser method will automatically generate a random one.
                    // Set updatePassword to false  so that updated users don't have their passwords overwritten.
                    if (Utilities.IsNullOrEmpty(password))
                        updatePassword = false;

                    IIdentity userIdentity = null;
                    bool isDomainUser = UserManager.IsDomainUser(userName);
                    bool userExists = isDomainUser;

                    if (!isDomainUser)
                    {
                        //Create the user
                        userIdentity = SaveUser(userName,
                                            password,
                                            email,
                                            domain,
                                            emailColumnIndex >= 0,
                                            //If the email address field was specified email address should be updated even if they are empty
                                            updatePassword,
                                            UpdateExisting,
                                            out userExists);

                        if (userIdentity != null)
                            userName = userIdentity.Name;
                    }

                    //If we have a valid user, update group memberships and user roles
                    if (userIdentity != null || (isDomainUser && UpdateExisting))
                    {
                        //Increment counts
                        if (userExists)
                            updatedCount++;
                        else
                            createdCount++;

                        //Update profile, if necessary
                        if (profileUpdateRequired)
                            UpdateUserProfile(userName, userToLoad, selectedFields);

                        //Update user roles1
                        List<string> roles = new List<string>(selectedRoles);
                        UpdateUserSecurityRoles(userName, userExists, roles);

                        //Update group memberships
                        UpdateGroupMemberships(userName, userExists, selectedGroups);

                        //Prune role list, if necessary
                        if (editorRoleSelected && surveyEditorLimit.HasValue && currentEditorCount.HasValue)
                        {
                            currentEditorCount++;

                            if (currentEditorCount >= surveyEditorLimit)
                            {
                                //Remove editor roles
                                selectedRoles = RemoveEditorRoles(selectedRoles);

                                //Set value to false to prevent additional checking/pruning of the list
                                editorRoleSelected = false;
                            }
                        }


                        //clear from cache
                        UserManager.ExpireCachedPrincipal(userName);
                        ProfileManager.RemoveProfileFromCache(userName);
                    }

                    //Now save groups
                    foreach (Group group in TempGroupCache.Values)
                    {
                        group.Save();
                    }

                    //Set progress status
                    ProgressProvider.SetProgressCounter(ProgressKey, progressBaseMessage,
                                                        updatedCount + createdCount,
                                                        userCount, 100, 100);
                }
            }
            catch (Exception ex)
            {
                //Set progress status
                ProgressProvider.SetProgress(ProgressKey, "An error occurred while importing users.", ex.Message,
                                             ProgressStatus.Error, 100, 100);

                WriteResult(new { success = false, error = ex.Message });

                return;
            }

            //Set progress status
            //Set progress status
            ProgressProvider.SetProgress(ProgressKey,
                                         string.Format(
                                             WebTextManager.GetText("/pageText/users/import.aspx/completed"),
                                             createdCount, updatedCount), string.Empty,
                                         ProgressStatus.Completed, 100, 100);

            WriteResult(new { success = true });
        }

        /// <summary>
        /// Create/update a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="domain"></param>
        /// <param name="updateEmailAddress"></param>
        /// <param name="updatePassword"></param>
        /// <param name="updateExisting"></param>
        /// <param name="userExists"></param>
        private IIdentity SaveUser(string userName, string password, string email, string domain,
                                   bool updateEmailAddress, bool updatePassword, bool updateExisting,
                                   out bool userExists)
        {
            userExists = false;

            string username = ApplicationManager.AppSettings.AllowHTMLNames
                                  ? userName
                                  : HttpContext.Current.Server.HtmlEncode(userName);
            string createStatus;

            //Attempt to create a new user
            CheckboxPrincipal workingUser = UserManager.CreateUser(username, password, domain, email, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out createStatus);

            //If a new user was not created check to see if a user exists error was thrown
            if (createStatus.ToLower().Contains("user already exists")
                && workingUser == null)
            {
                userExists = true;

                //Update user password, if necessary and only if password was explicitly specified as part of the import
                //A null value is ignored in the underlying stored procedure
                if (!updatePassword)
                    password = null;

                //Update email address, if necessary and only if email address was explicitly specified as part of the import
                //A null value is ignored in the underlying stored procedure
                if (!updateEmailAddress)
                    email = null;

                if (updateExisting)
                {
                    //When updating an existing user no need to specify a new username
                    string status;
                    workingUser = UserManager.UpdateUser(username, null, domain, password, email, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);
                }
            }

            return workingUser != null ? workingUser.Identity : null;
        }

        /// <summary>
        /// Check selected user fields to determine if there are profile properties to update.
        /// </summary>
        private bool IsProfileUpdateRequired(IEnumerable<string> fields)
        {
            //If only fields are "identity" fields, then no update is required.
            return fields
                .Any(
                    userField =>
                    !userField.Equals(WebTextManager.GetText("/pageText/users/import.aspx/username"),
                                      StringComparison.InvariantCultureIgnoreCase)
                    &&
                    !userField.Equals(WebTextManager.GetText("/pageText/users/import.aspx/domain"),
                                      StringComparison.InvariantCultureIgnoreCase)
                    &&
                    !userField.Equals(WebTextManager.GetText("/pageText/users/import.aspx/password"),
                                      StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Update a user's profile with the specified profile property values
        /// </summary>
        /// <param name="userName">Identity of new user.</param>
        /// <param name="profilePropertyValues">Value of user profile properties.</param>
        /// <param name="selectedUserFields"></param>
        private void UpdateUserProfile(string userName, List<string> profilePropertyValues,
                                       List<string> selectedUserFields)
        {
            //Save the profile
            ProfileManager.UpdateUserProfile(userName, profilePropertyValues, selectedUserFields);
            var properties = ProfileManager.GetPropertiesList();
            foreach(var field in selectedUserFields)
            {
                if(properties.Exists(p => p.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase) && p.FieldType == CustomFieldType.RadioButton))
                {
                    ProfileManager.UpdateRadioFieldSelectedOption(profilePropertyValues[selectedUserFields.IndexOf(field)], field, UserManager.GetUserPrincipal(userName).UserGuid);
                }
            }
            
            //Add the user properties
            /*Dictionary<string, string> profile = ProfileManager.GetProfile(userIdentity.Name);

            for (int i = 0; i < selectedUserFields.Count; i++)
            {
                //Avoid potential index out of range, which shouldn't occur, but check to be
                // safe.
                if (i >= profilePropertyValues.Count)
                {
                    break;
                }

                profile[selectedUserFields[i]] = profilePropertyValues[i];
            }

            //Save the profile
            ProfileManager.StoreProfile(userIdentity.Name, profile);*/
        }


        /// <summary>
        /// Update user role memberhips, but ensure license limits aren't violated
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userExists"></param>
        /// <param name="selectedRoles"></param>
        private void UpdateUserSecurityRoles(string userName, bool userExists, List<string> selectedRoles)
        {
            //Now, update user roles or just plain add them.  When updating a user, remove any roles from the list that the
            // user was already a member of and remove the user from roles not in the selected roles list.
            if (userExists)
            {
                var currentIdentityRoles = new List<string>(RoleManager.ListNonDefaultRolesForUser(userName));

                var rolesToRemove = new List<string>();

                foreach (string roleName in currentIdentityRoles)
                {
                    //Remove user from roles NOT in selected roles list
                    //Do not remove the system administrator role from users
                    //This is done in order to prevent accidentally removing all system administrators
                    if (!selectedRoles.Contains(roleName) &&
                        !roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rolesToRemove.Add(roleName);
                    }
                        //If user is already in roles, remove it from list of roles to add
                    else
                    {
                        selectedRoles.Remove(roleName);
                    }
                }

                //Remove the user from roles not in the import list
                RoleManager.RemoveUserFromRoles(userName, rolesToRemove.ToArray());
            }

            //Now add roles to user
            var rolesToAdd = new List<string>();
            foreach (string roleName in selectedRoles)
            {
                if (roleName == "Survey Editor" && ApplicationManager.UseSimpleSecurity)
                {
                    //Add the user to every role except system admin
                    rolesToAdd.AddRange(from simpleRoleName in RoleManager.ListRoles()
                                        where
                                            !String.Equals(simpleRoleName, "System Administrator",
                                                           StringComparison.InvariantCultureIgnoreCase)
                                        select roleName);
                    break;
                }

                rolesToAdd.Add(roleName);
            }

            RoleManager.AddUserToRoles(userName, rolesToAdd.ToArray());
        }

        /// <summary>
        /// Update a user's group memberships to add them to new groups
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userExists"></param>
        /// <param name="groupsToJoin"></param>
        private void UpdateGroupMemberships(string userName, bool userExists, List<int> groupsToJoin)
        {
            groupsToJoin = new List<int>(groupsToJoin); //clone the list to avoid target groups removal
            if (userExists)
            {
                List<Group> groupsMemberships = GroupManager.GetGroupMemberships(userName);

                foreach (Group group in groupsMemberships)
                {
                    if (groupsToJoin.Contains(group.ID.Value))
                    {
                        groupsToJoin.Remove(group.ID.Value);
                    }
                }
            }

            //Add user to the group.  Cache groups and save all at once for better performance.
            var groupCache = TempGroupCache;
            bool needToInvalidateCache = false;

            foreach (int groupId in groupsToJoin)
            {
                if (!groupCache.ContainsKey(groupId))
                {
                    Group group = GroupManager.GetGroup(groupId);

                    if (group != null)
                    {
                        groupCache[groupId] = group;
                    }
                }

                if (groupCache.ContainsKey(groupId))
                {
                    var principal = UserManager.GetUserPrincipal(userName);
                    if (principal != null)
                        groupCache[groupId].AddUser(principal);
                    
                    needToInvalidateCache = true;
                }
            }

            if (needToInvalidateCache)
            {
                GroupManager.InvalidateUserMemberships(userName);
            }

            TempGroupCache = groupCache;
        }

        /// <summary>
        /// Remove survey editor roles from the specified list.
        /// </summary>
        /// <param name="rolesList"></param>
        private List<string> RemoveEditorRoles(IEnumerable<string> rolesList)
        {
            //Create new list of roles
            return (from roleName in rolesList
                    where !roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase)
                    let role = RoleManager.GetRole(roleName)
                    where role != null && !role.HasPermission("Form.Edit") && !role.HasPermission("Analysis.Administer")
                    select roleName).ToList();

        }


        /// <summary>
        /// Get whether the survey editor role was selected
        /// </summary>
        private bool IsSurveyEditorRoleSelected(IEnumerable<string> roleList)
        {
            foreach (string userRole in roleList)
            {
                if (userRole.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                Role r = RoleManager.GetRole(userRole);

                if (r.HasPermission("Form.Edit") || r.HasPermission("Analysis.Administer"))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Temporary cache of user groups to eliminate need to constantly load/save groups.
        /// </summary>
        private Dictionary<int, Group> TempGroupCache
        {
            get
            {
                return Session["TempGroupCache"] as Dictionary<int, Group> ??
                       new Dictionary<int, Group>();
            }
            set { Session["TempGroupCache"] = value; }
        }



        /// <summary>
        /// Get list of import row errors
        /// </summary>
        protected List<List<string>> UsersToLoad
        {
            get { return Session["ImportUsersUsersToLoad"] as List<List<string>> ?? new List<List<string>>(); }
            set { Session["ImportUsersUsersToLoad"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> SelectedUserFields
        {
            get { return Session["SelectedUserFields"] as List<string> ?? new List<string>(); }
            set { Session["SelectedUserFields"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> SelectedUserRoles
        {
            get { return Session["SelectedUserRoles"] as List<string> ?? new List<string>(); }
            set { Session["SelectedUserRoles"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<int> SelectedUserGroups
        {
            get { return Session["SelectedUserGroups"] as List<int> ?? new List<int>(); }
            set { Session["SelectedUserGroups"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected long? SurveyEditorLimit
        {
            get { return Session["SurveyEditorLimit"] as long?; }
            set { Session["SurveyEditorLimit"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected long? StartingSurveyEditorCount
        {
            get { return Session["StartingSurveyEditorCount"] as long?; }
            set { Session["StartingSurveyEditorCount"] = value; }
        }
    }
}