using Checkbox.Forms.Items;
using Prezza.Framework.Data;
using System;
using System.Data;
using System.Collections.Generic;
using Checkbox.Forms;

namespace Checkbox.Analytics.Items.Configuration
{
    [Serializable]
    public class HeatMapData : AnalysisItemData
    {
        public override string ItemDataTableName { get { return "HeatMapItemData"; } }

        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetHeatMapSummary"; } }

        public int? PrimarySourceItemID { get; set; }

        public bool UseMeanValues { get; set; }

        public bool RandomizeResponses { get; set; }
        
        public Dictionary<int, double> SigmaValues { get; set; }

        protected override void LoadBaseObjectData(DataRow data)
        {
            if (data != null)
            {
                UseMeanValues = DbUtility.GetValueFromDataRow(data, "UseMeanValues", false);
                RandomizeResponses = DbUtility.GetValueFromDataRow(data, "RandomizeResponses", true);
            }

            base.LoadBaseObjectData(data);
        }

        protected override void LoadAdditionalData(Common.PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);
            //after loading source item we must check that PrimarySourceItem is among them
            if (PrimarySourceItemID.HasValue && !SourceItemIds.Contains(PrimarySourceItemID.Value))
                PrimarySourceItemID = null;

            SigmaValues = ResponseTemplateManager.GetResponseTemplateSectionsESigma(ID.Value, false);
        }

        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            //Save item data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertHeatMapSummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            command.AddInParameter("UseMeanValues", DbType.Boolean, true);
            command.AddInParameter("RandomizeResponses", DbType.Boolean, true);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        protected override Item CreateItem()
        {
            return new HeatMapItem();
        }

        /// <summary>
        /// Update the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateHeatMapSummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            command.AddInParameter("UseMeanValues", DbType.Boolean, UseMeanValues);
            command.AddInParameter("RandomizeResponses", DbType.Boolean, RandomizeResponses);

            db.ExecuteNonQuery(command, t);

            InsertHeatMapESigma(db, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Insert heatmap eSigma values
        /// </summary>
        /// <param name="t"></param>
        protected void InsertHeatMapESigma(Database db, IDbTransaction t)
        {
            foreach (var item in SigmaValues)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertHeatMap_eSigma");
                command.AddInParameter("ReportId", DbType.Int32, ID);
                command.AddInParameter("SectionId", DbType.Int32, item.Key);
                command.AddInParameter("eSigmaValue", DbType.Double, item.Value.ToString("0.##"));

                db.ExecuteNonQuery(command, t);
            }
            
        }

    }
}
