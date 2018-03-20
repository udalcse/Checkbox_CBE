//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Collections.ObjectModel;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information associated with Select1 items.
    /// </summary>
    [Serializable]
    public class Select1Data : SelectItemData
    {
        /// <summary>
        /// Data table name for select 1 configuration data.
        /// </summary>
        public override string ItemDataTableName { get { return "Select1Data"; } }

        /// <summary>
        /// Load procedure name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetSelect1"; } }

        /// <summary>
        /// Create an instance of a Select1 configuration in the persistent store
        /// </summary>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertSelect1");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("AllowOther", DbType.Int32, AllowOther);
            command.AddInParameter("OtherTextID", DbType.String, OtherTextID);
            command.AddInParameter("Randomize", DbType.Int32, Randomize);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Update an instance of a Select1 configuration in the persistent store
        /// </summary>
        protected override void Update(IDbTransaction transaction)
        {
            base.Update(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateSelect1");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("AllowOther", DbType.Int32, AllowOther);
            command.AddInParameter("OtherTextID", DbType.String, OtherTextID);
            command.AddInParameter("Randomize", DbType.Int32, Randomize);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }


        ///// <summary>
        ///// Get a dataset containing configuration for this item data.
        ///// </summary>
        ///// <returns></returns>
        //protected override DataSet GetConcreteConfigurationDataSet()
        //{
        //    if (ID <= 0)
        //    {
        //        throw new ApplicationException("No DataID specified.");
        //    }

        //    try
        //    {
        //        Database db = DatabaseFactory.CreateDatabase();
        //        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetSelect1");
        //        command.AddInParameter("ItemID", DbType.Int32, ID);

        //        DataSet ds = base.GetConcreteConfigurationDataSet();
        //        DataSet selectDataSet = new DataSet();

        //        db.LoadDataSet(command, selectDataSet, ConfigurationDataTableNames);

        //        ds.Merge(selectDataSet);

        //        if (ds.Tables.Contains("ItemOptions"))
        //        {
        //            ds.Tables["ItemOptions"].PrimaryKey = new[] { ds.Tables["ItemOptions"].Columns["OptionID"], ds.Tables["ItemOptions"].Columns["ItemID"] };
        //        }

        //        if (ds.Tables.Contains("ItemLists"))
        //        {
        //            ds.Tables["ItemLists"].PrimaryKey = new[] { ds.Tables["ItemLists"].Columns["ItemID"], ds.Tables["ItemLists"].Columns["ListID"] };
        //        }


        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

        //        if (rethrow)
        //        {
        //            throw;
        //        }

        //        return null;
        //    }
        //}



        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            AllowOther = DbUtility.GetValueFromDataRow(data, "AllowOther", 0) == 1;
            Randomize = DbUtility.GetValueFromDataRow(data, "Randomize", 0) == 1;
            BindinedPropertyId = DbUtility.GetValueFromDataRow<int?>(data, "BindedPropertyId", null);
        }

        /// <summary>
        /// Create an instance of a select1 item based on this configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new Select1();
        }

        /// <summary>
        /// Ensure a max of one option is selected by default
        /// </summary>
        public override ReadOnlyCollection<ListOptionData> Options
        {
            get
            {
                bool selected = false;
                ReadOnlyCollection<ListOptionData> options = base.Options;

                foreach (ListOptionData option in options)
                {
                    if (option.IsDefault)
                    {
                        if (selected)
                        {
                            option.IsDefault = false;
                        }

                        selected = true;
                    }
                }

                return options;
            }
        }


    }
}
