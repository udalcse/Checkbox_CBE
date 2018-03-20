using System.ServiceModel;
using System.ServiceModel.Web;
using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// The interface that defines all user related web service methods.
    /// </summary>
    [ServiceContract]
    public interface IUserManagementApi
    {
        /// <summary>
        /// Authenticate the provided login credentials.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The associated password.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{String}"/> object where T is of type <see cref="System.String"/>. </para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an encrypted forms auth token.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string> AuthenticateUser(string username, string password);

        /// <summary>
        /// Retrieve a <see cref="UserData"/> object which contains complete details about a user.
        /// Available data includes, account type, unique identifier, email address, profile, assigned roles and group memberships.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is of type <see cref="UserData"/>. </para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<UserData> GetUserData(string authToken, string uniqueIdentifier);

        /// <summary>
        /// Get a paged, sorted, and filtered list of UserData objects.
        /// </summary>
        /// <param name="authToken">Encrypted forms auth token identifying the requesting user.</param>
        /// <param name="provider">Membership provider name.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="UserData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> GetUsers(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get a paged, sorted, and filtered list of UserData objects.
        /// </summary>
        /// <param name="authToken">Encrypted forms auth token identifying the requesting user.</param>
        /// <param name="provider">Membership provider name.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="UserData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> GetUsersByPeriod(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName);

        /// <summary>
        /// Get a paged, sorted, and filtered list of UserData objects.
        /// </summary>
        /// <param name="authToken">Encrypted forms auth token identifying the requesting user.</param>
        /// <param name="provider">Membership provider name.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="UserData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> GetUsersTenantByPeriod(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName);

        /// <summary>
        /// Get a paged, sorted, and filtered list of UserData objects that belongs to the role specified.
        /// </summary>
        /// <param name="authToken">Encrypted forms auth token identifying the requesting user.</param>
        /// <param name="role">Role that is granted for the users</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="UserData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> GetUsersInRole(string authToken, string role, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);
        
        /// <summary>
        /// Get a paged, sorted, and filtered list of user identities.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="PagedListResult{String}"/> of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains users that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<string[]>> ListUserIdentities(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get a user's profile.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="SimpleNameValueCollection"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property containing profile property/value pairs.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <returns>.</returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<SimpleNameValueCollection> GetUserProfile(string authToken, string uniqueIdentifier);

        /// <summary>
        /// Update a user's profile.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The user's unique identifier.</param>
        /// <param name="profile">The new profile properties.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateUserProfile(string authToken, string uniqueIdentifier, SimpleNameValueCollection profile);

        /// <summary>
        /// Create a Checkbox user account.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userName">The name of the new user.</param>
        /// <param name="password">The new user's password.</param>
        /// <param name="emailAddress">The new user's email address.</param>
        /// <param name="profile">The new user's profile.</param>
        /// <param name="updateIfExists">When true, update existing users with the same name rather than throw a duplicate name exception.</param>
        /// <remarks>
        /// A user must have at least one role. By default new user's are implicitly assigned the "Report Viewer" and "Respondent" roles. 
        /// When roles are explicitly assigned to the user the default roles are removed.
        /// </remarks>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="string"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the unique identifier of the new user.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> CreateUser(string authToken, string userName, string password, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists);

        /// <summary>
        /// Create a new network user account.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userName">The name of the new user.</param>
        /// <param name="domain">The new user's domain.</param>
        /// <param name="profile">The new user's profile.</param>
        /// <param name="emailAddress">The new user's email address.</param>
        /// <param name="updateIfExists">When true, update existing users with the same name rather than throw a duplicate name exception.</param>
        /// <remarks>
        /// A user must have at least one role. By default new user's are implicitly assigned the "Report Viewer" and "Respondent" roles. 
        /// When roles are explicitly assigned to the user the default roles are removed.
        /// </remarks>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="string"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the unique identifier of the new user.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> CreateNetworkUser(string authToken, string userName, string domain, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists);

        /// <summary>
        /// Update a user's email address.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userIdentity">The unique identifier of the user to update.</param>
        /// <param name="newEmailAddress">The new email address.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> ChangeUserEmailAddress(string authToken, string userIdentity, string newEmailAddress);

        /// <summary>
        /// Update a user's password.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userIdentity">The unique identifier of the user to update.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> ChangeUserPassword(string authToken, string userIdentity, string newPassword);

        /// <summary>
        /// Update a network user's domain.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userName">The name of the user to update.</param>
        /// <param name="oldDomain">The user's current domain name.</param>
        /// <param name="newDomain">The new domain to assign to the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="string"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the unique identifier of the updated user.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> ChangeUserDomain(string authToken, string userName, string oldDomain, string newDomain);

        /// <summary>
        /// Delete a user and optionally the user's responses.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user to delete.</param>
        /// <param name="deleteResponses">Indicates whether or not the user's responses should be deleted as well.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>User responses are soft deleted.</remarks>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteUser(string authToken, string uniqueIdentifier, bool deleteResponses);

        /// <summary>
        /// Deletes one or more users. The user's responses can optionally be deleted as well.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifierList">The array of user unique identifier to be deleted.</param>
        /// <param name="deleteResponses">Indicates whether or not the user's responses should be deleted as well.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>User responses are soft deleted.</remarks>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteUsers(string authToken, string[] uniqueIdentifierList, bool deleteResponses);

        /// <summary>
        /// Delete all responses made by the the specified users.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifierList">An array of user unique identifiers.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>User responses are soft deleted.</remarks>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteResponsesOfUsers(string authToken, string[] uniqueIdentifierList);

        /// <summary>
        /// Delete a user's profile.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userIdentity">The user's unique identifier.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteUserProfile(string authToken, string userIdentity);

        /// <summary>
        /// Change a user's name.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of user to rename.</param>
        /// <param name="newUniqueIdentifier">The user's new unique identifier.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="string"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the unique identifier of the updated user.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> RenameUser(string authToken, string uniqueIdentifier, string newUniqueIdentifier);

        /// <summary>
        /// Indicates whether or not a user with a given unique identifier exists.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier to check.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="bool"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property indicates if the user exists or not.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<bool> UserExists(string authToken, string uniqueIdentifier);

        /// <summary>
        /// Get the list of roles assigned to a user.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The user's unique identifier.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an array of role names.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string[]> ListUserRoles(string authToken, string uniqueIdentifier);

        /// <summary>
        /// Get the list of all available user roles and the permissions that could be assigned to the role.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an array of role names.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<RoleData[]> ListAllAvailableUserRoles(string authToken);

        /// <summary>
        /// Assign a role to a user.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <param name="roleName">The name of the role to assign to the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>
        /// Available Roles:
        /// <list type="bullet">
        ///     <item>
        ///         <term>Group Administrator</term>
        ///         <description> - Is capable of managing user groups.</description>
        ///     </item>
        ///     <item>
        ///         <term>Report Administrator</term>
        ///         <description> - Is capable of creating and managing reports.</description>
        ///     </item>
        ///     <item>
        ///         <term>Report Viewer</term>
        ///         <description> - Is capable of viewing reports.</description> 
        ///     </item>
        ///     <item>
        ///         <term>Respondent</term>
        ///         <description> - Is capable of responding to surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>Survey Administrator</term>
        ///         <description> - Is capable creating, editing and activating surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>Survey Editor</term>
        ///         <description> - Is capable editing existing surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>System Administrator</term>
        ///         <description> - Is a super user that has no restricted actions.</description>
        ///     </item>
        ///     <item>
        ///         <term>User Administrator</term>
        ///         <description> - Is capable of creating and managing user accounts.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserToRole(string authToken, string uniqueIdentifier, string roleName);

        /// <summary>
        /// Revoke a role from the specified user.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <param name="roleName">The name of the role to remove the user from.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>
        /// Available Roles:
        /// <list type="bullet">
        ///     <item>
        ///         <term>Group Administrator</term>
        ///         <description> - Is capable of managing user groups.</description>
        ///     </item>
        ///     <item>
        ///         <term>Report Administrator</term>
        ///         <description> - Is capable of creating and managing reports.</description>
        ///     </item>
        ///     <item>
        ///         <term>Report Viewer</term>
        ///         <description> - Is capable of viewing reports.</description> 
        ///     </item>
        ///     <item>
        ///         <term>Respondent</term>
        ///         <description> - Is capable of responding to surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>Survey Administrator</term>
        ///         <description> - Is capable creating, editing and activating surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>Survey Editor</term>
        ///         <description> - Is capable editing existing surveys.</description>
        ///     </item>
        ///     <item>
        ///         <term>System Administrator</term>
        ///         <description> - Is a super user that has no restricted actions.</description>
        ///     </item>
        ///     <item>
        ///         <term>User Administrator</term>
        ///         <description> - Is capable of creating and managing user accounts.</description>
        ///     </item>
        /// </list>
        /// </remarks>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUserFromRole(string authToken, string uniqueIdentifier, string roleName);

        /// <summary>
        /// Get a paged, sorted, and filtered list of user groups.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{UserGroupData}"/> of <see cref="UserGroupData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains user groups that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserGroupData[]>> ListUserGroups(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get a paged, sorted, and filtered list of user groups.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{UserGroupData}"/> of <see cref="UserGroupData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains user groups that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserGroupData[]>> ListUserGroupsByPeriod(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName);

        /// <summary>
        /// Create a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userGroupName">The name of the new group.</param>
        /// <param name="userGroupDescription">The description of the new group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the id of the new group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> CreateUserGroup(string authToken, string userGroupName, string userGroupDescription);

        /// <summary>
        /// Copy a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group to copy.</param>
        /// <remarks>
        /// Note that a group's Access Control List (ACL) and default policy are not copied.
        /// </remarks>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the id of the new group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> CopyUserGroup(string authToken, int groupId);

        /// <summary>
        /// Copy one or more user groups.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupIdList">The list of group IDs to copy.</param>
        /// <remarks>
        /// Note that a group's Access Control List (ACL) and default policy are not copied.
        /// </remarks>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="int"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an array of user group IDs.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int[]> CopyUserGroups(string authToken, int[] groupIdList);

        /// <summary>
        /// Rename a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="newGroupName">The user group's new name.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> RenameUserGroup(string authToken, int groupId, string newGroupName);

        /// <summary>
        /// Update a user group's description.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="newGroupDescription">The new group description.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
 
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> ChangeUserGroupDescription(string authToken, int groupId, string newGroupDescription);

        /// <summary>
        /// Delete the specified user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="databaseId">The id of the user group to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteUserGroup(string authToken, int databaseId);

        /// <summary>
        /// Delete the specified user groups.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupIds">The list the user group IDs to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the count of user groups that were deleted.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string[]> DeleteUserGroups(string authToken, int[] groupIds);

        /// <summary>
        /// Add one or more users to a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="userUniqueIdentifiers">The list of user unique identifiers to add to the group.</param>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUsersToGroup(string authToken, string[] userUniqueIdentifiers, int groupId);

        /// <summary>
        /// Remove a user from a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="userUniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUserFromGroup(string authToken, int groupId, string userUniqueIdentifier);

        /// <summary>
        /// Remove multiple users from a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="userUniqueIdentifiers">A list of unique identifier to remove from the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed 
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUsersFromGroup(string authToken, int groupId, string[] userUniqueIdentifiers);

        /// <summary>
        /// Remove all users from a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed 
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveAllUsersFromGroup(string authToken, int groupId);

        /// <summary>
        /// delete all members in selected group from checkbox
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed 
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteAllGroupMembersFromCheckBox(string authToken, int groupId);

        /// <summary>
        /// Set the default policy permissions for a user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="permissions">The list of permissions.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SetGroupDefaultPolicyPermissions(string authToken, int groupId, string[] permissions);

        /// <summary>
        /// Grant a user access to the specified user group. Any existing permissions for the user
        /// on that group will be overwritten.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The id of the group to grant access to.</param>
        /// <param name="userUnqiueIdentifier">The unique identifier of the user to be granted access.</param>
        /// <param name="permissions">The list of permissions to grant to the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>
        /// Available Permissions:
        /// <list type="bullet">
        ///     <item>
        ///         <description>Group.Create</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.Delete</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.Edit</description> 
        ///     </item>		
        ///     <item>
        ///         <description>Group.ManageUsers</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.View</description>  
        ///     </item>
        /// </list>
        /// </remarks>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserToGroupAccessList(string authToken, int securedGroupId, string userUnqiueIdentifier, string[] permissions);

        /// <summary>
        /// Grant the permissible user group access to the secured user group. Any existing permissions for the 
        /// permissible group on the secured group will be overwritten.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The group that is having its permissions altered.</param>
        /// <param name="permissibleGroupId">The group that is being granted permission.</param>
        /// <param name="permissions">The list of permissions to grant to the permissible group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>
        /// Available Permissions:
        /// <list type="bullet">
        ///     <item>
        ///         <description>Group.Create</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.Delete</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.Edit</description> 
        ///     </item>		
        ///     <item>
        ///         <description>Group.ManageUsers</description> 
        ///     </item>
        ///     <item>
        ///         <description>Group.View</description>  
        ///     </item>
        /// </list>
        /// </remarks>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddGroupToGroupAccessList(string authToken, int securedGroupId, int permissibleGroupId, string[] permissions);

        /// <summary>
        /// Remove a user from a user group's access control list (ACL).
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The id of the group that is having its ACL altered.</param>
        /// <param name="userUniqueIdentifier">The unique identifier of the user to remove from the secured group's ACL.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUserFromGroupAccessList(string authToken, int securedGroupId, string userUniqueIdentifier);

        /// <summary>
        /// Remove the permissible user group from the secured user group's access control list (ACL).
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The id of the group that is having its ACL altered.</param>
        /// <param name="permissibleGroupId">The id of the group to be removed from the secured group's ACL.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveGroupFromGroupAccessList(string authToken, int securedGroupId, int permissibleGroupId);

        /// <summary>
        /// Get a group's default permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<string[]> ListGroupDefaultPolicyPermissions(string authToken, int groupId);

        /// <summary>
        /// List all access control list entries for the specified user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="AclEntry"/>s.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<AclEntry[]> ListAllGroupAccessListEntries(string authToken, int groupId);

        /// <summary>
        /// Retrieve the list of permissions that a user has been granted to the specified user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The id of the user group.</param>
        /// <param name="userUniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>
        /// The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.
        /// If the specified user is not on the secured group's access control list, the default policy permissions
        /// for the secured user group is returned otherwise the user's permissions are returned.
        /// </para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<string[]> ListGroupAccessListPermissionsForUser(string authToken, int securedGroupId, string userUniqueIdentifier);

        /// <summary>
        /// Retrieve the list of permissions that the permissible user group has been granted to the secured user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="securedGroupId">The id of the secured user group.</param>
        /// <param name="permissibleGroupId">The id of the permissible user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>
        /// The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.
        /// If the permissible user group is not on the secured group's access control list, the default policy permissions
        /// for the secured user group is returned otherwise the permissible groups permissions is returned.
        /// </para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<string[]> ListGroupAccessListPermissionsForGroup(string authToken, int securedGroupId, int permissibleGroupId);

        /// <summary>
        /// Retrieve a <see cref="UserGroupData"/> object which contains complete details about a user group.
        /// Available data includes, id, name, description, membership count and the creator's name.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupName">The name of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="UserGroupData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<UserGroupData> GetUserGroupByName(string authToken, string groupName);

        /// <summary>
        /// Retrieve a <see cref="UserGroupData"/> object which contains complete details about a user group.
        /// Available data includes, id, name, description, membership count and the creator's name.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="UserGroupData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<UserGroupData> GetUserGroupById(string authToken, int groupId);

        /// <summary>
        /// Get a paged, sorted, and filtered list of users in a specified user groups.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the group.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="UserData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of users that match the filter criteria and are members of the user group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> ListUserGroupMembers(string authToken, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get a filtered list of users NOT in the specified user group.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="groupId">The id of the group.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="UserData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of users that match the filter criteria and are NOT members of the user group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> ListUsersNotInGroup(string authToken, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get available new group users list 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> ListPotentialUsersForNewGroup(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get new group users list 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PagedListResult<UserData[]>> ListCurrentUsersForNewGroup(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);
        /// <summary>
        /// Search for users. Usernames and email addresses are the fields comparisons are made against.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="searchTerm">The value to search for.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="GroupedResult{T}"/> of <see cref="UserData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of users that match the search criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        /// <summary>
        /// Remove user from new group users list
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<bool> ListCurrentUsersForNewGroupRemoveUser(string authToken, string userId);

        /// <summary>
        /// Add user to new group users list
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<bool> ListCurrentUsersForNewGroupAddUser(string authToken, string userId);

        [WebGet]
        [OperationContract]
        ServiceOperationResult<GroupedResult<UserData>[]> SearchUsers(string authToken, string searchTerm);

        /// <summary>
        /// Search for user groups. The group's name and the name of the group's creator are the fields comparisons are made against.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="searchTerm">The value to search for.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="GroupedResult{T}"/> of <see cref="UserGroupData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of user groups that match the search criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>

        [WebGet]
        [OperationContract]
        ServiceOperationResult<GroupedResult<UserGroupData>[]> SearchGroups(string authToken, string searchTerm);
    }
}
