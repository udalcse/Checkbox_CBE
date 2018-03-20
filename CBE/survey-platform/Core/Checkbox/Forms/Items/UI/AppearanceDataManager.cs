using System;
using System.Collections;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Caching;

namespace Checkbox.Forms.Items.UI
{
    ///<summary>
    ///Static manager class for managing item appearance data.
    ///</summary>
    public static class AppearanceDataManager
    {
        private static readonly CacheManager _cacheManagerDefaultTypes;
        private static readonly CacheManager _cacheManagerTypeNamesByID;
        private static readonly CacheManager _cacheManagerTypeAssembliesByID;
        private static readonly CacheManager _cacheManagerTypeNamesByCode;
        private static readonly CacheManager _cacheManagerTypeAssembliesByCode;
        private static readonly CacheManager _cacheManagerAppearanceDataByItemID;

        /// <summary>
        /// Constructor to init the default types table
        /// </summary>
        static AppearanceDataManager()
        {
            _cacheManagerDefaultTypes = CacheFactory.GetCacheManager("appearanceDataDefaultTypesCacheManager");
            _cacheManagerTypeNamesByID = CacheFactory.GetCacheManager("appearanceTypeNamesByIDCacheManager");
            _cacheManagerTypeAssembliesByID = CacheFactory.GetCacheManager("appearanceTypeAssembliesByIDCacheManager");
            _cacheManagerTypeNamesByCode = CacheFactory.GetCacheManager("appearanceTypeNamesByCodeCacheManager");
            _cacheManagerTypeAssembliesByCode = CacheFactory.GetCacheManager("appearanceTypeAssembliesByCodeCacheManager");
            _cacheManagerAppearanceDataByItemID = CacheFactory.GetCacheManager("appearanceDataByItemIDCacheManager");
        }

