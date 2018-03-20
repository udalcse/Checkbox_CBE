//using System;
//using System.Data;
//using System.Collections.Generic;
//using Checkbox.Security.Principal;
//using Checkbox.Users;
//using Checkbox.Common;
//using Checkbox.Invitations;
//using Checkbox.Globalization.Text;
//using Checkbox.Forms.Security.Principal;
//using Checkbox.Forms.Security.Providers;
//using Prezza.Framework.Common;
//using Prezza.Framework.Security;
//using Prezza.Framework.ExceptionHandling;
//using Prezza.Framework.Security.Principal;

namespace Checkbox.Forms
{
    /// <summary>
    /// Controller for handling flow of a response.
    /// </summary>
    public class ResponseController
    {
    }
}
//    {
//        #region Private Fields

//        /// <summary>
//        /// Session information for the response
//        /// </summary>
//        private IResponseSession _responseSession;

//        /// <summary>
//        /// Display flags
//        /// </summary>
//        private ResponseViewDisplayFlags _staticDisplayFlags;

//        /// <summary>
//        /// List of completed responses
//        /// </summary>
//        private DataTable _completedResponseList;

//        /// <summary>
//        /// Get the lang
//        /// </summary>
//        private Dictionary<string, string> _supportedLanguages;

//        #endregion

//        /// <summary>
//        /// Create a response controller.  Set initial session state to "none"
//        /// </summary>
//        public ResponseController()
//        {
//            SessionState = ResponseSessionState.None;
//        }

//        /// <summary>
//        /// Get the current response session state
//        /// </summary>
//        public ResponseSessionState SessionState { get; private set; }

//        /// <summary>
//        /// Get the response
//        /// </summary>
//        public Response Response { get; private set; }

//        /// <summary>
//        /// Get whether a validation alert should be displayed when a response page is not
//        /// validated on MoveNext.
//        /// </summary>
//        public bool EnablePageValidationAlert { get; private set; }

//        /// <summary>
//        /// Get the default language of the survey
//        /// </summary>
//        public string SurveyDefaultLanguage { get; private set; }

//        /// <summary>
//        /// Get display flags for the repsonse view, based on survey configuration
//        /// </summary>
//        public ResponseViewDisplayFlags DisplayFlags
//        {
//            get
//            {
//                //Modify the display flags for current response state
//                if (SessionState == ResponseSessionState.EditResponse && !_responseLimitReached)
//                {
//                    return ResponseViewDisplayFlags.CreateNewButton;
//                }

//                if (SessionState != ResponseSessionState.TakeSurvey)
//                {
//                    return ResponseViewDisplayFlags.None;
//                }

//                ResponseViewDisplayFlags flags = _staticDisplayFlags;

//                if (Response != null && Response.StateIsValid)
//                {
//                    if (!Response.CanNavigateForward)
//                    {
//                        flags &= ~ResponseViewDisplayFlags.ProgressBar;
//                        flags &= ~ResponseViewDisplayFlags.PageNumbers;
//                        flags &= ~ResponseViewDisplayFlags.SaveButton;
//                    }

//                    if (Response.CanNavigateForward && !Response.CompleteOnNext)
//                    {
//                        flags |= ResponseViewDisplayFlags.NextButton;
//                    }

//                    if (Response.CanNavigateForward && Response.CompleteOnNext)
//                    {
//                        flags |= ResponseViewDisplayFlags.FinishButton;
//                    }

//                    if (Response.CanNavigateBack)
//                    {
//                        flags |= ResponseViewDisplayFlags.BackButton;
//                    }
//                }

//                return flags;
//            }
//        }

//        /// <summary>
//        /// Get the style template id
//        /// </summary>
//        public int? StyleTemplateId { get; private set; }

//        /// <summary>
//        /// Get the list of supported survey languages
//        /// </summary>
//        public Dictionary<string, string> SupportedLanguages
//        {
//            get
//            {
//                if (_supportedLanguages == null)
//                {
//                    _supportedLanguages = new Dictionary<string, string>();
//                }

