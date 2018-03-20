using System;
using System.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data container for matrix item data
    /// </summary>
    public class MatrixDataSet : ItemDataSet
    {
        private const string ItemPositionsTableName = "ItemPositions";
        private const string ColumnTypesTableName = "ColumnTypes";
        private const string RowDataTableName = "RowData";
        private const string ColumnProtoypeItemTableName = "Items";

        /// <summary>
        /// Construct new item data set
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <param name="dataTableName"></param>
        public MatrixDataSet(string itemTypeName, string dataTableName)
            : base(itemTypeName, dataTableName, "ItemId", ItemPositionsTableName, ColumnTypesTableName, RowDataTableName, ColumnProtoypeItemTableName)
        {
        }

        /// <summary>
        /// Initialize data tables for data set
        /// </summary>
        protected override void InitializeDataTables()
        {
            base.InitializeDataTables();

            DataTable itemPositionMappings = new DataTable { TableName = ItemPositionsTableName };
            itemPositionMappings.Columns.Add("MatrixID", typeof(Int32));
            itemPositionMappings.Columns.Add("Row", typeof(Int32));
            itemPositionMappings.Columns.Add("Column", typeof(Int32));
            itemPositionMappings.Columns.Add("ItemID", typeof(Int32));
            itemPositionMappings.Columns.Add("ItemTypeName", typeof(string));

            DataTable columnTypes = new DataTable { TableName = ColumnTypesTableName };
            columnTypes.Columns.Add("MatrixID", typeof(Int32));
            columnTypes.Columns.Add("Column", typeof(Int32));
            columnTypes.Columns.Add("ColumnPrototypeID", typeof(Int32));
            columnTypes.Columns.Add("UniqueAnswers", typeof(bool));
            columnTypes.Columns.Add("Width", typeof(Int32));
            columnTypes.Columns.Add("ItemTypeName", typeof(string));

            DataTable rowData = new DataTable { TableName = RowDataTableName };
            rowData.Columns.Add("MatrixID", typeof(Int32));
            rowData.Columns.Add("Row", typeof(Int32));
            rowData.Columns.Add("Alias", typeof(string));
            rowData.Columns.Add("IsSubHeading", typeof(bool));
            rowData.Columns.Add("IsOther", typeof(bool));

            Tables.Add(itemPositionMappings);
            Tables.Add(columnTypes);
            Tables.Add(rowData);
        }
        
        /// <summary>
        /// Get data row for child positions
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetChildPositions()
        {
            return Tables[ItemPositionsTableName].Select();
        }

        /// <summary>
        /// Get data row for child positions sorted in ascending order by row position.
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetRowData()
        {
            return Tables[RowDataTableName].Select(null, "[Row] ASC");
        }

        /// <summary>
        /// Get data for matrix columns sorted in ascending order by column position.
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetColumnData()
        {
            return Tables[ColumnTypesTableName].Select(null, "[Column] ASC");
        }
    }
}
