using System;
using System.Data;
using Checkbox.Forms.Items;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreItemData : AnalysisItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "NetPromoterScoreItemData"; }
        }

        /// <summary>
        /// Get sproc name to load item
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetNetPromoterScoreTable"; }
        }

        /// <summary>
        /// Create an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertNetPromoterScoreTable");

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
            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateNetPromoterScoreTable");

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
            return new NetPromoterScoreItem();
        }
    }
}