//                return _supportedLanguages;
//            }
//        }

//        /// <summary>
//        /// Indicator of whether response limit reached message was returned by auth provider
//        /// </summary>
//        private bool _responseLimitReached;

//        /// <summary>
//        /// Create a new response controller.  Returns a boolean indicating if the 
//        /// initialization was successful.  If not, the message param is populated
//        /// with more detailed information.
//        /// </summary>
//        public bool Initialize(IResponseSession responseSession, out string message)
//        {
//            bool success = false;

//            try
//            {
//                ArgumentValidation.CheckForNullReference(responseSession, "Response Session");

//                _responseSession = responseSession;

//                //Initialize the response object
//                success = InitializeResponse(out message);
//            }
//            catch (Exception ex)
//            {
//                ExceptionPolicy.HandleException(ex, "BusinessProtected");

//                SessionState = ResponseSessionState.Error;
//                message = ex.Message;
//            }

//            return success;
//        }

//        /// <summary>
//        /// Move to the next page.  Return a boolean indicating if the page was advanced.  If not,
//        /// that could indicate a validation error.
//        /// </summary>
//        public bool MoveNext()
//        {
//            //Check if navigation is allowed, to handle the case where  browser back button is used to take the
//            // survey back.
//            if (!IsNavigationAllowed())
//            {
//                //If we got here, we're trying to submit a page to an already-completed survey
//                // but without being in edit mode.  Just show completion page in that event.
//                //Go directly to completion page w/out firing events such as MoveNext(),
//                // move Previous(), OnLoad()
//                Response.GoToCompletionPage();
//                return true;
//            }

//            //Get the id of the current page
//            int? currentPageId = Response.CurrentPage != null ? Response.CurrentPage.PageId : (int?)null;

//            //Move next
//            Response.MoveNext();

//            //Store the new current page id in the response session
//            if (Response.CurrentPage != null)
//            {
//                _responseSession.CurrentPageId = Response.CurrentPage.PageId;
//            }
//            else
//            {
//                _responseSession.CurrentPageId = null;
//            }

//            //Now return a boolean if the current page didn't advance and is not valid
//            if (Response.CurrentPage == null)
//            {
//                //If current page is null, just return false
//                return true;
//            }

//            //Otherwise, return true if page is valid or page advanced
//            return (currentPageId != _responseSession.CurrentPageId) || Response.CurrentPage.Valid;
//        }

//        /// <summary>
//        /// Move to the previous page
//        /// </summary>
//        public void MovePrevious()
//        {
//            if (!IsNavigationAllowed())
//            {
//                //If we got here, we're trying to submit a page to an already-completed survey
//                // but without being in edit mode.  Just show completion page in that event.
//                //Go directly to completion page w/out firing events such as MoveNext(),
//                // move Previous(), OnLoad().
//                Response.GoToCompletionPage();
//                return;
//            }

//            Response.MovePrevious();

//            if (Response.CurrentPage != null)
//            {
//                _responseSession.CurrentPageId = Response.CurrentPage.PageId;
//            }
//            else
//            {
//                _responseSession.CurrentPageId = null;
//            }
//        }
        

//        /// <summary>
//        /// Check to see if navigation is still allowed.  For completed responses, navigation is only
//        /// allowed when in edit mode.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsNavigationAllowed()
//        {
//            //Navigation is allowed when the response is not complete
//            // or it is being edited.
//            return !Response.Completed || _responseSession.AdminEditMode || _responseSession.RespondentEditMode;
//        }

//        /// <summary>
//        /// Finish the survey
//        /// </summary>
//        public void Finish()
//        {
//            if (!IsNavigationAllowed())
//            {
//                //If we got here, we're trying to submit a page to an already-completed survey
//                // but without being in edit mode.  Just show completion page in that event.
//                //Go directly to completion page w/out firing events such as MoveNext(),
//                // move Previous(), OnLoad().
//                Response.GoToCompletionPage();
//                return;
//            }

