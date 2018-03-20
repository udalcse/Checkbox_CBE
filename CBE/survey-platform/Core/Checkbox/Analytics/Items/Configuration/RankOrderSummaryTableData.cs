using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Forms.Items;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Configuration data object for rank order summary analysis item. 
    /// It shows the sum of points across all responses for each answer.
    /// </summary>
    [Serializable]
    public class RankOrderSummaryTableData : AnalysisItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "RankOrderSummaryTableData"; }
        }

        /// <summary>
        /// Get sproc name to load item
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetRankOrderSummaryTable"; }
        }


        /// <summary>
        /// Get/set option calculation point
        /// </summary>
        public StatisticsItemReportingOption ReportOption { get; set; }

        /// <summary>
        /// Create an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertRankOrderSummaryTable");

            commandWrapper.AddInParameter("ItemID", DbType.Int32, ID);
            commandWrapper.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(commandWrapper, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateRankOrderSummaryTable");

            commandWrapper.AddInParameter("ItemID", DbType.Int32, ID);
            commandWrapper.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(commandWrapper, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new RankOrderSummary();
        }
    }
}
