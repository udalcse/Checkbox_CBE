using System;

namespace Checkbox.Wcf.Services.Proxies
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class ResponseDataServiceProxy : System.ServiceModel.ClientBase<IResponseDataService>, IResponseDataService
    {

        public ResponseDataServiceProxy()
        {
        }

        public ResponseDataServiceProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ResponseDataServiceProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ResponseDataServiceProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ResponseDataServiceProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<ResponseSummaryData> GetSurveyResponseSummary(string authTicket, int surveyId)
        {
            return Channel.GetSurveyResponseSummary(authTicket, surveyId);
        }

        public ServiceOperationResult<object> UpdateResponseAnswer(string authToken, int surveyId, long responseId, long answerId, string answerText, int? optionID, DateTime? dateCreated)
        {
            return Channel.UpdateResponseAnswer(authToken, surveyId, responseId, answerId, answerText, optionID, dateCreated);
        }

        public ServiceOperationResult<string> GetLifecycleResponseDataInMonths(string authToken, int surveyId, int numberOfRecentMonths)
        {
            return Channel.GetLifecycleResponseDataInMonths(authToken, surveyId, numberOfRecentMonths);
        }

        public ServiceOperationResult<string> GetLifecycleResponseDataInDays(string authToken, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            return Channel.GetLifecycleResponseDataInDays(authToken, surveyId, periodLengthInDays, numberOfPeriods);
        }

        public ServiceOperationResult<ResponseAggregatedData> GetLifecycleAggregatedResponseDataInDays(string authTicket, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            return Channel.GetLifecycleAggregatedResponseDataInDays(authTicket, surveyId, periodLengthInDays, numberOfPeriods);
        }


        public ServiceOperationResult<ResponseData[]> ListRecentSurveyResponses(string authTicket, int surveyId, int count)
        {
            return Channel.ListRecentSurveyResponses(authTicket, surveyId, count);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponses(string authTicket, int surveyId, int pageNumber, int resultsPerPage, string filterField, string filterValue, string sortField, bool sortAscending)
        {
            return Channel.ListSurveyResponses(authTicket, surveyId, pageNumber, resultsPerPage, filterField, filterValue, sortField, sortAscending);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListSurveyResponsesByPeriod(string authTicket, int surveyId, int pageNumber, int resultsPerPage, string filterField, string filterValue, string sortField, bool sortAscending, int period, string dateFieldName)
        {
            return Channel.ListSurveyResponsesByPeriod(authTicket, surveyId, pageNumber, resultsPerPage, filterField, filterValue, sortField, sortAscending, period, dateFieldName);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListFilteredSurveyResponsesByPeriod(string authTicket, int surveyId, int pageNumber, int resultsPerPage, string filterField, string filterValue, string sortField, bool sortAscending, int period, string dateFieldName, string filterKey, int profileFieldId = 0)
        {
            return Channel.ListFilteredSurveyResponsesByPeriod(authTicket, surveyId, pageNumber, resultsPerPage, filterField, filterValue, sortField, sortAscending, period, dateFieldName, filterKey, profileFieldId);
        }

        public ServiceOperationResult<object> DeleteSurveyResponses(string authTicket, int surveyId, long[] responseList)
        {
            return Channel.DeleteSurveyResponses(authTicket, surveyId, responseList);
        }

        public ServiceOperationResult<object> DeleteAllSurveyResponses(string authTicket, int surveyId)
        {
            return Channel.DeleteAllSurveyResponses(authTicket, surveyId);
        }

        public ServiceOperationResult<object> DeleteAllSurveyTestResponses(string authTicket, int surveyId)
        {
            return Channel.DeleteAllSurveyTestResponses(authTicket, surveyId);
        }

        public ServiceOperationResult<GroupedResult<ResponseData>[]> Search(string authTicket, string searchTerm)
        {
            return Channel.Search(authTicket, searchTerm);
        }

        public ServiceOperationResult<ResponseItemAnswerData[]> GetAnswersForResponseByGuid(string authTicket, int surveyId, string languageCode, Guid responseGuid)
        {
            return Channel.GetAnswersForResponseByGuid(authTicket, surveyId, languageCode, responseGuid);
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
