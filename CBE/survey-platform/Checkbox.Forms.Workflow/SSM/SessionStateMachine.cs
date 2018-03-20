using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Security.Principal;
using Checkbox.Forms.Workflow.RSM;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Globalization.Text;
using Prezza.Framework.Security;
using Checkbox.Forms.Security.Providers;
using Checkbox.Users;
using Checkbox.Forms.Security.Principal;
using Checkbox.Analytics;
using Prezza.Framework.Caching;

namespace Checkbox.Forms.Workflow.SSM
{
    /// <summary>
    /// Session state machine
    /// </summary>
    [Serializable]
    public class SessionStateMachine : StateMachine.StateMachine<SSMState>
    {
        #region Properties
        /// <summary>
        /// Current session data
        /// </summary>
        public ResponseSessionData ResponseSessionData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Respondent data
        /// </summary>
        public CheckboxPrincipal Respondent
        {
            get;
            internal set;
        }

        /// <summary>
        /// State machine for the response
        /// </summary>
        public ResponseStateMachine ResponseStateMachine
        {
            get;
            private set;
        }

        /// <summary>
        /// Authentication error text
        /// </summary>
        private string AuthError
        {
            get;
            set;
        }

        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public override void CleanupBeforeCaching()
        {
            if (ResponseStateMachine != null)
            {
                ResponseStateMachine.CleanupBeforeCaching();
            }
        }

        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public virtual void RestoreAfterCaching()
        {
            if (ResponseStateMachine != null)
            {
                ResponseStateMachine.RestoreAfterCaching(CacheContext);
            }
        }

