using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Pagination;
using Checkbox.Security.Providers;
using System.Security.Principal;
using Checkbox.Users;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Providers
{
    /// <summary>
    /// Membership provider that supports chaining requests through multiple providers. 
    /// </summary>
    /// <remarks>This provider's configuration relates to operation of the chaining, not specific
    /// details about membership, such as password formats, etc.  Those details are left to the
    /// individual child providers.  Changing user information is only supported for the Checkbox
    /// membership provider.  Membership provider properties, such as password retrieval, reset, etc. are
    /// returned only for the Checkbox provider.  Additional providers are used only for user listing and login.</remarks>
    public class ChainingMembershipProvider : MembershipProvider, ICheckboxMembershipProvider
    {
        private const string EXTERNAL_USER = "ExternalUser";

        private List<string> _providerList;

        //Cache of user principals to avoid repeated calls to get users and profiles that will would add
        private static CacheManager _membershipUsersCache;

        /// <summary>
        /// Flag to indicate specified providers have been verified.
        /// </summary>
        private bool ProvidersVerified { get; set; }

        /// <summary>
        /// Checkbox membership provider
        /// </summary>
        public string ProviderName { get { return MembershipProviderManager.CHAINING_MEMBERSHIP_PROVIDER_NAME; } }

        /// <summary>
        /// Get list of providers in "chain"
        /// </summary>
        public List<string> ProviderList
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
        /// Get/set name of checkbox membership provider
        /// </summary>
        private string CheckboxMembershipProviderName { get; set; }

        /// <summary>
        /// Get/set application name
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Properties proxied to Checkbox membership provider.
        /// </summary>
        public override bool EnablePasswordRetrieval { get { return CheckboxProvider.EnablePasswordRetrieval; } }
        public override bool EnablePasswordReset { get { return CheckboxProvider.EnablePasswordReset; } }
        public override bool RequiresQuestionAndAnswer { get { return CheckboxProvider.RequiresQuestionAndAnswer; } }
        public override int MaxInvalidPasswordAttempts { get { return CheckboxProvider.MaxInvalidPasswordAttempts; } }
        public override int PasswordAttemptWindow { get { return CheckboxProvider.PasswordAttemptWindow; } }
        public override bool RequiresUniqueEmail { get { return CheckboxProvider.RequiresUniqueEmail; } }
        public override MembershipPasswordFormat PasswordFormat { get { return CheckboxProvider.PasswordFormat; } }
        public override int MinRequiredPasswordLength { get { return CheckboxProvider.MinRequiredPasswordLength; } }
        public override int MinRequiredNonAlphanumericCharacters { get { return CheckboxProvider.MinRequiredNonAlphanumericCharacters; } }
        public override string PasswordStrengthRegularExpression { get { return CheckboxProvider.PasswordStrengthRegularExpression; } }

        /// <summary>
        /// Get a reference to the Checkbox Membership Provider
        /// </summary>
        public MembershipProvider CheckboxProvider { get { return Membership.Providers[CheckboxMembershipProviderName]; } }

        /// <summary>
        /// Initialize w/configuration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _membershipUsersCache = CacheFactory.GetCacheManager("membershipUsersCacheManager");

            if (MembershipProviderManager.DisableForeignProviders)
                return;

            //Store name of checkbox-specific provider, which is used for most operations
            CheckboxMembershipProviderName = config["checkboxMembershipProvider"];

            //Get list of other chained providers
            if (Utilities.IsNotNullOrEmpty(config["chainedProviders"]))
            {
                ProviderList.AddRange(config["chainedProviders"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (!ProviderList.Contains(CheckboxMembershipProviderName))
            {
                ProviderList.Insert(0, CheckboxMembershipProviderName);
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
                    if (Membership.Providers[providerName] == null)
                    {
                        throw new Exception("Chained membership provider [" + providerName + "] specified in web.config could not be found.");
                    }
                }

                ProvidersVerified = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            EnsureProviders();

            return CheckboxProvider.CreateUser(
                username,
                password,
                email,
                passwordQuestion,
                passwordAnswer,
                isApproved,
                providerUserKey,
                out status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            EnsureProviders();

            return CheckboxProvider.ChangePasswordQuestionAndAnswer(
                username,
                password,
                newPasswordQuestion,
                newPasswordAnswer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer)
        {
            EnsureProviders();

            foreach (string providerName in ProviderList)
            {
                try
                {
                    if (Membership.Providers[providerName].GetUser(username, false) != null)
                    {
                        return Membership.Providers[providerName].GetPassword(username, answer);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            EnsureProviders();

            return CheckboxProvider.ChangePassword(
                username,
                oldPassword,
                newPassword);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer)
        {
            EnsureProviders();

            return CheckboxProvider.ResetPassword(
                username,
                answer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(MembershipUser user)
        {
            EnsureProviders();

            CheckboxProvider.UpdateUser(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string username, string password)
        {
            if (MembershipProviderManager.DisableForeignProviders)
                return false;

            EnsureProviders();

            foreach (string providerName in ProviderList)
            {
                try
                {
                    if (Membership.Providers[providerName].GetUser(username, false) != null)
                    {
                        return Membership.Providers[providerName].ValidateUser(username, password);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName)
        {
            EnsureProviders();

            return CheckboxProvider.UnlockUser(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetUser(providerUserKey.ToString(), userIsOnline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (String.IsNullOrEmpty(username))
                return null;

            //Check cache
            if (_membershipUsersCache.Contains(username))
            {
                var membershipUser = _membershipUsersCache[username] as MembershipUser;

                if (membershipUser != null)
                    return membershipUser;
            }

            EnsureProviders();

            string userIdentityName = username;
            bool isNameWithDomain = UserManager.IsDomainUser(username);
            if (isNameWithDomain)
                userIdentityName = username.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();

            foreach (string providerName in ProviderList)
            {
                try
                {
                    MembershipUser user = Membership.Providers[providerName].GetUser(userIdentityName, userIsOnline);

                    if (user != null)
                    {
                        //Flag user as an external user
                        if (string.Compare(CheckboxMembershipProviderName, user.ProviderName, true) != 0)
                        {
                            user.Comment = EXTERNAL_USER;

                            //Get domain user with domain name in name
                            user = UserManager.GetDomainMembershipUser(user);

                            if (isNameWithDomain && !user.UserName.Equals(username))
                                continue;
                        }

                        //add to cache if not CheckboxMembershipProvider
                        if (!providerName.Equals(CheckboxMembershipProviderName))
                            _membershipUsersCache.Add(username, user);

                        return user;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override string GetUserNameByEmail(string email)
        {

            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            EnsureProviders();

            foreach (string providerName in ProviderList)
            {
                try
                {
                    string userName = Membership.Providers[providerName].GetUserNameByEmail(email);

                    if (Utilities.IsNotNullOrEmpty(userName))
                    {
                        return userName;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            EnsureProviders();

            return CheckboxProvider.DeleteUser(
                username,
                deleteAllRelatedData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            EnsureProviders();

            MembershipUserCollection userCollection = new MembershipUserCollection();

            totalRecords = 0;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? int.MaxValue : pageSize;

            if (MembershipProviderManager.DisableForeignProviders)
                return userCollection;

            foreach (string providerName in ProviderList)
            {
                try
                {
                    if (Membership.Providers[providerName] is ActiveDirectoryMembershipProvider)
                    {
                        //AD paging starts with 0, Checkbox starts with 1
                        pageIndex--;
                    }

                    int tempRecordCount;
                    var tempUserCollection = Membership.Providers[providerName].GetAllUsers(pageIndex, pageSize, out tempRecordCount);
                    totalRecords += tempRecordCount;

                    var checkboxProvider = Membership.Providers[providerName] is CheckboxMembershipProvider;
                    foreach (MembershipUser tempUser in tempUserCollection)
                    {
                        if (userCollection[tempUser.UserName] == null)
                        {
                            if (!checkboxProvider)
                            {
                                //Get domain user with domain name in name
                                var user = UserManager.GetDomainMembershipUser(tempUser);
                                if (user != null)
                                {
                                    user.Comment = EXTERNAL_USER;
                                    userCollection.Add(user);
                                }
                            }
                            else
                                userCollection.Add(tempUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return userCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            EnsureProviders();

            return CheckboxProvider.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            EnsureProviders();

            var userCollection = new MembershipUserCollection();

            totalRecords = 0;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? int.MaxValue : pageSize;

            foreach (string providerName in ProviderList)
            {
                try
                {
                    var filter = usernameToMatch;
                    if (Membership.Providers[providerName] is ActiveDirectoryMembershipProvider)
                    {
                        //set filter to match LDAP 'like' query format
                        filter = '*' + filter + '*';
                        //AD paging starts with 0, Checkbox starts with 1
                        pageIndex--;
                    }

                    int tempRecordCount;
                    var tempUserCollection = Membership.Providers[providerName].FindUsersByName(filter, pageIndex, pageSize, out tempRecordCount);
                    totalRecords += tempRecordCount;

                    var checkboxProvider = Membership.Providers[providerName] is CheckboxMembershipProvider;
                    foreach (MembershipUser tempUser in tempUserCollection)
                    {
                        if (userCollection[tempUser.UserName] == null)
                        {
                            if (!checkboxProvider)
                            {
                                //Get domain user with domain name in name
                                var user = UserManager.GetDomainMembershipUser(tempUser);
                                if (user != null)
                                {
                                    user.Comment = EXTERNAL_USER;
                                    userCollection.Add(user);
                                }
                            }
                            else
                                userCollection.Add(tempUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return userCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            EnsureProviders();

            var userCollection = new MembershipUserCollection();

            totalRecords = 0;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? int.MaxValue : pageSize;

            foreach (string providerName in ProviderList)
            {
                try
                {
                    var filter = emailToMatch;
                    if (Membership.Providers[providerName] is ActiveDirectoryMembershipProvider)
                    {
                        //set filter to match LDAP 'like' query format
                        filter = '*' + filter + '*';
                        //AD paging starts with 0, Checkbox starts with 1
                        pageIndex--;
                    }

                    int tempRecordCount;
                    var tempUserCollection = Membership.Providers[providerName].FindUsersByEmail(filter, pageIndex, pageSize, out tempRecordCount);
                    totalRecords += tempRecordCount;

                    var checkboxProvider = Membership.Providers[providerName] is CheckboxMembershipProvider;
                    foreach (MembershipUser tempUser in tempUserCollection)
                    {
                        if (userCollection[tempUser.UserName] == null)
                        {
                            if (!checkboxProvider)
                            {
                                //Get domain user with domain name in name
                                var user = UserManager.GetDomainMembershipUser(tempUser);
                                if (user != null)
                                {
                                    user.Comment = EXTERNAL_USER;
                                    userCollection.Add(user);
                                }
                            }
                            else
                                userCollection.Add(tempUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
            }

            return userCollection;
        }

        #region ICheckboxMembershipProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="statusText"></param>
        /// <returns></returns>
        public string CreateUser(string uniqueIdentifier, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, string creator, out string statusText)
        {
            return ((ICheckboxMembershipProvider)CheckboxProvider).CreateUser(
                uniqueIdentifier,
                password,
                email,
                passwordQuestion,
                passwordAnswer,
                isApproved,
                providerUserKey,
                creator,
                out statusText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public string[] ListUsersByEmail(string emailToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords)
        {
            MembershipUserCollection collection = FindUsersByEmail(
                emailToMatch,
                pageIndex,
                pageSize,
                out totalRecords);

            return SortUsers(collection, sortField, sortAscending);
        }

        /// <summary>
        /// Sorts user collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        private string[] SortUsers(MembershipUserCollection collection, string sortField, bool sortAscending)
        {
            List<string> userList;
            if ("Email".Equals(sortField, StringComparison.InvariantCultureIgnoreCase))
            {
                var users = from MembershipUser user in collection select user;
                userList = sortAscending ? users.OrderBy(u => u.Email).Select(u => u.UserName).ToList()
                    : users.OrderByDescending(u => u.Email).Select(u => u.UserName).ToList();
            }
            else if ("UniqueIdentifier".Equals(sortField, StringComparison.InvariantCultureIgnoreCase))
            {
                var users = from MembershipUser user in collection select user.UserName;
                userList = sortAscending ? users.OrderBy(u => u).ToList()
                    : users.OrderByDescending(u => u).ToList();
            }
            else
            {
                userList = (from MembershipUser user in collection select user.UserName).ToList();
            }
            return userList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public string[] ListUsersByName(string usernameToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords)
        {
            MembershipUserCollection collection = FindUsersByName(
               usernameToMatch,
               pageIndex,
               pageSize,
               out totalRecords);

            return SortUsers(collection, sortField, sortAscending);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] ListAllUsers(PaginationContext paginationContext)
        {
            int totalRecords;
            string[] sortedList;

            //Check to see if we're filtering by user name or email, and if so, call specific methods for those
            if ("UniqueIdentifier".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrEmpty(paginationContext.FilterValue))
            {
                sortedList = ListUsersByName(paginationContext.FilterValue, paginationContext.CurrentPage, paginationContext.PageSize, 
                    paginationContext.SortField, paginationContext.SortAscending, out totalRecords);

                paginationContext.ItemCount = totalRecords;
            }
            //Check to see if we're filtering by user name or email, and if so, call specific methods for those
            else if ("Email".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrEmpty(paginationContext.FilterValue))
            {
                sortedList = ListUsersByEmail(paginationContext.FilterValue, paginationContext.CurrentPage, paginationContext.PageSize,
                    paginationContext.SortField, paginationContext.SortAscending, out totalRecords);

                paginationContext.ItemCount = totalRecords;
            }
            else
            {
                var allUsers = GetAllUsers(paginationContext.CurrentPage, paginationContext.PageSize, out totalRecords);
                sortedList = SortUsers(allUsers, paginationContext.SortField, paginationContext.SortAscending);
            }

            paginationContext.ItemCount = totalRecords;

            if (paginationContext.PageSize <= 0)
                return sortedList;

            return sortedList.Skip(paginationContext.GetStartIndex())
                             .Take(paginationContext.PageSize).ToArray();
        }

        public string[] ListOnlyTenantUsers(PaginationContext paginationContext)
        {
            int totalRecords;
            string[] sortedList;

            //Check to see if we're filtering by user name or email, and if so, call specific methods for those
            if ("UniqueIdentifier".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrEmpty(paginationContext.FilterValue))
            {
                sortedList = ListUsersByName(paginationContext.FilterValue, paginationContext.CurrentPage, paginationContext.PageSize,
                    paginationContext.SortField, paginationContext.SortAscending, out totalRecords);

                paginationContext.ItemCount = totalRecords;
            }
            //Check to see if we're filtering by user name or email, and if so, call specific methods for those
            else if ("Email".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrEmpty(paginationContext.FilterValue))
            {
                sortedList = ListUsersByEmail(paginationContext.FilterValue, paginationContext.CurrentPage, paginationContext.PageSize,
                    paginationContext.SortField, paginationContext.SortAscending, out totalRecords);

                paginationContext.ItemCount = totalRecords;
            }
            else
            {
                var allUsers = GetAllUsers(paginationContext.CurrentPage, paginationContext.PageSize, out totalRecords);
                sortedList = SortUsers(allUsers, paginationContext.SortField, paginationContext.SortAscending);
            }

            paginationContext.ItemCount = totalRecords;

            if (paginationContext.PageSize <= 0)
                return sortedList;

            return sortedList.Skip(paginationContext.GetStartIndex())
                             .Take(paginationContext.PageSize).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetUserIntrinsicProperties(string userName)
        {
            MembershipUser user = GetUser(userName, false);

            if (user == null)
            {
                return new Dictionary<string, object>();
            }

            //If a user is a non-checkbox user, just populate email
            if (user.Comment == EXTERNAL_USER)
            {
                var propertiesDictionary = new Dictionary<string, object>();
                propertiesDictionary["UniqueIdentifier"] = userName;

                try
                {
                    propertiesDictionary["Email"] = user.Email ?? string.Empty;
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }

                return propertiesDictionary;
            }

            //For other users, get checkbox data
            return ((ICheckboxMembershipProvider)CheckboxProvider).GetUserIntrinsicProperties(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public string GetUserNameFromGuid(Guid userGuid)
        {
            return ((ICheckboxMembershipProvider)CheckboxProvider).GetUserNameFromGuid(userGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="newUserName"></param>
        /// <param name="newDomain"></param>
        /// <param name="newPassword"></param>
        /// <param name="newEmailAddress"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateUser(string userUniqueIdentifier, string newUserName, string newDomain, string newPassword, string newEmailAddress, string modifier, out string status)
        {
            return ((ICheckboxMembershipProvider)CheckboxProvider).UpdateUser(
                userUniqueIdentifier,
                newUserName,
                newDomain,
                newPassword,
                newEmailAddress,
                modifier, 
                out status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public void ExpireCachedUserNonProfileProperties(string uniqueIdentifier)
        {
            ((ICheckboxMembershipProvider)CheckboxProvider).ExpireCachedUserNonProfileProperties(uniqueIdentifier);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifer"></param>
        /// <returns></returns>
        public IIdentity GetUserIdentity(string uniqueIdentifer)
        {
            MembershipUser user = GetUser(uniqueIdentifer, false);

            if (user != null)
            {
                //Set provider name if comment is set.  Comment is used to indicate whether the user
                // is a network user or password user when retrieve by Checkbox membership provider.
                if (Utilities.IsNullOrEmpty(user.Comment))
                {
                    user.Comment = EXTERNAL_USER;
                }

                //Use auth type to represent user as a checkbox password/network user, or other
                return new GenericIdentity(user.UserName, user.Comment);
            }

            return null;
        }
    }
}
