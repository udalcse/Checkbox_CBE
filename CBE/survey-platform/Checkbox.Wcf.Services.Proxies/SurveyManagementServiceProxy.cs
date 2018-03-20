using System;

namespace Checkbox.Wcf.Services.Proxies
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class SurveyManagementServiceProxy : System.ServiceModel.ClientBase<ISurveyManagementService>, ISurveyManagementService
    {

        public SurveyManagementServiceProxy()
        {
        }

        public SurveyManagementServiceProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public SurveyManagementServiceProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SurveyManagementServiceProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SurveyManagementServiceProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFolders(string authTicket, int parentItemId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            return Channel.ListSurveysAndFolders(authTicket, parentItemId, pageNumber, pageSize, filter, includeSurveyResponseCount);
        }

        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriod(string authTicket, int parentItemId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount)
        {
            return Channel.ListSurveysAndFoldersByPeriod(authTicket, parentItemId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount);
        }

        public ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriodByActiveStatus(string authTicket, int parentId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive)
        {
            return Channel.ListSurveysAndFoldersByPeriodByActiveStatus(authTicket, parentId, pageNumber, pageSize, filterField, filter, period, dateFieldName, includeSurveyResponseCount, includeActive, includeInactive);
        }

        public ServiceOperationResult<SurveyListItem[]> ListFavoriteSurveys(string authTicket, int parentId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            return Channel.ListSurveysAndFolders(authTicket, parentId, pageNumber, pageSize, filter, includeSurveyResponseCount);
        }

        public ServiceOperationResult<bool> AddFavoriteSurvey(string authTicket, int surveyId)
        {
            return Channel.AddFavoriteSurvey(authTicket, surveyId);
        }

        public ServiceOperationResult<bool> RemoveFavoriteSurvey(string authTicket, int surveyId)
        {
            return Channel.AddFavoriteSurvey(authTicket, surveyId);
        }

        public ServiceOperationResult<bool> IsFavoriteSurvey(string authTicket, int surveyId)
        {
            return Channel.IsFavoriteSurvey(authTicket, surveyId);
        }

        public ServiceOperationResult<PagedListResult<SurveyListItem[]>> ListAvailableSurveys(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListAvailableSurveys(authTicket, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<object> AddSurveyPage(string authToken, int responseTemplateId)
        {
            return Channel.AddSurveyPage(authToken, responseTemplateId);
        }

        public ServiceOperationResult<SurveyMetaData> GetSurveyMetaData(string authTicket, int surveyId)
        {
            return Channel.GetSurveyMetaData(authTicket, surveyId);
        }

        public ServiceOperationResult<int> CopySurvey(string authTicket, int surveyId, string surveyName)
        {
            return Channel.CopySurvey(authTicket, surveyId, surveyName);
        }

        public ServiceOperationResult<SurveyMetaData> GetSurveyInfoByGuid(string authTicket, Guid rtGuid)
        {
            return Channel.GetSurveyInfoByGuid(authTicket, rtGuid);
        }

        public ServiceOperationResult<SurveyMetaData> GetSurveyInfoByName(string authTicket, string surveyName)
        {
            return Channel.GetSurveyInfoByName(authTicket, surveyName);
        }

        public ServiceOperationResult<object> AddUserToSurveyAccessList(string authTicket, string uniqueIdentifier, int responseTemplateID, string[] permissions)
        {
            return Channel.AddUserToSurveyAccessList(authTicket, uniqueIdentifier, responseTemplateID, permissions);
        }

        public ServiceOperationResult<SurveyPageMetaData> GetSurveyPageData(string authTicket, int surveyId, int pageId)
        {
            return Channel.GetSurveyPageData(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<QuestionResult> IfAlternateUrlIsAvailable(string authTicket, string customUrl, string serverApplicationPath)
        {
            return Channel.IfAlternateUrlIsAvailable(authTicket, customUrl, serverApplicationPath);
        }

        public ServiceOperationResult<IItemMetadata> GetSurveyItemData(string authTicket, int surveyId, int itemId)
        {
            return Channel.GetSurveyItemData(authTicket, surveyId, itemId);
        }

        public ServiceOperationResult<IItemMetadata[]> GetColumnPrototypes(string authTicket, int surveyId, int itemId)
        {
            return Channel.GetColumnPrototypes(authTicket, surveyId, itemId);
        }

        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            return Channel.GetConditionDataForSurveyPage(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<RuleMetaData[]> GetBranchDataForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            return Channel.GetBranchDataForSurveyPage(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByTypeAndRuleType(string authToken, int responseTemplateId, int dependentItemId, string leftParamType, string ruleType, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.GetExpressionLeftParamByTypeAndRuleType(authToken, responseTemplateId, dependentItemId, leftParamType, ruleType, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByType(string authToken, int responseTemplateId, int dependentItemId, string leftParamType, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.GetExpressionLeftParamByType(authToken, responseTemplateId, dependentItemId, leftParamType, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<SimpleNameValueCollection> GetExpressionOperators(string authToken, int responseTemplateId, string leftParamType, string leftParam, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.GetExpressionOperators(authToken, responseTemplateId, leftParamType, leftParam, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<ExpressionRightParamData> GetExpressionRightParams(string authToken, int responseTemplateId, string leftParamType, string leftParam, string selectedOperator, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.GetExpressionRightParams(authToken, responseTemplateId, leftParamType, leftParam, selectedOperator, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<ExpressionRightParamData> GetExistingExpressionRightParams(string authToken, int responseTemplateId, int expressionId, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.GetExistingExpressionRightParams(authToken, responseTemplateId, expressionId, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<ExpressionMetaData> AddExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.AddExpression(authToken, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<ExpressionMetaData> EditExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            return Channel.EditExpression(authToken, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, expressionId, leftParamType, leftParam, selectedOperator, data, maxSourceQuestionPagePosition, languageCode);
        }

        public ServiceOperationResult<int[]> RemoveExpression(string authToken, int responseTemplateId, int expressionId)
        {
            return Channel.RemoveExpression(authToken, responseTemplateId, expressionId);
        }

        public ServiceOperationResult<object> ReorganizeExpressions(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, string connector)
        {
            return Channel.ReorganizeExpressions(authToken, ruleType, dependentItemId, dependentPageId, targetPageId, responseTemplateId, rootExpressionId, connector);
        }

        public ServiceOperationResult<object> SetPageBranchTargetPage(string authToken, int responseTemplateId, int ruleId, int targetPageId)
        {
            return Channel.SetPageBranchTargetPage(authToken, responseTemplateId, ruleId, targetPageId);
        }

        public ServiceOperationResult<PageLogic> GetLogicForSurveyPage(string authTicket, int surveyId, int pageId)
        {
            return Channel.GetLogicForSurveyPage(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyItem(string authTicket, int surveyId, int itemId)
        {
            return Channel.GetConditionDataForSurveyItem(authTicket, surveyId, itemId);
        }

        public ServiceOperationResult<IItemMetadata> GetLibraryItemData(string authTicket, int libraryId, int itemId)
        {
            return Channel.GetLibraryItemData(authTicket, libraryId, itemId);
        }

        public ServiceOperationResult<bool> SetItemLibraryOptions(string authTicket, int itemId, bool shouldShow)
        {
            return Channel.SetItemLibraryOptions(authTicket, itemId, shouldShow);
        }

        public ServiceOperationResult<PagedListResult<StyleListItem[]>> ListSurveyStyleTemplates(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListSurveyStyleTemplates(authTicket, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<PagedListResult<LibraryData[]>> ListItemLibraries(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListItemLibraries(authTicket, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<LibraryData> GetLibraryData(string authTicket, int libraryId)
        {
            return Channel.GetLibraryData(authTicket, libraryId);
        }

        public ServiceOperationResult<SurveyListItem> GetSurveyListItem(string authTicket, int listItemId, string listItemType)
        {
            return Channel.GetSurveyListItem(authTicket, listItemId, listItemType);
        }

        public ServiceOperationResult<bool> DeleteFolder(string authTicket, int folderId)
        {
            return Channel.DeleteFolder(authTicket, folderId);
        }

        public ServiceOperationResult<bool> DeleteSurvey(string authTicket, int surveyId)
        {
            return Channel.DeleteSurvey(authTicket, surveyId);
        }

        public ServiceOperationResult<object> DeleteLibraries(string authTicket, int[] libraryIds)
        {
            return Channel.DeleteLibraries(authTicket, libraryIds);
        }

        public ServiceOperationResult<object> MoveSurveyItem(string authTicket, int responseTemplateId, int itemId,
                                                       int? newPageId, int position)
        {
            return Channel.MoveSurveyItem(authTicket, responseTemplateId, itemId, newPageId, position);
        }

        public ServiceOperationResult<object> MoveSurveyPage(string authTicket, int responseTemplateId, int pageId, int position)
        {
            return Channel.MoveSurveyPage(authTicket, responseTemplateId, pageId, position);
        }

        public ServiceOperationResult<object> CopySurveyPage(string authTicket, int surveyId, int pageId)
        {
            return Channel.CopySurveyPage(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<object> DeleteSurveyPage(string authTicket, int surveyId, int pageId)
        {
            return Channel.DeleteSurveyPage(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<object> DeleteSurveyItem(string authTicket, int surveyId, int itemId)
        {
            return Channel.DeleteSurveyItem(authTicket, surveyId, itemId);
        }

        public ServiceOperationResult<object> AddResponsePipeToSurvey(string authTicket, int surveyId, int itemId, string pipeValue)
        {
            return Channel.AddResponsePipeToSurvey(authTicket, surveyId, itemId, pipeValue);
        }

        public ServiceOperationResult<GroupedResult<SurveyListItem>[]> Search(string authTicket, string searchTerm)
        {
            return Channel.Search(authTicket, searchTerm);
        }

        public ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int surveyId, int pageId)
        {
            return Channel.ListPageItemsData(authTicket, surveyId, pageId);
        }

        public ServiceOperationResult<object> AddPageBreak(int pageId, bool shouldPageBreak, int templateId)
        {
            return Channel.AddPageBreak(pageId, shouldPageBreak, templateId);
        }

        public ServiceOperationResult<object> AddItemsFromLibrary(int pageId, int itemId, int responseTemplateId, int libraryId)
        {
            return Channel.AddItemsFromLibrary(pageId, itemId, responseTemplateId, libraryId);
        }
    }
}