//            Response.MoveNext();

//            if (Response.CurrentPage != null)
//            {
//                _responseSession.CurrentPageId = Response.CurrentPage.PageId;
//            }
//            else
//            {
//                _responseSession.CurrentPageId = null;
//            }
//        }

//        /// <summary>
//        /// Go to a specific page id in the response.
//        /// </summary>
//        /// <param name="pageId">Index of response page to move to.</param>
//        public void GoToPage(int pageId)
//        {
//            if (Response != null)
//            {
//                Response.MoveToPage(pageId);
//            }
//        }

//        /// <summary>
//        /// Initialize the response.  Return a boolean where false indicates auth failure or some other situation
//        /// preventing starting the survey immediately.
//        /// </summary>
//        private bool InitializeResponse(out string message)
//        {
//            //Load the survey and create a response
//            ResponseTemplate rt = LoadResponseTemplate();
//            SurveyDefaultLanguage = rt.LanguageSettings.DefaultLanguage;

//            //Set style template id
//            StyleTemplateId = rt.StyleSettings.StyleTemplateId;
//            EnablePageValidationAlert = rt.StyleSettings.ShowValidationErrorAlert;

//            if (_responseSession.ForceNew && !_responseSession.IsFormPost)
//            {
//                //Clear persisted values
//                _responseSession.ClearPersistedValues();
//            }

//            ExtendedPrincipal respondent = GetRespondent();

//            //Authorize. The output parameter contains specific details
//            // that can be displayed in the UI
//            if (!AuthorizeRespondent(rt, respondent, out message))
//            {
//                return false;
//            }

//            //Get list of supported survey languages
//            foreach (string supportedLanguage in rt.LanguageSettings.SupportedLanguages)
//            {
//                //Add the language and localized text to the supported languages dictionary.
//                // Use the GetText overload that allows specifying default values and alternate languages
//                // to use when no text for the specified language is found
//                SupportedLanguages[supportedLanguage] = TextManager.GetText(
//                    "/languageText/" + supportedLanguage,
//                    supportedLanguage,
//                    supportedLanguage,
//                    rt.LanguageSettings.SupportedLanguages.ToArray());
//            }

//            //If forcing new, initialize the new response here so it is avialable later
//            if (_responseSession.ForceNew && !_responseSession.IsFormPost)
//            {
//                Response = rt.CreateResponse(rt.LanguageSettings.DefaultLanguage);

//                //Nothing to edit, so initialize a new response
//                Response.Initialize(
//                    _responseSession.RespondentIPAddress,
//                    _responseSession.ServerUserContext,
//                    rt.LanguageSettings.DefaultLanguage,
//                    _responseSession.IsTest,
//                    _responseSession.Respondent);

//                _responseSession.ResponseGuid = Response.GUID;
//            }

//            //No language set yet, so ask the user if necessary
//            string languageCode = GetLanguageCode(rt);

//            if (Utilities.IsNullOrEmpty(languageCode))
//            {
//                SessionState = ResponseSessionState.SelectLanguage;
//                return false;
//            }

//            //Password required
//            if (!CheckPassword(rt))
//            {
//                SessionState = ResponseSessionState.EnterPassword;
//                return false;
//            }

//            //Create a response, if necessary
//            if (Response == null)
//            {
//                Response = rt.CreateResponse(languageCode);

//                //Get the response state
//                ResponseState responseState = GetResponseState(rt);

//                //If there is no current response, figure out whether to show the "edit" screen
//                // or to just jump into a new response
//                if (responseState == null)
//                {
//                    //If respondent edit is allowed, load the list of responses, otherwise
//                    // just create a new one.
//                    if (rt.BehaviorSettings.AllowEdit)
//                    {
//                        _completedResponseList = ResponseStateManager.GetCompletedResponseList(_responseSession.Respondent, rt.ID.Value);

