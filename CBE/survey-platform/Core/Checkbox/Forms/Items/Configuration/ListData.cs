using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for list data
    /// </summary>
    [Serializable]
    public class ListData : PersistedDomainObject, ICloneable, IEquatable<ListData>, IEquatable<Int32>
    {
        private readonly Dictionary<int, ListOptionData> _listOptions;
        private readonly List<int> _listOptionIds;

        private bool _imported;

        private readonly List<int> _deletedOptions;

        //Lock object to help with high-load concurrency issue related to null options and duplicate options
        private readonly object _lockObject = new object();

        /// <summary>
        /// Get name of load data procedure
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ListData_Get"; } }

        /// <summary>
        /// Get name of object type
        /// </summary>
        public override string ObjectTypeName { get { return "ListData"; } }

        /// <summary>
        /// List data constructor.  Accepts data table name and identity column name as parameters.
        /// </summary>
        public ListData()
        {
            _imported = false;
            _listOptions = new Dictionary<int, ListOptionData>();
            _listOptionIds = new List<int>();
            _deletedOptions = new List<int>();
        }

        /// <summary>
        /// Load options data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            //Load options
            lock (_lockObject)
            {
                _listOptions.Clear();
                _listOptionIds.Clear();
                _deletedOptions.Clear();

                base.LoadAdditionalData(data);

                DataRow[] optionRows = ((ListDataSet)data).GetOptionRows();

                foreach (DataRow optionRow in optionRows)
                {
                    var optionData = new ListOptionData(optionRow);

                    _listOptionIds.Add(optionData.OptionID);
                    _listOptions[optionData.OptionID] = optionData;
                }
            }
        }

        /// <summary>
        /// Get configuration data for this object
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ListDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Get the ID of the item this list is associated with
        /// </summary>
        public int? ItemID { get; set; }

        /// <summary>
        /// Get the name text id
        /// </summary>
        protected const string TextIdPrefix = "listData";

        /// <summary>
        /// Get the option at the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public ListOptionData GetOptionAt(Int32 position)
        {
            if (_listOptions.Count >= position)
            {
                return _listOptions.ElementAt(position - 1).Value;
            }

            return null;
        }

        /// <summary>
        /// Get the list of options for the list
        /// </summary>
        public ReadOnlyCollection<ListOptionData> ListOptions
        {
            get
            {
                lock (_lockObject)
                {
                    var optionList = (from optionId in _listOptionIds
                                      where _listOptions.ContainsKey(optionId)
                                      select _listOptions[optionId]).ToList();

                    return new ReadOnlyCollection<ListOptionData>(optionList);
                }
            }
        }

        /// <summary>
        /// Remove an option from the list
        /// </summary>
        public void RemoveOption(Int32 optionID)
        {
            lock (_lockObject)
            {
                if (!_deletedOptions.Contains(optionID))
                {
                    _deletedOptions.Add(optionID);
                }

                if (_listOptionIds.Contains(optionID))
                {
                    _listOptionIds.Remove(optionID);
                }

                if (_listOptions.ContainsKey(optionID))
                {
                    _listOptions.Remove(optionID);
                }
            }
        }

        /// <summary>
        /// Add a list option
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="category"></param>
        /// <param name="isDefault"></param>
        /// <param name="isOther"></param>
        /// <param name="itemID"></param>
        /// <param name="points"></param>
        /// <param name="position"></param>
        /// <param name="isNoneOfAbove"></param>
        /// <param name="contentID"></param>
        /// <param name="optionID">Can be set a real option ID when we perform option cloning</param>
        /// <param name="temporaryGuid"> </param>
        public ListOptionData AddOption(Int32 itemID, string alias, string category, bool isDefault, Int32 position, double points, bool isOther, bool isNoneOfAbove, Int32? contentID, int optionID = -1, Guid? temporaryGuid = null)
        {
            lock (_lockObject)
            {
                if (optionID == -1)
                {
                    //We use negative ids to indicate new options, so find a negative id that does not already
                    // exist in the list.
                    optionID = -1000;

                    if (_listOptionIds.Count > 0)
                    {
                        optionID = Math.Min(_listOptionIds.Min(), optionID);
                        optionID--;
                    }
                }

                var optionData = new ListOptionData
                {
                    OptionID = optionID,
                    Alias = alias,
                    Category = category,
                    IsDefault = isDefault,
                    Position = position,
                    Points = points,
                    IsOther = isOther,
                    IsNoneOfAbove = isNoneOfAbove,
                    ContentID = contentID,
                };

                //Now, add option at correct position in list.  Option positions are 1-based but
                // option list is a 0-index based list, so be sure to account for that.  Also make
                // sure position is valid, so we don't have a list of 5 options w/nonconsecutive
                // positions.
                if (position >= _listOptionIds.Count)
                {
                    _listOptionIds.Add(optionID);
                }
                else
                {
                    _listOptionIds.Insert(position - 1, optionID);
                }

                //Add to options collection
                _listOptions[optionID] = optionData;

                return optionData;
            }
        }

        /// <summary>
        /// Update an option
        /// </summary>
        /// <param name="optionID"></param>
        /// <param name="itemID"></param>
        /// <param name="alias"></param>
        /// <param name="category"></param>
        /// <param name="isDefault"></param>
        /// <param name="position"></param>
        /// <param name="points"></param>
        /// <param name="isOther"></param>
        /// <param name="isNoneOfAbove"></param>
        /// <param name="contentID"></param>
        /// <param name="temporaryGuid"> </param>
        public void UpdateOption(Int32 optionID, Int32 itemID, string alias, string category, bool isDefault, Int32 position, double points, bool isOther, bool isNoneOfAbove, Int32? contentID, Guid? temporaryGuid = null)
        {
            //Find the option and update it
            lock (_lockObject)
            {
                ListOptionData optionData = _listOptions.FirstOrDefault(o => o.Key == optionID).Value;

                optionData.Alias = alias;
                optionData.Category = category;
                optionData.IsDefault = isDefault;
                optionData.Position = position;
                optionData.Points = points;
                optionData.IsOther = isOther;
                optionData.IsNoneOfAbove = isNoneOfAbove;
                optionData.ContentID = contentID;

                //Remove value from list
                _listOptionIds.Remove(optionData.OptionID);

                //Re-insert at correct position  Positions are 1-based, so
                // that must be taken into account when inserting into list.
                if (position > _listOptionIds.Count)
                {
                    _listOptionIds.Add(optionData.OptionID);
                }
                else if (position > 0)
                {
                    _listOptionIds.Insert(position - 1, optionData.OptionID);
                }

                optionData.OptionID = optionID;
            }
        }

        /// <summary>
        /// Delete the list
        /// </summary>
        /// <param name="t"></param>
        public override void Delete(IDbTransaction t)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Create the list item data.
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            //Create the list, does not do too much for now other than assign an id
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ListData_Create");
            command.AddOutParameter("ListID", DbType.Int32, 4);
            db.ExecuteNonQuery(command, transaction);

            object id = command.GetParameterValue("ListID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to create item options data.");
            }

            ID = (int)id;

            //Now insert options
            UpsertOptions(transaction);

            //Clear collections
            _deletedOptions.Clear();
        }

        /// <summary>
        /// Update the item data
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Update(IDbTransaction transaction)
        {
            //TODO: Set updated flag on list data
            //No update is actually done to list data table

            //Remove deleted options
            DeleteRemovedOptions(transaction);

            //Update current options
            UpsertOptions(transaction);

            //Clear collections
            _deletedOptions.Clear();
        }

        public void InsertOption(IDbTransaction transaction, ListOptionData optionData)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            int oldID = optionData.OptionID;

            command = db.GetStoredProcCommandWrapper("ckbx_sp_ListData_InsertOption");
            command.AddInParameter("ListID", DbType.Int32, ID);
            command.AddInParameter("ItemID", DbType.Int32, ItemID);

            //Add common parameters
            command.AddInParameter("Alias", DbType.String, optionData.Alias);
            command.AddInParameter("Category", DbType.String, optionData.Category);
            command.AddInParameter("IsDefault", DbType.Boolean, optionData.IsDefault);

            //Use position in list as position, but add 1 since positions are 1-based and list
            // indexes are 0-based.
            command.AddInParameter("Position", DbType.Int32, optionData.Position);

            command.AddInParameter("IsOther", DbType.Boolean, optionData.IsOther);
            command.AddInParameter("IsNoneOfAbove", DbType.Boolean, optionData.IsNoneOfAbove);
            command.AddInParameter("Points", DbType.Double, optionData.Points);
            command.AddInParameter("ContentID", DbType.Int32, optionData.ContentID);

            //For new options, retrieve updated database id);
            command.AddOutParameter("OptionId", DbType.Int32, 4);

            //Execute command
            db.ExecuteNonQuery(command, transaction);

            //Otherwise, try to get the new option's id and update internal
            // collections.
            int newOptionID = (int)command.GetParameterValue("OptionId");

            lock (_lockObject)
            {
                //Update option id
                optionData.OptionID = newOptionID;

                //Add to dictionary w/new id
                _listOptions[newOptionID] = optionData;

                //Remove old association
                _listOptions.Remove(oldID);

                _listOptionIds.Remove(oldID);
                _listOptionIds.Add(newOptionID);
            }
        }

        /// <summary>
        /// Insert new options
        /// </summary>
        private void UpsertOptions(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            //Now update options.  Iterate through list option ids collection
            // which has options in correct order.
            var updatedOptionIdList = new List<int>();

            for (int i = 0; i < _listOptionIds.Count; i++)
            {
                int optionId = _listOptionIds[i];

                if (_listOptions.ContainsKey(optionId))
                {
                    ListOptionData optionData = _listOptions[optionId];

                    DBCommandWrapper command;

                    //Use different sprocs, depending on insert/update
                    if (optionData.OptionID > 0)
                    {
                        command = db.GetStoredProcCommandWrapper("ckbx_sp_ListData_UpdateOption");
                        command.AddInParameter("OptionId", DbType.Int32, optionData.OptionID);
                    }
                    else
                    {
                        command = db.GetStoredProcCommandWrapper("ckbx_sp_ListData_InsertOption");
                        command.AddInParameter("ListID", DbType.Int32, ID);
                        command.AddInParameter("ItemID", DbType.Int32, ItemID);
                    }

                    //Add common parameters
                    command.AddInParameter("Alias", DbType.String, optionData.Alias);
                    command.AddInParameter("Category", DbType.String, optionData.Category);
                    command.AddInParameter("IsDefault", DbType.Boolean, optionData.IsDefault);

                    //Use position in list as position, but add 1 since positions are 1-based and list
                    // indexes are 0-based.
                    command.AddInParameter("Position", DbType.Int32, i + 1);

                    command.AddInParameter("IsOther", DbType.Boolean, optionData.IsOther);
                    command.AddInParameter("IsNoneOfAbove", DbType.Boolean, optionData.IsNoneOfAbove);
                    command.AddInParameter("Points", DbType.Double, optionData.Points);
                    command.AddInParameter("ContentID", DbType.Int32, optionData.ContentID);

                    //For new options, retrieve updated database id);
                    if (optionData.OptionID < 0)
                    {
                        command.AddOutParameter("OptionId", DbType.Int32, 4);
                    }

                    //Execute command
                    db.ExecuteNonQuery(command, transaction);

                    //If option was not new, simply move on to the next one.
                    if (optionData.OptionID > 0)
                    {
                        updatedOptionIdList.Add(optionData.OptionID);
                        continue;
                    }

                    //Otherwise, try to get the new option's id and update internal
                    // collections.
                    object newOptionValue = command.GetParameterValue("OptionId");

                    if (newOptionValue != null && newOptionValue != DBNull.Value)
                    {
                        var newOptionId = (int)newOptionValue;

                        //Update option id
                        optionData.OptionID = newOptionId;

                        //Add to dictionary w/new id
                        _listOptions[newOptionId] = optionData;

                        //Remove old association
                        _listOptions.Remove(optionId);

                        //Add to list of new option ids
                        updatedOptionIdList.Add(newOptionId);
                    }
                }
            }

            //Now replace list of optiond ids w/list of updated option ids
            _listOptionIds.Clear();
            _listOptionIds.AddRange(updatedOptionIdList);
        }

        /// <summary>
        /// Delete removed options
        /// </summary>
        private void DeleteRemovedOptions(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (int optionId in _deletedOptions)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ListData_DeleteOption");
                command.AddInParameter("OptionID", DbType.Int32, optionId);

                db.ExecuteNonQuery(command, transaction);
            }
        }

        ///// <summary>
        ///// Save option texts
        ///// </summary>
        //protected virtual void SaveOptionTextTable()
        //{
        //    if (_data.Tables.Contains(TextTableName))
        //    {
        //        DataRow[] rows = _data.Tables[TextTableName].Select(null, null, DataViewRowState.CurrentRows);

        //        foreach (DataRow row in rows)
        //        {
        //            if (row["LanguageCode"] != DBNull.Value && row["TextValue"] != DBNull.Value && row["ComputedTextID"] != DBNull.Value)
        //            {
        //                string languageCode = (string)row["LanguageCode"];
        //                string text = (string)row["TextValue"];
        //                string textID = (string)row["ComputedTextID"];
        //                TextManager.SetText(textID, languageCode, text);
        //            }
        //        }
        //    }
        //}

        #region ICloneable Members

        /// <summary>
        /// Clone the option list.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = new ListData();
            clone.ID = ID;

            foreach (ListOptionData option in ListOptions)
            {
                ListOptionData optionData = clone.AddOption(-1, option.Alias, option.Category, option.IsDefault, option.Position, option.Points, option.IsOther, option.IsNoneOfAbove, option.ContentID, option.OptionID);
            }

            return clone;
        }

        #endregion


        #region IEquatable<ListData> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ListData other)
        {
            if (other.ID == ID)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region IEquatable<int> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(int other)
        {
            if (other == ID)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
