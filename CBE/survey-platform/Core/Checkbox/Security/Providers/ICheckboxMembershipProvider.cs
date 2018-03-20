using System;
using System.Collections.Generic;
using System.Web.Security;
using Checkbox.Pagination;
using System.Security.Principal;

namespace Checkbox.Security.Providers
{
    /// <summary>
    /// Interface definition for membership provider used by Checkbox.
    /// </summary>
    public interface ICheckboxMembershipProvider
    {
        /// <summary>
        /// Name of membership provider
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        bool ChangePassword(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Create a user
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
        string CreateUser(string uniqueIdentifier, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, string creator, out string statusText);

        /// <summary>
        /// Gets membership user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        MembershipUser GetUser(string username, bool userIsOnline);

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="uniqueIdentifer"></param>
        /// <returns></returns>
        IIdentity GetUserIdentity(string uniqueIdentifer);

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        bool DeleteUser(string userName, bool deleteAllRelatedData);

        /// <summary>
        /// Find users based on email address.
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <param name="sortField"></param>
        /// <returns></returns>
        string[] ListUsersByEmail(string emailToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords);

        /// <summary>
        /// Find users by email address.
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <param name="sortField"></param>
        /// <returns></returns>
        string[] ListUsersByName(string usernameToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords);

        /// <summary>
        /// Validate a given user's credentials
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ValidateUser(string userName, string password);

        /// <summary>
        /// List all available users.
        /// </summary>
        /// <returns></returns>
        string[] ListAllUsers(PaginationContext paginationContext);

        /// <summary>
        /// List all users with tenant id.
        /// </summary>
        /// <returns></returns>
        string[] ListOnlyTenantUsers(PaginationContext paginationContext);

        /// <summary>
        /// Get a user's password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        string GetPassword(string userName, string answer);

        /// <summary>
        /// Get intrinsic properties (i.e. non profile) of a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Dictionary<string, object> GetUserIntrinsicProperties(string userName);

        /// <summary>
        /// Get the name of a user based on the user's guid
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        string GetUserNameFromGuid(Guid userGuid);

        /// <summary>
        /// Update a user's properties.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="newUserName"></param>
        /// <param name="newDomain"></param>
        /// <param name="newPassword"></param>
        /// <param name="newEmailAddress"></param>
        /// <param name="status"></param>
        bool UpdateUser(string userUniqueIdentifier, string newUserName, string newDomain, string newPassword, string newEmailAddress, string modifier, out string status);

        /// <summary>
        /// Remove user from cache with user non profile properties
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        void ExpireCachedUserNonProfileProperties(string uniqueIdentifier);
    }
}
