using System;
using System.Data;

using Checkbox.Forms.Items;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Item data for total score item
    /// </summary>
    [Serializable]
    public class TotalScoreItemData : AnalysisItemData
    {
        /// <summary>
        /// Data table name
        /// </summary>
        public override string ItemDataTableName { get { return "TotalScoreItemData"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetTotalScore"; } }

        /// <summary>
        /// Insert an instance of a total score item into the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertTotalScore");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update an instance of a total score item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateTotalScore");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create an instance of a total score item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new TotalScoreItem();
        }
    }
}
