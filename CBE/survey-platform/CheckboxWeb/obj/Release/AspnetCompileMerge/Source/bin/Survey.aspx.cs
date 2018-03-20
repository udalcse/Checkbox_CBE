using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.LocalProxies;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Forms.Security.Principal;

namespace CheckboxWeb
{
    /// <summary>
    /// Container for survey processing
    /// </summary>
    public partial class Survey : PersistedStatePage
    {
        /// <summary>
        /// Response session for handling session related information
        /// </summary>
        private FormQueryParameters _queryParameters;
        private ISurveyResponseService _responseSvc;
        private ResponseSessionData _sessionData;
        private bool _hasSPC;
       
        private CacheContext CacheContext;


        private string _pageItemsUpdateState;

        //Control variables to prevent infinite loop when binding, which can happen if page has
        // no visible renderers, but move next returns to same page (this probably would indicate a malfunction
        // in the survey workflow)
        private int _lastPageId;
        private int _loopCounter;
        private int _pageIndex = 0;
        private int _nextPage = 0;
        private int _currentPage = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool ShowValidationAlert
        {
            get;
            set;
        }

        //Response Guid. Need for loading data while PDF converting
        [QueryParameter]
        public Guid? rGuid { get; set; }

        public FormQueryParameters QueryParameters { get { return _queryParameters; } }

        public ResponseTemplate RenderedResponseTemplate { get; set; }
        /// <summary>
        /// Builds valid URL for the client side
        /// </summary>
        protected string SPCInitUrlEncoded
        {
            get
            {
                var s = Request.Url.ToString().Replace("'", "\\'");
                //the case of the SSL Offloading: from the proxy server we are getting http:// 
                if (ApplicationManager.AppSettings.EnableSslOffloading 
                    || ApplicationManager.ApplicationURL.StartsWith("https://"))
                {
                    s = s.Replace("http://", "https://");
                }
                return Utilities.AdvancedHtmlEncode(s); 
            }
        }

        protected string SocialMediaDescription;

