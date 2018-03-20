using System;
using System.Web;
using System.Web.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Wcf.Services.LocalProxies
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalReportDataServiceProxy : IReportDataService
    {
        /// <summary>
        /// Get guid associated with current user.
        /// </summary>
        /// <returns></returns>
        private CheckboxPrincipal GetCurrentPrincipal(string encryptedTicket)
        {
            //Use ticket if provided
            if (!string.IsNullOrEmpty(encryptedTicket))
            {
                var decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);

                //Check user data for user guid
                if (string.IsNullOrEmpty(decryptedTicket.UserData))
                {
                    return null;
                }

                Guid userGuid;

                if (!Guid.TryParse(decryptedTicket.UserData, out userGuid))
                {
                    return null;
                }

                return UserManager.GetUserByGuid(userGuid);
            }

            //Fallback to current user
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User as CheckboxPrincipal;
            }

            return null;
        }

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
                    ResultData = ReportDataServiceImplementation.GetItem(GetCurrentPrincipal(authTicket), reportId, itemId, languageCode, includeIncompleteResponses, true)
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
                    ResultData = ReportDataServiceImplementation.GetItem(GetCurrentPrincipal(authTicket), reportId, itemId, languageCode, includeIncompleteResponses, true)
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
                    ResultData = ReportDataServiceImplementation.GetResultsForSurveyItem(GetCurrentPrincipal(authTicket), surveyId, surveyItemId, includeIncompleteResponses, languageCode)
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
