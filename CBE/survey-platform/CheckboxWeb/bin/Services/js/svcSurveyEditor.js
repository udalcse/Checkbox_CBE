/****************************************************************************
 * svcSurveyEditor.js                                                       *
 *    Helper class and methods for editing surveys.                         *
 ****************************************************************************/

function SyncServiceCall(serviceUrl) {
    var functionResult = '';
    $.ajax({
        type: 'GET',
        async: false,
        url: serviceUrl,
        dataType: 'json',
        cache: false,
        error: serviceHelper.onAjaxError,
        success: function (result, code, xhr) {
            if (result == null || result.d == null) {
                serviceHelper.onAjaxError(null, 'Service call succeeded, but returned null data.  Checkbox error log may contain more details.', null);
            }
            functionResult = result.d.ResultData;
        }
    });
    return functionResult;
}

//Instance of helper object
var svcSurveyEditor = new surveyEditorObj();

//Invitation management JS helper
function surveyEditorObj() {

    //Get service url
    this.getServiceUrl = function (operationName) {
        return serviceHelper.getServiceUrl('SurveyEditorService.svc', 'json', operationName);
    };

    //Get invitation data
    this.listPipeSourcesD = function (authToken, surveyId, maxPagePosition, languageCode, callbackArgs) {
        if (surveyId == null) {
            surveyId = -1;
        }

        if (maxPagePosition == null) {
            maxPagePosition = -1;
        }

        var itemTypeId = 0;
        var selectedItemText = $(".pageItems .activeContent").find(".fixed_150.left").text();
        var itemType = $("#itemType").val();
        if (selectedItemText.indexOf("Single-Line") >= 0 || itemType === "singleline") itemTypeId = 1;
        else if (selectedItemText.indexOf("Radio") >= 0 || itemType === "radiobutton") itemTypeId = 3;

        if (itemTypeId === 0) {
            return serviceHelper.makeServiceCallD(
                authToken,
                svcSurveyEditor.getServiceUrl('ListPipeSources'),
                { authToken: authToken, surveyId: surveyId, maxPagePosition: maxPagePosition, languageCode: languageCode },
                callbackArgs);
        } else {
            return serviceHelper.makeServiceCallD(
                authToken,
                svcSurveyEditor.getServiceUrl('ListPipeSources'),
                { authToken: authToken, surveyId: surveyId, maxPagePosition: maxPagePosition, languageCode: languageCode, customFieldTypeId: itemTypeId },
                callbackArgs);
        }
    };

    //Returns date format of the current culture
    this.getDateFormat = function (authToken) {
        var serviceUrl = this.getServiceUrl('GetDateFormat').replace('//', '/');
        return SyncServiceCall(serviceUrl);
    };

    //Returns time format of the current culture
    this.getTimeFormat = function (authToken) {
        var serviceUrl = this.getServiceUrl('GetTimeFormat').replace('//', '/');
        return SyncServiceCall(serviceUrl);
    };     

    //Togggle on/off survey settings
    this.toggleSurveySettingD = function (authToken, surveyId, settingName, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('ToggleSetting'),
            { authToken: authToken, surveyId: surveyId, settingName: settingName },
            callbackArgs);
    };    
    
    //Togggle on/off survey settings
    this.updateSurveySettingD = function(authToken, surveyId, settingName, settingValueString, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('UpdateSetting'),
            { authToken: authToken, surveyId: surveyId, settingName: settingName, settingValueAsString:settingValueString},
            callbackArgs);
    };

    //Update alternate URL.  Empty value = remove mapping
    this.setAlternateUrlD = function (authToken, surveyId, altUrl, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('SetAlternateUrl'),
            { authToken: authToken, surveyId: surveyId, altUrl: altUrl},
            callbackArgs);
    };

    //Update alternate URL.  Empty value = remove mapping
    this.setStyleTemplateD = function (authToken, surveyId, styleTemplateId, type, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('SetStyleTemplate'),
            { authToken: authToken, surveyId: surveyId, styleTemplateId: styleTemplateId, styleType: type },
            callbackArgs);
    };

    //returns item active status
    this.getItemIsActiveD = function (authToken, surveyId, itemId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('GetItemIsActive'),
            { authToken: authToken, surveyId: surveyId, itemId: itemId },
            callbackArgs);
    };

    //returns item active status
    this.copyActionIsAvailableForItem = function (authToken, surveyId, itemId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('CopyActionIsAvailableForItem'),
            { authToken: authToken, surveyId: surveyId, itemId: itemId },
            callbackArgs);
    };

    //returns item active status
    this.getStatus = function (authToken, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('GetStatus'),
            { authToken: authToken, surveyId: surveyId },
            callbackArgs);
    };

    //returns autocomplete list items
    this.getAutocompleteListData = function (authToken, listId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('ListAutocompleteListData'),
            { authToken: authToken, listId: listId },
            callbackArgs);
    };

    //toggles item active status
    this.toggleItemActiveStatusD = function (authToken, surveyId, itemId, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('ToggleItemActiveStatus'),
            { authToken: authToken, surveyId: surveyId, itemId: itemId },
            callbackArgs);
    };

    //Get localizable text data associated with survey
    this.getLocalizableTextsD = function (authToken, surveyId, languageCode, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('GetLocalizableTexts'),
            { authToken: authToken, surveyId: surveyId, languageCode: languageCode },
            callbackArgs);
    };

    //Update text associated with a survey
    this.updateSurveyTextD = function (authToken, surveyId, textKey, textValue, languageCode, callbackArgs) {
        return serviceHelper.makeServicePostCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('UpdateSurveyText'),
            { authToken: authToken, surveyId: surveyId, textKey: textKey, textValue: textValue, languageCode: languageCode },
            callbackArgs);
    };

    //Determine whether or not rules will be changed if page deletes.
    this.willRulesBeChangedIfPageDeletes = function (authToken, surveyId, pageId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('WillRulesBeChangedIfPageDeletes'),
            { authToken: authToken, surveyId: surveyId, pageId: pageId },
            callbackArgs);
    };

    //Selects a default language for the survey
    this.setDefaultLanguage = function (authToken, surveyId, language, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('SetDefaultLanguage'),
            { authToken: authToken, surveyId: surveyId, language: language },
            callbackArgs);
    };
    
    //Determine whether or not rules will be changed if page deletes.
    this.updateConditionSourceD = function (authToken, surveyId, value, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('UpdateConditionSource'),
            { authToken: authToken, surveyId: surveyId, value: value },
            callbackArgs);
    };

    //Adds a default language for the survey
    this.addDefaultLanguage = function (authToken, surveyId, language, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('AddDefaultLanguage'),
            { authToken: authToken, surveyId: surveyId, language: language },
            callbackArgs);
    };

    //Adds a default language for the survey
    this.removeDefaultLanguage = function (authToken, surveyId, language, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authToken,
            svcSurveyEditor.getServiceUrl('RemoveDefaultLanguage'),
            { authToken: authToken, surveyId: surveyId, language: language },
            callbackArgs);
    };
    
}