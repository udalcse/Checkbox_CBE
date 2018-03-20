/****************************************************************************
 * svcSearch.js                                                      *
 *    Helper class and methods for authorizing access to Checkbox secured   * 
 *    resources.                                                            *
 ****************************************************************************/

//Instance of helper object
var svcSearch = new SearchServiceObj();

//Invitation management JS helper
function SearchServiceObj() { 
    //Get service url
    this.getServiceUrl = function (operationName) {
        return serviceHelper.getServiceUrl('SearchService.svc', 'json', operationName);
    };

    //init search
    this.Initialize = function (uniqueIdentifier, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            '',
            svcSearch.getServiceUrl('Initialize'),
            { userAuthID: uniqueIdentifier },
            callback,
            callbackArgs);
    };

    //run search or get search results
    this.Search = function (authTicket, Term, RequestID, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcSearch.getServiceUrl('Search'),
            { userAuthID: authTicket, Term: Term, RequestID: RequestID },
            callback,
            callbackArgs);
    };

    //get all search settings
    this.GetSearchSettings = function (authTicket, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcSearch.getServiceUrl('GetSearchSettings'),
            {},
            callback,
            callbackArgs);
    };

    //change display order for object type
    this.UpdateSearchResultsOrder = function (authTicket, objectType, order, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcSearch.getServiceUrl('UpdateSearchResultsOrder'),
            {objectType : objectType,
            order : order},
            callback,
            callbackArgs);
    };

    //change roles which users may search for the objects of the given type
    this.UpdateObjectsRoles = function (authTicket, objectType, roles, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcSearch.getServiceUrl('UpdateObjectsRoles'),
            { 
                objectType: objectType,
                roles: roles
            },
            callback,
            callbackArgs);
    };

    //include or exclude these objects in universal search
    this.ToggleSearchObjectType = function (authTicket, objectType, included, callback, callbackArgs)
    {
        return serviceHelper.makeServiceCall(
            authTicket,
            svcSearch.getServiceUrl('ToggleSearchObjectType'),
            {
                objectType: objectType,
                included: included
            },
            callback,
            callbackArgs);
    }

}