using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using Checkbox.Common;
using Checkbox.Management;
using Prezza.Framework.Caching;
using Prezza.Framework.Logging;

namespace Checkbox.Web.Providers
{
    public class ActiveDirectoryRoleProvider : RoleProvider
    {
        private CacheManager _adRolesCache;
        private Dictionary<string, List<string>> _groupToRoleMappings;

        /// <summary>
        /// Get/set application name
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Get/set whether to cache active director roles.
        /// </summary>
        private bool CacheAdRoles { get; set; }

        /// <summary>
        /// Get/set domain distinguishing name.
        /// </summary>
        private string DomainDn { get; set; }

        /// <summary>
        /// Get/set user name for connection to ad
        /// </summary>
        private string ConnectionUserName { get; set; }

        /// <summary>
        /// Get/set connection password
        /// </summary>
        private string ConnectionPassword { get; set; }

        /// <summary>
        /// Specify whether AD users with no default roles specified 
        /// </summary>
        private bool UseDefaultRoles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(String name, NameValueCollection config)
        {
            //Base method
            base.Initialize(name, config);


            //Get setting controlling caching of role membership
             if(Utilities.IsNotNullOrEmpty(config["cacheADRoles"])
                && "false".Equals(config["cacheADRoles"], StringComparison.InvariantCultureIgnoreCase))
            {
                CacheAdRoles = false;
            }
            else
            {
                CacheAdRoles = true;
                
                //Attempt to create the role cache
                _adRolesCache = CacheFactory.GetCacheManager("adIdentityRolesCacheManager");

                if (_adRolesCache == null)
                {
                    throw new Exception("Unable to initialize ActiveDirectoryRoleProvider.  Identity role caching is enabled in web.config, but adIdentityRolesCacheManager could not be created.");
                }
            }

            //Get domain distinguished name for this provider
            if(Utilities.IsNullOrEmpty(config["domainDn"]))
            {
                throw new Exception("domainDn parameter was not specified in Active Directory Role Provider configuration.");
            }

            DomainDn = config["domainDn"];
            ConnectionUserName = config["connectionUsername"];
            ConnectionPassword = config["connectionPassword"];

            //Get whether to use default roles for AD users that have no mapped roles in checkbox
            UseDefaultRoles = "true".Equals(config["applyDefaultRolesToUsersWithNoMappedRoles"], StringComparison.InvariantCultureIgnoreCase);

            //Parse out the ad groups to Checkbox roles mappings
            _groupToRoleMappings = new Dictionary<string,List<string>>(StringComparer.InvariantCultureIgnoreCase);

            ParseGroupToRoleMappings(config["roleMappings"]);
        }

        /// <summary>
        /// Parse mappings between groups and roles
        /// </summary>
        /// <param name="roleMappings"></param>
        private void ParseGroupToRoleMappings(string roleMappings)
        {
            if (Utilities.IsNullOrEmpty(roleMappings))
            {
                return;
            }

            //Mappings are in format:  group1=role1,group2=role2, etc.
            string[] splitMappings = roleMappings.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            //For each split mapping, split group = role portion
            foreach(string splitMapping in splitMappings)
            {
                string[] mapping = splitMapping.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                if(mapping.Length == 2
                    && Utilities.IsNotNullOrEmpty(mapping[0])
                    && Utilities.IsNotNullOrEmpty(mapping[1]))
                {
                    //Add group to role mapping to dictionary
                    if (!_groupToRoleMappings.ContainsKey(mapping[0].Trim()))
                    {
                        _groupToRoleMappings[mapping[0].Trim()] = new List<string>();
                    }

                    _groupToRoleMappings[mapping[0].Trim()].Add(mapping[1].Trim());
                }
            }
        }

