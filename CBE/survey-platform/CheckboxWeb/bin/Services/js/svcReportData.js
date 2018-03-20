/****************************************************************************
* svcReportData.js                                                          *
*    Helper class and methods for accessing report data services.           *
****************************************************************************/

//Instance of helper object
var svcReportData = new reportDataObj();

//Response data JS helper
function reportDataObj() {

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('ReportDataService.svc', 'json', operationName);
    };

    this.getResultsForSurveyItemD = function (authTicket, surveyId, surveyItemId, includeIncompleteResponses, languageCode, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcReportData.getServiceUrl('GetResultsForSurveyItem'),
            {authTicket:authTicket, surveyId:surveyId, surveyItemId:surveyItemId, includeIncompleteResponses:includeIncompleteResponses, languageCode:languageCode},
            callbackArgs);
    };
}
