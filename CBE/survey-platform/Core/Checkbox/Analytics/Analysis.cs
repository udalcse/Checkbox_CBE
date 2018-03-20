using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Filters;
using Checkbox.Analytics.Items;
using Checkbox.Forms.Items;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Contains business logic for running instance of an analysis template.
    /// </summary>
    [Serializable]
    public class Analysis
    {
        //Analysis data id

        //Response templates this analysis applies to
        private readonly List<Int32> _responseTemplateIDs;

        //Analysis Items
        private readonly Dictionary<Int32, Item> _items;

        //Analysis Pages
        private readonly List<AnalysisPage> _pages;

        //Analysis-wide filters

        //Analysis data
        private AnalysisAnswerData _answerData;

        //Date time to use as reference date for individual items to use
        // when checking reference dates.
        private DateTime? _resultCacheReferenceDate;

        /// <summary>
        /// Indiates whether to skip freshness checks when loading answer data
        /// </summary>
        private bool _skipFreshnessChecks;

        /// <summary>
        /// Default constructor that initializes member collections.
        /// </summary>
        public Analysis()
        {
            _items = new Dictionary<int, Item>();
            _pages = new List<AnalysisPage>();
            Filters = new AnalysisFilterCollection();
            _responseTemplateIDs = new List<int>();
        }


        /// <summary>
        /// Get the ID of the analysis.
        /// </summary>
        public Int32 ID { get; private set; }

        /// <summary>
        /// Manually set id of analysis
        /// </summary>
        /// <param name="analysisId"></param>
        public void SetId(int analysisId)
        {
            ID = analysisId;

            if (Filters != null)
            {
                Filters.ParentID = analysisId;
            }
        }

        /// <summary>
        /// Get the collection of filters associated with this analysis, excluding the min/max completion date values which are available via
        /// the <see cref="MinResponseDate"/> and <see cref="MaxResponseDate"/> properties respectively.
        /// </summary>
        public AnalysisFilterCollection Filters { get; private set; }

        /// <summary>
        /// Get language code for analysis
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Get/set earliest date for completed results to include in the Analysis.  When this property is set, only completed responses finished on or after this date OR 
        /// incomplete responses modified on or after this date will be modified.
        /// </summary>
        /// <remarks>A NULL value indicates that no responses should be excluded for not meeting a minimum completed date criterion.</remarks>
        public DateTime? MinResponseDate { get; set; }

        /// <summary>
        /// Get/set latest date for completed results to include in the Analysis.  When this property is set, only completed responses finished on or before this date OR 
        /// incomplete responses modified on or before this date will be modified.
        /// </summary>
        /// <remarks>A NULL value indicates that no responses should be excluded for not meeting a maximum completed date criterion.</remarks>
        public DateTime? MaxResponseDate { get; set; }

        /// <summary>
        /// Get whether incomplete responses should be included in the analysis.
        /// </summary>
        public bool IncludeIncompleteResponses { get; private set; }

        /// <summary>
        /// Get whether test responses should be included in the analysis.
        /// </summary>
        public bool IncludeTestResponses { get; private set; }

        /// <summary>
        /// Get progress key used for progress tracking.
        /// </summary>
        public string ProgressKey { get; private set; }

        /// <summary>
        /// Get the <see cref="AnalysisAnswerData"/> object associated with this Analysis.  This object contains all item and option text and aliases as well as information about
        /// each response (and associated answers) included in the analysis.
        /// </summary>
        public AnalysisAnswerData Data
        {
            get { return _answerData ?? (_answerData = LoadAnswerData(IncludeIncompleteResponses, null)); }
        }

        ///<summary>
        ///</summary>
        ///<param name="itemIDs"></param>
        ///<returns></returns>
        public AnalysisAnswerData FillData(IEnumerable<int> itemIDs)
        {
            return _answerData ?? (_answerData = LoadAnswerData(IncludeIncompleteResponses, itemIDs));
        }

        /// <summary>
        /// Get/set the name of the analysis.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialize the analysis with specified language and filter information.
        /// </summary>
        /// <param name="filterCollection">Collection of non-completion-date dependent response filters.</param>
        /// <param name="languageCode">Language code for the analysis.</param>
        /// <param name="startFilter">Minimum date filter for response completion date.</param>
        /// <param name="endFilter">Maximum date filter for response completion date.</param>
        /// <param name="includeIncompleteResponses">Specify whether incomplete responses should be included or not.</param>
        /// <param name="includeTestResponses">Specify whether test responses should be included or not.</param>
        /// <param name="progressKey">Key to use for tracking progress.</param>
        /// <param name="analysisModifiedDate">Date/Time analysis was last modified</param>
        public void Initialize(AnalysisFilterCollection filterCollection, string languageCode, DateTime? startFilter, DateTime? endFilter, 
            bool includeIncompleteResponses, bool includeTestResponses, DateTime? analysisModifiedDate)
        {
            LanguageCode = languageCode;
            Filters = filterCollection;

            //Get reference date for result cache
            DateTime? surveyReferenceDate = AnalysisDataProxy.GetSurveyReferenceDate(ResponseTemplateIDs);

            if (surveyReferenceDate.HasValue && !analysisModifiedDate.HasValue)
            {
                _resultCacheReferenceDate = surveyReferenceDate;
            }
            else if (!surveyReferenceDate.HasValue && analysisModifiedDate.HasValue)
            {
                _resultCacheReferenceDate = analysisModifiedDate;
            }
            else if (!surveyReferenceDate.HasValue)
            {
                _resultCacheReferenceDate = null;
            }
            else
            {
                _resultCacheReferenceDate = surveyReferenceDate.Value > analysisModifiedDate.Value
                    ? surveyReferenceDate
                    : analysisModifiedDate;
            }

            _skipFreshnessChecks = true;

            //Set the item's reference to the greater of the analysis reference date or the survey reference date.
            foreach (Item item in _items.Values)
            {
                if (item is AnalysisItem)
                {
                    ((AnalysisItem)item).Analysis = this;
                    ((AnalysisItem)item).ResultValidationReferenceDate = _resultCacheReferenceDate;

                    //Not point in validating all analysis items at this time since one failure means mandatory 
                    // freshness checks when loading analysis data.  Items will validate templates themselves
                    // when the load source data, so no need to pre-validate here.
                    if (_skipFreshnessChecks)
                    {
                        _skipFreshnessChecks = _skipFreshnessChecks && ((AnalysisItem)item).TemplatesValidated;
                    }
                }
            }

            MinResponseDate = startFilter;
            MaxResponseDate = endFilter;
            IncludeIncompleteResponses = includeIncompleteResponses;
            IncludeTestResponses = includeTestResponses;
        }

        /// <summary>
        /// List response template ids for items reported on by this analysis.
        /// </summary>
        /// <remarks>Currently, Checkbox only supports reporting on items from a single response template in an analysis, though this may change in the future.</remarks>
        public List<Int32> ResponseTemplateIDs
        {
            get { return _responseTemplateIDs; }
        }

        /// <summary>
        /// Add a response template id to the list of response templates reported on by this analysis.
        /// </summary>
        /// <param name="responseTemplateID">ID of response template to add to the list.</param>
        /// <remarks>Currently, Checkbox only supports reporting on items from a single response template in an analysis, though this may change in the future.  As
        /// a result, only the first value added will be used.</remarks>
        public void AddResponseTemplateID(Int32 responseTemplateID)
        {
            if (!_responseTemplateIDs.Contains(responseTemplateID))
            {
                _responseTemplateIDs.Add(responseTemplateID);
            }
        }

        /// <summary>
        /// Load analysis data, optionally with data for incomplete responses. 
        /// </summary>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="itemIDs"></param>
        private AnalysisAnswerData LoadAnswerData(bool includeIncompleteResponses, IEnumerable<int> itemIDs)
        {
            var answerData = new AnalysisAnswerData(_skipFreshnessChecks);
            answerData.Load(
                itemIDs,
                ResponseTemplateIDs[0],
                Filters,
                MinResponseDate,
                MaxResponseDate,
                includeIncompleteResponses,
                IncludeTestResponses,
                ProgressKey);

            return answerData;
        }

        /// <summary>
        /// Get a list of pages contained in this analysis.
        /// </summary>
        public ReadOnlyCollection<AnalysisPage> Pages
        {
            get { return new ReadOnlyCollection<AnalysisPage>(_pages); }
        }

        /// <summary>
        /// Add a page to the analysis.
        /// </summary>
        /// <param name="page">Page to add to the analysis.</param>
        public void AddPage(AnalysisPage page)
        {
            _pages.Add(page);
        }

        /// <summary>
        /// Add an item to the collection of items contained in this analysis.
        /// </summary>
        /// <param name="item">Item to add to the collection.</param>
        /// <remarks>Only analysis items or display-only items are supported in analyses.  Items that expect user input will not work if added to an analysis.</remarks>
        public void AddItem(Item item)
        {
            if (item != null)
            {
                _items[item.ID] = item;
            }
        }

        /// <summary>
        /// Get the item with the specified id from the analysis' item collection.
        /// </summary>
        /// <param name="itemID">ID of item to retrieve.</param>
        /// <returns>Item with the specified ID, or NULL if the item is not found.</returns>
        public Item GetItem(Int32 itemID)
        {
            if (_items.ContainsKey(itemID))
            {
                return _items[itemID];
            }
            
            return null;
        }

        /// <summary>
        /// Get a listing of items contained in the analysis.
        /// </summary>
        public List<Item> Items
        {
            get
            {
                return new List<Item>(_items.Values);
                
            }
        }
    }
}