        /// <summary>
        /// Retrieve listing of all roles to which a specified user belongs.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>String array of roles</returns>
        public override String[] GetRolesForUser(String userName)
        {
             if (Utilities.IsNullOrEmpty(userName))
            {
                return new string[] { };
            }

            //First, check cache if enabled
            if (CacheAdRoles
                && _adRolesCache.Contains(userName.ToLower()))
            {
                return ((List<string>)_adRolesCache[userName.ToLower()]).ToArray();
            }
            
            //Build list of AD groups user belongs to
            var adGroups = new List<string>();

            //Get user principal
            using (var context = new PrincipalContext(ContextType.Domain, null, DomainDn, ContextOptions.SimpleBind, ConnectionUserName, ConnectionPassword))
            {
                try
                {
                    var p = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
                    
                    if(p == null)
                        throw new Exception("Could not find user principal for [" + userName + "] in provided container.");

                    List<Principal> groups;
                    try
                    {
                        groups = p.GetAuthorizationGroups().ToList(); // this will get ALL security groups, distribution groups will not be 
                        groups.AddRange(p.GetGroups());      // this will get security groups of which the user is a direct member and ALL distribution groups
                    }
                    catch (System.IO.FileNotFoundException) //swallow this exception and continue -- http://support.microsoft.com/kb/823196
                    {
                        groups = p.GetAuthorizationGroups().ToList(); // this will get ALL security groups, distribution groups will not be 
                        groups.AddRange(p.GetGroups());      // this will get security groups of which the user is a direct member and ALL distribution groups
                    }

                    adGroups.AddRange(from GroupPrincipal @group in groups where Utilities.IsNotNullOrEmpty(@group.SamAccountName) select @group.SamAccountName);
                }
                catch (Exception ex)
                {
                    //Do nothing for error.  Since providers can be chained, we don't necessarily want to throw errors
                    // when a user can't be found, etc.
                    Logger.Write(
                        "ActiveDirectoryRoleProvider was unable to determine user AD Group Memberships for [" + userName + "].  Error was: " + ex.Message,
                        "Warning",
                        1,
                        1000,
                        Severity.Warning);
                }
            }

            //Map ad groups to checkbox roles
            List<string> checkboxRoles = MapAdGroupsToCheckboxGroups(adGroups.Distinct());

            //If no AD groups map to checkbox roles, see if default roles for network users should apply
            if (checkboxRoles.Count == 0
                && UseDefaultRoles)
            {
                checkboxRoles.AddRange(ApplicationManager.AppSettings.DefaultRolesForUnAuthenticatedNetworkUsers);
            }

            //Cache roles
            if (CacheAdRoles)
            {
                _adRolesCache.Add(userName.ToLower(), checkboxRoles);
            }

            //Return values
            return checkboxRoles.ToArray();
        }

        /// <summary>
        /// For a list of ad groups, return a list of corresponding checkbox roles.  Multiple
        /// groups can map to the same role.
        /// </summary>
        /// <param name="adGroups"></param>
        /// <returns></returns>
        private List<string> MapAdGroupsToCheckboxGroups(IEnumerable<string> adGroups)
        {
            //Now attempt to map ad groups to checkbox groups
            var checkboxRoles = new List<string>();

            foreach(string adGroup in adGroups)
            {
                if(_groupToRoleMappings.ContainsKey(adGroup))
                {
                    checkboxRoles.AddRange(_groupToRoleMappings[adGroup]);
                }
            }

            return checkboxRoles;
        }

        /// <summary>
        /// Map a checkbox role to ad group(s). Multiple groups can map to the same role.
        /// </summary>
        /// <param name="checkboxRole"></param>
        /// <returns></returns>
        private IEnumerable<string> MapCheckboxRoleToAdGroups(string checkboxRole)
        {
            return _groupToRoleMappings.Keys.Where(key => _groupToRoleMappings[key].Contains(checkboxRole)).ToList();
        }

        /// <summary>
        /// Retrieve listing of all users in a specified role.
        /// </summary>
        /// <param name="roleName">String array of users</param>
        /// <returns></returns>
        public override String[] GetUsersInRole(String roleName)
        {
            //Check for empty name
            if(Utilities.IsNullOrEmpty(roleName))
            {
                return new string[] {};
            }

            //Map checkbox role to ad groups
            IEnumerable<string> adGroups = MapCheckboxRoleToAdGroups(roleName);

            //Now list users in ad groups
            var userList = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, null, DomainDn, ContextOptions.SimpleBind, ConnectionUserName, ConnectionPassword))
            {
                try
                {
                    foreach(string adGroup in adGroups)
                    {
                        GroupPrincipal p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, adGroup);
                        var users = p.GetMembers(true);

                        foreach(var user in users)
                        {
                            if(!userList.Contains(user.SamAccountName))
                            {
                                userList.Add(user.SamAccountName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Do nothing for error.  Since providers can be chained, we don't necessarily want to throw errors
                    // when a user can't be found, etc.
                    //throw new ProviderException("An error occurred when the Active Directory Role Provider attempted to query Active Directory to list users in groups.", ex);
                }
            }
         
            return userList.ToArray();
        }

        /// <summary>
        /// Determine if a specified user is in a specified role.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns>Boolean indicating membership</returns>
        public override bool IsUserInRole(string userName, string roleName)
        {
            var usersInRole = new List<string>(GetUsersInRole(roleName));

            return usersInRole.Contains(userName);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="rolename"></param>
        /// <returns></returns>
        public override bool RoleExists(string rolename)
        {
           throw new NotSupportedException();
        }

        /// <summary>
        /// Return sorted list of usernames like usernameToMatch in rolename
        /// </summary>
        /// <param name="roleName">Role to check</param>
        /// <param name="usernameToMatch">Partial username to check</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            //Check for empty role or user name.  In these cases, return zero length
            // array.
            if(Utilities.IsNullOrEmpty(roleName))
            {
                return new string[] {};
            }

            if(Utilities.IsNullOrEmpty(usernameToMatch))
            {
                return new string[] {};
            }
            
            //Build list of matched users
            string[] usersInRole = GetUsersInRole(roleName);

            //Return values.
            return usersInRole.Where(user => user.ToLower().Contains(usernameToMatch.ToLower())).ToArray();
        }
        

        #region NonSupported Base Class Functions
        /// <summary>
        /// AddUsersToRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// CreateRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void CreateRole(string rolename)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// DeleteRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// RemoveUsersFromRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
