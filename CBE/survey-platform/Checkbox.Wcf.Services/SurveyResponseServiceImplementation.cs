using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Security.Principal;
using Checkbox.Forms.Workflow;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Survey response service
    /// </summary>
    public static class SurveyResponseServiceImplementation
    {
        private static SurveySessionStateMachineHost WorkflowHost { get; set; }

        /// <summary>
        /// Create instance of workflow host
        /// </summary>
        static SurveyResponseServiceImplementation()
        {
            lock (typeof(SurveyResponseServiceImplementation))
            {
                WorkflowHost = new SurveySessionStateMachineHost();
                WorkflowHost.Startup();
            }
        }

        /// <summary>
        /// Perform any necessary finalization, such as unloading workflow engine during application unload.
        /// </summary>
        public static void Finalize()
        {
            WorkflowHost.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSessionId"></param>
        /// <returns></returns>
        public static Guid? GetResumeResponseSessionKey(Guid currentSessionId, object cacheContext)
        {
            return WorkflowHost.GetResumeInstanceId(currentSessionId, cacheContext);
        }

        /// <summary>
        /// Return session data by session key.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static ResponseSessionData GetResponseSessionData(Guid sessionKey, object cacheContext, Guid? rGuid = null, bool forceSet = false)
        {
            return WorkflowHost.GetSessionData(sessionKey, cacheContext, rGuid, forceSet);
        }
       
        /// <summary>
        /// List responses a user has already completed for a survey.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static SurveyResponseData[] ListResponsesForUser(Guid sessionKey, object cacheContext)
        {
            var sessionData = WorkflowHost.GetSessionData(sessionKey, cacheContext);

            if (sessionData == null || !sessionData.ResponseTemplateId.HasValue)
            {
                return new SurveyResponseData[]{};
            }

            ResponseTemplate responseTemplate = null;
            if (cacheContext != null)
            {
                var ctx = cacheContext as CacheContext;
                if (ctx != null)
                    responseTemplate = ctx.ResponseTemplate;
            }

            if (responseTemplate == null)
            {
                responseTemplate = ResponseTemplateManager.GetResponseTemplate(sessionData.ResponseTemplateId.Value);
                if (cacheContext != null)
                {
                    var ctx = cacheContext as CacheContext;
                    if (ctx != null)
                        ctx.ResponseTemplate = responseTemplate;
                }
            }

            if(responseTemplate == null)
            {
                return new SurveyResponseData[] {};
            }
            
            var respondent = Utilities.IsNotNullOrEmpty(sessionData.AuthenticatedRespondentUid)
               ? UserManager.GetUserPrincipal(sessionData.AuthenticatedRespondentUid)
               : new AnonymousRespondent(sessionData.AnonymousRespondentGuid.Value);

            if (respondent == null)
            {
                return new SurveyResponseData[]{};
            }

            return ResponseManager
                .ListResponsesForRespondent(
                    respondent,
                    sessionData.ResponseTemplateId.Value,
                    responseTemplate.BehaviorSettings.AnonymizeResponses)
                .Select(responseData => CreateSurveyResponseData(responseData, responseTemplate.ID.Value))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static bool GetMoreResponsesAllowed(Guid sessionKey, object cacheContext)
        {
            var sessionData = WorkflowHost.GetSessionData(sessionKey, cacheContext);

            if (sessionData == null || !sessionData.ResponseTemplateId.HasValue)
            {
                return false;
            }

            var ctx = cacheContext as CacheContext;
            ResponseTemplate responseTemplate = null;


            if (ctx != null)
            {
                responseTemplate = ctx.ResponseTemplate;
            }

            if (responseTemplate == null)
            {
                responseTemplate = ResponseTemplateManager.GetResponseTemplate(sessionData.ResponseTemplateId.Value);
                if (ctx != null)
                    ctx.ResponseTemplate = responseTemplate;
            }

            if (responseTemplate == null)
            {
                return false;
            }

            var respondent = Utilities.IsNotNullOrEmpty(sessionData.AuthenticatedRespondentUid)
               ? UserManager.GetUserPrincipal(sessionData.AuthenticatedRespondentUid)
               : new AnonymousRespondent(sessionData.AnonymousRespondentGuid.Value);

            if (respondent == null)
            {
                return false;
            }

            return ResponseTemplateManager.MoreResponsesAllowed(
                responseTemplate.ID.Value,
                responseTemplate.BehaviorSettings.MaxTotalResponses,
                responseTemplate.BehaviorSettings.MaxResponsesPerUser,
                respondent,
                responseTemplate.BehaviorSettings.AnonymizeResponses);
        }

        

        /// <summary>
        /// Get current state of response based on session key
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static ResponseSessionState GetCurrentSessionState(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetCurrentState(sessionKey, cacheContext);
        }


        /// <summary>
        /// Get id of template for given session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static int GetTemlpateIdForSession(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetTemplateIdForSession(sessionKey, cacheContext);
        }

        /// <summary>
        /// Get selected language for session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static string GetLanguageForSession(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetLanguageForSession(sessionKey, cacheContext);
        }

        /// <summary>
        /// Create a new response session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        public static ResponseSessionState CreateResponseSession(Guid sessionKey, ResponseSessionData sessionData, object cacheContext)
        {
            return WorkflowHost.CreateResponseSession(sessionKey, sessionData, cacheContext);
        }

        /// <summary>
        /// Initialize respondent
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="anonymousRespondentGuid"></param>
        /// <param name="invitationRecipientGuid"></param>
        /// <returns></returns>
        public static ResponseSessionState InitializeRespondent(Guid sessionKey, string userUniqueIdentifier, Guid? anonymousRespondentGuid, Guid? invitationRecipientGuid, Guid? directInvitationRecipientGuid, object cacheContext)
        {
            return WorkflowHost.SetRespondent(sessionKey, userUniqueIdentifier, anonymousRespondentGuid, invitationRecipientGuid, directInvitationRecipientGuid, cacheContext);
        }

        /// <summary>
        /// Mark that a user logged-in.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public static ResponseSessionState UserLoggedIn(Guid sessionKey, string userUniqueIdentifier, object cacheContext)
        {
            return WorkflowHost.UserLoggedIn(sessionKey, userUniqueIdentifier, cacheContext);
        }

        /// <summary>
        /// Mark that language was selected
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static ResponseSessionState LanguageSelected(Guid sessionKey, string languageCode, object cacheContext)
        {
            return WorkflowHost.LanguageSelected(sessionKey, languageCode, cacheContext);
        }


        /// <summary>
        /// Mark that password was entered.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ResponseSessionState PasswordEntered(Guid sessionKey, string password, object cacheContext)
        {
            return WorkflowHost.PasswordEntered(sessionKey, password, cacheContext);
        }

        /// <summary>
        /// Mark that new response should be created.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        /*public static ResponseSessionState CreateNewResponse(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.CreateNewResponse(sessionKey, cacheContext);
        }*/


        /// <summary>
        /// Mark that existing response selected to edit.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="responseId"></param>
        /// <returns></returns>
        public static ResponseSessionState EditResponse(Guid sessionKey, long responseId)
        {
            return WorkflowHost.EditResponse(sessionKey, responseId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static SurveyResponseData GetSurveyResponse(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetResponseData(sessionKey, cacheContext);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static SurveyResponsePage GetResponsePage(Guid sessionKey, int? pageId, object cacheContext)
        {
            return WorkflowHost.GetPageData(sessionKey, pageId, cacheContext);
        }

        /// <summary>
        /// Sets the page.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="cacheContext">The cache context.</param>
        public static void SetPage(Guid sessionKey, int? pageId, object cacheContext)
        {
            WorkflowHost.SetPage(sessionKey, pageId, cacheContext);
        }

        /// <summary>
        /// Get response pages
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static SurveyResponsePage[] GetAllSurveyResponsePages(Guid sessionKey, object cacheContext, Guid? rGuid = null)
        {
            return WorkflowHost.GetPages(sessionKey,cacheContext, rGuid).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="cacheContext"></param>
        /// <returns></returns>
        public static Dictionary<int, bool> GetVisitedPageVisibilities(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetVisitedPageVisibilities(sessionKey, cacheContext);
        }

        /// <summary>
        /// Get response item with specified id
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static ItemProxyObject[] GetItems(Guid sessionKey, IEnumerable<int> itemId, object cacheContext)
        {
            return WorkflowHost.GetItemDatas(sessionKey, itemId, cacheContext);
        }

        /// <summary>
        /// Post response item
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ResponseItemPostResult[] PostResponseItems(Guid sessionKey, SurveyResponseItem[] items, object cacheContext)
        {
            return WorkflowHost.PostResponseItemData(sessionKey, items, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static PagePostResult MoveToNextPage(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.PostResponsePage(sessionKey, null, SurveyPageAction.MoveForward, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static PagePostResult UpdateConditions(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.UpdateConditions(sessionKey, cacheContext);
        }

        /// <summary>
        /// Updates the conditions.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="cacheContext">The cache context.</param>
        /// <returns></returns>
        public static void IgnoreConditionTypes(Guid sessionKey, object cacheContext, List<string> types )
        {
             WorkflowHost.IgnoreSurveyConditionTypes(types, sessionKey, cacheContext);
        }


        /// <summary>
        /// Updates the conditions.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="cacheContext">The cache context.</param>
        /// <returns></returns>
        public static void СlearIgnoreConditionTypes(Guid sessionKey, object cacheContext)
        {
            WorkflowHost.СlearIgnoreConditionTypes(sessionKey, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static PagePostResult SaveProgress(Guid sessionKey, object cacheContext)
        {
           return WorkflowHost.PostResponsePage(sessionKey, null, SurveyPageAction.Save, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static PagePostResult MoveToPreviousPage(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.PostResponsePage(sessionKey, null, SurveyPageAction.MoveBackward, cacheContext);
        }

       

        /// <summary>
        /// Get all items for the current response page
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static ItemProxyObject[] GetAllPageItems(Guid sessionKey, int? pageId, object cacheContext)
        {
            SurveyResponsePage responsePage = GetResponsePage(sessionKey, pageId, cacheContext);

            if (responsePage == null)
            {
                return new SurveyResponseItem[] { };
            }

            return GetItems(sessionKey, responsePage.ItemIds, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static ResponseSessionState ResumeSavedResponse(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.ResumeSavedResponse(sessionKey, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseRow"></param>
        /// <returns></returns>
        private static SurveyResponseData CreateSurveyResponseData(ResponseData responseData, int responseTemplateId)
        {
            return new SurveyResponseData
            {
                ResponseId = responseData.Id,
                ResponseGuid = responseData.Guid,
                ResponseTemplateId = responseTemplateId,
                UniqueIdentifier = responseData.UserIdentifier,
                AnonymousRespondentGuid = responseData.AnonymousRespondentGuid,
                DateStarted = responseData.Started,
                DateCompleted = responseData.CompletionDate,
                DateLastEdited = responseData.LastEditDate,
                WorkflowInstanceId = responseData.WorkflowSessionId
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static PageNumberInfo GetPageNumber(Guid sessionKey, object cacheContext)
        {
            return WorkflowHost.GetPageNumber(sessionKey, cacheContext);
        }
    }
}
