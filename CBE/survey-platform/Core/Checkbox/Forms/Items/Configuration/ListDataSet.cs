using System;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data container for lists of options.
    /// </summary>
    public class ListDataSet : LocalizedPersistedDomainObjectDataSet
    {
        /// <summary>
        /// Get the name of the text table
        /// </summary>
        private const string TextTableName = "OptionTexts"; 

        /// <summary>
        /// Get the name of the table containing item options
        /// </summary>
        private const string OptionsTableName = "ItemOptions";

        /// <summary>
        /// 
        /// </summary>
        public override string  ParentDataTableName{get{return string.Empty;}}

        /// <summary>
        /// Get data for abstract ("List") data.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="owningObjectId"></param>
        /// <returns></returns>
        protected override DBCommandWrapper CreateAbstractDataCommand(Database db, int owningObjectId)
        {
            //TODO: Add item list mapping?
            return null;
        }

        ///<summary>
        ///</summary>
        ///<param name="objectTypeName"></param>
        public ListDataSet(string objectTypeName)
            : base(objectTypeName, "ListData", "ListId", OptionsTableName)
        {
        }

        /// <summary>
        /// Init data
        /// </summary>
        protected override void  InitializeDataTables()
        {
            var dt = new DataTable { TableName = OptionsTableName };

            dt.Columns.Add("OptionID", typeof(Int32));
            dt.Columns.Add("ItemID", typeof(Int32));
            dt.Columns.Add("TextID", typeof(string));
            dt.Columns.Add("Alias", typeof(string));
            dt.Columns.Add("IsDefault", typeof(bool));
            dt.Columns.Add("Position", typeof(Int32));
            dt.Columns.Add("IsOther", typeof(bool));
            dt.Columns.Add("IsNoneOfAbove", typeof(bool));
            dt.Columns.Add("Points", typeof(double));
            dt.Columns.Add("Deleted", typeof(bool));
            dt.Columns.Add("ListID", typeof(Int32));
            dt.Columns.Add("ImageID", typeof(Int32));
            dt.Columns.Add("OptionText", typeof(string)); //Used only for editing, not persisted by data

            DataColumn[] pkColumns = { dt.Columns["OptionID"] };
            dt.Columns["OptionID"].AutoIncrement = true;
            dt.Columns["OptionID"].AutoIncrementSeed = -1;
            dt.Columns["OptionID"].AutoIncrementStep = -1;

            dt.PrimaryKey = pkColumns;

            Tables.Add(dt);
        }

        /// <summary>
        /// Initialize text tables
        /// </summary>
        protected override void InitializeTextTables()
        {
            base.InitializeTextTables();

            var textData = new DataTable { TableName = TextTableName };
            textData.Columns.Add("OptionID", typeof(Int32));
            textData.Columns.Add("TextIDSuffix", typeof(string));
            textData.Columns.Add("ComputedTextID", typeof(string));
            textData.Columns.Add("TextID", typeof(string));
            textData.Columns.Add("LanguageCode", typeof(string));
            textData.Columns.Add("TextValue", typeof(string));
            textData.Columns["ComputedTextID"].Expression = "Iif(OptionID IS NULL OR TextIDSuffix IS NULL, '', '/listOption/' + OptionID + '/' + TextIDSuffix)";

            Tables.Add(textData);
        }

        /// <summary>
        /// Get data rows for item options
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetOptionRows()
        {
            return Tables[OptionsTableName].Select(null, "Position ASC");
        }

        /// <summary>
        /// Get data rows for item options
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetOptionRows(int optionPosition)
        {
            return Tables[OptionsTableName].Select("Position = " + optionPosition);
        }
    }
}
