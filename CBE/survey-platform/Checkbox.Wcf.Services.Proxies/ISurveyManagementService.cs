using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for survey management service, which allows basic survey and folder
    /// management functionality, including listing surveys, moving surveys, etc.
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(SurveyItemMetaData))]
    [ServiceKnownType(typeof(ItemMetaData))]
    public interface ISurveyManagementService
    {
        /// <summary>
        /// List all surveys and folders that are children of the specified parent
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFolders(string authTicket, int parentId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount);

        /// <summary>
        /// List all surveys and folders that are children of the specified parent
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField"></param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriod(string authTicket, int parentId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount);

        /// <summary>
        /// List accessible surveys and folders that are children of the specified parent folder, which can be filtered by IsActive status
        /// </summary>
        /// <param name="authTicket">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentId">The id of the parent folder.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField">"Name" or empty string for all searchable fields filtering</param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount">Determines if survey response count are included in the retrieved data.</param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="SurveyListItem"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the child surveys and folders.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriodByActiveStatus(string authTicket, int parentId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive);

        /// <summary>
        /// List of favorite surveys 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="parentId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListFavoriteSurveys(string authTicket, int parentId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount);

        /// <summary>
        /// Add survey to list of favorites
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<Boolean> AddFavoriteSurvey(string authTicket, int surveyId);

        /// <summary>
        /// Remove survey from list of favorites
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<Boolean> RemoveFavoriteSurvey(string authTicket, int surveyId);

        /// <summary>
        /// Check if specified survey is favorite
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<Boolean> IsFavoriteSurvey(string authTicket, int surveyId);

        /// <summary>
        /// List all available surveys for the user
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<SurveyListItem[]>> ListAvailableSurveys(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get simple list item associated with a single survey or folder.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="listItemId"></param>
        /// <param name="listItemType">Allowed values are "survey" or "folder"</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem> GetSurveyListItem(string authTicket, int listItemId, string listItemType);

        /// <summary>
        /// Get metadata associated with a survey.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyMetaData> GetSurveyMetaData(string authTicket, int surveyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId">ID of survey to copy</param>
        /// <param name="surveyName">New survey's name</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> CopySurvey(string authTicket, int surveyId, string surveyName);


            /// <summary>
        /// Get metadata associated with a survey by GUID.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="rtGuid"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyMetaData> GetSurveyInfoByGuid(string authTicket, Guid rtGuid);

        /// <summary>
        /// Get metadata associated with a survey by Name.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyMetaData> GetSurveyInfoByName(string authTicket, string surveyName);

        /// <summary>
        /// Add a user to a survey's access list with the specified permissions.
        /// </summary>
        /// <param name="authTicket">Authentication ticket.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user.</param>
        /// <param name="responseTemplateID">ID of the survey to add a user to.</param>
        /// <param name="permissions">Permissions to apply.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserToSurveyAccessList(string authTicket, string uniqueIdentifier, int responseTemplateID, string[] permissions);

        /// <summary>
        /// Get metadata associated with a survey page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyPageMetaData> GetSurveyPageData(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Indicate if the alternate url is available or not.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="customUrl"></param>
        /// <param name="serverApplicationPath"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<QuestionResult> IfAlternateUrlIsAvailable(string authTicket, string customUrl, string serverApplicationPath);

        /// <summary>
        /// Get metadata associated with a survey item.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata> GetSurveyItemData(string authTicket, int surveyId, int itemId);

        /// <summary>
        /// Retrieve the prototype items for the matrix
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="itemId">The id if the item.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="IItemMetadata"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata[]> GetColumnPrototypes(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Get data for a page condition.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyPage(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Get data for page branches.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData[]> GetBranchDataForSurveyPage(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Retrieve available left parameters for the selected expression type
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="dependentItemId">The id of item where condition will be applied.</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="ruleType">Rule parameter type: see RuleType enum for more details</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByTypeAndRuleType(string authToken, int responseTemplateId, int dependentItemId, string leftParamType, string ruleType, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Retrieve available left parameters for the selected expression type
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="dependentItemId">The id of item where condition will be applied.</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> GetExpressionLeftParamByType(string authToken, int responseTemplateId, int dependentItemId, string leftParamType, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Retrieve available operators for expression by selected left operand 
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="leftParam">Left parameter</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> GetExpressionOperators(string authToken, int responseTemplateId, string leftParamType, string leftParam, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Retrieve available right operand options and type for expression by selected left operand type, left operand and operator
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="leftParam">Left parameter</param>
        /// <param name="selectedOperator">Operator</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ExpressionRightParamData> GetExpressionRightParams(string authToken, int responseTemplateId, string leftParamType, string leftParam, string selectedOperator, int maxSourceQuestionPagePosition, string languageCode);


        /// <summary>
        /// Retrieve available right operand options and type for expression by selected left operand type, left operand and operator
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="expressionId">Expression ID</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ExpressionRightParamData> GetExistingExpressionRightParams(string authToken, int responseTemplateId, int expressionId, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Adds a new expression
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="ruleType">Rule type.</param>
        /// <param name="dependentItemId">Item Id which depends on this rule</param>
        /// <param name="dependentPageId">Page Id which depends on this rule</param>
        /// <param name="targetPageId">Target page ID where to jump if rule type is Branching.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="rootExpressionId">Expression ID of the root expression in the rule</param>
        /// <param name="expressionId">Parent expression ID</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="leftParam">Left parameter</param>
        /// <param name="selectedOperator">Operator</param>
        /// <param name="data">Right parameter data</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ExpressionMetaData> AddExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Edit an expression
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="ruleType">Rule type.</param>
        /// <param name="dependentItemId">Item Id which depends on this rule</param>
        /// <param name="dependentPageId">Page Id which depends on this rule</param>
        /// <param name="targetPageId">Target page ID where to jump if rule type is Branching.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="rootExpressionId">Expression ID of the root expression in the rule</param>
        /// <param name="expressionId">Parent expression ID</param>
        /// <param name="leftParamType">Parameter type: see ExpressionSourceType enum for more details</param>
        /// <param name="leftParam">Left parameter</param>
        /// <param name="selectedOperator">Operator</param>
        /// <param name="data">Right parameter data</param>
        /// <param name="maxSourceQuestionPagePosition">Maximal position for the source question</param>
        /// <param name="languageCode">Language</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ExpressionMetaData> EditExpression(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode);

        /// <summary>
        /// Deletes an expression
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="expressionId">Expression ID</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int[]> RemoveExpression(string authToken, int responseTemplateId, int expressionId);

        /// <summary>
        /// Change OR connectors to AND or vice versa
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="ruleType">Rule type.</param>
        /// <param name="dependentItemId">Item Id which depends on this rule</param>
        /// <param name="dependentPageId">Page Id which depends on this rule</param>
        /// <param name="targetPageId">Target page ID where to jump if rule type is Branching.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="rootExpressionId">Expression ID of the root expression in the rule</param>
        /// <param name="connector">OR or AND</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> ReorganizeExpressions(string authToken, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, string connector);

        /// <summary>
        ///  Set target page for the given rule
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> SetPageBranchTargetPage(string authToken, int responseTemplateId, int ruleId, int targetPageId);

        /// <summary>
        /// Get logic for page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PageLogic> GetLogicForSurveyPage(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Get data for survey item condition.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyItem(string authTicket, int surveyId, int itemId);

        /// <summary>
        /// Get metadata associated with a library item.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata> GetLibraryItemData(string authTicket, int libraryId, int itemId);

        /// <summary>
        /// Get metadata associated with a library item.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> SetItemLibraryOptions(string authTicket, int itemId, bool shouldShow);

        /// <summary>
        /// List survey style templates available to view and/or edit for the specified user.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<StyleListItem[]>> ListSurveyStyleTemplates(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> AddItemsFromLibrary(int pageId, int itemId, int responseTemplateId, int libraryId);

        /// <summary>
        /// List libraries available to user
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<LibraryData[]>> ListItemLibraries(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get data for a library
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<LibraryData> GetLibraryData(string authTicket, int libraryId);


        /// <summary>
        /// Delete the specified folder
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteFolder(string authTicket, int folderId);

        /// <summary>
        /// Delete the specified survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteSurvey(string authTicket, int surveyId);

        /// <summary>
        /// Delete the specified libraries
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="libraryIds"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteLibraries(string authTicket, int[] libraryIds);


        /// <summary>
        /// Change survey item position
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="itemId"></param>
        /// <param name="newPageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MoveSurveyItem(string authToken, int responseTemplateId, int itemId,
                                                      int? newPageId, int position);

        /// <summary>
        /// Add new survey page
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddSurveyPage(string authToken, int responseTemplateId);

        /// <summary>
        /// Change survey page position
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MoveSurveyPage(string authToken, int responseTemplateId, int pageId, int position);

        /// <summary>
        /// Copy the specific survey page. The new page will follow after the source page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> CopySurveyPage(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Delete the specific survey page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyPage(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Delete the specific survey item
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyItem(string authTicket, int surveyId, int itemId);

        /// <summary>
        /// Add response pipe to survey
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="pipeValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddResponsePipeToSurvey(string authTicket, int surveyId, int itemId, string pipeValue);

        /// <summary>
        /// Search for surveys and folders.  Return value is a set of grouped results matching various parameters.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<SurveyListItem>[]> Search(string authTicket, string searchTerm);


        /// <summary>
        /// Returns a set of items metadata for the specific survey page.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int surveyId, int pageId);

        /// <summary>
        /// Inserts a page break after given page
        /// </summary>
        /// <param name="pageId">Page id after which page break will be inserted</param>
        /// <param name="templateId">Template id that the pageId belongs to in order to mark template as updated</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> AddPageBreak(int pageId, bool shouldPageBreak, int templateId);
    }
}
