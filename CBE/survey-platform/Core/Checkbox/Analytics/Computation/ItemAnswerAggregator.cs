using System;

using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics.Data;
using Checkbox.Forms.Data;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Implementation class for item answer aggregation
    /// </summary>
    public class ItemAnswerAggregator
    {
        private List<string> _groupNames;
        private Dictionary<string, List<int>> _itemGroups;
        private Dictionary<int, GroupedReportOptionData> _optionDictionary;
        private Dictionary<int, List<int>> _itemOptionDictionary;
        private Dictionary<int, GroupedReportItemData> _itemDictionary;
        private Dictionary<long, List<long>> _responseAnswerDictionary;
        private Dictionary<long, ItemAnswer> _answerDictionary;
        private Dictionary<int, Dictionary<long, int>> _itemResponses;
        private Dictionary<int, Dictionary<long, int>> _optionResponses;
        private Dictionary<int, List<long>> _itemAnswerDictionary;
        private Dictionary<int, List<long>> _optionAnswerDictionary;
        private Dictionary<int, Dictionary<int, List<long>>> _itemOptionAnswerCountsDictionary;

        private List<DetailResult> _answerData;

        /// <summary>
        /// Extension of report item data class to include grouping information
        /// </summary>
        protected class GroupedReportItemData : LightweightItemMetaData
        {
            /// <summary>
            /// Get/set the name of the group the item belongs 
            /// </summary>
            public string GroupName { get; set; }

            /// <summary>
            /// Get/set the item's "position" relative other items.
            /// </summary>
            public int Position { get; set; }
        }

        /// <summary>
        /// Extension of report option data class to include grouping information
        /// </summary>
        protected class GroupedReportOptionData : LightweightOptionMetaData
        {
            /// <summary>
            /// Get/set the group name
            /// </summary>
            public string GroupName { get; set; }
        }


        /// <summary>
        /// Construct a new item answer aggregator
        /// </summary>
        public ItemAnswerAggregator(bool preferAlias)
        {
            InitializeAnswerData();
            PreferAlias = preferAlias;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DetailResult> AnswerData
        {
            get { return _answerData ?? (_answerData = new List<DetailResult>()); }
        }

        /// <summary>
        /// Get whether aliases are used
        /// </summary>
        public bool PreferAlias { get; private set; }

        /// <summary>
        /// Get the list of group names
        /// </summary>
        protected List<string> GroupNames
        {
            get { return _groupNames ?? (_groupNames = new List<string>()); }
        }

        /// <summary>
        /// Get a dictionary containing a list of answer ids for a given
        /// item.
        /// </summary>
        public Dictionary<int, List<long>> ItemAnswers
        {
            get { return _itemAnswerDictionary ?? (_itemAnswerDictionary = new Dictionary<int, List<long>>()); }
        }

        /// <summary>
        /// Get a dictionary containing a list of answers for a given
        /// item option.
        /// </summary>
        public Dictionary<int, List<long>> OptionAnswers
        {
            get { return _optionAnswerDictionary ?? (_optionAnswerDictionary = new Dictionary<int, List<long>>()); }
        }

        /// <summary>
        /// Get a dictionary containing a list of answers counts for a given
        /// item and given option.
        /// </summary>
        public Dictionary<int, Dictionary<int, List<long>>> ItemOptionAnswerCounts
        {
            get { return _itemOptionAnswerCountsDictionary ?? (_itemOptionAnswerCountsDictionary = new Dictionary<int, Dictionary<int, List<long>>>()); }
        }

        /// <summary>
        /// Get the item response ids.  Key is the id of the item and
        /// The value is a dictionary instead of a list so that fast "contains" 
        /// access can be used.  Value is a dictionary of response ids and # of 
        /// answers for the item for the response id
        /// </summary>
        public Dictionary<int, Dictionary<long, int>> ItemResponses
        {
            get { return _itemResponses ?? (_itemResponses = new Dictionary<int, Dictionary<long, int>>()); }
        }

        /// <summary>
        /// Get the option response ids.  Key is the id of the option and
        /// The value is a dictionary instead of a list so that fast "contains" 
        /// access can be used.  Value is a dictionary of response ids and # of 
        /// answers for the option for the response id
        /// </summary>
        public Dictionary<int, Dictionary<long, int>> OptionResponses
        {
            get { return _optionResponses ?? (_optionResponses = new Dictionary<int, Dictionary<long, int>>()); }
        }

        /// <summary>
        /// Get a list of group names
        /// </summary>
        public Dictionary<string, List<int>> ItemGroups
        {
            get { return _itemGroups ?? (_itemGroups = new Dictionary<string, List<int>>()); }
        }

        /// <summary>
        /// Get the item options dictionary
        /// </summary>
        public Dictionary<int, List<int>> ItemOptionDictionary
        {
            get { return _itemOptionDictionary ?? (_itemOptionDictionary = new Dictionary<int, List<int>>()); }
        }

        /// <summary>
        /// Get the dictionary of options
        /// </summary>
        protected Dictionary<int, GroupedReportOptionData> OptionDictionary
        {
            get { return _optionDictionary ?? (_optionDictionary = new Dictionary<int, GroupedReportOptionData>()); }
        }

        /// <summary>
        /// Get the item dictionary
        /// </summary>
        protected Dictionary<int, GroupedReportItemData> ItemDictionary
        {
            get { return _itemDictionary ?? (_itemDictionary = new Dictionary<int, GroupedReportItemData>()); }
        }

        /// <summary>
        /// Get the response answer dictionary
        /// </summary>
        public Dictionary<long, List<long>> ResponseAnswerDictionary
        {
            get { return _responseAnswerDictionary ?? (_responseAnswerDictionary = new Dictionary<long, List<long>>()); }
        }

        /// <summary>
        /// Get the answer dictionary
        /// </summary>
        public Dictionary<long, ItemAnswer> AnswerDictionary
        {
            get { return _answerDictionary ?? (_answerDictionary = new Dictionary<long, ItemAnswer>()); }
        }

        /// <summary>
        /// Initialize the data store
        /// </summary>
        private void InitializeAnswerData()
        {
            //Clear collections
            ResponseAnswerDictionary.Clear();
            AnswerDictionary.Clear();
            ItemDictionary.Clear();
            ItemOptionDictionary.Clear();
            OptionDictionary.Clear();
            ItemGroups.Clear();
            GroupNames.Clear();
            OptionAnswers.Clear();
            OptionResponses.Clear();
            ItemAnswers.Clear();
            ItemResponses.Clear();
            ItemOptionAnswerCounts.Clear();

            AnswerData.Clear();
        }

        /// <summary>
        /// Add an answer group.
        /// </summary>
        /// <param name="groupName">Name of the group to add.</param>
        /// <remarks>The group will be added to the end of the list</remarks>
        public virtual void AddAnswerGroup(string groupName)
        {
            GroupNames.Add(groupName);
            ItemGroups[groupName] = new List<int>();
        }

        /// <summary>
        /// Add an item.
        /// </summary>
        /// <param name="itemID">ID of the item to add.</param>
        /// <param name="itemText">Text associated with the item.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <remarks>The item will not be in a group and will be added to the end of the list.</remarks>
        public virtual void AddItem(Int32 itemID, string itemText, string itemType)
        {
            AddItem(itemID, itemText, itemType, null);
        }

        /// <summary>
        /// Add an item and insert the item into an item group.
        /// </summary>
        /// <param name="itemID">ID of the item to add.</param>
        /// <param name="itemText">Text of the item.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <remarks>The item will be added to the end  of the list of items in the group.</remarks>
        public virtual void AddItem(Int32 itemID, string itemText, string itemType, string groupName)
        {
            var itemData = new GroupedReportItemData
            {
                ItemId = itemID,
                GroupName = groupName,
                Position = (ItemDictionary.Count + 1),
                ItemType = itemType
            };

            //Set text, actual language is not important as long as same
            // language is used when getting text
            itemData.SetText("[LANGUAGE]", itemText);

            ItemDictionary[itemID] = itemData;
        }

        /// <summary>
        /// Add an item option to the data
        /// </summary>
        /// <param name="itemID">ID of the item the option is associated with.</param>
        /// <param name="optionID">ID of the option to add.</param>
        /// <param name="optionText">Text of the option to add.</param>
        public virtual void AddItemOption(Int32 itemID, Int32 optionID, string optionText)
        {
            AddItemOption(itemID, optionID, optionText, null, false);
        }

        /// <summary>
        /// Add an item option to the data.
        /// </summary>
        /// <param name="itemID">ID of the item the option is associated with.</param>
        /// <param name="optionID">ID of the option to add.</param>
        /// <param name="optionText">Text of the option to add.</param>
        /// <param name="points">Point value for the option</param>
        /// <param name="isOther">Specify whether the option is an "other" option or not.</param>
        public virtual void AddItemOption(Int32 itemID, Int32 optionID, string optionText, double? points, bool isOther)
        {
            int optionPosition;

            if (ItemOptionDictionary.ContainsKey(itemID))
            {
                optionPosition = ItemOptionDictionary[itemID].Count + 1;
            }
            else
            {
                ItemOptionDictionary[itemID] = new List<int>();
                optionPosition = 1;
            }

            var optionData = new GroupedReportOptionData
            {
                ItemId = itemID,
                OptionId = optionID,
                Points = points ?? 0,
                Position = optionPosition,
                IsOther = isOther
            };

            //Set text, actual language is not important as long as same
            // language is used when getting text
            optionData.SetText("[LANGUAGE]", optionText);

            ItemOptionDictionary[itemID].Add(optionID);
            OptionDictionary[optionID] = optionData;

        }

        /// <summary>
        /// Add an open-ended answer to the data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response associated with the item.</param>
        /// <param name="itemID">ID of the item.</param>
        /// <param name="answer">Answer text</param>
        /// <remarks>This method should be used only for open-ended items, such as single and multi line texts.</remarks>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Int32 itemID, string answer)
        {
            AddAnswer(answerId, responseID, itemID, null, answer);
        }

        /// <summary>
        /// Update the answer to an item
        /// </summary>
        /// <param name="answerId">Answer id to update.</param>
        /// <param name="answer">New answer</param>
        protected virtual void UpdateAnswer(long answerId, string answer)
        {
            if (AnswerDictionary.ContainsKey(answerId))
            {
                AnswerDictionary[answerId].AnswerText = answer;
            }
        }

        /// <summary>
        /// Add an option answer to the data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response.</param>
        /// <param name="itemID">ID of the answer.</param>
        /// <param name="optionID">ID of the option.</param>
        /// <remarks>This method should be used for item options that are NOT other options.</remarks>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Int32 itemID, int? optionID)
        {
            AddAnswer(answerId, responseID, itemID, optionID, null);
        }

        /// <summary>
        /// Add an other option to the answer data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response.</param>
        /// <param name="itemID">ID of the item.</param>
        /// <param name="optionID">ID of the option.</param>
        /// <param name="answer">Answer</param>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Int32 itemID, int? optionID, string answer)
        {
            AddAnswer(answerId, responseID, null, itemID, optionID, answer);
        }

        /// <summary>
        /// Add an other option to the answer data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response.</param>
        /// <param name="responseGuid">GUID of the response</param>
        /// <param name="itemID">ID of the item.</param>
        /// <param name="optionID">ID of the option.</param>
        /// <param name="answer">Answer</param>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Guid? responseGuid, Int32 itemID, int? optionID, string answer)
        {
            AddAnswer(answerId, responseID, responseGuid, itemID, optionID, answer, null, null);
        }

        internal void AddAnswerWithPoints(Int64 answerId, Int64 responseID, Int32 itemID, int? optionID, double? points)
        {
            AddAnswer(answerId, responseID, null, itemID, optionID, null, points, null);
        }

        internal void AddAnswerWithPointsAndPage(Int64 answerId, Int64 responseID, Int32 itemID, int? optionID, double? points, int? pageId)
        {
            AddAnswer(answerId, responseID, null, itemID, optionID, null, points, pageId);
        }

        /// <summary>
        /// Add an other option to the answer data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response.</param>
        /// <param name="responseGuid">GUID of the response</param>
        /// <param name="itemID">ID of the item.</param>
        /// <param name="optionID">ID of the option.</param>
        /// <param name="answer">Answer</param>
        /// <param name="points">Points of the answer</param>
        /// <param name="pageId"></param>
        /// <param name="count"></param>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Guid? responseGuid, Int32 itemID, int? optionID, string answer, double? points, int? pageId, int? count)
        {
            var answerData = new ItemAnswer
            {
                AnswerId = answerId,
                ResponseId = responseID,
                ResponseGuid = responseGuid,
                ItemId = itemID,
                OptionId = optionID,
                AnswerText = answer,
                Points = points,
                PageId = pageId,
            };

            if (count.HasValue)
                answerData.Count = count.Value;

            AddAnswer(answerData);
        }

        /// <summary>
        /// Add an other option to the answer data.
        /// </summary>
        /// <param name="answerId">ID of the answer</param>
        /// <param name="responseID">ID of the response.</param>
        /// <param name="responseGuid">GUID of the response</param>
        /// <param name="itemID">ID of the item.</param>
        /// <param name="optionID">ID of the option.</param>
        /// <param name="answer">Answer</param>
        /// <param name="points">Points of the answer</param>
        /// <param name="pageId"></param>
        public virtual void AddAnswer(Int64 answerId, Int64 responseID, Guid? responseGuid, Int32 itemID, int? optionID, string answer, double? points, int? pageId)
        {
            AddAnswer(answerId, responseID, responseGuid, itemID, optionID, answer, points, pageId, null);
        }

        /// <summary>
        /// Store the provided answer data object in the answer dictionary and 
        /// also update the other dictionaries and list used internally for quicker
        /// data access.
        /// </summary>
        /// <param name="answerData"></param>
        private void AddAnswer(ItemAnswer answerData)
        {
            //Store the answer
            AnswerDictionary[answerData.AnswerId] = answerData;

            //Link answer to a response
            AddResponseAnswer(answerData.ResponseId, answerData.AnswerId);

            //Link respones to an item & option
            AddItemResponse(answerData.ItemId, answerData.OptionId, answerData.ResponseId);

            //Link answer to an item & option
            AddItemAnswer(answerData.ItemId, answerData.OptionId, answerData.AnswerId);
        }

        /// <summary>
        /// Add a value to the response answers dictionary
        /// </summary>
        /// <param name="responseId"></param>
        /// <param name="answerId"></param>
        private void AddResponseAnswer(long responseId, long answerId)
        {
            //Link the answer to a response
            if (!ResponseAnswerDictionary.ContainsKey(responseId))
            {
                ResponseAnswerDictionary[responseId] = new List<long>();
            }

            ResponseAnswerDictionary[responseId].Add(answerId);
        }

        /// <summary>
        /// Link a response to an item and also link the response to an
        /// option, if applicable
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"></param>
        /// <param name="responseId"></param>
        private void AddItemResponse(int itemId, int? optionId, long responseId)
        {
            //Link the response to an item and store # of answers
            if (!ItemResponses.ContainsKey(itemId))
            {
                ItemResponses[itemId] = new Dictionary<long, int>();
            }

            if (!ItemResponses[itemId].ContainsKey(responseId))
            {
                ItemResponses[itemId][responseId] = 1;
            }
            else
            {
                ItemResponses[itemId][responseId]++;
            }

            if (optionId.HasValue)
            {
                //Link response to an option, and store # of answers
                if (!OptionResponses.ContainsKey(optionId.Value))
                {
                    OptionResponses[optionId.Value] = new Dictionary<long, int>();
                }

                if (!OptionResponses[optionId.Value].ContainsKey(responseId))
                {
                    OptionResponses[optionId.Value][responseId] = 1;
                }
                else
                {
                    OptionResponses[optionId.Value][responseId]++;
                }

                //Add response to the dictionary by item and option
                if (!ItemOptionAnswerCounts.ContainsKey(itemId))
                {
                    ItemOptionAnswerCounts[itemId] = new Dictionary<int, List<long>>();
                }
                if (!ItemOptionAnswerCounts[itemId].ContainsKey(optionId.Value))
                {
                    ItemOptionAnswerCounts[itemId][optionId.Value] = new List<long>();
                }
                if (!ItemOptionAnswerCounts[itemId][optionId.Value].Contains(responseId))
                {
                    ItemOptionAnswerCounts[itemId][optionId.Value].Add(responseId);
                }
            }
        }

        /// <summary>
        /// Link an answer to an item and option
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"></param>
        /// <param name="answerId"></param>
        private void AddItemAnswer(int itemId, int? optionId, long answerId)
        {
            if (!ItemAnswers.ContainsKey(itemId))
            {
                ItemAnswers[itemId] = new List<long>();
            }

            ItemAnswers[itemId].Add(answerId);

            if (optionId.HasValue)
            {
                if (!OptionAnswers.ContainsKey(optionId.Value))
                {
                    OptionAnswers[optionId.Value] = new List<long>();
                }

                OptionAnswers[optionId.Value].Add(answerId);
            }
        }

        /// <summary>
        /// Get a count of the total number of responses for an item.  If itemid is
        /// null, total response count is returned.
        /// </summary>
        /// <returns>Number of unique response ids.</returns>
        public virtual Int32 GetResponseCount(int? itemId)
        {
            if (!itemId.HasValue)
            {
                return ResponseAnswerDictionary.Count;
            }

            if (ItemResponses.ContainsKey(itemId.Value))
            {
                return ItemResponses[itemId.Value].Count;
            }

            return 0;
        }

        /// <summary>
        /// Get sum points for the specified item
        /// </summary>
        /// <param name="itemID">ID of item to get sum points for.</param>
        /// <returns></returns>
        public virtual double GetItemSumPoints(int itemID)
        {
            if (ItemAnswers.ContainsKey(itemID))
            {
                return ItemAnswers[itemID].Sum(p => AnswerDictionary[p].Points) ?? 0;
            }

            return 0;
        }

        /// <summary>
        /// Get the number of answers for an item
        /// </summary>
        /// <param name="itemId">ID of item to get number of answers for.</param>
        /// <returns>Number of answers for the item.</returns>
        public virtual Int32 GetItemAnswerCount(int itemId)
        {
            if (ItemAnswers.ContainsKey(itemId))
            {
                return ItemAnswers[itemId].Count;
            }

            return 0;
        }

        /// <summary>
        /// Get the number of answers to the item with the specified answer text.
        /// </summary>
        /// <param name="itemId">ID of item to get answer count for.</param>
        /// <param name="answerText">Text of answer to count.</param>
        /// <returns>Number of answers for the item with the specified answer text.  Non-open-ended answers (i.e. select item) are not included in this total.</returns>
        public virtual Int32 GetItemAnswerCount(int itemId, string answerText)
        {
            int answerCount = 0;

            if (ItemAnswers.ContainsKey(itemId))
            {
                List<long> itemAnswers = ItemAnswers[itemId];

                answerCount = (from answerId in itemAnswers
                               where AnswerDictionary.ContainsKey(answerId)
                               select AnswerDictionary[answerId]).Count(answer => string.Compare(answer.AnswerText, answerText, true) == 0);
            }

            return answerCount;
        }

        /// <summary>
        /// Get the number of answers for an option.
        /// </summary>
        /// <param name="optionId">ID of option to get answer count for.</param>
        /// <returns>Number of answers for the option.</returns>
        public virtual Int32 GetOptionAnswerCount(int optionId)
        {
            if (OptionAnswers.ContainsKey(optionId))
            {
                return OptionAnswers[optionId].Count;
            }

            return 0;
        }

        /// <summary>
        /// Get the points sum of the specified option across all the answers.
        /// </summary>
        /// <param name="optionId">ID of option to get sum points for.</param>
        /// <returns></returns>
        public virtual double GetOptionSumPoints(int optionId)
        {
            if (OptionAnswers.ContainsKey(optionId))
            {
                return OptionAnswers[optionId].Sum(p => AnswerDictionary[p].Points) ?? 0;
            }

            return 0;
        }

        /// <summary>
        /// Get the number of answers for an option.
        /// </summary>
        /// <param name="optionId">ID of option to get answer count for.</param>
        /// <returns>Number of answers for the option.</returns>
        public virtual Int32 GetOptionAnswerCount(int itemId, int optionId)
        {
            if (ItemOptionAnswerCounts.ContainsKey(itemId))
            {
                if (ItemOptionAnswerCounts[itemId].ContainsKey(optionId))
                {
                    return ItemOptionAnswerCounts[itemId][optionId].Count;
                }
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Get the number of groups
        /// </summary>
        /// <returns>Number of item groups.</returns>
        public virtual Int32 GetGroupCount()
        {
            return GroupNames.Count;
        }

        /// <summary>
        /// Get the group text based on group position.
        /// </summary>
        /// <param name="position">Position of the group</param>
        /// <returns>Group name</returns>
        public virtual string GetGroupName(Int32 position)
        {
            if (position > 0 && (position - 1 < GroupNames.Count))
            {
                return GroupNames[position - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get a group's position based on it's name
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>Position</returns>
        public virtual int? GetGroupPosition(string groupName)
        {
            int position = GroupNames.IndexOf(groupName);

            if (position > 0)
            {
                return position;
            }

            return null;
        }

        /// <summary>
        /// Get the group text based on group position.
        /// </summary>
        /// <param name="itemID">ItemID</param>
        /// <returns>Group name</returns>
        public virtual string GetGroupNameForItem(Int32 itemID)
        {
            if (ItemDictionary.ContainsKey(itemID))
            {
                return ItemDictionary[itemID].GroupName ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the position of an item.
        /// </summary>
        /// <param name="itemID">ItemID</param>
        /// <returns>Group name</returns>
        public virtual Int32 GetItemPosition(Int32 itemID)
        {
            if (ItemDictionary.ContainsKey(itemID))
            {
                return ItemDictionary[itemID].Position;
            }

            return -1;
        }

        /// <summary>
        /// Get the type of the item.
        /// </summary>
        /// <param name="itemID">ID of item to get type of.</param>
        /// <returns>Type name of item.</returns>
        public virtual string GetItemType(Int32 itemID)
        {
            if (ItemDictionary.ContainsKey(itemID))
            {
                return ItemDictionary[itemID].ItemType;
            }

            return string.Empty;
        }


        /// <summary>
        /// Get the text for an item.
        /// </summary>
        /// <param name="itemID">ID of an item.</param>
        /// <returns>Item's text.</returns>
        public virtual string GetItemText(Int32 itemID)
        {
            if (ItemDictionary.ContainsKey(itemID))
            {
                return ItemDictionary[itemID].GetText(PreferAlias, "[LANGUAGE]");
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the id of the parent item of an option.
        /// </summary>
        /// <param name="optionID">ID of the option.</param>
        /// <returns>Item ID</returns>
        public virtual Int32 GetItemID(Int32 optionID)
        {
            if (OptionDictionary.ContainsKey(optionID))
            {
                return OptionDictionary[optionID].ItemId;
            }

            return -1;
        }

        /// <summary>
        /// Get the position of an option
        /// </summary>
        /// <param name="optionID">ID of the option.</param>
        /// <returns>Option position</returns>
        public virtual Int32 GetOptionPosition(Int32 optionID)
        {
            if (OptionDictionary.ContainsKey(optionID))
            {
                return OptionDictionary[optionID].Position;
            }

            return -1;
        }

        /// <summary>
        /// Get the text of an option
        /// </summary>
        /// <param name="optionID">ID of the option.</param>
        /// <returns>Option text</returns>
        public virtual string GetOptionText(Int32 optionID)
        {
            if (OptionDictionary.ContainsKey(optionID))
            {
                return OptionDictionary[optionID].GetText(PreferAlias, "[LANGUAGE]");
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the point value for an option
        /// </summary>
        /// <param name="optionID"></param>
        /// <returns></returns>
        public virtual double GetOptionPoints(Int32 optionID)
        {
            if (OptionDictionary.ContainsKey(optionID))
            {
                return OptionDictionary[optionID].Points;
            }

            return 0;
        }

        /// <summary>
        /// Return a boolean value indicating if the specified option is an "other" 
        /// option.
        /// </summary>
        /// <param name="optionID">ID of option to check.</param>
        /// <returns></returns>
        public virtual bool GetOptionIsOther(Int32 optionID)
        {
            if (OptionDictionary.ContainsKey(optionID))
            {
                return OptionDictionary[optionID].IsOther;
            }

            return false;
        }

        /// <summary>
        /// Get a list of response ids
        /// </summary>
        /// <returns>List of response ids</returns>
        public virtual List<Int64> GetResponseIDs()
        {
            return new List<long>(ResponseAnswerDictionary.Keys);
        }

        /// <summary>
        /// Get a list of option ids, ordered by position
        /// </summary>
        /// <param name="itemID">ID of options' parent item.</param>
        /// <returns>List of option ids</returns>
        public virtual List<Int32> GetOptionIDs(Int32 itemID)
        {
            if (ItemOptionDictionary.ContainsKey(itemID))
            {
                return ItemOptionDictionary[itemID];
            }

            return new List<int>();
        }

        /// <summary>
        /// Get a list of all item ids ordered by position
        /// </summary>
        /// <returns>List of item ids.</returns>
        public virtual List<Int32> GetItemIDs()
        {
            return new List<int>(ItemDictionary.Keys);
        }

        /// <summary>
        /// Get a list of item ids
        /// </summary>
        /// <param name="groupName">Name of the group.  If null, all items not in groups are returned, if empty all items are returned.</param>
        /// <returns>List of item ids.</returns>
        public virtual List<Int32> GetItemIDs(string groupName)
        {
            if (ItemGroups.ContainsKey(groupName))
            {
                return ItemGroups[groupName];
            }

            return new List<int>();
        }

        /// <summary>
        /// Get the answer data
        /// </summary>
        /// <returns></returns>
        public List<DetailResult> GetAggregatedAnswerData()
        {
            if (AnswerData.Count == 0)
            {
                PopulateAggregatedAnswerDataProtected();
            }

            return AnswerData;
        }

        /// <summary>
        /// Get the aggregated answer data
        /// </summary>
        /// <returns>DataTable with all data in a single table.</returns>
        protected virtual void PopulateAggregatedAnswerDataProtected()
        {
            //Clear data
            AnswerData.Clear();

            //Build a list of response ids
            var responseIds = GetResponseIDs();

            //Sort the list
            responseIds.Sort();

            //Now build the output table
            foreach (var responseId in responseIds)
            {
                var answerIds = ResponseAnswerDictionary[responseId];

                foreach (var answerId in answerIds)
                {
                    if (!AnswerDictionary.ContainsKey(answerId))
                    {
                        continue;
                    }

                    var answer = AnswerDictionary[answerId];

                    //Build an object array to use to add values to the data table
                    var dataValue = new DetailResult
                                        {
                                            ResponseId = responseId,
                                            ResponseGuid = answer.ResponseGuid,
                                            //ItemPosition = GetItemPosition(answer.ItemId),
                                            ItemId = answer.ItemId,
                                            ResultText = answer.AnswerText,
                                            ResultKey = answer.ItemId.ToString(),
                                            //ItemText = GetItemText(answer.ItemId),
                                            //GroupName = GetGroupName(answer.ItemId),
                                            OptionId = answer.OptionId,
                                            PageId = answer.PageId
                                        };


                    //Get group values
                    dataValue.ResultIndex = GetGroupPosition(dataValue.ResultKey) ?? 0;

                    if (answer.OptionId.HasValue)
                    {
                        //dataValue.OptionPosition = GetOptionPosition(answer.OptionId.Value);
                        //dataValue.OptionText = GetOptionText(answer.OptionId.Value);
                        dataValue.AnswerScore = GetOptionPoints(answer.OptionId.Value);
                        dataValue.IsAnswerOther = GetOptionIsOther(answer.OptionId.Value);
                    }
                    else
                    {
                        if (answer.Points != null)
                        {
                            dataValue.AnswerScore = Convert.ToSingle(answer.Points);
                        }
                    }


                    AnswerData.Add(dataValue);
                }
            }
        }

        /// <summary>
        /// Get a list of answer texts for an item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public virtual List<string> GetAnswerTexts(int itemID)
        {
            return GetAggregatedAnswerData()
                    .Where(answer => answer.ItemId == itemID)
                    .Select(answer => answer.ResultText)
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .ToList();
        }

        /// <summary>
        /// List answers for a response
        /// </summary>
        /// <param name="responseId"></param>
        /// <returns></returns>
        protected List<ItemAnswer> ListResponseAnswers(long responseId)
        {
            var answers = new List<ItemAnswer>();

            if (ResponseAnswerDictionary.ContainsKey(responseId))
            {
                List<long> answerIds = ResponseAnswerDictionary[responseId];

                answers.AddRange(from answerId in answerIds
                                 where AnswerDictionary.ContainsKey(answerId)
                                 select AnswerDictionary[answerId]);
            }

            return answers;
        }
    }
}
