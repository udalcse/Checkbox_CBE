/****************************************************************************
* svcStyleManagement.js                                                     *
*    Helper class and methods for accessing style management service.       *
****************************************************************************/

//Instance of helper object
var svcStyleManagement = new styleManagementObj();

//Style management JS helper
function styleManagementObj() {

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('StyleManagementService.svc', 'json', operationName);
    };

    //List form styles
    this.listFormStyles = function(authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListFormStyles'),
            {
                authToken: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue
            },
            callback,
            callbackArgs);
    };

    //List form styles -- Deferred
    this.listFormStylesD = function(authTicket, pagingArgs, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('ListFormStyles'),
            {
                authToken: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue
            },
            callbackArgs);
    };

    //List chart styles
    this.listChartStyles = function(authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListChartStyles'),
            {
                authToken: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue
            },
            callback,
            callbackArgs);
    };

    //Get list item data for single style list item
    this.getStyleListItem = function(authTicket, styleId, type, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetStyleListItem'),
            { styleId: styleId, authTicket: authTicket, type: type },
            callback,
            callbackArgs);
    };

    //Delete the specified styles
    this.deleteFormStyles = function(authTicket, styleIds, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteFormStyles'),
            { authTicket: authTicket, styleIds: styleIds },
            callback,
            callbackArgs);
    };

    //Delete the specified style
    this.deleteFormStyle = function(authTicket, styleId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteFormStyle'),
            { authTicket: authTicket, styleId: styleId },
            callback,
            callbackArgs);
    };

    //Delete the specified styles
    this.deleteChartStyles = function(authTicket, styleIds, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteChartStyles'),
            { authTicket: authTicket, styleIds: styleIds },
            callback,
            callbackArgs);
    };

    //Delete the specified style
    this.deleteChartStyle = function(authTicket, styleId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteChartStyle'),
            { authTicket: authTicket, styleId: styleId },
            callback,
            callbackArgs);
    };

    //Copy the specified style
    this.copyStyle = function(authTicket, styleId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('CopyStyle'),
            { authTicket: authTicket, styleId: styleId },
            callback,
            callbackArgs);
    };
}