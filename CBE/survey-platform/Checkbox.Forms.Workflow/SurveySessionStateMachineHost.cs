using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Forms.Workflow.RSM;
using Checkbox.Forms.Workflow.SSM;
using Checkbox.Forms.Items;
using Checkbox.Analytics;
using System.Data;
using Prezza.Framework.Caching;

namespace Checkbox.Forms.Workflow
{
    /// <summary>
    /// Manager that controls state machines for every response
    /// </summary>
    public class SurveySessionStateMachineHost
    {
        /// <summary>
        /// Initialize workflow & start up
        /// </summary>
        public void Startup()
        {
            _absentSessionsCache = CacheFactory.GetCacheManager("absentSessions");
            _sessionStateMachinesCache = CacheFactory.GetCacheManager("stateMachines");
            _sessionStateMachinesByResponseCache = CacheFactory.GetCacheManager("stateMachinesByResponse");
        }

        /// <summary>
        /// Shut down workflow
        /// </summary>
        public void Shutdown()
        {
        }

        #region State Machines Handling Methods
        /// <summary>
        /// Get the state machine instance associated with a session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        /*protected SessionStateMachine GetStateMachine(Guid sessionKey)
        {
            try
            {
                if (!_sessionStateMachinesCache.Contains(sessionKey.ToString()))
                {
                    if (_absentSessionsCache.Contains(sessionKey.ToString()))
                        return null;

                    Guid? responseGuid = ResponseManager.GetResponseGuidBySessionGuid(sessionKey);
                    if (responseGuid == null)
                    {
                        _absentSessionsCache.Add(sessionKey.ToString(), true);
                        return null;
                    }


                    ResponseTemplate template = ResponseTemplateManager.GetResponseTemplateFromResponseGUID(responseGuid.Value);
                    if (template == null)
                        throw new Exception(string.Format("Survey for response {0} has no been found.", responseGuid.Value));

                    ResponseData response = ResponseManager.GetResponseData(responseGuid.Value);
                    ResponseSessionData sessionData = new ResponseSessionData()
                    {
                        AnonymousRespondentGuid = response.AnonymousRespondentGuid,
                        AuthenticatedRespondentUid = response.UserIdentifier,
                        Invitee = response.Invitee,
                        SelectedLanguage = response.ResponseLanguage,
                        ResponseGuid = response.Guid,
                        ResponseId = response.Id,
                        ResponseTemplateId = template.ID,
                        ResponseTemplateGuid = template.GUID,
                        RespondentIpAddress = response.RespondentIp,
                        SessionGuid = sessionKey,
                        SessionState = ResponseSessionState.TakeSurvey
                    };

                    SessionStateMachine ssm = CreateSessionStateMachine(sessionKey, sessionData);
                    ssm.Initialize(SSMState.HandleResponse);
                    return ssm;

                }
                var res = SessionStateMachine.GetFromCache(_sessionStateMachinesCache, sessionKey);
                res.RestoreAfterCaching();
                return res;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }*/

        /// <summary>
        /// Create a state machine and put it to the cache
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionData"></param>
        public SessionStateMachine CreateSessionStateMachine(Guid sessionKey, ResponseSessionData sessionData, object cacheContext)
        {
            try
            {
                if (AbsentSessionsCache.Contains(sessionKey.ToString()))
                {
                    if (AbsentSessionsCache.Contains(sessionKey.ToString()))
                    {
                        AbsentSessionsCache.Remove(sessionKey.ToString());
                    }
                }

                if (SessionStateMachinesCache.Contains(sessionKey.ToString()))
                    return SessionStateMachine.GetFromCache(SessionStateMachinesCache, sessionKey, cacheContext as CacheContext);
                SessionStateMachine ssm = new SessionStateMachine() { ResponseSessionData = sessionData, CacheContext = cacheContext as CacheContext };
                ssm.Cache(SessionStateMachinesCache);
                return ssm;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        #endregion State Machines Handling Methods

        #region Caches
        private static CacheManager _absentSessionsCache;
        private static CacheManager _sessionStateMachinesCache;
        private static CacheManager _sessionStateMachinesByResponseCache;

        private static CacheManager AbsentSessionsCache
        {
            get { return _absentSessionsCache ?? (_absentSessionsCache = CacheFactory.GetCacheManager("absentSessions")); }
        }

        private static CacheManager SessionStateMachinesCache
        {
            get { return _sessionStateMachinesCache ?? (_sessionStateMachinesCache = CacheFactory.GetCacheManager("stateMachines")); }
        }

        private static CacheManager SessionStateMachinesByResponseCache
        {
            get { return _sessionStateMachinesByResponseCache ?? (_sessionStateMachinesByResponseCache = CacheFactory.GetCacheManager("stateMachinesByResponse")); }
        }

        /*

        static readonly Dictionary<Guid, bool> _absentSessions = new Dictionary<Guid, bool>();
        static readonly object _absentSessionsLockObject = new object();

        static readonly Dictionary<Guid, SessionStateMachine> _sessionStateMachines = new Dictionary<Guid, SessionStateMachine>();
        static readonly object _sessionStateMachinesLockObject = new object();

        static readonly Dictionary<Guid, SessionStateMachine> _sessionStateMachinesByResponse = new Dictionary<Guid, SessionStateMachine>();
        static readonly object _sessionStateMachinesByResponseLockObject = new object();

        static readonly Dictionary<Guid, DateTime> _sessionActivity = new Dictionary<Guid, DateTime>();
        static readonly object _sessionActivityLockObject = new object();
        */
        static DateTime _lastCacheCleanup;
        /// <summary>
        /// Update caches
        /// </summary>
        /// <param name="ssm"></param>
        /// <param name="sessionKey"></param>
        private void UpdateCache(SessionStateMachine ssm, Guid sessionKey)
        {
            try
            {
                //Add session machine to cache by response GUID
                if (ssm.ResponseStateMachine != null && ssm.ResponseStateMachine.Response != null && ssm.ResponseStateMachine.Response.GUID.HasValue
                    && !SessionStateMachinesByResponseCache.Contains(ssm.ResponseStateMachine.Response.GUID.Value.ToString()))
                {
                    if (!SessionStateMachinesByResponseCache.Contains(ssm.ResponseStateMachine.Response.GUID.Value.ToString()))
                    {
                        SessionStateMachinesByResponseCache.Add(ssm.ResponseStateMachine.Response.GUID.Value.ToString(), sessionKey.ToString());
                    }
                }

                //update session activity
                //_sessionActivityCache.Add(sessionKey.ToString(), DateTime.Now);

                //clear old sessions
                /*
                if (_lastCacheCleanup < DateTime.Now.AddMinutes(-2))
                {
                    Guid[] oldSessions = null;
                    lock (_sessionActivityLockObject)
                    {
                        oldSessions = (from k in _sessionActivity.Keys where _sessionActivity[k] < DateTime.Now.AddMinutes(-2) select k).ToArray();
                    }

                    if (oldSessions != null && oldSessions.Length > 0)
                    {
                        foreach (Guid k in oldSessions)
                            RemoveFromCaches(k);
                    }

                    _lastCacheCleanup = DateTime.Now;
                }*/
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }
        /// <summary>
        /// Removes session from caches
        /// </summary>
        /// <param name="sessionKey"></param>
        private void RemoveFromCaches(Guid sessionKey)
        {
            try
            {
                Guid? responseGuid = null;

                SessionStateMachine ssm = SessionStateMachinesCache[sessionKey.ToString()] as SessionStateMachine;
                if (ssm == null)
                    return;

                if (ssm.ResponseStateMachine != null && ssm.ResponseStateMachine.Response != null && ssm.ResponseStateMachine.Response.GUID.HasValue)
                    responseGuid = ssm.ResponseStateMachine.Response.GUID.Value;

                if (responseGuid.HasValue)
                {
                    if (SessionStateMachinesByResponseCache.Contains(responseGuid.Value.ToString()))
                        SessionStateMachinesByResponseCache.Remove(responseGuid.Value.ToString());
                }

                if (SessionStateMachinesCache.Contains(sessionKey.ToString()))
                {
                    SessionStateMachinesCache.Remove(sessionKey.ToString());
                }

                if (AbsentSessionsCache.Contains(sessionKey.ToString()))
                {
                    AbsentSessionsCache.Remove(sessionKey.ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        #endregion Caches

        #region Workflow Data/Processing


        /// <summary>
        /// Get state of session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ResponseSessionState GetCurrentState(Guid sessionKey, object cacheContext)
        {
            try
            {
                SessionStateMachine ssm = GetStateMachine(sessionKey, cacheContext);
                //Return NONE state if no workflow found
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                ResponseSessionData sessionData = ssm.ResponseSessionData;

                //Return state
                return sessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public Guid? GetResumeInstanceId(Guid sessionKey, object cacheContext)
        {
            try
            {
                SessionStateMachine ssm = GetStateMachine(sessionKey, cacheContext);
                return ssm == null ? null : (ssm.ResponseSessionData == null ? null : ssm.ResponseSessionData.ResumeInstanceId);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Get state of session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public int GetTemplateIdForSession(Guid sessionKey, object cacheContext)
        {
            try
            {
                SessionStateMachine ssm = GetStateMachine(sessionKey, cacheContext);
                //Check for a loaded workflow
                if (ssm == null)
                {
                    //Ask response manager directly since session is possibly unloaded
                    var surveyId = Checkbox.Analytics.ResponseManager.GetSurveyIdFromWorkflowSessionGuid(sessionKey);

                    return surveyId.HasValue ? surveyId.Value : -1;
                }

                ResponseSessionData sessionData = ssm.ResponseSessionData;

                return sessionData != null && sessionData.ResponseTemplateId.HasValue
                           ? sessionData.ResponseTemplateId.Value
                           : -1;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Get selected language for session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public string GetLanguageForSession(Guid sessionKey, object cacheContext)
        {
            try
            {
                SessionStateMachine ssm = GetStateMachine(sessionKey, cacheContext);
                //Check for a loaded workflow
                if (ssm != null && ssm.ResponseSessionData != null)
                    return ssm.ResponseSessionData.SelectedLanguage;

                return string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        public ResponseSessionState CreateResponseSession(Guid sessionKey, ResponseSessionData sessionData, object cacheContext)
        {
            try
            {
                //Ensure session key in data matches input
                sessionData.SessionGuid = sessionKey;

                //Ensure workflow does not already exist.  If it does, return it.
                var ssm = GetStateMachine(sessionKey, cacheContext);

                if (ssm != null)
                {
                    return ssm.ResponseSessionData.SessionState;
                }

                //Check the existing session machine in the cache
                if (sessionData.ResponseGuid.HasValue)
                {
                    bool sessionByResponseWasFound = false;
                    bool needToClearCache = false;

                    ssm = SessionStateMachinesByResponseCache[sessionData.ResponseGuid.Value.ToString()] as SessionStateMachine;
                    if (ssm == null || ssm.ResponseStateMachine == null)
                    {
                        RemoveFromCaches(sessionKey);
                    }
                    else
                    {
                        if (sessionData.ResumeInstanceId.HasValue)
                        {
                            ssm.PerformAction(SSMAction.ResumeSavedResponse, sessionData);
                        }
                        else
                        {
                            ssm.PerformAction(SSMAction.EditResponse, sessionData);
                        }
                        //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                        ssm.Cache(SessionStateMachinesCache);

                        return ssm.ResponseSessionData.SessionState;
                    }
                }

                //Create a state machine instance
                ssm = CreateSessionStateMachine(sessionKey, sessionData, cacheContext);

                //Run state machine
                ssm.PerformAction(SSMAction.CreateSession, sessionData);

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);
                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="anonymousRespondentGuid"></param>
        /// <param name="invitationRecipientGuid"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ResponseSessionState SetRespondent(Guid sessionKey, string userUniqueIdentifier, Guid? anonymousRespondentGuid, Guid? invitationRecipientGuid, Guid? directInvitationRecipientGuid, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                //Run state machine
                ssm.PerformAction(SSMAction.SetRespondent, new ResponseSessionData
                                {
                                    SessionGuid = sessionKey,
                                    AuthenticatedRespondentUid = userUniqueIdentifier,
                                    AnonymousRespondentGuid = anonymousRespondentGuid,
                                    InvitationRecipientGuid = invitationRecipientGuid,
                                    DirectInvitationRecipientGuid = directInvitationRecipientGuid
                });

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Mark that a user logged-in.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public ResponseSessionState UserLoggedIn(Guid sessionKey, string userUniqueIdentifier, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                //Run state machine
                ssm.PerformAction(SSMAction.LogUserIn, userUniqueIdentifier);

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Mark that language was selected.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ResponseSessionState LanguageSelected(Guid sessionKey, string languageCode, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                //Run state machine
                ssm.PerformAction(SSMAction.SetResponseLanguage, languageCode);

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Mark that password was entered
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResponseSessionState PasswordEntered(Guid sessionKey, string password, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                //Run state machine
                ssm.PerformAction(SSMAction.SetPassword, password);

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Creates a new response. Is used in the response import feature that was not implemented yet.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        /*public ResponseSessionState CreateNewResponse(Guid sessionKey, object cacheContext)
        {
#warning Unused method: public ResponseSessionState CreateNewResponse(Guid sessionKey)
            throw new Exception("Not implemented yet.");
            uncomment this code. may be that works, but I can not test it -- no calls were found.
            var ssm = GetStateMachine(sessionKey);

            //no machine in the cache
            if (ssm == null)
            {
                return ResponseSessionState.None;
            }

            //Run state machine
            ssm.PerformAction(SSMAction.CreateNewResponse, null);

            //Now re-get the state
            return GetCurrentState(sessionKey);
        }*/


        /// <summary>
        /// Mark that an existing response was selected.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="responseId"></param>
        /// <returns></returns>
        public ResponseSessionState EditResponse(Guid sessionKey, long responseId)
        {
#warning Unused method: public ResponseSessionState EditResponse(Guid sessionKey, long responseId)
            throw new Exception("Not implemented yet.");
            /*  uncomment this code. may be that works, but I can not test it -- no calls were found.
            var ssm = GetStateMachine(sessionKey);

            //no machine in the cache
            if (ssm == null)
            {
                return ResponseSessionState.None;
            }

            //Run state machine
            ssm.PerformAction(SSMAction.SelectResponse, responseId);

            //Now re-get the state
            return GetCurrentState(sessionKey);
             */
        }

        /// <summary>
        /// Return the status of the response
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SurveyResponseData GetResponseData(Guid sessionKey, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return null;
                }

                return ssm.ResponseStateMachine == null ? null : ssm.ResponseStateMachine.Response.GetDataTransferObject();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Return the page data. If the pageId is NULL this method returns the current page
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SurveyResponsePage GetPageData(Guid sessionKey, int? pageId, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return null;
                }

                return ssm.ResponseStateMachine == null ? null : ssm.ResponseStateMachine.GetPage(pageId);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

 
        public void SetPage(Guid sessionKey, int? pageId, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                ssm.ResponseStateMachine.SetPage(pageId.Value);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }



        public List<SurveyResponsePage> GetPages(Guid sessionKey, object cacheContext, Guid? rGuid = null)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext, rGuid);

                //no machine in the cache
                if (ssm == null)
                {
                    return null;
                }

                return ssm.ResponseStateMachine == null ? null : ssm.ResponseStateMachine.GetPages();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="cacheContext"></param>
        /// <returns></returns>
        public Dictionary<int, bool> GetVisitedPageVisibilities(Guid sessionKey, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return null;
                }

                return ssm.ResponseStateMachine == null ? null : ssm.ResponseStateMachine.GetVisitedPageVisibilities();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Returns items' data for all item ids in the given list
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        public ItemProxyObject[] GetItemDatas(Guid sessionKey, IEnumerable<int> itemIds, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return null;
                }

                return ssm.ResponseStateMachine == null ? null : ssm.ResponseStateMachine.GetItemDatas(itemIds);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }


        /// <summary>
        /// Post users answers to database
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="responseItems"></param>
        /// <returns></returns>
        public ResponseItemPostResult[] PostResponseItemData(Guid sessionKey, SurveyResponseItem[] responseItems, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null || ssm.ResponseStateMachine == null)
                {
                    return null;
                }

                //Ensure item is part of current page.  
                //TODO: Should out of band postings be allowed?

                //Do nothing if no response or events are not the correct type
                if (ssm.ResponseStateMachine.Response == null
                    || ssm.ResponseStateMachine.Response.CurrentPage == null)
                {
                    return null;
                }

                //Do nothing if no survey item posted.
                if (responseItems.Length == 0)
                {
                    return null;
                }

                foreach (var postedItem in responseItems)
                {
                    //Ensure item is on current page and part of response
                    Item responseItem = ssm.ResponseStateMachine.Response.GetItem(postedItem.ItemId);

                    if (responseItem == null)
                    {
                        return null;
                    }

                    //If item is not on page, return
                    if (!ssm.ResponseStateMachine.Response.CurrentPage.ContainsItem(postedItem.ItemId))
                    {
                        return null;
                    }

                    //Otherwise, update the item
                    responseItem.UpdateFromDataTransferObject(postedItem);
                }

                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                //Get the item back now that processing has completed
                var updatedItems = GetItemDatas(sessionKey, responseItems.Select(item => item.ItemId), cacheContext);

                return
                    updatedItems
                        .Where(item => item is SurveyResponseItem)
                        .Select(item =>
                                    new ResponseItemPostResult(
                                        item.ItemId,
                                        ((SurveyResponseItem)item).IsValid,
                                        ((SurveyResponseItem)item).ValidationErrors)).ToArray();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Post response page. This method performs navigation from one page to another
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="responsePage"></param>
        /// <param name="pageAction"></param>
        /// <returns></returns>
        public PagePostResult PostResponsePage(Guid sessionKey, SurveyResponsePage responsePage, SurveyPageAction pageAction, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null || ssm.ResponseStateMachine == null)
                {
                    return null;
                }

                ssm.PerformAction(SSMAction.PostPage, pageAction);

                //_sessionStateMachinesCache.Remove(sessionKey.ToString());
                ssm.Cache(SessionStateMachinesCache);

                //Now get updated page
                SurveyResponsePage updatedPage = responsePage == null || responsePage.PageId <= 0
                    ? GetPageData(sessionKey, null, cacheContext)
                    : GetPageData(sessionKey, responsePage.PageId, cacheContext);

                if (updatedPage == null)
                {
                    return null;
                }

                UpdateCache(ssm, sessionKey);

                return new PagePostResult(
                    updatedPage.PageId,
                    updatedPage.IsValid,
                    updatedPage.ValidationErrors,
                    updatedPage.HasSPC,
                    GetCurrentState(sessionKey, cacheContext));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Update Current Page Conditions
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="cacheContext"> </param>
        /// <returns></returns>
        public PagePostResult UpdateConditions(Guid sessionKey, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null || ssm.ResponseStateMachine == null)
                {
                    return null;
                }

                ssm.PerformAction(SSMAction.PostPage, SurveyPageAction.UpdateConditions);

                //Now get the current page
                SurveyResponsePage currentPage = GetPageData(sessionKey, null, cacheContext);
                
                if (currentPage == null)
                {
                    return null;
                }

                UpdateCache(ssm, sessionKey);

                return new PagePostResult(
                    currentPage.PageId,
                    currentPage.IsValid,
                    currentPage.ValidationErrors,
                    currentPage.HasSPC,
                    GetCurrentState(sessionKey, cacheContext));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Resume saved response. The response may be exist in the cache of session machines and can be created from the response data in the database
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ResponseSessionState ResumeSavedResponse(Guid sessionKey, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                //no machine in the cache
                if (ssm == null)
                {
                    return ResponseSessionState.None;
                }

                ssm.PerformAction(SSMAction.ResumeSavedResponse, ssm.ResponseSessionData);

                //update current page conditions
                if (ssm.ResponseStateMachine != null && ssm.ResponseStateMachine.Response != null)
                {
                    ssm.ResponseStateMachine.Response.UpdateCurrentPageConditions();
                }

                //replace machine in the cache
                //_sessionStateMachinesCache.Remove(sessionKey.ToString());

                ssm.Cache(SessionStateMachinesCache);

                return ssm.ResponseSessionData.SessionState;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }


        /// <summary>
        /// Get session data by firing event handled by workflow and responded to in kind by another
        /// event.  Proper functioning requires synchronous access via manual scheduler.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ResponseSessionData GetSessionData(Guid sessionKey, object cacheContext, Guid? rGuid = null, bool forceSet = false)
        {
            try
            {
                ResponseSessionData sessionData = null;

                var ssm = GetStateMachine(sessionKey, cacheContext, rGuid, forceSet);

                //no machine in the cache
                if (ssm == null)
                {
                    return new ResponseSessionData() { SessionState = ResponseSessionState.None };
                }
                else
                {
                    sessionData = ssm.ResponseSessionData;
                }

                //Return
                return sessionData;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        public void IgnoreSurveyConditionTypes(List<string> conditionTypes, Guid sessionKey, object cacheContext)
        {
            var sm = GetStateMachine(sessionKey, cacheContext);

            if (sm != null)
                sm.ResponseStateMachine.Response.AddIgnoreCondtionTypes(conditionTypes);
        }

        public void СlearIgnoreConditionTypes( Guid sessionKey, object cacheContext)
        {
            var sm = GetStateMachine(sessionKey, cacheContext);

            if (sm != null)
                sm.ResponseStateMachine.Response.СlearIgnoreConditionTypes();
        }

        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="cacheContext"></param>
        /// <returns></returns>
        private SessionStateMachine GetStateMachine(Guid sessionKey, object cacheContext, Guid? rGuid = null, bool forceSet = false)
        {
            var ctx = cacheContext as CacheContext;
            //retrieve from the context
            if (ctx != null && !forceSet)
            {
                if (ctx.Storage.ContainsKey("SessionStateMachine"))
                    return ctx.Storage["SessionStateMachine"] as SessionStateMachine;
            }

            try
            {
                SessionStateMachine ssm = null;
                if (!SessionStateMachinesCache.Contains(sessionKey.ToString()) || forceSet)
                {
                    if (AbsentSessionsCache.Contains(sessionKey.ToString()))
                        return null;

                    Guid? responseGuid = rGuid == null ? ResponseManager.GetResponseGuidBySessionGuid(sessionKey) : rGuid;
                    if (responseGuid == null)
                    {
                        AbsentSessionsCache.Add(sessionKey.ToString(), true);
                        return null;
                    }

                    ResponseTemplate template = ResponseTemplateManager.GetResponseTemplateFromResponseGUID(responseGuid.Value);
                    if (template == null)
                        throw new Exception(string.Format("Survey for response {0} has no been found.", responseGuid.Value));

                    ResponseData response = ResponseManager.GetResponseData(responseGuid.Value);
                    ResponseSessionData sessionData = new ResponseSessionData()
                    {
                        AnonymousRespondentGuid = response.AnonymousRespondentGuid,
                        AuthenticatedRespondentUid = response.UserIdentifier,
                        Invitee = response.Invitee,
                        SelectedLanguage = response.ResponseLanguage,
                        ResponseGuid = response.Guid,
                        ResponseId = response.Id,
                        ResponseTemplateId = template.ID,
                        ResponseTemplateGuid = template.GUID,
                        RespondentIpAddress = response.RespondentIp,
                        SessionGuid = sessionKey,
                        SessionState = ResponseSessionState.TakeSurvey
                    };

                    if (SessionStateMachinesCache.Contains(sessionKey.ToString()) && forceSet)
                    {
                        SessionStateMachinesCache.Remove(sessionKey.ToString());
                    }

                    ssm = CreateSessionStateMachine(sessionKey, sessionData, cacheContext);
                    ssm.Initialize(SSMState.HandleResponse);

                    //save to the context
                    if (ctx != null)
                    {
                        ctx.Storage.Add("SessionStateMachine", ssm);
                    }

                    ssm.Cache(SessionStateMachinesCache);

                    return ssm;

                }
                ssm = SessionStateMachine.GetFromCache(SessionStateMachinesCache, sessionKey, ctx);
                ssm.RestoreAfterCaching();

                //save to the context
                if (ctx != null)
                {
                    ctx.Storage.Add("SessionStateMachine", ssm);
                }

                return ssm;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }

        /// <summary>
        /// Return current page number
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public PageNumberInfo GetPageNumber(Guid sessionKey, object cacheContext)
        {
            try
            {
                var ssm = GetStateMachine(sessionKey, cacheContext);

                if (ssm == null || ssm.ResponseStateMachine == null)
                    return null;

                return ssm.ResponseStateMachine.GetPageNumber();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw;
            }
        }
        #endregion
    }
}