        #endregion Properties

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public SessionStateMachine()
        {
            Initialize(SSMState.Initial);
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="state"></param>
        public override void Initialize(SSMState state)
        {
            base.Initialize(state);

            if (state == SSMState.HandleResponse)
            {
                createResponseStateMachine();
            }
        }
        #endregion Initialization

        #region Check Data Methods

        /// <summary>
        /// Check that the password is set
        /// </summary>
        /// <returns></returns>
        internal State<SSMState> CheckPassword()
        {
            if (passwordNeeded())
            {
                ResponseSessionData.SessionState = ResponseSessionState.EnterPassword;
                return this[SSMState.PasswordRequired];
            }
            
            return CheckAuthorization();
        }

        /// <summary>
        /// Check that the respondent has enought permissions to pass the survey
        /// </summary>
        /// <returns></returns>
        internal State<SSMState> CheckAuthorization()
        {
            var authProvider =
                 AuthorizationFactory.GetAuthorizationProvider("ResponseTemplateAuthorizationProvider") as ResponseTemplateAuthorizationProvider;

            if (authProvider == null)
            {
                throw new Exception("Authorization provider was null or was not a ResponseTemplateAuthorizationProvider.");
            }

            ResponseTemplate template = CacheContext == null || CacheContext.ResponseTemplate == null ?
                ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value) : CacheContext.ResponseTemplate;

            if (template != null && template.IsDeleted)
            {
                ResponseSessionData.SessionState = ResponseSessionState.SurveyDeleted;
                return this[SSMState.Stopped];
            }

            Respondent = string.IsNullOrEmpty(ResponseSessionData.AuthenticatedRespondentUid) ?
                new AnonymousRespondent(ResponseSessionData.AnonymousRespondentGuid.Value) :
                UserManager.GetUserPrincipal(ResponseSessionData.AuthenticatedRespondentUid);
            
            /*
            Respondent = template.BehaviorSettings.SecurityType != SecurityType.PasswordProtected &&
                         template.BehaviorSettings.SecurityType != SecurityType.Public &&
                            !string.IsNullOrEmpty(ResponseSessionData.AuthenticatedRespondentUid) 
                             ? UserManager.GetUserPrincipal(ResponseSessionData.AuthenticatedRespondentUid)
                             : new AnonymousRespondent(ResponseSessionData.AnonymousRespondentGuid.Value);
            */

            //Authorize the respondent
            var isAuthorized = authProvider.Authorize(Respondent, template, "Form.Fill", ResponseSessionData.InvitationRecipientGuid);

            //authProvider.Authorize for the Edit Response case returns ResponseTemplateAuthorizationProvider.EditOnly
            if (!isAuthorized)
            {
                AuthError = isAuthorized ? string.Empty : authProvider.GetAuthorizationErrorType();
                if (!ResponseTemplateAuthorizationProvider.EditOnly.Equals(AuthError))
                {
                    //case when an admin edits responses
                    isAuthorized = ResponseSessionData.IsEdit && authProvider.Authorize(Respondent, template, "Analysis.Responses.Edit", ResponseSessionData.InvitationRecipientGuid);
                    AuthError = isAuthorized ? string.Empty : authProvider.GetAuthorizationErrorType();
                }
            }
            else
                AuthError = "";
            
            //if failed to authorize using logged in user 
            if (!(Respondent is AnonymousRespondent) && !isAuthorized && !ResponseTemplateAuthorizationProvider.EditOnly.Equals(AuthError))
            {
                //logic allows to pass survey as Anonymous
                if (template.BehaviorSettings.SecurityType != SecurityType.PasswordProtected &&
                         template.BehaviorSettings.SecurityType != SecurityType.Public)
                {
                    var anonRespondent = new AnonymousRespondent(ResponseSessionData.AnonymousRespondentGuid.Value);
                    isAuthorized = authProvider.Authorize(anonRespondent, template, "Form.Fill", ResponseSessionData.InvitationRecipientGuid);
                    if (isAuthorized)
                    {
                        Respondent = anonRespondent;
                    }
                }
            }


            //if (template.BehaviorSettings.AnonymizeResponses)
            //    Respondent = new AnonymousRespondent(ResponseSessionData.AnonymousRespondentGuid.Value);

            if (AuthError.Equals("ResponseLimitReached", System.StringComparison.InvariantCultureIgnoreCase))
            {
                ResponseSessionData.SessionState = ResponseSessionState.ResponseLimitReached;
                return this[SSMState.Stopped];
            }

            //If authorized OR response limit reached, move on to check for resume/edit case
            if (isAuthorized 
                || AuthError == ResponseTemplateAuthorizationProvider.EditOnly 
                || AuthError == ResponseTemplateAuthorizationProvider.ResponseLimitReached)
            {
                bool anonymous = Respondent is AnonymousRespondent;

                //fill language from user attribute
                if ("User".Equals(template.LanguageSettings.LanguageSource, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (Respondent.ProfileProperties.ContainsKey(template.LanguageSettings.LanguageSourceToken))
                    {
                        string userLang = Respondent.ProfileProperties[template.LanguageSettings.LanguageSourceToken];
                        if (!string.IsNullOrEmpty(userLang) && template.LanguageSettings.SupportedLanguages.Contains(userLang))
                        {
                            ResponseSessionData.SelectedLanguage = userLang;
                        }
                    }
                }

                //set default languge if there don't exist any others
                if (string.IsNullOrEmpty(ResponseSessionData.SelectedLanguage) &&
                    template.LanguageSettings.SupportedLanguages.Count == 1)
                {
                    ResponseSessionData.SelectedLanguage = template.LanguageSettings.DefaultLanguage;
                }

                var language = ResponseSessionData.SelectedLanguage;
                if (!string.IsNullOrEmpty(language) && ResponseStateMachine != null)
                    ResponseStateMachine.UpdateLanguage(language);

                var state = CheckResponseToEdit();
                if (state == this[SSMState.Stopped] || (!anonymous && state == this[SSMState.SelectResponse]))
                    return state;

                if (ResponseStateMachine != null)
                {
                    if (string.IsNullOrEmpty(language))
                        language = ResponseStateMachine.ResponseSessionData.SelectedLanguage;

                    if (!string.IsNullOrEmpty(language))
                        ResponseStateMachine.UpdateLanguage(language);                    
                }
                ResponseSessionData.SelectedLanguage = language;

                if (string.IsNullOrEmpty(language) &&
                    template.LanguageSettings.SupportedLanguages.Count > 1)
                {
                    ResponseSessionData.SessionState = ResponseSessionState.SelectLanguage;
                    return this[SSMState.LanguageRequired];
                }

                if (!ResponseSessionData.ResponseGuid.HasValue && !ResponseSessionData.ResponseId.HasValue)
                {
                    if (anonymous || state == null)
                    {
                        //create a new response
                        return checkResponseLimitAndCreateResponse();
                    }
                }

                return state;
            }

            //If not active, we're done.
            if (AuthError == ResponseTemplateAuthorizationProvider.SurveyNotActive)
            {
                ResponseSessionData.SessionState = ResponseSessionState.SurveyNotActive;
                return this[SSMState.Stopped];
            }

            //Before start date
            if (AuthError == ResponseTemplateAuthorizationProvider.BeforeStartDate)
            {
                ResponseSessionData.SessionState = ResponseSessionState.BeforeStartDate;
                return this[SSMState.Stopped];
            }

            //After end date
            if (AuthError == ResponseTemplateAuthorizationProvider.AfterEndDate)
            {
                ResponseSessionData.SessionState = ResponseSessionState.AfterEndDate;
                return this[SSMState.Stopped];
            }

            if (AuthError == ResponseTemplateAuthorizationProvider.InvitationOnly)
            {
                ResponseSessionData.SessionState = ResponseSessionState.InvitationRequired;
                return this[SSMState.Stopped];
            }

            //Last case is login required
            ResponseSessionData.SessionState = ResponseSessionState.LoginRequired;
            return this[SSMState.Authorize];
        }




        /// <summary>
        /// Check for any responses to edit
        /// </summary>
        /// <returns></returns>
        internal State<SSMState> CheckResponseToEdit()
        {
            //If edit mode not specified or response guid not specified, do not edit.  Since editing occurs only for
            // completed responses (and therefore, workflows) editing via workflow instance id is not supported since
            // workflows are cleaned up and removed from persistent data store once completed.
            if (!ResponseSessionData.ForceNew && (!ResponseSessionData.IsEdit
                || !ResponseSessionData.ResponseGuid.HasValue))
            {
                //Set state to resume existing to cause flow through to resume check
                State<SSMState> resumeState = CheckResponseToResume();
                if (resumeState != null)
                    return resumeState;
            }

            //Check to see if user is survey admin and allowed to edit.  Otherwise, defer to template settings
            var responseTemplate = CacheContext == null || CacheContext.ResponseTemplate == null ?
                ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value) : CacheContext.ResponseTemplate;

            var isAuthorized = AuthorizationFactory.GetAuthorizationProvider().Authorize(Respondent, responseTemplate, "Form.Administer") 
                || AuthorizationFactory.GetAuthorizationProvider().Authorize(Respondent, responseTemplate, "Analysis.Responses.Edit");
            
            if (!isAuthorized)
            {
                isAuthorized = responseTemplate.BehaviorSettings.AllowEdit;
            }
            
            //If not editing, then return
            if (!isAuthorized && responseTemplate.BehaviorSettings.SecurityType != SecurityType.Public)
            {
                //ResponseSessionData.SessionState = ResponseSessionState.LoginRequired;
                //return this[SSMState.Authorize];
                return null;
            }

            //if there is something to resume -- resume
            if (ResponseSessionData.ResumeInstanceId.HasValue || ResponseSessionData.ResponseGuid.HasValue)
            {
                createResponseStateMachine(responseTemplate);
                
                if (ResponseSessionData.SessionState == ResponseSessionState.TakeSurvey)
                {
                    return this[SSMState.HandleResponse];
                }
                else
                {
                    return CheckResponseToResume();
                }
            }

            //if forcing a new response -- create a new response
            if (ResponseSessionData.ForceNew)
            {
                return checkResponseLimitAndCreateResponse();
            }

            //if editing is available and there are any completed surveys -- show the list
            if (responseTemplate.BehaviorSettings.AllowEdit)
            {
                //Check for response(s) to edit
                var responseList =
                    ResponseManager.ListResponsesForRespondent(
                        Respondent,
                        ResponseSessionData.ResponseTemplateId.Value,
                        responseTemplate.BehaviorSettings.AnonymizeResponses)
                        .Where(responseData => responseData.CompletionDate.HasValue)
                        .ToList();

                if (responseList.Count > 0)
                {
                    ResponseSessionData.SessionState = ResponseSessionState.EditResponse;
                    return this[SSMState.SelectResponse];
                }
            }

            //try to create a new response otherwise
            if (!this.AuthError.Equals("ResponseLimitReached", StringComparison.InvariantCultureIgnoreCase)
                && !this.AuthError.Equals("EditOnly", StringComparison.InvariantCultureIgnoreCase))
            {
                return createResponse();
            }
            else
            {
                ResponseSessionData.SessionState = ResponseSessionState.ResponseLimitReached;
                return this[SSMState.Stopped];
            }
        }

