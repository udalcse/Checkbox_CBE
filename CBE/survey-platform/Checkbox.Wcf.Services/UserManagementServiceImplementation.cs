using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Prezza.Framework.Security;
using Checkbox.Analytics;
using Checkbox.Timeline;

namespace Checkbox.Wcf.Services
{
    public static class UserManagementServiceImplementation
    {

        #region User Management

        /// <summary>
        /// Get a list of user identities
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField">Filter field.</param>
        /// <param name="filterValue">Value of filter field to match.</param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <returns>List of user identities matching filters (or all identities if filter values are null).</returns>
        public static PagedListResult<string[]> ListUserIdentities(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, DateTime? startDate, string dateFieldName)
        {
            //Authorize user
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var paginationContext = CreatePaginationContext(pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, startDate, dateFieldName);

            var listResults = UserManager.ListUsers(callingPrincipal, paginationContext, provider);

            return new PagedListResult<string[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = listResults.ToArray()
            };
        }

        /// <summary>
        /// Get a list of user that have Tenant profile setup
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField">Filter field.</param>
        /// <param name="filterValue">Value of filter field to match.</param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <returns>List of user identities matching filters (or all identities if filter values are null).</returns>
        public static PagedListResult<string[]> ListUserByTenantId(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, DateTime? startDate, string dateFieldName)
        {
            //Authorize user
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var paginationContext = CreatePaginationContext(pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, startDate, dateFieldName);

            var listResults = UserManager.ListUsersByTenantId(callingPrincipal, paginationContext, provider);

            return new PagedListResult<string[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = listResults.ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="startDate"></param>
        /// <param name="dateFieldName"></param>
        /// <returns></returns>
        private static PaginationContext CreatePaginationContext(int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, DateTime? startDate, string dateFieldName)
        {
            switch (filterField)
            {
                case "UniqueIdentifier":
                case "Email":
                case "UserName":
                case "GUID":
                case "Created":
                case "CreatedBy":
                case "ModifiedDate":
                case "ModifiedBy":
                    break;
                default:
                    filterField = "UniqueIdentifier";
                    break;
            }

            switch (sortField)
            {
                case "UniqueIdentifier":
                case "Email":
                    break;
                default:
                    sortField = "UniqueIdentifier";
                    break;
            }

            switch (dateFieldName)
            {
                case "Created":
                case "ModifiedDate":
                    break;
                default:
                    sortField = string.Empty;
                    break;
            }

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            if (!string.IsNullOrEmpty(dateFieldName))
            {
                paginationContext.StartDate = startDate;
                paginationContext.EndDate = DateTime.Now;
                paginationContext.DateFieldName = dateFieldName;
            }

            return paginationContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="role"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<string[]> ListUserIdentitiesInRole(CheckboxPrincipal callingPrincipal, string role, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Authorize user
            if (!callingPrincipal.IsInRole("System Administrator") && !callingPrincipal.IsInRole("User Administrator"))
                throw new ServiceAuthorizationException();

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            var listResults = UserManager.ListUsersInRole(callingPrincipal, role, paginationContext);

            return new PagedListResult<string[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = listResults.ToArray()
            };
        }

        /// <summary>
        /// Get a list of user identities
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField">Filter field.</param>
        /// <param name="filterValue">Value of filter field to match.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>List of user identities matching filters (or all identities if filter values are null).</returns>
        public static PagedListResult<UserData[]> GetUsers(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            if (period == 0)
                dateFieldName = "";

            var listResult = ListUserIdentities(callingPrincipal, provider, pageNumber, pageSize, sortField,
                sortAscending, filterField, filterValue, TimelineManager.GetStartFilterDate(period), TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName));

            return new PagedListResult<UserData[]>
            {
                TotalItemCount = listResult.TotalItemCount,
                ResultPage = listResult.ResultPage.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
            };
        }

        public static PagedListResult<UserData[]> GetTenantUsers(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            if (period == 0)
                dateFieldName = "";

            var listResult = ListUserByTenantId(callingPrincipal, provider, pageNumber, pageSize, sortField,
                sortAscending, filterField, filterValue, TimelineManager.GetStartFilterDate(period), TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName));

            return new PagedListResult<UserData[]>
            {
                TotalItemCount = listResult.TotalItemCount,
                ResultPage = listResult.ResultPage.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
            };
        }

        /// <summary>
        /// Get a list of user identities
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField">Filter field.</param>
        /// <param name="filterValue">Value of filter field to match.</param>
        /// <returns>List of user identities matching filters (or all identities if filter values are null).</returns>
        public static PagedListResult<PageItemUserData[]> GetPageItemUsersData(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            // AD optimization
            // TODO : rewrite pagination logic for other providers
            if (!UserManager.IsActiveDirectoryMembershipProvider(provider))
            {
                var users = GetUsers(callingPrincipal, provider, pageNumber, pageSize, sortField, sortAscending,
                    filterField, filterValue, 0, string.Empty);

                return new PagedListResult<PageItemUserData[]>
                {
                    TotalItemCount = users.TotalItemCount,
                    ResultPage = users.ResultPage.Select(u => u.GetPageItemUserData()).ToArray()
                };
            }

            //Authorize user
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var paginationContext = CreatePaginationContext(pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, TimelineManager.GetStartFilterDate(0), string.Empty);
            var listResults = UserManager.GetPageItemUsersData(callingPrincipal, paginationContext, provider);

            return new PagedListResult<PageItemUserData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = listResults
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="role"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<UserData[]> GetUsersInRole(CheckboxPrincipal callingPrincipal, string role, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            var listResult = ListUserIdentitiesInRole(callingPrincipal, role, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);

            return new PagedListResult<UserData[]>
            {
                TotalItemCount = listResult.TotalItemCount,
                ResultPage = listResult.ResultPage.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
            };
        }
        /// <summary>
        /// Get the profile associated with a given user.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="userIdentity"></param>
        /// <returns>SimpleNameValue object for the specified identity.</returns>
        public static SimpleNameValueCollection GetUserProfile(CheckboxPrincipal callingPrincipal, string userIdentity)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var user = UserManager.GetUserPrincipal(userIdentity);
            if (UserDoesNotExist(user))
            {
                throw new UserDoesNotExistException(userIdentity);
            }

            return new SimpleNameValueCollection(ProfileManager.GetProfile(userIdentity));

        }

        /// <summary>
        /// Clear all information in a users profile
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userIdentity"></param>
        public static void DeleteUserProfile(CheckboxPrincipal callingPrincipal, string userIdentity)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            var userPrincipal = UserManager.GetUserPrincipal(userIdentity);
            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(userIdentity);
            }

            ProfileManager.DeleteProfile(userIdentity);
        }

