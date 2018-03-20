////===============================================================================
//// Prezza Technologies Checkbox
//// Copyright © Checkbox Survey Inc  All rights reserved.
////===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Globalization.Text;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Management.Licensing;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Caching;

namespace Checkbox.Security
{
    /// <summary>
    /// Provides interface accessing user role data
    /// </summary>
    public static class RoleManager
    {
        private static ICheckboxRoleProvider _roleProvider;
        private static CheckboxLicense _license;

        private static CacheManager _rolePermissionsCacheManager;
        private static CacheManager _roleIdsCacheManager;

        /// <summary>
        /// Initialize the manager with the role provider to use.
        /// </summary>
        /// <param name="roleProvider"></param>
        public static void Initialize(ICheckboxRoleProvider roleProvider)
        {
            //Probably  not necessary, but add to prevent double initialization
            lock (typeof(RoleManager))
            {
                if (_roleProvider == null)
                {
                    _roleProvider = roleProvider;
                }

                _roleIdsCacheManager = CacheFactory.GetCacheManager("roleIds");
                if (ApplicationManager.AppSettings.CacheRolePermissions)
                {
                    _rolePermissionsCacheManager = CacheFactory.GetCacheManager("rolePermissions");
                }

                var licenseProvider = new CheckboxLicenseProvider();
                _license = licenseProvider.GetLicense() as CheckboxLicense;
            }
        }

        /// <summary>
        /// Get a reference to the profile provider.
        /// </summary>
        private static ICheckboxRoleProvider Provider
        {
            get
            {
                if (_roleProvider == null)
                {
                    throw new Exception("Role Manager has not been initialized with a Checkbox role provider.");
                }

                return _roleProvider;
            }
        }

        /// <summary>
        /// Get all Simple Security roles
        /// </summary>
        /// <returns></returns>
        public static List<string> ListSimpleSecurityRoles()
        {
            var roles = new List<string>
                            {
                                TextManager.GetText("/simpleSecurity/systemAdministrator"),
                                TextManager.GetText("/simpleSecurity/surveyAdministrator"),
                                TextManager.GetText("/simpleSecurity/respondent")
                            };
            return roles;
        }

        /// <summary>
        /// Gets all available roles
        /// </summary>
        /// <returns></returns>
        public static List<string> ListRoles()
        {
            return new List<string>(Provider.GetAllRoles());
        }

