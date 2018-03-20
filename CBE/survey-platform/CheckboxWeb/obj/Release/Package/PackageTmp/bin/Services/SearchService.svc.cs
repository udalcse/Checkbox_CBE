using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SearchService : ISearchService
    {
        #region ISearchService Members

        /// <summary>
        /// Prepares cached structures to perform fast search
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <returns></returns>        
        public ServiceOperationResult<bool> Initialize(string userAuthID)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SearchServiceImplementation.Initialize(AuthenticationService.GetCurrentPrincipal(userAuthID), ApplicationManager.AppSettings.SearchAccessibleObjectExpPeriodSeconds)
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
        /// Get Search
        /// </summary>
        /// <param name="progressKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<SearchAnswer> Search(string userAuthID, string Term, string RequestID)
        {
            try
            {
                Guid requestID = string.IsNullOrEmpty(RequestID) ? Guid.Empty : Guid.Parse(RequestID);
                return new ServiceOperationResult<SearchAnswer>
                {
                    CallSuccess = true,
                    ResultData = SearchServiceImplementation.Search(AuthenticationService.GetCurrentPrincipal(userAuthID), Term, requestID)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SearchAnswer>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        
        /// <summary>
        /// Returns search settings
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <returns></returns>
        public ServiceOperationResult<SearchSettingsInfo[]> GetSearchSettings(string userAuthID)
        {
            try
            {
                return new ServiceOperationResult<SearchSettingsInfo[]>
                {
                    CallSuccess = true,
                    ResultData = SearchServiceImplementation.GetSearchSettings(AuthenticationService.GetCurrentPrincipal(userAuthID))
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SearchSettingsInfo[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
        
        /// <summary>
        /// Changes the order of the search results
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateSearchResultsOrder(string userAuthID, string objectType, int order)
        {
            try
            {
                SearchServiceImplementation.UpdateSearchResultsOrder(AuthenticationService.GetCurrentPrincipal(userAuthID), objectType, order);
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

        /// <summary>
        /// Includes or excludes these objects from search
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="included"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ToggleSearchObjectType(string userAuthID, string objectType, bool included)
        {
            try
            {
                SearchServiceImplementation.ToggleSearchObjectType(AuthenticationService.GetCurrentPrincipal(userAuthID), objectType, included);
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

        /// <summary>
        /// Changes the roles set which may search for the objects of given type
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateObjectsRoles(string userAuthID, string objectType, string roles)
        {
            try
            {
                SearchServiceImplementation.UpdateObjectsRoles(AuthenticationService.GetCurrentPrincipal(userAuthID), objectType, roles);
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


        #endregion ISearchService Members
    }
}
