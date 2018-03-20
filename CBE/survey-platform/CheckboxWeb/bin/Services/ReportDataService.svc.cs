using System;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportDataService : IReportDataService
    {
        #region IReportDataService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemProxyObject> GetItem(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<IItemProxyObject>
                {
                    CallSuccess = true,
                    ResultData = ReportDataServiceImplementation.GetItem(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, itemId, languageCode, includeIncompleteResponses, false)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemProxyObject>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                return new ServiceOperationResult<IItemProxyObject>
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemProxyObject> GetItemNoPreload(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<IItemProxyObject>
                {
                    CallSuccess = true,
                    ResultData = ReportDataServiceImplementation.GetItem(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, itemId, languageCode, includeIncompleteResponses, true)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemProxyObject>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                return new ServiceOperationResult<IItemProxyObject>
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
        /// <param name="surveyId"></param>
        /// <param name="surveyItemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemProxyObject> GetResultsForSurveyItem(string authTicket, int surveyId, int surveyItemId, bool includeIncompleteResponses, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<IItemProxyObject>
                {
                    CallSuccess = true,
                    ResultData = ReportDataServiceImplementation.GetResultsForSurveyItem(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, surveyItemId, includeIncompleteResponses, languageCode)
                };
            }
            catch (Exception ex)
            {
                return new ServiceOperationResult<IItemProxyObject>
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
