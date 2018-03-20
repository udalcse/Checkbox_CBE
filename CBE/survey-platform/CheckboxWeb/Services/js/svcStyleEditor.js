/****************************************************************************
* svcStyleManagement.js                                                     *
*    Helper class and methods for accessing style management service.       *
****************************************************************************/

//Instance of helper object
var svcStyleEditor = new styleEditorObj();

function styleEditorObj() {

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('StyleEditorService.svc', 'json', operationName);
    };

    //Get metadata for style
    this.getStyleData = function(authTicket, styleId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcStyleEditor.getServiceUrl('GetStyleData'),
            { authTicket: authTicket, styleId: styleId },
            callback,
            callbackArgs);
    };

    this.getStyleElementProperty = function(authTicket, styleId, elementName, property, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcStyleEditor.getServiceUrl('GetStyleElementProperty'),
            { authTicket: authTicket, styleId: styleId, elementName: elementName, property: property },
            callback,
            callbackArgs);
    };

    this.getStyleElementProperties = function(authTicket, styleId, elementName, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcStyleEditor.getServiceUrl('GetStyleElementProperties'),
            { authTicket: authTicket, styleId: styleId, elementName: elementName },
            callback,
            callbackArgs);
    };

    this.saveFormStyle = function(authTicket, styleId, languageCode, styleData, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcStyleEditor.getServiceUrl('SaveFormStyle'),
            { authTicket: authTicket, styleId: styleId, languageCode: languageCode, style: styleData },
            callback,
            callbackArgs);
    };

    this.updateStyleTemplateSetting = function(authTicket, styleId, settingName, newValue, callbackArgs) {
        serviceHelper.makeServiceCallD(
            authTicket,
            svcStyleEditor.getServiceUrl('UpdateStyleTemplateSetting'),
            { authTicket: authTicket, styleId: styleId, settingName: settingName, newValue: newValue },
            callbackArgs);
    };
}
