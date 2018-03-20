/****************************************************************************
 * svcReportManagement.js                                                   *
 *    Helper class and methods for accessing report management services.    *
 ****************************************************************************/

//Instance of helper object
var svcReportManagement = new reportManagementObj();

//Report management JS helper
function reportManagementObj() {

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('ReportMetaDataService.svc', 'json', operationName);
    };

    //Get reports for survey
    this.listReportsForSurvey = function(authTicket, surveyId, nameFilter, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListReportsForSurvey'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                resultsPerPage: pagingArgs.resultsPerPage,
                nameFilter: nameFilter,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending
            },
            callback,
            callbackArgs);
    };

        //Get reports for survey
        this.listReportsByPeriod = function (authTicket, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListReportsByPeriod'),
            {
                authTicket: authTicket,
                period: pagingArgs.period,
                dateFieldName: pagingArgs.dateFieldName,
                pageNumber: pagingArgs.pageNumber,
                resultsPerPage: pagingArgs.resultsPerPage,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending
            },
            callback,
            callbackArgs);
        };

    //Get filters for survey
    this.listReportFiltersForSurvey = function(authTicket, surveyId, nameFilter, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListReportFiltersForSurvey'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                resultsPerPage: pagingArgs.pageSize,
                nameFilter: nameFilter,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending
            },
            callback,
            callbackArgs);
    };

    this.deleteFilters = function(authTicket, surveyId, filterIds, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcReportManagement.getServiceUrl('DeleteFilters'),
            {
                authTicket: authTicket,
                surveyId: surveyId,
                Ids: filterIds
            },
            callback,
            callbackArgs);
    };

    //List recent reports for survey
    this.listRecentReportsForSurvey = function(authTicket, surveyId, count, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListRecentReportsForSurvey'),
            { responseTemplateId: surveyId, authTicket: authTicket, count: count },
            callback,
            callbackArgs);
    };

    //List recent reports for survey
    this.listRecentReportsForSurveyD = function (authTicket, surveyId, count, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcReportManagement.getServiceUrl('ListRecentReportsForSurvey'),
            { responseTemplateId: surveyId, authTicket: authTicket, count: count },
            callbackArgs);
    };

    //Get report count for the specified survey
    this.getReportCountForSurvey = function(authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('GetReportCountForSurvey'),
            {
                authTicket: authTicket,
                responseTemplateId: surveyId
            },
            callback,
            callbackArgs);
    };

    //Get metadata for report
    this.getReportMetaData = function(authTicket, reportId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('GetReportWithId'),
            { reportId: reportId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get metadata for report page
    this.getReportPageMetaData = function(authTicket, reportId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('GetReportPageWithId'),
            { authTicket: authTicket, reportId: reportId, templatePageId: pageId },
            callback,
            callbackArgs);
    };

    //Get metadata for report item
    this.getReportItemMetaData = function(authTicket, reportId, itemId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('GetItemMetaData'),
            { authTicket: authTicket, reportId: reportId, itemId: itemId },
            callback,
            callbackArgs);
    };

    //Get metadata for report page items
    this.listReportPageItemsData = function (authTicket, reportId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListPageItemsData'),
            { authTicket: authTicket, reportId: reportId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Search reports
    this.search = function(authTicket, searchTerm, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('Search'),
            { authTicket: authTicket, searchTerm: searchTerm },
            callback,
            callbackArgs);
    };

    //Delete recent reports for survey
    this.deleteReport = function(authTicket, reportId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcReportManagement.getServiceUrl('DeleteReport'),
            { authTicket: authTicket, reportID: reportId },
            callback,
            callbackArgs);
    };

    //Delete item in report
    this.deleteReportItem = function(authTicket, reportId, itemId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcReportManagement.getServiceUrl('DeleteItem'),
            { authTicket: authTicket, reportId: reportId, itemId: itemId },
            callback,
            callbackArgs);
    };

    //Delete page in report
    this.deleteReportPage = function(authTicket, reportId, pageId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcReportManagement.getServiceUrl('DeletePage'),
            { authTicket: authTicket, reportId: reportId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //move report item
    this.moveReportItem = function (authTicket, reportId, itemId, newPageId, position, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcReportManagement.getServiceUrl('MoveReportItem'),
        {
            authTicket: authTicket,
            reportId: reportId,
            itemId: itemId,
            newPageId: newPageId,
            position: position
        },
        callback,
        callbackArgs);
    };

    //move report page
    this.moveReportPage = function (authTicket, reportId, pageId, position, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcReportManagement.getServiceUrl('MoveReportPage'),
        {
            authTicket: authTicket,
            reportId: reportId,
            pageId: pageId,
            position: position
        },
        callback,
        callbackArgs);
    };

    //add new report page
    this.addReportPage = function (authTicket, reportId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcReportManagement.getServiceUrl('AddReportPage'),
        {
            authTicket: authTicket,
            reportId: reportId
        },
        callback,
        callbackArgs);
    };

    //List available reports
    this.listAvailableReports = function (authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcReportManagement.getServiceUrl('ListAvailableReports'),
            {
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                skipAuthentication: pagingArgs.skipAuthentication
            },
            callback,
            callbackArgs);
    };



}
