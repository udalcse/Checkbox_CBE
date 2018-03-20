using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for survey management API, which allows basic survey and folder
    /// management functionality, including listing surveys, moving surveys.
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(SurveyItemMetaData))]
    [ServiceKnownType(typeof(ItemMetaData))]
    public interface ISurveyManagementApi
    {
        /// <summary>
        /// List accessible surveys and folders that are children of the specified parent folder.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentFolderId">The id of the parent folder.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount">Determines if survey response count are included in the retrieved data.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="SurveyListItem"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the child surveys and folders.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFolders(string authToken, int parentFolderId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount);

        /// <summary>
        /// List accessible surveys and folders that are children of the specified parent folder.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentFolderId">The id of the parent folder.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField">"Name" or empty string for all searchable fields filtering</param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount">Determines if survey response count are included in the retrieved data.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="SurveyListItem"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the child surveys and folders.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriod(string authToken, int parentFolderId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount);

        /// <summary>
        /// List accessible surveys and folders that are children of the specified parent folder, which can be filtered by IsActive status
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="parentFolderId">The id of the parent folder.</param>
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
        ServiceOperationResult<SurveyListItem[]> ListSurveysAndFoldersByPeriodByActiveStatus(string authToken, int parentFolderId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive);

        /// <summary>
        /// Retrieve a paged, sorted and filtered list of surveys.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="SurveyListItem"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of surveys that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<SurveyListItem[]>> ListAvailableSurveys(string authToken,
                                                                                        int pageNumber,
                                                                                        int pageSize,
                                                                                        string sortField,
                                                                                        bool sortAscending,
                                                                                        string filterField,
                                                                                        string filterValue);

        /// <summary>
        /// Retrieve a <see cref="SurveyListItem"/> object for a survey or folder.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="listItemId">The id of the survey or folder.</param>
        /// <param name="listItemType">Allowed values are "survey" or "folder"</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="SurveyListItem"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyListItem> GetSurveyListItem(string authToken, int listItemId, string listItemType);

        /// <summary>
        /// Retrieve a survey's metadata.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="SurveyMetaData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyMetaData> GetSurveyMetaData(string authToken, int surveyId);


        /// <summary>
        /// Retrieve the metadata associated with a specific survey page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="SurveyPageMetaData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyPageMetaData> GetSurveyPageData(string authToken, int surveyId, int pageId);

        /// <summary>
        /// Retrieve the metadata associated with a specific survey item.
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
        ServiceOperationResult<IItemMetadata> GetSurveyItemData(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Retrieve the condition logic for a survey page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="RuleMetaData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyPage(string authToken, int surveyId, int pageId);

        /// <summary>
        /// Retrieve the branch logic for a survey page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="RuleMetaData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData[]> GetBranchDataForSurveyPage(string authToken, int surveyId, int pageId);

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
        /// Edits an expression
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
        /// Get page logic for page
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PageLogic> GetLogicForSurveyPage(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Retrieve the condition logic for an item.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="RuleMetaData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RuleMetaData> GetConditionDataForSurveyItem(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Retrieve the metadata associated with a specific item in a library.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryId">The id of the library.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="IItemMetadata"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata> GetLibraryItemData(string authToken, int libraryId, int itemId);

        /// <summary>
        /// Retrieve the metadata associated with a specific item in a library.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryId">The id of the library.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="IItemMetadata"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> SetItemLibraryOptions(string authTicket, int itemId, bool shouldShow);

        /// <summary>
        /// Get a paged, sorted and filtered list of accessible survey styles.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="StyleListItem"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of survey styles that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<StyleListItem[]>> ListSurveyStyleTemplates(string authToken,
                                                                                            int pageNumber,
                                                                                            int pageSize,
                                                                                            string sortField,
                                                                                            bool sortAscending,
                                                                                            string filterField,
                                                                                            string filterValue);


        /// <summary>
        /// Get a paged, sorted and filtered list of accessible libraries.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="LibraryData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of libraries that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<LibraryData[]>> ListLibraries(string authToken,
                                                                                int pageNumber,
                                                                                int pageSize,
                                                                                string sortField,
                                                                                bool sortAscending,
                                                                                string filterField,
                                                                                string filterValue);

        /// <summary>
        /// Retrieve a library by id.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryId">The id of the library.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="LibraryData"/>.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<LibraryData> GetLibrary(string authToken, int libraryId);


        /// <summary>
        /// Delete a folder.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="folderId">The id of the folder.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="bool"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property indicates whether or not the action completed successfully or not.</para>
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteFolder(string authToken, int folderId);

        /// <summary>
        /// Delete a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="bool"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property indicates whether or not the action completed successfully or not.</para>
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteSurvey(string authToken, int surveyId);

        /// <summary>
        /// Delete one or more library.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="libraryIds">The list of library ids to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteLibraries(string authToken, int[] libraryIds);

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
        /// Copy a survey page. The new page is placed directly after the source page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> CopySurveyPage(string authToken, int surveyId, int pageId);

        /// <summary>
        /// Delete a survey page.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyPage(string authToken, int surveyId, int pageId);

        /// <summary>
        /// Delete a survey item.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteSurveyItem(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Move survey item
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <param name="newPageId">new page id</param>
        /// <param name="position">new position</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MoveSurveyItem(string authToken, int responseTemplateId, int itemId,
                                                      int? newPageId, int position);


        /// <summary>
        /// Move survey page
        /// </summary>
        /// <param name="authTicket">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="pageId">page id</param>
        /// <param name="position">new position</param>
        /// <returns></returns>
        ServiceOperationResult<object> MoveSurveyPage(string authTicket, int responseTemplateId, int pageId,
                                                      int position);

        /// <summary>
        /// Search for surveys and folders.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="searchTerm">The value to search for.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="GroupedResult{T}"/> of <see cref="SurveyListItem"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of surveys and folders that match the search criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<SurveyListItem>[]> Search(string authToken, string searchTerm);
    }
}
