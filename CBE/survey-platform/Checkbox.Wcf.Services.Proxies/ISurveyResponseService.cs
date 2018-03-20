using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    // NOTE: If you change the interface name "ISurveyResponseService" here, you must also update the reference to "ISurveyResponseService" in Web.config.

    [ServiceKnownType(typeof(MatrixAdditionalData))]
    //[ServiceKnownType(typeof(UploadItemAdditionalData))]
    [ServiceKnownType(typeof(SurveyResponseItem))]
    [ServiceKnownType(typeof(ReportItemInstanceData))]
    [ServiceContract]
    public interface ISurveyResponseService
    {
        #region Pre-Response Methods

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> InitializeRespondent(Guid sessionKey, string respondentUniqueIdentifier, Guid? anonymousRespondentGuid, Guid? invitationRecipientGuid, Guid? directInvitationRecipientGuid, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> SetAuthenticatedRespondent(Guid sessionKey, string respondentUniqueIdentifier, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> SetPassword(Guid sessionKey, string password, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> SetLanguage(Guid sessionKey, string languageCode, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> SelectResponse(Guid sessionKey, int responseId);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> ResumeSavedResponse(Guid sessionKey, object cacheContext);

       
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SetPage(Guid sessionKey, int? pageId, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<Guid?> GetResumeSessionKey(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyResponseData[]> ListResponsesForRespondent(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> GetMoreResponsesAllowed(Guid sessionKey, object cacheContext);

        #endregion

        #region Get Data

        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetLanguageForSession(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseSessionState> GetCurrentSessionState(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetTemplateIdForSession(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseSessionState> CreateResponseSession(Guid sessionKey, ResponseSessionData sessionData, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<SurveyResponseData> GetResponse(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyResponsePage> GetCurrentResponsePage(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyResponsePage> GetResponsePage(Guid sessionKey, int pageId, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<SurveyResponsePage[]> GetAllResponsePages(Guid sessionKey, object cacheContext, Guid? rGuid = null);

        //[OperationContract]
        //[WebGet]
        //ServiceOperationResult<ItemProxyObject[]> GetItems(Guid sessionKey, int[] itemIds);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<ItemProxyObject[]> GetAllCurrentPageItems(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<ItemProxyObject[]> GetAllPageItems(Guid sessionKey, int pageId, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<PageNumberInfo> GetPageNumber(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseSessionData> GetResponseSessionData(Guid sessionKey, object cacheContext, Guid? rGuid = null, bool forceSet = false);

        [OperationContract]
        [WebGet]
        ServiceOperationResult<Dictionary<int, bool>> GetVisitedPageVisibilities(Guid sessionKey, object cacheContext);
        #endregion

        #region Post Data

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<PagePostResult> MoveToNextPage(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<PagePostResult> SaveProgress(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<PagePostResult> MoveToPreviousPage(Guid sessionKey, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<ResponseItemPostResult[]> PostResponseItems(Guid sessionKey, SurveyResponseItem[] item, object cacheContext);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<PagePostResult> UpdateConditions(Guid sessionKey, object cacheContext);
        #endregion


    }
}