		/// <summary>
		/// Gets all available roles
		/// </summary>
		/// <returns></returns>
		public static List<string> ListRoles(bool includeSysAdmin)
        {
			string[] roles = Provider.GetAllRoles();

            List<string> result = new List<string>(roles.Length);

			Array.ForEach<string>(roles, (role) => AddRole(result, role, !includeSysAdmin));

			return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="role"></param>
        /// <param name="skipSysAdmin"></param>
		private static void AddRole(List<string> roles, string role, bool skipSysAdmin)
		{
			if (skipSysAdmin && role == "System Administrator")
				return;

			roles.Add(role);
		}

        /// <summary>
        /// List roles for a specific user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string[] ListRolesForUser(string userName)
        {
            string[] userRoles = Provider.GetRolesForUser(userName);

            //If user has no specific roles, apply system default roles
            if (userRoles.Length == 0)
            {
                return ApplicationManager.AppSettings.DefaultRolesForUnAuthenticatedNetworkUsers;
            }

            return userRoles;
        }

        /// <summary>
        /// List roles for a specific user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string[] ListNonDefaultRolesForUser(string userName)
        {
            return Provider.GetRolesForUser(userName);
        }

        /// <summary>
        /// Checks that the user doesn't have any roles except Respondent
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsJustARespondent(string userName)
        {
            string[] roles = ListRolesForUser(userName);
            foreach (var r in roles)
            {
                if (!"Respondent".Equals(r, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Factory method to create an instance of a given role.
        /// </summary>
        /// <param name="roleName">Role name to instantiate.</param>
        /// <returns>Name of roll.</returns>
        public static Role GetRole(string roleName)
        {
            List<string> rolePermissions;

            if (ApplicationManager.AppSettings.CacheRolePermissions
                && _rolePermissionsCacheManager.Contains(GetRoleCacheKey(roleName)))
            {
                rolePermissions = _rolePermissionsCacheManager.GetData(GetRoleCacheKey(roleName)) as List<string>;
            }
            else
            {
                rolePermissions = new List<string>();

                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Role_GetPermissions");
                command.AddInParameter("RoleName", DbType.String, roleName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            rolePermissions.Add(DbUtility.GetValueFromDataReader(reader, "PermissionName", string.Empty));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                if (ApplicationManager.AppSettings.CacheRolePermissions)
                {
                    _rolePermissionsCacheManager.Add(GetRoleCacheKey(roleName), rolePermissions);
                }
            }

            //Return the new role
            return new Role(rolePermissions.ToArray())
            {
                Name = roleName,
                Id = GetRoleId(roleName)
            };
        }

        /// <summary>
        /// Get a role cache key name from role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private static string GetRoleCacheKey(string roleName)
        {
            return (ApplicationManager.ApplicationDataContext + "_" + roleName).ToLower();
        }

        /// <summary>
        /// Get the ID of the named role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private static int GetRoleId(string roleName)
        {
            if (_roleIdsCacheManager.Contains(roleName))
            {
                return (int)_roleIdsCacheManager.GetData(roleName);
            }
            

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Role_GetId");
            command.AddInParameter("RoleName", DbType.String, roleName);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        if (reader["RoleID"] != DBNull.Value)
                        {
                            _roleIdsCacheManager.Add(roleName, (int)reader["RoleID"]);
                            return (int)reader["RoleID"];
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return -1;
        }

        /// <summary>
        /// Determine if the specified identiy has a role with the specified permission.
        /// </summary>
        /// <param name="userIdentity">Identity to check role permissions for.</param>
        /// <param name="permission">Permissiont to check.</param>
        public static bool UserHasRoleWithPermission(string userIdentity, string permission)
        {
            ArgumentValidation.CheckForEmptyString(permission, "permission");

            string[] roleNames = ListRolesForUser(userIdentity);

            return roleNames.Select(GetRole).Any(r => r != null && r.HasPermission(permission));
        }

        /// <summary>
        /// Add users to a specific role
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="userRoles"></param>
        public static void AddUserToRoles(string uniqueIdentifier, string[] userRoles)
        {
            string message;
            if (!CheckLicenseForAddUserToRoles(uniqueIdentifier, userRoles, out message))
            {
                if(!string.IsNullOrEmpty(message))
                    throw new CheckboxLicenseException(message);

                return;
            }

            Provider.AddUsersToRoles(new[] { uniqueIdentifier }, userRoles);
            
            //Ensure user updated
            UserManager.ExpireCachedPrincipal(uniqueIdentifier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="userRoles"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool CheckLicenseForAddUserToRoles(string uniqueIdentifier, IEnumerable<string> userRoles, out string message)
        {
            message = null;

            //check license existing
            SurveyEditorLimit limit;
            if ((limit = _license.SurveyEditorLimit) == null)
                return true;

            //check if user want admin role
            bool wantAdminRole = false;
            foreach (var roleName in userRoles)
            {
                if (roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase))
                {
                    wantAdminRole = true;
                    break;
                }

                Role role = GetRole(roleName);
                if (role != null && (role.HasPermission("Form.Edit") || role.HasPermission("Analysis.Administer")))
                {
                    wantAdminRole = true;
                    break;
                }
            }
            if (!wantAdminRole)
                return true;
            
            //check if user already in admin role
            bool isAdmin = false;
            string[] userToEditRoles = ListRolesForUser(uniqueIdentifier);

            foreach (string roleName in userToEditRoles)
            {
                if (roleName.Equals("System Administrator", StringComparison.InvariantCultureIgnoreCase))
                {
                    isAdmin = true;
                    break;
                }

                Role role = GetRole(roleName);
                if (role != null && (role.HasPermission("Form.Edit") || role.HasPermission("Analysis.Administer")))
                {
                    isAdmin = true;
                    break;
                }
            }
            if (isAdmin)
                return true;

            //check license validation
            LimitValidationResult result = limit.Validate(out message);
            if (result != LimitValidationResult.LimitNotReached)
                return false;

            return true;
        }

        /// <summary>
        /// Add users to a specific role
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="userRoles"></param>
        public static void RemoveUserFromRoles(string uniqueIdentifier, string[] userRoles)
        {
            Provider.RemoveUsersFromRoles(new[] { uniqueIdentifier }, userRoles);

            //Ensure user updated
            UserManager.ExpireCachedPrincipal(uniqueIdentifier);
        }

        /// <summary>
        /// List users in a given role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string[] GetUsersInRole(string role)
        {
            return Provider.GetUsersInRole(role);
        }
    }
}