        /// <summary>
        /// 
        /// </summary>
        private string StoredResponseSessionDataKey
        {
            get { return "SurveySession_" + (_queryParameters.ResumeInstanceId ?? (_queryParameters.InvitationGuid ?? (_queryParameters.ResponseGuid ?? _queryParameters.SurveyGuid))); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object LoadPageStateFromPersistenceMedium()
        {
            var viewState = Request.Form["__VSTATE"];
            var bytes = Convert.FromBase64String(viewState);
            bytes = Utilities.Decompress(bytes);
            var formatter = new LosFormatter();
            return formatter.Deserialize(Convert.ToBase64String(bytes));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewState"></param>
        protected override void SavePageStateToPersistenceMedium(object viewState)
        {
            var formatter = new LosFormatter();
            var writer = new StringWriter();
            formatter.Serialize(writer, viewState);
            var viewStateString = writer.ToString();
            var bytes = Convert.FromBase64String(viewStateString);
            bytes = Utilities.Compress(bytes);
            ClientScript.RegisterHiddenField("__VSTATE", Convert.ToBase64String(bytes));
        }        
        
        /// <summary>
        /// Perform page initialization
        /// </summary>
        protected override void OnPageInit()
        {
            try
            {
                base.OnPageInit();

                //Validate query parameters.
                _queryParameters = new FormQueryParameters();
                ValidateQueryParameters();

                //TODO: Make proxy used configurable
                //Initialize service proxy

                //WCF client for eventual use w/distributed survey front-end servers and central 
                // processing server
                //_responseSvc = new SurveyResponseServiceProxy("PoxSurveyResponseClient"); 
                _responseSvc = new LocalSurveyResponseServiceProxy();

                HttpContext.Current.Response.AddHeader("p3p", "CP=\"This Site Does Not have a P3P policy navigate to www.checkbox.com/p3p to learn more\"");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage, "An error occurred. Unable to continue."), ex.Message);
            }
        }

        /// <summary>
        /// Attempt to get identifier for current survey session from view state
        /// </summary>
        protected override void OnPageLoad()
        {
            try
            {
                base.OnPageLoad();

                //force reauthentication
                if ("true".Equals(Request.Params["forceReAuthentication"], StringComparison.InvariantCultureIgnoreCase))
                {
                    var currentPrincipal = UserManager.GetCurrentPrincipal();

                    if (currentPrincipal != null)
                        UserManager.ExpireLoggedInUser(currentPrincipal.Identity.Name);

                    //Logout forms authentication
                    FormsAuthentication.SignOut();

                    //Abandon session & clear session cookie
                    Session.Abandon();

                    if (ApplicationManager.AppSettings.PreventSessionReuse)
                        Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

                    Response.Redirect(Request.Url.ToString().ToLower().Replace("forcereauthentication=true", ""), false);
                    return;
                }

                var currentUser = HttpContext.Current.User as CheckboxPrincipal;

                if(ApplicationManager.AppSettings.IsPrepMode && currentUser == null || currentUser is AnonymousRespondent)
                {
                    Session.Abandon();
                }

                //make sure that we logged in as a user from an invitation if it is prep mode
                if (ApplicationManager.AppSettings.IsPrepMode && _queryParameters.InvitationGuid.HasValue)
                {
                    if (currentUser != null && currentUser.IsRoleContains("Administrator"))
                    {
                        var recepient = InvitationManager.GetRecipientByGuid(_queryParameters.InvitationGuid.Value);
                        var guid = UserManager.GetUserGuid(recepient.UniqueIdentifier);
                        var user = UserManager.GetUserByGuid(guid);

                        if (!recepient.EmailToAddress.Equals(currentUser.Email))
                        {
                            UserManager.ExpireLoggedInUser(currentUser.Identity.Name);
                            ReloginUserAs(user.Identity.Name);

                            var url = string.IsNullOrEmpty(Request.QueryString["restoreUser"]) ? $"{Request.RawUrl}&restoreUser={currentUser.Identity.Name}" : Request.RawUrl;

                            Response.Redirect(url, false);
                            
                            return;
                        }
                    }
                }

                //check that we have selected language already
                string prevSessionLanguage = "";
                int? prevResponseTemplateId = null;
                Guid? prevResponseTemplateGuid = null;
                if (ApplicationManager.AppSettings.StoreSessionKeyInHttpSesssion
                    && Session[StoredResponseSessionDataKey] != null)
                {
                    prevSessionLanguage = ((ResponseSessionData)Session[StoredResponseSessionDataKey]).SelectedLanguage;
                    prevResponseTemplateId = ((ResponseSessionData)Session[StoredResponseSessionDataKey]).ResponseTemplateId;
                    prevResponseTemplateGuid = ((ResponseSessionData)Session[StoredResponseSessionDataKey]).ResponseTemplateGuid;
                }

                //Get session data
                _sessionData = GetSessionData();

                //Set user context for logging purposes
                SetUserContext();

                //create new cache context
                CacheContext = new CacheContext();

                 //If resuming, attempt to determine response id from session or response guid
                if (!_sessionData.ResponseTemplateId.HasValue
                    && (_sessionData.ResumeInstanceId.HasValue || _sessionData.ResponseGuid.HasValue))
                {
                    int? surveyId = null;

                    //Handle response edit or resume with legacy response id
                    if(_sessionData.ResponseGuid.HasValue)
                        surveyId = ResponseManager.GetSurveyIdFromResponseGuid(_sessionData.ResponseGuid.Value);

                    if (surveyId == null)
                    {
                        var getIdResult = _responseSvc.GetTemplateIdForSession(_sessionData.SessionGuid, CacheContext);

                        if (getIdResult.CallSuccess)
                            surveyId = getIdResult.ResultData;
                    }

                    if(!surveyId.HasValue || surveyId <= 0)
                        throw new Exception("Unable to determine survey from session id: " + _sessionData.SessionGuid);

                    _sessionData.ResponseTemplateId = surveyId.Value;
                
                    //try to restore session language
                    var getLangResult = _responseSvc.GetLanguageForSession(_sessionData.SessionGuid, CacheContext);

                    if (getLangResult.CallSuccess)
                        _sessionData.SelectedLanguage = getLangResult.ResultData;
                }

                if (!_sessionData.ResponseTemplateId.HasValue)
                    throw new Exception("Unable to determine survey from session id: " + _sessionData.SessionGuid);
                
                //We are working with the same survey, let's restore it's language
                if (string.IsNullOrEmpty(_sessionData.SelectedLanguage) && !_queryParameters.ForceNew && 
                    (prevResponseTemplateId == _sessionData.ResponseTemplateId
                    || _sessionData.ResponseTemplateGuid.HasValue && prevResponseTemplateGuid == _sessionData.ResponseTemplateGuid))
                {
                    _sessionData.SelectedLanguage = prevSessionLanguage;
                }

                //Reset session. Page items aren't updated
                _pageItemsUpdateState = "PageItemsUpdateCondition_" + _sessionData.ResponseTemplateId.Value;
                Session[_pageItemsUpdateState] = false;

                //Check survey id, except if resuming a response
                CacheContext.ResponseTemplate = ResponseTemplateManager.GetResponseTemplate(_sessionData.ResponseTemplateId.Value);
                RenderedResponseTemplate = CacheContext.ResponseTemplate;

                if (CacheContext.ResponseTemplate == null)
                    throw new Exception("Unable to load survey with id: " + _sessionData.ResponseTemplateId);

                //init google analytics scontrol if track id is set
                if (!string.IsNullOrEmpty(CacheContext.ResponseTemplate.BehaviorSettings.GoogleAnalyticsTrackingID))
                {
                    _googleAnalytics.Visible = true;
                    _googleAnalytics.Initialize(CacheContext.ResponseTemplate.BehaviorSettings.GoogleAnalyticsTrackingID);
                }

                if (string.IsNullOrEmpty(_sessionData.SelectedLanguage))
                    _sessionData.SelectedLanguage = GetSelectedLanguage();

                ShowValidationAlert = CacheContext.ResponseTemplate.StyleSettings.ShowValidationErrorAlert;
                _sessionData.ResponseTemplateGuid = CacheContext.ResponseTemplate.GUID;

                SocialMediaDescription = SurveyEditorServiceImplementation.GetSurveyText("SOCIAL_MEDIA_DESCRIPTION",
                    (int)_sessionData.ResponseTemplateId, CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage, CacheContext.ResponseTemplate.LanguageSettings.SupportedLanguages.ToArray());

                //Get current session state.
                var sessionstate = GetCurrentSessionState();

                ApplyStyleTemplate();

                InitializeAndBindResponseView(sessionstate);

                //NWC: 2010-12-20.  Login event may not be appropriate
                // dpepending on session state.  Next action needs to depend on state,
                // for example if state requires language selection, next event needs to be to
                // select language.

                //Persist session data to viewstate/session

                var printingNotStartedPdf = (_sessionData.SessionState == ResponseSessionState.None &&
                                       _queryParameters.ExportMode == ExportMode.ClientPdf);
             
                //don't save state if a survey is not started and mode is client printing pdf 
                if (!printingNotStartedPdf)
                    PersistSessionData();


             

                //handle ajax postback
                DoSurveyActions();

                //set no-cache headers
                Response.Cache.SetNoStore();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage, "An error occurred. Unable to continue."), ex.Message);
            }
        }

        private void ReloginUserAs(string name)
        {
            //clear current user information 
            FormsAuthentication.SignOut();
            Session.Abandon();

            FormsAuthentication.SetAuthCookie(name, false);
            HttpContext.Current.User = UserManager.GetUserPrincipal(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoSurveyActions()
        {
            if (!IsPostBack)
                return;
            
            //check, if there was survey footer action buttons postback
            switch (Action)
            {
                case "next":
                    MoveNext();
                    break;
                case "finish":
                    Finish();
                    break;
                case "back":
                    MovePrevious();
                    break;
                case "save":
                    ResponseViewSaveProgress();
                    break;
                case "login":
                    UserLoggedIn();
                    break;
                case "password":
                    PasswordEntered();
                    break;
                case "language":
                    LanguageSelected();
                    break;
                case "resume-by-email":
                    SendResumeLinkByEmail();
                    break;
                case "spc":
                    RefreshSPC();
                    break;
            }
        }

        private string GetActionNameFromPostBackControl()
        {
            WebControl control = null;

            //first we will check the "__EVENTTARGET" because if post back made by the controls
            //which used "_doPostBack" function also available in Request.Form collection.
            string ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctrlname))
                control = FindFirstChildControl<WebControl>(ctrlname);
            else
            {
                // if __EVENTTARGET is null, the control is a button type and we need to
                // iterate over the form collection to find it
                foreach (string ctl in Page.Request.Form)
                {
                    string ctrlStr = ctl;

                    //handle ImageButton they having an additional "quasi-property" in their Id which identifies
                    //mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);

                    control = FindFirstChildControl<Button>(ctrlStr);

                    if (control != null)
                        break;
                }
            }

            return control != null ? control.Attributes["data-action"] : null;
        }

