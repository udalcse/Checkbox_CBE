using System;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserManagementService : IUserManagementService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> AuthenticateUser(string userName, string password)
        {
            var service = new AuthenticationService();

            return service.Login(userName, password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<UserData> GetUserData(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<UserData>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUserData(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<UserData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<UserData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<string[]>> ListUserIdentities(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUserIdentities(AuthenticationService.GetCurrentPrincipal(authToken), null, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, DateTime.MinValue, null)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> GetUsers(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUsers(AuthenticationService.GetCurrentPrincipal(authToken), provider, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, 0, null)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> GetUsersByPeriod(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUsers(AuthenticationService.GetCurrentPrincipal(authToken), provider, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> GetUsersTenantByPeriod(string authToken, string provider, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetTenantUsers(AuthenticationService.GetCurrentPrincipal(authToken), provider, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<PageItemUserData[]>> GetPageItemUsersData(string authToken, string provider, int pageNumber, int pageSize,
            string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetPageItemUsersData(AuthenticationService.GetCurrentPrincipal(authToken), provider, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="role"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> GetUsersInRole(string authToken, string role, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUsersInRole(AuthenticationService.GetCurrentPrincipal(authToken), role, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> GetUserProfile(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUserProfile(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateUserProfile(string authToken, string uniqueIdentifier, SimpleNameValueCollection profile)
        {
            try
            {
                UserManagementServiceImplementation.UpdateUserProfile(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier, profile);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="emailAddress"></param>
        /// <param name="profile"></param>
        /// <param name="updateIfExists"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> CreateUser(string authToken, string userName, string password, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.CreateUser(AuthenticationService.GetCurrentPrincipal(authToken), userName, password, emailAddress, profile, updateIfExists)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userName"></param>
        /// <param name="domain"></param>
        /// <param name="emailAddress"></param>
        /// <param name="profile"></param>
        /// <param name="updateIfExists"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> CreateNetworkUser(string authToken, string userName, string domain, string emailAddress, SimpleNameValueCollection profile, bool updateIfExists)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.CreateNetworkUser(AuthenticationService.GetCurrentPrincipal(authToken), userName, domain, emailAddress, profile, updateIfExists)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userIdentity"></param>
        /// <param name="newEmailAddress"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ChangeUserEmailAddress(string authToken, string userIdentity, string newEmailAddress)
        {
            try
            {
                UserManagementServiceImplementation.ChangeUserEmailAddress(AuthenticationService.GetCurrentPrincipal(authToken), userIdentity, newEmailAddress);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userIdentity"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ChangeUserPassword(string authToken, string userIdentity, string newPassword)
        {
            try
            {
                UserManagementServiceImplementation.ChangeUserPassword(AuthenticationService.GetCurrentPrincipal(authToken), userIdentity, newPassword);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userIdentity"></param>
        /// <param name="oldDomain"></param>
        /// <param name="newDomain"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> ChangeUserDomain(string authToken, string userIdentity, string oldDomain, string newDomain)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ChangeUserDomain(AuthenticationService.GetCurrentPrincipal(authToken), userIdentity, oldDomain, newDomain)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="deleteResponses"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteUser(string authToken, string uniqueIdentifier, bool deleteResponses)
        {
            try
            {
                UserManagementServiceImplementation.DeleteUser(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier, deleteResponses);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifierList"></param>
        /// <param name="deleteResponses"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteUsers(string authToken, string[] uniqueIdentifierList, bool deleteResponses)
        {
            try
            {
                String status;

                UserManagementServiceImplementation.DeleteUsers(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifierList, deleteResponses, out status);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = status
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifierList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteResponsesOfUsers(string authToken, string[] uniqueIdentifierList)
        {
            try
            {
                UserManagementServiceImplementation.DeleteResponsesOfUsers(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifierList);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteUserProfile(string authToken, string userIdentity)
        {
            try
            {
                UserManagementServiceImplementation.DeleteUserProfile(AuthenticationService.GetCurrentPrincipal(authToken), userIdentity);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="newUniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> RenameUser(string authToken, string uniqueIdentifier, string newUniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.RenameUser(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier, newUniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> UnlockUser(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.UnlockUser(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> LockUser(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.LockUser(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> UserExists(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.UserExists(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public ServiceOperationResult<RoleData[]> ListAllAvailableUserRoles(string authToken)
        {
            try
            {
                return new ServiceOperationResult<RoleData[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListAllAvailableUserRoles(AuthenticationService.GetCurrentPrincipal(authToken))
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<RoleData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<string[]> ListUserRoles(string authToken, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUserRoles(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddUserToRole(string authToken, string uniqueIdentifier, string roleName)
        {
            try
            {
                UserManagementServiceImplementation.AddUserToRole(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier, roleName);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveUserFromRole(string authToken, string uniqueIdentifier, string roleName)
        {
            try
            {
                UserManagementServiceImplementation.RemoveUserFromRole(AuthenticationService.GetCurrentPrincipal(authToken), uniqueIdentifier, roleName);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserGroupData[]>> ListUserGroups(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUserGroups(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, 0, null)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false, FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserGroupData[]>> ListUserGroupsByPeriod(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUserGroups(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userGroupName"></param>
        /// <param name="userGroupDescription"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> CreateUserGroup(string authToken, string userGroupName, string userGroupDescription)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.CreateUserGroup(AuthenticationService.GetCurrentPrincipal(authToken), userGroupName, userGroupDescription)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> CopyUserGroup(string authToken, int groupId)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.CopyUserGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupIdList"></param>
        /// <returns></returns>
        public ServiceOperationResult<int[]> CopyUserGroups(string authToken, int[] groupIdList)
        {
            try
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.CopyUserGroups(AuthenticationService.GetCurrentPrincipal(authToken), groupIdList)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <param name="newGroupName"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> RenameUserGroup(string authToken, int groupId, string newGroupName)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.RenameUserGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupId, newGroupName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <param name="newGroupDescription"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ChangeUserGroupDescription(string authToken, int groupId, string newGroupDescription)
        {
            try
            {
                UserManagementServiceImplementation.ChangeUserGroupDescription(AuthenticationService.GetCurrentPrincipal(authToken), groupId, newGroupDescription);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteUserGroup(string authToken, int databaseID)
        {
            try
            {
                UserManagementServiceImplementation.DeleteUserGroup(AuthenticationService.GetCurrentPrincipal(authToken), databaseID);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="databaseIdList"></param>
        /// <returns></returns>
        public ServiceOperationResult<string[]> DeleteUserGroups(string authToken, int[] databaseIdList)
        {
            string[] notDeletedGroups = null;
            
            try
            {
                var resultData = UserManagementServiceImplementation.DeleteUserGroups(AuthenticationService.GetCurrentPrincipal(authToken), databaseIdList, out notDeletedGroups);

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = resultData
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage,
                    ResultData = notDeletedGroups
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message,
                    ResultData = notDeletedGroups
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userUniqueIdentifiers"></param>
        /// <param name="groupDatabaseID"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddUsersToGroup(string authToken, string[] userUniqueIdentifiers, int groupDatabaseID)
        {
            try
            {

                UserManagementServiceImplementation.AddUsersToGroup(AuthenticationService.GetCurrentPrincipal(authToken), userUniqueIdentifiers, groupDatabaseID);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupDatabaseID"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveUserFromGroup(string authToken, int groupDatabaseID, string userUniqueIdentifier)
        {
            try
            {
                UserManagementServiceImplementation.RemoveUserFromGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupDatabaseID, userUniqueIdentifier);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupDatabaseID"></param>
        /// <param name="userUniqueIdentifiers"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveUsersFromGroup(string authToken, int groupDatabaseID, string[] userUniqueIdentifiers)
        {
            try
            {
                UserManagementServiceImplementation.RemoveUsersFromGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupDatabaseID, userUniqueIdentifiers);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<object> RemoveAllUsersFromGroup(string authToken, int groupDatabaseID)
        {
            try
            {
                UserManagementServiceImplementation.RemoveAllUsersFromGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupDatabaseID);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<object> DeleteAllGroupMembersFromCheckBox(string authToken, int groupDatabaseID)
        {
            try
            {
                String message;

                UserManagementServiceImplementation.DeleteAllGroupMembersFromCheckBox(AuthenticationService.GetCurrentPrincipal(authToken), groupDatabaseID, out message);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = message
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="databaseID"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> SetGroupDefaultPolicyPermissions(string authToken, int databaseID, string[] permissions)
        {
            try
            {
                UserManagementServiceImplementation.SetGroupDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authToken), databaseID, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupID"></param>
        /// <param name="userUnqiueIdentifier"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddUserToGroupAccessList(string authToken, int securedGroupID, string userUnqiueIdentifier, string[] permissions)
        {
            try
            {
                UserManagementServiceImplementation.AddToGroupAccessList(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupID, userUnqiueIdentifier, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupID"></param>
        /// <param name="permissibleGroupID"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddGroupToGroupAccessList(string authToken, int securedGroupID, int permissibleGroupID, string[] permissions)
        {
            try
            {
                UserManagementServiceImplementation.AddToGroupAccessList(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupID, permissibleGroupID, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupId"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveUserFromGroupAccessList(string authToken, int securedGroupId, string userUniqueIdentifier)
        {
            try
            {
                UserManagementServiceImplementation.RemoveFromGroupAccessList(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupId, userUniqueIdentifier);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupID"></param>
        /// <param name="permissibleGroupID"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveGroupFromGroupAccessList(string authToken, int securedGroupID, int permissibleGroupID)
        {
            try
            {
                UserManagementServiceImplementation.RemoveFromGroupAccessList(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupID, permissibleGroupID);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public ServiceOperationResult<string[]> ListGroupDefaultPolicyPermissions(string authToken, int databaseID)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListGroupDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authToken), databaseID)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List all access control list entries for the specified user group.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public ServiceOperationResult<AclEntry[]> ListAllGroupAccessListEntries(string authToken, int databaseID)
        {
            try
            {
                return new ServiceOperationResult<AclEntry[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListAllGroupAccessListEntries(AuthenticationService.GetCurrentPrincipal(authToken), databaseID)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<AclEntry[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<AclEntry[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupID"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<string[]> ListGroupAccessListPermissionsForUser(string authToken, int securedGroupID, string userUniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListGroupAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupID, userUniqueIdentifier)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="securedGroupID"></param>
        /// <param name="permissibleGroupID"></param>
        /// <returns></returns>
        public ServiceOperationResult<string[]> ListGroupAccessListPermissionsForGroup(string authToken, int securedGroupID, int permissibleGroupID)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListGroupAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authToken), securedGroupID, permissibleGroupID)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ServiceOperationResult<UserGroupData> GetUserGroupByName(string authToken, string groupName)
        {
            try
            {
                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUserGroupByName(AuthenticationService.GetCurrentPrincipal(authToken), groupName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ServiceOperationResult<UserGroupData> GetUserGroupById(string authToken, int groupId)
        {
            try
            {
                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.GetUserGroupById(AuthenticationService.GetCurrentPrincipal(authToken), groupId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<UserGroupData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> ListUserGroupMembers(string authToken, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUserGroupMembers(AuthenticationService.GetCurrentPrincipal(authToken), groupId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<UserData>[]> SearchUsers(string authToken, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<UserData>[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.SearchUsers(AuthenticationService.GetCurrentPrincipal(authToken), searchTerm)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<GroupedResult<UserData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<UserData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<UserGroupData>[]> SearchGroups(string authToken, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<UserGroupData>[]>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.SearchGroups(AuthenticationService.GetCurrentPrincipal(authToken), searchTerm)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<GroupedResult<UserGroupData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<UserGroupData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #region IUserManagementService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> ListUsersNotInGroup(string authToken, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListUsersNotInGroup(AuthenticationService.GetCurrentPrincipal(authToken), groupId, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<PageItemUserData[]>> ListPageItemUserDataForGroup(string authToken, string provider, int groupId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListPageItemUserDataForGroup(AuthenticationService.GetCurrentPrincipal(authToken), provider, groupId, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get list of available new group users
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> ListPotentialUsersForNewGroup(string authToken,  int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListPotentialUsersForNewGroup(AuthenticationService.GetCurrentPrincipal(authToken), string.Empty, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<PageItemUserData[]>> ListPotentialUsersForNewGroupByProvider(string authToken, string provider, int pageNumber, int pageSize,
            string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListPageItemPotentialUsersForNewGroup(AuthenticationService.GetCurrentPrincipal(authToken), provider, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List of new group members
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> ListCurrentUsersForNewGroup(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListCurrentUsersForNewGroup(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add member to new group
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> ListCurrentUsersForNewGroupAddUser(string authToken, string userId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListCurrentUsersForNewGroupAddUser(AuthenticationService.GetCurrentPrincipal(authToken), userId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Remove user from new group
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> ListCurrentUsersForNewGroupRemoveUser(string authToken, string userId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManagementServiceImplementation.ListCurrentUsersForNewGroupRemoveUser(AuthenticationService.GetCurrentPrincipal(authToken), userId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
        #endregion
    }
}
