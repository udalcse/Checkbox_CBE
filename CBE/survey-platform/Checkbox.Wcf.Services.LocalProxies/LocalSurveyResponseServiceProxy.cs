using System;
using System.Collections.Generic;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Wcf.Services.LocalProxies
{
    /// <summary>
    /// Non-WCF proxy for access to survey data for Checkbox survey processing that does not have
    /// WCF and serialization overhead of service-level.
    /// </summary>
    public class LocalSurveyResponseServiceProxy : ISurveyResponseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyResponseData[]> ListResponsesForRespondent(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<SurveyResponseData[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.ListResponsesForUser(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyResponseData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyResponsePage> GetCurrentResponsePage(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<SurveyResponsePage>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetResponsePage(sessionKey, null, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyResponsePage>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="cacheContext"></param>
        /// <returns></returns>
        public ServiceOperationResult<Dictionary<int, bool>> GetVisitedPageVisibilities(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<Dictionary<int, bool>>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetVisitedPageVisibilities(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<Dictionary<int, bool>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyResponsePage[]> GetAllResponsePages(Guid sessionKey, object cacheContext, Guid? rGuid = null)
        {
            try
            {
                return new ServiceOperationResult<SurveyResponsePage[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetAllSurveyResponsePages(sessionKey, cacheContext, rGuid)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyResponsePage[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sessionKey"></param>
        ///// <param name="itemId"></param>
        ///// <returns></returns>
        //public ServiceOperationResult<ItemProxyObject[]> GetItems(Guid sessionKey, int[] itemIds)
        //{
        //    try
        //    {
        //        return new ServiceOperationResult<ItemProxyObject[]>
        //        {
        //            CallSuccess = true,
        //            ResultData = SurveyResponseServiceImplementation.GetItems(sessionKey, itemIds)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "Service");

        //        return new ServiceOperationResult<ItemProxyObject[]>
        //        {
        //            CallSuccess = false,
        //            FailureExceptionType = ex.GetType().ToString(),
        //            FailureMessage = ex.Message
        //        };
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<ItemProxyObject[]> GetAllCurrentPageItems(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ItemProxyObject[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetAllPageItems(sessionKey, null, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ItemProxyObject[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="respondentUniqueIdentifier"></param>
        /// <param name="anonymousRespondentGuid"></param>
        /// <param name="invitationRecipientGuid"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionState> InitializeRespondent(Guid sessionKey, string respondentUniqueIdentifier, Guid? anonymousRespondentGuid, Guid? invitationRecipientGuid, Guid? directInvitationRecipientGuid, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.InitializeRespondent(sessionKey, respondentUniqueIdentifier, anonymousRespondentGuid, invitationRecipientGuid, directInvitationRecipientGuid, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="respondentUniqueIdentifier"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionState> SetAuthenticatedRespondent(Guid sessionKey, string respondentUniqueIdentifier, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.UserLoggedIn(sessionKey, respondentUniqueIdentifier, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="password"></param>
        public ServiceOperationResult<ResponseSessionState> SetPassword(Guid sessionKey, string password, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.PasswordEntered(sessionKey, password, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="languageCode"></param>
        public ServiceOperationResult<ResponseSessionState> SetLanguage(Guid sessionKey, string languageCode, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.LanguageSelected(sessionKey, languageCode, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="responseId"></param>
        public ServiceOperationResult<ResponseSessionState> SelectResponse(Guid sessionKey, int responseId)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.EditResponse(sessionKey, responseId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        /*public ServiceOperationResult<ResponseSessionState> CreateResponse(Guid sessionKey)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.CreateNewResponse(sessionKey)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }*/


        /// <summary>
        /// Get state of current session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionState> GetCurrentSessionState(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetCurrentSessionState(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> GetTemplateIdForSession(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetTemlpateIdForSession(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false, FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetLanguageForSession(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetLanguageForSession(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyResponseData> GetResponse(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<SurveyResponseData>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetSurveyResponse(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyResponseData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionState> CreateResponseSession(Guid sessionKey, ResponseSessionData sessionData, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.CreateResponseSession(sessionKey, sessionData, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<SurveyResponsePage> GetResponsePage(Guid sessionKey, int pageId, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<SurveyResponsePage>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetResponsePage(sessionKey, pageId, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SurveyResponsePage>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<ItemProxyObject[]> GetAllPageItems(Guid sessionKey, int pageId, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ItemProxyObject[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetAllPageItems(sessionKey, pageId, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ItemProxyObject[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sessionKey"></param>
        ///// <param name="pageId"></param>
        ///// <returns></returns>
        //public void SetPage(Guid sessionKey, int? pageId, object cacheContext)
        //{
        //    SurveyResponseServiceImplementation.SetPage(sessionKey, pageId, cacheContext);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionData> GetResponseSessionData(Guid sessionKey, object cacheContext, Guid? rGuid = null, bool forceSet = false)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionData>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetResponseSessionData(sessionKey, cacheContext, rGuid, forceSet)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagePostResult> MoveToNextPage(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.MoveToNextPage(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagePostResult> UpdateConditions(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.UpdateConditions(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagePostResult> SaveProgress(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.SaveProgress(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagePostResult> MoveToPreviousPage(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.MoveToPreviousPage(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagePostResult>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseItemPostResult[]> PostResponseItems(Guid sessionKey, SurveyResponseItem[] items, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseItemPostResult[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.PostResponseItems(sessionKey, items, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseItemPostResult[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseSessionState> ResumeSavedResponse(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.ResumeSavedResponse(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseSessionState>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public void SetPage(Guid sessionKey, int? pageId, object cacheContext)
        {
            SurveyResponseServiceImplementation.SetPage(sessionKey, pageId, cacheContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<Guid?> GetResumeSessionKey(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<Guid?>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetResumeResponseSessionKey(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<Guid?>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> GetMoreResponsesAllowed(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetMoreResponsesAllowed(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

	 /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<PageNumberInfo> GetPageNumber(Guid sessionKey, object cacheContext)
        {
            try
            {
                return new ServiceOperationResult<PageNumberInfo>
                {
                    CallSuccess = true,
                    ResultData = SurveyResponseServiceImplementation.GetPageNumber(sessionKey, cacheContext)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PageNumberInfo>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }
    }
}