        /// <summary>
        /// Get appearance data
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static AppearanceData GetAppearanceData(string typeName, Int32 id)
        {
            AppearanceDataFactory factory = new AppearanceDataFactory();
            AppearanceData data = factory.CreateAppearanceData(typeName);
            data.ID = id;
            data.Load();
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceData(string typeName)
        {
            AppearanceDataFactory factory = new AppearanceDataFactory();
            AppearanceData data = factory.CreateAppearanceData(typeName);
            return data;
        }

        /// <summary>
        /// Get the specified appearance
        /// </summary>
        /// <param name="appearanceID"></param>
        /// <param name="appearanceCode"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceData(Int32 appearanceID, string appearanceCode, PersistedDomainObjectDataSet ds)
        {
            AppearanceData appearanceData = GetAppearanceDataForCode(appearanceCode);

            if (appearanceData != null)
            {
                appearanceData.ID = appearanceID;

                if (ds != null)
                {
                    appearanceData.Load(ds);
                }
                else
                {
                    appearanceData.Load();
                }
            }

            return appearanceData;
        }

        /// <summary>
        /// Get the appearance with the specified appearance id
        /// </summary>
        /// <param name="appearanceID"></param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceData(Int32 appearanceID)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearanceData_Get");
            command.AddInParameter("AppearanceID", DbType.Int32, appearanceID);

            AppearanceData data = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        string appearanceCode = (string)reader["AppearanceCode"];

                        data = GetAppearanceDataForCode(appearanceCode);

                        // now that we have the default appearance data
                        // we reload the style specific data
                        data.ID = appearanceID;
                        data.Load();
                    }
                }
                catch (Exception ex)
                {
                    bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                    if (rethrow)
                        throw;
                }
                finally
                {
                    reader.Close();
                }
            }

            return data;
        }

        /// <summary>
        /// Get the appearance data object associated with the specified item.
        /// </summary>
        /// <param name="itemId">ID of the item to get associated appearance data object for.</param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceDataForItem(int itemId)
        {
            return GetAppearanceDataForItem(itemId, null);
        }

        /// <summary>
        /// Removes appearance data from cache for item
        /// </summary>
        /// <param name="itemId"></param>
        public static void CleanUpAppearanceDataCacheForItem(int itemId)
        {
            if (_cacheManagerAppearanceDataByItemID.Contains(itemId.ToString()))
                _cacheManagerAppearanceDataByItemID.Remove(itemId.ToString());
        }

        /// <summary>
        /// Get the appearance data object associated with the specified item and execute read queries
        /// in the context of the specified transaction.
        /// </summary>
        /// <param name="itemId">ID of the item to get the appearance of.</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceDataForItem(int itemId, IDbTransaction t)
        {
            //Don't bother for negative ids
            if (itemId <= 0)
            {
                return null;
            }


            AppearanceData data = _cacheManagerAppearanceDataByItemID.GetData(itemId.ToString()) as AppearanceData;
            if (data != null)
                return data;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetAppearance");
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            //Open a reader, using a transaction if provided
            IDataReader reader = t != null ? db.ExecuteReader(command, t) : db.ExecuteReader(command);

            try
            {
                if (reader.Read())
                {
                    string dataTypeName = (string)reader["DataTypeName"];
                    string dataTypeAssembly = (string)reader["DataTypeAssembly"];
                    int appearanceId = Convert.ToInt32(reader["AppearanceID"]);

                    data = GetAppearanceData(dataTypeName + "," + dataTypeAssembly, appearanceId);
                    _cacheManagerAppearanceDataByItemID.Add(itemId.ToString(), data);
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                    throw;
            }
            finally
            {
                reader.Close();
            }


            return data;
        }

        /// <summary>
        /// Get the default appearance code for an item type
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <returns></returns>
        public static string GetDefaultAppearanceCodeForType(int itemTypeId)
        {
            string appearanceCode = null;

            if (_cacheManagerDefaultTypes.Contains(itemTypeId.ToString()))
            {
                appearanceCode = (string)_cacheManagerDefaultTypes.GetData(itemTypeId.ToString());
            }
            else
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_GetItemDefault");
                command.AddInParameter("ItemTypeID", DbType.Int32, itemTypeId);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            appearanceCode = (string)reader["DefaultAppearanceCode"];
                        }
                    }
                    finally
                    {
                        reader.Close();

                        if (appearanceCode != null)
                        {
                            _cacheManagerDefaultTypes.Add(itemTypeId.ToString(), appearanceCode);
                        }
                    }
                }
            }
            return appearanceCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <returns></returns>
        public static AppearanceData GetDefaultAppearanceDataForType(int itemTypeId)
        {
            string dataTypeName = null;
            string dataTypeAssembly = null;

            if (_cacheManagerTypeNamesByID.Contains(itemTypeId.ToString()))
            {
                dataTypeName = (string)_cacheManagerTypeNamesByID.GetData(itemTypeId.ToString());
                dataTypeAssembly = (string)_cacheManagerTypeAssembliesByID.GetData(itemTypeId.ToString());
            }
            else
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_GetItemDefault");
                command.AddInParameter("ItemTypeID", DbType.Int32, itemTypeId);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            dataTypeName = (string)reader["DataTypeName"];
                            dataTypeAssembly = (string)reader["DataTypeAssembly"];
                        }
                    }
                    catch (Exception ex)
                    {
                        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                        if (rethrow)
                            throw;
                    }
                    finally
                    {
                        reader.Close();

                        if (dataTypeName != null && dataTypeAssembly != null)
                        {
                            _cacheManagerTypeNamesByID.Add(itemTypeId.ToString(), dataTypeName);
                            _cacheManagerTypeAssembliesByID.Add(itemTypeId.ToString(), dataTypeAssembly);
                        }
                    }
                }
            }

            AppearanceData data = null;

            if (dataTypeName != null && dataTypeName.Trim() != string.Empty && dataTypeAssembly != null && dataTypeAssembly.Trim() != string.Empty)
            {
                data = GetAppearanceData(dataTypeName + "," + dataTypeAssembly);
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appearanceCode"></param>
        /// <returns></returns>
        public static AppearanceData GetAppearanceDataForCode(string appearanceCode)
        {
            string dataTypeName = null;
            string dataTypeAssembly = null;

            if (_cacheManagerTypeNamesByCode.Contains(appearanceCode.ToLower()))
            {
                dataTypeName = (string)_cacheManagerTypeNamesByCode.GetData(appearanceCode.ToLower());
                dataTypeAssembly = (string)_cacheManagerTypeAssembliesByCode.GetData(appearanceCode.ToLower());
            }
            else
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_GetByCode");
                command.AddInParameter("AppearanceCode", DbType.String, appearanceCode);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            dataTypeName = (string)reader["DataTypeName"];
                            dataTypeAssembly = (string)reader["DataTypeAssembly"];
                        }
                    }
                    finally
                    {
                        reader.Close();

                        if (dataTypeName != null && dataTypeAssembly != null)
                        {
                            _cacheManagerTypeNamesByCode.Add(appearanceCode.ToLower(), dataTypeName);
                            _cacheManagerTypeAssembliesByCode.Add(appearanceCode.ToLower(), dataTypeAssembly);
                        }
                    }
                }
            }

            return GetAppearanceData(dataTypeName + "," + dataTypeAssembly);
        }

        /// <summary>
        /// Copy the specified appearance
        /// </summary>
        /// <param name="appearanceToCopy"></param>
        /// <returns></returns>
        public static AppearanceData CopyAppearance(AppearanceData appearanceToCopy)
        {
            return appearanceToCopy.Copy();
        }

        /// <summary>
        /// Update an item appearance code.
        /// </summary>
        /// <param name="appearanceID"></param>
        /// <param name="newAppearanceCode"></param>
        public static void UpdateAppearanceCode(int appearanceID, string newAppearanceCode)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_UpdateCode");

            command.AddInParameter("AppearanceID", DbType.Int32, appearanceID);
            command.AddInParameter("NewAppearanceCode", DbType.String, newAppearanceCode);

            db.ExecuteNonQuery(command);
        }
    }
}