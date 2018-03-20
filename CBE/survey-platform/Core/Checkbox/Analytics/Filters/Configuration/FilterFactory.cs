using System;
using System.Collections.Generic;
using System.Data;

using Checkbox.Common;
using Checkbox.Forms.Logic;

using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Factory for creating and loading filter data objects through reflection based on type information maintained the database.
    /// </summary>
    public static class FilterFactory
    {
        private static Dictionary<string, Type> _typeCache;

        /// <summary>
        /// Get a reference to the type cache
        /// </summary>
        private static Dictionary<string, Type> TypeCache
        {
            get
            {
                if (_typeCache == null)
                {
                    _typeCache = new Dictionary<string, Type>();
                }

                return _typeCache;
            }
        }

        /// <summary>
        /// Based on a filter type name, get the filter type
        /// </summary>
        /// <param name="filterTypeName"></param>
        /// <returns></returns>
        private static Type GetFilterType(string filterTypeName)
        {
            //Get the assembly qualified name of the type
            if (TypeCache.ContainsKey(filterTypeName))
            {
                return TypeCache[filterTypeName];
            }
            
            Type theType = null;

            string typeAQN = GetTypeAQN(filterTypeName);

            if (Utilities.IsNotNullOrEmpty(typeAQN))
            {
                theType = Type.GetType(typeAQN);

                if (theType != null)
                {
                    TypeCache[filterTypeName] = theType;
                }
            }

            return theType;
        }

        /// <summary>
        /// Get the AQN of the filter type
        /// </summary>
        /// <param name="filterTypeName"></param>
        /// <returns></returns>
        private static string GetTypeAQN(string filterTypeName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_GetTypeInfo");
            command.AddInParameter("FilterTypeName", DbType.String, filterTypeName);

            string typeAQN = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        string assemblyName = DbUtility.GetValueFromDataReader<string>(reader, "DataTypeAssemblyName", null);
                        string className = DbUtility.GetValueFromDataReader<string>(reader, "DataTypeClassName", null);

                        if (Utilities.IsNotNullOrEmpty(assemblyName) && Utilities.IsNotNullOrEmpty(className))
                        {
                            typeAQN = className + "," + assemblyName;
                        }
                    }
                }
                catch
                {
                    reader.Close();
                    throw;
                }
            }

            return typeAQN;
        }

        /// <summary>
        /// Get a filter's type name from its id
        /// </summary>
        /// <param name="filterId"></param>
        /// <returns></returns>
        private static string GetFilterTypeName(int filterId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_GetTypeName");
            command.AddInParameter("FilterID", DbType.Int32, filterId);

            string typeName = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        typeName = DbUtility.GetValueFromDataReader<string>(reader, "FilterTypeName", null);
                    }
                }
                catch
                {
                    reader.Close();
                    throw;
                }
            }

            return typeName;
        }

        /// <summary>
        /// Instantiate a <see cref="FilterData"/> object of the specified type (Item, Profile, Response).
        /// </summary>
        /// <param name="filterTypeName">Type of filter to create.</param>
        /// <returns><see cref="FilterData"/> object for the specified type of filter.</returns>
        /// <exception cref="Exception">Exceptions are thrown if the type name can't be matched to a FilterData class.</exception>
        public static FilterData CreateFilterData(string filterTypeName)
        {
            Type filterType = GetFilterType(filterTypeName);

            if (filterType == null)
            {
                throw new Exception("Unable to locate type information for filter type: " + filterTypeName);
            }

            if (!typeof(FilterData).IsAssignableFrom(filterType))
            {
                throw new Exception("Filter type data class for [" + filterTypeName + "] does not extend FilterData class.");
            }
            
            FilterData data =  (FilterData)Activator.CreateInstance(filterType);

            if (data != null)
            {
                data.Initialize(filterTypeName);
            }

            return data;
        }

        /// <summary>
        /// Create the unloaded filter data for the filter with the specified id.
        /// </summary>
        /// <param name="filterId"></param>
        /// <returns></returns>
        public static FilterData CreateFilterData(int filterId)
        {
            string filterTypeName = GetFilterTypeName(filterId);

            if (string.IsNullOrEmpty(filterTypeName))
            {
                return null;
            }

            var filterData = CreateFilterData(filterTypeName);

            if (filterData == null)
            {
                return null;
            }

            filterData.ID = filterId;
            if (filterId > 0)
                filterData.Load();

            return filterData;
        }
            
        /// <summary>
        /// Load a filter data object with the specified filter ID.  The type of filter returned will depend on the type information 
        /// stored for the filter data in the database, which will be determined through a lookup in the main filter type table.
        /// </summary>
        /// <param name="filterId">ID of filter to load.</param>
        /// <returns><see cref="FilterData"/> object for the filter with the specified ID, or NULL if no filter data is found
        /// for the specified Id.</returns>
        public static FilterData GetFilterData(int filterId)
        {
            var filterData = CreateFilterData(filterId);

            if (filterData != null)
            {
                filterData.Load();
            }

            return filterData;
        }

        /// <summary>
        /// Create a new <see cref="FilterData"/> object.
        /// </summary>
        /// <param name="filterTypeName">Type name of <see cref="FilterData"/> object to create.</param>
        /// <param name="op"><see cref="LogicalOperator"/> for filter.</param>
        /// <param name="value">Comparison value to used.</param>
        /// <returns><see cref="FilterData"/> object initialized with the specified values.</returns>
        public static FilterData CreateFilterData(string filterTypeName, LogicalOperator op, object value)
        {
            FilterData f = CreateFilterData(filterTypeName);

            if (f != null)
            {
                f.Operator = op;
                f.Value = value;
            }

            return f;
        }
    }
}
