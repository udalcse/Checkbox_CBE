using System;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// Service implementation for style management service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class StyleManagementService : IStyleManagementService
    {
        #region IStyleManagementService Members

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
        public ServiceOperationResult<PagedListResult<StyleListItem[]>> ListFormStyles(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<StyleListItem[]>>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.ListFormStyles(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<StyleListItem[]>>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<StyleListItem[]>>
                {
                    CallSuccess = false,
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
        public ServiceOperationResult<StyleListItem[]> ListChartStyles(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<StyleListItem[]>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.ListChartStyles(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<StyleListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<StyleListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ServiceOperationResult<StyleListItem> GetStyleListItem(string authTicket, int styleId, string type)
        {
            try
            {
                return new ServiceOperationResult<StyleListItem>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.GetStyleListItem(AuthenticationService.GetCurrentPrincipal(authTicket), styleId, type)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<StyleListItem>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<StyleListItem>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteFormStyle(string authTicket, int styleId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.DeleteFormStyle(AuthenticationService.GetCurrentPrincipal(authTicket), styleId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteFormStyles(string authTicket, int[] styleIds)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.DeleteFormStyles(AuthenticationService.GetCurrentPrincipal(authTicket), styleIds)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteChartStyle(string authTicket, int styleId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.DeleteChartStyle(AuthenticationService.GetCurrentPrincipal(authTicket), styleId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> CopyFormStyle(string authTicket, int styleId, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = StyleManagementServiceImplementation.CopyFormStyle(AuthenticationService.GetCurrentPrincipal(authTicket), styleId, languageCode)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion
    }
}