        private string Action
        {
            get { return Request.Form["action"] ?? GetActionNameFromPostBackControl(); }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PersistSessionData()
        {
            //Update session, if necessary
            ViewState[StoredResponseSessionDataKey] = _sessionData;

            if (ApplicationManager.AppSettings.StoreSessionKeyInHttpSesssion)
            {
                Session[StoredResponseSessionDataKey] = _sessionData;
            }
        }

        /// <summary>
        /// Apply the survey's style template.
        /// </summary>
        private void ApplyStyleTemplate()
        {
            bool isMobile = IsBrowserMobile;

            if (isMobile)
            {
                MobileStyle mobileStyle = CacheContext.ResponseTemplate.StyleSettings.MobileStyleId.HasValue ?
                    MobileStyleManager.GetStyle(CacheContext.ResponseTemplate.StyleSettings.MobileStyleId.Value, TextManager.DefaultLanguage) :
                    MobileStyleManager.GetDefaultStyle(TextManager.DefaultLanguage);

                if (mobileStyle != null)
                {
                    _mobileStyleInclude.Source = mobileStyle.CssUrl;
                    //if there is some not default mobile style applied, don't apply PC 
                    if (!mobileStyle.IsDefault)
                        return;
                }
            }

            if (!CacheContext.ResponseTemplate.StyleSettings.StyleTemplateId.HasValue)
                return;

            StyleTemplate st = StyleTemplateManager.GetStyleTemplate(CacheContext.ResponseTemplate.StyleSettings.StyleTemplateId.Value);

            if (st == null)
                return;

            string css = isMobile ? st.GetCssForMobile() : st.GetCss();
            string tinyMceStyles = GetTinyMceStyles(ref css);

            _surveyStylePlace.Controls.Clear();
            _surveyStylePlace.Controls.Add(new LiteralControl("<style type=\"text/css\">" + css + "</style>"));

            if (!CacheContext.ResponseTemplate.StyleSettings.HideFooterHeader || !IsBrowserMobile)
            {
                //Hardcode for english until ml style editing is funcitonal
                _responseView.ApplyHeaderAndFooter(
                    TextManager.GetText(st.HeaderTextID, "en-US"),
                    TextManager.GetText(st.FooterTextID, "en-US")
                    );
            }

            string jsMethodName =  "addStylesToTinyMce(\"" + tinyMceStyles + "\")";
            ScriptManager.RegisterClientScriptBlock(this, typeof(string), "tinyMceStyles", jsMethodName, true);

            /*_responseView.ApplyHeaderAndFooter(
                TextManager.GetText(st.HeaderTextID, _sessionData.SelectedLanguage ?? CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage),
                TextManager.GetText(st.FooterTextID, _sessionData.SelectedLanguage ?? CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage)
            );*/
        }

        private string GetTinyMceStyles(ref string css)
        {
            var indexOfFlyoverStyle = css.IndexOf("#tinymce a[href='#']");
            var endOfFlyoverStyle = -1;

            if(indexOfFlyoverStyle != -1)
                endOfFlyoverStyle = css.IndexOf("}", indexOfFlyoverStyle);

            var flyoverStyle = string.Empty;

            if(indexOfFlyoverStyle != -1 && endOfFlyoverStyle != -1)
                flyoverStyle = css.Substring(indexOfFlyoverStyle, (endOfFlyoverStyle + 1) - indexOfFlyoverStyle);

            if(flyoverStyle.Length > 0)
                css = css.Replace(flyoverStyle, string.Empty);

            var indexOfLinkStyle = css.IndexOf("#tinymce a:not([href*='#'])");
            var endOfLinkStyle = -1;

            if (indexOfLinkStyle != -1)
                endOfLinkStyle = css.IndexOf("}", indexOfLinkStyle);

            var linkStyle = string.Empty;

            if(indexOfLinkStyle != -1 && endOfLinkStyle != -1)
                linkStyle = css.Substring(indexOfLinkStyle, (endOfLinkStyle + 1) - indexOfLinkStyle);

            if(linkStyle.Length > 0)
                css = css.Replace(linkStyle, string.Empty);

            return string.Concat(flyoverStyle, linkStyle);
        }

        protected bool IsBrowserMobile
        {
            get { return WebUtilities.IsBrowserMobile(Request); }
        }

        protected bool IsAjaxifyingSupported
        {
            get { return WebUtilities.IsAjaxifyingSupported(Request); }
        }

        /// <summary>
        /// Initialize and bind the response view based on current state.
        /// </summary>
        private void InitializeAndBindResponseView(ResponseSessionState sessionState)
        {
            ResponseSessionState previousSessionState = sessionState;

            if (sessionState == ResponseSessionState.Error)
            {
                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage, "An error occurred. Unable to continue."), "Response session state indicates error.");
                return;
            } 

            //If response process started, create a new session
            if (sessionState == ResponseSessionState.None)
            {
                var createStateResult = _responseSvc.CreateResponseSession(_sessionData.SessionGuid, _sessionData, CacheContext);

                if (!createStateResult.CallSuccess)
                {
                    throw new Exception("An error occurred while creating a new response session: " + createStateResult.FailureMessage);
                }

                sessionState = createStateResult.ResultData;
            }

            //
            if (sessionState == ResponseSessionState.RespondentRequired)
            {
                string respondentUID = _sessionData.AuthenticatedRespondentUid;

                if (string.IsNullOrEmpty(respondentUID) && User != null && User.Identity != null)
                {
                    respondentUID = User.Identity.Name;
                }
                var respondentSetResult = _responseSvc.InitializeRespondent(_sessionData.SessionGuid,
                                                                            respondentUID,
                                                                            _sessionData.AnonymousRespondentGuid,
                                                                            _sessionData.InvitationRecipientGuid,
                                                                            _sessionData.DirectInvitationRecipientGuid,
                                                                            CacheContext);

                if (!respondentSetResult.CallSuccess)
                {
                    throw new Exception("An error occurred while initializing the respondent: " + respondentSetResult.FailureMessage);
                }

                sessionState = respondentSetResult.ResultData;
            }

            //If looking for password, check to see if password already entered and use that one instead
            if (sessionState == ResponseSessionState.EnterPassword
                && Session["EnteredPassword"] != null)
            {
                _sessionData.EnteredPassword = Session["EnteredPassword"] as string;

                var setPasswordResult = _responseSvc.SetPassword(_sessionData.SessionGuid, _sessionData.EnteredPassword,
                                                                            CacheContext);

                if (!setPasswordResult.CallSuccess)
                {
                    throw new Exception("An error occurred while initializing the respondent: " + setPasswordResult.FailureMessage);
                }

                sessionState = setPasswordResult.ResultData;

                //If state is still enter password, erase current password so we don't hit this code again and proceed normally.
                if (sessionState == ResponseSessionState.EnterPassword)
                {
                    _sessionData.EnteredPassword = string.Empty;
                    Session["EnteredPassword"] = null;
                    PersistSessionData();
                }
            }

