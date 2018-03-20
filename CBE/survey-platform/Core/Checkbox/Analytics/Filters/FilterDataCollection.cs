using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Collection of filter configuration objects.
    /// </summary>
    [Serializable]
    public abstract class FilterDataCollection
    {
        private DataSet _filterData;

        private List<FilterData> _filterDataObjects;

        /// <summary>
        /// Constructor
        /// </summary>
        protected FilterDataCollection()
        {
            InitializeData();
        }

        /// <summary>
        /// Initialize the filter data
        /// </summary>
        protected virtual void InitializeData()
        {
            _filterDataObjects = new List<FilterData>();

            _filterData = new DataSet();

            DataTable filterMapTable = new DataTable();
            filterMapTable.Columns.Add("FilterID", typeof(Int32));
            filterMapTable.Columns.Add("ParentID", typeof(Int32));
            filterMapTable.Columns.Add("ParentType", typeof(string));
            filterMapTable.TableName = FilterMapTableName;

            _filterData.Tables.Add(filterMapTable);
        }

        /// <summary>
        /// Get/set filter parent id.
        /// </summary>
        /// <remarks>Typically the parent of a filter collection is an <see cref="Analysis"/> when running a report or is a <see cref="ResponseTemplate"/> when editing 
        /// a survye, so this value will represent the ID of the <see cref="Analysis"/> or <see cref="ResponseTemplate"/> this filter collection is associated with.</remarks>
        public Int32 ParentID { get; set; }

        /// <summary>
        /// Get/set the type of a parent for this filter collection.
        /// </summary>
        public virtual string ParentType { get; set; }

        /// <summary>
        /// Get the raw data in <see cref="DataSet"/> form for the filter data collection.
        /// </summary>
        protected DataSet FilterData
        {
            get { return _filterData; }
        }

        /// <summary>
        /// Retrieve configuration information for the filter collection from the database and populate the list of <see cref="FilterData"/> objects
        /// contained in the collection.
        /// </summary>
        /// <param name="parentID">ID of the parent object (see <see cref="ParentID"/>) for the filter collection.</param>
        /// <returns><see cref="DataSet"/> containing configuration information for the filter collection.</returns>
        public DataSet Load(Int32 parentID)
        {
            ParentID = parentID;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filters_Get");
            command.AddInParameter("ParentID", DbType.Int32, parentID);
            command.AddInParameter("ParentType", DbType.String, ParentType);

            DataSet ds = db.ExecuteDataSet(command);

            if (ds.Tables.Count == 1)
            {
                ds.Tables[0].TableName = FilterMapTableName;
            }

            _filterDataObjects.Clear();
            FilterMapTable.Clear();

            //Now get the data for each filter
            if (ds.Tables.Contains(FilterMapTableName))
            {
                DataRow[] rows = ds.Tables[FilterMapTableName].Select("ParentID = " + parentID, null, DataViewRowState.CurrentRows);

                foreach (DataRow row in rows)
                {
                    if (row["FilterID"] != DBNull.Value)
                    {
                        int filterId = DbUtility.GetValueFromDataRow(row, "FilterID", -1);

                        FilterData filterData = FilterFactory.CreateFilterData(filterId);

                        if (filterData != null)
                        {
                            _filterDataObjects.Add(filterData);

                            FilterMapTable.ImportRow(row);
                        }
                    }
                }
            }

            //Merge the data back in
            foreach (FilterData f in _filterDataObjects)
            {
                ds.Merge(f.GetConfigurationDataSet(f.ID.Value));
            }

            return ds;
        }

        /// <summary>
        /// Import filters from the specified <see cref="DataSet"/>.  This clears internal collections so it should
        /// only be called on a new filter collection.
        /// </summary>
        /// <param name="ds"><see cref="DataSet"/> containing filter information to import.</param>
        /// <param name="parentID">ID of parent object of the filter collection.</param>
        public void Import(DataSet ds, Int32 parentID)
        {
            _filterDataObjects.Clear();
            FilterMapTable.Clear();

            if (ds != null && ds.Tables.Contains(FilterMapTableName))
            {
                DataRow[] rows = ds.Tables[FilterMapTableName].Select("ParentID = " + parentID + " AND ParentType = '" + ParentType + "'", null, DataViewRowState.CurrentRows);

                foreach (DataRow row in rows)
                {
                    int? filterId = DbUtility.GetValueFromDataRow<int?>(row, "FilterID", null);
                    
                    if (filterId.HasValue)
                    {
                        FilterData filterData = FilterFactory.GetFilterData(filterId.Value);

                        if (filterData != null)
                        {
                            AddFilter(filterData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Persist the filter collection data to the database as part of the specified transaction contect.
        /// </summary>
        /// <param name="t">Transaction to participate in when persisting data to the database.</param>
        /// <param name="db"> </param>
        /// <param name="clearMapping"> </param>
        /// <remarks>This method does NOT create a container transaction if no transaction context is passed, meaning all updates to contained filter data objects
        /// will occur one at time.</remarks>
        public void Save(IDbTransaction t, Database db, bool clearMapping)
        {
            if (clearMapping)
                ClearMapping(db, t);

            foreach (FilterData filterDataObject in _filterDataObjects)
            {
                filterDataObject.Save(t);
                MapFilter(t, db, filterDataObject.ID.Value);
            }
            
            /*
            //Update maps
            db.UpdateDataSet(
                _filterData,
                FilterMapTableName,
                GetInsertFilterMapCommand(db),
                null,
                GetDeleteFilterMapCommand(db),
                t);*/
        }

        protected void ClearMapping(Database db, IDbTransaction t)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_ClearMap");
            command.AddInParameter("ParentID", DbType.Int32, ParentID);
            command.AddInParameter("ParentType", DbType.String, ParentType);

            db.ExecuteNonQuery(command, t);
        }

        protected void MapFilter(IDbTransaction t, Database db, int filterId)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_InsertMap");
            command.AddInParameter("FilterID", DbType.Int32, filterId);
            command.AddInParameter("ParentID", DbType.Int32, ParentID);
            command.AddInParameter("ParentType", DbType.String, ParentType);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Get the list of filter data objects contained in this collection.
        /// </summary>
        /// <returns>List of filter data objects.</returns>
        public List<FilterData> GetFilterDataObjects()
        {
            return _filterDataObjects;
        }

        /// <summary>
        /// Get a list of a filter data objects contained in this collection initialized with the specified language.
        /// </summary>
        /// <returns>List of filter data objects.</returns>
        /// <param name="languageCode">Language parameter is used when the ToString() method is filters is called to display a human-readable version of the 
        /// filter objects settings.</param>
        public List<Filter> GetFilters(string languageCode)
        {
            return _filterDataObjects.Select(filterData => filterData.CreateFilter(languageCode)).ToList();
        }

        /// <summary>
        /// Add a filter with the specified ID of the filter collection.
        /// </summary>
        /// <param name="filterId">ID of filter to add to the collection.</param>
        /// <remarks>Filter with specified ID is loaded from the database then associated <see cref="FilterData"/> is added to the internal collection
        /// of <see cref="FilterData"/> objects.</remarks>
        public virtual void AddFilter(int filterId)
        {
            FilterData filter = FilterFactory.GetFilterData(filterId);

            if (filter != null)
            {
                AddFilter(filter);
            }
        }

        /// <summary>
        /// Add a <see cref="FilterData"/> object to the collection.
        /// </summary>
        /// <param name="filter"><see cref="FilterData"/> object to add.</param>
        public virtual void AddFilter(FilterData filter)
        {
            if(_filterDataObjects != null)
            {
                if (_filterDataObjects.Contains(filter))
                {
                    //Remove and re-add the filter to update it
                    _filterDataObjects.Remove(filter);
                    _filterDataObjects.Add(filter);
                }
                else
                {
                    if (filter.ID == null || filter.ID <= 0)
                    {
                        filter.Save();
                    }

                    DataRow filterMapRow = FilterMapTable.NewRow();
                    filterMapRow["FilterID"] = filter.ID;
                    FilterMapTable.Rows.Add(filterMapRow);

                    _filterDataObjects.Add(filter);
                }
            }
        }

        /// <summary>
        /// Delete the filter map
        /// </summary>
        /// <param name="filterID"></param>
        protected virtual void DeleteFilterMap(Int32 filterID)
        {
            DataRow[] rows = FilterMapTable.Select("FilterID = " + filterID, null, DataViewRowState.CurrentRows);

            if (rows.Length > 0)
            {
                rows[0].Delete();
            }
        }

        /// <summary>
        /// Remove the association between a filter and this filter collection.
        /// </summary>
        /// <param name="filterID">ID of filter to remove from the collection.</param>
        /// <remarks>The filter is not removed from the database, but the associations between it and this collection are removed.</remarks>
        public virtual void DeleteFilter(Int32 filterID)
        {
            FilterData filter = FilterFactory.GetFilterData(filterID);
            DeleteFilter(filter);
        }

        /// <summary>
        /// Remove the association between a filter and this filter collection.
        /// </summary>
        /// <param name="filter"><see cref="FilterData"/> object to remove from the collection.</param>
        /// <remarks>The filter is not removed from the database, but the associations between it and this collection are removed.</remarks>
        public virtual void DeleteFilter(FilterData filter)
        {
            if (filter != null)
            {
                DeleteFilterMap(filter.ID.Value);

                if (_filterDataObjects.Contains(filter))
                {
                    _filterDataObjects.Remove(filter);
                }
            }
        }

        /// <summary>
        /// Get the name of the <see cref="DataTable"/> in the collection's configuration <see cref="DataSet"/> that maps filters to their containing collections.
        /// </summary>
        public virtual string FilterMapTableName
        {
            get { return "FilterMap"; }
        }

        /// <summary>
        /// Get a reference to the filter mapping <see cref="DataTable"/> in the collection's configuration <see cref="DataSet"/>.
        /// </summary>
        protected virtual DataTable FilterMapTable
        {
            get
            {
                if (_filterData.Tables.Contains(FilterMapTableName))
                {
                    return _filterData.Tables[FilterMapTableName];
                }
                
                return null;
            }
        }


        /// <summary>
        /// Get a command to insert a filter/parent mapping
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetInsertFilterMapCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_InsertMap");
            command.AddInParameter("FilterID", DbType.Int32, "FilterID", DataRowVersion.Current);
            command.AddInParameter("ParentID", DbType.Int32, ParentID);
            command.AddInParameter("ParentType", DbType.String, ParentType);

            return command;
        }


        /// <summary>
        /// Get a command to delete a filter/parent mapping
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeleteFilterMapCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_DeleteMap");
            command.AddInParameter("FilterID", DbType.Int32, "FilterID", DataRowVersion.Current);
            command.AddInParameter("Parentid", DbType.Int32, ParentID);
            command.AddInParameter("ParentType", DbType.String, ParentType);

            return command;
        }
    }
}