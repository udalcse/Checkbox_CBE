using System;
using System.Data;
using Checkbox.Forms.Items;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// GovernancePriorityGraphData
    /// </summary>
    /// <seealso cref="Checkbox.Analytics.Items.Configuration.AnalysisItemData" />
    public class GovernancePriorityGraphData :  AnalysisItemData
    {
        /// <summary>
        /// Get the data table name
        /// </summary>
        public override string ItemDataTableName => "GovernancePriorityItemData";

        /// <summary>
        /// Get name of load data stored procedure
        /// </summary>
        protected override string LoadSprocName => "ckbx_sp_ItemData_GetGovernancePrioritySummary";

        /// <summary>
        /// Creates the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <exception cref="System.Exception">Unable to save analysis item.  DataID is <= 0.</exception>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertGovernancePrioritySummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            db.ExecuteNonQuery(command, transaction);

            UpdateSourceItemTables(transaction);
        }

        /// <summary>
        /// Update the average score item data
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <exception cref="System.Exception">Unable to save analysis item.  DataID is <= 0.</exception>
        protected override void Update(IDbTransaction transaction)
        {
            base.Update(transaction);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateGovernancePrioritySummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, transaction);

            UpdateSourceItemTables(transaction);
        }


        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new GovernancePrioritySummaryItem();
        }
    }
}