        /// <summary>
        /// Update the user profile.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="userIdentity">Identity to update the profile for.</param>
        /// <param name="profile">Profile property collection.</param>
        public static void UpdateUserProfile(CheckboxPrincipal callingPrincipal, string userIdentity, SimpleNameValueCollection profile)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            var userPrincipal = UserManager.GetUserPrincipal(userIdentity);
            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(userIdentity);
            }

            foreach (var simpleNameValue in profile.NameValueList)
            {
                userPrincipal[simpleNameValue.Name] = simpleNameValue.Value;
            }

            //Save
            userPrincipal.SaveProfile();
        }

        /// <summary>
        /// Create a user with the given profile
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="userName">Name of the new user.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="domain">Domain for the new user.</param>
        /// <param name="emailAddress"></param>
        /// <param name="profile">Profile for the new user.</param>
        /// <param name="updateIfExists">When true, if a user with the same name exists, update that user.</param>
        /// <returns>Identity of newly-created user.</returns>
        private static string CreateUser(CheckboxPrincipal callingPrincipal,
                                         string userName,
                                         string password,
                                         string domain,
                                         string emailAddress,
                                         SimpleNameValueCollection profile,
                                         bool updateIfExists)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("A username is required.");
            }

            var uniqueIdentifier = UserManager.GenerateUniqueIdentifier(userName, domain);
            var userPrincipal = UserManager.GetUserPrincipal(uniqueIdentifier);
            var exist = UserManager.UserExists(uniqueIdentifier);

            if (exist && !updateIfExists)
            {
                throw new Exception("User already exists: " + userName);
            }

            //Create the user, if necessary
            if (!exist)
            {

                string status;

                userPrincipal = UserManager.CreateUser(userName, password, domain, emailAddress, callingPrincipal.Identity.Name, out status);

                if (userPrincipal == null)
                {
                    throw new Exception("An error occurred while creating the user: " + status);
                }
            }
            else
            {
                //If updating the user, update the password
                if ((Utilities.IsNotNullOrEmpty(password) || Utilities.IsNotNullOrEmpty(domain)))
                {
                    string status;
                    UserManager.UpdateUser(uniqueIdentifier, userName, domain, password, emailAddress, callingPrincipal.Identity.Name, out status);
                }
            }

            //Set the profile
            if (profile != null)
            {
                ValidateProperties(profile);

                foreach (var item in profile.NameValueList)
                {
                    userPrincipal[item.Name] = item.Value;
                }
            }

            return userPrincipal.Identity.Name;
        }

        /// <summary>
        /// Create a user with the given profile
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="userName">Name of the new user.</param>
        /// <param name="password">Password for the new user.</param>
        /// <param name="emailAddress"></param>
        /// <param name="profile">Profile for the new user.</param>
        /// <param name="updateIfExists">When true, if a user with the same name exists, update that user.</param>
        /// <returns>Identity of newly-created user.</returns>
        public static string CreateUser(CheckboxPrincipal callingPrincipal, string userName, string password, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists)
        {
            return CreateUser(callingPrincipal, userName, password, null, emailAddress, profile, updateIfExists);
        }

        /// <summary>
        /// Create a user with the given profile
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="userName">Name of the new user.</param>
        /// <param name="domain">Domain for the new user.</param>
        /// <param name="emailAddress"></param>
        /// <param name="profile">Profile for the new user.</param>
        /// <param name="updateIfExists">When true, if a user with the same name exists, update that user.</param>
        /// <returns>Identity of newly-created user.</returns>
        public static string CreateNetworkUser(CheckboxPrincipal callingPrincipal, string userName, string domain, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentException("A domain name is required.");
            }

            return CreateUser(callingPrincipal, userName, null, domain, emailAddress, profile, updateIfExists);
        }

        /// <summary>
        /// Update a user's email address.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user with the password to update.</param>
        /// <param name="newEmailAddress">New email address for the user.</param>
        public static void ChangeUserEmailAddress(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, string newEmailAddress)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            var user = UserManager.GetUserPrincipal(uniqueIdentifier);

            // Validate input
            if (UserDoesNotExist(user))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            // a user is allowed to set their email address to an empty string
            if (!string.IsNullOrWhiteSpace(newEmailAddress))
            {
                var validator = new EmailValidator();
                if (!validator.Validate(newEmailAddress.Trim()))
                {
                    throw new ArgumentException("The new email address is invalid.");
                }
            }

            string status;
            UserManager.UpdateUser(uniqueIdentifier, null, null, null, newEmailAddress, callingPrincipal.Identity.Name, out status);
        }

        /// <summary>
        /// Update a users password.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user with the password to update.</param>
        /// <param name="newPassword">New password for the user.</param>
        public static void ChangeUserPassword(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, string newPassword)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            var user = UserManager.GetUserPrincipal(uniqueIdentifier);

            //Verify the user to be updated actually exists
            if (UserDoesNotExist(user))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            if (Utilities.IsNullOrEmpty(newPassword))
            {
                throw new Exception("Password cannot be null or empty.");
            }

            string status;

            UserManager.UpdateUser(uniqueIdentifier, null, null, newPassword, null, callingPrincipal.Identity.Name, out status);
        }

        /// <summary>
        /// Update a user's domain.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="userName">Unique identifier of the user with the password to update.</param>
        /// <param name="oldDomain">The old domain.</param>
        /// <param name="newDomain">The new domain</param>
        public static string ChangeUserDomain(CheckboxPrincipal callingPrincipal, string userName, string oldDomain, string newDomain)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            //Validate inputs
            if (Utilities.IsNullOrEmpty(oldDomain))
            {
                throw new ArgumentException("The user's current domain cannot be null or an empty string.");
            }

            if (Utilities.IsNullOrEmpty(newDomain))
            {
                throw new Exception("The user's new domain cannot be null or an empty string.");
            }

            string status;

            var userUniqueIdentifier = Utilities.IsNotNullOrEmpty(oldDomain)
                ? UserManager.GenerateUniqueIdentifier(userName, oldDomain)
                : userName;

            //Confirm that the user exists
            var user = UserManager.GetUserPrincipal(userUniqueIdentifier);
            if (UserDoesNotExist(user))
            {
                throw new UserDoesNotExistException(userUniqueIdentifier);
            }

            user = UserManager.UpdateUser(
                userUniqueIdentifier,
                userName,
                newDomain,
                null,
                null,
                callingPrincipal.Identity.Name,
                out status);

            return user != null ? user.Identity.Name : status;
        }

        /// <summary>
        /// Ensures that all profile properties in a collection are existing Custom User Fields.
        /// </summary>
        /// <param name="newProfileProperties"></param>
        /// <exception cref="Exception"></exception>
        private static void ValidateProperties(SimpleNameValueCollection newProfileProperties)
        {
            var profileProperties = ProfileManager.ListPropertyNames();

            var invalidProperties = new StringBuilder();

            foreach (var item in newProfileProperties.NameValueList.Where(item => item.Name != null).Where(item => !item.Name.Equals("Email", StringComparison.InvariantCultureIgnoreCase)
                                                                                                                   && !profileProperties.Contains(item.Name, StringComparer.InvariantCultureIgnoreCase)))
            {
                invalidProperties.AppendFormat("{0}, ", item.Name);
            }

            if (invalidProperties.Length > 0)
            {
                throw new Exception("Invalid properties found in profile update: " + invalidProperties);
            }
        }

        /// <summary>
        /// Rename a user
        /// </summary>
        /// <param name="callingPrincipal">Login context for user attempting to rename a user</param>
        /// <param name="uniqueIdentifier">Unique identifier of user to rename.</param>
        /// <param name="newUniqueIdentifier">New unique identifier for user.</param>
        public static string RenameUser(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, string newUniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException("The original username is required.");
            }

            if (string.IsNullOrWhiteSpace(newUniqueIdentifier))
            {
                throw new ArgumentException("The new username is required.");
            }

            var userPrincipal = UserManager.GetUserPrincipal(uniqueIdentifier);
            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            if (UserExists(callingPrincipal, newUniqueIdentifier))
            {
                throw new Exception("User already exists: " + newUniqueIdentifier);
            }

            string status;
            var updatedUser = UserManager.UpdateUser(uniqueIdentifier, newUniqueIdentifier, null, null, null, callingPrincipal.Identity.Name, out status);

            return updatedUser.Identity.Name;
        }

        /// <summary>
        /// Unlock the user
        /// </summary>
        /// <param name="callingPrincipal">Login context for user attempting to unlock a user</param>
        /// <param name="uniqueIdentifier">Unique identifier of user to unlock.</param>        
        public static bool UnlockUser(CheckboxPrincipal callingPrincipal, string uniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException("The username is required.");
            }

            UserManager.UnlockUser(uniqueIdentifier);

            return true;
        }

        /// <summary>
        /// Lock the user
        /// </summary>
        /// <param name="callingPrincipal">Login context for user attempting to lock a user</param>
        /// <param name="uniqueIdentifier">Unique identifier of user to unlock.</param>        
        public static bool LockUser(CheckboxPrincipal callingPrincipal, string uniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException("The username is required.");
            }

            UserManager.LockUser(uniqueIdentifier);

            return true;
        }

        /// <summary>
        /// Delete a user and, optionally, any responses entered by that user.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the user attempting the delete.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to delete.</param>
        /// <param name="deleteResponses">Indicate if the user's responses should be deleted as well.</param>
        public static void DeleteUser(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, bool deleteResponses)
        {
            string status;
            //Delete user identity
            var successful = UserManager.DeleteUser(callingPrincipal, uniqueIdentifier, deleteResponses, out status);

            if (successful)
            {
                //Delete profile
                DeleteUserProfile(callingPrincipal, uniqueIdentifier);
            }
            else
            {
                throw new Exception(status);
            }
        }

        /// <summary>
        /// Delete a users and  any responses entered by this users.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the user attempting the delete.</param>
        /// <param name="uniqueIdentifierList">Array of unique identifiers of the users to delete.</param>
        /// <param name="deleteResponses">Indicate if the users' responses should be deleted as well.</param>
        public static void DeleteUsers(CheckboxPrincipal callingPrincipal, string[] uniqueIdentifierList, bool deleteResponses, out string status)
        {
            status = string.Empty;

            var currentUserName = callingPrincipal.Identity.Name;

            foreach (var uniqueIdentifier in uniqueIdentifierList)
            {
                if (currentUserName.Equals(uniqueIdentifier))
                {
                    status = WebTextManager.GetText("/users/groupDashboardTemplate/deleteHimself");
                    continue;
                }

                try
                {
                    DeleteUser(callingPrincipal, uniqueIdentifier, deleteResponses);
                }
                catch (Exception ex)
                {
                    status += "  " + ex.Message;
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete all responses of the selected users
        /// </summary>
        /// <param name="callingPrincipal">Login context for the user attempting the delete.</param>
        /// <param name="uniqueIdentifierList">Array of unique identifiers of the users which responses should be deleted.</param>
        public static void DeleteResponsesOfUsers(CheckboxPrincipal callingPrincipal, string[] uniqueIdentifierList)
        {
            Security.AuthorizeUserContext(callingPrincipal, GroupManager.GetEveryoneGroup(), "Group.ManageUsers");

            foreach (var uniqueIdentifier in uniqueIdentifierList)
            {
                ResponseManager.DeleteUserResponses(uniqueIdentifier);
            }
        }

        /// <summary>
        /// Checks if a user exists with the given unique identifier
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user</param>
        /// <param name="uniqueIdentifier">Unique identifier to check</param>
        /// <returns>Flag indicating if the given unique identifier is already in use</returns>
        public static bool UserExists(CheckboxPrincipal callingPrincipal, string uniqueIdentifier)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var user = UserManager.GetUserPrincipal(uniqueIdentifier);

            return user != null && user.Identity.AuthenticationType != UserManager.EXTERNAL_USER_AUTHENTICATION_TYPE;
        }

        /// <summary>
        /// Find users by name/email address
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<UserData>[] SearchUsers(CheckboxPrincipal callingPrincipal, string searchTerm)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var paginationContext = new PaginationContext
                                        {
                                            SortField = "UniqueIdentifier",
                                            SortAscending = true,
                                            FilterValue = searchTerm,
                                            FilterField = "UniqueIdentifier"
                                        };

            var usersByName = UserManager.ListUsers(callingPrincipal, paginationContext);

            paginationContext.FilterField = "Email";
            var usersByEmail = UserManager.ListUsers(callingPrincipal, paginationContext);

            var result = new List<GroupedResult<UserData>>
                             {
                                 new GroupedResult<UserData>
                                     {
                                         GroupKey = "matchingUserName",
                                         GroupResults =
                                             usersByName.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
                                     },
                                 new GroupedResult<UserData>
                                     {
                                         GroupKey = "matchingEmailAddress",
                                         GroupResults =
                                             usersByEmail.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
                                     }
                             };

            return result.ToArray();
        }

        #endregion

        #region User Role Management

        /// <summary>
        /// Get the list of all available user roles and the permissions that could be assigned to the role.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <returns></returns>
        public static RoleData[] ListAllAvailableUserRoles(CheckboxPrincipal callingPrincipal)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.ManageUsers");

            return (from role in RoleManager.ListRoles()
                    let roleObj = RoleManager.GetRole(role)
                    select new RoleData
                               {
                                   Name = role, Description = roleObj.Description, Permissions = roleObj.Permissions.ToArray()
                               }).ToArray();
        }

        /// <summary>
        /// List the roles a user is a member of
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to list roles for.</param>
        /// <returns>Array of user role names.</returns>
        public static string[] ListUserRoles(CheckboxPrincipal callingPrincipal, string uniqueIdentifier)
        {
            //Make sure the calling user can manage users
            Security.AuthorizeUserContext(callingPrincipal, GroupManager.GetEveryoneGroup(), "Group.ManageUsers");

            var userPrincipal = UserManager.GetUserPrincipal(uniqueIdentifier);

            // Validate input
            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            return userPrincipal.GetRoles();
        }

        /// <summary>
        /// Add a user to a role.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to add to a role.</param>
        /// <param name="roleName">Name of the role to add the user to.</param>
        public static void AddUserToRole(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, string roleName)
        {
            //Make sure the calling user can manage users
            Security.AuthorizeUserContext(callingPrincipal, GroupManager.GetEveryoneGroup(), "Group.ManageUsers");

            //Confirm that the user exists
            var user = UserManager.GetUserPrincipal(uniqueIdentifier);
            if (UserDoesNotExist(user))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("The role name can not be null or an empty string.");
            }

            const string sysAdmin = "System Administrator";
            if (roleName == sysAdmin && !callingPrincipal.IsInRole(sysAdmin))
            {
                throw new Exception("Only System Administrator could assign this role.");
            }

            RoleManager.AddUserToRoles(uniqueIdentifier, new[] { roleName });
        }

        /// <summary>
        /// Remove a user from a role.
        /// </summary>
        /// <param name="callingPrincipal">Login context for the calling user.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from the role.</param>
        /// <param name="roleName">Name of the role to remove the user from.</param>
        public static void RemoveUserFromRole(CheckboxPrincipal callingPrincipal, string uniqueIdentifier, string roleName)
        {
            //Make sure the calling user can manage users
            Security.AuthorizeUserContext(callingPrincipal, GroupManager.GetEveryoneGroup(), "Group.ManageUsers");

            // Validate input
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("A role name is required.");
            }

            var userPrincipal = UserManager.GetUserPrincipal(uniqueIdentifier);
            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(uniqueIdentifier);
            }

            const string sysAdmin = "System Administrator";
            if (roleName == sysAdmin && !callingPrincipal.IsInRole(sysAdmin))
            {
                throw new Exception("Only System Administrator could unassign this role.");
            }

            RoleManager.RemoveUserFromRoles(uniqueIdentifier, new[] { roleName });
        }

        #endregion


        #region Group Management

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<UserGroupData>[] SearchGroups(CheckboxPrincipal callingPrincipal, string searchTerm)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var paginationContext = new PaginationContext
                                        {
                                            SortField = "GroupName",
                                            SortAscending = true,
                                            FilterValue = searchTerm,
                                            PermissionJoin = PermissionJoin.Any,
                                            Permissions = new List<string>(new[] { "Group.View" }),
                                            FilterField = "GroupName"
                                        };

            var resultsByName = GroupManager.ListAccessibleGroups(callingPrincipal, paginationContext, true);

            paginationContext.FilterField = "CreatedBy";
            var resultsByOwner = GroupManager.ListAccessibleGroups(callingPrincipal, paginationContext, false);

            var result = new List<GroupedResult<UserGroupData>>
                             {
                                 new GroupedResult<UserGroupData>
                                     {
                                         GroupKey = "matchingName",
                                         GroupResults =
                                             resultsByName.Select(groupId => GetUserGroupById(callingPrincipal, groupId))
                                             .ToArray()
                                     },
                                 new GroupedResult<UserGroupData>
                                     {
                                         GroupKey = "matchingOwner",
                                         GroupResults =
                                             resultsByOwner.Select(
                                                 groupId => GetUserGroupById(callingPrincipal, groupId)).ToArray()
                                     }
                             };

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="userUniqueIdentifier"> </param>
        /// <returns></returns>
        private static UserGroupData ToUserGroupData(Group g, string userUniqueIdentifier)
        {
            return new UserGroupData
            {
                DatabaseId = g.ID.Value,
                Name = g.Name,
                Description = g.Description,
                MemberCount = g.MemberCount,
                CreatedBy = g.CreatedBy,
                CanCopy = AuthorizationServiceImplementation.UserHasRolePermission(userUniqueIdentifier, "Group.Create"),
                CanDelete = AuthorizationServiceImplementation.AuthorizeAccess(userUniqueIdentifier, SecuredResourceType.UserGroup, g.ID.ToString(), "Group.Delete")
            };
        }

        /// <summary>
        /// Get information about a user group.  Null is returned if a group with the specified
        /// name can't be found.
        /// </summary>
        /// <param name="callingPrincipal">User context of calling user.</param>
        /// <param name="groupName">Name of group.</param>
        /// <returns>UserGroupData object or NULL if group not found.</returns>
        public static UserGroupData GetUserGroupByName(CheckboxPrincipal callingPrincipal, string groupName)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(groupName);

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupName);
            }

            return ToUserGroupData(g, callingPrincipal.Identity.Name);
        }

        /// <summary>
        /// Get information about a user group.  Null is returned if a group with the specified
        /// id can't be found.
        /// </summary>
        /// <param name="callingPrincipal">Guid context representing the web services user.</param>
        /// <param name="groupId">Database id of the group.</param>
        /// <returns>UserGroupData object or NULL if group is not found.</returns>
        public static UserGroupData GetUserGroupById(CheckboxPrincipal callingPrincipal, int groupId)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(groupId);

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupId);
            }

            return ToUserGroupData(g, callingPrincipal.Identity.Name);
        }

        /// <summary>
        /// Get a list of user groups
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>Array of user group info objects.</returns>
        public static PagedListResult<UserGroupData[]> ListUserGroups(CheckboxPrincipal callingPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");
            if (period == 0)
                dateFieldName = "";

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = "GroupName",
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue,
                Permissions = new List<string> { "Group.View" },
                DateFieldName = TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName),
                StartDate = TimelineManager.GetStartFilterDate(period) 
            };

            var groupIds = GroupManager.ListAccessibleGroups(callingPrincipal, paginationContext, false);

            return new PagedListResult<UserGroupData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = groupIds.Select(groupId => GetUserGroupById(callingPrincipal, groupId)).ToArray()
            };
        }

        /// <summary>
        /// Create a user group
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user</param>
        /// <param name="userGroupName">Name of the new group.</param>
        /// <param name="userGroupDescription">Description of the new group.</param>
        /// <returns>ID of the newly created group.  If the value is negative, a group was not successfully created.</returns>
        public static int CreateUserGroup(CheckboxPrincipal callingPrincipal, string userGroupName, string userGroupDescription)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Create");

            if (Utilities.IsNullOrEmpty(userGroupName))
            {
                throw new Exception("No group name specified.");
            }

            //Trim the name
            userGroupName = userGroupName.Trim();

            if (GroupManager.GetGroup(userGroupName) != null)
            {
                throw new Exception("User group already exists: " + userGroupName);
            }

            var g = GroupManager.CreateGroup(userGroupName, userGroupDescription);

            if (g != null)
            {
                g.Modifier = callingPrincipal.Identity.Name;
                g.Save();

                if (g.ID.HasValue)
                {
                    return g.ID.Value;
                }
            }

            return -1;
        }

        /// <summary>
        /// Copy a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="groupId">ID of the group, which should be copied.</param>
        /// <returns>ID of the newly created group.  If the value is negative, a group was not successfully created.</returns>
        public static int CopyUserGroup(CheckboxPrincipal callingPrincipal, int groupId)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Create");
            var languageCode = TextManager.DefaultLanguage;

            var groupToCopy = GroupManager.GetGroup(groupId);

            // Validate input
            if (groupToCopy == null || !groupToCopy.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupId);
            }

            try
            {
                var group = GroupManager.CopyGroup(GroupManager.GetGroup(groupId), languageCode, callingPrincipal);

                return group.ID ?? -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>
        /// Copy a user groups.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user.</param>
        /// <param name="groupIdList">ID list of the groups, which should be copied.</param>
        /// <returns>ID list of the newly created groups.</returns>
        public static int[] CopyUserGroups(CheckboxPrincipal callingPrincipal, int[] groupIdList)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Create");
            var languageCode = TextManager.DefaultLanguage;
            var newIdList = new List<int>();

            try
            {
                newIdList.AddRange(from groupId in groupIdList select GroupManager.CopyGroup(GroupManager.GetGroup(groupId), languageCode, callingPrincipal) into @group where @group.ID.HasValue select @group.ID.Value);

                return newIdList.ToArray();
            }
            catch (Exception)
            {
                return newIdList.ToArray();
            }
        }

        /// <summary>
        /// Update the name of a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="newGroupName">New name of the group.</param>
        /// <returns>new name of the group</returns>
        public static string RenameUserGroup(CheckboxPrincipal callingPrincipal, int groupId, string newGroupName)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var currentGroupWithName = GroupManager.GetGroup(newGroupName);
            var currentGroupWithId = GroupManager.GetGroup(groupId);

            // Validate input
            if (currentGroupWithId == null || !currentGroupWithId.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupId);
            }

            if (currentGroupWithName != null && currentGroupWithName.ID != groupId)
            {
                throw new Exception("Group already exists: " + newGroupName);
            }

            Security.AuthorizeUserContext(callingPrincipal, currentGroupWithId, "Group.Edit");

            currentGroupWithId.Name = newGroupName;
            currentGroupWithId.Save();

            return GroupManager.GetGroup(newGroupName).Name;
        }

        /// <summary>
        /// Update the description of a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context identifying the requesting user</param>
        /// <param name="groupId">The id of the user group.</param>
        /// <param name="newGroupDescription">New description of the group.</param>
        public static void ChangeUserGroupDescription(CheckboxPrincipal callingPrincipal, int groupId, string newGroupDescription)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");
            var currentGroupWithId = GroupManager.GetGroup(groupId);

            // Validate input
            if (currentGroupWithId == null || !currentGroupWithId.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupId);
            }

            Security.AuthorizeUserContext(callingPrincipal, currentGroupWithId, "Group.Edit");
            currentGroupWithId.Description = newGroupDescription;
            currentGroupWithId.Save();
        }

        /// <summary>
        /// Delete the specified user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseID">Database id of the user group to delete.</param>
        public static void DeleteUserGroup(CheckboxPrincipal callingPrincipal, int databaseID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Delete");

            var g = GroupManager.GetGroup(databaseID);

            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(databaseID);
            }

            Security.AuthorizeUserContext(callingPrincipal, g, "Group.Delete");

            GroupManager.DeleteGroup(databaseID, callingPrincipal);

        }

        /// <summary>
        /// Delete the specified groups
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseIdList">Database id list of the user groups to delete.</param>
        /// <param name="notDeletedGroupsList">List of groups that were not deleted.</param>
        /// <returns>Number of actually deleted groups</returns>
        public static string[] DeleteUserGroups(CheckboxPrincipal callingPrincipal, int[] databaseIdList, out string[] notDeletedGroupsList)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Delete");

            var deletedGroups = new List<string>();
            var notDeletedGroups = new List<string>(databaseIdList.Select(id => id.ToString()));

            try
            {
                foreach (int databaseId in databaseIdList)
                {
                    var g = GroupManager.GetGroup(databaseId);

                    if (g == null)
                        throw new UserGroupDoesNotExistException(databaseId);

                    Security.AuthorizeUserContext(callingPrincipal, g, "Group.Delete");

                    string name = null;
                    var group = GroupManager.GetGroup(databaseId);

                    if (@group != null)
                        name = GroupManager.GetGroup(databaseId).Name;

                    GroupManager.DeleteGroup(databaseId, callingPrincipal);
                    notDeletedGroups.Remove(databaseId.ToString());
                    deletedGroups.Add(name);
                }
            }
            catch (Exception)
            {
                notDeletedGroupsList = notDeletedGroups.ToArray();
                throw;
            }

            notDeletedGroupsList = notDeletedGroups.ToArray();
            return deletedGroups.ToArray();
        }

        /// <summary>
        /// List the members of a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseID">Database id of the group to list members for.</param>
        /// <returns>Array of user unique identifiers.</returns>
        public static string[] ListUserGroupMembers(CheckboxPrincipal callingPrincipal, int databaseID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(databaseID);

            if (g != null)
            {
                Security.AuthorizeUserContext(callingPrincipal, g, "Group.View");
                return g.GetUserIdentifiers()
                    .OrderBy(identifier => identifier)
                    .ToArray();
            }

            return new string[] { };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="provider"></param>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<PageItemUserData[]> ListPageItemUserDataForGroup(CheckboxPrincipal callingPrincipal, string provider, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(groupId);

            //Return empty list if group not loaded
            if (g == null)
            {
                return new PagedListResult<PageItemUserData[]> { ResultPage = new PageItemUserData[] { } };
            }

            //Check access to particular group
            Security.AuthorizeUserContext(callingPrincipal, g, "Group.View");

            //Search for users matching term
            var pagedUserList = GetPageItemUsersData(callingPrincipal, provider, pageNumber, pageSize, sortField, sortAscending, "UniqueIdentifier", filterValue);

            var groupMemberList = new List<string>(g.GetUserIdentifiers());
            groupMemberList.Sort();

            foreach (var user in pagedUserList.ResultPage)
            {
                user.IsInList = groupMemberList.Any(m => user.UniqueIdentifier.Equals(m, StringComparison.InvariantCultureIgnoreCase));
            }

            return pagedUserList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<UserData[]> ListUsersNotInGroup(CheckboxPrincipal callingPrincipal, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(groupId);

            //Return empty list if group not loaded
            if (g == null)
            {
                return new PagedListResult<UserData[]> { ResultPage = new UserData[] { } };
            }

            //Check access to particular group
            Security.AuthorizeUserContext(callingPrincipal, g, "Group.View");

            //Search for users matching term
            var allUserList = UserManager.ListUsers(
                callingPrincipal,
                new PaginationContext { FilterField = "UniqueIdentifier", FilterValue = filterValue, SortAscending = sortAscending });

            var groupMemberList = new List<string>(g.GetUserIdentifiers());
            groupMemberList.Sort();

            var availableList =
                allUserList
                    .Where(
                        userIdentifier =>
                        groupMemberList.BinarySearch(userIdentifier, StringComparer.InvariantCultureIgnoreCase) < 0)
                    .Distinct();

            var pagedList = pageNumber > 0 && pageSize > 0
                ? availableList.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                : availableList;

            return new PagedListResult<UserData[]>
            {
                TotalItemCount = availableList.Count(),
                ResultPage = pagedList.Select(userId => GetUserData(callingPrincipal, userId)).ToArray()
            };
        }

        /// <summary>
        /// Get available list of new group users
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<UserData[]> ListPotentialUsersForNewGroup(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var listResult = ListUserIdentities(callingPrincipal, provider, pageNumber, pageSize, sortField,
                sortAscending, "UniqueIdentifier", filterValue, null, string.Empty);

            var resultList = listResult.ResultPage.Select(userName => GetUserData(callingPrincipal, userName)).ToArray();

            var currentUsers = GroupManager.ListCurrentUsersForNewGroup(callingPrincipal, filterValue);

            //mark available users if they are already in current list
            foreach (var userData in resultList)
            {
                if (currentUsers.Contains(userData.UniqueIdentifier))
                {
                    userData.IsInList = true;
                }
            }
            return new PagedListResult<UserData[]>
            {
                TotalItemCount = listResult.TotalItemCount,
                ResultPage = resultList
            };
        }

        /// <summary>
        /// Get available list of new group users
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<PageItemUserData[]> ListPageItemPotentialUsersForNewGroup(CheckboxPrincipal callingPrincipal, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var listResult = ListUserIdentities(callingPrincipal, provider, pageNumber, pageSize, sortField,
                sortAscending, "UniqueIdentifier", filterValue, null, string.Empty);

            var currentUsers = GroupManager.ListCurrentUsersForNewGroup(callingPrincipal, filterValue);

            var resultList = listResult.ResultPage.Select(uniqueIdentifier => new PageItemUserData
            {
                UniqueIdentifier = uniqueIdentifier,
                IsInList = currentUsers.Contains(uniqueIdentifier)
            });

            return new PagedListResult<PageItemUserData[]>
            {
                TotalItemCount = listResult.TotalItemCount,
                ResultPage = resultList.ToArray()
            };
        }

        /// <summary>
        /// Get list of new group users
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<UserData[]> ListCurrentUsersForNewGroup(CheckboxPrincipal callingPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");
            var currentUserList = GroupManager.ListCurrentUsersForNewGroup(callingPrincipal, filterValue);
            var pagedList = pageNumber > 0 && pageSize > 0
            ? currentUserList.Skip((pageNumber - 1) * pageSize).Take(pageSize)
            : currentUserList;
            return new PagedListResult<UserData[]>
            {
                TotalItemCount = currentUserList.Count(),
                //order user by unique identifier
                ResultPage = (sortAscending) ? 
                    pagedList.Select(userId => GetUserData(callingPrincipal, userId)).OrderBy(user => user.UniqueIdentifier).ToArray() 
                    : pagedList.Select(userId => GetUserData(callingPrincipal, userId)).OrderByDescending(user => user.UniqueIdentifier).ToArray()
            };
         }

        /// <summary>
        /// Add user to new group users list
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userId"></param>
        public static bool ListCurrentUsersForNewGroupAddUser(CheckboxPrincipal callingPrincipal, string userId)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");
            return GroupManager.ListCurrentUsersForNewGroupAddUser(callingPrincipal, userId);
        }

        /// <summary>
        /// Remove user from new group users list
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userId"></param>
        public static bool ListCurrentUsersForNewGroupRemoveUser(CheckboxPrincipal callingPrincipal, string userId)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");
            return GroupManager.ListCurrentUsersForNewGroupRemoveUser(callingPrincipal, userId);
        }

        /// <summary>
        /// List the members of a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseID">Database id of the group to list members for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>Array of user unique identifiers.</returns>
        public static PagedListResult<UserData[]> ListUserGroupMembers(CheckboxPrincipal callingPrincipal, int databaseID, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var g = GroupManager.GetGroup(databaseID);

            //Return empty list if group not loaded
            if (g == null)
            {
                return new PagedListResult<UserData[]> { ResultPage = new UserData[] { } };
            }

            //Check access to particular group
            Security.AuthorizeUserContext(callingPrincipal, g, "Group.View");

            //List group users
            var groupUserList = g.GetUserIdentifiers();

            //Handle case where sorting, filtering, and paging can be done before loading user data
            if ((Utilities.IsNullOrEmpty(filterValue) || "UniqueIdentifier".Equals(filterField, StringComparison.InvariantCultureIgnoreCase))
                && (Utilities.IsNullOrEmpty(sortField) || "UniqueIdentifier".Equals(sortField, StringComparison.InvariantCultureIgnoreCase)))
            {
                //Either not filtering and not sorting or only doing those operations on user list, so pare the list down
                var filteredList = Utilities.IsNullOrEmpty(filterValue)
                    ? groupUserList
                    : groupUserList.Where(userIdentifier => userIdentifier.IndexOf(filterValue, StringComparison.InvariantCultureIgnoreCase) >= 0);

                var sortedList = Utilities.IsNullOrEmpty(sortField)
                    ? filteredList
                    : sortAscending
                        ? filteredList.OrderBy(userName => userName)
                        : filteredList.OrderByDescending(userName => userName);

                var pagedList = pageNumber > 0 && pageSize > 0
                    ? sortedList.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    : sortedList;

                return new PagedListResult<UserData[]>
                {
                    TotalItemCount = sortedList.Count(),
                    ResultPage = pagedList.Select(userName => GetUserData(callingPrincipal, userName)).ToArray()
                };
            }

            //We have to use a user data list for some operations if we get to this point, so build the list.
            IEnumerable<string> userDataListSource;

            //Handle case where filtering can be done first
            if (Utilities.IsNullOrEmpty(filterValue) || "UniqueIdentifier".Equals(filterField, StringComparison.InvariantCultureIgnoreCase))
            {
                //Either not filtering and not sorting or only doing those operations on user list, so pare the list down
                userDataListSource = Utilities.IsNullOrEmpty(filterValue)
                    ? groupUserList
                    : groupUserList.Where(userIdentifier => userIdentifier.IndexOf(filterValue, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            else
            {
                userDataListSource = new List<string>(groupUserList);
            }

            //Build user data list
            var userDataList = userDataListSource.Select(userName => GetUserData(callingPrincipal, userName));

            //Filter, if necessary
            var filteredUserDataList =
                Utilities.IsNullOrEmpty(filterValue)
                || "UniqueIdentifier".Equals(filterField, StringComparison.InvariantCultureIgnoreCase)
                    ? userDataList
                    : userDataList.Where(
                        userData =>
                        userData.Profile[filterField] != null && userData.Profile[filterField].IndexOf(filterValue, StringComparison.InvariantCultureIgnoreCase) >= 0);

            //Sort, if necessary with special cases for Email and UniqueIdentifier
            var sortedUserDataList = Utilities.IsNotNullOrEmpty(sortField)
                    ? "UniqueIdentifier".Equals(sortField, StringComparison.InvariantCultureIgnoreCase)
                        ? sortAscending
                                ? filteredUserDataList.OrderBy(userData => userData.UniqueIdentifier)               //User name ascending
                                : filteredUserDataList.OrderByDescending(userData => userData.UniqueIdentifier)     //User name descending
                        : "Email".Equals(sortField, StringComparison.InvariantCultureIgnoreCase)
                            ? sortAscending
                                ? filteredUserDataList.OrderBy(userData => userData.Email)                          //Email ascending
                                : filteredUserDataList.OrderByDescending(userData => userData.Email)                //Email descending
                            : sortAscending
                                ? filteredUserDataList.OrderBy(userData => userData.Profile[sortField])             //Profile field ascending
                                : filteredUserDataList.OrderByDescending(userData => userData.Profile[sortField])   //Profile field descending
                    : filteredUserDataList;     //No filter

            //Page, if necessary
            var pagedUserDataList = pageNumber > 0 && pageSize > 0
                ? sortedUserDataList.Skip(pageNumber - 1 * pageSize).Take(pageSize)
                : sortedUserDataList;

            //Return result
            return new PagedListResult<UserData[]>
            {
                TotalItemCount = sortedUserDataList.Count(),
                ResultPage = pagedUserDataList.ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userUniqueIdentifer"></param>
        /// <param name="inList"></param>
        /// <returns></returns>
        public static UserData GetUserData(CheckboxPrincipal callingPrincipal, string userUniqueIdentifer, bool inList = false)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.View");

            var userPrincipal = UserManager.GetUserPrincipal(userUniqueIdentifer);

            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(userUniqueIdentifer);
            }

            var profileProperties = ProfileManager.GetProfileProperties(userPrincipal.Identity.Name,
                userId: userPrincipal.UserGuid);

            var allEmail = profileProperties.Any()
               ? profileProperties.Where(item => item.FieldType == CustomFieldType.Email && !string.IsNullOrWhiteSpace(item.Value))
                   .Select(item => item.Value)
                   .ToList()
               : new List<string>();

            if (!string.IsNullOrWhiteSpace(userPrincipal.Email))
            {
                allEmail.Insert(0, userPrincipal.Email);
            }

            var result = new UserData
            {
                AuthenticationType = userPrincipal.Identity.AuthenticationType,
                UniqueIdentifier = userPrincipal.Identity.Name,
                Email = userPrincipal.Email,
                UserGuid = userPrincipal.UserGuid,
                Profile = new SimpleNameValueCollection(profileProperties
                    .ToDictionary(key => key.Name, value => value.Value)),
                RoleMemberships = userPrincipal.GetRoles(),
                GroupMemberships = GroupManager.GetGroupMemberships(userUniqueIdentifer).Select(p => p.Name).ToArray(),
                LockedOut = userPrincipal.LockedOut,
                IsInList = inList,
                AllEmails = allEmail
               
            };

            var optOutdata = InvitationManager.GetOptedOutSurveyListByEmail(userPrincipal.Email);
            var optedOutOfSender = optOutdata.LastOrDefault(o => o.Type > (int) InvitationOptOutType.BlockSurvey);
            if (optedOutOfSender != null)
            {
                var optOutType = (InvitationOptOutType)optedOutOfSender.Type;
                result.OptedOutFromAccount = TextManager.GetText("/users/userDashboardTemplate/" + optOutType.ToString().ToLower());
                result.OptedOutFromAccountComment = string.IsNullOrEmpty(optedOutOfSender.UserComment) ? null : optedOutOfSender.UserComment;
            }
            else
            {
                result.OptedOutSurveys = new SimpleNameValueCollection(
                        optOutdata.ToDictionary(d => d.ResponseTemplateId.ToString(), d => d.ResponseTemplateName));
            }

            return result;
        }

        /// <summary>
        /// Add a user to a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="userUniqueIdentifiers">Unique identifier of the user to add to the group.</param>
        /// <param name="groupDatabaseID">Database id of the group to add the user to.</param>
        public static void AddUsersToGroup(CheckboxPrincipal callingPrincipal, string[] userUniqueIdentifiers, int groupDatabaseID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var userGroup = GroupManager.GetGroup(groupDatabaseID);

            // Validate input
            if (userGroup == null || !userGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupDatabaseID);
            }

            Security.AuthorizeUserContext(callingPrincipal, userGroup, "Group.Edit");

            foreach (var userUniqueIdentifier in userUniqueIdentifiers)
            {
                userGroup.AddUser(userUniqueIdentifier);
                GroupManager.InvalidateUserMemberships(userUniqueIdentifier);
            }

            userGroup.Modifier = callingPrincipal.Identity.Name;
            userGroup.Save();
        }

        /// <summary>
        /// Remove a user from the user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="groupDatabaseID">Database id of the group to remove the user from.</param>
        /// <param name="userUniqueIdentifier">Unique identifier of the user to remove from the group.</param>
        public static void RemoveUserFromGroup(CheckboxPrincipal callingPrincipal, int groupDatabaseID, string userUniqueIdentifier)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(groupDatabaseID);
            var userPrincipal = UserManager.GetUserPrincipal(userUniqueIdentifier.Replace("&#39;", "'"));

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupDatabaseID);
            }

            if (UserDoesNotExist(userPrincipal))
            {
                throw new UserDoesNotExistException(userUniqueIdentifier);
            }

            Security.AuthorizeUserContext(callingPrincipal, g, "Group.Edit");

            g.RemoveUser(userUniqueIdentifier);
            g.Modifier = callingPrincipal.Identity.Name;
            g.Save();
            GroupManager.InvalidateUserMemberships(userUniqueIdentifier);
        }


        /// <summary>
        /// Remove list of users from the user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="groupDatabaseID">Database id of the group to remove the users from.</param>
        /// <param name="userUniqueIdentifiers">Unique identifier list of the users to remove from the group.</param>
        public static void RemoveUsersFromGroup(CheckboxPrincipal callingPrincipal, int groupDatabaseID, string[] userUniqueIdentifiers)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(groupDatabaseID);

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(groupDatabaseID);
            }

            Security.AuthorizeUserContext(callingPrincipal, g, "Group.Edit");

            foreach (var userUniqueIdentifier in userUniqueIdentifiers)
            {
                var userPrincipal = UserManager.GetUserPrincipal(userUniqueIdentifier.Replace("&#39;", "'"));

                if (UserDoesNotExist(userPrincipal))
                {
                    throw new UserDoesNotExistException(userUniqueIdentifier);
                }

                g.RemoveUser(userUniqueIdentifier);
                GroupManager.InvalidateUserMemberships(userUniqueIdentifier);
            }

            g.Modifier = callingPrincipal.Identity.Name;
            g.Save();
        }

        /// <summary>
        /// Remove all of users from the user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="groupDatabaseID">Database id of the group to remove the users from.</param>
        public static void RemoveAllUsersFromGroup(CheckboxPrincipal callingPrincipal, int groupDatabaseID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(groupDatabaseID);
            var usersInGroup = g.GetUserIdentifiers();

            if (g != null)
            {
                Security.AuthorizeUserContext(callingPrincipal, g, "Group.Edit");

                foreach (string userUniqueIdentifier in usersInGroup)
                {
                    g.RemoveUser(userUniqueIdentifier);
                    GroupManager.InvalidateUserMemberships(userUniqueIdentifier);
                }

                g.Modifier = callingPrincipal.Identity.Name;
                g.Save();
            }
        }

        /// <summary>
        /// Set the default policy permissions for a user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseID">Database id of the group to set permissions for.</param>
        /// <param name="permissions">Permissions to set on the default policy.</param>
        public static void SetGroupDefaultPolicyPermissions(CheckboxPrincipal callingPrincipal, int databaseID, string[] permissions)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(databaseID);

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(databaseID);
            }

            Security.AuthorizeUserContext(callingPrincipal, g, "Group.Edit");
            SecurityEditor securityEditor = g.GetEditor();

            if (securityEditor != null)
            {
                securityEditor.Initialize(callingPrincipal);
                securityEditor.SetDefaultPolicy(g.CreatePolicy(permissions));
            }
        }



        public static void DeleteAllGroupMembersFromCheckBox(CheckboxPrincipal callingPrincipal, int groupDatabaseID, out string status)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(groupDatabaseID);
            var usersInGroup = g.GetUserIdentifiers();
            status = "All group members were deleted";

            if (g != null)
            {
                Security.AuthorizeUserContext(callingPrincipal, g, "Group.Edit");
                DeleteUsers(callingPrincipal, usersInGroup, true, out status);
            }
        }


        /// <summary>
        /// Grant a user access to the group with the specified permissions.  Any existing permissions for the user
        /// on that group will be overwritten.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Group to set access to.</param>
        /// <param name="userUniqueIdentifier">Unique identifier of the user to access the group.</param>
        /// <param name="permissions">Permissions to grant to the user.</param>
        public static void AddToGroupAccessList(CheckboxPrincipal callingPrincipal, int securedGroupID, string userUniqueIdentifier, string[] permissions)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var userGroup = GroupManager.GetGroup(securedGroupID);
            var userToAdd = UserManager.GetUserPrincipal(userUniqueIdentifier);

            // Validate input
            if (userGroup == null || !userGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (UserDoesNotExist(userToAdd))
            {
                throw new UserDoesNotExistException(userUniqueIdentifier);
            }

            var editor = userGroup.GetEditor();
            editor.Initialize(callingPrincipal);
            editor.ReplaceAccess(userToAdd, permissions);

            editor.SaveAcl();
        }

        /// <summary>
        /// Grant another user group access to a user group with the specified permissions.  Any existing permissions for the 
        /// permissible group on the secured group will be overwritten.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Group to set access to.</param>
        /// <param name="permissibleGroupID">Group that will have access to the secured group.</param>
        /// <param name="permissions">Permissions to grant to the permissible group.</param>
        public static void AddToGroupAccessList(CheckboxPrincipal callingPrincipal, int securedGroupID, int permissibleGroupID, string[] permissions)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var securedGroup = GroupManager.GetGroup(securedGroupID);
            var permissibleGroup = GroupManager.GetGroup(permissibleGroupID);

            // Validate input
            if (securedGroup == null || !securedGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (permissibleGroup == null || !permissibleGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(permissibleGroupID);
            }

            var editor = securedGroup.GetEditor();
            editor.Initialize(callingPrincipal);
            editor.ReplaceAccess(permissibleGroup, permissions);

            editor.SaveAcl();
        }

        /// <summary>
        /// Remove a user's access to the secured group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Database id of the secured group.</param>
        /// <param name="userUniqueIdentifier">Unique identifier of the user to remove from the group's access list.</param>
        public static void RemoveFromGroupAccessList(CheckboxPrincipal callingPrincipal, int securedGroupID, string userUniqueIdentifier)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(securedGroupID);
            var principal = UserManager.GetUserPrincipal(userUniqueIdentifier);

            // Validate input
            if (g == null || !g.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (UserDoesNotExist(principal))
            {
                throw new UserDoesNotExistException(userUniqueIdentifier);
            }

            var editor = g.GetEditor();
            editor.Initialize(callingPrincipal);
            editor.RemoveAccess(principal);

            editor.SaveAcl();
        }

        /// <summary>
        /// Remove a user group's access to the secured group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Database id of the secured group.</param>
        /// <param name="permissibleGroupID">Database id of the group to be removed from the secured group's access list.</param>
        public static void RemoveFromGroupAccessList(CheckboxPrincipal callingPrincipal, int securedGroupID, int permissibleGroupID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var securedGroup = GroupManager.GetGroup(securedGroupID);
            var permissibleGroup = GroupManager.GetGroup(permissibleGroupID);

            // Validate input
            if (securedGroup == null || !securedGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (permissibleGroup == null || !permissibleGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(permissibleGroupID);
            }

            var editor = securedGroup.GetEditor();
            editor.Initialize(callingPrincipal);
            editor.RemoveAccess(permissibleGroup);

            editor.SaveAcl();
        }

        /// <summary>
        /// List the permissions on the group's default policy
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="databaseID"></param>
        /// <returns>Array of permission names.</returns>
        public static string[] ListGroupDefaultPolicyPermissions(CheckboxPrincipal callingPrincipal, int databaseID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(databaseID);

            // Validate input
            if (g == null || !g.ID.HasValue || g.DefaultPolicy == null)
            {
                throw new UserGroupDoesNotExistException(databaseID);
            }

            return g.DefaultPolicy.Permissions.ToArray();
        }

        /// <summary>
        /// List all access control list entries for the specified user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupId">The unique id of the user group.</param>
        /// <returns></returns>
        public static AclEntry[] ListAllGroupAccessListEntries(CheckboxPrincipal callingPrincipal, int securedGroupId)
        {
            var userGroup = GroupManager.GetGroup(securedGroupId);

            //Authorize
            Security.AuthorizeUserContext(callingPrincipal, userGroup, "Group.Edit");

            // Validate input
            if (userGroup == null || !userGroup.ID.HasValue || userGroup.ACL == null)
            {
                throw new UserGroupDoesNotExistException(securedGroupId);
            }

            var allEntries = userGroup.ACL.SelectAll();
            var aclEntries = allEntries.Select(SecurityManagementServiceImplementation.GetAclEntry);

            aclEntries = aclEntries.OrderBy(entry => entry.EntryType).ThenBy(entry => entry.ShortEntryIdentifier);

            return aclEntries.ToArray();
        }

        /// <summary>
        /// List the access list permissions a particular user has for the specified user group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Database id of the user group.</param>
        /// <param name="userUniqueIdentifier">Unique identifier of the user to check permissions for.</param>
        /// <returns>If the user is not on the group's access list, the default policy permissions for the group will be returned otherwise the user's
        /// access list permissions will be returned.</returns>
        public static string[] ListGroupAccessListPermissions(CheckboxPrincipal callingPrincipal, int securedGroupID, string userUniqueIdentifier)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var g = GroupManager.GetGroup(securedGroupID);
            var principal = UserManager.GetUserPrincipal(userUniqueIdentifier);

            // Validate input
            if (g == null || !g.ID.HasValue || g.ACL == null)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (UserDoesNotExist(principal))
            {
                throw new UserDoesNotExistException(userUniqueIdentifier);
            }

            var p = g.ACL.GetPolicy(principal);

            return p != null ? p.Permissions.ToArray() : new string[] { };
        }

        /// <summary>
        /// List the access list permissions a particular group has for the specified secured group.
        /// </summary>
        /// <param name="callingPrincipal">Guid context token identifying the requesting user.</param>
        /// <param name="securedGroupID">Database id of the secured group.</param>
        /// <param name="permissibleGroupID">Database id of the permissible group to get permissions for.</param>
        /// <returns>If the user is not on the group's access list, the default policy permissions for the group will be returned otherwise the user's
        /// access list permissions will be returned.</returns>
        public static string[] ListGroupAccessListPermissions(CheckboxPrincipal callingPrincipal, int securedGroupID, int permissibleGroupID)
        {
            Security.AuthorizeUserContext(callingPrincipal, "Group.Edit");

            var securedGroup = GroupManager.GetGroup(securedGroupID);
            var permissibleGroup = GroupManager.GetGroup(permissibleGroupID);

            // Validate input
            if (securedGroup == null || !securedGroup.ID.HasValue || securedGroup.ACL == null)
            {
                throw new UserGroupDoesNotExistException(securedGroupID);
            }

            if (permissibleGroup == null || !permissibleGroup.ID.HasValue)
            {
                throw new UserGroupDoesNotExistException(permissibleGroupID);
            }

            var p = securedGroup.ACL.GetPolicy(permissibleGroup);

            return p != null ? p.Permissions.ToArray() : new string[] { };
        }


        #endregion

        /// <summary>
        /// A temp method that determines if a user exists or not while we determine the correct way
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool UserDoesNotExist(CheckboxPrincipal user)
        {
            //Check user null
            if (user == null)
            {
                return true;
            }

            //Check authentication type. External users are considered to always "exist" so their profile
            // and user roles can be managed in Checkbox
            if (user.Identity.AuthenticationType == UserManager.EXTERNAL_USER_AUTHENTICATION_TYPE)
            {
                return false;
            }

            return user.UserGuid == Guid.Empty;

        }
    }
}