//                        //If no more responses allowed, or list of completed responses has some values, set a flag
//                        // and return.
//                        if (_responseLimitReached ||
//                            (_completedResponseList != null
//                            && _completedResponseList.Select().Length > 0))
//                        {
//                            SessionState = ResponseSessionState.EditResponse;

//                            return false;
//                        }
//                    }

//                    //Nothing to edit and new responses are allowed, so initialize a new response
//                    Response.Initialize(
//                        _responseSession.RespondentIPAddress,
//                        _responseSession.ServerUserContext,
//                        languageCode,
//                        _responseSession.IsTest,
//                        _responseSession.Respondent);

//                    _responseSession.ResponseGuid = Response.GUID;
//                }
//                else
//                {
//                    //Restore the response state
//                    Response.Restore(responseState);

//                    //If editing the survey, move to the start in cases of non-postback or if posting back
//                    // from selecting language or entering passoword.
//                    if (!_responseSession.IsFormPost
//                        || SessionState == ResponseSessionState.SelectLanguage
//                        || SessionState == ResponseSessionState.EnterPassword)
//                    {
//                        //Move to start if editing
//                        if (_responseSession.AdminEditMode || _responseSession.RespondentEditMode)
//                        {
//                            Response.MoveToStart();
//                        }
//                        else
//                        {
//                            //Re-Run rules so that conditions have been executed by calling
//                            // load current page
//                            Response.LoadCurrentPage();
//                        }
//                    }
//                }
//            }

//            //Set language code
//            Response.LanguageCode = languageCode;

//            //Bind completed event
//            Response.ResponseCompleted += _response_ResponseCompleted;

//            SetStaticDisplayFlags(rt);

//            //Set state of response session
//            SessionState = ResponseSessionState.TakeSurvey;
//            return true;
//        }

//        /// <summary>
//        /// Handle completion of response
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void _response_ResponseCompleted(object sender, ResponseStateEventArgs e)
//        {
//            //Unbind Event
//            Response.ResponseCompleted -= _response_ResponseCompleted;

//            //Track invitation completed
//            if (Response != null && _responseSession.RecipientGuid.HasValue)
//            {
//                long? recipientId = InvitationManager.GetRecipientId(_responseSession.RecipientGuid.Value);

//                if (recipientId.HasValue)
//                {
//                    Invitation.RecordResponse(recipientId.Value, Response.ID.Value);
//                }
//            }

//            //Clear persisted values
//            _responseSession.ClearPersistedValues();
//        }

//        /// <summary>
//        /// Set the display flags that apply to all pages.
//        /// </summary>
//        /// <param name="rt"></param>
//        private void SetStaticDisplayFlags(ResponseTemplate rt)
//        {
//            _staticDisplayFlags = ResponseViewDisplayFlags.None;

//            if (rt.StyleSettings.ShowItemNumbers)
//            {
//                _staticDisplayFlags |= ResponseViewDisplayFlags.ItemNumbers;
//            }

//            if (rt.StyleSettings.ShowPageNumbers)
//            {
//                _staticDisplayFlags |= ResponseViewDisplayFlags.PageNumbers;
//            }

//            if (rt.StyleSettings.ShowProgressBar)
//            {
//                _staticDisplayFlags |= ResponseViewDisplayFlags.ProgressBar;
//            }

//            if (rt.BehaviorSettings.ShowSaveAndQuit)
//            {
//                _staticDisplayFlags |= ResponseViewDisplayFlags.SaveButton;
//            }

//            if (rt.StyleSettings.ShowTitle)
//            {
//                _staticDisplayFlags |= ResponseViewDisplayFlags.Title;
//            }
//        }

//        /// <summary>
//        /// Load the response template
//        /// </summary>
//        private ResponseTemplate LoadResponseTemplate()
//        {
//            ResponseTemplate rt = null;

