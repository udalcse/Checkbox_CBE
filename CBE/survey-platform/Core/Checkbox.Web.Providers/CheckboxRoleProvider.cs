using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;

namespace Checkbox.Web.Providers
{
    /// <summary>
    /// Implementation of security role provider that fits into ASP.NET provider model.  To maintain
    /// compatibility with non web application uses of Checkbox, role functionality is implemented in
    /// the Checkbox.Security.Providers.BaseRoleProvider class and this class just proxies calls to
    /// that provider.
    /// </summary>
    public class CheckboxRoleProvider : RoleProvider, ICheckboxRoleProvider
    {
        private const string ROLE_PROVIDER_DATABASE_NAME = "CheckboxRoleProvider";

        private readonly CacheManager _identityRoleCache;
        private readonly List<string> _roleNames;

        private readonly object _lockObject = new object();

        /// <summary>
        /// Get/set application name
        /// </summary>
        public override string ApplicationName { get; set; }

        public CheckboxRoleProvider()
        {
            _roleNames = new List<string>();

            //Create identity role cache, if necessary
            //KTK: Commented out app setting check to get install working
            //TODO: Figure out the correct way to handle this app setting check during installation when the database has not yet been set up
            //if (ApplicationManager.AppSettings.CacheIdentityRoles)
            //{
                //Attempt to create the role cache
                _identityRoleCache = CacheFactory.GetCacheManager("identityRolesCacheManager");

                if (_identityRoleCache == null)
                {
                    throw new Exception("Unable to initialize BaseRoleProvider.  Identity role caching is enabled in application settings, but IdentityRolesCache could not be created.");
                }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        public override void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            //Next, add users to roles
            foreach (string userName in userNames)
            {
                foreach (string roleName in roleNames)
                {
                    Database db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_AddToRole");
                    command.AddInParameter("UniqueIdentifier", DbType.String, userName);
                    command.AddInParameter("RoleName", DbType.String, roleName);

                    db.ExecuteNonQuery(command);

                    if (ApplicationManager.AppSettings.CacheIdentityRoles)
                    {
                        lock (_lockObject)
                        {
                            List<string> userRoles = new List<string>(GetRolesForUser(userName));

                            if (!userRoles.Contains(roleName))
                            {
                                userRoles.Add(roleName);

                                _identityRoleCache.Add(userName.ToLower(), userRoles);
                            }
                        }
                    }
                }
                var currentUser = HttpContext.Current.User as CheckboxPrincipal;
                string modifier = currentUser != null ? currentUser.Identity.Name : userName;

                UserManager.SetUserModifier(userName, modifier);
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="roleName"></param>
        public override void CreateRole(string roleName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns></returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Find  users in a specified role.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="usernameToMatch"></param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (Utilities.IsNullOrEmpty(roleName)
               || Utilities.IsNullOrEmpty(usernameToMatch))
            {
                return new string[] { };
            }

            Database db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Role_FindUsers");
            command.AddInParameter("RoleName", DbType.String, roleName);
            command.AddInParameter("UniqueIdentifier", DbType.String, usernameToMatch);

            List<string> userList = new List<string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        userList.Add(DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userList.ToArray();
        }

        /// <summary>
        /// List all roles
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            if (_roleNames.Count == 0)
            {
                lock (_lockObject)
                {
                    //Add second check to prevent case where continue was true before lock, but not after
                    if (_roleNames.Count == 0)
                    {
                        Database db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Role_ListAll");

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    _roleNames.Add(DbUtility.GetValueFromDataReader(reader, "RoleName", string.Empty));
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
            }

            return _roleNames.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public override string[] GetRolesForUser(string userUniqueIdentifier)
        {
            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                return new string[] { };
            }

            //First, check cache if enabled
            if (ApplicationManager.AppSettings.CacheIdentityRoles
                && _identityRoleCache.Contains(userUniqueIdentifier.ToLower()))
            {
                return ((List<string>)_identityRoleCache[userUniqueIdentifier.ToLower()]).ToArray();
            }

            //Otherwise, load roles
            var db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);

            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Role_ListForUser");
            command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier.Replace("'", "&#39;"));

            var roleList = new List<string>();

            IDataReader reader = null;
            try
            {
                reader = db.ExecuteReader(command);
                while (reader.Read())
                {
                    roleList.Add(DbUtility.GetValueFromDataReader(reader, "RoleName", string.Empty));
                }
                reader.Close();
            }
            catch (SqlException)
            {
                //if there is no 'ckbx_sp_Role_ListForUser' function. Lets try to find 'ckbx_Role_ListForUser'
                command = db.GetStoredProcCommandWrapper("ckbx_Role_ListForUser");
                command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier.Replace("'", "&#39;"));

                reader = db.ExecuteReader(command);
                while (reader.Read())
                {
                    roleList.Add(DbUtility.GetValueFromDataReader(reader, "RoleName", string.Empty));
                }
                reader.Close();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            //Cache data, if enabled
            if (ApplicationManager.AppSettings.CacheIdentityRoles)
            {
                _identityRoleCache.Add(userUniqueIdentifier.ToLower(), roleList);
            }

            return roleList.ToArray();
        }


        /// <summary>
        /// List the users in a specific role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override string[] GetUsersInRole(string roleName)
        {
            if (Utilities.IsNullOrEmpty(roleName))
            {
                return new string[] { };
            }

            Database db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Role_ListUsersInRole");
            command.AddInParameter("RoleName", DbType.String, roleName);

            List<string> userList = new List<string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        userList.Add(DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userList.ToArray();
        }

        /// <summary>
        /// Return a boolean indicating if a user is in a given role.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override bool IsUserInRole(string userName, string roleName)
        {
            if (Utilities.IsNullOrEmpty(roleName)
              || Utilities.IsNullOrEmpty(userName))
            {
                return false;
            }

            //Get user roles
            string[] userRoles = GetRolesForUser(userName);

            //Check for membership
            return userRoles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Remove users from the specified files
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
        {
            Database db = DatabaseFactory.CreateDatabase(ROLE_PROVIDER_DATABASE_NAME);

            foreach (string userName in userNames)
            {
                foreach (string roleName in roleNames)
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_RemoveFromRole");
                    command.AddInParameter("UniqueIdentifier", DbType.String, userName);
                    command.AddInParameter("RoleName", DbType.String, roleName);
                    db.ExecuteNonQuery(command);

                    if (ApplicationManager.AppSettings.CacheIdentityRoles)
                    {
                        lock (_lockObject)
                        {
                            if (_identityRoleCache.Contains(userName.ToLower()))
                            {
                                List<string> userRoles = (List<string>)_identityRoleCache[userName.ToLower()];

                                if (userRoles.Contains(roleName))
                                {
                                    userRoles.Remove(roleName);
                                }

                                _identityRoleCache.Add(userName.ToLower(), userRoles);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if a role exists.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override bool RoleExists(string roleName)
        {
            if (Utilities.IsNullOrEmpty(roleName))
            {
                return false;
            }

            //Get role names
            string[] roleNames = GetAllRoles();

            //Check for existence
            return roleNames.Contains(roleName, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
