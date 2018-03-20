using System;

namespace Checkbox.Wcf.Services.Proxies
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class ResponseDataManagemenApiProxy : System.ServiceModel.ClientBase<IResponseDataManagementApi>, IResponseDataManagementApi
    {

        public ResponseDataManagemenApiProxy()
        {
        }

        public ResponseDataManagemenApiProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ResponseDataManagemenApiProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ResponseDataManagemenApiProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ResponseDataManagemenApiProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<ResponseSummaryData> GetSurveyResponseSummary(string authToken, int surveyId)
        {
            return Channel.GetSurveyResponseSummary(authToken, surveyId);
        }

        public ServiceOperationResult<object> UpdateResponseAnswer(string authToken, int surveyId, long responseId, long answerId, string answerText, int? optionID, DateTime? dateCreated)
        {
            return Channel.UpdateResponseAnswer(authToken, surveyId, responseId, answerId, answerText, optionID, dateCreated);
        }

        public ServiceOperationResult<ResponseData[]> ListRecentSurveyResponses(string authToken, int surveyId, int count)
        {
            return Channel.ListRecentSurveyResponses(authToken, surveyId, count);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponses(string authToken, int surveyId, int pageNumber, int pageSize, string filterField, string filterValue, string sortField, bool sortAscending)
        {
            return Channel.ListSurveyResponses(authToken, surveyId, pageNumber, pageSize, filterField, filterValue, sortField, sortAscending);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponsesByPeriod(string authToken, int surveyId, int pageNumber, int pageSize, string filterField, string filterValue, string sortField, bool sortAscending, int period, string dateFieldName)
        {
            return Channel.ListSurveyResponsesByPeriod(authToken, surveyId, pageNumber, pageSize, filterField, filterValue, sortField, sortAscending, period, dateFieldName);
        }

        public ServiceOperationResult<object> DeleteSurveyResponses(string authToken, int surveyId, long[] responseIds)
        {
            return Channel.DeleteSurveyResponses(authToken, surveyId, responseIds);
        }

        public ServiceOperationResult<object> DeleteAllSurveyResponses(string authToken, int surveyId)
        {
            return Channel.DeleteAllSurveyResponses(authToken, surveyId);
        }

        public ServiceOperationResult<object> DeleteAllSurveyTestResponses(string authToken, int surveyId)
        {
            return Channel.DeleteAllSurveyTestResponses(authToken, surveyId);
        }

        public ServiceOperationResult<GroupedResult<ResponseData>[]> Search(string authToken, string searchTerm)
        {
            return Channel.Search(authToken, searchTerm);
        }

        public ServiceOperationResult<string> GetLifecycleResponseData(string authToken, int surveyId, int numberOfRecentMonths)
        {
            return Channel.GetLifecycleResponseData(authToken, surveyId, numberOfRecentMonths);
        }

        public ServiceOperationResult<string> GetLifecycleResponseData(string authToken, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            return Channel.GetLifecycleResponseData(authToken, surveyId, periodLengthInDays, numberOfPeriods);
        }

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
            return Channel.ExportResponses(authToken,
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
                StripHTMLTagsFromAnswers);
        }

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
            return Channel.ExportResponsesTabular(authToken,
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
                includeScoreData);
        }
    }
}
