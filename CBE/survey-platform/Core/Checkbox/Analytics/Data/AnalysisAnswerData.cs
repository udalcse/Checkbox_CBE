using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Analytics.Filters;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Container for item, option, and answer data associated with a running analysis.
    /// </summary>
    [Serializable]
    public class AnalysisAnswerData
    {
        /// <summary>
        /// Indicates profile data has been preloaded or not for a response
        /// </summary>
        private List<int> _preloadedProfileRtIds;

        /// <summary>
        /// Dictionary of answers to particular items across all responses
        /// </summary>
        private Dictionary<int, List<ItemAnswer>> _itemAnswersDictionary;

        /// <summary>
        /// Dictionary of answers to a particular item in a given response
        /// </summary>
        private Dictionary<Pair<int, long>, List<long>> _itemResponseAnswersDictionary;

        /// <summary>
        /// Dictionary of answers to a particular item option in a given response
        /// </summary>
        private Dictionary<Triplet<int, int, long>, long> _itemOptionResponseAnswersDictionary;

        /// <summary>
        /// Dictionary of answer objects on a per-response basis
        /// </summary>
        private Dictionary<long, List<ItemAnswer>> _responseAnswerObjectsDictionary;

        /// <summary>
        /// Dictionary of answers
        /// </summary>
        private Dictionary<long, ItemAnswer> _answersDictionary;

        /// <summary>
        /// Dictionary of response-releated properties
        /// </summary>
        private Dictionary<long, ResponseProperties> _responsePropertiesDictionary;

        /// <summary>
        /// Get the collection of response property names
        /// </summary>
        private ReadOnlyCollection<string> _responsePropertyNames;

        /// <summary>
        /// Dictionary to map ResponseProperties.PropertyName values to ordinals in the data reader.
        /// Using GetOrdinal on a reader is inefficient, AND not all properties are in the reader. This
        /// dictionary serves as a cache for the ordinals and as a flag to indicate if field names
        /// are present in the answer data reader.
        /// </summary>
        private Dictionary<string, int> _answerReaderOrdinals;

        /// <summary>
        /// Store list of response ids that pass given filter criteria
        /// </summary>
        private Dictionary<int, List<long>> _filterPassedResponseIds;

        /// <summary>
        /// Store list of responses that pass filters associated with items.  Responses that don't pass
        /// report-level filters are not included to begin with.
        /// </summary>
        private Dictionary<int, List<long>> FilterPassedResponseIds
        {
            get { return _filterPassedResponseIds ?? (_filterPassedResponseIds = new Dictionary<int, List<long>>()); }
        }

        /// <summary>
        /// Get the list of response property names which are cached locally
        /// </summary>
        private IEnumerable<string> ResponsePropertyNames
        {
            get { return _responsePropertyNames ?? (_responsePropertyNames = ResponseProperties.PropertyNames); }
        }

        /// <summary>
        /// Get answer reader field to ordinal mapping
        /// </summary>
        private Dictionary<string, int> AnswerReaderOrdinals
        {
            get { return _answerReaderOrdinals ?? (_answerReaderOrdinals = new Dictionary<string, int>()); }
        }

        /// <summary>
        /// Get the response properties dictionary
        /// </summary>
        protected Dictionary<long, ResponseProperties> ResponsePropertiesDictionary
        {
            get {return _responsePropertiesDictionary ??(_responsePropertiesDictionary = new Dictionary<long, ResponseProperties>());}
        }

        /// <summary>
        /// Get the answers dictionary
        /// </summary>
        private Dictionary<long, ItemAnswer> AnswerDictionary
        {
            get { return _answersDictionary ?? (_answersDictionary = new Dictionary<long, ItemAnswer>()); }
        }

        /// <summary>
        /// Get the dictionary linking response ids to response answer objects.
        /// </summary>
        /// <remarks>This dictionary is lazy-loaded by calls to GetResponseAnswers(...)
        /// method.</remarks>
        private Dictionary<long, List<ItemAnswer>> ResponseAnswerObjectDictionary
        {
            get {
                return _responseAnswerObjectsDictionary ??(_responseAnswerObjectsDictionary = new Dictionary<long, List<ItemAnswer>>());
            }
        }

        /// <summary>
        /// Get the dictionary linking items to answer objects.
        /// </summary>
        /// <remarks>This dictionary is lazy-loaded by calls to ListItemAnswers(int itemId)
        /// method.</remarks>
        private Dictionary<int, List<ItemAnswer>> ItemAnswersDictionary
        {
            get { return _itemAnswersDictionary ?? (_itemAnswersDictionary = new Dictionary<int, List<ItemAnswer>>()); }
        }

        /// <summary>
        /// Get the dictionary linking items to answer objects.
        /// </summary>
        private Dictionary<Pair<int, long>, List<long>> ItemResponseAnswersDictionary
        {
            get {return _itemResponseAnswersDictionary ??(_itemResponseAnswersDictionary = new Dictionary<Pair<int, long>, List<long>>()); }
        }

        /// <summary>
        /// Get the dictionary linking item options to answer objects.
        /// </summary>
        private Dictionary<Triplet<int, int, long>, long> ItemOptionResponseAnswersDictionary
        {
            get {return _itemOptionResponseAnswersDictionary ??(_itemOptionResponseAnswersDictionary = new Dictionary<Triplet<int, int, long>, long>()); }
        }

        /// <summary>
        /// Boolean indicating templates for items these answers refer to have been validated as not having
        /// been modified, so calls to survey meta data proxy don't need to do a freshness check.
        /// </summary>
        public bool SkipFreshnessChecks { get; private set; }

        /// <summary>
        /// Return a boolean value indicating whether this answer data object has loaded data for the analysis.
        /// </summary>
        public bool AnswerDataLoaded { get; private set; }

        /// <summary>
        /// Get language code used by this analysis data for retrieving item and option tests.
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Get key for in-progress load
        /// </summary>
        public string ProgressKey { get; private set; }

        /// <summary>
        /// Initialize the analysis data with the specified language code.
        /// </summary>
        /// <param name="languageCode"></param>
        public void Initialize(string languageCode)
        {
            AnswerDataLoaded = false;
            LanguageCode = languageCode;
        }

        /// <summary>
        /// Construct answer data object, and specify whether or not to skip
        /// freshness checks.
        /// </summary>
        /// <param name="skipDateModifiedChecks"></param>
        public AnalysisAnswerData(bool skipDateModifiedChecks)
        {
            SkipFreshnessChecks = skipDateModifiedChecks;
        }

        /// <summary>
        /// Add an answer to answer dictionaries
        /// </summary>
        /// <param name="answer"></param>
        private void AddAnswerToDictionaries(ItemAnswer answer)
        {
            //Add to answers dictionary
            AnswerDictionary[answer.AnswerId] = answer;

            if (!ItemAnswersDictionary.ContainsKey(answer.ItemId))
            {
                ItemAnswersDictionary[answer.ItemId] = new List<ItemAnswer>();
            }

            //Add to item answers dictionary
            ItemAnswersDictionary[answer.ItemId].Add(answer);

            //Add to response answer objects dictionary
            if (!ResponseAnswerObjectDictionary.ContainsKey(answer.ResponseId))
            {
                ResponseAnswerObjectDictionary[answer.ResponseId] = new List<ItemAnswer>();
            }

            ResponseAnswerObjectDictionary[answer.ResponseId].Add(answer);

            //Item response answers dictionary
            var itemRaKey = new Pair<int, long>(answer.ItemId, answer.ResponseId);

            if (!ItemResponseAnswersDictionary.ContainsKey(itemRaKey))
            {
                ItemResponseAnswersDictionary[itemRaKey] = new List<long>(1);
            }

            ItemResponseAnswersDictionary[itemRaKey].Add(answer.AnswerId);

            //Item option response answers dictionary
            if (answer.OptionId.HasValue)
            {
                var optionRaKey = new Triplet<int, int, long>(answer.ItemId, answer.OptionId.Value, answer.ResponseId);

                ItemOptionResponseAnswersDictionary[optionRaKey] = answer.AnswerId;
            }
        }

        /// <summary>
        /// Remove an answer from answer dictionaries
        /// </summary>
        /// <param name="answer"></param>
        private void RemoveAnswerFromDictionaries(ItemAnswer answer)
        {
            //Remove from answers dictionary
            AnswerDictionary.Remove(answer.AnswerId);

            //Remove from item answers
            if (ItemAnswersDictionary.ContainsKey(answer.ItemId))
            {
                ItemAnswersDictionary[answer.ItemId].Remove(answer);
            }

            //Remove from response answer objects dictionary
            if (ResponseAnswerObjectDictionary.ContainsKey(answer.ResponseId))
            {
                ResponseAnswerObjectDictionary[answer.ResponseId].Remove(answer);
            }

            //Item response answers dictionary
            var itemRaKey = new Pair<int, long>(answer.ItemId, answer.ResponseId);

            if (ItemResponseAnswersDictionary.ContainsKey(itemRaKey))
            {
                ItemResponseAnswersDictionary[itemRaKey].Remove(answer.AnswerId);
            }

            //Item option response answers dictionary
            if (answer.OptionId.HasValue)
            {
                var optionRaKey = new Triplet<int, int, long>(answer.ItemId, answer.OptionId.Value, answer.ResponseId);

                ItemOptionResponseAnswersDictionary.Remove(optionRaKey);
            }
        }

        /// <summary>
        /// Remove respnoses from answer dictionaries
        /// </summary>
        /// <param name="responseId"></param>
        private void RemoveResponsesFromDictionaries(long responseId)
        {
            //List all answers in responses
            var answersToRemove =
                AnswerDictionary.Values.Where(answer => answer.ResponseId == responseId).ToList();

            //Remove answers
            foreach (var answerToRemove in answersToRemove)
            {
                RemoveAnswerFromDictionaries(answerToRemove);
            }

            //Now remove repsonses
            ResponseAnswerObjectDictionary.Remove(responseId);
        }

        /// <summary>
        /// Preload profile data
        /// </summary>
        public void EnsureProfileDataPreloaded(int responseTemplateId)
        {
            if (ProfileManager.CheckboxProvider == null)
            {
                return;
            }

            if (_preloadedProfileRtIds == null)
            {
                _preloadedProfileRtIds = new List<int>();
            }
            
            if (!_preloadedProfileRtIds.Contains(responseTemplateId))
            {
                ProfileManager.CheckboxProvider.PreLoadProfilesForTemplateResponses(responseTemplateId);
                _preloadedProfileRtIds.Add(responseTemplateId);
            }
        }

        /// <summary>
        /// Load analysis answer data
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="filterCollection"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="includeTestResponses"> </param>
        /// <param name="progressKey"></param>
        public void Load(IEnumerable<int> itemIds, int responseTemplateId, AnalysisFilterCollection filterCollection,
            DateTime? startDate, DateTime? endDate, bool includeIncompleteResponses, bool includeTestResponses, string progressKey)
        {
            //Clear the answer dictionary
            AnswerDictionary.Clear();
            AnswerReaderOrdinals.Clear();
            ResponseAnswerObjectDictionary.Clear();
            ItemAnswersDictionary.Clear();
            ItemResponseAnswersDictionary.Clear();
            ItemOptionResponseAnswersDictionary.Clear();

            ProgressKey = progressKey;

            bool trackProgress = Utilities.IsNotNullOrEmpty(progressKey);

            if (trackProgress)
            {
                ProgressProvider.SetProgress(
                        progressKey,
                        TextManager.GetText("/controlText/analysisData/loadingAnswersFromDatabase", LanguageCode),
                        string.Empty,
                        ProgressStatus.Running,
                        50,                     //Since loading items is first 50% (numbers are somewhat abitrary and chosen for display purposes)
                        100);
            }

            var filterStrings = filterCollection.BuildFilterStrings();
            string itemIdsString = null;
            if (itemIds != null)
            {
                itemIdsString = itemIds.Aggregate(itemIdsString, (current, id) => current + (id + ","));
                itemIdsString = itemIdsString.Remove(itemIdsString.Length - 1);
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateId);
            command.AddInParameter("ItemIDString", DbType.String, itemIdsString);
            command.AddInParameter("IncludeIncompleteResponses", DbType.Byte, includeIncompleteResponses);
            command.AddInParameter("IncludeTestResponses", DbType.Byte, includeTestResponses);
            foreach (var filterParameter in filterStrings.Keys)
            {
                if (filterStrings[filterParameter].Length > 0)
                {
                    filterStrings[filterParameter].Insert(0, " AND ");
                    command.AddInParameter(filterParameter, DbType.String, filterStrings[filterParameter].ToString());
                }
            }
            command.AddInParameter("StartDate", DbType.DateTime, startDate);
            command.AddInParameter("EndDate", DbType.DateTime, endDate);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (trackProgress)
                    {
                        ProgressProvider.SetProgress(
                                progressKey,
                                TextManager.GetText("/controlText/analysisData/processingAnswers", LanguageCode),
                                string.Empty,
                                ProgressStatus.Running,
                                60,                   // (numbers are somewhat abitrary and chosen for display purposes)
                                100);
                    }
                    while (reader.Read())
                    {
                        AddItemAnswerObject(reader);
                    }
                }
                finally
                {
                    //Close the reader and rethrow the exception
                    reader.Close();
                }
            }

            if (trackProgress)
            {
                ProgressProvider.SetProgress(
                        progressKey,
                        TextManager.GetText("/controlText/analysisData/answersLoaded", LanguageCode),
                        string.Empty,
                        ProgressStatus.Running,
                        85,                     //(numbers are somewhat abitrary and chosen for display purposes)
                        100);
            }

            AnswerDataLoaded = true;
        }

        /// <summary>
        /// Build a query to select answers for the analysis.
        /// </summary>
        /// <param name="filterCollection"></param>
        /// <returns></returns>
        /*private string BuildFilterString(AnalysisFilterCollection filterCollection)
        {
            var sb = new StringBuilder();
           
            
            if (filterCollection != null)
            {
                string filterClause = filterCollection.FilterString;

                if (!string.IsNullOrEmpty(filterClause))
                {
                    sb.Append(" AND ");
                    sb.Append("(" + filterClause + ")");
                }
            }

            return sb.ToString();
        }*/


        /// <summary>
        /// Add an answer data object to the internal dictionary
        /// </summary>
        /// <param name="reader"></param>
        private void AddItemAnswerObject(IDataReader reader)
        {
            //Populate the repsonse properties
            PopulateResponseProperties(reader);

            //Create the answer data object.  For responses with NO answered questions, this value
            // could be null, so handle that case.
            ItemAnswer answerData = CreateItemAnswerObject(reader);

            if (answerData == null)
            {
                return;
            }

            AddAnswerToDictionaries(answerData);
        }

        /// <summary>
        /// Create an answer data object for the given row
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private ItemAnswer CreateItemAnswerObject(IDataReader reader)
        {
            var answerId = DbUtility.GetValueFromDataReader<long?>(reader, "AnswerId", null);

            if (!answerId.HasValue)
            {
                return null;
            }

            var answerData = new ItemAnswer
            {
                ResponseId = (long)reader["ResponseId"],
                ResponseGuid = DbUtility.GetValueFromDataReader<Guid?>(reader, "GUID", null),
                AnswerId = (long)reader["AnswerID"],
                AnswerText = DbUtility.GetValueFromDataReader(reader, "AnswerText", string.Empty).Trim(),
                ItemId = (int)reader["ItemId"],
                OptionId = DbUtility.GetValueFromDataReader<int?>(reader, "OptionId", null),
                Points = DbUtility.GetValueFromDataReader<double?>(reader, "Points", null),
            };

            if (answerData.OptionId.HasValue)
            {
                answerData.IsOther = SurveyMetaDataProxy.GetOptionIsOther(answerData.OptionId.Value);
            }

            return answerData;
        }

        /// <summary>
        /// Populate the report data answer with additional response properties
        /// </summary>
        /// <param name="reader"></param>
        private void PopulateResponseProperties(IDataReader reader)
        {
            var responseId = DbUtility.GetValueFromDataReader<long>(reader, "ResponseId", -1);

            //Make sure response id is not null
            if (responseId <= 0)
            {
                return;
            }

            //Do nothing if we have already populated the data for this response
            if (ResponsePropertiesDictionary.ContainsKey(responseId))
            {
                return;
            }

            //Populate ordinal list, if necessary
            if (AnswerReaderOrdinals.Count == 0)
            {
                ReadOnlyCollection<string> propertyNames = ResponseProperties.PropertyNames;

                foreach (string propertyName in propertyNames)
                {
                    try
                    {
                        AnswerReaderOrdinals[propertyName] = reader.GetOrdinal(propertyName);
                    }
                    catch
                    {
                        //Ignore and keep going
                    }
                }
            }

            var props = new ResponseProperties();

            foreach (string propertyName in ResponsePropertyNames)
            {
                if (AnswerReaderOrdinals.ContainsKey(propertyName))
                {
                    object propertyValue = reader[AnswerReaderOrdinals[propertyName]];

                    props.SetValue(propertyName, propertyValue == DBNull.Value ? null : propertyValue);
                }
            }

            //Special case for response guid, which is stored as "GUID" in dataset, but differentiated by
            // name in response properties collection.  Not changing response properties collection for now to
            // avoid any piping or other issues
            props.SetValue("ResponseGuid", DbUtility.GetValueFromDataReader(reader, "GUID", Guid.Empty));

            ResponsePropertiesDictionary[responseId] = props;
        }

        /// <summary>
        /// Return a filtered dictionary containing data rows for response ids.
        /// </summary>
        /// <param name="filters">Filter information.</param>
        protected void FilterResponseData(List<Filter> filters)
        {
            //Build a list of response ids to remove
            var responseIdsToRemove = ResponseAnswerObjectDictionary.Keys
                .Where(responseId => !ValidateResponse(responseId, filters))
                .ToList();

            //Remove responses & answers that did not pass the filter test
            foreach (long responseId in responseIdsToRemove)
            {
                RemoveResponsesFromDictionaries(responseId);
            }
        }

        /// <summary>
        /// Valiate answer rows for a specific response.
        /// </summary>
        /// <param name="responseId">Response to validate.</param>
        /// <param name="filters">Filters to apply.</param>
        /// <returns></returns>
        protected bool ValidateResponse(long responseId, List<Filter> filters)
        {
            foreach (Filter f in filters)
            {
                //If filter has already been evaluated for all responses, use stored value instead of running
                // again.
                if (FilterPassedResponseIds.ContainsKey(f.FilterId))
                {
                    //If response already passed for this filter, move on to next filter
                    if (FilterPassedResponseIds[f.FilterId].Contains(responseId))
                    {
                        continue;
                    }

                    //If response not passed for this filter, return false
                    return false;
                }

                if (f is IAnswerDataObjectFilter)
                {
                    if (!((IAnswerDataObjectFilter)f).EvaluateFilter(GetResponseAnswers(responseId), this, ResponsePropertiesDictionary))
                    {
                        return false;
                    }
                }
            }

            return true;
        }



        /// <summary>
        /// Get a list loaded responses ids.
        /// </summary>
        /// <returns>List of response ids.</returns>
        public List<long> ListResponseIds()
        {
            return ResponsePropertiesDictionary.Keys.OrderBy(key => key).ToList();
        }

        /// <summary>
        /// Get a list of loaded answer ids.
        /// </summary>
        /// <returns>List of answer ids.</returns>
        public List<long> ListAnswerIds()
        {
            return new List<long>(AnswerDictionary.Keys);
        }

        /// <summary>
        /// Get the properties object for a response.  The properties contains started time, end time,
        /// and other properties of the response.
        /// </summary>
        /// <param name="responseId">ID of response to get properties for.</param>
        /// <returns>Response properties object or NULL if properties object not found.</returns>
        public ResponseProperties GetResponseProperties(long responseId)
        {
            if (ResponsePropertiesDictionary.ContainsKey(responseId))
            {
                return ResponsePropertiesDictionary[responseId];
            }

            return null;
        }

        public Dictionary<long, ResponseProperties> GetAllResponseProperties()
        {
            return ResponsePropertiesDictionary;

        }

        /// <summary>
        /// Get a list of answer data objects for a given response.
        /// </summary>
        /// <param name="responseId">ID of the response to list answers for.</param>
        /// <returns>List of answer data objects.</returns>
        public List<ItemAnswer> GetResponseAnswers(long responseId)
        {
            if (ResponseAnswerObjectDictionary.ContainsKey(responseId))
            {
                return ResponseAnswerObjectDictionary[responseId];
            }

            return new List<ItemAnswer>();
        }

        /// <summary>
        /// List all answers for an item
        /// </summary>
        /// <param name="itemId">Id of the item.</param>
        /// <returns></returns>
        public List<ItemAnswer> ListItemAnswers(int itemId)
        {
            if (ItemAnswersDictionary.ContainsKey(itemId))
            {
                return ItemAnswersDictionary[itemId];
            }

            return new List<ItemAnswer>();
        }

        /// <summary>
        /// Get a filtered list of answers to a specific item.
        /// </summary>
        /// <param name="itemId">Id of the item.</param>
        /// <param name="responseTemplateId"></param>
        /// <param name="filtersToApply">Filters to apply before returning list of answers.</param>
        /// <returns>List of item answer objects maching the supplied filters.</returns>
        public List<ItemAnswer> ListItemAnswers(int itemId, int responseTemplateId, List<Filter> filtersToApply)
        {
            //Get all answers for the item
            List<ItemAnswer> answers = ListItemAnswers(itemId);

            //If there are no filters, return all answers for the item
            if (filtersToApply == null || filtersToApply.Count == 0)
            {
                return answers;
            }

            //Apply each filter to all responses, if not yet done so
            foreach (Filter f in filtersToApply)
            {
                //Run filters, if necessary
                if (!FilterPassedResponseIds.ContainsKey(f.FilterId))
                {
                    FilterPassedResponseIds[f.FilterId] = new List<long>();

                    //If not answer object filter, don't know what to do so assume passed
                    if (!(f is IAnswerDataObjectFilter))
                    {
                        FilterPassedResponseIds[f.FilterId] = new List<long>(ResponsePropertiesDictionary.Keys);
                    }
                    else
                    {
                        //Ensure profile data preloaded, if necessary
                        if (f is ProfileFilter)
                        {
                            EnsureProfileDataPreloaded(responseTemplateId);
                        }

                        //Otherwise, evaluate filter for each response
                        List<long> responseIds = new List<long>(ResponsePropertiesDictionary.Keys);
                        foreach (long responseId in responseIds)
                        {
                            if (((IAnswerDataObjectFilter)f).EvaluateFilter(GetResponseAnswers(responseId), this, ResponsePropertiesDictionary))
                            {
                                FilterPassedResponseIds[f.FilterId].Add(responseId);
                            }
                        }
                    }
                }
            }

            //If there are filters, remove answers that are not part of "valid" responses
            var validResponseIds = new List<long>();
            var invalidResponseIds = new List<long>();

            var validAnswers = new List<ItemAnswer>();

            //First, get a list of valid response ids that the specified item was answered
            // as a part of.
            foreach (ItemAnswer answer in answers)
            {
                //First, validate or invalidate the response if it has not been validated
                if (!validResponseIds.Contains(answer.ResponseId)
                    && !invalidResponseIds.Contains(answer.ResponseId))
                {
                    if (ValidateResponse(answer.ResponseId, filtersToApply))
                    {
                        //Add response to valid responses list
                        validResponseIds.Add(answer.ResponseId);

                        //Add answer to valid answer list
                        validAnswers.Add(answer);
                    }
                    else
                    {
                        //Add response id to invalid responses list
                        invalidResponseIds.Add(answer.ResponseId);
                    }
                }
                //If response id was already validated, add the answer
                else if (validResponseIds.Contains(answer.ResponseId))
                {
                    validAnswers.Add(answer);
                }
            }

            return validAnswers;
        }

        /// <summary>
        /// Get answers for the given response and item.
        /// </summary>
        /// <param name="responseId">ID of the response.</param>
        /// <param name="itemId">ID of item to get answers for.</param>
        /// <returns>List of answers to the item from the specified response.</returns>
        public List<ItemAnswer> ListItemResponseAnswers(long responseId, int itemId)
        {
            var answers = new List<ItemAnswer>();
            var key = new Pair<int, long>(itemId, responseId);

            if (ItemResponseAnswersDictionary.ContainsKey(key))
            {
                answers.AddRange(from answerId in ItemResponseAnswersDictionary[key]
                                 where AnswerDictionary.ContainsKey(answerId)
                                 select AnswerDictionary[answerId]);
            }

            return answers;
        }

        /// <summary>
        /// Get answers for the given response and item.
        /// </summary>
        /// <param name="responseId">ID of the response.</param>
        /// <param name="itemId">ID of item to get answers for.</param>
        /// <param name="responseTemplateId"> </param>
        /// <returns>List of answers to the item from the specified response.</returns>
        public List<ItemAnswer> ListItemResponseAnswers(long responseId, int itemId, int responseTemplateId)
        {
            var answers = new List<ItemAnswer>();
            var key = new Pair<int, long>(itemId, responseId);

            if (ItemResponseAnswersDictionary.ContainsKey(key))
            {
                answers.AddRange(from answerId in ItemResponseAnswersDictionary[key]
                                 where AnswerDictionary.ContainsKey(answerId)
                                 select AnswerDictionary[answerId]);
            }
            else
            {
                Load(new List<int> {itemId}, 
                    responseTemplateId,
                    new AnalysisFilterCollection(),
                    null,
                    null,
                    true,
                    true,
                    string.Empty);

                return ListItemResponseAnswers(responseId, itemId);
            }

            return answers;
        }

        /// <summary>
        /// Get answers for the given response and item option.
        /// </summary>
        /// <param name="responseId">ID of the response.</param>
        /// <param name="itemId"></param>
        /// <param name="optionId">ID of the option.</param>
        /// <returns>List of answers for the selected option in the specified response.</returns>
        public ItemAnswer GetOptionAnswer(long responseId, int itemId, int optionId)
        {
            var key = new Triplet<int, int, long>(itemId, optionId, responseId);

            if (ItemOptionResponseAnswersDictionary.ContainsKey(key))
            {
                return AnswerDictionary[ItemOptionResponseAnswersDictionary[key]];
            }

            return null;
        }

        /// <summary>
        /// Calculate the "score" for a given response id.
        /// </summary>
        /// <param name="responseId">ID of response to calculate score of.</param>
        /// <param name="specifiedItems"></param>
        /// <returns>Total score for the response.</returns>
        public double CalculateResponseScore(long responseId, List<Item> specifiedItems = null)
        {
            if (specifiedItems != null)
            {
                return GetResponseAnswers(responseId)
                    .Where(answerData => answerData.Points.HasValue && specifiedItems.Any(i => i.ID == answerData.ItemId || (i is TabularItem && ((TabularItem)i).Items.Any(c => c.ID == answerData.ItemId))) )
                    .Sum(answerData => answerData.Points.Value);
            }

            return GetResponseAnswers(responseId)
                .Where(answerData => answerData.Points.HasValue)
                .Sum(answerData => answerData.Points.Value);
        }

        /// <summary>
        /// Calculate detailed score info for a given response id.
        /// </summary>
        public DetailedScoreData CalculateResponseDetailedScoreData(Response response, long responseId, ResponseTemplate rt)
        {
            if (!ResponsePropertiesDictionary.ContainsKey(responseId) ||
                !ResponseAnswerObjectDictionary.ContainsKey(responseId))
                return null;

            var pageScoreData = new List<PageScoreData>();

            var state = new ResponseState();
            state.Load(rt.ID.Value, ResponsePropertiesDictionary[responseId], ResponseAnswerObjectDictionary[responseId]);
            response.RestoreForScoreCalculation(state);

            double surveyScore = 0d;
            double surveyPossibleScore = 0d;

            var pages = response.GetResponsePages();
            foreach (var page in pages)
            {
                if (page.PageType != TemplatePageType.ContentPage)
                    continue;

                double score = 0d;
                double maxScore = 0d;

                if (!page.Excluded)
                {
                    score = page.Items.Where(i => !i.Excluded).OfType<IScored>().Sum(i => i.GetScore());
                    maxScore = page.Items.Where(i => !i.Excluded).OfType<IScored>().Sum(i => i.GetPossibleMaxScore());

                    surveyScore += score;
                    surveyPossibleScore += maxScore;
                }

                pageScoreData.Add(new PageScoreData
                                  {
                                      IsExcluded = page.Excluded,
                                      Position = page.Position - 1,
                                      CurrentScore = score,
                                      MaxPossibleScore = maxScore
                                  });
            }

            return new DetailedScoreData
                   {
                       CurrentSurveyScore = surveyScore,
                       PossibleSurveyMaxScore = surveyPossibleScore,
                       PageScores = pageScoreData
                   };
        }

        /// <summary>
        /// Get set range start
        /// </summary>
        public DateTime? DateRangeStart { get; set; }

        /// <summary>
        /// Get/set range end
        /// </summary>
        public DateTime? DateRangeEnd { get; set; }
    }
}
