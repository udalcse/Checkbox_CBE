//===============================================================================
// Prezza Technologies Application Framework
// Copyright Â© Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Collections;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Checkbox.Configuration;
using Checkbox.Security.Principal;
using Checkbox.Security.Providers;
using Checkbox.Users.Data;
using Prezza.Framework.Data;
using Prezza.Framework.Caching;
using Prezza.Framework.Logging;
using Prezza.Framework.Security;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.Caching.Expirations;
using Checkbox.Common;
using Checkbox.Security;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Forms.Validation;
using System.Web.Security;
using Checkbox.Forms.Security.Principal;
using Checkbox.Timeline;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Users
{
    /// <summary>
    ///  Manage application interaction with user objects and mangage state information for application users.
    /// </summary>
    public static class UserManager
    {
        //TODO: Enable/Disable principal caching by app setting

        /// <summary>
        /// Authentication type indicating user is authenticated outside of Checkbox, such as with
        /// an IIS authenticated network user, AD user, or other user.
        /// </summary>
        public const string EXTERNAL_USER_AUTHENTICATION_TYPE = "ExternalUser";                                            //Indicates user external to Checkbox, such as AD or other users.
        //Checkbox is able to update profile/roles for such users but not
        // intrinsic properties.
        /// <summary>
        /// User is a "Network User" added to Checkbox user store.
        /// </summary>
        public const string NETWORK_USER_AUTHENTICATION_TYPE = "CheckboxNetworkUser";                                      //Checkbox style Network User

        /// <summary>
        /// User is a standard Checkbox user.
        /// </summary>
        public const string PASSWORD_USER_AUTHENTICATION_TYPE = "CheckboxPasswordUser";                                    //Regular name/password user

        //Cache of user principals to avoid repeated calls to get users and profiles that will would add
        // significant "chatter" to database.
        private static CacheManager _principalCache;
        private static CacheManager _loginCacheManager;
        private static List<FileInfo> _tourMessages;

        private static int _authTimeout;

        private static ICheckboxMembershipProvider _membershipProvider;

        private static readonly object _lockObject = new object();

        /// <summary>
        /// Error code indicating an attempt to create/rename a user with an existing name
        /// </summary>
        public const string ERROR_USER_NOT_UNIQUE = "USERNOTUNIQUE";

        /// <summary>
        /// Initialize the user manager.  As part of the initialization process, caches for security tokens, logged-in 
        /// user contexts, and user session tokens (which provide mappings between security tokens and user contexts)
        /// are created.
        /// 
        /// </summary>
        public static void Initialize(ICheckboxMembershipProvider defaultProvider)
        {
            _membershipProvider = defaultProvider;
            _loginCacheManager = CacheFactory.GetCacheManager("loggedInUserCacheManager");
            _principalCache = CacheFactory.GetCacheManager("userPrincipalCacheManager");
        }

        /// <summary>
        /// Set auth timeout. Generally should be the same as forms auth timeout
        /// </summary>
        /// <param name="timeoutInMinutes"></param>
        public static void SetAuthTimeout(int timeoutInMinutes)
        {
            _authTimeout = timeoutInMinutes;
        }

        /// <summary>
        /// Get membership provider reference
        /// </summary>
        private static ICheckboxMembershipProvider MembershipProvider
        {
            get
            {
                return _membershipProvider;
            }
        }

        /// <summary>
        /// Remove princpal from principal cache
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void ExpireCachedPrincipal(string uniqueIdentifier)
        {
            if (_principalCache.Contains(uniqueIdentifier))
            {
                lock (_lockObject)
                {
                    //Re check in case item removed betweeen first check and lock
                    if (_principalCache.Contains(uniqueIdentifier))
                    {
                        _principalCache.Remove(uniqueIdentifier);
                    }
                }
                try
                {
                    TimelineManager.ClearByPrincipal(uniqueIdentifier);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");
                }
            }

            MembershipProvider.ExpireCachedUserNonProfileProperties(uniqueIdentifier);
        }

        /// <summary>
        /// Get user name from full unique identifier, which may include domain name.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public static string ParseUserName(string userUniqueIdentifier)
        {
            if (string.IsNullOrEmpty(userUniqueIdentifier))
            {
                return string.Empty;
            }

            var splitName = userUniqueIdentifier.Split(new[] {@"\"}, StringSplitOptions.RemoveEmptyEntries);

            if (splitName.Length != 2 || string.IsNullOrEmpty(splitName[1]))
            {
                return userUniqueIdentifier;
            }

            return splitName[1];
        }

        /// <summary>
        /// Return a boolean indicating if a user is a checkbox user
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public static bool IsCheckboxUser(string userUniqueIdentifier)
        {
            var userPrincipal = GetUserPrincipal(userUniqueIdentifier);

            //If user not found or user is not a Checkbox network/password user, do nothing
            return
                userPrincipal != null
                && userPrincipal.Identity != null
                && !EXTERNAL_USER_AUTHENTICATION_TYPE.Equals(userPrincipal.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get user password.  Return empty string if not found
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public static string GetUserPassword(string userUniqueIdentifier)
        {
            return MembershipProvider.GetPassword(ParseUserName(userUniqueIdentifier), string.Empty);
        }

        /// <summary>
        /// Construct and return a principal for the user associated with the specified identity.  Attempts
        /// to find user before returning.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique user identifier.</param>
        /// <returns>CheckboxPrincipal object.</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal GetUserPrincipal(string uniqueIdentifier)
        {
            return GetUserPrincipal(uniqueIdentifier, true);
        }

        /// <summary>
        /// Construct a principal for the user
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public static CheckboxPrincipal GetUserPrincipal(string uniqueIdentifier, bool useCache)
        {
            //Check argument
            if (Utilities.IsNullOrEmpty(uniqueIdentifier))
            {
                return null;
            }

            //Check cache
            if (useCache && _principalCache.Contains(uniqueIdentifier)) 
            {
                var principal = _principalCache[uniqueIdentifier];

                if (principal != null)
                {
                    //Check if user auth type is set to "Forms" which happens behind the scenes with login page.  Remove user from
                    // cache and reload to get proper auth type.
                    if (((CheckboxPrincipal)principal).Identity == null ||
                            ((CheckboxPrincipal)principal).Identity.AuthenticationType.Equals("Forms", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ExpireCachedPrincipal(uniqueIdentifier);
                    }
                    else
                    {
                        return (CheckboxPrincipal)principal;
                    }
                }
            }

            //Get the principal
            return GetUserPrincipal(
                MembershipProvider.GetUserIdentity(ParseUserName(uniqueIdentifier))
                    ?? new GenericIdentity(uniqueIdentifier, EXTERNAL_USER_AUTHENTICATION_TYPE),
                useCache
            );
        }

        /// <summary>
        /// Generate a unique identifier from a user name and domain name
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GenerateUniqueIdentifier(string userName, string domainName)
        {
            return Utilities.IsNullOrEmpty(domainName)
                ? userName
                : domainName + "\\" + userName;
        }

        /// <summary>
        /// Determine if user is a domain user.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public static bool IsDomainUser(string userUniqueIdentifier)
        {
            return !string.IsNullOrEmpty(userUniqueIdentifier) && userUniqueIdentifier.Contains("\\");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public static CheckboxPrincipal GetUserPrincipal(IIdentity userIdentity, bool useCache)
        {
            if (userIdentity == null)
            {
                return null;
            }

            //Check if user auth type is set to "Forms" which happens behind the scenes with login page.  Remove user from
            // cache and reload to get proper auth type.
            if (useCache && _principalCache.Contains(userIdentity.Name))
            {

                if (userIdentity.AuthenticationType.Equals("Forms", StringComparison.InvariantCultureIgnoreCase))
                {
                    ExpireCachedPrincipal(userIdentity.Name);
                }
                else
                {
                    return (CheckboxPrincipal)_principalCache[userIdentity.Name];
                }
            }

            //Create, cache & return principal
            //Pass NULL for profile to defer loading until it is actually used.
            var userPrincipal = new CheckboxPrincipal(
                userIdentity,
                MembershipProvider.GetUserIntrinsicProperties(ParseUserName(userIdentity.Name)),
                RoleManager.ListRolesForUser(userIdentity.Name),
                null);

            //Add user 
            _principalCache.Add(userIdentity.Name, userPrincipal);

            return userPrincipal;
        }

        /// <summary>
        /// Get a GUID associated with the user
        /// </summary>
        /// <param name="uniqueIdentifier">User unique identifier.</param>
        /// <returns>String representation of the guid.</returns>
        public static Guid GetUserGuid(string uniqueIdentifier)
        {
            var intrinsicUserProperties = MembershipProvider.GetUserIntrinsicProperties(ParseUserName(uniqueIdentifier));

            if (intrinsicUserProperties != null
                && intrinsicUserProperties.ContainsKey("GUID"))
            {
                return new Guid(intrinsicUserProperties["GUID"].ToString());
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Get the email address associated with the user
        /// </summary>
        /// <param name="uniqueIdentifier">User unique identifier.</param>
        /// <returns>String representation of the email address.</returns>
        public static string GetUserEmail(string uniqueIdentifier)
        {
            var intrinsicUserProperties = MembershipProvider.GetUserIntrinsicProperties(ParseUserName(uniqueIdentifier));

            if (intrinsicUserProperties != null
                && intrinsicUserProperties.ContainsKey("Email")
                && intrinsicUserProperties["Email"] != null)
            {
                return intrinsicUserProperties["Email"].ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// Get a principal for the first user with a matching email address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static CheckboxPrincipal GetUserWithEmail(string emailAddress)
        {
            int totalRecords;

            if (Utilities.IsNullOrEmpty(emailAddress))
            {
                return null;
            }

            //List users
            var userNameList = MembershipProvider.ListUsersByEmail(emailAddress, 0, int.MaxValue, string.Empty, false, out totalRecords);

            //The list method will return partial matches, so attempt to find exact match.
            return (from userName in userNameList
                    where emailAddress.Equals(GetUserEmail(userName), StringComparison.InvariantCultureIgnoreCase)
                    select GetUserPrincipal(userName)).FirstOrDefault();
        }

        /// <summary>
        /// Get a principal for the first user with a matching name and email address.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static CheckboxPrincipal FindUser(string name, string emailAddress)
        {
            int totalRecords;

            if (Utilities.IsNullOrEmpty(name))
            {
                return GetUserWithEmail(emailAddress);
            }

            //List users
            var userNameList = MembershipProvider.ListUsersByEmail(emailAddress, 0, int.MaxValue, string.Empty, false, out totalRecords);

            //The list method will return partial matches, so attempt to find exact match.
            return (from userName in userNameList
                    where userName.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                    GetUserEmail(userName).Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase)
                    select GetUserPrincipal(userName)).FirstOrDefault();
        }

        /// <summary>
        /// Get a user principal based on the GUID associated with the user.
        /// </summary>
        /// <param name="userGuid">GUID associated with the user.</param>
        /// <returns>CheckboxPrincipal object representing the user, or null if no user is
        /// associated with the specified GUID.</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal GetUserByGuid(Guid userGuid)
        {
            var userName = MembershipProvider.GetUserNameFromGuid(userGuid);

            return Utilities.IsNullOrEmpty(userName) ? null : GetUserPrincipal(userName);
        }

        /// <summary>
        /// Determines if a user already exists with the specified user unique identifier
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public static bool UserExists(string uniqueIdentifier)
        {
            return MembershipProvider.GetUserIdentity(ParseUserName(uniqueIdentifier)) != null;
        }

        /// <summary>
        /// Validates a proposed unique identifier to ensure it meets formatting requirements
        /// - Uniqueidentifiers may not contain the characters $ ' 
        /// </summary>
        /// <param name="proposedUniqueIdentifier"></param>
        /// <returns></returns>
        public static bool ValidateUniqueIdentifierFormat(string proposedUniqueIdentifier)
        {
            const string userNameRegex = @"[^\w @.'-]+?";
            const string AD_userNameRegex = @"[^\w @.'-\\]+?";

            //Check for funky values in user name
            var regex = new Regex(StaticConfiguration.DisableForeighMembershipProviders ? userNameRegex : AD_userNameRegex);
            return !regex.IsMatch(proposedUniqueIdentifier);
        }

        /// <summary>
        /// Set the context for the logged in user.  The context contains information about
        /// what the user is doing and what screens the user is viewing.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="userHostName">Name of the computer the user is accessing Checkbox from.</param>
        /// <param name="userHostAddress">IP address the user is accessing Checkbox from.</param>
        /// <param name="userAgent">User agent string returned by the user's web browser.</param>
        /// <param name="currentContext">Current context of the user within Checkbox.  Typically, this
        /// is path to the aspx page the user is accessing.</param>
        public static void SetUserContext(string userName, string userHostName, string userHostAddress, string userAgent, string currentContext)
        {
            //Replace the item in the cache to refresh it to prevent expiration
            if (_loginCacheManager == null) return;
            var loginInfo =
                new UserLoginInfo(
                    userName,
                    userHostName,
                    userHostAddress,
                    userAgent) { CurrentUrl = currentContext };

            //If the login info is null, build a new one
            _loginCacheManager.Add(userName, loginInfo, CacheItemPriority.Normal, null, new SlidingTime(new TimeSpan(0, 0, _authTimeout, 0)));
        }

        /// <summary>
        /// Return the current logged-in principal.
        /// </summary>
        /// <returns><see cref="CheckboxPrincipal"/> object for the logged-in user.</returns>
        public static CheckboxPrincipal GetCurrentPrincipal()
        {
            return Thread.CurrentPrincipal as CheckboxPrincipal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool CheckPrincipalPermissionForUser(ExtendedPrincipal principal, string uniqueIdentifier, string permission)
        {
            if (Utilities.IsNullOrEmpty(uniqueIdentifier))
            {
                return false;
            }

            if (principal == null)
            {
                return false;
            }

            //if the target user is SysAdmin, it cannot be edited   
            var targetUser = GetUserPrincipal(uniqueIdentifier);
            if (targetUser != null && targetUser.IsInRole("System Administrator") && !principal.IsInRole("System Administrator") &&
                (permission == "Group.Create" || permission == "Group.Delete" || permission == "Group.Edit"))
                return false;

            //Check if current user can edit "Everyone".  If yes, user can be edited.
            if (AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, GroupManager.GetEveryoneGroup(), permission))
            {
                return true;
            }

            //If no everyone group permission, check to see if user is in ANY group that the current principal can manage users in.
            var pc = new PaginationContext
            {
                PermissionJoin = PermissionJoin.Any,
                Permissions = new List<string> { permission }
            };

            var manageableGroups = GroupManager.ListAccessibleGroups(principal, pc, true);

            //If user can manage users in no groups, then user obviously can't manage this user
            if (manageableGroups.Count == 0)
            {
                return false;
            }

            //Now get groups user to manage is in, if there overlap between this list of groups and 
            // the groups logged-in user can mange, then user can be managed.

            //Check if user can edit everyone
            var identityGroups = GroupManager.ListGroupMembershipIds(uniqueIdentifier);

            return manageableGroups.Any(identityGroups.Contains);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="permission"></param>
        private static bool CheckCurrentPrincipalPermissionForUser(string uniqueIdentifier, string permission)
        {
            //Get the current principal
            ExtendedPrincipal currentPrincipal = GetCurrentPrincipal();

            return CheckPrincipalPermissionForUser(currentPrincipal, uniqueIdentifier, permission);
        }

        /// <summary>
        /// Return a boolean indicating if the current principal can edit the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique identifier of user to check.</param>
        /// <returns></returns>
        public static bool CanCurrentPrincipalEditUser(string uniqueIdentifier)
        {
            return CheckCurrentPrincipalPermissionForUser(uniqueIdentifier, "Group.ManageUsers");
        }

        /// <summary>
        /// Return a boolean indicating if the current principal can edit the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique identifier of user to check.</param>
        /// <returns></returns>
        public static bool CanCurrentPrincipalViewUser(string uniqueIdentifier)
        {
            return CheckCurrentPrincipalPermissionForUser(uniqueIdentifier, "Group.View");
        }

        /// <summary>
        /// Delete a user and associated information.
        /// </summary>
        /// <param name="callingPrincipal">User attempting to perform the operation.</param>
        /// <param name="status">Status message to return.</param>
        /// <param name="userUniqueIdentifier">Identity of the user to delete.</param>
        /// <param name="deleteResponses">Indicates whether the users responses should be deleted.</param>
        /// <returns>Boolean indicating the success of the operation.</returns>
        /// <api>User Management</api>
        public static bool DeleteUser(ExtendedPrincipal callingPrincipal, string userUniqueIdentifier, bool deleteResponses, out string status)
        {
            var userPrincipal = GetUserPrincipal(userUniqueIdentifier);

            if (userPrincipal == null)
            {
                status = "Unable to delete user because user identity was null.";
                return false;
            }

            var currentUserName = callingPrincipal.Identity.Name;

            if (currentUserName == userUniqueIdentifier)
            {
                status = "DELETE_SELF_ERROR";
                return false;
            }

            var groups = GroupManager.GetGroupMemberships(userPrincipal.Identity.Name);
            // Check if there is any group containing userPrincipal manageable by callingPrincipal
            var canBeDeleted = groups.Aggregate(false, (current, group) => current || (AuthorizationFactory.GetAuthorizationProvider().Authorize(callingPrincipal, group, "Group.ManageUsers")));
            canBeDeleted = canBeDeleted ||
                           AuthorizationFactory.GetAuthorizationProvider().Authorize(callingPrincipal, GroupManager.GetEveryoneGroup(),
                                                                                     "Group.ManageUsers");
            if (!canBeDeleted)
            {
                throw new AuthorizationException();
            }

            //delete the user from the groups
            foreach (var group in groups)
            {
                group.RemoveUser(userPrincipal);
            }

            //Clear out any cached principals
            ExpireCachedPrincipal(userUniqueIdentifier);

            //Remove user from all roles to ensure their roles get flushed from the role cache
            RoleManager.RemoveUserFromRoles(userUniqueIdentifier, RoleManager.ListRoles().ToArray());

            //remove user from all acl
            AccessManager.DeleteUserEntriesInAllAcl(userUniqueIdentifier);

            //Delete the user from Checkbox
            status = string.Empty;
            return MembershipProvider.DeleteUser(userUniqueIdentifier, deleteResponses);
        }

        /// <summary>
        /// Get the domain associated with the identity.
        /// </summary>
        /// <param name="uniqueIdentifier">Identity of the user.</param>
        /// <returns>Domain associated with the user.</returns>
        public static string GetDomain(string uniqueIdentifier)
        {
            //For the sake of efficiency, parse the user name to get the domain
            if (uniqueIdentifier.IndexOf("/") >= 0)
            {
                return uniqueIdentifier.Split('/')[0];
            }

            return uniqueIdentifier.IndexOf(@"\") >= 0 ? uniqueIdentifier.Split('\\')[0] : string.Empty;
        }

        /// <summary>
        /// Update the user's identity and profile information.
        /// </summary>
        /// <param name="userUniqueIdentifier">Unique identifier of the user to update.</param>
        /// <param name="newUserName">New user name for the user.  A NULL value should be passed if the user name is not to be changed.</param>
        /// <param name="newDomain">New NT/AD domain name for the user.</param>
        /// <param name="newPassword">New password for the user.  A NULL value should be passed if the password is not to be changed.</param>
        /// <param name="newEmailAddress">New email address for the user.</param>
        /// <param name="modifier"> </param>
        /// <param name="status">Output parameter with status from the stored procedure or an error message.</param>
        /// <returns><see cref="CheckboxPrincipal"/> representing the updated user.</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal UpdateUser(string userUniqueIdentifier, string newUserName, string newDomain, string newPassword, string newEmailAddress, string modifier, out string status)
        {
            //validate e-mail
            if (!string.IsNullOrEmpty(newEmailAddress))
            {
                if (!_emailValidator.Validate(newEmailAddress))
                    throw new Exception("Invalid e-mail.");
            }

            var oldUser = GetUserPrincipal(userUniqueIdentifier);
            var oldUserName = oldUser.Identity.Name;

            //Perform update
            if (!MembershipProvider.UpdateUser(
                userUniqueIdentifier,
                newUserName,
                newDomain,
                newPassword,
                newEmailAddress,
                modifier,
                out status))
            {
                //Return null principal on failure
                return null;
            }

            //Clear cache
            ExpireCachedPrincipal(userUniqueIdentifier);

            //If the user's name has changed clear the cache of the new name as well.
            if (Utilities.IsNotNullOrEmpty(newUserName))
            {
                ExpireCachedPrincipal(newUserName);
            }

            if (Utilities.IsNotNullOrEmpty(newUserName))
            {
                var newUserUniqueIdentifier = Utilities.IsNullOrEmpty(newDomain)
                    ? newUserName
                    : string.Format("{0}/{1}", newDomain, newUserName);

                //user name changed -- refresh groups members
                var user = GetUserPrincipal(newUserUniqueIdentifier);
                if (user != null)
                {
                    var groups = GroupManager.GetGroupMemberships(user.Identity.Name);
                    foreach (var group in groups)
                    {
                        group.RemoveUserFromCache(oldUserName);
                    }
                }

                //Return new principal, which will populate cache too
                return user;
            }

            return GetUserPrincipal(userUniqueIdentifier);
        }

        /// <summary>
        /// Applies filtering, sorting and pagination to a raw list of users
        /// </summary>
        /// <param name="rawUserList">A list of user unique identifiers</param>
        /// <param name="paginationContext"></param>
        /// <returns>A list of user unique identifiers meeting the filter, sort, and page criteria of the pagination context</returns>
        /// <remarks>This method allows lists of user unique identifiers to be filtered and sorted based on profile properties, which may not be known to the call that generates the raw list</remarks>
        public static List<string> FilterPageAndSortUserList(List<string> rawUserList, PaginationContext paginationContext)
        {
            List<string> userList;

            //If there is a sort or filter specified, we're going to have to load only the profile properties which will be necessary to sort/filter
            if (paginationContext.IsFiltered || paginationContext.IsSorted)
            {
                var userProfiles = new List<Dictionary<string, string>>();

                foreach (var username in rawUserList)
                {
                    var userProfile = new Dictionary<string, string>();

                    //Update/Add specific properties
                    userProfile["UniqueIdentifier"] = username;
                    userProfile["Domain"] = GetDomain(username);

                    if (paginationContext.IsFiltered && String.Equals(paginationContext.FilterField, "Email", StringComparison.InvariantCultureIgnoreCase)
                        || paginationContext.IsSorted && String.Equals(paginationContext.SortField, "Email", StringComparison.InvariantCultureIgnoreCase))
                        userProfile["Email"] = GetUserEmail(username);

                    string[] userRoles = null;

                    //Profile                    
                    if (paginationContext.IsFiltered && String.Equals(paginationContext.FilterField, "RoleMembership", StringComparison.InvariantCultureIgnoreCase)
                        || paginationContext.IsSorted && String.Equals(paginationContext.SortField, "RoleMembership", StringComparison.InvariantCultureIgnoreCase))
                    {
                        userRoles = RoleManager.ListRolesForUser(username);
                        userProfile["RoleMembership"] = string.Join(",", userRoles);
                    }

                    if (paginationContext.IsFiltered && String.Equals(paginationContext.FilterField, "RoleMembership", StringComparison.InvariantCultureIgnoreCase)
                        || paginationContext.IsSorted && String.Equals(paginationContext.SortField, "RoleMembership", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var permissionList = new List<string>();
                        if (userRoles == null)
                            userRoles = RoleManager.ListRolesForUser(username);

                        foreach (string role in userRoles)
                        {
                            permissionList.AddRange(RoleManager.GetRole(role).Permissions);
                        }

                        userProfile["RolePermissions"] = string.Join(",", permissionList.Distinct());
                    }

                    //Get profile, if there are no neccessary filter/sort field 
                    if (paginationContext.IsFiltered && !userProfile.ContainsKey(paginationContext.FilterField)
                        || paginationContext.IsSorted && !userProfile.ContainsKey(paginationContext.SortField))
                    {
                        foreach (var option in ProfileManager.GetProfile(username))
                        {
                            userProfile.Add(option.Key, option.Value);
                        }
                    }

                    userProfiles.Add(userProfile);
                }

                //Filter, if necessary
                var filteredList = (paginationContext.IsFiltered
                                        ? userProfiles.Where(
                                            profile =>
                                                profile.ContainsKey(paginationContext.FilterField)
                                                && profile[paginationContext.FilterField].ToLower().Contains(paginationContext.FilterValue.ToLower()))
                                        : userProfiles);

                //Sort, if necessary, and select user ids
                userList = (paginationContext.IsSorted
                                ? paginationContext.SortAscending
                                ? filteredList.OrderBy(profile => profile.ContainsKey(paginationContext.SortField) ? profile[paginationContext.SortField] : string.Empty)
                                      : filteredList.OrderByDescending(profile => profile.ContainsKey(paginationContext.SortField) ? profile[paginationContext.SortField] : string.Empty)
                                : filteredList)
                    .Select(profile => profile["UniqueIdentifier"])
                    .ToList();

            }
            else
            {
                userList = rawUserList;
                userList.Sort();
            }

            //Finally, select page of results and set item count
            paginationContext.ItemCount = userList.Count;

            if (paginationContext.PageSize > 0 && paginationContext.CurrentPage > 0)
            {
                return userList
                    .Skip(paginationContext.GetStartIndex())
                    .Take(paginationContext.PageSize)
                    .ToList();
            }

            return userList.ToList();
        }

        /// <summary>
        /// List users in the system.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<string> ListUsers(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext)
        {
            //If user can see all users, take a shortcut and let membership provider perform all sorting, filtering, etc.
            if (callingPrincipal.IsInRole("System Administrator")
                || AuthorizationFactory.GetAuthorizationProvider().Authorize(callingPrincipal, GroupManager.GetEveryoneGroup(), "Group.View"))
            {
                return MembershipProvider.ListAllUsers(paginationContext).ToList();
            }

            return IntersectMembers(callingPrincipal, paginationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        private static List<string> IntersectMembers(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext)
        {
            var originalPageSize = paginationContext.PageSize;
            var originalPageNumber = paginationContext.CurrentPage;

            paginationContext.PageSize = -1;
            paginationContext.CurrentPage = -1;

            var pageOfAccessibleMembers = GroupManager.ListMembersOfAccessibleGroups(callingPrincipal, paginationContext);

            paginationContext.PageSize = originalPageSize;
            paginationContext.CurrentPage = originalPageNumber;

            int requiredNumberOfvalidMembers = paginationContext.GetStartIndex() + paginationContext.PageSize;

            List<string> validMembers = new List<string>();

            foreach (var memberName in pageOfAccessibleMembers)
            {
                var member = MembershipProvider.GetUser(memberName, false);
                
                if (member != null)
                {
                    validMembers.Add(memberName);

                    if (validMembers.Count >= requiredNumberOfvalidMembers)
                        break;
                }
            }

            return validMembers
                .Skip(paginationContext.GetStartIndex())
                .Take(paginationContext.PageSize)
                .ToList();
        }

        /// <summary>
        /// List users in the system by given provider. Implements true paging.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static List<string> ListUsers(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext, string provider)
        {
            if (StaticConfiguration.DisableForeighMembershipProviders || string.IsNullOrEmpty(provider) || !HasEveryoneGroupAccess(callingPrincipal))
                return ListUsers(callingPrincipal, paginationContext);

            var innerProvider = Membership.Providers[provider];
            var checkboxProvider = innerProvider as ICheckboxMembershipProvider;

            if (checkboxProvider != null)
                return checkboxProvider.ListAllUsers(paginationContext).ToList();

            var total = 0;
            var users = new MembershipUserCollection();
            var page = paginationContext.CurrentPage;

            if (innerProvider is ActiveDirectoryMembershipProvider)
            {
                page = page - 1;
                if (page < 0)
                    page = 0;
            }

            var pageSize = paginationContext.PageSize > 0 ? paginationContext.PageSize : int.MaxValue;

            if (!string.IsNullOrEmpty(paginationContext.FilterValue))
            {
                var filter = paginationContext.FilterValue;
                if (innerProvider is ActiveDirectoryMembershipProvider)
                {
                    //set filter to match LDAP 'like' query format
                    filter = '*' + paginationContext.FilterValue + '*';
                }

                switch (paginationContext.FilterField)
                {
                    case "UniqueIdentifier":
                        users = innerProvider.FindUsersByName(filter, page, pageSize, out total);
                        break;
                    case "Email":
                        users = innerProvider.FindUsersByEmail(filter, page, pageSize, out total);
                        break;
                }
            }
            else
                users = innerProvider.GetAllUsers(page, pageSize, out total);

            paginationContext.ItemCount = total;

            return (from MembershipUser u in GetDomainMembershipUsers(users) select u.UserName).ToList();
        }

        /// <summary>
        /// List users in the system by given provider. Implements true paging.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static List<string> ListUsersByTenantId(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext, string provider)
        {
             return MembershipProvider.ListOnlyTenantUsers(paginationContext).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static bool IsActiveDirectoryMembershipProvider(string providerName)
        {
            return Membership.Providers[providerName] is ActiveDirectoryMembershipProvider;
        }

        /// <summary>
        /// List users in the system by given provider. Implements true paging.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static PageItemUserData[] GetPageItemUsersData(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext, string provider)
        {
            var activeDirectoryProvider = Membership.Providers[provider] as ActiveDirectoryMembershipProvider;
            if (activeDirectoryProvider != null)
            {
                var total = 0;
                var users = new MembershipUserCollection();
                var page = paginationContext.CurrentPage;

                page = page - 1;
                if (page < 0)
                    page = 0;

                var pageSize = paginationContext.PageSize > 0 ? paginationContext.PageSize : int.MaxValue;

                if (!string.IsNullOrEmpty(paginationContext.FilterValue))
                {
                    var filter = '*' + paginationContext.FilterValue + '*';
                    //set filter to match LDAP 'like' query format

                    switch (paginationContext.FilterField)
                    {
                        case "UniqueIdentifier":
                            users = activeDirectoryProvider.FindUsersByName(filter, page, pageSize, out total);
                            break;
                        case "Email":
                            users = activeDirectoryProvider.FindUsersByEmail(filter, page, pageSize, out total);
                            break;
                    }
                }
                else
                    users = activeDirectoryProvider.GetAllUsers(page, pageSize, out total);

                paginationContext.ItemCount = total;

                return (from MembershipUser u in users let domainUser = GetDomainMembershipUser(u) select new PageItemUserData { Email = domainUser.Email, UniqueIdentifier = domainUser.UserName }).ToArray();
            }
            else 
                throw new NotImplementedException();

            return new PageItemUserData[0];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <returns></returns>
        public static bool HasEveryoneGroupAccess(ExtendedPrincipal callingPrincipal)
        {
            //pass if user is System Admin
            if (callingPrincipal.IsInRole("System Administrator"))
                return true;

            //user has personal acl
            var everyoneGroup = GroupManager.GetEveryoneGroup();
            var policy = everyoneGroup.ACL.GetPolicy(callingPrincipal);
            if (policy != null)
            {
                if (policy.Permissions.Contains("Group.View"))
                    return true;
            }

            //everyone group has access to itself
            policy = everyoneGroup.ACL.GetPolicy(everyoneGroup);
            if (policy != null && policy.Permissions.Contains("Group.View"))
                return true;
            
            //one of user's group has access 
            foreach(var group in GroupManager.GetGroupMemberships(callingPrincipal.Identity.Name))
            {
                policy = everyoneGroup.ACL.GetPolicy(group);
                if (policy != null)
                {
                    if (policy.Permissions.Contains("Group.View"))
                        return true;
                }
            }

            //policy has default access
            policy = everyoneGroup.DefaultPolicy;
            return policy != null && policy.Permissions.Contains("Group.View");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MembershipUser GetDomainMembershipUser(MembershipUser user)
        {
            //Attempt to translate to NT account
            if (user.ProviderUserKey != null && user.ProviderUserKey is SecurityIdentifier)
            {
                try
                {
                    var si = user.ProviderUserKey as SecurityIdentifier;

                    if (si.IsValidTargetType(typeof(NTAccount)))
                    {
                        var ntAccount = si.Translate(typeof(NTAccount)) as NTAccount;

                        if (ntAccount == null)
                        {
                            return user;
                        }

                        return new MembershipUser(
                            user.ProviderName,
                            ntAccount.Value,
                            null,
                            user.Email,
                            user.PasswordQuestion,
                            user.Comment,
                            user.IsApproved,
                            user.IsLockedOut,
                            user.CreationDate,
                            DateTime.MinValue,  //Last login date, last activity date not supported by AD membership provider
                            DateTime.MinValue,
                            user.LastPasswordChangedDate,
                            user.LastLockoutDate);

                    }
                }
                catch
                {

                }
            }

            return user;
        }

        ///<summary>
        ///</summary>
        ///<param name="users"></param>
        ///<returns></returns>
        public static IEnumerable<MembershipUser> GetDomainMembershipUsers(MembershipUserCollection users)
        {
            return (from MembershipUser user in users select GetDomainMembershipUser(user)).ToList();
        }

        /// <summary>
        /// List users in the system for the given role.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<string> ListUsersInRole(ExtendedPrincipal callingPrincipal, string role, PaginationContext paginationContext)
        {
             //Otherwise, get filtered list w/out paging
            var originalPageSize = paginationContext.PageSize;
            var originalPageNumber = paginationContext.CurrentPage;

            paginationContext.PageSize = -1;
            paginationContext.CurrentPage = -1;

            //Get filtered/sorted list w/out paging
            var unpagedUserList = RoleManager.GetUsersInRole(role);

            paginationContext.PageSize = originalPageSize;
            paginationContext.CurrentPage = originalPageNumber;

            if (paginationContext.PageSize <= 0 || paginationContext.CurrentPage <= 0)
            {
                return unpagedUserList.ToList();
            }

            return unpagedUserList
                .Skip(paginationContext.GetStartIndex())
                .Take(paginationContext.PageSize)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <returns></returns>
        public static List<string> ListUsers(ExtendedPrincipal callingPrincipal)
        {
            return ListUsers(callingPrincipal, new PaginationContext());
        }

        /// <summary>
        /// Validator for user emails
        /// </summary>
        private static EmailValidator _emailValidator = new EmailValidator();

        /// <summary>
        /// Create a user and return an identity for it.
        /// </summary>
        /// <param name="name">UserName of the user to create.</param>
        /// <param name="password">Created user's password.</param>
        /// <param name="domain">Domain of the created user.</param>
        /// <param name="emailAddress"></param>
        /// <param name="status">Status message for the operation.</param>
        /// <returns>User Identity</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal CreateUser(string name, string password, string domain, string emailAddress, string creator, out string status)
        {
            if (name == null || name.Replace("'", string.Empty).Trim() == string.Empty)
            {
                status = "User name must contain at least one alphanumeric character.";
                return null;
            }

            //If user is not a network user, require a password.
            if (Utilities.IsNullOrEmpty(domain)
                && Utilities.IsNullOrEmpty(password))//|| password.Length > 50)
            {
                // If no password has been specified generate a random value.
                password = GeneratePassword(8);
            }

            if (password != null)
            {
                password = password.Trim();
            }

            if (domain != null)
            {
                domain = domain.Trim();
            }

            //validate e-mail only if the e-mail was set
            if (!string.IsNullOrEmpty(emailAddress))
            {
                if (!_emailValidator.Validate(emailAddress))
                    throw new ArgumentNullException("Invalid e-mail.");
            }

            var uniqueIdentifier = Utilities.IsNotNullOrEmpty(domain)
                ? domain + "/" + name
                : name;


            var createdUserId = MembershipProvider.CreateUser(
                    uniqueIdentifier,
                    password,
                    emailAddress,
                    string.Empty,
                    string.Empty,
                    true,
                    uniqueIdentifier,
                    creator,
                    out status);

            //expire possible cash
            ExpireCachedPrincipal(uniqueIdentifier);

            return GetUserPrincipal(createdUserId);
        }

        /// <summary>
        /// Generate a pseudo random password which is length characters long.
        /// </summary>
        /// <param name="length">The length of the password being generated.</param>
        /// <returns></returns>
        public static string GeneratePassword(int length)
        {
            return Guid.NewGuid().ToString().Substring(0, length);
        }

        /// <summary>
        /// Given a user guid, return an authenticated principal.
        /// </summary>
        /// <param name="guid">GUID identifying a user.</param>
        /// <returns>CheckboxPrincipal associated with the user, or NULL if the guid is not associated with a user.</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal AuthenticateUser(Guid guid)
        {
            //Get an instance of the default authentication provider
            try
            {
                //Check if GUID maps to a user
                var userName = MembershipProvider.GetUserNameFromGuid(guid);

                //If no, return null
                if (Utilities.IsNullOrEmpty(userName))
                {
                    return null;
                }

                return GetUserPrincipal(userName);
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }

                Logger.Write("An exception occurred while attempting to authenticate a user with guid [" + guid + "].  As dictated by the exception policy, the error was not rethrown.", "Warning");
                return null;
            }
        }


        /// <summary>
        /// Validate user login credentials are correct
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ValidateLoginCredentials(string userName, string password)
        {
            return MembershipProvider.ValidateUser(userName, password);
        }

        /// <summary>
        /// Authenticate a user using name/password credentials.
        /// </summary>
        /// <param name="name">Unique name for the user.</param>
        /// <param name="password">User password.</param>
        /// <returns>CheckboxPrincipal associated with the user if the user is successfully authenticated.</returns>
        /// <api>User Management</api>
        public static CheckboxPrincipal AuthenticateUser(string name, string password)
        {
            //Get an instance of the default authentication provider
            try
            {
                //Step 1:  Handle IIS Authenticated network user
                if (name.Split('/').Length > 1)
                {
                    //If attempting to login interactively, return null
                    if (Utilities.IsNotNullOrEmpty(password))
                    {
                        return null;
                    }

                    //If user is not found by any membership provider, do not authenticate
                    if (ApplicationManager.AppSettings.RequireRegisteredUsers
                        && !UserExists(name))
                    {
                        return null;
                    }

                    //Otherwise, get and return a principal.  Since authentication happens on every request, no way
                    // to enforce concurrent login rules. 
                    return GetUserPrincipal(name);
                }

                //For other users, check concurrency settings and username/password
                //Make sure the user isn't already logged in
                if (ApplicationManager.AppSettings.ConcurrentLoginMode == ConcurrentLoginMode.NotAllowed)
                {
                    if (_loginCacheManager != null)
                    {
                        if (_loginCacheManager.Contains(name))
                        {
                            Logger.Write("Concurrent login attempt failed for user [" + name + "].", "Warning", 3, -1, Severity.Warning);
                            return null;
                        }
                    }
                }

                if (Utilities.IsNullOrEmpty(name))
                {
                    return null;
                }

                if (password == null)
                {
                    password = string.Empty;
                }

                //Attempt to validate credentials
                if (!MembershipProvider.ValidateUser(name, password))
                {
                    return null;
                }

                //Otherwise, get a principal
                CheckboxPrincipal principal = GetUserPrincipal(name);

                if (principal == null)
                {
                    return null;
                }

                //Expire user from login cache if necessary.
                if (_loginCacheManager != null)
                {
                    //If concurrency is set to logout existing, remove existing login.
                    if (ApplicationManager.AppSettings.ConcurrentLoginMode == ConcurrentLoginMode.LogoutCurrent)
                    {
                        if (_loginCacheManager != null)
                        {
                            if (_loginCacheManager.Contains(name))
                            {
                                //TODO: Actually invalidate user forms ticket, if possible
                                try
                                {
                                    Logger.Write("Logging out user [" + name + "] due to concurrent login.", "Warning", 3, -1, Severity.Warning);
                                }
                                catch (Exception)
                                {
                                    //suppress exception
                                }

                                _loginCacheManager.Remove(name);
                            }
                        }
                    }
                }

                return principal;
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }

                Logger.Write("An exception occurred while attempting to authenticate a user with name [" + name + "].  As dictated by the exception policy, the error was not rethrown.", "Warning");
                return null;
            }
        }

        /// <summary>
        /// Return a list of UserLoginInfo objects for all users currently logged-in.
        /// </summary>
        /// <returns>List of UserLoginInfo objects.</returns>
        /// <api>User Management</api>
        public static UserLoginInfo[] LoggedInUsers()
        {
            var infoList = new ArrayList();

            var keys = _loginCacheManager.ListKeys();

            if (keys != null)
            {
                foreach (string key in keys)
                {
                    var info = (UserLoginInfo)_loginCacheManager[key];

                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
            }

            return (UserLoginInfo[])infoList.ToArray(typeof(UserLoginInfo));
        }

        /// <summary>
        /// Remove user from logged-in user cache.  This does not cause a user to be logged-out.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from cache</param>
        /// <returns>Boolean indicating success.</returns>
        /// <api>User Management</api>
        public static void ExpireLoggedInUser(string uniqueIdentifier)
        {
            //Attempt to get the token for the user to logout
            if (_loginCacheManager == null) return;
            if (_loginCacheManager.Contains(uniqueIdentifier))
            {
                _loginCacheManager.Remove(uniqueIdentifier);
            }
        }

        /// <summary>
        /// Count number of unencrypted passwords in db
        /// </summary>
        /// <returns></returns>
        public static int CountUnencryptedPasswords()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_CountUnencrypted");

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader(reader, "IdentityCount", 0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return 0;
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="userName"></param>       
        public static void UnlockUser(string userName)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_UpdateLockOut");
            command.AddInParameter("UserName", DbType.String, userName);
            command.AddInParameter("FailedLogins", DbType.Int32, 0);
            command.AddInParameter("LockedOut", DbType.Boolean, false);

            db.ExecuteNonQuery(command);
            ExpireCachedPrincipal(userName);
        }

        /// <summary>
        /// Lock user
        /// </summary>
        /// <param name="userName"></param>       
        public static void LockUser(string userName)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_UpdateLockOut");
            command.AddInParameter("UserName", DbType.String, userName);
            command.AddInParameter("FailedLogins", DbType.Int32, 0);
            command.AddInParameter("LockedOut", DbType.Boolean, true);

            db.ExecuteNonQuery(command);
            ExpireCachedPrincipal(userName);
        }

        /// <summary>
        /// List unencrypted user passwords.
        /// </summary>
        /// <returns></returns>
        public static List<UserDto> ListUnencryptedPasswordUsers()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_ListUnencrypted");

            var userList = new List<UserDto>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var uniqueIdentifier = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);
                        var password = DbUtility.GetValueFromDataReader(reader, "Password", string.Empty);
                        var domain = DbUtility.GetValueFromDataReader(reader, "Domain", string.Empty);
                        var email = DbUtility.GetValueFromDataReader(reader, "Email", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(uniqueIdentifier))
                        {
                            userList.Add(new UserDto(
                                uniqueIdentifier,
                                email,
                                domain,
                                password));
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userList;
        }


        /// <summary>
        /// Hash passwords for all users that have not had passwords hashed yet.
        /// </summary>
        /// <returns>Boolean indicating whether the operation was successful or not.</returns>
        public static void EncryptUserPassword(UserDto userDtoObject, string modifier)
        {
            string dummy;

            UpdateUser(
                userDtoObject.UniqueIdentifier,
                userDtoObject.UniqueIdentifier,
                userDtoObject.Domain,
                userDtoObject.Password,
                userDtoObject.EmailAddress,
                modifier,
                out dummy);
        }

        /// <summary>
        /// Determines if product information is available to be displayed to a user.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public static List<FileInfo> GetProductTourMessages(ExtendedPrincipal principal, string applicationPath)
        {
            if (principal == null) { return new List<FileInfo>(); }

            var messages = GetMessagesByRole(principal, applicationPath);
            var availableMessages = new List<FileInfo>();

            if (messages.Count > 0)
            {
                var db = DatabaseFactory.CreateDatabase();

                using (var connection = db.GetConnection())
                {
                    connection.Open();
                    var t = connection.BeginTransaction();

                    try
                    {
                        foreach (var message in messages)
                        {
                            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Message_OptOut_Get");
                            command.AddInParameter("UniqueIdentifier", DbType.String, principal.Identity.Name);
                            command.AddInParameter("Page", DbType.String, message.Name);

                            var result = db.ExecuteScalar(command, t) ?? string.Empty;

                            int optOut;
                            if (!int.TryParse(result.ToString(), out optOut)) continue;
                            if (optOut == 0)
                            {
                                availableMessages.Add(message);
                            }
                        }
                    }
                    catch
                    {
                        t.Rollback();
                        throw;
                    }
                    finally
                    {
                        t.Commit();
                        connection.Close();
                    }
                }
            }

            return availableMessages;
        }

        /// <summary>
        /// Opt a user out of the product tour messages.
        /// </summary>
        /// <param name="userName">Name of user to opt-out.</param>
        /// <param name="messages">Messages to opt out of.</param>
        public static void OptOutOfProductTourMessages(string userName, List<FileInfo> messages)
        {
            //Do nothing if no user or messages
            if (Utilities.IsNullOrEmpty(userName) || messages == null) { return; }

            var db = DatabaseFactory.CreateDatabase();

            using (var connection = db.GetConnection())
            {
                connection.Open();
                var t = connection.BeginTransaction();

                try
                {
                    foreach (var message in messages)
                    {
                        var command = db.GetStoredProcCommandWrapper("ckbx_sp_Message_OptOut_Set");
                        command.AddInParameter("UniqueIdentifier", DbType.String, userName);
                        command.AddInParameter("Page", DbType.String, message.Name);

                        db.ExecuteNonQuery(command, t);
                    }
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
                finally
                {
                    t.Commit();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Load the list of files available for display when a user logs in.
        /// </summary>
        /// <param name="messagePath"></param>
        private static void LoadTourMessages(string messagePath)
        {
            _tourMessages = new List<FileInfo>();

            if (!Directory.Exists(messagePath)) return;
            try
            {
                var files = Directory.GetFiles(messagePath);
                foreach (FileInfo info in from file in files where File.Exists(file) select new FileInfo(file))
                {
                    _tourMessages.Add(info);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Determines if there are messages available for a specified user.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="messagePath"></param>
        /// <returns></returns>
        private static List<FileInfo> GetMessagesByRole(ExtendedPrincipal identity, string messagePath)
        {
            var messages = new List<FileInfo>();
            if (_tourMessages == null) { LoadTourMessages(messagePath); }

            foreach (string formmatedRole in identity.GetRoles().Select(role => role.Replace(" ", string.Empty)))
            {
                messages.AddRange(from message in _tourMessages
                                  let start = message.Name.IndexOf("_") == -1 ? 0 : message.Name.IndexOf("_") + 1
                                  let name = message.Name.Substring(start, message.Name.LastIndexOf(message.Extension) - start)
                                  where formmatedRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                  select message);
            }
            return messages;
        }

        public static ICheckboxMembershipProvider CurrentMembershipProvider
        {
            get
            {
                return MembershipProvider;
            }
        }

        /// <summary>
        /// Set modifier that edits user info
        /// </summary>
        /// <param name="userToEdit"></param>
        /// <param name="modifier"></param>
        public static void SetUserModifier(string userToEdit, string modifier)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Credentials_SetModifier");
            command.AddInParameter("UniqueIdentifier", DbType.String, userToEdit);
            command.AddInParameter("ModifiedBy", DbType.String, modifier);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="encryptedTicket"> </param>
        /// <returns></returns>
        public static FormsAuthenticationTicket GenerateAuthenticationTicket(CheckboxPrincipal principal, out string encryptedTicket)
        {
            string userData;

            if (principal is AnonymousRespondent)
            {
                userData = AnonymousRespondent.IDENTITY_NAME;
            }
            else
            {
                userData = IsCheckboxUser(principal.Identity.Name)
                    ? principal.UserGuid.ToString()
                    : string.Format("{0}::{1}", ParseUserName(principal.Identity.Name), principal.Email);
            }

            //Create auth ticket
            var ticket = new FormsAuthenticationTicket(
                1,
                principal.Identity.Name,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                userData,
                FormsAuthentication.FormsCookiePath);

            encryptedTicket = FormsAuthentication.Encrypt(ticket);
            return ticket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        public static CheckboxPrincipal GetPrincipalByTicket(string encryptedTicket)
        {
            //Use ticket if provided
            if (!string.IsNullOrEmpty(encryptedTicket))
            {
                var decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);

                //Check user data for user guid
                if (string.IsNullOrEmpty(decryptedTicket.UserData))
                {
                    return null;
                }

                if (decryptedTicket.UserData.Equals(AnonymousRespondent.IDENTITY_NAME))
                {
                    return new AnonymousRespondent(Guid.NewGuid());
                }

                Guid userGuid;

                if (Guid.TryParse(decryptedTicket.UserData, out userGuid))
                {
                    return GetUserByGuid(userGuid);
                }

                var userData = decryptedTicket.UserData.Split(new[] {"::"}, StringSplitOptions.None);
                return FindUser(userData[0] /*name*/, userData[1]/*email*/);
            }

            return null;
        }

        /// <summary>
        /// Gets CSV import user column data
        /// </summary>
        /// <returns></returns>
        public static List<string> GetUserImportConfigs()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_GetUserImportConfig");

            var userImportConfigs = new List<string>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var csvFileColumn = DbUtility.GetValueFromDataReader(reader, "FieldName", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(csvFileColumn))
                        {
                            userImportConfigs.Add(csvFileColumn);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userImportConfigs;
        }
    }
}
