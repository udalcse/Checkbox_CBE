/****************************************************************************
* svcResponseData.js                                                        *
*    Helper class and methods for accessing response data service.          *
****************************************************************************/

//Instance of helper object
var svcResponseData = new responseDataObj();

//Response data JS helper
function responseDataObj() {
    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('ResponseDataService.svc', 'json', operationName);
    };

    //Get response summary count data for survey
    this.getResponseSummary = function(authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('GetSurveyResponseSummary'),
            { surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get response summary count data for survey
    this.getResponseSummaryD = function(authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcResponseData.getServiceUrl('GetSurveyResponseSummary'),
            { surveyId: surveyId, authTicket: authTicket },
            callbackArgs);
    };

    //Get get lifecycle response data for survey
    this.getLifecycleResponseDataInMonths = function (authTicket, surveyId, numberOfRecentMonths, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('GetLifecycleResponseDataInMonths'),
            { numberOfRecentMonths: numberOfRecentMonths, surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get get lifecycle response data for survey
    this.getLifecycleResponseDataInDays = function (authTicket, surveyId, periodLengthInDays, numberOfPeriods, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('GetLifecycleResponseDataInDays'),
            { periodLengthInDays: periodLengthInDays, numberOfPeriods: numberOfPeriods, surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get get lifecycle response data for survey
    this.getLifecycleAggregatedResponseDataInDays = function (authTicket, surveyId, periodLengthInDays, numberOfPeriods, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('GetLifecycleAggregatedResponseDataInDays'),
            { periodLengthInDays: periodLengthInDays, numberOfPeriods: numberOfPeriods, surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //List recent responses to survey
    this.listRecentResponses = function(authTicket, surveyId, count, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('ListRecentSurveyResponses'),
            { surveyId: surveyId, authTicket: authTicket, count: count },
            callback,
            callbackArgs);
    };

    //List recent responses to survey
    this.listRecentResponsesD = function (authTicket, surveyId, count, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcResponseData.getServiceUrl('ListRecentSurveyResponses'),
            { surveyId: surveyId, authTicket: authTicket, count: count },
            callbackArgs);
    };

    //List page of responses to a survey
    this.listSurveyResponses = function (authTicket, surveyId, pagingArgs, callback, callbackArgs, filterKey) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('ListFilteredSurveyResponsesByPeriod'),
            {
                surveyId: surveyId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                resultsPerPage: pagingArgs.resultsPerPage,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                profileFieldId: pagingArgs.profielFieldId,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                period: pagingArgs.period,
                dateFieldName: pagingArgs.dateFieldName,
                filterKey: filterKey
            },
            callback,
            callbackArgs);
    };

    //Delete Selected Responses
    this.deleteSelectedResponses = function(authTicket, surveyId, responseList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcResponseData.getServiceUrl('DeleteSurveyResponses'),
            {
                surveyId: surveyId,
                authTicket: authTicket,
                responseList: responseList
            },
            callback,
            callbackArgs);
    };

    //Delete All Responses
    this.deleteAllSurveyResponses = function(authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcResponseData.getServiceUrl('DeleteAllSurveyResponses'),
            {
                surveyId: surveyId,
                authTicket: authTicket
            },
            callback,
            callbackArgs);
    };

    //Delete Test Responses
    this.deleteTestSurveyResponses = function(authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcResponseData.getServiceUrl('DeleteAllSurveyTestResponses'),
            {
                surveyId: surveyId,
                authTicket: authTicket
            },
            callback,
            callbackArgs);
    };
    
    //Search
    this.search = function(authTicket, searchTerm, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcResponseData.getServiceUrl('Search'),
            { authTicket: authTicket, searchTerm: searchTerm },
            callback,
            callbackArgs
        );
    };
}

