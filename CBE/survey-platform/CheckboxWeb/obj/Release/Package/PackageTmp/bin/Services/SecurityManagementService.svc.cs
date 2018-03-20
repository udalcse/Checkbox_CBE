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
    public class SecurityManagementService : ISecurityManagementService
    {
        #region ACL Management

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<AclEntry[]>> GetAclEntries(string authTicket, SecuredResourceType resourceType, int resourceId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = true,
                    ResultData = SecurityManagementServiceImplementation.GetAclEntries(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
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
        /// <param name="authTicket"></param>
        /// <param name="provider"></param>
        /// <param name="resourceType"></param>
        /// <param name="permissionToGrant"></param>
        /// <param name="resourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<AclEntry[]>> GetAvailableEntries(string authTicket, string provider, SecuredResourceType resourceType, int resourceId, string permissionToGrant, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = true,
                    ResultData = SecurityManagementServiceImplementation.GetAvailableAclEntries(AuthenticationService.GetCurrentPrincipal(authTicket), provider, resourceType, resourceId, permissionToGrant, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="permissionToCheck"></param>
        /// <param name="resourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<AclEntry[]>> GetCurrentEntries(string authTicket, SecuredResourceType resourceType, int resourceId, string permissionToCheck, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = true,
                    ResultData = SecurityManagementServiceImplementation.GetCurrentAclEntries(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, permissionToCheck, pageNumber, pageSize, sortField, sortAscending, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<AclEntry[]>>
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <param name="permissionToGrant"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddEntryToAcl(string authTicket, SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier, string permissionToGrant)
        {
            try
            {
                SecurityManagementServiceImplementation.AddEntryToAcl(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, aclEntryType, aclEntryIdentifier, permissionToGrant);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemoveEntryFromAcl(string authTicket, SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier)
        {
            try
            {
                SecurityManagementServiceImplementation.RemoveEntryFromAcl(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, aclEntryType, aclEntryIdentifier);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
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


        #endregion

        #region Permissions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PermissionEntry[]> GetPolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId)
        {
            try
            {
                return new ServiceOperationResult<PermissionEntry[]>
                {
                    CallSuccess = true,
                    ResultData = SecurityManagementServiceImplementation.GetPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, policyId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PermissionEntry[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PermissionEntry[]>
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="permissionMasks"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdatePolicyMaskedPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId, string[] permissionMasks)
        {
            try
            {
                SecurityManagementServiceImplementation.UpdatePolicyMaskedPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, policyId, permissionMasks);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdatePolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId, string[] permissions)
        {
            try
            {
                SecurityManagementServiceImplementation.UpdatePolicyPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, policyId, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PermissionMaskEntry[]> GetMaskedPolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId)
        {
            try
            {
                return new ServiceOperationResult<PermissionMaskEntry[]>
                {
                    CallSuccess = true,
                    ResultData = SecurityManagementServiceImplementation.GetMaskedPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), resourceType, resourceId, policyId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PermissionMaskEntry[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PermissionMaskEntry[]>
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