//            if (_responseSession.SurveyGuid.HasValue)
//            {
//                rt = ResponseTemplateManager.GetResponseTemplate(_responseSession.SurveyGuid.Value);
//            }
//            else if (_responseSession.RecipientGuid.HasValue)
//            {
//                _responseSession.SurveyGuid = InvitationManager.GetResponseTemplateGuidForInvitation(_responseSession.RecipientGuid.Value);

//                if (_responseSession.SurveyGuid.HasValue)
//                {
//                    rt = ResponseTemplateManager.GetResponseTemplate(_responseSession.SurveyGuid.Value);
//                }
//            }

//            if (rt == null)
//            {
//                throw new Exception(TextManager.GetText("/controlText/responseController/unableToLoadSurvey", TextManager.DefaultLanguage, "Unable to load a survey.  The survey or invitation id was not present in the URL or refers to a deleted survey or invitation."));
//            }

//            return rt;
//        }

//        /// <summary>
//        /// Get the current respondent
//        /// </summary>
//        /// <returns></returns>
//        private CheckboxPrincipal GetRespondent()
//        {
//            CheckboxPrincipal principal = null;

//            //Check for logged-in user
//            if (_responseSession.UserGuid.HasValue)
//            {
//                principal = UserManager.GetUserByGuid(_responseSession.UserGuid.Value);
//            }

//            //If no logged-in user, check response session or invitation
//            if (principal == null)
//            {
//                if (_responseSession.Respondent != null)
//                {
//                    principal = _responseSession.Respondent;
//                }
//                else
//                //Create an anonymous respondent
//                {
//                    //Attempt to auto-login the respondent if there is an invitation w/auto login
//                    if (_responseSession.RecipientGuid.HasValue)
//                    {
//                        //Check if invitation can be located
//                        Invitation invitation = InvitationManager.GetInvitationForRecipient(_responseSession.RecipientGuid.Value);

//                        //Auto login users if enabled, otherwise create a principal using the invitation id as the respondent guid.  This makes
//                        // any resume/edit behavior consistent for the same invitation link across browsers and machines.
//                        if (invitation != null && invitation.Template.LoginOption == LoginOption.Auto)
//                        {
//                            //See if the invitation is for a registered user or not
//                            string respondentUniqueIdentifier = InvitationManager.GetRecipientUniqueIdentifier(_responseSession.RecipientGuid.Value);

//                            if (Utilities.IsNotNullOrEmpty(respondentUniqueIdentifier))
//                            {
//                                principal = UserManager.GetUserPrincipal(respondentUniqueIdentifier);
//                            }
//                        }
//                        else
//                        {
//                            principal = new AnonymousRespondent(_responseSession.RecipientGuid.Value);
//                        }
//                    }
//                }
//            }


//            //If no auto login and no invitation principal was found, create an anonymous respondent
//            if (principal == null)
//            {
//                principal = new AnonymousRespondent(_responseSession.AnonymousRespondentGuid.Value);
//            }

//            _responseSession.Respondent = principal;
//            return principal;
//        }

//        /// <summary>
//        /// Authorize the respondent
//        /// </summary>
//        /// <param name="rt">Response template</param>
//        /// <param name="respondent">Respondent</param>
//        /// <param name="message">Message indicating reasons for auth failure.</param>
//        /// <returns>Boolean indicating success of authorization.</returns>
//        private bool AuthorizeRespondent(ResponseTemplate rt, ExtendedPrincipal respondent, out string message)
//        {
//            message = string.Empty;

//            string[] languageList = rt.LanguageSettings.SupportedLanguages.ToArray();
//            string currentLanguage = GetLanguageCode(rt);

//            if (Utilities.IsNullOrEmpty(currentLanguage))
//            {
//                currentLanguage = rt.LanguageSettings.DefaultLanguage;
//            }

//            //0) A deleted survey is no longer available to anyone
//            if (rt.IsDeleted)
//            {
//                message = TextManager.GetText("/pageText/survey.aspx/surveyDeleted", currentLanguage, "This survey is no longer available.", languageList);
//                return false;
//            }

