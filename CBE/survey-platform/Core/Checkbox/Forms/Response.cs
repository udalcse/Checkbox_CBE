using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Piping;
using Checkbox.Forms.Piping.Tokens;
using Checkbox.Forms.Security.Principal;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// The live Response, holds the state.
    /// </summary>
    [Serializable]
    public class Response : IDisposable, IXmlSerializable
    {
        /// <summary>
        /// Event fired when the response state is restored from persistent storage
        /// </summary>
        public event StateRestoredHandler StateRestored;

        /// <summary>
        /// Delegate for response state restored events.
        /// </summary>
        /// <param name="sender">Response object firing the event.</param>
        /// <param name="e">Event arguments.</param>
        public delegate void StateRestoredHandler(object sender, ResponseStateEventArgs e);

        /// <summary>
        /// Event fired when response is copmleted.
        /// </summary>
        public event ResponseCompletedHandler ResponseCompleted;

        /// <summary>
        /// Delegate for response completed events.
        /// </summary>
        /// <param name="sender">Response that is complete.</param>
        /// <param name="e">Event arguments.</param>
        public delegate void ResponseCompletedHandler(object sender, ResponseStateEventArgs e);

        /// <summary>
        /// Event fired when response is saved but not completed.
        /// </summary>
        public event ResponseSavedHandler ResponseSaved;

        /// <summary>
        /// Delegate for response save events (without completion).
        /// </summary>
        /// <param name="sender">Response that is incomplete but saved.</param>
        /// <param name="e">Event arguments.</param>
        public delegate void ResponseSavedHandler(object sender, ResponseStateEventArgs e);

        /// <summary>
        /// Delegate for response page changed events.
        /// </summary>
        /// <param name="sender">Response moving to a new page.</param>
        /// <param name="e">Event arguments.</param>
        public delegate void ResponsePageChangedHandler(object sender, ResponsePageChangedEventArgs e);

        /// <summary>
        /// Event fired when current page index of a response changes.
        /// </summary>
        public event ResponsePageChangedHandler PageChanged;

        // the state
        private ResponseState _responseState;

        // rules
        private RulesEngine _rulesEngine;

        //Respondent
        private CheckboxPrincipal _respondent;

        // page log
        private readonly Stack<ResponsePage> _visitedPages;

        private readonly Dictionary<int, Item> _items;
        private readonly List<ResponsePage> _pages;
        private readonly List<ItemToken> _responsePipes;
        private readonly AppSettings _appSettings;
        private readonly Dictionary<int, int> _dynamicItemNumbers;
        private readonly Dictionary<int, int> _staticItemNumbers;

        private readonly PipeMediator _pipeMediator;

        private readonly List<int> _preloadedPageIds = new List<int>();

        //Navigation
        private int? _nextPagePump;

        public Response(CachableResponse r)
        {
            _appSettings = new AppSettings();
            _nextPagePump = r.NextPagePump;
            _pipeMediator = r.PipeMediator ?? new PipeMediator();
            _respondent = r.Respondent;
            _rulesEngine = r.RulesEngine;
            _responseState = r.ResponseState;            
            _visitedPages = new Stack<ResponsePage>();
            _responsePipes = r.ResponsePipes;
            CurrentPageIndex = r.CurrentPageIndex;
            
            _items = new Dictionary<int, Item>();
            _pages = new List<ResponsePage>();
            _dynamicItemNumbers = new Dictionary<int, int>();
            _staticItemNumbers = new Dictionary<int, int>();
        }

        /// <summary>
        /// Inits visible pages list from cacheable object
        /// </summary>
        /// <param name="r"></param>
        public void InitVisiblePages(CachableResponse r)
        {
            foreach (var p in r.VisitedPages)
            {
                ResponsePage rp = GetPage(p);
                if (rp != null)
                    _visitedPages.Push(rp);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CachableResponse GetCachable()
        {            
            return new CachableResponse { ResponseState = _responseState, RulesEngine = RulesEngine, Respondent = _respondent,
                                            VisitedPages = _visitedPages.Where(vp => vp != null).OrderBy(p => p.Position).Select(p => p.PageId).ToArray(),
                                            ResponsePipes = _responsePipes,
                                            PipeMediator = _pipeMediator,
                                            NextPagePump = _nextPagePump,
                                            CurrentPageIndex = CurrentPageIndex,
                                            LanguageCode = LanguageCode
            };
        }

        //Text

        //Scoring

        /// <summary>
        /// Construct a response object and initialize internal collections.
        /// </summary>
        public Response()
        {
            _items = new Dictionary<int, Item>();
            _pages = new List<ResponsePage>();
            _visitedPages = new Stack<ResponsePage>();
            _responseState = new ResponseState();
            _responsePipes = new List<ItemToken>();
            _pipeMediator = new PipeMediator();
            _appSettings = new AppSettings();
            _dynamicItemNumbers = new Dictionary<int, int>();
            _staticItemNumbers = new Dictionary<int, int>();
        }

        /// <summary>
        /// Initialize the response
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="networkUser"></param>
        /// <param name="languageCode"></param>
        /// <param name="isTest"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="invitee"></param>
        /// <param name="respondent">Principal associated with the response.</param>
        /// <param name="startDate"></param>
        public void Initialize(string ipAddress, string networkUser, string languageCode, bool isTest, string invitee, CheckboxPrincipal respondent, Guid? sessionGuid, DateTime? startDate = null)
        {
            Guid? respondentGuid = null;

            if (respondent is AnonymousRespondent)
            {
                respondentGuid = respondent.UserGuid;
            }

            _respondent = respondent;

            if (!ApplicationManager.AppSettings.LogNetworkUser)
            {
                networkUser = string.Empty;
            }

            _visitedPages.Clear();

            if (!_appSettings.IsPrepMode)
            {
                _responseState.InsertResponseData(
                    TemporaryGUID ?? Guid.NewGuid(),
                    ResponseTemplateID,
                    Pages[0].PageId,
                    ipAddress,
                    respondent.Identity.Name,
                    networkUser,
                    languageCode,
                    respondentGuid,
                    isTest,
                    AnonymizeResponses,
                    invitee,
                    sessionGuid,
                    startDate);

                _responseState.Save();
            }

            TemporaryGUID = null;

            Invitee = invitee;

            //Set the state for child items
            foreach (AnswerableItem item in Items.Values.OfType<AnswerableItem>())
            {
                item.AnswerData = State;
            }

            //Initialize pipe mediator.  Do this after answer data is set so any 
            // pipe initialization that relies on answer state can happen.  Also make sure this
            // happens before running rules, since running rules calls page onload at which point
            // items may need their text.
            _pipeMediator.Initialize(this, respondent);

            //Run all page rules so that conditional items can retain their proper visible/hidden states
            RunAllRules();

            //Generate item numbers
            GenerateItemNumbers();

            if (CurrentPage != null)
            {
                _visitedPages.Push(CurrentPage);
                if (!_appSettings.IsPrepMode)
                    State.PushPageLog(CurrentPage.PageId);
            }

            //Fire the restored event
            if (StateRestored != null)
            {
                StateRestored(this, new ResponseStateEventArgs(_responseState));
            }
        }

        

        /// <summary>
        /// Sets default values for items
        /// </summary>
        public void InitializeItemsDefaults()
        {
            foreach (Item i in Items.Values)
            {
                i.InitializeDefaults();
            }
        }

        /// <summary>
        /// Get/set whether the back button is allowed
        /// </summary>
        internal bool AllowBack { get; set; }

        /// <summary>
        /// Gets/Sets a flag indicating whether an authenticated Respondent's user information is recorded
        /// </summary>
        internal bool AnonymizeResponses { get; set; }

        /// <summary>
        /// Get/set whether to show item numbers or not
        /// </summary>
        public bool ShowItemNumbers { get; set; }

        /// <summary>
        /// Get/set whether to calculate item numbers
        /// </summary>
        internal bool UseDynamicItemNumbers { get; set; }

        /// <summary>
        /// Get/set whether to calculate page numbers
        /// </summary>
        internal bool UseDynamicPageNumbers { get; set; }

        /// <summary>
        /// Gets the database PK ID for this Response
        /// </summary>
        public Int64? ID
        {
            get { return State.ResponseID; }
        }


        /// <summary>
        /// Public accessor for response template guid
        /// </summary>
        public Guid ResponseTemplateGuid { get; internal set; }

        /// <summary>
        /// Get the response template id
        /// </summary>
        public Int32 ResponseTemplateID { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Invitee { get; private set; }

        /// <summary>
        /// Gets a <see cref="Guid"/> identifying this Response
        /// </summary>
        public Guid? GUID
        {
            get { return State.Guid; }
        }

        /// <summary>
        ///Use that for temporary GUID, before it take its own from the database
        /// </summary>
        public Guid? TemporaryGUID { get; set; }

        /// <summary>
        /// Get Id of associated workflow instance.  This is used to reload workflow
        /// from persistent store.  Responses started before 5.0 will not have session
        /// id values.
        /// </summary>
        public Guid? WorkflowSessionId { get { return State.SessionId; } }

        /// <summary>
        /// Gets the date on which this Response was created
        /// </summary>
        public DateTime? DateCreated
        {
            get { return State.DateCreated; }
        }

        /// <summary>
        /// Get the date the response was completed.
        /// </summary>
        public DateTime? DateCompleted
        {
            get { return State.DateCompleted; }
        }

        /// <summary>
        /// Gets the date on which this Response was last modified
        /// </summary>
        public DateTime? LastModified
        {
            get { return State.LastModified; }
        }

        /// <summary>
        /// Get the current status of the response.
        /// </summary>
        /// <remarks>Always returns ResponseStatus.INPROGRESS</remarks>
        public ResponseStatus Status
        {
            get { return ResponseStatus.INPROGRESS; }
        }

        /// <summary>
        /// Get a reference to the pipe mediator object used by this response.
        /// </summary>
        public PipeMediator PipeMediator
        {
            get { return _pipeMediator; }
        }

        /// <summary>
        /// Get the IP addresss of the current respondent.
        /// </summary>
        public string IPAddress
        {
            get { return State.IpAddress; }
        }

        /// <summary>
        /// Get the network/AD name for the current respondent.
        /// </summary>
        public string NetworkUser
        {
            get { return State.NetworkUser; }
        }

        /// <summary>
        /// Get the Checkbox unique identifier of the current respondent.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return State.UniqueIdentifier; }
        }

        /// <summary>
        /// Get the ID of the last page viewed of the survey.
        /// </summary>
        public Int32? LastPageViewed
        {
            get { return State.LastPageViewed; }
        }

        /// <summary>
        /// Get/set the current language code for the response.
        /// </summary>
        public string LanguageCode
        {
            get { return State.LanguageCode; }
            set 
            {
                foreach (var item in Items.Values)
                {
                    item.LanguageCode = value;
                }
                State.LanguageCode = value; 
            }
        }

        /// <summary>
        /// Gets the <see cref="ResponseState"/> for the current Response
        /// </summary>
        private ResponseState State
        {
            get { return _responseState; }
        }

        /// <summary>
        /// Get a boolean indicating if the current response is "Complete" or not.
        /// </summary>
        public bool Completed
        {
            get { return State.IsComplete; }
        }

        /// <summary>
        /// Get a boolean indicating if the current response is "Test" or not.
        /// </summary>
        public bool IsTest
        {
            get { return State.IsTest; }
        }

        /// <summary>
        /// Gets a flag indicating if scoring is enabled for this response
        /// </summary>
        public bool ScoringEnabled { get; internal set; }

        /// <summary>
        /// Get a boolean value indicating that the response has a valid, initialized state
        /// </summary>
        public bool StateIsValid { get { return State != null; } }

        /// <summary>
        /// The total score for the survey, null if the survey is not scored.
        /// </summary>
        public double? TotalScore
        {
            get
            {
                if (!ScoringEnabled)
                {
                    return null;
                }

                return _pages.SelectMany(page => page.GetItems()).OfType<IScored>().Sum(item => item.GetScore());
            }
        }


        /// <summary>
        /// Adds the ignore condtion types.
        /// </summary>
        /// <param name="types">The types.</param>
        public void AddIgnoreCondtionTypes(List<string> types)
        {
            _rulesEngine.AddIgnoreRuleTypes(types);
        }

        /// <summary>
        /// Adds the ignore condtion types.
        /// </summary>
        public void СlearIgnoreConditionTypes()
        {
            _rulesEngine.ClearIgnoreTypeRules();
        }

        

        ///<summary>
        /// Removes answer data from specified item
        /// </summary>
        ///<param name="itemID"></param>
        public void DeleteAnswerDataForItem(int itemID)
        {
            State.DeleteAllAnswersForItem(itemID);
        }

        /// <summary>
        /// Overloaded.  Restores the <see cref="ResponseState"/> of an existing Response
        /// </summary>
        /// <param name="responseGuid">the globally unique identifier for the Response</param>
        public void Restore(Guid responseGuid)
        {
            Restore(responseGuid, null, false, false);
        }

        /// <summary>
        /// Overloaded.  Restores the <see cref="ResponseState"/> of an existing Response
        /// </summary>
        /// <param name="state">the <see cref="ResponseState"/> of the Response</param>
        /// <param name="isStarted"></param>
        public void Restore(ResponseState state, bool isStarted)
        {
            Restore(null, state, isStarted, true);
        }

        /// <summary>
        /// Overloaded.  Restores the <see cref="ResponseState"/> of an existing Response
        /// </summary>
        /// <param name="state">the <see cref="ResponseState"/> of the Response</param>
        public void RestoreForScoreCalculation(ResponseState state)
        {
            _responseState = state;

            if (_responseState.ResponseTemplateId != ResponseTemplateID)
                throw new InvalidOperationException("A ResponseTemplate mismatch was detected");

            RestoreVisitedPages();

            SetEmptyAnswersForSelectItemsWithNoOptionSelected(_responseState);

            //Set the state for child items
            Items.Values.OfType<AnswerableItem>().ToList().ForEach(i => i.AnswerData = State);

            //Run all page rules so that conditional items can retain their proper visible/hidden states
            RunAllRules();
        }

        /// <summary>
        /// Overloaded.  Restores the <see cref="ResponseState"/> of an existing Response
        /// </summary>
        /// <param name="responseGuid"></param>
        /// <param name="state">the <see cref="ResponseState"/> of the Response</param>
        /// <param name="isStarted"></param>
        /// <param name="loadAsync"></param>
        private void Restore(Guid? responseGuid, ResponseState state, bool isStarted, bool loadAsync)
        {
            if (state != null)
                _responseState = state;
            else if (responseGuid.HasValue)
                _responseState.Load(responseGuid.Value);

            if (_responseState.ResponseTemplateId != ResponseTemplateID)
                throw new InvalidOperationException("A ResponseTemplate mismatch was detected");

            RestoreVisitedPages();

            if (loadAsync)
                ReloadCurrentPage();

            if (responseGuid.HasValue)
            {
                SetEmptyAnswersForSelectItemsWithNoOptionSelected(_responseState);
                Invitee = State.Invitee;
            }
            else if (isStarted)
            {
                SetEmptyAnswersForSelectItemsWithNoOptionSelected(_responseState);
            }

            if (loadAsync)
            {
                //preload all piped items
                foreach (var page in _visitedPages)
                {
                    if (page.ItemIDs.Any(id => ResponsePipes.Any(p => p.ItemID == id)))
                        ReloadPageItems(page);
                }
            }

            //Set the state for child items
            foreach (AnswerableItem item in Items.Values.OfType<AnswerableItem>())
            {
                item.AnswerData = State;
            }

            //Initialize pipe mediator.  Do this after answer data is set so any 
            // pipe initialization that relies on answer state can happen.  Also make sure this
            // happens before running rules, since running rules calls page onload at which point
            // items may need their text.
            _pipeMediator.Initialize(this, Respondent);

            //Run all page rules so that conditional items can retain their proper visible/hidden states
            RunAllRules();

            //Generate item numbers
            GenerateItemNumbers();

            //Fire the restored event
            if (StateRestored != null)
            {
                StateRestored(this, new ResponseStateEventArgs(_responseState));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        public void ReloadCurrentPage(ResponseTemplate rt = null)
        {
            if (rt == null)
                rt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateID);

            ReloadPageItems(CurrentPage, rt);

            //load all pages which have source items for the current page or its items
            var conditionSourcePageIds = RulesEngine.ListSourcePageIdsBoundOnSpecifiedPageConditions(CurrentPage, rt);
            foreach (var pageId in conditionSourcePageIds)
            {
                var page = _pages.FirstOrDefault(p => p.PageId == pageId);
                if (page != null)
                {
                    ReloadPageItems(page, rt);
                    if (!_preloadedPageIds.Contains(pageId))
                        _preloadedPageIds.Add(pageId);
                }
            }

            LoadCurrentPage();

            if (CurrentPage.Excluded || CurrentPage.ExcludedNoItems)
                return;

            //we have to preload all previous pages to count scoring
            if (rt.BehaviorSettings.EnableScoring)
            {
                foreach (var page in _visitedPages.ToArray())
                {
                    if (!_preloadedPageIds.Contains(page.PageId))
                    {
                        ReloadPageItems(page, rt);
                        _preloadedPageIds.Add(page.PageId);
                    }
                }
            }

            InitializeSPCItemDependenciesForPage(CurrentPage);

            /*
            if (CurrentPage.PageType != TemplatePageType.HiddenItems)
            {
                var pageIdsToPreload = new List<int>();

                //preload next page
                if (Pages.Count > CurrentPageIndex + 1)
                    pageIdsToPreload.Add(Pages[CurrentPageIndex + 1].PageId);

                //preload previos page if we restore the response and go back
                if (_visitedPages.Count > 1)
                {
                    var prevPage = _visitedPages.ToArray()[_visitedPages.Count - 2];
                    pageIdsToPreload.Add(prevPage.PageId);
                }

                //preload all pages which could be jumped on by branching
                pageIdsToPreload.AddRange(RulesEngine.ListTargetPageIdsBoundOnSpecifiedPageBranching(CurrentPage, rt));

                var notPreloaded = pageIdsToPreload.Where(p => !_preloadedPageIds.Contains(p)).ToList();
                _preloadedPageIds.AddRange(notPreloaded);

                //load pages async
                foreach (var pageId in notPreloaded)
                {
                    LoadPageAsync(pageId, rt);
                }
            }*/
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="rt"></param>
        private void LoadPageAsync(int pageId, ResponseTemplate rt)
        {
            var page = _pages.FirstOrDefault(p => p.PageId == pageId);
            if (page != null && GUID.HasValue)
                rt.LoadItemsAsync(page.ItemIDs.ToArray(), LanguageCode, GUID.Value);
        }
        
        /// <summary>
        /// For restored select items with no options selected it's neccessary to set 'empty' answers for response state,
        /// as it causes to reset unchecked options to their defaults
        /// </summary>
        private void SetEmptyAnswersForSelectItemsWithNoOptionSelected(ResponseState state)
        {
            foreach (var item in _items)
            {
                var select = item.Value as SelectItem;
                var slider = item.Value as Slider;

                if (select != null &&
                    (slider == null || slider.ValueType != SliderValueType.NumberRange) &&
                    !state.GetOptionAnswersForItem(select.ID).Any())
                    state.SetEmptyAnswerForItem(select.ID);
            }
        }

        /// <summary>
        /// Get the respondent for the survey
        /// </summary>
        public CheckboxPrincipal Respondent
        {
            get
            {
                if (_respondent == null)
                {
                    //Get the respondent
                    if (RespondentGuid.HasValue)
                    {
                        _respondent = new AnonymousRespondent(RespondentGuid.Value);
                    }
                    else if (UniqueIdentifier != null)
                    {
                        _respondent = UserManager.GetUserPrincipal(UniqueIdentifier);
                    }
                }

                return _respondent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RestoreVisitedPages()
        {
            //Build the visited page stack
            int[] pageIDs = State.VisitedPages;

            _visitedPages.Clear();

            foreach (int t in pageIDs)
            {
                ResponsePage visitedPage = FindPage(t);

                if (visitedPage != null)
                {
                    _visitedPages.Push(visitedPage);
                }
            }

            //Set current page to first page by default, then try to load from response state
            CurrentPageIndex = 0;

            //Set the index of the last page viewed
            if (State.LastPageViewed.HasValue)
            {
                ResponsePage lastPageViewed = FindPage(State.LastPageViewed.Value);

                if (lastPageViewed != null)
                {
                    CurrentPageIndex = Pages.IndexOf(lastPageViewed);
                }
            }
        }

        /// <summary>
        /// Find a page with the specified page id.  Returns null if the page is not found.
        /// </summary>
        /// <param name="pageID">ID of the page to find.</param>
        /// <returns>NULL if page not found, otherwise return the page.</returns>
        private ResponsePage FindPage(int pageID)
        {
            return Pages.FirstOrDefault(p => p.PageId == pageID);
        }

        /// <summary>
        /// Gets the <see cref="Page"/> to which the Response is currently pointed
        /// </summary>
        public ResponsePage CurrentPage
        {
            get { return Pages[CurrentPageIndex]; }
        }

        public void SetPage(int? pageId)
        {
            var page = Pages.FirstOrDefault(item => item.PageId == pageId);
            CurrentPageIndex = Pages.IndexOf(page);
            ReloadCurrentPage();

        }

        /// <summary>
        /// Gets the index of the <see cref="Page"/> to which the Response is currently pointed
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// Get the count of pages.  Only include content pages
        /// </summary>
        public Int32 PageCount
        {
            get { return Pages.Count(page => page.PageType == TemplatePageType.ContentPage); }
        }

        /// <summary>
        /// Get the count of un-excluded pages. Only include content pages
        /// </summary>
        public Int32 EnabledPageCount
        {
            get { return Pages.Count(page => page.PageType == TemplatePageType.ContentPage && !page.Excluded); }
        }

        /// <summary>
        /// Get a count of visited pages
        /// </summary>
        public Int32 VisitedPageCount
        {
            get { return _visitedPages.Count(page => page.PageType == TemplatePageType.ContentPage && !page.Excluded); }
        }

        /// <summary>
        /// Move to the start of the survey
        /// </summary>
        public void MoveToStart()
        {
            _visitedPages.Clear();
            State.ClearPageLog();
            CurrentPageIndex = 0;

            //Reset the last page viewed
            State.LastPageViewed = CurrentPage.PageId;

            //Save the state
            State.Save();
        }

        /// <summary>
        /// Move to a specific page in the response by calling MoveNext() or MovePrevious() as necessary.
        /// </summary>
        /// <param name="pageID"></param>
        public void MoveToPage(Int32 pageID)
        {
            //Find out where we need to go
            int pageIndex = -1;

            for (int i = 0; i < _pages.Count; i++)
            {
                if (_pages[i] != null && _pages[i].PageId == pageID)
                {
                    pageIndex = i;
                    break;
                }
            }

            //Keep track to make sure we don't loop infinitely
            int lastPageIndex = CurrentPageIndex;

            if (pageIndex == CurrentPageIndex)
            {
                return;
            }

            if (pageIndex > CurrentPageIndex)
            {
                while (CurrentPageIndex < pageIndex)
                {
                    MoveNext(false);

                    if (lastPageIndex == CurrentPageIndex)
                    {
                        return;
                    }

                    lastPageIndex = CurrentPageIndex;
                }
            }
            else
            {
                while (CurrentPageIndex > pageIndex)
                {
                    MovePrevious();

                    if (lastPageIndex == CurrentPageIndex)
                    {
                        return;
                    }

                    lastPageIndex = CurrentPageIndex;
                }
            }
        }

        /// <summary>
        /// Go directly to the completion page without calling MoveNext(), Finish()
        /// OnLoad() or other page-related events.
        /// </summary>
        public void GoToCompletionPage()
        {
            CurrentPageIndex = Pages.Count - 1;
        }

        /// <summary>
        /// Sets the index that will be moved to on the next call of MoveNext()
        /// </summary>
        /// <param name="pageIndex">the index of the page to move to</param>
        public void PrimeNextPage(int pageIndex)
        {
            _nextPagePump = pageIndex;
        }

        /// <summary>
        /// Persist the current state of the response
        /// </summary>
        public void SaveCurrentState()
        {
            //Set the modified date
            State.LastModified = DateTime.Now;

            //Save the state
            State.Save();
        }

        /// <summary>
        /// Save imported response
        /// </summary>
        public void SaveImportedState(DateTime started, DateTime completed, DateTime lastModified, bool isCompleted)
        {
            State.DateCompleted = completed;
            State.IsComplete = isCompleted;
            State.LastModified = lastModified;

            if (isCompleted && Pages.Count > 0)
                MoveToPage(Pages[Pages.Count - 1].PageId);

            //Save the state
            State.Save();
        }

        /// <summary>
        /// Validate current page and move next
        /// </summary>
        public void MoveNext()
        {
            MoveNext(true);
        }

        /// <summary>
        /// Advances the current Page to the next Page containing Enabled Items
        /// </summary>
        private void MoveNext(bool validateCurrentPage)
        {
            //Call onload to run rules
            CurrentPage.OnLoad(false);

            //Check if current page is valid
            if (validateCurrentPage && !CurrentPage.Valid)
            {
                return;
            }

            bool complete = false;
            Int32 currentPagePosition = Pages[CurrentPageIndex].Position;

            //Use some temp variables for readability
            bool morePages = (CurrentPageIndex < Pages.Count - 1);

            bool continueAdvancing = morePages;

            ResponseTemplate rt = null;
            if (continueAdvancing)
                rt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateID);

            //Advance to the next page, if necessary
            while (continueAdvancing)
            {
                CurrentPage.OnUnLoad();

                //The next page pump will be "primed" when a branch action has executed
                // and we are moving next.  In that case, skip to the page specified by
                // the branch action and mark all intervening pages as excluded.
                if (_nextPagePump != null)
                {
                    for (int pi = (CurrentPageIndex + 1); pi < _nextPagePump.Value; pi++)
                    {
                        Pages[pi].Excluded = true;
                    }

                    CurrentPageIndex = _nextPagePump.Value;

                    //Re-evaluate conditions on branch target page, which is necessary to handle cases
                    // when branching to a page that is also conditional and that condition is based
                    // on answer to question on page branched from.

                    //Run page rules only w/out firing events. Page eventing and item condition
                    // evaluation will occur below.
                    CurrentPage.RunRules(RuleEventTrigger.Load);

                    if (CurrentPage.Excluded)
                    {
                        for (int i = CurrentPageIndex + 1; i < Pages.Count; i++)
                        {
                            if (!Pages[i].Excluded)
                            {
                                CurrentPageIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                //Otherwise, increment the page index
                {
                    CurrentPageIndex++;
                }

                //if we move to the apge by branching, it could be unloaded 
                ReloadCurrentPage(rt);

                //if the page was excluded due to branching -- reinitialize defaults, because they were removed by DeleteAnswers
                if (CurrentPage.Excluded)
                {
                    foreach (var i in CurrentPage.GetItems())
                    {
                        i.InitializeDefaults();
                    }

                    LoadCurrentPage();
                }

                _nextPagePump = null;
                
                //Figure out if we should keep going
                morePages = (CurrentPageIndex < Pages.Count - 1);
                bool pageNotVisible = CurrentPage.Excluded || CurrentPage.GetItems().Count == 0;

                continueAdvancing = morePages && pageNotVisible;
            }

            //If the index of the next page is greater than the page count, that means we should consider the survey complete.
            if (CurrentPageIndex >= Pages.Count - 1)
            {
                complete = true;
                CurrentPageIndex = Pages.Count - 1;
            }

            State.LastModified = DateTime.Now;
            State.LastPageViewed = CurrentPage.PageId;

            //if it is prep mode we should not save response state
            if (!_appSettings.IsPrepMode)
                State.PushPageLog(CurrentPage.PageId);

            _visitedPages.Push(CurrentPage);

            //If the response is completed, update the state, save it, then fire events
            if (complete)
            {
                State.IsComplete = true;

                if (State.DateCompleted == null)
                {
                    State.DateCompleted = DateTime.Now;
                }
            }

            //Initialize the pipe mediator again to ensure that it has the current after postback state
            _pipeMediator.Initialize(this, Respondent);

            //Save the response state. if it is prep mode we should not save response state
            if (!_appSettings.IsPrepMode)
                State.Save();

            //Fire completed event after state is saved 1) to ensure that no completed processing
            // will prevent state from saving and 2) to ensure any post-completion processing that 
            // relies on the ckbx_Response.Completed field will have the correct values
            if (complete)
            {
                if (ResponseCompleted != null)
                {
                    ResponseCompleted(this, new ResponseStateEventArgs(State));
                }
            } else
            {
                if (ResponseSaved != null)
                {
                    ResponseSaved(this, new ResponseStateEventArgs(State));
                }
            }

            Int32 newPagePosition = CurrentPage.Position;

            if (PageChanged != null)
            {
                PageChanged(this, new ResponsePageChangedEventArgs(currentPagePosition, newPagePosition));
            }

            if (UseDynamicItemNumbers)
            {
                PopulateDynamicItemNumberDictionary();
            }

            if (rt != null)
            {
                SaveBindedUserFields(rt);
            }
        }

        private void SaveBindedUserFields(Template rt)
        {
            //only matrix items
            var items = rt.ListTemplateItemIds()
                .Select(rt.GetItem).Where(item => item.ItemTypeName.Equals("Matrix") || item.ItemTypeName.Equals("RadioButtons")).ToList();

            if (GUID != null)
            {
                foreach (var item in items)
                {
                    if (item.ID != null)
                        PropertyBindingManager.SaveBindedFieldResponse(item.ID.Value, (Guid) GUID, Respondent,
                            item.ItemTypeName);
                }
            }

        }

        /// <summary>
        /// Retreats the current Page to the last visited Page
        /// </summary>
        public void MovePrevious()
        {
            Int32 currentPagePosition = Pages[CurrentPageIndex].Position;

            //Go back to find the first non-excluded page with visible items, but don't go past
            // the beginning of the survey.
            do
            {
                //The current page is on the top of the stack, so pop it off and then get the 
                // index of the current page.
                ResponsePage page;
                do
                {
                    _visitedPages.Pop();
                    page = _visitedPages.Peek();
                } 
                while ((_visitedPages.Count > 0 && page == null));
                
                CurrentPageIndex = Pages.IndexOf(page);

                ReloadCurrentPage();

                //Call onload, but only to run rules
                if (!CurrentPage.Excluded)
                {
                    CurrentPage.OnLoad(true);
                }

                State.PopPageLog();
                State.LastPageViewed = CurrentPage.PageId;
                State.Save();

            } while ((!CurrentPageHasVisibleItems || CurrentPage.Excluded) && CanNavigateBack);

            Int32 newPagePosition = CurrentPage.Position;

            //If we went back as far back as we can go and nothing is still included/visible, then just stay on the current page
            if ((!CurrentPageHasVisibleItems || CurrentPage.Excluded) && !CanNavigateBack)
            {
                newPagePosition = currentPagePosition;
            }

            if (PageChanged != null)
            {
                PageChanged(this, new ResponsePageChangedEventArgs(currentPagePosition, newPagePosition));
            }

            if (UseDynamicItemNumbers)
            {
                PopulateDynamicItemNumberDictionary();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rt"></param>
        internal void ReloadPageItems(ResponsePage page, ResponseTemplate rt = null)
        {
            if (!page.ItemsLoaded)
            {
                if (rt == null)
                    rt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateID);

                foreach (var id in page.ItemIDs)
                {
                    var item = AddItem(id, rt);

                    //load answer data
                    var ai = item as AnswerableItem;
                    if (ai != null)
                    {
                        string otherText = string.Empty;
                        var si = item as SelectItem;
                        if (si != null)
                            otherText = si.OtherText;

                        ai.AnswerData = State;

                        if (si != null && otherText != null)
                            si.OtherText = otherText;
                    }
                }

                page.RebuildItemList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="rt"></param>
        private Item AddItem(int itemId, ResponseTemplate rt)
        {
            var item = rt.LoadItem(itemId, LanguageCode, GUID.Value);
            AddItem(item);
            return item;
        }

        /// <summary>
        /// Get a boolean indicating whether the current page has visible items
        /// </summary>
        private bool CurrentPageHasVisibleItems
        {
            get { return CurrentPage.GetItems().Any(item => item.Visible); }
        }

        /// <summary>
        /// Attempt to load the item order from the response state for a page.
        /// Empty list is returned if the state contains no item order information.
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public List<Int32> GetPageItemOrder(Int32 pageID)
        {
            return State.GetPageItemOrder(pageID);
        }

        /// <summary>
        /// Perisist the item order information to the response state.
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="itemID"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SavePageItemOrder(int pageID, int itemID, int position)
        {
            State.SavePageItemOrder(pageID, itemID, position);
        }

        /// <summary>
        /// Attempt to load the option order from the response state for an item.
        /// Empty list is returned if the state contains no item order information.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public List<Int32> GetItemOptionOrder(Int32 itemID)
        {
            return State.GetItemOptionOrder(itemID);
        }

        /// <summary>
        /// Perisist the item order information to the response state.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="optionID"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SaveItemOptionOrder(int itemID, int optionID, int position)
        {
            State.SaveItemOptionOrder(itemID, optionID, position);
        }


        /// <summary>
        /// Generate page item numbers
        /// </summary>
        public void GenerateItemNumbers()
        {
            PopulateDynamicItemNumberDictionary();
            PopulateStaticItemNumberDictionary();
        }

        /// <summary>
        /// Get the item number for an item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int? GetItemNumber(Int32 itemID)
        {
            if (UseDynamicItemNumbers)
            {
                if (_dynamicItemNumbers.ContainsKey(itemID))
                {
                    return _dynamicItemNumbers[itemID];
                }

                return null;
            }

            if (_staticItemNumbers.ContainsKey(itemID))
            {
                return _staticItemNumbers[itemID];
            }

            return null;
        }

        /// <summary>
        /// Get the current page number, taking into account any dynamic numbering options
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPageDisplayNumber()
        {
            if (UseDynamicPageNumbers)
            {
                return VisitedPageCount;
            }

            //check the page type
            if (CurrentPage.PageType == TemplatePageType.Completion || CurrentPage.PageType == TemplatePageType.HiddenItems)
                return -1;

            //Subtract 1, because in 5.0 page positions starts from 1
            return CurrentPage.Position - 1;
        }

        /// <summary>
        /// Get the total page count, taking into account any dynamic numbering options
        /// </summary>
        /// <returns></returns>
        public int GetPageCountDisplayNumber()
        {
            if (UseDynamicPageNumbers)
            {
                return EnabledPageCount;
            }

            return PageCount;
        }

        /// <summary>
        /// Calculate a number of the first item at the current page.
        /// </summary>
        /// <remarks>
        /// Only Visible, IAnswerable items are included in the count.
        /// </remarks>
        public int GetFirstItemNumber()
        {
            if (CurrentPage.PageType == TemplatePageType.Completion || CurrentPage.PageType == TemplatePageType.HiddenItems)
                return -1; //don't show any numbers

            int itemNumber = 1;

            var visited = VistedPageStack.ToArray().Reverse().ToArray();
            foreach (var page in visited)
            {
                ReloadPageItems(page);

                //for dynamic item numbers we need to run rules for each page
                if (UseDynamicItemNumbers)
                    page.OnLoad(false);
            }

            //Calculate the item numbers
            if (UseDynamicItemNumbers)
            {
                //Dyanamically calculate question number of first item on page.  Number is sum of number of previously shown
                // answerable items + 1, so we need to exclude items on conditionally hidden/skipped pages as well as individually skipped items
                // themselves.
                itemNumber +=
                    VistedPageStack
                        .Where(
                            page =>
                            page.PageType == TemplatePageType.ContentPage && page.Position < CurrentPage.Position &&
                            !page.Excluded)
                        .Sum(
                            page =>
                            page.GetItems().Count(item => !item.Excluded 
                                                     && item is IAnswerable 
                                                     && !"HiddenItem".Equals((string) item.ItemTypeName, StringComparison.InvariantCultureIgnoreCase)));
            }
            else
            {
                //Calculate number as sum of all previously shown answerable items + 1;
                itemNumber +=
                    Pages
                        .Where(
                            page =>
                            page.PageType == TemplatePageType.ContentPage && page.Position < CurrentPage.Position)
                        .Sum(
                            page =>
                            page.GetItems().Count(item => item is IAnswerable 
                                                     && !"HiddenItem".Equals((string) item.ItemTypeName, StringComparison.InvariantCultureIgnoreCase)));
            }

            return itemNumber;
        }        

        /// <summary>
        /// Calculate item numbers based on position in the survey
        /// </summary>
        /// <remarks>
        /// Only Visible, IAnswerable items are included in the count.
        /// </remarks>
        private void PopulateStaticItemNumberDictionary()
        {
            var itemNumber = 1;
            _staticItemNumbers.Clear();

            foreach (var item in Pages.Where(page => page.Position > 0).SelectMany(page => page.GetItems().Where(item => item is IAnswerable && !item.Excluded)))
            {
                _staticItemNumbers[item.ID] = itemNumber;
                itemNumber++;
            }
        }

        /// <summary>
        /// Calculate item numbers, based on visibility, etc.
        /// </summary>
        /// <remarks>
        /// Only Visible, IAnswerable items are included in the count.
        /// </remarks>
        private void PopulateDynamicItemNumberDictionary()
        {

            int itemNumber = 1;
            _dynamicItemNumbers.Clear();

            //Calculate the item numbers
            foreach (var item in from page in Pages where page.Position > 0 && !page.Excluded from item in page.GetItems() where item is IAnswerable && !item.Excluded select item)
            {
                _dynamicItemNumbers[item.ID] = itemNumber;
                itemNumber++;
            }
        }

        /// <summary>
        /// Run all page rules by calling pages OnLoad event
        /// </summary>
        private void RunAllRules()
        {
            
            foreach (ResponsePage page in Pages)
            {
                //Run rules for each page.  Normally, rules are only run when the page is loaded, 
                // but for the dynamic numbering to work, rules must always be run for each page
                page.OnLoad(CurrentPage != null && page.PageId == CurrentPage.PageId);
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RulesEngine"/> associated with this Response
        /// </summary>
        internal RulesEngine RulesEngine
        {
            get
            {
                if (_rulesEngine == null)
                {
                    throw new ApplicationException("RulesEngine was not initialized");
                }

                return _rulesEngine;
            }
            set { _rulesEngine = value; }
        }

        /// <summary>
        /// Gets a boolean flag indicating whether the Pages pointer can be moved forward
        /// </summary>
        public bool CanNavigateForward
        {
            get
            {
                if (Pages.IndexOf(CurrentPage) == (Pages.Count - 1))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a boolean flag indicating whether the Pages pointer can be moved back
        /// </summary>
        public bool CanNavigateBack
        {
            get
            {
                //Check of back button is enabled, survey is at start or survey is at end
                if (!AllowBack || CurrentPage.Position <= 1 || CurrentPageIndex == (Pages.Count - 1))
                {
                    return false;
                }

                //Next, make sure visited pages stack has pages other than the current page
                if (_visitedPages.Count == 0)
                {
                    return false;
                }

                //If we have only visited the current page, do not allow back button
                if (_visitedPages.Count == 1 && _visitedPages.Peek().Position == CurrentPage.Position)
                {
                    return false;
                }

                //Otherwise, allow back button if non-excluded pages are present before the current page
                for (int index = Pages.IndexOf(CurrentPage); index > 0; index--)
                {
                    if (!Pages[index - 1].Excluded && Pages[index - 1].Position > 0)
                    {
                        return true;
                    }
                }

                //No non-excluded pages found prior to this page, no back button allowed
                return false;
            }
        }

        /// <summary>
        /// Gets a boolean flag indicating whether a call to MoveNext() will complete the Response
        /// </summary>
        public bool CompleteOnNext
        {
            get
            {
                //Merely checking for included pages works, but does not account for the fact that
                // conditions based on answers to items on the current page can affect the value.  For example,
                // in a two page survey where the second page is visible only if a question on the first page is
                // answered, the finish button will be shown by default since in its initial state, the
                // condition is false. A user would answer the question, click "Finish" then see a second page
                // also with a finish button.  A more complicated check could be made, (i.e. evaluate all 
                // conditions except those depending on items on this page) but the amount of work
                // for the amount of gain would be minimal.
                int completionPagePosition = Pages[Pages.Count - 1].Position;

                //If the survey is 
                if (CurrentPage.Position < completionPagePosition - 1)
                {
                    return false;
                }

                return true;

                //if (Completed)
                //{
                //    return false;
                //}

                //// technically, the Response controller has to do work even if this returns true
                //// because it must still create renderers for invisible items
                //for (int index = Pages.IndexOf(CurrentPage); index < (Pages.Count - 2); index++)
                //{
                //    // if any subsequent page is included, return false
                //    if (!Pages[(index + 1)].Excluded)
                //        return false;
                //}
                //return true;
            }
        }

        /// <summary>
        /// Get a collection of visited pages.
        /// </summary>
        public ReadOnlyCollection<ResponsePage> VistedPageStack
        {
            get
            {
                return new ReadOnlyCollection<ResponsePage>(_visitedPages.ToArray());
            }
        }

        /// <summary>
        /// Get the items in the response.
        /// </summary>
        public Dictionary<int, Item> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Indicate if the response contains an item with this id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool ContainsItem(int itemId)
        {
            return Items.ContainsKey(itemId);
        }

        /// <summary>
        /// Add item to items collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            var responseItem = item as ResponseItem;
            if (responseItem != null)
            {
                responseItem.Response = this;
            }
            else
            {
                var analysisItem = item as AnalysisItem;
                if (analysisItem != null)
                    analysisItem.RunMode = true;
            }

            Items[item.ID] = item;
        }

        /// <summary>
        /// Add a page to a response
        /// </summary>
        /// <param name="responsePage"></param>
        public void AddPage(ResponsePage responsePage)
        {
            responsePage.Parent = this;
            Pages.Add(responsePage);
        }

        /// <summary>
        /// Get the pages of the response.
        /// </summary>
        private List<ResponsePage> Pages
        {
            get { return _pages; }
        }
        
        /// <summary>
        /// Replacement for the property ResponsePages to decrease size of the serialized object
        /// </summary>
        /// <returns></returns>
        public List<ResponsePage> GetResponsePages()
        {
            return _pages.ToList();
        }

        /// <summary>
        /// Get page with id
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ResponsePage GetPage(int pageId)
        {
            return _pages.Find(p => p.PageId == pageId);
        }

        public List<ResponsePage> GetPages()
        {
            return _pages;
        }

        /// <summary>
        /// Get page by index
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ResponsePage GetPageByIndex(int idx)
        {
            return _pages[idx];
        }
        /// <summary>
        /// Get/set the text ID of the survey title
        /// </summary>
        internal string TitleTextID { get; set; }

        /// <summary>
        /// Get/set the description of the survey
        /// </summary>
        internal string DescriptionTextID { get; set; }

        /// <summary>
        /// Get the Title
        /// </summary>
        public string Title
        {
            get
            {
                string title = TextManager.GetText(TitleTextID, LanguageCode);

                if (Utilities.IsNotNullOrEmpty(title))
                {
                    return title;
                }

                return TemplateName;
            }
        }

        /// <summary>
        /// Get the description
        /// </summary>
        public string Description
        {
            get { return TextManager.GetText(DescriptionTextID, LanguageCode); }
        }

        /// <summary>
        /// Get/set the template name
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Get the guid of the respondent associated with this response
        /// </summary>
        public Guid? RespondentGuid
        {
            get
            {
                if (_responseState != null)
                {
                    return _responseState.RespondentGuid;
                }

                return null;
            }
        }

        /// <summary>
        /// Get an item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public Item GetItem(int itemID)
        {
            if (!_items.ContainsKey(itemID))
            {
                return null;
            }

            return _items[itemID];
        }

        /// <summary>
        /// Get a list of response pipes for the survey associated with the response.
        /// </summary>
        public List<ItemToken> ResponsePipes
        {
            get { return _responsePipes; }
        }

        /// <summary>
        /// Load the current page
        /// </summary>
        public void LoadCurrentPage()
        {
            if (CurrentPage != null)
            {
                CurrentPage.OnLoad(true);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose of the response and any child items.  We dispose the response so that Dispose() can be called for items,
        /// the rules engine, and anything else that contains datatables, views, etc.   Disposing these items will call
        /// suppressfinalize for them and hopefully save some work for the finalizer thread.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //Instruct GC not to finialize this object
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overridable dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Dispose items
                foreach (Item i in Items.Values)
                {
                    i.Dispose();
                }

                //Dispose the state
                if (_responseState != null)
                {
                    _responseState.Dispose();
                }

                //Dispose pipe mediator
                if (_pipeMediator != null)
                {
                    _pipeMediator.Dispose();
                }
            }
        }

        #endregion

        /// <summary>
        /// Get data transfer object for survey response
        /// </summary>
        public SurveyResponseData GetDataTransferObject()
        {
            return new SurveyResponseData
            {
                AnonymousRespondentGuid = RespondentGuid,
                CurrentPageId = CurrentPage.PageId,
                ResponseGuid = GUID.HasValue ? GUID.Value : Guid.Empty,
                ResponseId = ID.HasValue ? ID.Value : -1,
                ResponseTemplateId = ResponseTemplateID,
                UniqueIdentifier = UniqueIdentifier,
                DateCompleted = DateCompleted,
                DateLastEdited = LastModified,
                DateStarted = DateCreated
            };
        }

        #region IXmlSerializable Members

        //"Lite" implementation of serialization for select properties
        //used for email response, display response, etc.  In the future,
        //the response class will become more of a container and a controller
        //will handle states and state transitions.
        
        /// <summary>
        /// Return NULL per MSDN documentation.
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// De-Serialize the response, not current supported.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize the response to XML
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("response");
            writer.WriteAttributeString("responseId", ID.ToString());

            //Response properties
            writer.WriteStartElement("responseProperties");
            XmlUtility.SerializeNameValueCollection(writer, GetResponseProperties(), true);
            writer.WriteEndElement();

            //Template pages
            writer.WriteStartElement("pages");
            var pages = GetResponsePages();

            foreach (ResponsePage page in pages)
            {
                ReloadPageItems(page, null);
                page.WriteXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Get response properties
        /// </summary>
        /// <returns></returns>
        private NameValueCollection GetResponseProperties()
        {
            var values = new NameValueCollection();
            var props = new ResponseProperties();
            props.Initialize(this);

            ReadOnlyCollection<string> propertyNames = ResponseProperties.PropertyNames;

            foreach (string propertyName in propertyNames)
            {
                values[propertyName] = props.GetStringValue(propertyName);
            }

            return values;
        }


        #endregion


        /// <summary>
        /// Returns items' numbers in the page starting from 0
        /// 
        /// In case of static items numbering the items which were hidden due to conditions must makes holes in 
        /// item number line.
        /// </summary>
        /// <param name="requestedPage"></param>
        /// <returns></returns>
        public int[] GetItemNumbers(ResponsePage requestedPage)
        {
            var numbers = new List<int>();
            var currentNum = 0;
            foreach (var i in requestedPage.GetItems())
            {
                if (!i.Excluded)
                {
                    numbers.Add(currentNum++);
                }
                else
                {
                    if (!UseDynamicItemNumbers)
                        ++currentNum;
                }
            }
            return numbers.ToArray();
        }

        /// <summary>
        /// Recalculates page items' conditions
        /// </summary>
        /// <returns></returns>
        public void UpdateCurrentPageConditions()
        {
            if (CurrentPage == null)
                return;
            CurrentPage.OnLoad(true);
        }

        /// <summary>
        /// Initialize flags that determines dependencies of other items from the each one
        /// </summary>
        internal void InitializeSPCItemDependenciesForPage(ResponsePage page)
        {
            if (page != null)
            {
                foreach (var i in page.GetItems())
                {
                    var ri = _items[i.ID] as AnswerableItem;
                    if (ri != null)
                    {
                        var mi = ri as MatrixItem;
                        if (mi != null)
                        {
                            foreach (var child in mi.Items)
                            {
                                ri = child as AnswerableItem;
                                if (ri != null)
                                {
                                    ri.IsSPCArgument = RulesEngine.ItemsDependOn(page.GetItems(), ri);
                                }
                            }
                        }
                        else
                        {
                            ri.IsSPCArgument = RulesEngine.ItemsDependOn(page.GetItems(), i);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Response status enum.
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Response is in-progress
        /// </summary>
        INPROGRESS = 1,

        /// <summary>
        /// Response has been completed
        /// </summary>
        COMPLETED,

        /// <summary>
        /// Response has been completed and is now being edited
        /// </summary>
        EDITING,

        /// <summary>
        /// Response has been marked as deleted
        /// </summary>
        DELETED
    }

    /// <summary>
    /// Event arguments for response state events
    /// </summary>
    public class ResponseStateEventArgs : EventArgs
    {
        private readonly ResponseState _state;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="state">Current response state.</param>
        public ResponseStateEventArgs(ResponseState state)
        {
            _state = state;
        }

        /// <summary>
        /// Get the response state
        /// </summary>
        public ResponseState State
        {
            get { return _state; }
        }
    }

    /// <summary>
    /// Event arguments for response page changing.
    /// </summary>
    public class ResponsePageChangedEventArgs : EventArgs
    {
        private readonly Int32 _previousPagePosition;
        private readonly Int32 _newPagePosition;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="previousPage">Position of page moving from.</param>
        /// <param name="newPage">Position of page moving to.</param>
        public ResponsePageChangedEventArgs(Int32 previousPage, Int32 newPage)
        {
            _previousPagePosition = previousPage;
            _newPagePosition = newPage;
        }

        /// <summary>
        /// Get the position of the previous page.
        /// </summary>
        public Int32 PreviousPage
        {
            get { return _previousPagePosition; }
        }

        /// <summary>
        /// Get the position of the new current page.
        /// </summary>
        public Int32 NewPage
        {
            get { return _newPagePosition; }
        }
    }
}
