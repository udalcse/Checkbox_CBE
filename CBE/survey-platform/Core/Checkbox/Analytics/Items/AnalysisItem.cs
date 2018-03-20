using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Forms.Items;
using Checkbox.Analytics.Filters;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Common;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Abstract base class for items that appear in reports and perform the task of summarizing and displaying (optionally) filtered
    /// response data in aggregate form for a survey.  Object lifecycle should not exceed one page execution.
    /// 
    /// Analysis items are bound directly to renderers so they implement the IItemDataTransferObject interface.  At some future point
    /// renderers may access analysis itme data through a service layer and this abstraction will make for an easier transition at that
    /// time.
    /// </summary>
    [Serializable]
    public abstract class AnalysisItem : Item
    {
        private Dictionary<Int32, Int32> _responseCounts;
        private DateTime? _metaDataLastModified;
        private AnalysisTemplate _report;

        //Used for running w/in a survey, when report service is not used, or when
        // report service has no data for a report item.
        // report service has no data for a report item.
        private AnalysisAnswerData _analysisData;

        /// <summary>
        /// Temporary cache for data to avoid too much cache talk.
        /// </summary>
        private AnalysisItemResult TempResult { get; set; }


        /// <summary>
        /// Get a reference to current report
        /// </summary>
        protected AnalysisTemplate Report
        {
            get
            {
                if (_report == null)
                {
                    if (SourceAnalysisTemplateId.HasValue)
                        _report = AnalysisTemplateManager.GetAnalysisTemplate(SourceAnalysisTemplateId.Value, true);
                    else
                        throw new Exception("Item not initialized correctly.");
                }
                return _report;
            }
        }

        /// <summary>
        /// Constructor that initializes internal collections used by the item.
        /// </summary>
        protected AnalysisItem()
        {
            FilterCollection = new AnalysisItemFilterCollection();
            SourceItemIds = new List<int>();

            RunMode = false;
        }

        /// <summary>
        /// Gets or sets a source response template ID
        /// </summary>
        public int SourceResponseTemplateId { get; set; }

        /// <summary>
        /// Gets or sets a source analysis template ID
        /// </summary>
        public int? SourceAnalysisTemplateId { get; set; }

        /// <summary>
        /// Get list of source item ids
        /// </summary>
        public List<int> SourceItemIds { get; protected set; }

        /// <summary>
        /// Get/set date to use for validating result data
        /// </summary>
        public DateTime? ResultValidationReferenceDate { get; set; }

        /// <summary>
        /// Get a read-only collection of filters applied to this report item.
        /// </summary>
        /// <remarks>This read-only collection is suitable for use by interfaces that need to view the list of associated filters.</remarks>
        public ReadOnlyCollection<Filter> GetFilters()
        {
            return new ReadOnlyCollection<Filter>(FilterCollection.GetFilters(LanguageCode));
        }

        /// <summary>
        /// Get whether to include source item and option data in item dto.  In most cases, this
        /// data is required, so default value is true.  
        /// </summary>
        protected virtual bool IncludeSourceDataInDto { get { return true; } }

        /// <summary>
        /// Get the collection of filters applied to this report item.
        /// </summary>
        /// <remarks>Classes that extend AnalysisItem should use this method to access the filter collection.</remarks>
        protected AnalysisItemFilterCollection FilterCollection { get; private set; }

        /// <summary>
        /// Return whether the item is operating in "Preview" mode.
        /// </summary>
        /// <remarks>Preview mode use used to a display a preview of what the item will look like when rendered in a report. Live response data
        /// is not presented.</remarks>
        public virtual bool PreviewMode
        {
            get { return (!RunMode && ApplicationManager.AppSettings.ShowPreviewInAnalysis); }
        }

        /// <summary>
        /// Get/set whether running in outside the context of a report, such as when used in a 
        /// poll or survey.
        /// </summary>
        public bool StandaloneMode { get; set; }

        /// <summary>
        /// Get the <see cref="AnalysisAnswerData" /> containing the item, option, response, and answer information this item will use
        /// to fetch/generate it's reporting data.
        /// </summary>
        protected AnalysisAnswerData GetAnalysisData(bool includeIncompleteAnswers, bool includeTestAnswers, bool applyFilters = false)
        {
            if (Analysis != null)
            {
                StandaloneMode = false;
                return Analysis.FillData(SourceItemIds);
            }

            if (_analysisData == null)
            {
                //Filter list to just include answsers to source items
                var filterCollection = new AnalysisFilterCollection();
                filterCollection.AddFilter(new SourceItemFilterData(SourceItemIds));

                var existingFilters = new AnalysisFilterCollection();

                if (applyFilters)
                {
                    var filters = this.GetFilters();
                    foreach (var filter in filters)
                        existingFilters.AddFilter(filter.FilterId);
                }

                _analysisData = new AnalysisAnswerData(TemplatesValidated);
                _analysisData.Initialize(LanguageCode);
                _analysisData.Load(
                    SourceItemIds,
                    SourceResponseTemplateId,
                    applyFilters ? existingFilters :  new AnalysisFilterCollection(),
                    null,
                    null,
                    includeIncompleteAnswers,
                    includeTestAnswers,
                    string.Empty);
                StandaloneMode = true;
            }

            return _analysisData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string BuildSourceIdsString()
        {
            string result = null;
            if (SourceItemIds != null && SourceItemIds.Count > 0)
            {
                result = SourceItemIds.Aggregate(result, (current, sourceItemId) => current + (sourceItemId + ","));
                result = result.Remove(result.Length - 1);
            }

            return result;
        }

        /// <summary>
        /// Build a query to select answers for the analysis.
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, StringBuilder> BuildFilterStrings()
        {
            if (Analysis != null && Analysis.Filters != null)
            {
                var filterStrings = Analysis.Filters.BuildFilterStrings();
                 
                var filters = FilterCollection.GetFilters(LanguageCode);

                var res = new Dictionary<string, StringBuilder>();

                //insert AND operator to join
                foreach (var fp in filterStrings.Keys)
                {
                    if (filterStrings[fp].Length > 0)
                        res.Add(fp, filterStrings[fp].Insert(0, " AND "));
                }

                foreach (var f in filters)
                {
                    if (f is ItemQueryFilter)
                    {
                        if (!res.ContainsKey(f.FilterParameter))
                            res.Add(f.FilterParameter, new StringBuilder());
                        
                        var sb = res[f.FilterParameter];
                        sb.AppendFormat(@"AND 
                            (ckbx_ResponseAnswers.ResponseID in (select distinct ResponseID from ckbx_ResponseAnswers a where {0}))",
                            ((ItemQueryFilter) f).FilterString.Replace("ItemID", "a.ItemID")
                                .Replace("OptionID", "a.OptionID")
                                .Replace("AnswerText", "a.AnswerText"));
                    }
                    else if (f is IQueryFilter)
                    {
                        var qf = f as IQueryFilter;
                        res.Add(f.FilterParameter, new StringBuilder(qf.FilterString.Insert(0, " AND ")));
                    }
                }

                return res;
            }
            return null;
        }


        /// <summary>
        /// Configure the item with it's meta-data, language code for displaying text, and the collection of applied filters.
        /// </summary>
        /// <param name="itemData">AnalysisItem's configuration information.</param>
        /// <param name="languageCode">Language code to use for fetching text.</param>
        /// <param name="filters">Filters applied to the item.</param>
        public void Configure(ItemData itemData, string languageCode, AnalysisItemFilterCollection filters)
        {
            FilterCollection = filters;
            _metaDataLastModified = itemData.LastModified;

            Configure(itemData, languageCode, SourceResponseTemplateId);
        }

        /// <summary>
        /// Get/set whether the item is in run mode (i.e. part of a report or survey, not in a report editor or survey editor).
        /// </summary>
        public bool RunMode { get; set; }

        /// <summary>
        /// Get/set whether item and option aliases should be used in favor of item and option text values.
        /// </summary>
        public bool UseAliases { get; set; }

        /// <summary>
        /// Get a data transfer object for report items which is the item itself.
        /// </summary>
        /// <returns></returns>
        public override IItemProxyObject CreateDataTransferObject()
        {
            return new ReportItemInstanceData();
        }

        /// <summary>
        /// Get instance values for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values = base.GetInstanceDataValuesForSerialization();

            values["RunMode"] = RunMode.ToString();
            values["StandaloneMode"] = StandaloneMode.ToString();
            values["PreviewMode"] = PreviewMode.ToString();

            return values;
        }


        /// <summary>
        /// Configure the item with it's meta-data, language code for displaying text, and no filters.
        /// </summary>
        /// <param name="itemData">AnalysisItem's configuration information.</param>
        /// <param name="languageCode">Language code to use for fetching text.</param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            SourceAnalysisTemplateId = itemData.ParentTemplateId;

            UseAliases = ((AnalysisItemData)itemData).UseAliases;

            if (((AnalysisItemData)itemData).ResponseTemplateIds.Count > 0)
            {
                SourceResponseTemplateId = ((AnalysisItemData)itemData).ResponseTemplateIds[0];
            }

            SourceItemIds = new List<int>(((AnalysisItemData)itemData).SourceItemIds);
        }

        /// <summary>
        /// List all <see cref="ItemAnswer"/> for the specified item.
        /// </summary>
        /// <param name="itemId">ID of item to list answers for.</param>
        /// <returns>List of <see cref="ItemAnswer"/> objects corresponding to answers to the item.</returns>
        protected List<ItemAnswer> GetItemAnswers(int itemId)
        {
            return GetItemAnswers(itemId, Report.IncludeIncompleteResponses, Report.IncludeTestResponses);
        }

        /// <summary>
        /// List all <see cref="ItemAnswer"/> for the specified item.
        /// </summary>
        /// <param name="itemId">ID of item to list answers for.</param>
        /// <param name="includeIncompleteAnswers"></param>
        /// <param name="includeTestResponses"></param>
        /// <returns>List of <see cref="ItemAnswer"/> objects corresponding to answers to the item.</returns>
        protected List<ItemAnswer> GetItemAnswers(int itemId, bool includeIncompleteAnswers, bool includeTestResponses)
        {
            return GetAnalysisData(includeIncompleteAnswers, includeTestResponses).ListItemAnswers(itemId, SourceResponseTemplateId, FilterCollection.GetFilters(TextManager.DefaultLanguage));
        }

        /// <summary>
        /// Get the text for an item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public virtual string GetItemText(Int32 itemID)
        {
            return SurveyMetaDataProxy.GetItemText(itemID, LanguageCode, UseAliases, TemplatesValidated);
        }

        /// <summary>
        /// Get the type name of a source item.
        /// </summary>
        /// <param name="itemID">ID of item to get type name of.</param>
        /// <returns>Type name of source item.</returns>
        public virtual string GetSourceItemTypeName(Int32 itemID)
        {
            return SurveyMetaDataProxy.GetItemTypeName(itemID, true);
        }

        /// <summary>
        /// Get the text for an option
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="optionID"></param>
        /// <returns></returns>
        public virtual string GetOptionText(Int32 itemID, Int32 optionID)
        {
            return SurveyMetaDataProxy.GetOptionText(itemID, optionID, LanguageCode, UseAliases, true);
        }

        /// <summary>
        /// Get/set the analysis
        /// </summary>
        public Analysis Analysis { get; set; }

        /// <summary>
        /// Generate data key for caching
        /// </summary>
        /// <returns></returns>
        protected string GenerateDataKey()
        {
            return PreviewMode ? "Preview" : "Run";
        }

        private bool? _templatesValidated;

        /// <summary>
        /// Get/set whether source templates have not been modified since result last stored in cache
        /// </summary>
        public virtual bool TemplatesValidated
        {
            get
            {
                if (!_templatesValidated.HasValue)
                {
                    _templatesValidated = ValidateTemplateData();
                }

                return _templatesValidated.Value;
            }
        }

        /// <summary>
        /// Validate template data
        /// </summary>
        /// <returns></returns>
        public bool ValidateTemplateData()
        {
            AnalysisDataProxyCacheItem<AnalysisItemResult> cacheItem = AnalysisDataProxy.GetResultFromCache<AnalysisItemResult>(
                ID, LanguageCode, GenerateDataKey());

            if (cacheItem == null)
            {
                return false;
            }

            if (ResponseTemplateManager.CheckTemplateUpdated(SourceResponseTemplateId, cacheItem.ReferenceDate))
                return false;

            return !ResponseTemplateManager.CheckTemplateResponsesUpdated(SourceResponseTemplateId, cacheItem.ReferenceDate);
        }

        /// <summary>
        /// Validate that the result data associated with this item is
        /// current.
        /// </summary>
        /// <returns></returns>
        private bool ValidateResultData(AnalysisDataProxyCacheItem<AnalysisItemResult> cacheItem)
        {
            if (cacheItem == null)
                return false;

            if (ResponseTemplateManager.CheckTemplateUpdated(SourceResponseTemplateId, cacheItem.ReferenceDate))
                return false;

            if(ResponseTemplateManager.CheckTemplateResponsesUpdated(SourceResponseTemplateId, cacheItem.ReferenceDate))
                return false;

            DateTime? dateToCheck = _metaDataLastModified;
            if (Analysis != null)
            {
                var filterCollectionLastModifiedDate = Analysis.Filters.GetLastModifiedDate();
                if (filterCollectionLastModifiedDate > _metaDataLastModified)
                    dateToCheck = _metaDataLastModified;
            }

            //Verify that any cached results data is as new or newer than
            // any changes to the analysis item's configuration
            if (AnalysisDataProxy.ValidateItemResultData(
                ID,
                LanguageCode,
                GenerateDataKey(),
                dateToCheck))
            {
                //Results ok compared to report item's configuration, now check
                // results against survey answer reference dates
                if (!AnalysisDataProxy.ValidateItemResultData(
                    ID,
                    LanguageCode,
                    GenerateDataKey(),
                    ResultValidationReferenceDate))
                    return false;
            }

            return false;
        }


        /// <summary>
        /// Get data for a
        /// </summary>
        /// <returns></returns>
        protected AnalysisItemResult GetResultData()
        {
            //Handle not in running report or running survey and preview mode turned off.
            if (!RunMode && !PreviewMode)
            {
                return null;
            }

            //Check temp result
            if (TempResult != null)
            {
                return TempResult;
            }

            AnalysisItemResult theData = null;

            AnalysisDataProxyCacheItem<AnalysisItemResult> cacheItem = AnalysisDataProxy.GetResultFromCache<AnalysisItemResult>(
                   ID,
                   LanguageCode,
                   GenerateDataKey());

            //Otherwise, check result cache
            if (ValidateResultData(cacheItem))
            {
                theData = cacheItem.Data;
            }
            else
            {
                //Depending on whether item previews are being used, either generate some preview data
                // or perform computations based on answer data retrieved by the analysis.  If reporting
                // service is enabled, try to use it to get the data.
                theData = PreviewMode
                    ? GeneratePreviewData()
                    : LoadAndProcessData();

                //Cache data if data is not null or if, in the case of datasets,
                // contains at least one table.
                if (theData != null)
                {
                    AnalysisDataProxy.AddResultToCache(
                        ID,
                        LanguageCode,
                        GenerateDataKey(),
                        theData);
                }
            }

            TempResult = theData;

            return theData;
        }

        /// <summary>
        /// Load any response/answer data this item needs and perform any 
        /// processing.  In most cases, data should be retrieved from the
        /// AnalysisAnswerData object maintained by the Analysis or from
        /// the Checkbox report service.
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult LoadAndProcessData()
        {

            //if (ApplicationManager.AppSettings.UseReportingService
            //    && Analysis != null)
            //{
            //    try
            //    {
            //        //Try to get data from the reporting service
            //        ReportCommunicationServiceClient rsClient = new ReportCommunicationServiceClient();
            //        theData = rsClient.GetReportItemData(
            //            ApplicationManager.ApplicationDataContext,
            //            Analysis.ID,
            //            ID,
            //            LanguageCode,
            //            GetAdditionalRsParams());
            //    }
            //    catch (Exception ex)
            //    {
            //        //Log error and move on
            //        ExceptionPolicy.HandleException(ex, "BusinessProtected");
            //    }
            //}

            return ProcessData();
        }

        ///// <summary>
        ///// Get additonal parameters to pass to the report service.
        ///// </summary>
        ///// <returns></returns>
        //protected virtual object[] GetAdditionalRsParams()
        //{
        //    return new object[] { };
        //}

        /// <summary>
        /// Process answer data to get results
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult ProcessData()
        {
            return null;
        }

        /// <summary>
        /// Get response counts for items that are part of this analysis
        /// </summary>
        protected Dictionary<Int32, Int32> ResponseCounts
        {
            get { return _responseCounts ?? (_responseCounts = new Dictionary<int, int>()); }
        }

        /// <summary>
        /// Get the response count for a particular item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public Int32 GetItemResponseCount(Int32 itemID)
        {
            if (!ResponseCounts.ContainsKey(itemID))
            {
                ResponseCounts[itemID] = GetResponseCountForItem(itemID);
            }

            return ResponseCounts[itemID];
        }

        /// <summary>
        /// Get the response count for an item.  It is up to concrete class to produce this value.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected virtual int GetResponseCountForItem(int itemId)
        {
            var resultData = GetResultData();

            if (resultData == null)
            {
                return 0;
            }

            return resultData.ItemResponseCounts.ContainsKey(itemId)
                ? resultData.ItemResponseCounts[itemId]
                : 0;
        }


        /// <summary>
        /// Get item alias and corresponding option aliases from configuration data
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="optionAliases"></param>
        /// <returns></returns>
        protected virtual string GetItemAliasFromConfigurationData(Int32 itemID, out Dictionary<Int32, string> optionAliases)
        {
            ItemData data = ItemConfigurationManager.GetConfigurationData(itemID);

            string alias = string.Empty;
            optionAliases = new Dictionary<int, string>();

            if (data != null && data.Alias != null)
            {
                alias = data.Alias;
            }

            //Now get options
            if (data is SelectItemData)
            {
                foreach (ListOptionData option in ((SelectItemData)data).Options)
                {
                    optionAliases[option.OptionID] = option.Alias ?? string.Empty;
                }
            }

            return alias;
        }

        /// <summary>
        /// Get the points value for an option
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="optionID">Option ID</param>
        /// <returns></returns>
        public double GetOptionPoints(int itemID, int optionID)
        {
            return SurveyMetaDataProxy.GetOptionPoints(itemID, optionID, TemplatesValidated);
        }

        /// <summary>
        /// Get a boolean indicating if an option is an "other" option.
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="optionID">Option ID</param>
        /// <returns></returns>
        public bool GetOptionIsOther(int itemID, int optionID)
        {
            return SurveyMetaDataProxy.GetOptionIsOther(optionID, TemplatesValidated);
        }

        /// <summary>
        /// Populate the report item data with preview information
        /// </summary>
        protected virtual AnalysisItemResult GeneratePreviewData()
        {
            return null;
        }

        /// <summary>
        /// Get option ids for preview
        /// </summary>
        /// <param name="itemId">Item to list options for.</param>
        public virtual List<int> GetItemOptionIdsForPreview(int itemId)
        {
            List<int> optionIds = GetItemOptionIdsForReport(itemId);

            //Trim list if longer than preview limit
            if (optionIds.Count > ApplicationManager.AppSettings.MaxReportPreviewOptions 
                && ItemTypeName != "NetPromoterScoreTable")
            {
                int excessCount = optionIds.Count - ApplicationManager.AppSettings.MaxReportPreviewOptions;

                optionIds.RemoveRange(optionIds.Count - excessCount, excessCount);
            }

            return optionIds;
        }

        /// <summary>
        /// Get option ids for preview
        /// </summary>
        /// <param name="itemId">Item to list options for.</param>
        public virtual List<int> GetItemOptionIdsForReport(int itemId)
        {
            var optionIds = new List<int>();

            //Find item and get options
            LightweightItemMetaData itemMetaData = SurveyMetaDataProxy.GetItemData(itemId, TemplatesValidated);

            if (itemMetaData != null)
            {
                optionIds.AddRange(itemMetaData.Options);
            }

            return optionIds;
        }

        /// <summary>
        /// Get preview answers for the item
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="answerIdSeed"></param>
        /// <param name="responseIdSeed"></param>
        /// <returns></returns>
        protected virtual List<ItemAnswer> GetItemPreviewAnswers(int itemID, long? answerIdSeed, long? responseIdSeed)
        {
            var answers = new List<ItemAnswer>();

            var optionIds = GetItemOptionIdsForPreview(itemID);

            long answerId = answerIdSeed ?? 1000;
            long responseId = responseIdSeed ?? 1000;

            if (optionIds.Count > 0)
            {
                //Generate a random set of answers for each option
                for (int i = 0; i < optionIds.Count; i++)
                {
                    LightweightOptionMetaData optionMetaData = SurveyMetaDataProxy.GetOptionData(optionIds[i], itemID, TemplatesValidated);

                    if (optionMetaData != null)
                    {
                        //Add option answers
                        for (int j = 0; j <= i; j++)
                        {
                            string answerText = string.Empty;

                            if (optionMetaData.IsOther)
                            {
                                answerText = TextManager.GetText("/controlText/analysisItem/other", LanguageCode, "Other") + " " + j;
                            }

                            answers.Add(new ItemAnswer
                            {
                                AnswerId = answerId++,
                                ResponseId = responseId++,
                                ItemId = itemID,
                                OptionId = optionIds[i],
                                IsOther = optionMetaData.IsOther,
                                AnswerText = answerText,
                                Points = (j+2) * 14
                            });
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    answers.Add(new ItemAnswer
                    {
                        AnswerId = answerId++,
                        ResponseId = responseId++,
                        ItemId = itemID,
                        AnswerText = TextManager.GetText("/controlText/analysisItem/sample" + i, LanguageCode, "Sample " + i),
                        Points = (i + 2) * 14
                    });
                }
            }

            return answers;
        }

        /// <summary>
        /// Get analysis item title text.
        /// </summary>
        /// <returns></returns>
        public string GetTitleText(bool showResponseCount)
        {
            if (SourceItemIds.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            try
            {
                //TODO: Handle multiple items
                sb.Append(GetItemText(SourceItemIds[0]));

                if (showResponseCount)
                {
                    sb.Append(SourceItemIds.Count > 1 ? "  " : Environment.NewLine);
                    sb.Append("<br>(");
                    sb.Append(SourceItemIds.Count > 1 ? "  " : String.Empty);
                    sb.Append(Environment.NewLine + "(");

                    sb.Append(GetItemResponseCount(SourceItemIds[0]));

                    sb.Append("  ");
                    sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                    sb.Append(")");
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return sb.ToString();
            }
        }

        #region Data Transfer

        /// <summary>
        /// Build up data transfer object for survey item.
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ReportItemInstanceData)
            {
                ((ReportItemInstanceData)itemDto).SourceResponseTemplateId = SourceResponseTemplateId;
                ((ReportItemInstanceData)itemDto).UseAliases = UseAliases;

                PopulateSourceItems((ReportItemInstanceData)itemDto);
                PopulateItemResultData((ReportItemInstanceData)itemDto);
                PopulateAppliedFilterData((ReportItemInstanceData) itemDto);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected virtual void PopulateAppliedFilterData(ReportItemInstanceData itemDto)
        {
            itemDto.AppliedFilterTexts = GetFilters().Select(filter => filter.FilterText).ToArray();
        }

        /// <summary>
        /// Populate results
        /// </summary>
        /// <param name="itemDto"></param>
        protected virtual void PopulateItemResultData(ReportItemInstanceData itemDto)
        {
            var itemResult = GetResultData();

            if (itemResult == null)
            {
                return;
            }

            itemDto.AggregateResults = itemResult.AggregateResults;
            itemDto.CalculateResult = itemResult.CalculateResults;
            itemDto.IsPreview = itemResult.IsPreview;
            itemDto.GradientColorDirectorMatrixResult = itemResult.GradientColorDirectorMatrixResult;
            itemDto.HeatMapAnalysisResult = itemResult.HeatMapAnalysisResult;
            itemDto.GroupedDetailResults = itemResult.GroupedDetailResults;
            itemDto.GroupedAggregateResults = itemResult.GroupedAggregateResults;            
        }

        /// <summary>
        /// Populate source texts for items and options.  Base analysis item class
        /// DOES NOT populate these values since they are typically encompassed by
        /// results data returned from object.  For more complicated items, such as a
        /// cross tab, it may be necessary to have direct access to this data so IncludeTextsInDto
        /// should be overridden with a "true" value in child classes to add such data.
        /// </summary>
        /// <param name="itemDto"></param>
        protected virtual void PopulateSourceItems(ReportItemInstanceData itemDto)
        {
            var sourceData = new List<ReportItemSourceItemData>();

            if (IncludeSourceDataInDto)
            {
                foreach (var itemId in SourceItemIds.ToList())
                {
                    var sourceItemData = GetSourceItem(itemId);

                    if (sourceItemData != null)
                    {
                        sourceData.Add(sourceItemData);
                    }
                }
            }

            itemDto.SourceItems = sourceData.ToArray();
        }

        /// <summary>
        /// Get source item data for report item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected virtual ReportItemSourceItemData GetSourceItem(int itemId)
        {
            var itemMetaData = SurveyMetaDataProxy.GetItemData(itemId, TemplatesValidated);

            if (itemMetaData == null)
            {
                return null;
            }

            var itemSourceProxy = new ReportItemSourceItemData
            {
                Alias = itemMetaData.Alias,
                Description = itemMetaData.GetDescription(LanguageCode),
                ItemId = itemId,
                ItemPosition = itemMetaData.ItemPosition,
                ItemType = itemMetaData.ItemType,
                PagePosition = itemMetaData.PagePosition,
                ParentRowNumber = itemMetaData.Coordinate != null ? (int?)itemMetaData.Coordinate.Y : null,
                ParentColumnNumber = itemMetaData.Coordinate != null ? (int?)itemMetaData.Coordinate.X : null,
                ReportingText = Utilities.StripHtmlTags(itemMetaData.GetText(UseAliases, LanguageCode)),
                Text = itemMetaData.GetText(false, LanguageCode),
                ResponseCount = GetItemResponseCount(itemId),
                AnswerCount = GetItemResponseCount(itemId)
            };


            itemSourceProxy.Options = (from optionId in itemMetaData.Options
                                       let optionData = SurveyMetaDataProxy.GetOptionData(optionId, itemId, true)
                                       where optionData != null
                                       select new ReportItemSourceOptionData
                                       {
                                           Alias = optionData.Alias,
                                           IsOther = optionData.IsOther,
                                           OptionId = optionId,
                                           Points = optionData.Points,
                                           Position = optionData.Position,
                                           ReportingText = optionData.GetText(UseAliases, LanguageCode),
                                           Text = optionData.GetText(false, LanguageCode)
                                       }).ToArray();

            return itemSourceProxy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {

        }

        #region Process data logic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="responseCount"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            responseCount = 0;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult AggregatedData()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        protected virtual AggregateResult[] AggregateResult(IEnumerable<ItemAnswer> answers)
        {
            return null;
        }

        #endregion

        #endregion
    }
}