//            //1) An Editor will always be able to take the survey
//            IAuthorizationProvider authProvider = AuthorizationFactory.GetAuthorizationProvider("ResponseTemplateAuthorizationProvider");

//            if (authProvider.Authorize(respondent, rt, "Form.Edit"))
//            {
//                return true;
//            }

//            //3) Check authorization using auth provider, etc.
//            bool fillAuthorized;

//            string permissionToCheck = _responseSession.AdminEditMode ? "Analysis.Responses.Edit" : "Form.Fill";

//            if (authProvider is ResponseTemplateAuthorizationProvider)
//            {
//                fillAuthorized = ((ResponseTemplateAuthorizationProvider)authProvider).Authorize(respondent, rt, permissionToCheck, _responseSession.RecipientGuid);
//            }
//            else
//            {
//                fillAuthorized = authProvider.Authorize(respondent, rt, permissionToCheck);
//            }

//            //Get auth failure type, if any
//            string failureType = authProvider.GetAuthorizationErrorType().ToLower();

//            //2)  Check invitation response count for invitation-only surveys and disqualify if count met or exceeded.
//            if (_responseSession.RecipientGuid.HasValue
//                && rt.BehaviorSettings.SecurityType == SecurityType.InvitationOnly
//                && rt.BehaviorSettings.MaxResponsesPerUser.HasValue)
//            {
//                if (InvitationManager.GetRecipientResponseCount(_responseSession.RecipientGuid.Value) >= rt.BehaviorSettings.MaxResponsesPerUser)
//                {
//                    fillAuthorized = false;

//                    failureType = rt.BehaviorSettings.AllowEdit ? "editonly" : "responselimitreached";
//                }
//            }


//            //Authorize the respondent
//            if (!fillAuthorized)
//            {
//                if (failureType == "notauthorized")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/respondentNotAuthorized", currentLanguage, "You are not authorized to take this survey.", languageList);
//                }
//                else if (failureType == "notactive")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/surveyNotActive", currentLanguage, "This survey has not been activated.", languageList);
//                }
//                else if (failureType == "loginrequired")
//                {
//                    SessionState = ResponseSessionState.LoginRequired;
//                    message = failureType;
//                }
//                else if (failureType == "beforestartdate")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/surveyNotYetActive", currentLanguage, "The activation period for this survey has not started.", languageList);
//                }
//                else if (failureType == "afterenddate")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/surveyActivationEnded", currentLanguage, "The activation period for this survey has ended.", languageList);
//                }
//                else if (failureType == "responselimitreached")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/MaxResponses", currentLanguage, "No more responses are permitted for this survey.", languageList);
//                }
//                else if (failureType == "editonly")
//                {
//                    //Auth granted, but don't allow creation of new response
//                    _responseLimitReached = true;
//                    return true;
//                }
//                else if (failureType == "noinvitation")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/noInvitation", currentLanguage, "This survey is only available with a valid invitation.", languageList);
//                }
//                else if (failureType == "invalidinvitation")
//                {
//                    message = TextManager.GetText("/pageText/survey.aspx/noInvitation", currentLanguage, "The specified invitation is not valid for this survey.", languageList);
//                }

//                return false;
//            }

//            return true;
//        }

//        /// <summary>
//        /// Get the current language code
//        /// </summary>
//        /// <param name="rt"></param>
//        /// <returns></returns>
//        private string GetLanguageCode(ResponseTemplate rt)
//        {
//            string languageCode = _responseSession.LanguageCode;

//            //Use a dictionary since it supports case-insensitive comparisons when using
//            // overloaded constructor.
//            Dictionary<string, string> surveyLanguagesDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

//            List<string> surveyLanguagesList = new List<string>(rt.LanguageSettings.SupportedLanguages);

//            foreach (string surveyLanguage in surveyLanguagesList)
//            {
//                surveyLanguagesDictionary[surveyLanguage] = surveyLanguage;
//            }

