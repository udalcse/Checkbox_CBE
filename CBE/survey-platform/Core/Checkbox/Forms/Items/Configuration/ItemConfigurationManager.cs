//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Checkbox.Forms.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using System.IO;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Prezza.Framework.Configuration;
using Prezza.Framework.Caching;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Simple container for item category information
    /// </summary>
    [Serializable]
    public class ItemCategoryInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ItemCategoryInfo()
        {
            ItemTypes = new List<ItemTypeInfo>();
        }

        /// <summary>
        /// Get parent category. Always returns empty string.
        /// </summary>
        public string ParentCategory { get { return string.Empty; } }

        /// <summary>
        /// Name of item category
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Database id of item category
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Text id of category name
        /// </summary>
        public string NameTextId { get; set; }

        /// <summary>
        /// Text id of category description
        /// </summary>
        public string DescriptionTextId { get; set; }

        /// <summary>
        /// Position of category relative to other categories
        /// </summary>
        public int CategoryPosition { get; set; }

        /// <summary>
        /// List of item types in the categories
        /// </summary>
        public List<ItemTypeInfo> ItemTypes { get; set; }
    }


    /// <summary>
    /// Simple container for storing type information
    /// </summary>
    [Serializable]
    public class ItemTypeInfo
    {
        /// <summary>
        /// Short name of item type
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Database id of item type
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Class name of item type's data object
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Assembly containing types data class
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Item category item belongs to
        /// </summary>
        public string ParentCategory { get; set; }

        /// <summary>
        /// Specifies whether item can be included in web surveys
        /// </summary>
        public bool SurveyCompatible { get; set; }

        /// <summary>
        /// Specifies whether item can be included in mobile surveys
        /// </summary>
        public bool MobileCompatible { get; set; }

        /// <summary>
        /// Specifies whether item can be included in reports
        /// </summary>
        public bool ReportCompatible { get; set; }

        /// <summary>
        /// Specifies whether item can be included in item libraries
        /// </summary>
        public bool LibraryCompatible { get; set; }

        /// <summary>
        /// Specifies the prefix of textID for an item
        /// </summary>
        public string TextIdPrefix { get; set; }
    }

    /// <summary>
    /// Manages the retrieval and caching of ItemConfigurations
    /// </summary>
    public static class ItemConfigurationManager
    {
        private static readonly CacheManager _cacheManagerTypesByName;
        private static readonly CacheManager _cacheManagerTypesById;
        private static readonly CacheManager _cacheManagerTypesByItemId;
        private static readonly CacheManager _cacheManagerItemDataById;
        private static readonly CacheManager _cacheManagerTypeNamesByItemId;
        private static readonly CacheManager _cacheManagerItemPrototypeByItemId;
        private static readonly CacheManager _cacheManagerItemBasicDataBySurveyIdAndPagePosition;

        /// <summary>
        /// Ordered list of type category names
        /// </summary>
        private static readonly List<ItemCategoryInfo> _typeCategories;

        /// <summary>
        /// 
        /// </summary>
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initialize the hashtable
        /// </summary>
        static ItemConfigurationManager()
        {
            _cacheManagerTypesByName = CacheFactory.GetCacheManager("itemConfigurationTypesByName");
            _cacheManagerTypesById = CacheFactory.GetCacheManager("itemConfigurationTypesById");
            _cacheManagerTypesByItemId = CacheFactory.GetCacheManager("itemConfigurationTypesByItemId");
            _cacheManagerItemDataById = CacheFactory.GetCacheManager("itemConfigurationItemDataById");
            _cacheManagerTypeNamesByItemId = CacheFactory.GetCacheManager("itemConfigurationTypeNamesByItemId");
            _cacheManagerItemPrototypeByItemId = CacheFactory.GetCacheManager("itemPrototypeByItemId");
            _cacheManagerItemBasicDataBySurveyIdAndPagePosition = CacheFactory.GetCacheManager("itemBasicDataBySurveyIdAndPagePosition");

            _typeCategories = new List<ItemCategoryInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <returns></returns>
        public static ItemTypeInfo GetTypeInfoByName(string itemTypeName)
        {
            if (_cacheManagerTypesByName.Contains(itemTypeName))
            {
                return _cacheManagerTypesByName.GetData(itemTypeName) as ItemTypeInfo;
            }

            string typeAssembly = string.Empty;
            string typeName = string.Empty;
            string textIdPrefix = string.Empty;
            int typeId = -1;

            lock (_lockObject)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemType_GetByName");
                command.AddInParameter("ItemName", DbType.String, itemTypeName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            typeAssembly = DbUtility.GetValueFromDataReader(reader, "ItemDataAssemblyName", string.Empty);
                            typeName = DbUtility.GetValueFromDataReader(reader, "ItemDataClassName", string.Empty);
                            typeId = DbUtility.GetValueFromDataReader(reader, "ItemTypeID", typeId);
                            textIdPrefix = DbUtility.GetValueFromDataReader(reader, "TextIdPrefix", string.Empty);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                if (Utilities.IsNotNullOrEmpty(typeAssembly)
                    && Utilities.IsNotNullOrEmpty(typeName)
                    && typeId > 0)
                {
                    var info = new ItemTypeInfo
                    {
                        AssemblyName = typeAssembly,
                        TypeName = itemTypeName,
                        ClassName = typeName,
                        TypeId = typeId,
                        TextIdPrefix = textIdPrefix
                    };

                    _cacheManagerTypesByName.Add(itemTypeName, info);
                    _cacheManagerTypesById.Add(typeId.ToString(), info);

                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <returns></returns>
        public static ItemTypeInfo GetTypeInfoById(int itemTypeId)
        {
            if (_cacheManagerTypesById.Contains(itemTypeId.ToString()))
            {
                return _cacheManagerTypesById.GetData(itemTypeId.ToString()) as ItemTypeInfo;
            }

            string typeAssembly = string.Empty;
            string typeName = string.Empty;
            string itemTypeName = string.Empty;
            string textIdPrefix = string.Empty;

            lock (_lockObject)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemType_Get");
                command.AddInParameter("ItemTypeID", DbType.Int32, itemTypeId);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            typeAssembly = DbUtility.GetValueFromDataReader(reader, "ItemDataAssemblyName", string.Empty);
                            typeName = DbUtility.GetValueFromDataReader(reader, "ItemDataClassName", string.Empty);
                            itemTypeName = DbUtility.GetValueFromDataReader(reader, "ItemName", string.Empty);
                            textIdPrefix = DbUtility.GetValueFromDataReader(reader, "TextIdPrefix", string.Empty);
                        }
                    }
                    finally
                    {
                        reader.Close();

                    }
                }

                if (Utilities.IsNotNullOrEmpty(typeAssembly)
                    && Utilities.IsNotNullOrEmpty(typeName)
                    && Utilities.IsNotNullOrEmpty(itemTypeName))
                {
                    var info = new ItemTypeInfo
                    {
                        AssemblyName = typeAssembly,
                        TypeName = itemTypeName,
                        ClassName = typeName,
                        TypeId = itemTypeId,
                        TextIdPrefix = textIdPrefix
                    };

                    _cacheManagerTypesByName.Add(itemTypeName, info);
                    _cacheManagerTypesById.Add(itemTypeId.ToString(), info);

                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Get numeric if a type from its text name.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static string GetTypeNameFromId(int typeId)
        {
            ItemTypeInfo typeInfo = GetTypeInfoById(typeId);

            return typeInfo != null
                ? typeInfo.TypeName
                : string.Empty;
        }

        /// <summary>
        /// Get string name of a type from its numeric id
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static int? GetTypeIdFromName(string typeName)
        {
            ItemTypeInfo typeInfo = GetTypeInfoByName(typeName);

            return typeInfo != null
                ? typeInfo.TypeId
                : (int?)null;
        }

        /// <summary>
        /// Create an instance of an item with the specified type name
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <returns></returns>
        public static ItemData CreateConfigurationData(string itemTypeName)
        {
            ItemTypeInfo info = GetTypeInfoByName(itemTypeName);

            if (info == null)
            {
                return null;
            }

            ItemData data = new ItemConfigurationFactory().CreateItemData(info.ClassName + ", " + info.AssemblyName);
            data.ItemTypeID = info.TypeId;
            data.ItemTypeName = info.TypeName;

            return data;

        }

        ///<summary>
        /// Create an instance of an item with the specified type name
        ///</summary>
        ///<param name="info"></param>
        ///<returns></returns>
        public static ItemData CreateConfigurationData(ItemTypeInfo info)
        {
            if (info == null)
            {
                return null;
            }

            ItemData data = new ItemConfigurationFactory().CreateItemData(info.ClassName + ", " + info.AssemblyName);
            data.ItemTypeID = info.TypeId;
            data.ItemTypeName = info.TypeName;

            return data;
        }

        /// <summary>
        /// Get a new configuration data for an item data of the specified type.
        /// </summary>
        /// <param name="itemTypeID"></param>
        /// <returns></returns>
        public static ItemData CreateConfigurationData(Int32 itemTypeID)
        {
            var info = GetTypeInfoById(itemTypeID);
            return CreateConfigurationData(info);
        }

        /// <summary>
        /// Gets the configuration for the ItemData with the specified identity.  Looks up the correct assembly/class type for the data object in from the 
        /// database.  
        /// </summary>
        /// <param name="identity">the unique identity of the ItemData</param>
        /// <param name="useCache">allow to use cache</param>
        /// <returns></returns>
        public static ItemData GetConfigurationData(Int32 identity, bool useCache = false)
        {
            return GetConfigurationData(identity, useCache, null);
        }

        /// <summary>
        /// Gets the configuration for the ItemData with the specified identity.  Looks up the correct assembly/class type for the data object in from the 
        /// database.  
        /// </summary>
        /// <param name="identity">the unique identity of the ItemData</param>
        /// <param name="useCache"></param>
        /// <param name="ds">Data set to try to load configuration data from first.</param>
        /// <returns></returns>
        private static ItemData GetConfigurationData(Int32 identity, bool useCache, PersistedDomainObjectDataSet ds)
        {
            if (identity <= 0)
                return null;

            if (useCache)
            {
                if (_cacheManagerItemDataById.Contains(identity.ToString()))
                    return _cacheManagerItemDataById.GetData(identity.ToString()) as ItemData;
            }

            var typeInfo = GetItemTypeInfo(identity);
            var configuration = CreateConfigurationData(typeInfo);
            
            configuration.ID = identity;
            configuration.Load(ds);

            _cacheManagerItemDataById.Add(identity.ToString(), configuration);

            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        public static void InvalidateCachedItemData(Int32 identity)
        {
            if (_cacheManagerItemDataById.Contains(identity.ToString()))
                _cacheManagerItemDataById.Remove(identity.ToString());
        }

        /// <summary>
        /// Get the type name of an item based on its id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetItemTypeName(int itemId)
        {
            if (_cacheManagerTypeNamesByItemId.Contains(itemId.ToString()))
            {
                return _cacheManagerTypeNamesByItemId.GetData(itemId.ToString()) as string;
            }

            string itemName = string.Empty;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetTypeInfo");
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        itemName = (string)reader["ItemName"];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            _cacheManagerTypeNamesByItemId.Add(itemId.ToString(), itemName);

            return itemName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIds"></param>
        private static Dictionary<int, ItemTypeInfo> ListTypeInfoForItems(List<int> itemIds)
        {
            Dictionary<int, ItemTypeInfo> result = new Dictionary<int, ItemTypeInfo>();

            string param = null;

            foreach (var id in itemIds)
            {
                if (_cacheManagerTypesByItemId.Contains(id.ToString()))
                {
                    result[id] = _cacheManagerTypesByItemId.GetData(id.ToString()) as ItemTypeInfo;
                }
                else
                {
                    if (string.IsNullOrEmpty(param))
                        param = id.ToString(CultureInfo.InvariantCulture);
                    else
                        param += ", " + id;
                }
            }

            if (string.IsNullOrEmpty(param))
                return result;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetTypeInfoForNumberOfItems");
            command.AddInParameter("ItemIDString", DbType.String, param);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", -1);

                        if (itemId > -1)
                        {
                            var itemTypeName = (string)reader["ItemName"];
                            var className = (string)reader["ItemDataClassName"];
                            var assemblyName = (string)reader["ItemDataAssemblyName"];
                            var typeId = DbUtility.GetValueFromDataReader(reader, "ItemTypeID", -1);
                            var textIdPrefix = DbUtility.GetValueFromDataReader(reader, "TextIdPrefix", string.Empty);

                            var info = new ItemTypeInfo
                            {
                                AssemblyName = assemblyName,
                                ClassName = className,
                                TypeName = itemTypeName,
                                TypeId = typeId,
                                TextIdPrefix = textIdPrefix
                            };

                            result[itemId] = info;

                            _cacheManagerTypesByName.Add(itemTypeName, info);
                            _cacheManagerTypesById.Add(typeId.ToString(), info);
                            _cacheManagerTypesByItemId.Add(itemId.ToString(), info);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIds"></param>
        public static Dictionary<int, ItemData> ListDataForItems(List<int> itemIds)
        {
            var info = ListTypeInfoForItems(itemIds);

            Dictionary<int, ItemData> result = new Dictionary<int, ItemData>();
            Dictionary<int, ItemData> newData = new Dictionary<int, ItemData>();

            string param = null;

            //get all existing from cache
            foreach (var i in info)
            {
                int id = i.Key;

                if (_cacheManagerItemDataById.Contains(id.ToString()))
                {
                    result[id] = _cacheManagerItemDataById.GetData(id.ToString()) as ItemData;
                }
                else
                {
                    var data = CreateConfigurationData(i.Value);
                    data.ID = id;

                    newData.Add(id, data);

                    if (string.IsNullOrEmpty(param))
                        param = id.ToString(CultureInfo.InvariantCulture);
                    else
                        param += ", " + id;
                }
            }

            if (string.IsNullOrEmpty(param))
                return result;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetNumberOfItems");
            command.AddInParameter("ItemIDString", DbType.String, param);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", -1);

                        if (newData.ContainsKey(itemId))
                        {
                            var itemData = newData[itemId];

                            var alias = DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty);
                            var itemTypeID = DbUtility.GetValueFromDataReader(reader, "ItemTypeID", -1);
                            var itemName = DbUtility.GetValueFromDataReader(reader, "ItemName", string.Empty);
                            var createdDate = DbUtility.GetValueFromDataReader<DateTime?>(reader, "CreatedDate", null);
                            var lastModified = DbUtility.GetValueFromDataReader<DateTime?>(reader, "ModifiedDate", null);
                            var isActive = DbUtility.GetValueFromDataReader(reader, "IsActive", true);
                            var parentItemID = DbUtility.GetValueFromDataReader(reader, "ParentItemID", (int?)null);

                            var dataSet = itemData.GetSpecificConfigurationDataSet(itemId);
                            itemData.Load(dataSet);
                            itemData.LoadAdditionalData(alias, itemTypeID, itemName, createdDate, lastModified, isActive, parentItemID);

                            result[itemId] = itemData;

                            _cacheManagerItemDataById.Add(itemId.ToString(), itemData);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Get the type name of an item based on its id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static ItemTypeInfo GetItemTypeInfo(int itemId)
        {
            if (_cacheManagerTypesByItemId.Contains(itemId.ToString()))
            {
                return _cacheManagerTypesByItemId.GetData(itemId.ToString()) as ItemTypeInfo;
            }

            string itemTypeName = string.Empty;
            string assemblyName = string.Empty;
            string className = string.Empty;
            int typeId = -1;
            string textIdPrefix = string.Empty;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetTypeInfo");
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        itemTypeName = (string)reader["ItemName"];
                        className = (string)reader["ItemDataClassName"];
                        assemblyName = (string)reader["ItemDataAssemblyName"];
                        typeId = DbUtility.GetValueFromDataReader(reader, "ItemTypeID", -1);
                        textIdPrefix = DbUtility.GetValueFromDataReader(reader, "TextIdPrefix", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            var info = new ItemTypeInfo
            {
                AssemblyName = assemblyName,
                ClassName = className,
                TypeName = itemTypeName,
                TypeId = typeId,
                TextIdPrefix = textIdPrefix
            };

            _cacheManagerTypesByName.Add(itemTypeName, info);
            _cacheManagerTypesById.Add(typeId.ToString(), info);
            _cacheManagerTypesByItemId.Add(itemId.ToString(), info);

            return info;
        }



        /// <summary>
        /// Get the configuration of the specified type for an item with the specified identity.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static ItemData GetConfigurationData(string itemType, Int32 identity)
        {
            return GetConfigurationData(itemType, identity, null);
        }

        /// <summary>
        /// Get configuration data for an item.  If a <see cref="DataSet"/> is passed, the item configuration is loaded from the 
        /// DataSet, otherwise the item configuration loads itself from the persistent store.
        /// </summary>
        /// <param name="itemName">the Type of the ItemData to get</param>
        /// <param name="identity">the database ID of the ItemData</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static ItemData GetConfigurationData(string itemName, Int32 identity, PersistedDomainObjectDataSet ds)
        {
            //Return null if no item name is specified
            if (itemName == null || itemName.Trim() == string.Empty)
            {
                return null;
            }

            ItemData configuration = CreateConfigurationData(itemName);

            if (configuration != null)
            {
                configuration.ID = identity;
                configuration.Load(ds);
            }

            return configuration;
        }

        /// <summary>
        /// This method collects basic item data up to the specified page.
        /// It excludes non-answerable and composite(matrix) items, but includes child items
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="maxPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static List<BasicItemMetaData> ListBasicItemsData(int responseTemplateId, int? maxPagePosition, string languageCode)
        {
            var result = new List<BasicItemMetaData>();

            ResponseTemplate rt = null;
            int pagePos = 1;

            if (_cacheManagerItemBasicDataBySurveyIdAndPagePosition != null)
            {
                int max = maxPagePosition ?? 999999;

                for (; ; pagePos++)
                {
                    var key = responseTemplateId + "_" + pagePos;

                    if (_cacheManagerItemBasicDataBySurveyIdAndPagePosition.Contains(key))
                    {
                        var data =
                            _cacheManagerItemBasicDataBySurveyIdAndPagePosition.GetData(key) as List<BasicItemMetaData>;

                        if (data != null)
                            result.AddRange(data);
                    }
                    else
                    {
                        // if we don't have page in cache, it may be non-answerable
                        rt = rt ?? (rt = ResponseTemplateManager.GetResponseTemplate(responseTemplateId));
                        var page = rt.GetPageAtPosition(pagePos);
                        if (page == null || page.IsAnyAnswerableItemOnPage(rt, true))
                        {
                            // othercase stop cycle and get other data from db 
                            break;
                        }
                    }

                    if (pagePos == max)
                        return result;
                }
            }

            //Get list of categories, which sproc returns in order of position
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_ListBasicItemData");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateId);
            command.AddInParameter("LanguageCode", DbType.String, languageCode);
            command.AddInParameter("StartPagePosition", DbType.Int32, pagePos);
            command.AddInParameter("EndPagePosition", DbType.Int32, maxPagePosition);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(new BasicItemMetaData
                                   {
                                        ItemId = DbUtility.GetValueFromDataReader(reader, "ItemID", -1),
                                        ItemText = DbUtility.GetValueFromDataReader(reader, "ItemText", string.Empty),
                                        PagePosition = DbUtility.GetValueFromDataReader(reader, "PagePosition", -1),
                                        ParentId = DbUtility.GetValueFromDataReader(reader, "ParentID", (int?)null),
                                        RowNumber = DbUtility.GetValueFromDataReader(reader, "RowNumber", (int?)null)
                                   });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //cache data
            if (_cacheManagerItemBasicDataBySurveyIdAndPagePosition != null)
            {
                var groupedByPage = result.GroupBy(r => r.PagePosition);
                foreach (var page in groupedByPage)
                {
                    var key = responseTemplateId + "_" + page.Key;
                    var val = page.ToList();

                    _cacheManagerItemBasicDataBySurveyIdAndPagePosition.Add(key, val);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void MarkBasicItemsDataUpdated(ResponseTemplate rt)
        {
            if (rt != null)
            {
                for (int i = 1; i < rt.PageCount; i++)
                {
                    var key = rt.ID + "_" + i;
                    _cacheManagerItemBasicDataBySurveyIdAndPagePosition.Remove(key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void MarkBasicItemsDataUpdated(int responseTemplateId)
        {
            var rt = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);
            MarkBasicItemsDataUpdated(rt);
        }

        /// <summary>
        /// List items in the survey.
        /// </summary>
        /// <param name="responseTemplateId">ID of template</param>
        /// <param name="maxPagePosition">Max position of items to include.</param>
        /// <param name="includeNonAnswerable"></param>
        /// <param name="includeComposites"></param>
        /// <param name="includeChildren">Include child items in the list.  If false, only top-level items will be included.</param>
        /// <param name="languageCode">Language code for item text.</param>
        /// <param name="dependentItemId">ID of the row item. Helps to exclude the same row items</param>
        /// <returns>List of item ids.</returns>
        public static List<LightweightItemMetaData> ListResponseTemplateItems(int responseTemplateId, int? maxPagePosition, bool includeNonAnswerable, bool includeComposites, bool includeChildren, string languageCode, int? dependentItemId = null)
        {
            var itemList = new List<LightweightItemMetaData>();
            ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (rt == null)
            {
                return itemList;
            }

            //Iterate through pages to get items
            foreach (int pageId in rt.ListTemplatePageIds())
            {
                TemplatePage page = rt.GetPage(pageId);

                //If page is null or position > max position, move on
                if (page == null || (maxPagePosition.HasValue && maxPagePosition.Value < page.Position))
                {
                    continue;
                }

                //Otherwise, list items and get data
                int[] pageItemIds = page.ListItemIds();

                foreach (int pageItemId in pageItemIds)
                {
                    LightweightItemMetaData lightweightItemData = SurveyMetaDataProxy.GetItemData(pageItemId, false);

                    if (lightweightItemData == null)
                    {
                        continue;
                    }

                    if ((includeNonAnswerable || lightweightItemData.IsAnswerable)
                        && (includeComposites || (lightweightItemData.Children.Count == 0 && lightweightItemData.ItemType != "Matrix")))
                    {
                        itemList.Add(lightweightItemData);
                    }

                    //Add any children, if necessary
                    if (includeChildren)
                    {
                        //the matrix can not depend on it's children
                        if (dependentItemId.HasValue && lightweightItemData.ItemId == dependentItemId.Value)
                            continue;

                        bool needToAddChildren = true;
                        if (dependentItemId.HasValue && lightweightItemData.Children.Contains(dependentItemId.Value))
                        {
                            var itemData = GetConfigurationData(pageItemId);
                            if (itemData is MatrixItemData)
                            {
                                var matrix = itemData as MatrixItemData;
                                var dependentItemCoordinates = matrix.GetItemCoordinate(dependentItemId.Value);
                                if (dependentItemCoordinates.Column == 1)
                                {
                                    //dependent item is a row!
                                    itemList.AddRange(
                                        (from id in lightweightItemData.Children where matrix.GetItemCoordinate(id).Row != dependentItemCoordinates.Row select id)                                        
                                            .Select(childItemId => SurveyMetaDataProxy.GetItemData(childItemId, false))
                                            .Where(
                                                lightweightChildItemData =>
                                                    lightweightChildItemData != null
                                                    && (includeNonAnswerable || lightweightChildItemData.IsAnswerable)
                                             )
                                    );
                                    needToAddChildren = false;
                                }
                            }
                        }


                        if (needToAddChildren)
                        {
                            itemList.AddRange(
                                lightweightItemData.Children
                                    .Select(childItemId => SurveyMetaDataProxy.GetItemData(childItemId, false))
                                    .Where(
                                        lightweightChildItemData =>
                                            lightweightChildItemData != null
                                            && (includeNonAnswerable || lightweightChildItemData.IsAnswerable)
                                     )
                            );
                        }
                    }
                }
            }

            return itemList;
        }

        /// <summary>
        /// List categorized items of the survey
        /// </summary>
        /// <param name="responseTemplateId">ID of template</param>
        /// <param name="maxPagePosition">Max position of items to include.</param>
        /// <param name="languageCode">Language code for item text</param>
        /// <returns></returns>
        public static List<LightweightItemMetaData> ListResponseTemplateCategorizedItems(int responseTemplateId, int? maxPagePosition, string languageCode)
        {
            var itemList = new List<LightweightItemMetaData>();
            ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (rt == null)
            {
                return itemList;
            }

            //Iterate through pages to get items
            foreach (int pageId in rt.ListTemplatePageIds())
            {
                TemplatePage page = rt.GetPage(pageId);

                //If page is null or position > max position, move on
                if (page == null || (maxPagePosition.HasValue && maxPagePosition.Value < page.Position))
                {
                    continue;
                }

                //Otherwise, list items and get data
                int[] pageItemIds = page.ListItemIds();

                foreach (int pageItemId in pageItemIds)
                {
                    LightweightItemMetaData lightweightItemData = SurveyMetaDataProxy.GetItemData(pageItemId, false);

                    if (lightweightItemData == null)
                    {
                        continue;
                    }

                    if (lightweightItemData.ItemType.Equals("CategorizedMatrix", StringComparison.InvariantCultureIgnoreCase))
                    {
                        itemList.Add(lightweightItemData);
                    }

                }
            }

            return itemList;                    
        }

        /// <summary>
        /// Get a list of item type ids
        /// </summary>
        /// <returns></returns>
        public static List<ItemCategoryInfo> ListItemTypes()
        {
            if (_typeCategories.Count == 0)
            {
                //Lock before building collection
                lock (_lockObject)
                {
                    //Check after lock
                    if (_typeCategories.Count == 0)
                    {
                        //Get list of categories, which sproc returns in order of position
                        Database db = DatabaseFactory.CreateDatabase();
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemType_ListCategories");

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    int categoryId = DbUtility.GetValueFromDataReader(reader, "CategoryId", -1);
                                    string categoryName = DbUtility.GetValueFromDataReader(reader, "CategoryName", string.Empty);
                                    string nameTextId = DbUtility.GetValueFromDataReader(reader, "CategoryTextId", string.Empty);
                                    string descriptionTextId = DbUtility.GetValueFromDataReader(reader, "CategoryDescriptionTextId", string.Empty);

                                    if (categoryId > 0
                                        && Utilities.IsNotNullOrEmpty(categoryName)
                                        && Utilities.IsNotNullOrEmpty(nameTextId)
                                        && Utilities.IsNotNullOrEmpty(descriptionTextId))
                                    {
                                        _typeCategories.Add(new ItemCategoryInfo
                                        {
                                            CategoryId = categoryId,
                                            NameTextId = nameTextId,
                                            DescriptionTextId = descriptionTextId,
                                            CategoryName = categoryName
                                        });
                                    }
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }

                        //Now populate items for categories
                        //Get list of items, which sproc returns in order of position
                        command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemType_List");

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    int position = DbUtility.GetValueFromDataReader(reader, "Position", -1);
                                    if (position < 0)
                                        continue;

                                    int itemTypeId = DbUtility.GetValueFromDataReader(reader, "ItemTypeId", -1);
                                    string itemName = DbUtility.GetValueFromDataReader(reader, "ItemName", string.Empty);
                                    string categoryName = DbUtility.GetValueFromDataReader<string>(reader, "CategoryName", null) ?? "Other";

                                    //Do two things....1 set RT/library/etc. compatible flags for item info and
                                    // add item to list of item types in a category
                                    if (itemTypeId > 0
                                        && Utilities.IsNotNullOrEmpty(itemName))
                                    {
                                        //Update item info
                                        ItemTypeInfo typeInfo = GetTypeInfoById(itemTypeId);

                                        if (typeInfo != null)
                                        {
                                            lock (_lockObject)
                                            {
                                                typeInfo.SurveyCompatible = DbUtility.GetValueFromDataReader(reader, "RTCompatible", false);
                                                typeInfo.LibraryCompatible = DbUtility.GetValueFromDataReader(reader, "LibraryCompatible", false);
                                                typeInfo.ReportCompatible = DbUtility.GetValueFromDataReader(reader, "ReportCompatible", false);
                                                typeInfo.MobileCompatible = DbUtility.GetValueFromDataReader(reader, "MobileCompatible", false);
                                                typeInfo.TextIdPrefix = DbUtility.GetValueFromDataReader(reader, "TextIdPrefix", string.Empty);
                                                typeInfo.ParentCategory = categoryName;

                                                _cacheManagerTypesById.Add(itemTypeId.ToString(), typeInfo);
                                                _cacheManagerTypesByName.Add(itemName, typeInfo);


                                                //Add item to list of items for a category
                                                ItemCategoryInfo categoryInfo = _typeCategories.Find(a => a.CategoryName.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));

                                                if (categoryInfo != null)
                                                {
                                                    categoryInfo.ItemTypes.Add(typeInfo);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
            }

            return _typeCategories;
        }

        /// <summary>
        /// Get a datatable with a list of item categories
        /// </summary>
        /// <returns></returns>
        public static DataTable GetItemCategoryList()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemType_ListCategories");
            DataSet ds = db.ExecuteDataSet(command);

            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        /// <summary>
        /// Copy an item by exporting / importing it
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns>Copied item, otherwise null if import was not successful</returns>
        private static ItemData CopyItemData(ItemData itemData, CheckboxPrincipal principal)
        {
            if (itemData == null || itemData.ID == null)
            {
                return null;
            }

            var stringWriter = new StringWriter();
            var writer = new XmlTextWriter(stringWriter);

            itemData.Export(writer);
            
            var document = new XmlDocument();
            document.LoadXml(stringWriter.ToString());

            var itemElement = document.SelectSingleNode("Item");

            return CopyItem(principal, itemElement);
        }

        /// <summary>
        /// Copy an item's data, appearance, and text
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns>Copied item, otherwise null if import was not successful</returns>
        public static ItemData CopyItem(ItemData itemData, CheckboxPrincipal principal)
        {
            if (itemData == null || itemData.ID == null)
            {
                return null;
            }

            var itemCopy = CopyItemData(itemData, principal);

            //Save copy so it gets an ID
            //Save copy so it gets an ID
            if (itemCopy == null)
                return null;

            itemCopy.Save();

            //Get appearance of original item & copy
            var itemAppearance = AppearanceDataManager.GetAppearanceDataForItem(itemData.ID.Value);

            if (itemAppearance != null)
            {
                //Null id and save, which will cause new appearance data to be created
                itemAppearance.ID = null;
                itemAppearance.Save(itemCopy.ID);
            }

            //Copy text of original item
            ItemTextDecorator textDecorator = itemData.CreateTextDecorator(TextManager.DefaultLanguage);
            textDecorator.Copy(itemCopy);

            if (itemCopy.ID.HasValue)
            {
                var bindedField = ProfileManager.GetPropertiesList().FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == itemData.ID));

                if (bindedField != null)
                    PropertyBindingManager.AddSurveyItemProfilePropertyMapping((int)itemCopy.ID, bindedField.FieldId);
            }

            //Re-save item copy to ensure proper updated timestamp
            itemCopy.Save();

            return itemCopy;
        }

        /// <summary>
        /// Retrieve the text for an item, up to a maximum length.   If the length is exceeded,
        /// the text will be truncated and ellipsis appended.  Use alias instead of text if so
        /// specified.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"></param>
        /// <param name="languageCode"></param>
        /// <param name="maxLength"></param>
        /// <param name="preferAlias"></param>
        /// <returns></returns>
        public static string GetItemOptionText(int itemId, int optionId, string languageCode, int? maxLength, bool preferAlias)
        {
            ItemData itemData = GetConfigurationData(itemId);

            if (itemData == null || !(itemData is SelectItemData))
            {
                return string.Empty;
            }

            //Find the option
            ListOptionData optionData = ((SelectItemData)itemData).GetOption(optionId);

            return optionData == null
                ? string.Empty :
                GetItemOptionText(itemData, optionData, languageCode, maxLength, preferAlias);
        }

        /// <summary>
        /// Retrieve the text for an item, up to a maximum length.   If the length is exceeded,
        /// the text will be truncated and ellipsis appended.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="optionData"></param>
        /// <param name="languageCode"></param>
        /// <param name="maxLength"></param>
        /// <param name="preferAlias">Prefer alias for getting text.</param>
        /// <returns></returns>
        public static string GetItemOptionText(ItemData itemData, ListOptionData optionData, string languageCode, int? maxLength, bool preferAlias)
        {
            //Use alias if requested
            if (preferAlias && Utilities.IsNotNullOrEmpty(optionData.Alias))
            {
                return Utilities.TruncateText(optionData.Alias, maxLength);
            }

            ItemTextDecorator decorator = itemData.CreateTextDecorator(languageCode);
            decorator.AddAlternateLanguages(new List<string>(TextManager.SurveyLanguages));
            string text = string.Empty;

            //Try to get labelled item text/subtext
            if (decorator is SelectItemTextDecorator)
            {
                text = ((SelectItemTextDecorator)decorator).GetOptionText(optionData.Position);
            }

            //If no text, try alias
            if (Utilities.IsNullOrEmpty(text))
            {
                text = optionData.Alias;
            }

            //Get default for na text for rating scales
            if (Utilities.IsNullOrEmpty(text))
            {
                if (itemData is RatingScaleItemData && optionData.IsOther)
                {
                    text = TextManager.GetText("/controlText/ratingScale/notApplicableDefault", languageCode);

                    if (Utilities.IsNullOrEmpty(text))
                    {
                        text = "n/a";
                    }
                }
            }

            //Get default for na text for other texts scales
            if (Utilities.IsNullOrEmpty(text))
            {
                if (itemData is SelectItemData)
                {
                    if (optionData.IsOther)
                        text = TextManager.GetText("/common/otherTextDefault", languageCode, "Other");
                    else if (optionData.IsNoneOfAbove)
                        text = TextManager.GetText("/common/noneOfAboveTextDefault", languageCode, "None Of The Above");
                }
            }

            //If still no text, use the item type name
            if (Utilities.IsNullOrEmpty(text))
            {
                text = optionData.OptionID.ToString();
            }

            //Remove characters from the middle to make text have proper length
            return Utilities.TruncateText(text, maxLength);
        }

        /// <summary>
        /// Retrieve the text for an item, up to a maximum length.   If the length is exceeded,
        /// the text will be truncated and ellipsis appended.  Use alias instead of text if so
        /// specified.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="maxLength"></param>
        /// <param name="preferAlias"></param>
        /// <param name="parentItemId"></param>
        /// <returns></returns>
        public static string GetItemText(int itemId, string languageCode, int? maxLength, bool preferAlias, int? parentItemId)
        {
            //If looking up parent, ensure parent loaded by metadata cache first
            if (parentItemId.HasValue)
            {
                //Ensure parent loaded
                var parentItemData = SurveyMetaDataProxy.GetItemData(parentItemId.Value, false);
            }

            return Utilities.TruncateText(
                SurveyMetaDataProxy.GetItemText(itemId, languageCode, preferAlias, parentItemId.HasValue),
                maxLength);
        }

        /// <summary>
        /// Get a list of option texts for a select item, up to a maximum length.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetOptionTexts(SelectItemData itemData, string languageCode, int? maxLength)
        {
            var texts = new Dictionary<int, string>();
            var decorator = itemData.CreateTextDecorator(languageCode) as SelectItemTextDecorator;

            if (decorator != null)
            {
                ReadOnlyCollection<ListOptionData> options = itemData.Options;

                foreach (ListOptionData option in options)
                {
                    if (itemData is RatingScaleItemData)
                    {
                        texts[option.OptionID] = option.IsOther ? TextManager.GetText(((RatingScaleItemData)itemData).NotApplicableTextID, languageCode) : option.Points.ToString();
                    }
                    else
                    {
                        texts[option.OptionID] = GetOptionText(option, decorator, maxLength);
                    }
                }
            }

            return texts;
        }

        /// <summary>
        /// Do work of truncating option text, etc.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="decorator"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static string GetOptionText(ListOptionData option, SelectItemTextDecorator decorator, int? maxLength)
        {
            string text;
            if (option.IsOther)
                text = decorator.OtherText;
            else if (option.IsNoneOfAbove)
                text = decorator.NoneOfAboveText;
            else
                text = decorator.GetOptionText(option.Position);

            if (Utilities.IsNullOrEmpty(text))
            {
                text = option.Alias;
            }

            if (Utilities.IsNullOrEmpty(text))
            {
                text = option.Position.ToString();
            }

            return Utilities.TruncateText(text, maxLength);
        }


        /// <summary>
        /// Retrieve the text for an item, up to a maximum length.   If the length is exceeded,
        /// the text will be truncated and ellipsis appended.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="maxLength"></param>
        /// <param name="preferAlias"></param>
        /// <param name="lookupParentText">If no text or alias is found for the item, look for the item's matrix parent for row text or column text</param>
        /// <returns></returns>
        public static string GetItemText(int itemId, string languageCode, int? maxLength, bool preferAlias, bool lookupParentText)
        {
            return GetItemText(
                itemId,
                languageCode,
                maxLength,
                preferAlias,
                lookupParentText ? GetItemParent(itemId) : null);
        }


        /// <summary>
        /// Get the id of an item's parent (if any).
        /// </summary>
        /// <param name="childItemId"></param>
        /// <returns></returns>
        /// <remarks>Currently only works for matrix questions.</remarks>
        public static int? GetItemParent(int childItemId)
        {
            //Step 1: Find the matrix parent
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetMatrixParentId");
            command.AddInParameter("ChildItemId", DbType.Int32, childItemId);

            int? matrixId = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        matrixId = DbUtility.GetValueFromDataReader<int?>(reader, "MatrixId", null);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return matrixId;
        }

        /// <summary>
        /// Copy list data from one item to another, preserving all id values.  This allows
        /// changing from select 1 to select many, etc.  The existing item is deleted.
        /// </summary>
        /// <param name="existingItem"></param>
        /// <param name="newItemName"></param>
        /// <returns></returns>
        public static SelectItemData ChangeSelectItemType(SelectItemData existingItem, string newItemName, CheckboxPrincipal principal)
        {
            //Making this work requires a little juggling.  First a new item of the correct type needs to be created
            // and given the id of the existing item.  Then the existing item needs clean up.
            //Store the texts for the existing item
            Dictionary<string, string> itemTexts = TextManager.GetAllTexts(existingItem.TextID);
            Dictionary<string, string> itemSubTexts = TextManager.GetAllTexts(existingItem.SubTextID);
            Dictionary<string, string> otherPrompts = TextManager.GetAllTexts(existingItem.OtherTextID);

            //Create a new item
            var newItemData = CreateConfigurationData(newItemName) as SelectItemData;

            if (newItemData == null)
            {
                return null;
            }

            if (existingItem.ID == null)
            {
                return null;
            }
            

            //Get new appearance code
            string newAppearanceCode = AppearanceDataManager.GetDefaultAppearanceCodeForType(newItemData.ItemTypeID);

            //Copy properties from old item to new item.
            newItemData.IsRequired = existingItem.IsRequired;
            newItemData.AllowOther = existingItem.AllowOther;
            newItemData.Randomize = existingItem.Randomize;
            newItemData.CreatedBy = principal.Identity.Name;

            //Save the new item
            newItemData.Save();

            //Delete existing and assign its id, lists, and options to the new item
            UpdateSelectItemID(existingItem, newItemData);

            //Update appearance code
            //Get existing appearance
            var existingAppearance = AppearanceDataManager.GetAppearanceDataForItem(existingItem.ID.Value) as SelectLayout;

            if (existingAppearance != null
                && existingAppearance.ID != null
                && newItemData.ID != null)
            {
                AppearanceDataManager.UpdateAppearanceCode(existingAppearance.ID.Value, newAppearanceCode);

                //Load the new appearance
                var newAppearance = AppearanceDataManager.GetAppearanceDataForItem(newItemData.ID.Value) as SelectLayout;

                if (newAppearance != null)
                {
                    //Copy properties
                    //TODO: Appearance CopyTo
                    //existingAppearance.CopyTo(newAppearance);

                    //Save
                    newAppearance.Save(newItemData.ID.Value);
                }
            }

            //Reload the item
            newItemData = GetConfigurationData(existingItem.ID.Value) as SelectItemData;

            if (newItemData == null)
            {
                return null;
            }

            //Store the texts for the new item
            foreach (string languageCode in itemTexts.Keys)
            {
                TextManager.SetText(newItemData.TextID, languageCode, itemTexts[languageCode]);
            }

            //Store the sub texts for the new item
            foreach (string languageCode in itemSubTexts.Keys)
            {
                TextManager.SetText(newItemData.SubTextID, languageCode, itemSubTexts[languageCode]);
            }

            //Store the other prompts for the new item
            foreach (string languageCode in otherPrompts.Keys)
            {
                TextManager.SetText(newItemData.OtherTextID, languageCode, otherPrompts[languageCode]);
            }

            return newItemData;
        }

        /// <summary>
        /// Update select item information in the database to move options and lists from one item to another and delete
        /// the old item
        /// </summary>
        /// <param name="existingItem"></param>
        /// <param name="newItem"></param>
        private static void UpdateSelectItemID(ItemData existingItem, ItemData newItem)
        {
            //Update type id
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper updateTypeCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Item_UpdateType");
            updateTypeCommand.AddInParameter("ItemID", DbType.Int32, existingItem.ID);
            updateTypeCommand.AddInParameter("ItemTypeID", DbType.Int32, newItem.ItemTypeID);
            updateTypeCommand.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(updateTypeCommand);

            //Update select item  specific data
            DBCommandWrapper updateSelectCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateSelectItemID");
            updateSelectCommand.AddInParameter("ExistingItemID", DbType.Int32, existingItem.ID);
            updateSelectCommand.AddInParameter("ExistingItemName", DbType.String, existingItem.ItemTypeName);
            updateSelectCommand.AddInParameter("NewItemID", DbType.Int32, newItem.ID);
            updateSelectCommand.AddInParameter("NewItemName", DbType.String, newItem.ItemTypeName);

            db.ExecuteNonQuery(updateSelectCommand);
        }

        /// <summary>
        /// Insert an entry into the base item table.
        /// </summary>
        public static int InsertAbstractItem(string itemTypeName, string alias, bool isActive, string CreatedBy, IDbTransaction transaction)
        {
            ItemTypeInfo typeInfo = GetTypeInfoByName(itemTypeName);

            if(typeInfo == null)
            {
                return -1;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_Insert");

            command.AddInParameter("ItemTypeID", DbType.Int32, typeInfo.TypeId);
            command.AddInParameter("Alias", DbType.String, alias);
            command.AddInParameter("CreatedDate", DbType.DateTime, DateTime.Now);
            command.AddInParameter("IsActive", DbType.Boolean, isActive);
            command.AddOutParameter("ItemID", DbType.Int32, 4);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);

            if (transaction != null)
            {
                db.ExecuteNonQuery(command, transaction);
            }
            else
            {
                db.ExecuteNonQuery(command);
            }

            //Set the item id so it can be used by the derived class' create method
            object id = command.GetParameterValue("ItemID");

            if (id == null || id == DBNull.Value)
            {
                return - 1;
            }

            return (int)id;
        }

        /// <summary>
        /// Mark an item as updated
        /// </summary>
        public static void UpdateAbstractItem(int itemId, string alias, bool isActive, string ModifiedBy, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_Update");

            command.AddInParameter("ItemID", DbType.Int32, itemId);
            command.AddInParameter("Alias", DbType.String, alias);
            command.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);
            command.AddInParameter("IsActive", DbType.Boolean, isActive);
            command.AddInParameter("ModifiedBy", DbType.String, ModifiedBy);

            if (transaction != null)
            {
                db.ExecuteNonQuery(command, transaction);
            }
            else
            {
                db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Mark an item as updated
        /// </summary>
        public static void DeleteAbstractItem(int itemId, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_Delete");
            command.AddInParameter("ItemID", DbType.Int32, itemId);
            command.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);

            if (transaction != null)
            {
                db.ExecuteNonQuery(command, transaction);
            }
            else
            {
                db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Return a boolean value indicating if the item type is in the "Questions" category.
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <returns></returns>
        public static bool IsItemTypeQuestion(string itemTypeName)
        {
            //Get info and list of question types
            ItemCategoryInfo categoryInfo = _typeCategories.Find(catInfo => catInfo.CategoryName.Equals("Question", StringComparison.InvariantCultureIgnoreCase));

            //Check if specified type is in list
            if (categoryInfo != null 
                && categoryInfo.ItemTypes.Find(itemInfo => itemInfo.TypeName.Equals(itemTypeName, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ItemData ImportItem(CheckboxPrincipal principal, XmlNode xmlNode, ItemImportReader itemReader, PersistedDomainObject.ReadExternalDataCallback callback = null)
        {
            //Get item 
            var itemTypeName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ItemTypeName"));

            if(string.IsNullOrEmpty(itemTypeName))
            {
                return null;
            }

            //Try to load by type name first as that's the only way guaranteed to cross server boundaries as
            // type ids are not fixed.  For backwards compatibility though, use type id as backup
            var itemData = CreateConfigurationData(itemTypeName);
            //Items that are not exportable reference data by ID in source database and are not exportable/importable
            // for that reason.
            // for that reason.
            if (itemData == null || !itemData.IsExportable)
            {
                return null;
            }

            itemData.Import(xmlNode, itemReader, callback, principal);
            itemData.CreatedBy = principal.Identity.Name;
            itemData.Save();

            return itemData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ItemData ImportItem(CheckboxPrincipal principal, XmlNode xmlNode, PersistedDomainObject.ReadExternalDataCallback callback = null)
        {
            return ImportItem(principal, xmlNode, null, callback);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>The difference between "ImportItem" and "CopyItem" is that the last one doesn't check "IsExportable" property.</remarks>
        /// <returns></returns>
        public static ItemData CopyItem(CheckboxPrincipal principal, XmlNode xmlNode, PersistedDomainObject.ReadExternalDataCallback callback = null)
        {
            //Get item 
            var itemTypeName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ItemTypeName"));

            if (string.IsNullOrEmpty(itemTypeName))
            {
                return null;
            }

            //Try to load by type name first as that's the only way guaranteed to cross server boundaries as
            // type ids are not fixed.  For backwards compatibility though, use type id as backup
            var itemData = CreateConfigurationData(itemTypeName);

            //Items that are not exportable reference data by ID in source database and are not exportable/importable
            // for that reason.
            if (itemData == null)
            {
                return null;
            }

            itemData.Import(xmlNode, callback, principal);

            itemData.CreatedBy = principal.Identity.Name;
            itemData.Save();

            return itemData;
        }

        /// <summary>
        /// Gets the prototype item id from the same column as the given item
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int? GetPrototypeItemID(int itemID, bool useCache = true)
        {
            if (_cacheManagerItemPrototypeByItemId.Contains(itemID.ToString()))
            {
                return _cacheManagerItemPrototypeByItemId.GetData(itemID.ToString()) as int?;
            }

            int? prototypeId = null;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetPrototype");
            command.AddInParameter("ItemID", DbType.Int32, itemID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        prototypeId = DbUtility.GetValueFromDataReader<int?>(reader, "ColumnPrototypeID", null);
                        if (prototypeId.HasValue)
                            _cacheManagerItemPrototypeByItemId.Add(itemID.ToString(), prototypeId.Value);
                    }
                    else
                        _cacheManagerItemPrototypeByItemId.Add(itemID.ToString(), null);
                }
                finally
                {
                    reader.Close();
                }
            }
            return prototypeId;
        }

        /// <summary>
        /// Returns a list of existing item aliases
        /// </summary>
        /// <returns></returns>
        public static List<string> GetItemAliases()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_GetAliases");

            var itemAliases = new List<string>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var alias = DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(alias))
                        {
                            itemAliases.Add(alias);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return itemAliases;
        }
    }
}
