using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.TakeSurvey;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey
{
    public delegate void ResponseViewEvent(object sender, ResponseViewEventArgs args);
  
    /// <summary>
    /// Catch-all event args class for response view events.
    /// </summary>
    public class ResponseViewEventArgs
    {
        /// <summary>
        /// Get/set logged-in user name
        /// </summary>
        public string LoggedInUser { get; set; }

        /// <summary>
        /// Get user-selected language
        /// </summary>
        public string SelectedLanguage { get; set; }

        /// <summary>
        /// Get user-entered password for survey
        /// </summary>
        public string EnteredPassword { get; set; }
    }

    /// <summary>
    /// Response view for displaying surveys and associated components
    /// </summary>
    public partial class ResponseView : Checkbox.Web.Common.UserControlBase
    {
        private LoginBase _loginControl;
        private EnterPasswordBase _enterPasswordControl;
        private LanguageSelectBase _languageSelectControl;
        private ProgressSavedBase _progressSavedControl;
        private ResponseSelectBase _responseSelectControl;

        private Guid _sessionGuid;
        private ResponseViewDisplayFlags _displayFlags;
        private SurveyLanguageSettings _languageSettings;
        private SurveyBehaviorSettings _behaviorSettings;
        private string _currentLanguage;
        private string _surveyTitle;
        private int _responseTemplateId;

        /// <summary>
        /// 
        /// </summary>
        public bool HasVisibleRenderers
        {
            get { return _pageView.HasVisibleRenderers; }
        }

        /// <summary>
        /// Initialize response view with current session state
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="displayFlags"></param>
        /// <param name="sessionState"></param>
        /// <param name="languageSettings"></param>
        /// <param name="behaviorSettings"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="surveyTitle"></param>
        public void Initialize(Guid sessionGuid, ResponseViewDisplayFlags displayFlags,
            ResponseSessionState sessionState, SurveyLanguageSettings languageSettings,
            SurveyBehaviorSettings behaviorSettings, string currentLanguage,
            int responseTemplateId, string surveyTitle)
        {
            _sessionGuid = sessionGuid;
            _displayFlags = displayFlags;
            _languageSettings = languageSettings;
            _behaviorSettings = behaviorSettings;
            _currentLanguage = currentLanguage;
            _surveyTitle = surveyTitle;
            _responseTemplateId = responseTemplateId;

            UpdateControlVisibility(displayFlags, sessionState);

            //If taking survey or just completed survey, do nothing else
            if (sessionState == ResponseSessionState.TakeSurvey || sessionState == ResponseSessionState.Completed)
                return;

            //Otherwise initalize pageview here in "nonsurvey" mode.
            _pageView.InitializeForNoResponsePage(currentLanguage);

            //login control
            if (_loginControl != null)
            {
                _loginControl.Initialize(currentLanguage, responseTemplateId);
                _pageView.AddControlToPageView(_loginControl);
            }

            //password control
            if (_enterPasswordControl != null)
            {
                _enterPasswordControl.Initialize(currentLanguage, responseTemplateId);
                _pageView.AddControlToPageView(_enterPasswordControl);
            }

            //language control
            if (_languageSelectControl != null)
            {
                _languageSelectControl.Initialize(languageSettings.SupportedLanguages, languageSettings.DefaultLanguage, responseTemplateId);
                _pageView.AddControlToPageView(_languageSelectControl);
            }

            //progress control
            if (_progressSavedControl != null)
            {
                _progressSavedControl.Initialize(sessionGuid, currentLanguage, behaviorSettings.EnableSendResumeEmail, surveyTitle, behaviorSettings.ResumeEmailFromAddress, responseTemplateId);
                _pageView.AddControlToPageView(_progressSavedControl);
            }

            //panels
            if (_notActivePanel.Visible)
            {
                _notActiveMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_NOTACTIVE", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_notActivePanel);
            }

            if (_surveyDeletedPanel.Visible)
            {
                _surveyDeletedMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_DELETED", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_notActivePanel);
            }

            if (_beforeStartDatePanel.Visible)
            {
                _beforeStartDateMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_NOTYETACTIVE", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_beforeStartDatePanel);
            }

            if (_afterEndDatePanel.Visible)
            {
                _afterEndDateMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_NOLONGERACTIVE", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_afterEndDatePanel);
            }

            if(_noResponsesPanel.Visible)
            {
                _noResponsesMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_MAXRESPONSES", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_noResponsesPanel);
            }

            if (_noInvitationPanel.Visible)
            {
                _noInvitationMsg.Text = SurveyEditorServiceImplementation.GetSurveyText("ACTIVATION_NOINVITATION", responseTemplateId, currentLanguage, languageSettings.SupportedLanguages.ToArray());
                _pageView.AddControlToPageView(_noInvitationPanel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="languageCode"></param>
        /// <param name="authenticatedRespondentGuid"></param>
        /// <param name="listResponsesCallback"></param>
        /// <param name="moreResponsesAllowedCallback"></param>
        public void InitializeResponseList(ResponseSessionData sessionData, string languageCode, Guid? authenticatedRespondentGuid, Func<Guid, SurveyResponseData[]> listResponsesCallback, Func<Guid, bool> moreResponsesAllowedCallback)
        {
            if (_responseSelectControl != null)
                _responseSelectControl.Initialize(sessionData, authenticatedRespondentGuid, languageCode, listResponsesCallback, moreResponsesAllowedCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerHtml"></param>
        /// <param name="footerHtml"></param>
        public void ApplyHeaderAndFooter(string headerHtml, string footerHtml)
        {
            _pageView.ApplyHeaderAndFooter(headerHtml, footerHtml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="subMessage"></param>
        public void ShowFatalError(string errorMessage, string subMessage)
        {
            UpdateControlVisibility(ResponseViewDisplayFlags.None, ResponseSessionState.None);

            _errorPanel.Visible = true;
            _errorMessage.Text = errorMessage;

            if (ApplicationManager.AppSettings.LogErrorsToUI)
            {
                _errorSubMessage.Text = subMessage;
                _moreInfoMessage.Text = TextManager.GetText("/errorMessages/common/moreInfo",
                                                            TextManager.DefaultLanguage,
                                                            "Checkbox error log or server event log may contain more information.");
            }
        }

        /// <summary>
        /// Bind to response page with callback to get response items. Call prevents need for response view to know details
        /// about service layer.
        /// </summary>
        /// <param name="responsePage">The response page.</param>
        /// <param name="getItemsCallback">The get items callback.</param>
        /// <param name="displayFlags">The display flags.</param>
        /// <param name="languageSettings">The language settings.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseTemplate">The response template.</param>
        /// <param name="pageNumbers">The page numbers.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="items">The items.</param>
        /// <param name="state">The state.</param>
        /// <param name="completeEventItems">The complete event items.</param>
        /// <param name="responseSessionData">The response session data.</param>
        /// <param name="rGuid">The r unique identifier.</param>
        public void BindToPage(SurveyResponsePage responsePage, GetResponsePageItemsCallback getItemsCallback,
            ResponseViewDisplayFlags displayFlags, SurveyLanguageSettings languageSettings, string languageCode,
            ResponseTemplate responseTemplate, PageNumberInfo pageNumbers, ExportMode exportMode,
            IEnumerable<IItemProxyObject> items = null, ResponseSessionState state = ResponseSessionState.None,
            IEnumerable<IItemProxyObject> completeEventItems = null, ResponseSessionData responseSessionData = null , Guid? rGuid = null)
        {
            RenderMode renderMode = RenderMode.Survey;
            if (WebUtilities.IsBrowserMobile(Request))
                renderMode = RenderMode.SurveyMobile;

            _pageView.Initialize(responsePage, getItemsCallback, renderMode, displayFlags, languageSettings,
                languageCode, responseTemplate, pageNumbers, exportMode, items, state, completeEventItems, responseSessionData, rGuid);
            _pageView.BindRenderers();
        }

        /// <summary>
        /// Update pate with user inputs
        /// </summary>
        public void UpdatePage()
        {
            _pageView.UpdateItems();
        }

        /// <summary>
        /// List items on page
        /// </summary>
        /// <returns></returns>
        public List<IItemProxyObject> ListPageItems()
        {
            return _pageView.ListItems();
        }

        /// <summary>
        /// Update visibility depending on displayflags and control state
        /// </summary>
        /// <param name="displayFlags"></param>
        /// <param name="controlState"></param>
        private void UpdateControlVisibility(ResponseViewDisplayFlags displayFlags, ResponseSessionState controlState)
        {
            SetVisibility(_errorPanel, ResponseSessionState.Error, controlState);
            
            //Page view is visible when completed, when taking survey, or when displaying survey-releated messages
            if (controlState == ResponseSessionState.TakeSurvey || controlState == ResponseSessionState.None)
                SetVisibility(_pageView, ResponseSessionState.TakeSurvey, controlState);

            SetVisibility(_notActivePanel, ResponseSessionState.SurveyNotActive, controlState);
            SetVisibility(_beforeStartDatePanel, ResponseSessionState.BeforeStartDate, controlState);
            SetVisibility(_afterEndDatePanel, ResponseSessionState.AfterEndDate, controlState);
            SetVisibility(_noResponsesPanel, ResponseSessionState.ResponseLimitReached, controlState);
            SetVisibility(_noInvitationPanel, ResponseSessionState.InvitationRequired, controlState);
            SetVisibility(_surveyDeletedPanel, ResponseSessionState.SurveyDeleted, controlState);

            //set visibility to edit response
            bool isMobile = IsBrowserMobile;
            if (controlState == ResponseSessionState.SelectLanguage)
            {
                _languageSelectControl = isMobile ? _languageSelectMobile : (LanguageSelectBase)_languageSelect;
                _languageSelectControl.Visible = true;
            }
            else if (_languageSelectControl != null)
                _languageSelectControl.Visible = false;

            if (controlState == ResponseSessionState.EditResponse)
            {
                _responseSelectControl = isMobile ? _responseSelectMobile : (ResponseSelectBase)_responseSelect;
                _responseSelectControl.Visible = true;
            }
            else if (_responseSelectControl != null)
                _responseSelectControl.Visible = false;

            if (controlState == ResponseSessionState.SavedProgress)
            {
                _progressSavedControl = isMobile ? _progressSavedMobile : (ProgressSavedBase)_progressSaved;
                _progressSavedControl.Visible = true;
            }
            else if (_progressSavedControl != null)
                _progressSavedControl.Visible = false;

            if (controlState == ResponseSessionState.EnterPassword)
            {
                _enterPasswordControl = isMobile ? _enterPasswordMobile : (EnterPasswordBase)_enterPassword;
                _enterPasswordControl.Visible = true;
            }
            else if (_enterPasswordControl != null)
                _enterPasswordControl.Visible = false;

            if (controlState == ResponseSessionState.LoginRequired)
            {
                _loginControl = isMobile ? _loginMobile : (LoginBase)_login;
                _loginControl.Visible = true;
            }
            else if (_loginControl != null)
                _loginControl.Visible = false;
        }

        private bool IsBrowserMobile
        {
            get { return WebUtilities.IsBrowserMobile(Request); }
        }

        /// <summary>
        /// Set visibility of controls if the current control state matches the required state
        /// </summary>
        /// <param name="c"></param>
        /// <param name="requiredState"></param>
        /// <param name="currentState"></param>
        private static void SetVisibility(Control c, ResponseSessionState requiredState, ResponseSessionState currentState)
        {
            if (c != null)
            {
                c.Visible = (requiredState == currentState);
            }
        }

        /// <summary>
        /// Sets the percentage completed 
        /// </summary>
        /// <param name="percent"></param>
        public void SetProgressPercent(double percent)
        {
            _pageView.SetProgressPercent(percent);
        }


        /// <summary>
        /// </summary>
        public string GetAuthenticatedUserName()
        {
            string userName = _loginControl.GetAuthenticatedUserName();
            
            if (string.IsNullOrEmpty(userName))
                return null;

            CheckboxPrincipal user = UserManager.GetUserPrincipal(userName);
            return user.Identity.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetSelectedLanguage()
        {
            return _languageSelectControl.SelectedLanguage;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetPasswordEntered()
        {
            return _enterPasswordControl.GetPasswordEntered();
        }      
  
        public void SendResumeEmail()
        {
            if(_progressSavedControl == null)
                Initialize(_sessionGuid, _displayFlags, ResponseSessionState.SavedProgress, _languageSettings, _behaviorSettings, _currentLanguage, _responseTemplateId, _surveyTitle);

            _progressSavedControl.SendResumeEmail();
        }
    }
}