        /// <summary>
        /// Checks the AuthError and if the limit has not been reached -- createa new response
        /// </summary>
        /// <returns></returns>
        private State<SSMState> checkResponseLimitAndCreateResponse()
        {
            if (!this.AuthError.Equals("ResponseLimitReached", System.StringComparison.InvariantCultureIgnoreCase)
                &&
                !this.AuthError.Equals("EditOnly", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return createResponse();
            }
            else
            {
                ResponseSessionData.SessionState = ResponseSessionState.ResponseLimitReached;
                return this[SSMState.Stopped];
            }
        }

        /// <summary>
        /// Check for response to resume
        /// </summary>
        /// <returns></returns>
        internal State<SSMState> CheckResponseToResume()
        {
            //First check to see if resume is allowed, otherwise move on to edit step
            var template = CacheContext == null ? null : CacheContext.ResponseTemplate;

            if (template == null)
            {
                template = ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value);
                if (CacheContext != null)
                {
                    CacheContext.ResponseTemplate = template;
                }
            }

            //If resume not allowed, create a new response
            if (!template.BehaviorSettings.AllowContinue)
            {
                return null;
            }

            //If resuming/editing via response guid, load that response directly.
            if (ResponseSessionData.ResponseGuid.HasValue)
            {
                ResponseStateMachine = new ResponseStateMachine(template, ResponseSessionData, Respondent);
                ResponseStateMachine.InitializeByResponseGuid(true, true);
                return this[SSMState.HandleResponse];
            }

            //Check for response(s) to resume
            var responseList =
                ResponseManager.ListResponsesForRespondent(
                    Respondent,
                    ResponseSessionData.ResponseTemplateId.Value,
                    template.BehaviorSettings.AnonymizeResponses)
                    .ToList();

            var incompleteResponses = responseList.Where(responseData => !responseData.CompletionDate.HasValue).ToList();

            int incomplete = incompleteResponses.Count;
            int complete = responseList.Count - incomplete;

            if (incomplete == 0 || (complete > 0 && template.BehaviorSettings.AllowEdit))
            {                
                return null; //nothing to resume
            }

            //If resume is allowed and only a single response to resume, figure out whether we
            // need to resume an existing workflow (response started in 5.x) or use this workflow to 
            // continue the response (started pre-5.x)
            if (incompleteResponses.Count > 0)
            {
                //save the response data 
                ResponseSessionData.ResponseId = incompleteResponses[0].Id;
                ResponseSessionData.ResponseGuid = incompleteResponses[0].Guid;
                //create Response State Machine
                ResponseStateMachine = new ResponseStateMachine(template, ResponseSessionData, Respondent);
                //resume the survey if no responses are allowed
                ResponseStateMachine.InitializeByResponseGuid(true, true);
                if ((template.BehaviorSettings.MaxResponsesPerUser.HasValue && template.BehaviorSettings.MaxResponsesPerUser == 1) || !template.BehaviorSettings.AllowEdit || Respondent is AnonymousRespondent)
                {
                    //start handling the survey if no responses are allowed
                    ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
                    return this[SSMState.HandleResponse];
                }                 
            }

            ResponseSessionData.SessionState = ResponseSessionState.EditResponse;
            return this[SSMState.SelectResponse];
        }

