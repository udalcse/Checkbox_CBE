/****************************************************************************
 * svcSurveyManagement.js                                                   *
 *    Helper class and methods for accessing survey management service.     *
 ****************************************************************************/

//Instance of helper object
var svcSurveyManagement = new surveyManagementObj();

//Survey management JS helper
function surveyManagementObj() {
    //Get service url
    this.getServiceUrl = function (operationName) {
        return serviceHelper.getServiceUrl('SurveyManagementService.svc', 'json', operationName);
    };

    //List surveys & folders contained in specified folder
    this.listSurveysAndFolders = function (authTicket, parentId, includeSurveyResponseCount, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListSurveysAndFolders'),
            { parentId: parentId, authTicket: authTicket, includeSurveyResponseCount: includeSurveyResponseCount },
            callback,
            callbackArgs);
    };

    //List surveys & folders contained in specified folder - DEFERRED OBJECT
    this.listSurveysAndFoldersD = function (authTicket, parentId, pageNumber, pageSize, includeSurveyResponseCount, filterField, filter, period, dateFieldName, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListSurveysAndFoldersByPeriod'),
            { parentId: parentId, authTicket: authTicket, includeSurveyResponseCount: includeSurveyResponseCount, pageNumber : pageNumber, pageSize : pageSize, filterField: filterField, filter : filter, period : period, dateFieldName : dateFieldName },
            callbackArgs);
    };

    this.listSurveysAndFoldersByActiveStatusD = function (authTicket, parentId, pageNumber, pageSize, includeSurveyResponseCount, filterField, filter, period, dateFieldName, includeActive, includeInactive, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListSurveysAndFoldersByPeriodByActiveStatus'),
            { parentId: parentId, authTicket: authTicket, includeSurveyResponseCount: includeSurveyResponseCount, pageNumber: pageNumber, pageSize: pageSize, filterField: filterField, filter: filter, period: period, dateFieldName: dateFieldName, includeActive: includeActive, includeInactive: includeInactive },
            callbackArgs);
    };

     this.listFavoriteSurveys = function (authTicket, parentId, pageNumber, pageSize, includeSurveyResponseCount, filter, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListFavoriteSurveys'),
            { parentId: parentId, authTicket: authTicket, includeSurveyResponseCount: includeSurveyResponseCount, pageNumber : pageNumber, pageSize : pageSize, filter : filter },
            callbackArgs);
    };

    this.AddFavoriteSurvey = function (authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('AddFavoriteSurvey'),
            {  authTicket: authTicket, surveyId: surveyId},
            callbackArgs);
    };

     this.RemoveFavoriteSurvey = function (authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('RemoveFavoriteSurvey'),
            {  authTicket: authTicket, surveyId: surveyId},
            callbackArgs);
    };

    this.IsFavoriteSurvey = function (authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('IsFavoriteSurvey'),
            {  authTicket: authTicket, surveyId: surveyId},
            callbackArgs);
    };
    
    //Get list item data for single survey list item
    this.getSurveyListItem = function (authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyListItem'),
            { listItemId: surveyId, authTicket: authTicket, listItemType: 'survey' },
            callback,
            callbackArgs);
    };

    //Get list item data for single folder list item
    this.getFolderListItem = function (authTicket, folderId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyListItem'),
            { listItemId: folderId, authTicket: authTicket, listItemType: 'folder' },
            callback,
            callbackArgs);
    };

    //Get metadata for survey
    this.getSurveyMetaData = function (authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyMetaData'),
            { surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get metadata for survey - DEFERRED OBJECT
    this.getSurveyMetaDataD = function (authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyMetaData'),
            { surveyId: surveyId, authTicket: authTicket },
            callbackArgs);
    };

    //Copy Survey
    this.copySurvey = function(authTicket, surveyId, surveyName, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl("CopySurvey"),
            {
                surveyId: surveyId,
                authTicket: authTicket,
                surveyName: surveyName
            }, callbackArgs);
    }

    //Copy Survey - DEFERRED OBJECT
    this.copySurveyD = function (authTicket, surveyId, surveyName, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl("CopySurvey"),
            {
                surveyId: surveyId,
                authTicket: authTicket,
                surveyName: surveyName
            }, callbackArgs);
    }

    //Get metadata for survey page
    this.getSurveyPageMetaData = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyPageData'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Get metadata for survey page
    this.ifAlternateUrlIsAvailable = function (authTicket, customUrl, serverApplicationPath, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('IfAlternateUrlIsAvailable'),
            { authTicket: authTicket, customUrl: customUrl, serverApplicationPath: serverApplicationPath },
            callback,
            callbackArgs);
    };

    //Get metadata for survey item
    this.listPageItemsData = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListPageItemsData'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Get metadata for survey item
    this.getSurveyItemMetaData = function (authTicket, surveyId, itemId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyItemData'),
            { authTicket: authTicket, surveyId: surveyId, itemId: itemId },
            callback,
            callbackArgs);
    };


    //Get metadata for survey item
    this.getSurveyItemMetaDataD = function (authTicket, surveyId, itemId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetSurveyItemData'),
            { authTicket: authTicket, surveyId: surveyId, itemId: itemId },
            callbackArgs);
    };

    //Get metadata for library item
    this.getLibraryItemMetaData = function (authTicket, libraryId, itemId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetLibraryItemData'),
            { authTicket: authTicket, libraryId: libraryId, itemId: itemId },
            callback,
            callbackArgs);
    };

    //Set library item options
    this.setItemLibraryOptions = function (authTicket, itemId, shouldShow, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('SetItemLibraryOptions'),
            { authTicket: authTicket, itemId: itemId, shouldShow: shouldShow },
            callback,
            callbackArgs);
    };

    //List available surveys
    this.listAvailableSurveys = function (authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListAvailableSurveys'),
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


    //List available style templates
    this.listSurveyStyleTemplates = function (authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListSurveyStyleTemplates'),
            {
                authTicket: authTicket,
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

    //List available item libraries
    this.listItemLibraries = function (authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListItemLibraries'),
            {
                authTicket: authTicket,
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

    //Get data for an item library
    this.getLibraryData = function (authTicket, libraryId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetLibraryData'),
            {
                authTicket: authTicket,
                libraryId: libraryId
            },
            callback,
            callbackArgs
            );
    };

    //delete selected libraries
    this.deleteLibraries = function (authTicket, libraryIds, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeleteLibraries'),
            {
                authTicket: authTicket,
                libraryIds: libraryIds
            },
            callback,
            callbackArgs);
    };

    //move survey item
    this.moveSurveyItem = function (authTicket, responseTemplateId, itemId, newPageId, position, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcSurveyManagement.getServiceUrl('MoveSurveyItem'),
        {
            authTicket: authTicket,
            responseTemplateId: responseTemplateId,
            itemId: itemId,
            newPageId: newPageId,
            position: position
        },
        callback,
        callbackArgs);
    };

    //move survey page
    this.moveSurveyPage = function (authTicket, responseTemplateId, pageId, position, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcSurveyManagement.getServiceUrl('MoveSurveyPage'),
        {
            authTicket: authTicket,
            responseTemplateId: responseTemplateId,
            pageId: pageId,
            position: position
        },
        callback,
        callbackArgs);
    };

    //move survey page
    this.addSurveyPage = function (authTicket, responseTemplateId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
        authTicket,
        svcSurveyManagement.getServiceUrl('AddSurveyPage'),
        {
            authTicket: authTicket,
            responseTemplateId: responseTemplateId,
        },
        callback,
        callbackArgs);
    };

    //TODO: Move to SurveyEditor Service
    //Delete the specified folder
    this.deleteFolder = function (authTicket, folderId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeleteFolder'),
            { authTicket: authTicket, folderId: folderId },
            callback,
            callbackArgs);
    };

    //TODO: Move to SurveyEditor Service
    //Delete the specified survey
    this.deleteSurvey = function (authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeleteSurvey'),
            { authTicket: authTicket, surveyId: surveyId },
            callback,
            callbackArgs);
    };

    //TODO: Move to SurveyEditor Service
    //Copy the specified survey page
    this.copySurveyPage = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('CopySurveyPage'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //TODO: Move to SurveyEditor Service
    //Delete the specified survey page
    this.deleteSurveyPage = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeleteSurveyPage'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //TODO: Move to SurveyEditor Service
    //Delete the specified survey item
    this.deleteSurveyItem = function (authTicket, surveyId, itemId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeleteSurveyItem'),
            { authTicket: authTicket, surveyId: surveyId, itemId: itemId },
            callback,
            callbackArgs);
    };

    this.addSurveyItemFromLibrary = function (pageId, itemId, surveyId, libraryId, callback, callbackArgs) {
        console.log("lib: " + libraryId);
        serviceHelper.makeServiceCall(
            "",
            svcSurveyManagement.getServiceUrl('AddItemsFromLibrary'),
            { pageId: pageId, itemId: itemId, responseTemplateId: surveyId, libraryId: libraryId },
            callback,
            callbackArgs);
    }

    //Add response pipe to the specific survey
    this.addResponsePipeToSurvey = function (authTicket, surveyId, itemId, pipeValue, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('AddResponsePipeToSurvey'),
            { authTicket: authTicket, surveyId: surveyId, itemId: itemId, pipeValue: pipeValue },
            callback,
            callbackArgs);
    };

    //Get page conditions
    this.getPageConditions = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetConditionDataForSurveyPage'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Get page branches
    this.getPageBranches = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetBranchDataForSurveyPage'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Get page logic
    this.getPageLogic = function (authTicket, surveyId, pageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetLogicForSurveyPage'),
            { authTicket: authTicket, surveyId: surveyId, pageId: pageId },
            callback,
            callbackArgs);
    };

    //Get item conditions
    this.getItemConditions = function (authTicket, surveyId, itemId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetConditionDataForSurveyItem'),
            { authTicket: authTicket, surveyId: surveyId, itemId: itemId },
            callback,
            callbackArgs);
    };

    //Search term
    this.search = function (authTicket, searchTerm, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('Search'),
            { authTicket: authTicket, searchTerm: searchTerm },
            callback,
            callbackArgs
            );
    };

    //List recent responses to survey
    this.listPeriods = function (authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ListPeriods'),
            { surveyId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //List recent responses to survey
    this.getPeriodCountForSurvey = function (authTicket, surveyId, callback) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetPeriodCountForSurvey'),
            { surveyId: surveyId, authTicket: authTicket },
            callback,
            null);
    };

    this.deletePeriod = function (authTicket, periodId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('DeletePeriod'),
            { periodId: periodId, authTicket: authTicket },
            callback,
            callbackArgs);
    }

    //Get expression left parameters by selected type
    this.getExpressionLeftParamByType = function (authTicket, responseTemplateId, dependentItemId, leftParamType, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetExpressionLeftParamByType'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, dependentItemId: dependentItemId, leftParamType: leftParamType, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Get expression left parameters by selected type and rule type
    this.getExpressionLeftParamByTypeAndRuleType = function (authTicket, responseTemplateId, dependentItemId, leftParamType, ruleType, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetExpressionLeftParamByTypeAndRuleType'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, dependentItemId: dependentItemId, leftParamType: leftParamType, ruleType: ruleType, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Get expression operators by selected left operand type and left operand
    this.getExpressionOperators = function (authTicket, responseTemplateId, leftParamType, leftParam, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetExpressionOperators'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, leftParamType: leftParamType, leftParam: leftParam, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Get expression operators by selected left operand type and left operand
    this.getExpressionRightParams = function (authTicket, responseTemplateId, leftParamType, leftParam, selectedOperator, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetExpressionRightParams'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, leftParamType: leftParamType, leftParam: leftParam, selectedOperator: selectedOperator, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Add a new expression
    this.addExpression = function (authTicket, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('AddExpression'),
            { authTicket: authTicket, ruleType: ruleType, dependentItemId: dependentItemId, dependentPageId: dependentPageId, targetPageId: targetPageId,
            responseTemplateId: responseTemplateId, rootExpressionId: rootExpressionId, expressionId: expressionId, leftParamType: leftParamType, leftParam: leftParam, selectedOperator: selectedOperator, data: data, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Remove an expression
    this.removeExpression = function (authTicket, responseTemplateId, expressionId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('RemoveExpression'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, expressionId: expressionId},
            callback,
            callbackArgs);
    };

    //Edit an new expression
    this.editExpression = function (authTicket, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('EditExpression'),
            { authTicket: authTicket, ruleType: ruleType, dependentItemId: dependentItemId, dependentPageId: dependentPageId, targetPageId: targetPageId,
            responseTemplateId: responseTemplateId, rootExpressionId: rootExpressionId, expressionId: expressionId, leftParamType: leftParamType, leftParam: leftParam, selectedOperator: selectedOperator, data: data, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    //Reorganize expressions -- change OR connectors to AND or vice versa
    this.reorganizeExpressions = function (authTicket, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, connector, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('ReorganizeExpressions'),
            { authTicket: authTicket, ruleType: ruleType, dependentItemId: dependentItemId, dependentPageId: dependentPageId, targetPageId: targetPageId,
            responseTemplateId: responseTemplateId, rootExpressionId: rootExpressionId, connector: connector},
            callback,
            callbackArgs);
    };

    //Reorganize expressions -- change OR connectors to AND or vice versa
    this.setPageBranchTargetPage = function (authTicket, responseTemplateId, ruleId, targetPageId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('SetPageBranchTargetPage'),
            { authTicket: authTicket, ruleId: ruleId, targetPageId: targetPageId, responseTemplateId: responseTemplateId },
            callback,
            callbackArgs);
    };

    //Get expression operators by selected left operand type and left operand for existing expression
    this.getExistingExpressionRightParams = function (authTicket, responseTemplateId, expressionId, maxSourceQuestionPagePosition, languageCode, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSurveyManagement.getServiceUrl('GetExistingExpressionRightParams'),
            { authTicket: authTicket, responseTemplateId: responseTemplateId, expressionId : expressionId, maxSourceQuestionPagePosition: maxSourceQuestionPagePosition, languageCode: languageCode },
            callback,
            callbackArgs);
    };

    this.addPageBreak = function(pageId, shouldPageBreak, templateId) {
        serviceHelper.makeServiceCall(
            null,
            svcSurveyManagement.getServiceUrl('AddPageBreak'),
            {pageId : pageId, shouldPageBreak : shouldPageBreak, templateId : templateId},
            null,
            null);
    }

}

