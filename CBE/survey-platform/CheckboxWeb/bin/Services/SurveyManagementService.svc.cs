using System;
using System.ServiceModel.Activation;
using Checkbox.Common;
using Checkbox.Forms.Security.Principal;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;
using System.Web;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// Service implementation for survey management service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SurveyManagementService : ISurveyManagementService
    {
        /// <summary>
        /// List surveys and folders for authenticated users.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFolders(string authTicket, int parentId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authTicket), parentId, pageNumber, pageSize, "Name", filter, 0, null, includeSurveyResponseCount, true, true)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List surveys and folders for authenticated users.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriod(string authTicket, int parentId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authTicket), parentId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount, true, true)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyListItem[]>
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
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField"></param>
        /// <param name="filter"></param>
        /// <param name="period"></param>
        /// <param name="dateFieldName"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriodByActiveStatus(string authTicket, int parentId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authTicket), parentId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount, includeActive, includeInactive)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        ///  List favorite surveys for authenticated users.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListFavoriteSurveys(string authTicket, int parentId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListFavoriteSurveys(AuthenticationService.GetCurrentPrincipal(authTicket), parentId, pageNumber, pageSize, filter, includeSurveyResponseCount)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add survey to list of favorite ones 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> AddFavoriteSurvey(string authTicket, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.AddFavoriteSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId)
                };
            }
            catch (NoUserException ex)
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
        /// Remove favorite survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> RemoveFavoriteSurvey(string authTicket, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.RemoveFavoriteSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId)
                };
            }
            catch (NoUserException ex)
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
        /// Check if specified survey is favorite
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> IsFavoriteSurvey(string authTicket, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.IsFavoriteSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId)
                };
            }
            catch (NoUserException ex)
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
        /// List available surveys for authenticated users.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<SurveyListItem[]>> ListAvailableSurveys(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                //If user is anonymous, check his information from Cookie/Session. If it doesn't exist, create new anonymous respondent.
                CheckboxPrincipal currentPrincipal = AuthenticationService.GetCurrentPrincipal(authTicket);
                if (currentPrincipal == null)
                {
                    //Attempt to get cookie value
                    if (ApplicationManager.AppSettings.SessionMode == AppSettings.SessionType.Cookies
                        && Utilities.IsNotNullOrEmpty(ApplicationManager.AppSettings.CookieName)
                        && HttpContext.Current != null
                        && HttpContext.Current.Request != null
                        && HttpContext.Current.Request.Cookies != null)
                    {
                        HttpCookie respondentCookie =
                            HttpContext.Current.Request.Cookies[ApplicationManager.AppSettings.CookieName];
                        if (respondentCookie != null)
                            currentPrincipal = new AnonymousRespondent(new Guid(respondentCookie.Value));
                    }

                    //Attemp to get session value
                    if (ApplicationManager.AppSettings.StoreSessionKeyInHttpSesssion
                    && HttpContext.Current.Session["SurveySession"] != null)
                    {

                        ResponseSessionData sessionData =
                            HttpContext.Current.Session["SurveySession"] as ResponseSessionData;

                        if (sessionData != null)
                        {
                            currentPrincipal =
                                new AnonymousRespondent(sessionData.AnonymousRespondentGuid ?? Guid.NewGuid());
                        }
                    }

                    if (currentPrincipal == null)
                        currentPrincipal = new AnonymousRespondent(Guid.NewGuid());
                }

                return new ServiceOperationResult<PagedListResult<SurveyListItem[]>>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListAvailableSurveys(currentPrincipal, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<SurveyListItem[]>>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<SurveyListItem[]>>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List surveys and folders for authenticated users.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="listItemId"></param>
        /// <param name="listItemType"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem> GetSurveyListItem(string authTicket, int listItemId, string listItemType)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyListItem(AuthenticationService.GetCurrentPrincipal(authTicket), listItemId, listItemType, true)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyListItem>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyListItem>
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
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyMetaData> GetSurveyMetaData(string authTicket, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyMetaData(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<int> CopySurvey(string authTicket, int surveyId, string surveyName)
        {
            try
            {
                return new ServiceOperationResult<int>()
                {
                    CallSuccess = true,
                    ResultData =
                        SurveyManagementServiceImplementation.CopySurvey(
                            AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, surveyName)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
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
        /// <param name="rtGuid"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyMetaData> GetSurveyInfoByGuid(string authTicket, Guid rtGuid)
        {
            try
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyInfoByGuid(AuthenticationService.GetCurrentPrincipal(authTicket), rtGuid)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyMetaData>
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
        /// <param name="surveyName"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyMetaData> GetSurveyInfoByName(string authTicket, string surveyName)
        {
            try
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyInfoByName(AuthenticationService.GetCurrentPrincipal(authTicket), surveyName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyMetaData>
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
        /// <param name="uniqueIdentifier"></param>
        /// <param name="responseTemplateID"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddUserToSurveyAccessList(string authTicket, string uniqueIdentifier, int responseTemplateID, string[] permissions)
        {
            try
            {
                SurveyManagementServiceImplementation.AddUserToSurveyAccessList(AuthenticationService.GetCurrentPrincipal(authTicket), uniqueIdentifier, responseTemplateID, permissions);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyPageMetaData> GetSurveyPageData(string authTicket, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<SurveyPageMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyPageMetaData(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SurveyPageMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyPageMetaData>
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
        /// <param name="customUrl"></param>
        /// <param name="serverApplicationPath"></param>
        /// <returns></returns>
        public ServiceOperationResult<QuestionResult> IfAlternateUrlIsAvailable(string authTicket, string customUrl, string serverApplicationPath)
        {
            try
            {
                return new ServiceOperationResult<QuestionResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.IfAlternateUrlIsAvailable(AuthenticationService.GetCurrentPrincipal(authTicket), customUrl, serverApplicationPath)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<QuestionResult>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<QuestionResult>
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
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata> GetSurveyItemData(string authTicket, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyItemMetaData(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, itemId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata>
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
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListPageItemsData(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata[]>
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
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata[]> GetColumnPrototypes(string authTicket, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetColumnPrototypes(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, itemId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata[]>
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
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageCondition(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId, TextManager.DefaultLanguage)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<RuleMetaData>
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
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData[]> GetBranchDataForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageBranches(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId, TextManager.DefaultLanguage)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<RuleMetaData[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<RuleMetaData[]>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="ruleType"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByTypeAndRuleType(string authToken, int responseTemplateId,
            int dependentItemId, string leftParamType, string ruleType, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetExpressionLeftParamByType(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, dependentItemId, leftParamType, ruleType, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByType(string authToken, int responseTemplateId, int dependentItemId, string leftParamType, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetExpressionLeftParamByType(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, dependentItemId, leftParamType, null, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionOperators(string authToken, int responseTemplateId, string leftParamType, string leftParam, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetExpressionOperators(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, leftParamType, leftParam, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<ExpressionRightParamData> GetExpressionRightParams(string authToken, int responseTemplateId, string leftParamType, string leftParam, string selectedOperator, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<ExpressionRightParamData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetExpressionRightParams(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, leftParamType, leftParam, selectedOperator, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ExpressionRightParamData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ExpressionRightParamData>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="expressionId"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<ExpressionRightParamData> GetExistingExpressionRightParams(string authToken, int responseTemplateId, int expressionId, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<ExpressionRightParamData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetExistingExpressionRightParams(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, expressionId, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ExpressionRightParamData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ExpressionRightParamData>
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
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="expressionId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="data"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<ExpressionMetaData> AddExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<ExpressionMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.AddExpression(AuthenticationService.GetCurrentPrincipal(authToken), ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ExpressionMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ExpressionMetaData>
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
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="expressionId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="data"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<ExpressionMetaData> EditExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<ExpressionMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.EditExpression(AuthenticationService.GetCurrentPrincipal(authToken), ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ExpressionMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ExpressionMetaData>
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
        /// <param name="responseTemplateId"></param>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public ServiceOperationResult<int[]> RemoveExpression(string authToken, int responseTemplateId, int expressionId)
        {
            try
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.RemoveExpression(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, expressionId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// Change OR connectors to AND or vice versa
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ReorganizeExpressions(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, string connector)
        {
            try
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ReorganizeExpressions(AuthenticationService.GetCurrentPrincipal(authToken), ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, connector)
                };
            }
            catch (NoUserException ex)
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


        /// <summary>
        ///  Set target page for the given rule
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> SetPageBranchTargetPage(string authToken, int responseTemplateId, int ruleId, int targetPageId)
        {
            try
            {
                SurveyManagementServiceImplementation.SetPageBranchTargetPage(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, ruleId, targetPageId);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = true
                };
            }
            catch (NoUserException ex)
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

        /// <summary>
        /// Get page logic
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PageLogic> GetLogicForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<PageLogic>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageLogic(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId, TextManager.DefaultLanguage)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PageLogic>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PageLogic>
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
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyItem(string authTicket, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetItemCondition(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, itemId, TextManager.DefaultLanguage)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<RuleMetaData>
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
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata> GetLibraryItemData(string authTicket, int libraryId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetLibraryItemMetaData(AuthenticationService.GetCurrentPrincipal(authTicket), libraryId, itemId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata>
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
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> SetItemLibraryOptions(string authTicket, int itemId, bool shouldShow)
        {
            try
            {
                SurveyManagementServiceImplementation.SetItemLibraryOptions(AuthenticationService.GetCurrentPrincipal(authTicket), itemId, shouldShow);

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true                    
                };
            }
            catch (NoUserException ex)
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
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<StyleListItem[]>> ListSurveyStyleTemplates(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<StyleListItem[]>>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveyStyleTemplates(AuthenticationService.GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
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

        public ServiceOperationResult<object> AddItemsFromLibrary(int pageId, int itemId, int responseTemplateId, int libraryId)
        {
            try
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.AddItemsFromLibrary(AuthenticationService.GetCurrentPrincipal(string.Empty), 
                    pageId, itemId, responseTemplateId, libraryId)
                };
            }
            catch(Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
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
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<LibraryData[]>> ListItemLibraries(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<LibraryData[]>>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListItemLibraries(AuthenticationService.GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<LibraryData[]>>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<LibraryData[]>>
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
        /// <param name="libraryId"></param>
        /// <returns></returns>
        public ServiceOperationResult<LibraryData> GetLibraryData(string authTicket, int libraryId)
        {
            try
            {
                return new ServiceOperationResult<LibraryData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetLibraryData(AuthenticationService.GetCurrentPrincipal(authTicket), libraryId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<LibraryData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<LibraryData>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Delete selected libraries
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="libraryIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteLibraries(string authTicket, int[] libraryIds)
        {
            try
            {
                return new ServiceOperationResult<object>
                           {
                               CallSuccess = true,
                               ResultData = SurveyManagementServiceImplementation.DeleteLibraries(AuthenticationService.GetCurrentPrincipal(authTicket), libraryIds)
                           };
            }
            catch (NoUserException ex)
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

        /// <summary>
        /// Changes item position
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="itemId"></param>
        /// <param name="newPageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MoveSurveyItem(string authToken, int responseTemplateId, int itemId, int? newPageId, int position)
        {
            try
            {
                SurveyManagementServiceImplementation.MoveSurveyItem(
                    AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, itemId, newPageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException)
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


        /// <summary>
        /// Changes item position
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MoveSurveyPage(string authToken, int responseTemplateId, int pageId, int position)
        {
            try
            {
                SurveyManagementServiceImplementation.MoveSurveyPage(
                    AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId, pageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException)
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

        /// <summary>
        /// Delete the specified folder
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteFolder(string authTicket, int folderId)
        {
            try
            {

                return new ServiceOperationResult<bool>
                           {
                               CallSuccess = true,
                               ResultData = SurveyManagementServiceImplementation.DeleteFolder(AuthenticationService.GetCurrentPrincipal(authTicket),
                                                                   folderId)
                           };
            }
            catch (NoUserException ex)
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
        /// Delete the specified survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteSurvey(string authTicket, int surveyId)
        {
            try
            {

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.DeleteSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId)
                };
            }
            catch (NoUserException ex)
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
        /// Add new survey page
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddSurveyPage(string authToken, int responseTemplateId)
        {
            try
            {
                SurveyManagementServiceImplementation.AddSurveyPage(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId);
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

        /// <summary>
        /// Copy the specific survey page. The new page will follow after the source page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> CopySurveyPage(string authTicket, int surveyId, int pageId)
        {
            try
            {
                SurveyManagementServiceImplementation.CopySurveyPage(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId);
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

        /// <summary>
        /// Delete the specific survey page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteSurveyPage(string authTicket, int surveyId, int pageId)
        {
            try
            {
                SurveyManagementServiceImplementation.DeleteSurveyPage(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, pageId);
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
        /// <summary>
        /// Delete the specific survey item
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteSurveyItem(string authTicket, int surveyId, int itemId)
        {
            try
            {
                SurveyManagementServiceImplementation.DeleteSurveyItem(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, itemId);
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

        /// <summary>
        /// Add response pipe to survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="pipeValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddResponsePipeToSurvey(string authTicket, int surveyId, int itemId, string pipeValue)
        {
            try
            {
                SurveyManagementServiceImplementation.AddResponsePipeToSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, itemId, pipeValue);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<SurveyListItem>[]> Search(string authTicket, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<SurveyListItem>[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.Search(AuthenticationService.GetCurrentPrincipal(authTicket), searchTerm)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<GroupedResult<SurveyListItem>[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<SurveyListItem>[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Inserts a page break after given page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="shouldPageBreak">determines whether page break should be inserted after page</param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddPageBreak(int pageId, bool shouldPageBreak, int templateId)
        {
            try
            {
                SurveyManagementServiceImplementation.AddPageBreak(pageId, shouldPageBreak, templateId);
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
                    FailureMessage = ex.Message
                };
            }
        } 
    }
}
