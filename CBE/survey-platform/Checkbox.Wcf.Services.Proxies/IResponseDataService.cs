using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for response data service, which allows extraction of response data from Checkbox.
    /// </summary>
    [ServiceContract]
    public interface IResponseDataService
    {
        /// <summary>
        /// Get a summary data object with some basic information about responses to survey.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseSummaryData> GetSurveyResponseSummary(string authTicket, int surveyId);

        /// <summary>
        /// List the [count] most recent responses to the survey.  
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseData[]> ListRecentSurveyResponses(string authTicket, int surveyId, int count);


        ///<summary>
        /// Updates response answer
        ///</summary>
        ///<param name="authToken"></param>
        ///<param name="surveyId"></param>
        ///<param name="responseId"></param>
        ///<param name="answerId"></param>
        ///<param name="answerText"></param>
        ///<param name="optionID"></param>
        ///<param name="dateCreated"></param>
        ///<returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateResponseAnswer(string authToken, int surveyId, long responseId,
                                                            long answerId, string answerText, int? optionID,
                                                            DateTime? dateCreated);

        /// <summary>
        /// List a page of responses to the survey.   If page value or resulsts per page is less than or equal to zero,
        /// all responses are returned.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponses(
            string authTicket, 
            int surveyId, 
            int pageNumber, 
            int resultsPerPage, 
            string filterField, 
            string filterValue, 
            string sortField, 
            bool sortAscending);

        /// <summary>
        /// List a page of responses to the survey.   If page value or resulsts per page is less than or equal to zero,
        /// all responses are returned.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponsesByPeriod(
            string authTicket,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            string dateFieldName);

        /// <summary>
        /// List a page of responses to the survey.   If page value or resulsts per page is less than or equal to zero,
        /// all responses are returned.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="profileFieldId"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListFilteredSurveyResponsesByPeriod(
            string authTicket,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            string dateFieldName,
            string filterKey,
            int profileFieldId);

        /// <summary>
        /// Delete the specified list of responses
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="responseList"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyResponses(string authTicket, int surveyId, long[] responseList);

        /// <summary>
        /// Delete all responses for a survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteAllSurveyResponses(string authTicket, int surveyId);

        /// <summary>
        /// Delete all test responses for a survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteAllSurveyTestResponses(string authTicket, int surveyId);

        /// <summary>
        /// Search for responses
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<ResponseData>[]> Search(string authTicket, string searchTerm);

        /// <summary>
        /// Get lifecycle response data
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="numberOfRecentMonths"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetLifecycleResponseDataInMonths(string authTicket, int surveyId, int numberOfRecentMonths);

        /// <summary>
        /// Get lifecycle response data
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetLifecycleResponseDataInDays(string authTicket, int surveyId, int periodLengthInDays, int numberOfPeriods);

        /// <summary>
        /// Get aggregated lifecycle response data
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseAggregatedData> GetLifecycleAggregatedResponseDataInDays(string authTicket, int surveyId, int periodLengthInDays, int numberOfPeriods);

        /// <summary>
        /// Get the response answers for a specified response
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseItemAnswerData[]> GetAnswersForResponseByGuid(string authTicket, int surveyId, string languageCode, Guid responseGuid);

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
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseExportData>> ExportResponses(
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
            bool StripHTMLTagsFromAnswers);

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
        [OperationContract]
        [WebGet]
         ServiceOperationResult<PagedListResult<TabularResponseExportData>> ExportResponsesTabular(
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
            bool includeScoreData);
    }
}