            //If we should be resuming an existing workflow, get the sessionKey
            if (sessionState == ResponseSessionState.ResumeExistingWorkflow)
            {

                var getSessionKeyResult = _responseSvc.GetResumeSessionKey(_sessionData.SessionGuid,
                                                                            CacheContext);

                if (!getSessionKeyResult.CallSuccess || !getSessionKeyResult.ResultData.HasValue)
                {
                    throw new Exception("Unable to get ID of session to resume.");
                }

                //If resuming, update current session key and re-fetch state information so that
                // current state matches state of response to resume.
                _sessionData.SessionGuid = getSessionKeyResult.ResultData.Value;

                GetCurrentSessionState();
            }

            //Update global state
            _sessionData.SessionState = sessionState;

            //Get display flags based on state
            ResponseViewDisplayFlags displayFlags = GetDisplayFlags(CacheContext.ResponseTemplate.StyleSettings, CacheContext.ResponseTemplate.BehaviorSettings, sessionState, _sessionData.IsEdit);
       


            //Set page title
            Title = Utilities.StripHtml(TextManager.GetText(CacheContext.ResponseTemplate.TitleTextID,
                                        string.IsNullOrEmpty(_sessionData.SelectedLanguage) ?
                                        CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage :
                                        _sessionData.SelectedLanguage, CacheContext.ResponseTemplate.Name));

            _pageTitleLiteral.Text = Title;

            var currentLanguage = !string.IsNullOrEmpty(_sessionData.SelectedLanguage)
                ? _sessionData.SelectedLanguage
                : CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage;

            //Initialize response view
            _responseView.Initialize(
                _sessionData.ResumeInstanceId ?? _sessionData.SessionGuid,
                displayFlags,
                sessionState,
                CacheContext.ResponseTemplate.LanguageSettings,
                CacheContext.ResponseTemplate.BehaviorSettings,
                currentLanguage,
                CacheContext.ResponseTemplate.ID.Value,
                Title);

            //If necessary, initialize response list
            if (sessionState == ResponseSessionState.EditResponse)
            {
                _responseView.InitializeResponseList(
                    _sessionData,
                    currentLanguage,
                    GetRespondentGuid(),
                    ListResponsesForUser,
                    GetMoreResponsesAllowed);
            }

