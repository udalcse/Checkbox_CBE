using System;
using System.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Data container for cross tab item configuration.
    /// </summary>
    public class CrossTabItemDataSet : AnalysisItemDataSet
    {

        /// <summary>
        /// Get the name of the table containing item-axis relationships
        /// </summary>
        protected const string AxisItemTableName = "AxisItems";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemTypeName"></param>
        public CrossTabItemDataSet(string itemTypeName)
            : this(itemTypeName, "CrossTabItemData")
        {
        }

        /// <summary>
        /// Constructor for data set.
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <param name="dataTableName"></param>
        public CrossTabItemDataSet(string itemTypeName, string dataTableName)
            : base(itemTypeName, dataTableName, "ItemId", SourceItemsTableName, ResponseTemplatesTableName, AxisItemTableName)
        {
        }

        /// <summary>
        /// Get the table containing axis item data
        /// </summary>
        protected DataTable AxisItemTable
        {
            get { return Tables[AxisItemTableName]; }
        }

        /// <summary>
        /// Initialize data
        /// </summary>
        protected override void InitializeDataTables()
        {
            base.InitializeDataTables();

            DataTable axisItemsTable = new DataTable();
            axisItemsTable.Columns.Add("AnalysisItemID", typeof(Int32));
            axisItemsTable.Columns.Add("ItemID", typeof(Int32));
            axisItemsTable.Columns.Add("Axis", typeof(string));
            axisItemsTable.TableName = AxisItemTableName;

            Tables.Add(axisItemsTable);
        }

        /// <summary>
        /// Get x axis items
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetXAxisItems()
        {
            return AxisItemTable.Select("Axis = 'X'");
        }

        /// <summary>
        /// Get y axis items
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetYAxisItems()
        {
            return AxisItemTable.Select("Axis = 'Y'");
        }
    }
}
