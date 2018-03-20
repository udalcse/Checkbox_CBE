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
    public class AuthorizationService : IAuthorizationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> HasRolePermission(string userUniqueIdentifier, string permission)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = AuthorizationServiceImplementation.UserHasRolePermission(userUniqueIdentifier, permission)
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
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="securedResourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> IsAuthorized(string userUniqueIdentifier, SecuredResourceType securedResourceType, string resourceId, string permission)
        {
           try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = AuthorizationServiceImplementation.AuthorizeAccess(userUniqueIdentifier, securedResourceType, resourceId, permission)
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
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="securedResourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionCsv"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> BatchIsAuthorized(string userUniqueIdentifier, SecuredResourceType securedResourceType, string resourceId, string permissionCsv)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = AuthorizationServiceImplementation.BatchAuthorizeAccess(userUniqueIdentifier, securedResourceType, resourceId, permissionCsv)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>{
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
    }
}