        #endregion Check Data Methods

        #region Private Methods
        /// <summary>
        /// Create a state machine for the response
        /// </summary>
        /// <param name="responseTemplate"></param>
        private void createResponseStateMachine(ResponseTemplate responseTemplate = null)
        {
            if (responseTemplate == null)
            {
                if (ResponseSessionData.ResponseTemplateId != null)
                {
                    responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value);
                }
                if (responseTemplate == null)
                    throw new Exception("Unable to get ResponseTemplate!");
            }

            ResponseStateMachine = new ResponseStateMachine(responseTemplate, ResponseSessionData, Respondent);

            bool isResponseIdSet = ResponseSessionData.ResponseId.HasValue;
            bool isEdit = ResponseSessionData.IsEdit;
            ResponseStateMachine.InitializeByResponseGuid(isResponseIdSet && !isEdit, isResponseIdSet || isEdit);
        }

        /// <summary>
        /// Create a new response
        /// </summary>
        /// <returns></returns>
        private State<SSMState> createResponse()
        {
            ResponseTemplate template = null;

            if (CacheContext != null)
                template = CacheContext.ResponseTemplate;

            if (template == null)
            {
                template = ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value);
                if (CacheContext != null && CacheContext.ResponseTemplate == null)
                    CacheContext.ResponseTemplate = template;
            }