            //If taking survey, bind items.  If current state is that progress was saved, continue survey if page is not posting back...
            // i.e. resuming
            if (sessionState == ResponseSessionState.TakeSurvey || sessionState == ResponseSessionState.Completed)
            {

                if (_queryParameters.ExportMode == ExportMode.ClientPdf)
                {
                    displayFlags = ResponseViewDisplayFlags.None;

                    var itemPages = _responseSvc.GetAllResponsePages(_sessionData.SessionGuid, CacheContext, rGuid)
                            .ResultData.Where(item => item.PageType == "ContentPage")
                            .ToList();

                    var completeEventPage = _responseSvc.GetAllResponsePages(_sessionData.SessionGuid, CacheContext, rGuid)
                            .ResultData
                            .FirstOrDefault(item => item.PageType == "Completion");

                    if (previousSessionState == ResponseSessionState.None)
                        SurveyResponseServiceImplementation.IgnoreConditionTypes(_sessionData.SessionGuid, CacheContext, new List<string> { "Checkbox.Forms.Logic.AnswerableOperand", "Checkbox.Forms.Logic.SelectItemOperand", "Checkbox.Forms.Logic.SumOperand" });

                    List<ItemProxyObject> itemObjects = new List<ItemProxyObject>();
                    List<ItemProxyObject> completeEventsItemObjects = new List<ItemProxyObject>();

                 
                    foreach (var item in itemPages)
                    {
                        _responseSvc.SetPage(_sessionData.SessionGuid, item.PageId, CacheContext);
                        var allPageItems = _responseSvc.GetAllPageItems(_sessionData.SessionGuid, item.PageId, CacheContext);
                        foreach (var pageItem in allPageItems.ResultData)
                        {
                            pageItem.PageId = item.PageId;
                        }
                        itemObjects.AddRange(allPageItems.ResultData);
                        itemObjects.Last().IsLastItemOnPage = true;
                    }

                    if (completeEventPage != null)
                    {
                        _responseSvc.SetPage(_sessionData.SessionGuid, completeEventPage.PageId, CacheContext);
                        var completeEventPageItems = _responseSvc.GetAllPageItems(_sessionData.SessionGuid,
                            completeEventPage.PageId, CacheContext);
                        completeEventsItemObjects.AddRange(completeEventPageItems.ResultData);
                    }

                    SurveyResponseServiceImplementation.СlearIgnoreConditionTypes(_sessionData.SessionGuid, CacheContext);
                    
                    _responseView.BindToPage(itemPages[0], GetPageItems, displayFlags,
                        CacheContext.ResponseTemplate.LanguageSettings, currentLanguage, CacheContext.ResponseTemplate,
                        new PageNumberInfo(), _queryParameters.ExportMode, itemObjects, _sessionData.SessionState, completeEventsItemObjects, null , rGuid);

                    // if it is pdf printing and initial state is none  we should not change state for this survey not to prevent starting it 
                    if (previousSessionState == ResponseSessionState.None || rGuid != null)
                    {
                        var cacheManger = CacheFactory.GetCacheManager("stateMachines");

                        foreach (var key in cacheManger.ListKeys())
                        {
                            if (key.Contains(_sessionData.SessionGuid.ToString()))
                                cacheManger.Remove(key);
                        }

                        Session.Remove(StoredResponseSessionDataKey);
                        _sessionData = new ResponseSessionData();
                    }
                }
                else
                {
                    var getPageResult = _responseSvc.GetCurrentResponsePage(_sessionData.SessionGuid, CacheContext);
                    var responsePage = getPageResult.ResultData;



                    if (!getPageResult.CallSuccess)
                    {
                        throw new Exception("An error occurred while getting the current response page: " + getPageResult.FailureMessage);
                    }



                    if (responsePage == null)
                    {
                        throw new Exception("Unable to get current page for survey.");
                    }

                    if (responsePage.PageId == _lastPageId)
                    {
                        _loopCounter++;
                    }
                    else
                    {
                        _loopCounter = 0;
                        _lastPageId = responsePage.PageId;
                    }

                    //Hide back button for hidden items and first page that have positions
                    // 1 and 2
                    if (responsePage.Position < 3)
                    {
                        displayFlags &= ~ResponseViewDisplayFlags.BackButton;
                    }
                    else
                    {
                        var visitedPagesResult = _responseSvc.GetVisitedPageVisibilities(_sessionData.SessionGuid, CacheContext);
                        if (!visitedPagesResult.CallSuccess)
                        {
                            throw new Exception("An error occurred while getting visited pages visibilities: " + getPageResult.FailureMessage);
                        }

                        var currentPage = responsePage;
                        bool visibleExist = false;
                        foreach (var page in visitedPagesResult.ResultData)
                        {
                            if (page.Key != currentPage.PageId && page.Value)
                                visibleExist = true;
                        }

                        if (!visibleExist)
                            displayFlags &= ~ResponseViewDisplayFlags.BackButton;
                    }

                    if (CacheContext.ResponseTemplate.BehaviorSettings.AllowFormReset &&
                        sessionState != ResponseSessionState.Completed)
                    {
                        displayFlags |= ResponseViewDisplayFlags.FormResetButton;
                    }

                    //Only bind if we are not in loop
                    if (_loopCounter < 2)
                    {
                        var getPageNumberResult = _responseSvc.GetPageNumber(_sessionData.SessionGuid, CacheContext);
                        if (!getPageNumberResult.CallSuccess)
                        {
                            throw new Exception("An error occurred while getting the current page number: " + getPageNumberResult.FailureMessage);
                        }

                        PageNumberInfo pageNumbers = getPageNumberResult.ResultData; //responsePage.Position - 1;

                        _responseView.BindToPage(responsePage, GetPageItems, displayFlags, CacheContext.ResponseTemplate.LanguageSettings, currentLanguage, CacheContext.ResponseTemplate, pageNumbers, _queryParameters.ExportMode, null , sessionState , null ,_sessionData);
                        if (!_responseView.HasVisibleRenderers)
                            MoveNext();

                        if ((displayFlags & ResponseViewDisplayFlags.ProgressBar) == ResponseViewDisplayFlags.ProgressBar)
                        {
                            if (pageNumbers.TotalPageCount > 0)
                            {
                                //Subtract 1 from page count so first page always shows as 0 and so that progress does not show
                                // fully complete BEFORE last page submitted.
                                _responseView.SetProgressPercent(((double)(pageNumbers.CurrentPageNumber - 1) / (double)pageNumbers.TotalPageCount) * 100.0);
                            }
                        }

                        if (!responsePage.PageType.Equals("HiddenItems"))
                            _hasSPC = responsePage.HasSPC;
                    }
                }
            }
        }

        /// <summary>
        /// Override prerender to handle all ways of rendering the page: init, click next or prev, page refresh
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            IncludeItemsScpecificScripts();

            string spcManagerName = IsAjaxifyingSupported ? "surveyWorkflow.IsSpcEnabled=" : "SPCManager.enabled=";

            //scripts for SPC
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "SPCBlock", "<script language=\"javascript\">$(document).ready(function(){" 
                + spcManagerName + _hasSPC.ToString().ToLower() + ";});</script>");
            if (CacheContext != null)
            {


                if (CacheContext.ResponseTemplate == null ||
                    CacheContext.ResponseTemplate.BehaviorSettings.AllowFormReset)
                {
                    EmbedResetButtonScript();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void IncludeItemsScpecificScripts()
        {
            RegisterClientScriptInclude("selectToUISlider", ResolveUrl("~/Resources/selectToUISlider.jQuery.js"));

            if (ApplicationManager.AppSettings.AddressFinderEnabled)
                RegisterClientScriptInclude("IncludeAddressFinder", "http://www.addressfinder.co.nz/assets/v2/widget.js");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Guid? GetRespondentGuid()
        {
            if(string.IsNullOrEmpty(_sessionData.AuthenticatedRespondentUid))
            {
                return null;
            }

            var respondent = UserManager.GetUserPrincipal(_sessionData.AuthenticatedRespondentUid);

            return respondent != null && respondent.UserGuid != Guid.Empty
                       ? respondent.UserGuid
                       : (Guid?) null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SurveyResponseData[] ListResponsesForUser(Guid sessionKey)
        {
            var listResponsesResult = _responseSvc.ListResponsesForRespondent(sessionKey, CacheContext);

            if (!listResponsesResult.CallSuccess)
            {
                throw new Exception("Unable to list responses for respondent.");
            }

            return listResponsesResult.ResultData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public bool GetMoreResponsesAllowed(Guid sessionKey)
        {
            var getMoreResult = _responseSvc.GetMoreResponsesAllowed(sessionKey, CacheContext);

            if (!getMoreResult.CallSuccess)
            {
                throw new Exception("Unable to determine whether to allow new responses.");
            }

            return getMoreResult.ResultData;
        }

        /// <summary>
        /// Get all items on a response page from response service.
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public IItemProxyObject[] GetPageItems(int pageId)
        {
            var getItemResult = _responseSvc.GetAllPageItems(_sessionData.SessionGuid, pageId, CacheContext);

            if (!getItemResult.CallSuccess)
            {
                ExceptionPolicy.HandleException(new Exception("Unable to load item data from response service: " + getItemResult.FailureMessage), "UIProcess");
            }

            return getItemResult.ResultData;
        }

        /// <summary>
        /// Get display flags
        /// </summary>
        /// <returns></returns>
        private ResponseViewDisplayFlags GetDisplayFlags(SurveyStyleSettings styleSettings, SurveyBehaviorSettings behaviorSettings, ResponseSessionState sessionState, bool isEdit)
        {
            var viewingSurvey =
                sessionState == ResponseSessionState.TakeSurvey
                || sessionState == ResponseSessionState.Completed;

            //If not taking survey, no response view display flags enabled
            if (!viewingSurvey)
            {
                return ResponseViewDisplayFlags.None;
            }

            //Get display flags based on survey settings.
            ResponseViewDisplayFlags displayFlags = styleSettings.GetDisplayFlags() | behaviorSettings.GetDisplayFlags();

            //Now adjust for current state of response (i.e. not on first page, etc.).
            if (sessionState == ResponseSessionState.Completed)
            {
                //Ensure back button not available
                //Set it first to ensure it is set, then "unset" with XOR. Cleanest way
                // since enum has "flags" attribute.
                displayFlags &= ~ResponseViewDisplayFlags.BackButton;
                displayFlags &= ~ResponseViewDisplayFlags.SaveButton;

                return displayFlags;
            }

            if (isEdit)
                displayFlags &= ~ResponseViewDisplayFlags.SaveButton;

            if (sessionState == ResponseSessionState.TakeSurvey)
            {
                displayFlags |= ResponseViewDisplayFlags.NextButton;
            }

            //For surveys w/only one survey page other than hidden/completion,
            // ensure buttons hidden
            if (CacheContext.ResponseTemplate.PageCount == 3)
            {
                displayFlags &= ~ResponseViewDisplayFlags.BackButton;
                displayFlags &= ~ResponseViewDisplayFlags.NextButton;
                displayFlags |= ResponseViewDisplayFlags.FinishButton;
            }


            return displayFlags;
        }

        /// <summary>
        /// Validate query parameters to ensure that either a survey id, invitation id, or 
        /// response id are present.
        /// </summary>
        private void ValidateQueryParameters()
        {
            //Load values
            _queryParameters.LoadValues();

            if (!_queryParameters.ValidateRequiredParameters())
            {
                throw new Exception("Survey URL appears to be invalid.  A valid URL will contain a survey id, invitation id, or response id.");
            }
        }

        /// <summary>
        /// Get key for current session.
        /// </summary>
        private ResponseSessionData GetSessionData()
        {
            ResponseSessionData sessionData = null;
            //Check viewstate on postback.  View state can be a little more robust since it could
            // survive a session expiration or server reboot, etc.
            if (Page.IsPostBack
                && ViewState[StoredResponseSessionDataKey] != null
                && ValidateSessionData((ResponseSessionData)ViewState[StoredResponseSessionDataKey]))
            {
                sessionData = (ResponseSessionData)ViewState[StoredResponseSessionDataKey];
            }

            //Otherwise fall back to session to hopefully prevent accidential restarting
            // if user re-selects survey url or survey returns from some external link
            // in a timely (i.e. before session expires) manner.
            else if (ApplicationManager.AppSettings.StoreSessionKeyInHttpSesssion
                     && Session[StoredResponseSessionDataKey] != null
                     && ValidateSessionData((ResponseSessionData) Session[StoredResponseSessionDataKey])
                )
            {
                sessionData = (ResponseSessionData) Session[StoredResponseSessionDataKey];

                //if Session's UID differs from InstanceGUID - update the SessionID
                if (_queryParameters.ResumeInstanceId.HasValue
                    && sessionData.SessionGuid != _queryParameters.ResumeInstanceId.Value
                    && sessionData.ResumeInstanceId == _queryParameters.ResumeInstanceId.Value)
                    sessionData.SessionGuid = _queryParameters.ResumeInstanceId.Value;

                ViewState[StoredResponseSessionDataKey] = sessionData;
            }
            else
            {
                //Create a session key instead
                sessionData = new ResponseSessionData
                {
                    SessionGuid = _queryParameters.ResumeInstanceId.HasValue
                                                  ? _queryParameters.ResumeInstanceId.Value
                                                  : Guid.NewGuid()
                };

                _queryParameters.InitializeResponseData(sessionData);

                //add client ip
                if (ApplicationManager.AppSettings.LogIpAddresses) sessionData.RespondentIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                ViewState[StoredResponseSessionDataKey] = sessionData;

                if (ApplicationManager.AppSettings.StoreSessionKeyInHttpSesssion && !User.Identity.IsAuthenticated)
                {
                    Session[StoredResponseSessionDataKey] = sessionData;
                }
            }

            //in case of lost session, lets try to restore it by iid
            if (sessionData.ResumeInstanceId.HasValue && Session[StoredResponseSessionDataKey] == null)
                sessionData.SessionGuid = sessionData.ResumeInstanceId.Value;

            return sessionData;
        }

        /// <summary>
        /// Get selected language, if any, from query string /session if template allows
        /// </summary>
        private string GetSelectedLanguage()
        {
            var selectedLanguage = string.Empty;

            if (CacheContext.ResponseTemplate.LanguageSettings.SupportedLanguages.Count < 2 || string.IsNullOrEmpty(CacheContext.ResponseTemplate.LanguageSettings.LanguageSourceToken))
            {
                return string.Empty;
            }

            if ("QueryString".Equals(CacheContext.ResponseTemplate.LanguageSettings.LanguageSource, StringComparison.InvariantCultureIgnoreCase))
            {
                selectedLanguage = Request.QueryString[CacheContext.ResponseTemplate.LanguageSettings.LanguageSourceToken];
            }

            if ("Session".Equals(CacheContext.ResponseTemplate.LanguageSettings.LanguageSource, StringComparison.InvariantCultureIgnoreCase))
            {
                selectedLanguage = (Session[CacheContext.ResponseTemplate.LanguageSettings.LanguageSourceToken] as string) ?? string.Empty;
            }

            bool browserDetection = false;
            if ("Browser".Equals(CacheContext.ResponseTemplate.LanguageSettings.LanguageSource, StringComparison.InvariantCultureIgnoreCase) && Request.UserLanguages != null)
            {
                browserDetection = true;
                selectedLanguage = Request.UserLanguages[0] ?? string.Empty;
            }

            if (CacheContext.ResponseTemplate.LanguageSettings.SupportedLanguages.Contains(selectedLanguage, StringComparer.InvariantCultureIgnoreCase))
            {
                return selectedLanguage;
            }
            
            if(browserDetection && "Default Language".Equals(CacheContext.ResponseTemplate.LanguageSettings.LanguageSourceToken, StringComparison.InvariantCultureIgnoreCase))
            {
                return CacheContext.ResponseTemplate.LanguageSettings.DefaultLanguage;
            }

            //tricky way to handle the case when user click Back at the browser and selects another language
            /*string lang = Request.Form["_responseView$_pageView$Layout_$_defaultZone$_languageSelect$_surveyLanguageList"];
            ResponseSessionData session = GetSessionData();
            if (session != null && session.SessionGuid != null &&                
                !string.IsNullOrEmpty(lang))
            {
                _responseSvc.SetLanguage(session.SessionGuid, lang, CacheContext);
                return lang;
            }*/

            return string.Empty;
        }

        /// <summary>
        /// Set the user context for reporting on admin screens
        /// </summary>
        protected override void SetUserContext()
        {
            if (_sessionData == null)
            {
                base.SetUserContext();
                return;
            }

            string userName;
            if (Utilities.IsNotNullOrEmpty(_sessionData.AuthenticatedRespondentUid))
            {
                userName = _sessionData.AuthenticatedRespondentUid;
            }
            else
            {
                var data = _responseSvc.GetResponseSessionData(_sessionData.SessionGuid, CacheContext, 
                    rGuid, rGuid != null);
                if (data.CallSuccess && !string.IsNullOrEmpty(data.ResultData.AuthenticatedRespondentUid))
                {
                    userName = data.ResultData.AuthenticatedRespondentUid;

                    //login the user to avoid piping issues
                    HttpContext.Current.User = UserManager.GetUserPrincipal(userName);
                }
                else
                    userName = string.Format("Anonymous [{0}]", _sessionData.AnonymousRespondentGuid);
            }

            //Set the caller's context
            UserManager.SetUserContext(
                userName,
                HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"],
                HttpContext.Current.Request.ServerVariables["URL"]);
        }

        /// <summary>
        /// Ensure session data matches query parameters
        /// </summary>
        /// <param name="sessionData"></param>
        /// <returns></returns>
        private bool ValidateSessionData(ResponseSessionData sessionData)
        {
            //Ensure values have not changed

            if (sessionData.SessionState == ResponseSessionState.Completed
                && _queryParameters.ExportMode == ExportMode.ClientPdf)
                return true;

            if (sessionData.SessionState == ResponseSessionState.None
                && _queryParameters.ExportMode == ExportMode.ClientPdf)
                return false;

            //If session data shows completed or not active states, invalidate session so user can return to new response,
            // select response to edit/resume, etc.
            if (sessionData.SessionState == ResponseSessionState.Completed
                || sessionData.SessionState == ResponseSessionState.SurveyNotActive
                || sessionData.SessionState == ResponseSessionState.BeforeStartDate
                || sessionData.SessionState == ResponseSessionState.AfterEndDate
                || sessionData.SessionState == ResponseSessionState.ResponseLimitReached
                || sessionData.SessionState == ResponseSessionState.InvitationRequired
                || sessionData.SessionState == ResponseSessionState.SurveyDeleted)
            {
                return false;
            }

            //Check survey guid
            if (sessionData.ResponseTemplateGuid.HasValue
                && _queryParameters.SurveyGuid.HasValue
                && sessionData.ResponseTemplateGuid != _queryParameters.SurveyGuid)
            {
                return false;
            }

            //Check response guid
            if (sessionData.ResponseGuid.HasValue
                && _queryParameters.ResponseGuid.HasValue
                && sessionData.ResponseGuid != _queryParameters.ResponseGuid)
            {
                return false;
            }

            //Check resume guid
            // a) Check to see if guids differ
            if (sessionData.ResumeInstanceId.HasValue
                && _queryParameters.ResumeInstanceId.HasValue
                && sessionData.ResumeInstanceId != _queryParameters.ResumeInstanceId)
            {
                return false;
            }

            // b) Check to see if guid is now in query string but not in session data, indicating
            //    respondent selected response to resume from list
            if (!sessionData.ResumeInstanceId.HasValue
                && _queryParameters.ResumeInstanceId.HasValue)
            {
                return false;
            }

            //Test Mode
            if (sessionData.IsTest && !_queryParameters.IsTest)
            {
                return false;
            }

            //Edit Mode
            if (!IsPostBack &&  _queryParameters.IsEdit)
            {
                return false;
            }

            //Forcing new
            //check also for previously sent invitation
            if (_queryParameters.InvitationGuid != sessionData.InvitationRecipientGuid)
            {
                return false;
            }

            //Forcing new
            if (!IsPostBack && _queryParameters.ForceNew)
            {
                return false;
            }

            if (!IsPostBack) //postback here may occur when a user tries to send a reminder email
            {
                ResponseSessionData fromSession = Session[StoredResponseSessionDataKey] as ResponseSessionData;
                if (fromSession != null)
                {
                    //the case when the session contains old response whereas the user decides to open responses list or create a new response
                    if ((fromSession.ResumeInstanceId.HasValue
                            || fromSession.SessionState == ResponseSessionState.SavedProgress)
                            && !_queryParameters.ResumeInstanceId.HasValue
                            )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private ResponseSessionState GetCurrentSessionState()
        {
            //Get current session state.
            var getStateResult = _responseSvc.GetCurrentSessionState(_sessionData.SessionGuid, CacheContext);

            if (!getStateResult.CallSuccess)
            {
                throw new Exception("Unable to get response session state: " + getStateResult.FailureMessage);
            }

            //If current state is ResponseSaved and we're not posting back, then resume saved response
            if (!IsPostBack && (getStateResult.ResultData == ResponseSessionState.SavedProgress 
                || getStateResult.ResultData == ResponseSessionState.EditResponse
                ) && _sessionData.ResumeInstanceId.HasValue)
            {
                if (ApplicationManager.AppSettings.SessionMode == AppSettings.SessionType.Cookieless)
                {
                    var result = _responseSvc.GetResponseSessionData(_sessionData.SessionGuid, CacheContext);

                    if (!result.CallSuccess)
                    {
                        throw new Exception("Unable to get response session state: " + result.FailureMessage);
                    }

                    if (result.ResultData != null)
                    {
                        _sessionData = result.ResultData;
                        ViewState[StoredResponseSessionDataKey] = _sessionData;
                    }
                }

                var resumeResult = _responseSvc.ResumeSavedResponse(_sessionData.SessionGuid, CacheContext);

                if (!resumeResult.CallSuccess)
                {
                    throw new Exception("Unable to resume response: " + getStateResult.FailureMessage);
                }

                _sessionData.SessionState = resumeResult.ResultData;
            }
            else
            {
                _sessionData.SessionState = getStateResult.ResultData;
            }

            return _sessionData.SessionState;
        }

        #region Event Handlers

        /// <summary>
        /// Handle user login.
        /// </summary>
        private void UserLoggedIn()
        {
            try
            {
                string loggedInUser = string.Empty;
                InitializeAndBindResponseView(_sessionData.SessionState);
                if (_sessionData.SessionState == ResponseSessionState.LoginRequired)
                {
                    SetUserContext();

                    loggedInUser = _responseView.GetAuthenticatedUserName();
                    if (!string.IsNullOrEmpty(loggedInUser))
                    {
                        var setRespondentResult = _responseSvc.SetAuthenticatedRespondent(_sessionData.SessionGuid,
                                                                                          loggedInUser, CacheContext);
                        if (!setRespondentResult.CallSuccess)
                        {
                            throw new Exception("Unable to set authenticated respondent: " +
                                                setRespondentResult.FailureMessage);
                        }

                        _sessionData.SessionState = setRespondentResult.ResultData;
                        _sessionData.AuthenticatedRespondentUid = loggedInUser;
                    }
                }

                InitializeAndBindResponseView(_sessionData.SessionState);

                PersistSessionData();

                if (!string.IsNullOrEmpty(loggedInUser))
                    FormsAuthentication.SetAuthCookie(loggedInUser, false);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(
                    WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage, "An error occurred. Unable to continue."),
                    ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PasswordEntered()
        {
            try
            {
                if (_sessionData.SessionState == ResponseSessionState.EnterPassword)
                {
                    string password = _responseView.GetPasswordEntered();

                    _sessionData.EnteredPassword = password;

                    var setPasswordResult = _responseSvc.SetPassword(_sessionData.SessionGuid, password, CacheContext);

                    if (!setPasswordResult.CallSuccess)
                    {
                        throw new Exception("Unable to set respondent password: " + setPasswordResult.FailureMessage);
                    }

                    _sessionData.SessionState = setPasswordResult.ResultData;

                    Session["EnteredPassword"] = _sessionData.SessionState != ResponseSessionState.EnterPassword
                                                     ? password
                                                     : null;
                }

                InitializeAndBindResponseView(_sessionData.SessionState);

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred",
                                                                    TextManager.DefaultLanguage,
                                                                    "An error occurred. Unable to continue."),
                                                                    ex.Message);
            }
        }

        /// <summary>
        /// Handle user language selected
        /// </summary>
        private void LanguageSelected()
        {
            try
            {
                if (_sessionData.SessionState == ResponseSessionState.SelectLanguage)
                {
                    string language = _responseView.GetSelectedLanguage();

                    _sessionData.SelectedLanguage = language;

                    var setLanguageResult = _responseSvc.SetLanguage(_sessionData.SessionGuid, language, CacheContext);

                    if (!setLanguageResult.CallSuccess)
                    {
                        throw new Exception("Unable to set authenticated respondent: " +
                                            setLanguageResult.FailureMessage);
                    }

                    _sessionData.SessionState = setLanguageResult.ResultData;
                }

                InitializeAndBindResponseView(_sessionData.SessionState);

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred",
                                                                    TextManager.DefaultLanguage,
                                                                    "An error occurred. Unable to continue."),
                                                                    ex.Message);
            }
        }


        /// <summary>
        /// Move next
        /// </summary>
        private void MoveNext()
        {
            try
            {
                //if (_queryParameters.ExportMode == ExportMode.Pdf)
                //{
                //    return;
                //}

                if (_sessionData.SessionState == ResponseSessionState.TakeSurvey)
                {
                    UpdateAndPostCurrentPageItems();

                    //Move next
                    //      TODO: Handle validation result for alert or other display
                    var moveResult = _responseSvc.MoveToNextPage(_sessionData.SessionGuid, CacheContext);

                    if (!moveResult.CallSuccess)
                    {
                        throw new Exception("An error occurred submitting response page: " + moveResult.FailureMessage);
                    }

                    //Use new session state
                    _sessionData.SessionState = moveResult.ResultData.NewSessionState;

                    if (moveResult.ResultData != null)
                        _hasSPC = moveResult.ResultData.HasSPC;
                }

                Session[_pageItemsUpdateState] = true;
                InitializeAndBindResponseView(_sessionData.SessionState);
                Session[_pageItemsUpdateState] = false;

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred",
                                                                    TextManager.DefaultLanguage,
                                                                    "An error occurred. Unable to continue."),
                                                                    ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Finish()
        {
            try
            {
                MoveNext();

                if (ApplicationManager.AppSettings.IsPrepMode && !string.IsNullOrEmpty(_queryParameters.RestoreUser))
                {
                    UserManager.ExpireLoggedInUser(HttpContext.Current.User.Identity.Name);
                    ReloginUserAs(_queryParameters.RestoreUser);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(
                    WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage,
                                           "An error occurred. Unable to continue."),
                                            ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshSPC()
        {
            try
            {
                UpdateAndPostCurrentPageItems();

                var moveResult = _responseSvc.UpdateConditions(_sessionData.SessionGuid, CacheContext);

                if (!moveResult.CallSuccess)
                {
                    throw new Exception("An error occurred submitting response page: " + moveResult.FailureMessage);
                }

                InitializeAndBindResponseView(ResponseSessionState.TakeSurvey);

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(
                    WebTextManager.GetText("/errorMessages/common/errorOccurred", TextManager.DefaultLanguage,
                                           "An error occurred. Unable to continue."),
                                            ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendResumeLinkByEmail()
        {
            _responseView.SendResumeEmail();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResponseViewSaveProgress()
        {
            try
            {
                if (_sessionData.SessionState == ResponseSessionState.TakeSurvey)
                {
                    UpdateAndPostCurrentPageItems();

                    var saveResult = _responseSvc.SaveProgress(_sessionData.SessionGuid, CacheContext);

                    if (!saveResult.CallSuccess)
                    {
                        throw new Exception("An error occurred saving response progress: " + saveResult.FailureMessage);
                    }

                    //Use new session state
                    _sessionData.SessionState = saveResult.ResultData.NewSessionState;
                }

                InitializeAndBindResponseView(_sessionData.SessionState);

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred",
                                                                    TextManager.DefaultLanguage,
                                                                    "An error occurred. Unable to continue."), ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MovePrevious()
        {
            try
            {
                //if (_queryParameters.ExportMode == ExportMode.Pdf)
                //{
                //    return;
                //}
             
                var sessionState = _sessionData.SessionState;

                if (sessionState == ResponseSessionState.TakeSurvey)
                {
                    if (ApplicationManager.AppSettings.SavePartialResponsesOnBackNavigation)
                    {
                        UpdateAndPostCurrentPageItems();
                    }

                    var moveResult = _responseSvc.MoveToPreviousPage(_sessionData.SessionGuid, CacheContext);

                    if (!moveResult.CallSuccess)
                    {
                        throw new Exception("An error occurred submitting response page: " + moveResult.FailureMessage);
                    }

                    //Use new session state
                    _sessionData.SessionState = moveResult.ResultData.NewSessionState;

                    if (moveResult.ResultData != null)
                        _hasSPC = moveResult.ResultData.HasSPC;
                }

                InitializeAndBindResponseView(sessionState);

                PersistSessionData();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _responseView.ShowFatalError(WebTextManager.GetText("/errorMessages/common/errorOccurred",
                                                                    TextManager.DefaultLanguage,
                                                                    "An error occurred. Unable to continue."), ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EmbedResetButtonScript()
        {
            RegisterClientScriptInclude("ResetButton", ResolveUrl("~/Resources/resetButtonFunctionality.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ResetButtonBindingBlock", "<script language=\"javascript\">$(document).ready(function(){bindResetButton('.reset-form-button', " +
                IsBrowserMobile.ToString().ToLower() + ");});</script>");
        }

        /// <summary>
        /// Post current page to server.
        /// </summary>
        private void UpdateAndPostCurrentPageItems()
        {
                //Update page
                _responseView.UpdatePage();

                //Post items
                _responseSvc
                    .PostResponseItems(
                        _sessionData.SessionGuid,
                        _responseView
                            .ListPageItems().OfType<SurveyResponseItem>()
                            .ToArray(),
                        CacheContext
                    );
        }


        #endregion
    }
}
