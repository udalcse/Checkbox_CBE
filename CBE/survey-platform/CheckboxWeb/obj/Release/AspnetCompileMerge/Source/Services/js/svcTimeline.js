/****************************************************************************
 * svcTimeline.js                                                      *
 *    Helper class and methods for authorizing access to Checkbox secured   * 
 *    resources.                                                            *
 ****************************************************************************/

//Instance of helper object
var svcTimeline = new timelineServiceObj();

//Invitation management JS helper
function timelineServiceObj() { 
    //Get service url
    this.getServiceUrl = function (operationName) {
        return serviceHelper.getServiceUrl('TimelineService.svc', 'json', operationName);
    };

    //Get timeline
    this.getTimeline = function (uniqueIdentifier, manager, requestId, parentObjectID, parentObjectType, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            '',
            svcTimeline.getServiceUrl('GetTimeline'),
            { userUniqueIdentifier: uniqueIdentifier, manager: manager, requestId: requestId, parentObjectID: parentObjectID ? parentObjectID : 0, parentObjectType: parentObjectType},
            callback,
            callbackArgs);
    };

    this.GetTimelineSettingsList = function (authTicket, managerName, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcTimeline.getServiceUrl('GetTimelineSettings'),
            {authTicket:authTicket, manager: managerName },
            callbackArgs);
    };

    this.UpdateTimelineSettingsForEvent = function (authTicket, managerName, eventName, periodName, value, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcTimeline.getServiceUrl('UpdateTimelineEventPeriod'),
            { authTicket: authTicket, manager: managerName, eventName: eventName, periodName: periodName, value: value },
            callbackArgs);
    };

    this.UpdateTimelineSettingsEventOrder = function(authTicket, managerName, eventName, eventOrder, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcTimeline.getServiceUrl('UpdateTimelineEventOrder'),
            { authTicket: authTicket, manager: managerName, eventName: eventName, eventOrder: eventOrder },
            callbackArgs);
    };
}