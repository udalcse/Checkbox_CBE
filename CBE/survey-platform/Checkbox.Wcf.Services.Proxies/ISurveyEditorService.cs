using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for survey editing service
    /// </summary>
    [ServiceContract]
    public interface ISurveyEditorService
    {
        #region Survey Operations
        
            //Add survey configuration options
            // - Get
            // - URLs
            // - Activation
            // - Limits
            // - Security

        /// <summary>
        /// List pipe sources for a given survey.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="maxPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <param name="customFieldTypeId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<PipeSource[]> ListPipeSources(string authToken, int surveyId, int maxPagePosition, string languageCode, int customFieldTypeId);

        /// <summary>
        /// Retrieve profile property keys alon with their ids
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="customFieldTypeId"></param>
        /// <returns></returns>S
        //ServiceOperationResult<Dictionary<int, string>> ListProfileKeys(int customFieldTypeId);

        /// <summary>
        /// Toggle the specified on/off survey setting.  Return value is new value of setting.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> ToggleSetting(string authToken, int surveyId, string settingName);

        /// <summary>
        /// Update specified survey setting to the specified value.  Return value is new value of setting.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValueAsString"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> UpdateSetting(string authToken, int surveyId, string settingName, string settingValueAsString);

        /// <summary>
        /// Set the "alternate" url for the survey
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="altUrl"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> SetAlternateUrl(string authToken, int surveyId, string altUrl);

        /// <summary>
        /// Set the style template for the survey.  A value of less than or equal to zero will mean no style template
        ///   shall be applied to the survey.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="styleTemplateId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> SetStyleTemplate(string authToken, int surveyId, int styleTemplateId, string styleType);

        /// <summary>
        /// Get all localizable texts for the survey
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<LocalizedTextValue[]> GetLocalizableTexts(string authToken, int surveyId, string languageCode);

        /// <summary>
        /// Determine whether or not rules will be changed if page deletes
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<QuestionResult> WillRulesBeChangedIfPageDeletes(string authToken, int surveyId, int pageId);

        /// <summary>
        /// Get all localizable texts for the survey
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="textKey"></param>
        /// <param name="textValue"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateSurveyText(string authToken, int surveyId, string textKey, string textValue, string languageCode);

        /// <summary>
        /// Saves survey default language
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> SetDefaultLanguage(string authToken, int surveyId, string language);

        /// <summary>
        /// Add specified language to the list of survey default languages
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> AddDefaultLanguage(string authToken, int surveyId, string language);

        /// <summary>
        /// Remove specified language from the list of survey default languages
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> RemoveDefaultLanguage(string authToken, int surveyId, string language);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetDateFormat(string authToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetTimeFormat(string authToken);

        #endregion

        #region Page Operations
            
            // - Get
            // - Add
            // - Move
            // - Delete
            // - Conditions
            // - Branching

        #endregion


        #region Item operations

        /// <summary>
        /// Get metadata for an item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<SurveyItemMetaData> GetItemMetadata(int itemId);

        /// <summary>
        /// Update metadata for an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemMetaData"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> UpdateItemMetadata(int itemId, ItemMetaData itemMetaData);

        /// <summary>
        /// Toggle item 'active' status
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> ToggleItemActiveStatus(string authToken, int surveyId, int itemId);


        /// <summary>
        /// Gets item status
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<bool> GetItemIsActive(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Check if 'copy action' is available for item
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<bool> CopyActionIsAvailableForItem(string authToken, int surveyId, int itemId);

        /// <summary>
        /// Gets survey summary status
        ///  </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string> GetStatus(string authToken, int surveyId);

        /// <summary>
        /// List autocopmplete list items
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string> ListAutocompleteListData(string authToken, int listId);

        // - Add
        // - Move
        // - Delete
        // - Conditions
        // - Branching

        #endregion

        #region Condition Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> UpdateConditionSource(string authToken, int surveyId, string value);

        #endregion
    }
}