            ResponseStateMachine = new ResponseStateMachine(template, ResponseSessionData, Respondent);
            ResponseStateMachine.InitializeAsNew();
            ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;

            return this[SSMState.HandleResponse];
        }

        /// <summary>
        /// Check if the password is needed
        /// </summary>
        /// <returns></returns>
        private bool passwordNeeded()
        {
            ResponseTemplate template = ResponseStateMachine == null ? null : ResponseStateMachine.ResponseTemplate;

            if (template == null && CacheContext != null)
                template = CacheContext.ResponseTemplate;

            if (template == null)
            {
                template = ResponseTemplateManager.GetResponseTemplate(ResponseSessionData.ResponseTemplateId.Value);
                if (CacheContext != null && CacheContext.ResponseTemplate == null)
                    CacheContext.ResponseTemplate = template;
            }

            return (template.BehaviorSettings.SecurityType == SecurityType.PasswordProtected &&
                   !template.BehaviorSettings.Password.Equals(ResponseSessionData.EnteredPassword));
        }

        /// <summary>
        /// Checks that the current survey has given security type
        /// </summary>
        public bool SurveyIs(SecurityType type)
        {
            if (ResponseSessionData == null || this.ResponseSessionData.ResponseTemplateId == null)
                return false;

            //try to get the survey from response state machine
            ResponseTemplate survey = ResponseStateMachine == null ? null : ResponseStateMachine.ResponseTemplate;

            if (survey == null && CacheContext != null)
                survey = CacheContext.ResponseTemplate;
   
            if (survey == null)
            {
                survey = ResponseTemplateManager.GetResponseTemplate(this.ResponseSessionData.ResponseTemplateId.Value);

                if (CacheContext != null && CacheContext.ResponseTemplate == null)
                    CacheContext.ResponseTemplate = survey;

                return (survey.BehaviorSettings.SecurityType == type);
            }

            return false;
        }
        #endregion Private Methods


        #region Caching
        public void Cache(CacheManager cacheManager)
        {
            cacheManager.Add(ResponseSessionData.SessionGuid.ToString(), _currentState);

            if (ResponseStateMachine != null)
            {
                ResponseStateMachine.Cache(cacheManager);
            }
            else
            {
                cacheManager.Remove("Response_" + ResponseSessionData.SessionGuid.ToString());
            }

            cacheManager.Add("ResponseSessionData_" + ResponseSessionData.SessionGuid.ToString(), ResponseSessionData);

            cacheManager.Add("Respondent_" + ResponseSessionData.SessionGuid.ToString(), Respondent);

            cacheManager.Add("ResponseTemplateID_" + ResponseSessionData.SessionGuid.ToString(), ResponseSessionData.ResponseTemplateId);
        }

        public static SessionStateMachine GetFromCache(CacheManager cacheManager, Guid key, CacheContext ctx)
        {
            SessionStateMachine ssm = new SessionStateMachine();
            ssm.CacheContext = ctx;
            ssm._currentState = (State<SSMState>)cacheManager.GetData(key.ToString());
            ssm.ResponseSessionData = cacheManager.GetData("ResponseSessionData_" + key.ToString()) as ResponseSessionData;
            ssm.Respondent = cacheManager.GetData("Respondent_" + key.ToString()) as CheckboxPrincipal;
            if (ssm.ResponseSessionData != null &&
                (ssm.ResponseSessionData.SessionState == ResponseSessionState.TakeSurvey ||
                ssm.ResponseSessionData.SessionState == ResponseSessionState.Completed ||
                ssm.ResponseSessionData.SessionState == ResponseSessionState.SavedProgress))
            {
                int rtID = (int)cacheManager.GetData("ResponseTemplateID_" + key.ToString());
                ResponseTemplate rt = ctx == null ? null : ctx.ResponseTemplate;
                if (rt == null)
                {
                    rt = ResponseTemplateManager.GetResponseTemplate(rtID);
                    if (rt != null && ctx != null)
                    {
                        ctx.ResponseTemplate = rt;
                    }
                }
                ssm.ResponseStateMachine = ResponseStateMachine.GetFromCache(cacheManager, key, rt, ssm.ResponseSessionData, ssm.Respondent);
            }
            
            return ssm;
        }

        public CacheContext CacheContext { get; set; }
        #endregion Caching

    }
}
