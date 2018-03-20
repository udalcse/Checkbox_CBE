using System;
using System.ServiceModel.Activation;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// Service implementation for survey management service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SurveyManagementApi : ISurveyManagementApi
    {
        /// <summary>
        /// List surveys and folders for authenticated users.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentFolderId">The id of the folder to list results for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFolders(string authToken, int parentFolderId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authToken), parentFolderId, pageNumber, pageSize, "Name", filter, 0, null, includeSurveyResponseCount, true, true)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentFolderId">The id of the folder to list results for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriod(string authToken, int parentFolderId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authToken), parentFolderId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount, true, true)
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
        /// <param name="authToken"></param>
        /// <param name="parentFolderId"></param>
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
        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriodByActiveStatus(string authToken, int parentFolderId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveysAndFolders(AuthenticationService.GetCurrentPrincipal(authToken), parentFolderId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount, includeActive, includeInactive)
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
        /// List available surveys for authenticated users.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentId"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<SurveyListItem[]>> ListAvailableSurveys(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<SurveyListItem[]>>
                {
                    CallSuccess = true,                    
                    ResultData = SurveyManagementServiceImplementation.ListAvailableSurveys(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="listItemId"></param>
        /// <param name="listItemType"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyListItem> GetSurveyListItem(string authToken, int listItemId, string listItemType)
        {
            try
            {
                return new ServiceOperationResult<SurveyListItem>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyListItem(AuthenticationService.GetCurrentPrincipal(authToken), listItemId, listItemType, true)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyMetaData> GetSurveyMetaData(string authToken, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<SurveyMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyMetaData(AuthenticationService.GetCurrentPrincipal(authToken), surveyId)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyPageMetaData> GetSurveyPageData(string authToken, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<SurveyPageMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyPageMetaData(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata> GetSurveyItemData(string authToken, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetSurveyItemMetaData(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyPage(string authToken, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageCondition(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId, TextManager.DefaultLanguage)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData[]> GetBranchDataForSurveyPage(string authToken, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageBranches(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId, TextManager.DefaultLanguage)
                };
            }
            catch(NoUserException ex)
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
        /// Get logic for survey page
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PageLogic> GetLogicForSurveyPage(string authToken, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<PageLogic>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetPageLogic(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId, TextManager.DefaultLanguage)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyItem(string authToken, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<RuleMetaData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetItemCondition(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId, TextManager.DefaultLanguage)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata> GetLibraryItemData(string authToken, int libraryId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetLibraryItemMetaData(AuthenticationService.GetCurrentPrincipal(authToken), libraryId, itemId)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<StyleListItem[]>> ListSurveyStyleTemplates(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<StyleListItem[]>>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListSurveyStyleTemplates(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField,filterValue)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<LibraryData[]>> ListLibraries(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<LibraryData[]>>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.ListItemLibraries(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        public ServiceOperationResult<LibraryData> GetLibrary(string authToken, int libraryId)
        {
            try
            {
                return new ServiceOperationResult<LibraryData>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.GetLibraryData(AuthenticationService.GetCurrentPrincipal(authToken), libraryId)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteLibraries(string authToken, int[] libraryIds)
        {
            try
            {
                return new ServiceOperationResult<object>
                           {
                               CallSuccess = true,
                               ResultData = SurveyManagementServiceImplementation.DeleteLibraries(AuthenticationService.GetCurrentPrincipal(authToken), libraryIds)
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteFolder(string authToken, int folderId)
        {
            try
            {

                return new ServiceOperationResult<bool>
                           {
                               CallSuccess = true,
                               ResultData = SurveyManagementServiceImplementation.DeleteFolder(AuthenticationService.GetCurrentPrincipal(authToken),
                                                                   folderId)
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
        /// Delete the specified survey
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteSurvey(string authToken, int surveyId)
        {
            try
            {

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.DeleteSurvey(AuthenticationService.GetCurrentPrincipal(authToken), surveyId)
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
        /// Copy the specific survey page. The new page will follow after the source page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> CopySurveyPage(string authToken, int surveyId, int pageId)
        {
            try
            {
                SurveyManagementServiceImplementation.CopySurveyPage(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId);
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteSurveyPage(string authToken, int surveyId, int pageId)
        {
            try
            {
                SurveyManagementServiceImplementation.DeleteSurveyPage(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId);
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
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteSurveyItem(string authToken, int surveyId, int itemId)
        {
            try
            {
                SurveyManagementServiceImplementation.DeleteSurveyItem(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId);
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
        /// <param name="authToken"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<SurveyListItem>[]> Search(string authToken, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<SurveyListItem>[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyManagementServiceImplementation.Search(AuthenticationService.GetCurrentPrincipal(authToken), searchTerm)
                };
            }
            catch(NoUserException ex)
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
    }
}
