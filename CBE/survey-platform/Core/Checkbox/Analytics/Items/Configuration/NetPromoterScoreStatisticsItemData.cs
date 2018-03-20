using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreStatisticsItemData : AnalysisItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "NetPromoterScoreStatisticsItemData"; }
        }

        /// <summary>
        /// Get sproc name to load item
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetNetPromoterScoreStatisticsTable"; }
        }

        /// <summary>
        /// Create an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertNetPromoterScoreStatisticsTable");

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
            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateNetPromoterScoreStatisticsTable");

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
            return new NetPromoterScoreStatisticsItem();
        }
    }
}
