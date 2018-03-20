using System;
using System.Collections.Generic;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Security.Providers;
using Prezza.Framework.Caching;

namespace Checkbox.Web.Providers
{
    /// <summary>
    /// Role provider implementation that supports proxying calls to other providers 
    /// (chaining) in a specific order.
    /// </summary>
    public class ChainingRoleProvider : RoleProvider, ICheckboxRoleProvider
    {
        private List<string> _providerList;

        private CacheManager _identityRoleCache;

        private readonly object _lockObject = new object();

        /// <summary>
        /// Get/set whether to cache identity roles or not.  This behavior is controlled
        /// by setting on this provider's provider element in web.config.
        /// </summary>
        private bool CacheIdentityRoles { get; set; }


        /// <summary>
        /// Flag to indicate specified providers have been verified.
        /// </summary>
        private bool ProvidersVerified { get; set; }

        /// <summary>
        /// Get list of providers in "chain"
        /// </summary>
        private List<string> ProviderList
        {
            get
            {
                if (_providerList == null)
                {
                    _providerList = new List<string>();
                }

                return _providerList;
            }
        }

        /// <summary>
        /// Get/set name of checkbox-specfific role provider
        /// </summary>
        private string CheckboxRoleProviderName { get; set; }

        /// <summary>
        /// Get a reference to the Checkbox role Provider
        /// </summary>
        public RoleProvider CheckboxProvider { get { return Roles.Providers[CheckboxRoleProviderName]; } }


        /// <summary>
        /// Get/set application n ame
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Initialize provider w/configuration settings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
            
            //Store name of checkbox-specific provider, which is used for most operations
            CheckboxRoleProviderName = config["checkboxMembershipProvider"];

            //Get list of other chained providers
            if (Utilities.IsNotNullOrEmpty(config["chainedProviders"]))
            {
                ProviderList.AddRange(config["chainedProviders"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
            }

            if (!ProviderList.Contains(CheckboxRoleProviderName))
            {
                ProviderList.Insert(0, CheckboxRoleProviderName);
            }

            //Initialize cache
            if (Utilities.IsNotNullOrEmpty(config["cacheIdentityRoles"])
               && "false".Equals(config["cacheIdentityRoles"], StringComparison.InvariantCultureIgnoreCase))
            {
                CacheIdentityRoles = false;
            }
            else
            {
                CacheIdentityRoles = true;

                //Attempt to create the role cache
                _identityRoleCache = CacheFactory.GetCacheManager("identityRolesCacheManager");

                if (_identityRoleCache == null)
                {
                    throw new Exception("Unable to initialize CheckboxRoleProvider.  Identity role caching is enabled in web.config, but IdentityRolesCache could not be created.");
                }
            }
        }

        /// <summary>
        /// Ensure
        /// </summary>
        private void EnsureProviders()
        {
            if (!ProvidersVerified)
            {
                foreach (string providerName in ProviderList)
                {
                    if (Roles.Providers[providerName] == null)
                    {
                        throw new Exception("Chained role provider [" + providerName + "] specified in web.config could not be found.");
                    }
                }

                ProvidersVerified = true;
            }
        }

        /// <summary>
        /// Add users to a rule, using Checkbox-specific role provider.
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        public override void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            EnsureProviders();

            CheckboxProvider.AddUsersToRoles(userNames, roleNames);

            if (CacheIdentityRoles)
            {
                foreach (string userName in userNames)
                {
                    foreach (string roleName in roleNames)
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
            }
        }

        /// <summary>
        /// Creating roles not supported.
        /// </summary>
        /// <param name="roleName"></param>
        public override void CreateRole(string roleName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Deleting roles not supported.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns></returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// List users in a given role.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userNameToMatch"></param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string roleName, string userNameToMatch)
        {
            EnsureProviders();

            List<string> roleUsers = new List<string>();

            foreach (string providerName in ProviderList)
            {
                roleUsers.AddRange(Roles.Providers[providerName].FindUsersInRole(roleName, userNameToMatch));
            }

            return roleUsers.ToArray();
        }

        /// <summary>
        /// List all roles reported by Checkbox provider
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            EnsureProviders();

            return CheckboxProvider.GetAllRoles();
        }

        /// <summary>
        /// Find all roles the specified user is in.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override string[] GetRolesForUser(string userName)
        {
            EnsureProviders();

            //Check for null/empty  user name
            if (Utilities.IsNullOrEmpty(userName))
            {
                return new string[] { };
            }

            //First, check cache if enabled
            if (CacheIdentityRoles
                && _identityRoleCache.Contains(userName.ToLower()))
            {
                return ((List<string>)_identityRoleCache[userName.ToLower()]).ToArray();
            }

            string[] providerRoles = new string[] {};

            //Search providers for first match and return
            foreach (string providerName in ProviderList)
            {
                providerRoles = Roles.Providers[providerName].GetRolesForUser(userName);

                if (providerRoles.Length > 0)
                {
                    break;
                }
            }

            //Cache data, if enabled
            if (CacheIdentityRoles
                && providerRoles.Length > 0)
            {
                _identityRoleCache.Add(userName.ToLower(), new List<string>(providerRoles));
            }

            //Return list
            return providerRoles;
        }

        /// <summary>
        /// Get users in the specified role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override string[] GetUsersInRole(string roleName)
        {
            EnsureProviders();

            List<string> roleUsers = new List<string>();

            foreach (string providerName in ProviderList)
            {
                roleUsers.AddRange(Roles.Providers[providerName].GetUsersInRole(roleName));
            }

            return roleUsers.ToArray();
        }

        /// <summary>
        /// Determine if a user is in a given role for any of the configured
        /// role providers.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override bool IsUserInRole(string userName, string roleName)
        {
            EnsureProviders();

            //Search providers for first match and return
            foreach (string providerName in ProviderList)
            {
                if (Roles.Providers[providerName].IsUserInRole(userName, roleName))
                {
                    return true;
                }
            }

            //No matcheds found, return false.
            return false;
        }

        /// <summary>
        /// Remove Checkbox users from a role.
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
        {
            EnsureProviders();

            CheckboxProvider.RemoveUsersFromRoles(userNames, roleNames);
        }

        /// <summary>
        /// Use checkbox provider to determine if a role exists.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override bool RoleExists(string roleName)
        {
            EnsureProviders();

            return CheckboxProvider.RoleExists(roleName);
        }
    }
}
