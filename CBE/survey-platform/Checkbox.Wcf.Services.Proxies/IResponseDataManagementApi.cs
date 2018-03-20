using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for the response data API, which allows extraction of response data from Checkbox.
    /// </summary>
    [ServiceContract]
    public interface IResponseDataManagementApi
    {
        /// <summary>
        /// Retrieve a summary of responses to a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="ResponseSummaryData"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the response summary data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseSummaryData> GetSurveyResponseSummary(string authToken, int surveyId);

        /// <summary>
        /// Retrieve a list of recent responses to a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="count">The maximum number of responses to retrieve.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="ResponseData"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the responses.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseData[]> ListRecentSurveyResponses(string authToken, int surveyId, int count);

        /// <summary>
        /// Updates response answer
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="responseId">The id of the response.</param>
        /// <param name="answerId">The id of the answer.</param>
        /// <param name="answerText">The text of the answer.</param>
        /// <param name="optionID">The id of the option.</param>
        /// <param name="dateCreated">Date when answer was created.</param>
        ///<returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateResponseAnswer(string authToken, int surveyId, long responseId,
                                                            long answerId, string answerText, int? optionID,
                                                            DateTime? dateCreated);

        /// <summary>
        /// Get a paged, sorted, and filtered list of survey responses.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="ResponseData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of responses that match the filter criteria and are members of the user group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponses(string authToken, 
                                                                                    int surveyId, 
                                                                                    int pageNumber, 
                                                                                    int pageSize, 
                                                                                    string filterField, 
                                                                                    string filterValue, 
                                                                                    string sortField,
                                                                                    bool sortAscending);

        /// <summary>
        /// Get a paged, sorted, and filtered list of survey responses.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="ResponseData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of responses that match the filter criteria and are members of the user group.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponsesByPeriod(string authToken,
                                                                                    int surveyId,
                                                                                    int pageNumber,
                                                                                    int pageSize,
                                                                                    string filterField,
                                                                                    string filterValue,
                                                                                    string sortField,
                                                                                    bool sortAscending,
                                                                                    int period,
                                                                                    string dateFieldName);

        /// <summary>
        /// Delete one or more survey response.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="responseIds">The list of response ids to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyResponses(string authToken, int surveyId, long[] responseIds);

        /// <summary>
        /// Delete all responses to a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteAllSurveyResponses(string authToken, int surveyId);

        /// <summary>
        /// Delete all test responses to a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteAllSurveyTestResponses(string authToken, int surveyId);

        /// <summary>
        /// Search for responses.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="searchTerm">The value to search for.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="GroupedResult{T}"/> of <see cref="ResponseData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of responses that match the search criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<ResponseData>[]> Search(string authToken, string searchTerm);

        /// <summary>
        /// Get lifecycle response data
        /// </summary>
        /// <param name="authTicket">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="numberOfRecentMonths"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetLifecycleResponseData(string authTicket, int surveyId, int numberOfRecentMonths);

        /// <summary>
        /// Get lifecycle response data
        /// </summary>
        /// <param name="authTicket">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetLifecycleResponseData(string authTicket, int surveyId, int periodLengthInDays, int numberOfPeriods);


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