//            //Language code exists and is in the survey, use it
//            if (Utilities.IsNotNullOrEmpty(languageCode) && surveyLanguagesDictionary.ContainsKey(languageCode))
//            {
//                return languageCode;
//            }

//            languageCode = _responseSession.GetRespondentLanguageCode(rt.LanguageSettings.LanguageSource, rt.LanguageSettings.LanguageSourceToken);

//            //Check if there is a language, and that the language code 
//            //  returned is NOT a survey language
//            if (Utilities.IsNotNullOrEmpty(languageCode)
//                && surveyLanguagesDictionary.ContainsKey(languageCode))
//            {
//                _responseSession.LanguageCode = languageCode;
//                return languageCode;
//            }

//            //If the survey only supports one language, use the default
//            if (surveyLanguagesList.Count == 1)
//            {
//                _responseSession.LanguageCode = rt.LanguageSettings.DefaultLanguage;
//                return rt.LanguageSettings.DefaultLanguage;
//            }

//            //No language
//            return null;
//        }

//        /// <summary>
//        /// Check the password
//        /// </summary>
//        /// <param name="rt"></param>
//        /// <returns></returns>
//        private bool CheckPassword(ResponseTemplate rt)
//        {
//            return (
//                (rt.BehaviorSettings.SecurityType != SecurityType.PasswordProtected)
//                || (rt.BehaviorSettings.Password == _responseSession.Password && _responseSession.Password == rt.BehaviorSettings.Password));
//        }

//        /// <summary>
//        /// Get the current response state
//        /// </summary>
//        /// <returns></returns>
//        private ResponseState GetResponseState(ResponseTemplate rt)
//        {
//            //If we have a guid, use that
//            if (_responseSession.ResponseGuid.HasValue)
//            {
//                if (_responseSession.RespondentEditMode || _responseSession.AdminEditMode)
//                {
//                    if (AuthorizationFactory.GetAuthorizationProvider("ResponseTemplateAuthorizationProvider").Authorize(_responseSession.Respondent, rt, "Analysis.Responses.Edit")
//                        || rt.BehaviorSettings.AllowEdit)
//                    {
//                        return ResponseStateManager.GetResponseState(_responseSession.ResponseGuid.Value);
//                    }

//                    throw new Exception("User does not have permission to edit survey responses and/or survey is not configured to allow respondents to edit their responses.");
//                }

//                return ResponseStateManager.GetResponseState(_responseSession.ResponseGuid.Value);
//            }

//            //If we make it this far, then there's no attempt to edit a completed response or resume a saved response
//            //If resume is enabled, try to get the respondent's latest incomplete response
//            if (rt.BehaviorSettings.AllowContinue)
//            {
//                return ResponseStateManager.GetLastIncompleteResponse(_responseSession.Respondent, rt.ID.Value);
//            }

//            return null;
//        }

//        /// <summary>
//        /// Get a list of responses editable by the current user.
//        /// </summary>
//        /// <returns></returns>
//        public DataTable GetCompletedResponseList()
//        {
//            return _completedResponseList;
//        }

//        /// <summary>
//        /// Dispose of the item
//        /// </summary>
//        public void Dispose()
//        {
//            Dispose(true);
//        }

//        /// <summary>
//        /// Dispose of any objects
//        /// </summary>
//        /// <param name="isDisposing"></param>
//        protected virtual void Dispose(bool isDisposing)
//        {
//            if (isDisposing)
//            {
//                if (Response != null)
//                {
//                    /*_response.PageChanged -= new Response.ResponsePageChangedHandler(_response_PageChanged);

//                    if (InvitationID.HasValue)
//                    {
//                        _response.ResponseCompleted -= new Response.ResponseCompletedHandler(_response_ResponseCompleted);
//                    }
//                    */
//                    //Just deference instead of disposing since we don't want to dispose of a potentially cached
//                    // item
//                    //_response.Dispose();
//                    Response = null;
//                }
//            }
//        }
//    }
//}
