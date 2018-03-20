using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Globalization.Text;
using Prezza.Framework.Caching;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Data
{
    /// <summary>
    /// Proxy class for faster, cached access to survey item and option meta data used
    /// in many places in the application where full item or response template configuration
    /// is not necessary.
    /// </summary>
    public static class SurveyMetaDataProxy
    {
        private static readonly CacheManager _surveyItemCache;
        private static readonly CacheManager _surveyOptionCache;
        private static readonly CacheManager _surveyListOptionDataCache;
        private static readonly CacheManager _surveyRulesCache;

        /// <summary>
        /// Static constructor to initialize cache manager
        /// </summary>
        static SurveyMetaDataProxy()
        {
            lock (typeof(SurveyMetaDataProxy))
            {
                _surveyItemCache = CacheFactory.GetCacheManager("surveyItemMetaDataCache");
                _surveyOptionCache = CacheFactory.GetCacheManager("surveyItemOptionMetaDataCache");
                _surveyListOptionDataCache = CacheFactory.GetCacheManager("surveyListOptionDataCache");
                _surveyRulesCache = CacheFactory.GetCacheManager("surveyConditionsCacheManager");
            }
        }

        /// <summary>
        /// Generate a cache key for survey items
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private static string GenerateItemCacheKey(int itemId)
        {
            return itemId.ToString();
        }

        /// <summary>
        /// Generate a cache key for survey items
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        private static string GenerateSurveyRulesCacheKey(int surveyId)
        {
            return "RulesEngineForSurvey_" + surveyId;
        }

        /// <summary>
        /// Generate a cache key for survey items
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        private static string GenerateRuleDataServiceCacheKey(int surveyId)
        {
            return "RuleDataServiceForSurvey_" + surveyId;
        }

        /// <summary>
        /// Generate a cache key for item options.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private static string GenerateItemOptionCacheKey(int optionId)
        {
            return optionId.ToString();
        }

        /// <summary>
        /// Generate a cache key for item options.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private static string GenerateListOptionDataCacheKey(int optionId)
        {
            return optionId.ToString();
        }

        /// <summary>
        /// Check to see if an item is valid.  An item will be considered invalid if the template
        /// that contains it has a modified date later than the reference date of the item's mapping.
        /// If the item is valid and contained in the cache, it will be returned.  Otherwise
        /// a null value will be returned.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="skipDateValidation"></param>
        /// <returns></returns>
        private static LightweightItemMetaData GetItemFromCache(int itemId, bool skipDateValidation)
        {
            string itemCacheKey = GenerateItemCacheKey(itemId);

            if (_surveyItemCache.Contains(itemCacheKey))
            {
                var itemMetaData = _surveyItemCache[itemCacheKey] as LightweightItemMetaData;

                //If the item is in the cache, also check the mapping table and verify that the template
                // containing the item has not been modified.
                if (itemMetaData != null && (skipDateValidation || itemMetaData.Validate()))
                {
                    return itemMetaData;
                }
            }

            //Item is not in cache, or no mapping data was found, or item's containing
            // template has been modified.
            return null;
        }

        /// <summary>
        /// Get an option from the option cache.  An option will be considered invalid if the template
        /// that contains the item that contains it has a modified date later than the reference date 
        /// of the item's mapping.   If the item is valid and contained in the cache, it will be
        /// returned.  Otherwise a null value will be returned.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private static ListOptionData GetListOptionDataFromCache(int optionId)
        {
            string itemCacheKey = GenerateListOptionDataCacheKey(optionId);

            if (_surveyListOptionDataCache.Contains(itemCacheKey))
            {
                return _surveyListOptionDataCache[itemCacheKey] as ListOptionData;
            }

            //Item is not in cache, or no mapping data was found, or item's containing
            // template has been modified.
            return null;
        }

        /// <summary>
        /// Add a lightweight item to the cache.
        /// </summary>
        /// <param name="listOptionData"></param>
        private static void AddListOptionDataToCache(ListOptionData listOptionData)
        {
            _surveyListOptionDataCache.Add(GenerateListOptionDataCacheKey(listOptionData.OptionID), listOptionData);
        }

        /// <summary>
        /// Add a lightweight item to the cache.
        /// </summary>
        /// <param name="lightweightItem"></param>
        public static void AddItemToCache(LightweightItemMetaData lightweightItem)
        {
            _surveyItemCache.Add(GenerateItemCacheKey(lightweightItem.ItemId), lightweightItem);
        }

        /// <summary>
        /// Get an option from the option cache.  An option will be considered invalid if the template
        /// that contains the item that contains it has a modified date later than the reference date 
        /// of the item's mapping.   If the item is valid and contained in the cache, it will be
        /// returned.  Otherwise a null value will be returned.
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="itemId">If null, do not check item freshness</param>
        /// <param name="skipDateValidation"></param>
        /// <returns></returns>
        private static LightweightOptionMetaData GetOptionFromCache(int optionId, int? itemId, bool skipDateValidation)
        {
            LightweightItemMetaData itemMetaData = null;

            //If item containing the option is not in the cache or is otherwise
            // invalid, return null.
            //Skip check if item id is null
            if (itemId.HasValue)
            {
                itemMetaData = GetItemFromCache(itemId.Value, skipDateValidation);

                if (itemMetaData == null)
                {
                    return null;
                }
            }

            string optionCacheKey = GenerateItemOptionCacheKey(optionId);

            if (_surveyOptionCache.Contains(optionCacheKey))
            {
                return _surveyOptionCache[optionCacheKey] as LightweightOptionMetaData;
            }

            //Option was not in cache, reload item and get option if item id known
            //Expire item
            if (itemId.HasValue)
            {
                RemoveItemFromCache(itemId.Value);
                AddOptionToCache(GetOptionData(optionId, itemId, skipDateValidation));
                AddOptionToCache(GetOptionData(optionId, itemId, skipDateValidation));

                //Re-check option cache
                if (_surveyOptionCache.Contains(optionCacheKey))
                {
                    return _surveyOptionCache[optionCacheKey] as LightweightOptionMetaData;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a lightweight item to the cache.
        /// </summary>
        /// <param name="lightweightOption"></param>
        private static void AddOptionToCache(LightweightOptionMetaData lightweightOption)
        {
            _surveyOptionCache.Add(GenerateItemOptionCacheKey(lightweightOption.OptionId), lightweightOption);
        }

        /// <summary>
        /// Remove an item and it's options from the cache.  Child
        /// item are not automatically removed.
        /// </summary>
        /// <param name="itemId"></param>
        public static void RemoveItemFromCache(int itemId)
        {
            string itemCacheKey = GenerateItemCacheKey(itemId);

            if (_surveyItemCache.Contains(itemCacheKey))
            {
                //Find item and options
                var itemMetaData = _surveyItemCache[itemCacheKey] as LightweightItemMetaData;

                if (itemMetaData != null)
                {
                    List<int> itemOptions = itemMetaData.Options;

                    //Remove any options from cache
                    foreach (int optionId in itemOptions)
                    {
                        string optionCacheKey = GenerateItemOptionCacheKey(optionId);

                        if (_surveyOptionCache.Contains(optionCacheKey))
                        {
                            _surveyOptionCache.Remove(optionCacheKey);
                        }
                    }
                }

                //Remove item from cache
                _surveyItemCache.Remove(itemCacheKey);
            }
        }

        /// <summary>
        /// Attempt to more efficiently get item data by loading item data object from template.  Assumes that all children will be retrieved after parents
        /// are loaded and children will therefore be in the cache.  Makes no provision for loading children not yet in cache.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="skipDateValidation"></param>
        /// <param name="containingTemplate"></param>
        /// <returns></returns>
        public static LightweightItemMetaData GetItemData(int itemId, bool skipDateValidation, ResponseTemplate containingTemplate)
        {
            //Check the cache
            LightweightItemMetaData lightweightData = GetItemFromCache(itemId, skipDateValidation);

            if (lightweightData != null)
            {
                return lightweightData;
            }

            ItemData itemData = containingTemplate.GetItem(itemId);

            if (itemData == null)
            {
                return null;
            }

            //If we get her, item is not in cache or not fressh
            return LoadItemData(itemData);
        }


        /// <summary>
        /// Get data for an item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="skipDateValidation"></param>
        public static LightweightItemMetaData GetItemData(int itemId, bool skipDateValidation)
        {
            return GetItemData(itemId, skipDateValidation, default(ItemData));
        }

        /// <summary>
        /// Get data for an item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="skipDateValidation"></param>
        /// <param name="itemData"></param>
        public static LightweightItemMetaData GetItemData(int itemId, bool skipDateValidation, ItemData itemData)
        {
            //Check the cache
            LightweightItemMetaData lightweightData = GetItemFromCache(itemId, skipDateValidation);

            if (lightweightData != null)
                return lightweightData;

            return LoadItemData(itemData ?? ItemConfigurationManager.GetConfigurationData(itemId, true));
        }

        /// <summary>
        /// Given an item data object, load
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public static LightweightItemMetaData LoadItemData(ItemData itemData)
        {
            if (itemData == null || !itemData.ID.HasValue)
            {
                return null;
            }

            //Lightweight item to return
            LightweightItemMetaData itemMetaData = null;
            LightweightItemMetaData ancestorMetaData = null;

            if (itemData.ParentItemId.HasValue)
            {
                //Get parent, which should cause child to be loaded
                ancestorMetaData = GetItemData(itemData.ParentItemId.Value, false);

                //Ensure item is listed as child of parent
                if (ancestorMetaData != null
                    && !ancestorMetaData.Children.Contains(itemData.ID.Value))
                {
                    ancestorMetaData.Children.Add(itemData.ID.Value);

                    //Re-add parent to cache
                    AddItemToCache(ancestorMetaData);
                }

                //Attempt to get child data from cache. Skip validation because the item
                // should have just been loaded
                itemMetaData = GetItemFromCache(itemData.ID.Value, true);
            }

            //If we didn't load the item during the process of loading a parent, load it now.
            if (itemMetaData == null)
            {
                itemMetaData = LoadItemData(itemData, true);
            }

            //Store the item in the cache and return
            if (itemMetaData != null)
            {
                //Ensure timestamps are accurate.  Since matrix item children are clones of the column prototype, it's possible
                // the child item's actual modified date will be after the column prototypes.  Resulting in isUpdated validation
                // always failing because the item metadata's modified date will be earlier than the corresponding ItemData's.  To 
                // avoid this, set modified date and recache
                itemMetaData.LastModified = itemData.LastModified.Value;

                //subheader elements are always not answerable
                if (ancestorMetaData != null && ancestorMetaData.ItemType.Equals("Matrix")
                    && CheckIfItemIsMatrixSubheader(ancestorMetaData, itemData.ID.Value))
                    itemMetaData.IsAnswerable = false;

                AddItemToCache(itemMetaData);
            }

            return itemMetaData;
        }

        /// <summary>
        /// Get data for an option.
        /// </summary>
        /// <param name="itemId">ID of parent item.</param>
        /// <param name="optionId">ID of option.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns></returns>
        public static LightweightOptionMetaData GetOptionData(int optionId, int? itemId, bool skipDateValidation)
        {
            LightweightOptionMetaData optionMetaData = GetOptionFromCache(optionId, itemId, skipDateValidation);

            if (optionMetaData == null
                && itemId.HasValue)
            {
                //Cause item to be loaded
                GetItemData(itemId.Value, skipDateValidation);

                //Don't pass item id to avoid lots of fresh checks
                optionMetaData = GetOptionFromCache(optionId, null, skipDateValidation);
            }

            return optionMetaData;
        }

        /// <summary>
        /// Get data for an option.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private static ListOptionData GetListOptionData(int optionId)
        {
            var listOptionData = GetListOptionDataFromCache(optionId);

            if (listOptionData != null) 
                return listOptionData;
            
            listOptionData = LoadListOptionData(optionId);
            if (listOptionData == null)
                return null;

            AddListOptionDataToCache(listOptionData);

            return listOptionData;
        }

        /// <summary>
        /// Load data for an item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="populateText"></param>
        private static LightweightItemMetaData LoadItemData(ItemData itemData, bool populateText)
        {
            if (itemData != null)
            {
                //Create the report item data object
                LightweightItemMetaData lightweightItemMetaData = CreateLightweightItemMetaDataObject(itemData);

                if (itemData is ResponseItemData)
                {
                    lightweightItemMetaData.RequiresAnswer = ((ResponseItemData)itemData).IsRequired;
                }

                //Get option data for select items.
                if (itemData is SelectItemData)
                {
                    ReadOnlyCollection<ListOptionData> listOptions = ((SelectItemData)itemData).Options;

                    foreach (ListOptionData listOptionData in listOptions)
                    {
                        //Build the option data object
                        LightweightOptionMetaData lightweightOptionMetaData = CreateLightweightOptionMetaDataObject(listOptionData);

                        lightweightOptionMetaData.ItemId = itemData.ID.Value;

                        //Add the object to the report option dictionary
                        AddOptionToCache(lightweightOptionMetaData);

                        //Add the option to the report item data object.
                        lightweightItemMetaData.Options.Add(listOptionData.OptionID);
                    }
                }

                //Populate item text
                if (populateText)
                {
                    PopulateItemText(lightweightItemMetaData, itemData, ResponseTemplateManager.ActiveSurveyLanguages);
                }

                //Add the item to the dictionary
                AddItemToCache(lightweightItemMetaData);

                //Load any children of this item
                LoadItemChildren(lightweightItemMetaData, itemData);

                return GetItemFromCache(lightweightItemMetaData.ItemId, true);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightweightItemMetaData"></param>
        /// <param name="itemData"></param>
        private static void PopulateMatrixItemDetails(LightweightItemMetaData lightweightItemMetaData, MatrixItemData itemData)
        {
            //Load row/column data
            for (int columnNumber = 1; columnNumber <= itemData.ColumnCount; columnNumber++)
            {
                if (columnNumber == itemData.PrimaryKeyColumnIndex)
                {
                    lightweightItemMetaData.AddColumn(columnNumber, -1, "PK");
                    continue;
                }

                var columnPrototypeId = itemData.GetColumnPrototypeId(columnNumber);

                if (columnPrototypeId <= 0)
                {
                    continue;
                }

                var columnPrototype = GetItemData(columnPrototypeId, false);

                if (columnPrototype != null)
                {
                    lightweightItemMetaData.AddColumn(columnNumber, columnPrototypeId, columnPrototype.ItemType);
                }
            }

            for (int rowNumber = 1; rowNumber <= itemData.RowCount; rowNumber++)
            {
                var rowKeyItemId = itemData.GetItemIdAt(
                    rowNumber,
                    itemData.PrimaryKeyColumnIndex);

                if (!rowKeyItemId.HasValue || rowKeyItemId <= 0)
                {
                    continue;
                }

                var rowType = !itemData.IsRowSubheading(rowNumber)
                                     ? itemData.IsRowOther(rowNumber)
                                           ? "Other"
                                           : "Normal"
                                     : "Subheading";

                lightweightItemMetaData.AddRow(rowNumber, rowKeyItemId.Value, rowType);
            }

            //Load child coordinates
            for (int rowNumber = 1; rowNumber <= itemData.RowCount; rowNumber++)
            {
                for (int columnNumber = 1; columnNumber <= itemData.ColumnCount; columnNumber++)
                {
                    var itemId = itemData.GetItemIdAt(rowNumber, columnNumber);

                    if (!itemId.HasValue)
                    {
                        continue;
                    }

                    lightweightItemMetaData.SetChildCoordinate(
                        itemId.Value,
                        new Coordinate(columnNumber, rowNumber));
                }
            }
        }

        /// <summary>
        /// Populate text for items.  Should generally not be called for children of matrix items.  Children should be loaded with the matrix item
        /// which loads text data more efficiently.
        /// </summary>
        /// <param name="lightweightItemMetaData"></param>
        /// <param name="itemData"></param>
        /// <param name="languageCodes"></param>
        private static void PopulateItemText(LightweightItemMetaData lightweightItemMetaData, ItemData itemData, IEnumerable<string> languageCodes)
        {
            foreach (string languageCode in languageCodes)
            {
                ItemTextDecorator decorator = itemData.CreateTextDecorator(languageCode);

                if (decorator is LabelledItemTextDecorator)
                {
                    lightweightItemMetaData.SetText(languageCode, Utilities.StripHtml(((LabelledItemTextDecorator)decorator).Text, null));
                    lightweightItemMetaData.SetDescription(languageCode, Utilities.StripHtml(((LabelledItemTextDecorator)decorator).SubText, null));
                }

                if (decorator is MessageItemTextDecorator)
                {
                    lightweightItemMetaData.SetText(languageCode, Utilities.StripHtml(((MessageItemTextDecorator)decorator).Message, null));
                }

                if (decorator is SelectItemTextDecorator)
                {
                    foreach (int optionId in lightweightItemMetaData.Options)
                    {
                        LightweightOptionMetaData cachedOption = GetOptionFromCache(optionId, null, false);

                        if (cachedOption == null)
                        {
                            continue;
                        }

                        cachedOption.SetText(languageCode, Utilities.StripHtml(((SelectItemTextDecorator)decorator).GetOptionText(cachedOption.Position), null));

                        //Refresh option in cache
                        AddOptionToCache(cachedOption);
                    }
                }
            }

            //Store list of texts populated for the item, this will be used to validate item in cache in case
            // current list of active languages changes.
            lightweightItemMetaData.PopulatedLanguages = new List<string>(languageCodes);
        }

        /// <summary>
        /// Create a report item data object for the given item data
        /// </summary>
        /// <param name="itemData">Data to create</param>
        /// <returns></returns>
        private static LightweightItemMetaData CreateLightweightItemMetaDataObject(ItemData itemData)
        {
            //Set item id and alias
            var lightweightItemMetaData = new LightweightItemMetaData
            {
                ItemId = itemData.ID.Value,
                Alias = itemData.Alias,
                AncestorId = 0,
                ItemType = itemData.ItemTypeName,
                LastModified = itemData.LastModified.HasValue ? itemData.LastModified.Value : DateTime.Now,
                IsAnswerable = itemData.ItemIsIAnswerable,
                IsScored = itemData.ItemIsIScored
            };

            //Set positional information
            PopulateItemPosition(lightweightItemMetaData);

            //Load any select item specific details
            if (itemData is SelectItemData)
            {
                //Set allow other value
                lightweightItemMetaData.AllowOther = ((SelectItemData)itemData).AllowOther;
            }

            //Populate matrix-specific details
            if (itemData is MatrixItemData)
            {
                PopulateMatrixItemDetails(lightweightItemMetaData, (MatrixItemData)itemData);
            }

            return lightweightItemMetaData;
        }

        /// <summary>
        /// Create a report option data object based on the list option data.
        /// </summary>
        /// <param name="listOptionData"></param>
        /// <returns></returns>
        private static LightweightOptionMetaData CreateLightweightOptionMetaDataObject(ListOptionData listOptionData)
        {
            //Build the option data object
            var optionData = new LightweightOptionMetaData
            {
                OptionId = listOptionData.OptionID,
                IsOther = listOptionData.IsOther,
                Points = listOptionData.Points,
                Alias = listOptionData.Alias,
                Position = listOptionData.Position
            };

            return optionData;
        }

        /// <summary>
        /// Load the ancestors table for this item
        /// </summary>
        /// <param name="lightweightParentItemData"></param>
        /// <param name="parentItemData"></param>
        private static void LoadItemChildren(LightweightItemMetaData lightweightParentItemData, ItemData parentItemData)
        {
            //Take advantage of knowledge of matrix item to load children more efficiently
            if (parentItemData is MatrixItemData)
            {
                LoadMatrixItemChildren((MatrixItemData)parentItemData, lightweightParentItemData);
            }
            else if (parentItemData is ICompositeItemData)
            {
                IEnumerable<int> childItemIds = ((ICompositeItemData)parentItemData).GetChildItemDataIDs();

                foreach (var childItemId in childItemIds)
                {
                    lightweightParentItemData.Children.Add(childItemId);
                    LoadItemData(ItemConfigurationManager.GetConfigurationData(childItemId), true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool CheckIfItemIsMatrixSubheader(LightweightItemMetaData matrixMetaData, int childItemId)
        {
            var coordinate = matrixMetaData.GetChildCoordinate(childItemId);
            var rowType = matrixMetaData.GetRowType(coordinate.Y);
            return rowType == RowType.Subheading.ToString();
        }

        /// <summary>
        /// Load children of a matrix item
        /// </summary>
        /// <param name="matrixItemData"></param>
        /// <param name="lightweightMatrixItemData"></param>
        private static void LoadMatrixItemChildren(MatrixItemData matrixItemData, LightweightItemMetaData lightweightMatrixItemData)
        {
            var rowTextItems = new Dictionary<int, LightweightItemMetaData>();
            var surveyLanguages = ResponseTemplateManager.ActiveSurveyLanguages;

            //Iterate matrix columns
            for (int column = 1; column <= matrixItemData.ColumnCount; column++)
            {
                //Load template for non-pk columns
                var lightweightColumnTemplate = column != matrixItemData.PrimaryKeyColumnIndex
                    ? GetItemData(matrixItemData.GetColumnPrototypeId(column), false)
                    : null;

                //Iterate matrix rows
                for (int row = 1; row <= matrixItemData.RowCount; row++)
                {
                    var childItemId = matrixItemData.GetItemIdAt(row, column);

                    if (!childItemId.HasValue)
                    {
                        continue;
                    }

                    //Ensure item is set as a matrix child)
                    if (!lightweightMatrixItemData.Children.Contains(childItemId.Value))
                    {
                        lightweightMatrixItemData.Children.Add(childItemId.Value);
                    }

                    //Except for row texts and "other" rows, which exist in PK column, child items are not FULL
                    // items, so create them as copies of the column template.
                    var lightweightChildItemData = column == matrixItemData.PrimaryKeyColumnIndex
                        ? GetItemData(childItemId.Value, false)
                        : lightweightColumnTemplate != null
                            ? lightweightColumnTemplate.Clone() as LightweightItemMetaData          //Column template should never be null when column # != pk index
                            : null;

                    if (lightweightChildItemData == null)
                    {
                        continue;
                    }

                    //Set id to be id of child item.
                    lightweightChildItemData.ItemId = childItemId.Value;

                    //Copy page/item position from parent
                    lightweightChildItemData.PagePosition = lightweightMatrixItemData.PagePosition;
                    lightweightChildItemData.ItemPosition = lightweightMatrixItemData.ItemPosition;

                    //Set row/column position
                    lightweightChildItemData.Coordinate = new Coordinate(column, row);

                    //Ensure row items and their texts are loaded.
                    if (column == matrixItemData.PrimaryKeyColumnIndex)
                    {
                        PopulateItemText(lightweightChildItemData, ItemConfigurationManager.GetConfigurationData(lightweightChildItemData.ItemId), surveyLanguages);
                        rowTextItems[row] = lightweightChildItemData;
                    }

                    //Set row text/alias
                    if (column != matrixItemData.PrimaryKeyColumnIndex && rowTextItems.ContainsKey(row))
                    {
                        lightweightChildItemData.MatrixRowAlias = rowTextItems[row].Alias;

                        foreach (var languageCode in surveyLanguages)
                        {
                            lightweightChildItemData.SetMatrixRowText(
                                rowTextItems[row].GetText(false, languageCode),
                                languageCode);

                        }

                        //Set "populated languages" of children to prevent constant reloading of text
                        lightweightChildItemData.PopulatedLanguages = new List<string>(surveyLanguages);
                    }

                    if (CheckIfItemIsMatrixSubheader(lightweightMatrixItemData, childItemId.Value))
                        lightweightChildItemData.IsAnswerable = false;

                    //Refresh child item in cache
                    AddItemToCache(lightweightChildItemData);
                }
            }

            //Refresh matrix item in cache
            AddItemToCache(lightweightMatrixItemData);
        }

        /// <summary>
        /// Get the point value for an option.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId">ID of the option.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns>Point value of the specified option.</returns>
        /// <remarks>If the option is not found in the internal options collection, 0 is returned.</remarks>
        public static double GetOptionPoints(int itemId, int optionId, bool skipDateValidation)
        {
            LightweightOptionMetaData data = GetOptionData(optionId, itemId == 0? null : (int?)itemId, skipDateValidation);

            return data != null ? data.Points : 0;
        }

        /// <summary>
        /// Get a boolean value indicating if an option is an "other" option.
        /// </summary>
        /// <param name="optionId">ID of the option.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns>Boolean value indicating if the option is an "other" option.</returns>
        /// <remarks>If the option is not found in the internal options collection, false is returned.</remarks>
        public static bool GetOptionIsOther(int optionId, bool skipDateValidation)
        {
            LightweightOptionMetaData data = GetOptionFromCache(optionId, null, skipDateValidation);

            return data != null && data.IsOther;
        }

        /// <summary>
        /// Get a boolean value indicating if an option is an "other" option.
        /// </summary>
        /// <param name="optionId">ID of the option.</param>
        /// <returns>Boolean value indicating if the option is an "other" option.</returns>
        /// <remarks>If the option is not found in the internal options collection, false is returned.</remarks>
        public static bool GetOptionIsOther(int optionId)
        {
            return GetListOptionData(optionId).IsOther;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private static ListOptionData LoadListOptionData(int optionId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetOptionData");
            command.AddInParameter("OptionID", DbType.Int32, optionId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new ListOptionData
                                   {
                                       IsNoneOfAbove = DbUtility.GetValueFromDataReader(reader, "IsNoneOfAbove", false),
                                       IsOther = DbUtility.GetValueFromDataReader(reader, "IsOther", false),
                                       Alias = DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty),
                                       Category = DbUtility.GetValueFromDataReader(reader, "Category", string.Empty),
                                       ContentID = DbUtility.GetValueFromDataReader(reader, "ContentID", default(int?)),
                                       OptionID = DbUtility.GetValueFromDataReader(reader, "OptionID", -1),
                                       IsDefault = DbUtility.GetValueFromDataReader(reader, "IsDefault", false),
                                       Position = DbUtility.GetValueFromDataReader(reader, "Position", -1),
                                       Points = DbUtility.GetValueFromDataReader(reader, "Points", 0)
                                   };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }


        /// <summary>
        /// Get the type name of an item.
        /// </summary>
        /// <param name="itemId">ID of item to get type name of.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns>ID of type name.</returns>
        public static string GetItemTypeName(Int32 itemId, bool skipDateValidation)
        {
            LightweightItemMetaData data = GetItemData(itemId, skipDateValidation);

            return data != null ? data.ItemType : string.Empty;
        }

        /// <summary>
        /// Get the text of an item
        /// </summary>
        /// <param name="itemId">ID of item to get text for.</param>
        /// <param name="languageCode"></param>
        /// <param name="preferAlias">Specify whether to return the item alias, if it exists.  If no alias exists, fall back to the item's text.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns>Text for the item.</returns>
        public static string GetItemText(Int32 itemId, string languageCode, bool preferAlias, bool skipDateValidation)
        {
            if (preferAlias)
            {
                string alias = GetAlias(itemId);
                if (!string.IsNullOrEmpty(alias))
                    return alias;
            }
            var config = ItemConfigurationManager.GetConfigurationData(itemId, true);
            var text = TextManager.GetText("/" + config.TextIdPrefix + "/" + itemId + "/text", languageCode);

            if (string.IsNullOrEmpty(text))
            {
                var data = GetItemData(itemId, skipDateValidation);
                text = data.GetText(preferAlias, languageCode);
            }

            return text ?? string.Empty;
        }

        /// <summary>
        /// Get the alias of an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private static string GetAlias(Int32 itemId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetAlias");
            command.AddInParameter("ItemId", DbType.Int32, itemId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
        
                }
            

            return string.Empty;
        }

        /// <summary>
        /// Get the text of an option
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId">ID of option to get text for.</param>
        /// <param name="languageCode"></param>
        /// <param name="preferAlias">Specify whether to return the option alias if it exists.  If no alias exists, fall back to the option text.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns></returns>
        public static string GetOptionText(Int32? itemId, Int32 optionId, string languageCode, bool preferAlias, bool skipDateValidation)
        {
            ListOptionData listOptionData = GetListOptionData(optionId);
            if (listOptionData == null)
                return string.Empty;

            if (listOptionData.IsOther)
                return TextManager.GetText("/controlText/exportItem/other", languageCode, "Other");

            if (listOptionData.IsNoneOfAbove)
                return TextManager.GetText("/controlText/exportItem/noneOfAbove", languageCode, "None Of The Above");

            if (preferAlias)
            {
                string alias = listOptionData.Alias;
                if (!string.IsNullOrEmpty(alias))
                    return alias;
            }

            string text = TextManager.GetText(String.Format("/listOption/{0}/text", optionId), languageCode);
            return string.IsNullOrEmpty(text) ? listOptionData.Alias : text;
        }
        /// <summary>
        /// List ids of all options for the specified item.
        /// </summary>
        /// <param name="itemId">ID of item to list options for.</param>
        /// <param name="skipDateValidation"></param>
        /// <returns>List of option ids.</returns>
        public static List<int> ListOptionIdsForItem(int itemId, bool skipDateValidation)
        {
            LightweightItemMetaData data = GetItemData(itemId, skipDateValidation);

            return data != null
                ? data.Options
                : new List<int>();
        }


        /// <summary>
        /// Populate item position text
        /// </summary>
        /// <param name="itemMetaData"></param>
        private static void PopulateItemPosition(LightweightItemMetaData itemMetaData)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetPosition");
            command.AddInParameter("ItemId", DbType.Int32, itemMetaData.ItemId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        itemMetaData.PagePosition = DbUtility.GetValueFromDataReader(reader, "PagePosition", -1);
                        itemMetaData.ItemPosition = DbUtility.GetValueFromDataReader(reader, "ItemPosition", -1);
                        int? row = DbUtility.GetValueFromDataReader(reader, "MatrixRowPosition", (int?)null);
                        int? column = DbUtility.GetValueFromDataReader(reader, "MatrixColumnPosition", (int?)null);

                        if (row.HasValue && column.HasValue)
                            itemMetaData.Coordinate = new Coordinate(row.Value, column.Value);
                        else
                            itemMetaData.Coordinate = null;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        #region Rules engine cache logic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static RulesEngine GetRulesEngineFromCache(int surveyId)
        {
            var key = GenerateSurveyRulesCacheKey(surveyId);

            if (_surveyRulesCache != null)
                return _surveyRulesCache.GetData(key) as RulesEngine;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        public static void AddRulesEngineToCache(int surveyId, RulesEngine engine)
        {
            var key = GenerateSurveyRulesCacheKey(surveyId);

            if (_surveyRulesCache != null)
                _surveyRulesCache.Add(key, engine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static void RemoveRulesEngineFromCache(int surveyId)
        {
            var key = GenerateSurveyRulesCacheKey(surveyId);

            if (_surveyRulesCache != null)
                _surveyRulesCache.Remove(key);
        }

        #endregion

        #region Rules data service cache logic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static RuleDataService GetRuleDataServiceFromCache(int surveyId)
        {
            var key = GenerateRuleDataServiceCacheKey(surveyId);

            if (_surveyRulesCache != null)
                return _surveyRulesCache.GetData(key) as RuleDataService;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static void AddRuleDataServiceToCache(int surveyId, RuleDataService service)
        {
            var key = GenerateRuleDataServiceCacheKey(surveyId);

            if (_surveyRulesCache != null)
                _surveyRulesCache.Add(key, service);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static void RemoveRuleDataServiceFromCache(int surveyId)
        {
            var key = GenerateRuleDataServiceCacheKey(surveyId);

            if (_surveyRulesCache != null)
                _surveyRulesCache.Remove(key);
        }

        #endregion

    }
}
