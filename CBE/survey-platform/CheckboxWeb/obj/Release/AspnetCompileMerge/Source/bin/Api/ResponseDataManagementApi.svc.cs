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
    public class ResponseDataManagementApi : IResponseDataManagementApi
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSummaryData> GetSurveyResponseSummary(string authToken, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<ResponseSummaryData>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.GetResponseSummary(AuthenticationService.GetCurrentPrincipal(authToken), surveyId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ResponseSummaryData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSummaryData>
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
        /// <param name="surveyId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseData[]> ListRecentSurveyResponses(string authToken, int surveyId, int count)
        {
            try
            {
                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.ListRecentResponses(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, count)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseData[]>
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
        /// <param name="surveyId"></param>
        /// <param name="responseId"></param>
        /// <param name="answerId"></param>
        /// <param name="answerText"></param>
        /// <param name="optionID"></param>
        /// <param name="dateCreated"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateResponseAnswer(string authToken, int surveyId, long responseId, long answerId, string answerText, int? optionID, DateTime? dateCreated)
        {
            try
            {
                ResponseDataServiceImplementation.UpdateResponseAnswer(
                    AuthenticationService.GetCurrentPrincipal(authToken), surveyId, responseId, answerId, answerText,
                    optionID, dateCreated);

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
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponses(
            string authToken, 
            int surveyId, 
            int pageNumber, 
            int pageSize, 
            string filterField, 
            string filterValue, 
            string sortField, 
            bool sortAscending)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.ListSurveyResponses(
                        AuthenticationService.GetCurrentPrincipal(authToken),
                        surveyId,
                        pageNumber,
                        pageSize,
                        filterField,
                        filterValue,
                        sortField,
                        sortAscending,
                        0,
                        null,
                        null)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export responses in with result in tabular form
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="DetailedResponseInfo"></param>
        /// <param name="DetailedUserInfo"></param>
        /// <param name="IncludeOpenEndedResults"></param>
        /// <param name="IncludeAliases"></param>
        /// <param name="IncludeHiddenItems"></param>
        /// <param name="IncludeIncompleteResponses"></param>
        /// <param name="StripHTMLTagsFromAnswers"></param>
        /// <param name="StripHTMLTagsFromQuestions"></param>
        /// <param name="MergeAnswersForSelectMany"></param>
        /// <param name="includeScoreData"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<TabularResponseExportData>> ExportResponsesTabular(
            string authToken,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            DateTime dtStart,
            DateTime dtEnd,
            bool DetailedResponseInfo,
            bool DetailedUserInfo,
            bool IncludeOpenEndedResults,
            bool IncludeAliases,
            bool IncludeHiddenItems,
            bool IncludeIncompleteResponses,
            bool StripHTMLTagsFromAnswers,
            bool StripHTMLTagsFromQuestions,
            bool MergeAnswersForSelectMany,
            bool includeScoreData)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<TabularResponseExportData>>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.ExportResponsesTabular(
                        AuthenticationService.GetCurrentPrincipal(authToken),
                        surveyId,
                        pageNumber,
                        resultsPerPage,
                        filterField,
                        filterValue,
                        sortField,
                        sortAscending,
                        period,
                        dtStart,
                        dtEnd,
                        DetailedResponseInfo,
                        DetailedUserInfo,
                        IncludeOpenEndedResults,
                        IncludeAliases,
                        IncludeHiddenItems,
                        IncludeIncompleteResponses,
                        StripHTMLTagsFromAnswers,
                        StripHTMLTagsFromQuestions,
                        MergeAnswersForSelectMany,
                        includeScoreData)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<TabularResponseExportData>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<TabularResponseExportData>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export responses
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="DetailedResponseInfo"></param>
        /// <param name="DetailedUserInfo"></param>
        /// <param name="IncludeOpenEndedResults"></param>
        /// <param name="IncludeAliases"></param>
        /// <param name="IncludeHiddenItems"></param>
        /// <param name="IncludeIncompleteResponses"></param>
        /// <param name="StripHTMLTagsFromAnswers"></param>
        /// <param name="ExportRankOrderPoints"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ResponseExportData>> ExportResponses(
            string authToken,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            DateTime dtStart,
            DateTime dtEnd,
            bool DetailedResponseInfo,
            bool DetailedUserInfo,
            bool IncludeOpenEndedResults,
            bool IncludeAliases,
            bool IncludeHiddenItems,
            bool IncludeIncompleteResponses,
            bool StripHTMLTagsFromAnswers)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ResponseExportData>>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.ExportResponses(
                        AuthenticationService.GetCurrentPrincipal(authToken),
                        surveyId,
                        pageNumber,
                        resultsPerPage,
                        filterField,
                        filterValue,
                        sortField,
                        sortAscending,
                        period,
                        dtStart,
                        dtEnd,
                        DetailedResponseInfo,
                        DetailedUserInfo,
                        IncludeOpenEndedResults,
                        IncludeAliases,
                        IncludeHiddenItems,
                        IncludeIncompleteResponses,
                        StripHTMLTagsFromAnswers)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<ResponseExportData>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ResponseExportData>>
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
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponsesByPeriod(
            string authToken,
            int surveyId,
            int pageNumber,
            int pageSize,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.ListSurveyResponses(
                        AuthenticationService.GetCurrentPrincipal(authToken),
                        surveyId,
                        pageNumber,
                        pageSize,
                        filterField,
                        filterValue,
                        sortField,
                        sortAscending,
                        period,
                        dateFieldName,
                        null)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
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
        /// <param name="surveyId"></param>
        /// <param name="responseIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteSurveyResponses(string authToken, int surveyId, long[] responseIds)
        {
            try
            {
                ResponseDataServiceImplementation.DeleteResponses(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, responseIds);
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
        /// <param name="surveyId"></param>
        /// <param name="numberOfRecentMonths"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetLifecycleResponseData(string authToken, int surveyId, int numberOfRecentMonths)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.GetLifecycleResponseDataInMonths(
                        AuthenticationService.GetCurrentPrincipal(authToken), surveyId, numberOfRecentMonths)
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
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetLifecycleResponseData(string authToken, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.GetLifecycleResponseDataInDays(
                        AuthenticationService.GetCurrentPrincipal(authToken), surveyId, periodLengthInDays, numberOfPeriods)
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
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteAllSurveyResponses(string authToken, int surveyId)
        {
            try
            {
                ResponseDataServiceImplementation.DeleteAllSurveyResponses(AuthenticationService.GetCurrentPrincipal(authToken), surveyId);
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
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteAllSurveyTestResponses(string authToken, int surveyId)
        {
            try
            {
                ResponseDataServiceImplementation.DeleteTestSurveyResponses(AuthenticationService.GetCurrentPrincipal(authToken), surveyId);
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
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<ResponseData>[]> Search(string authToken, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<ResponseData>[]>
                {
                    CallSuccess = true,
                    ResultData = ResponseDataServiceImplementation.Search(AuthenticationService.GetCurrentPrincipal(authToken), searchTerm)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<GroupedResult<ResponseData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<ResponseData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
    }
}
