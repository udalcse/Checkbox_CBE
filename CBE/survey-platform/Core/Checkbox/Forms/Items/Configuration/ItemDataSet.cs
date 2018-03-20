using System;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Base data set for item data class
    /// </summary>
    public class ItemDataSet : LocalizedPersistedDomainObjectDataSet
    {
        /// <summary>
        /// Construct new item data set
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <param name="dataTableName"></param>
        public ItemDataSet(string itemTypeName, string dataTableName)
            : this(itemTypeName, dataTableName, "ItemId")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        /// <param name="dataTableName"></param>
        /// <param name="identityColumnName"></param>
        /// <param name="additionalTableNames"></param>
        public ItemDataSet(string owningObjectTypeName, string dataTableName, string identityColumnName, params string[] additionalTableNames)
            : base(owningObjectTypeName, dataTableName, identityColumnName, additionalTableNames)
        {
        }

        /// <summary>
        /// Parent data table name
        /// </summary>
        public override string ParentDataTableName { get { return "Items"; } }

        /// <summary>
        /// Get the datarow for the parent data
        /// </summary>
        /// <returns></returns>
        public DataRow GetParentDataRow()
        {
            if (Tables.Contains(ParentDataTableName))
            {
                DataRow[] parentRows = Tables[ParentDataTableName].Select(string.Format("{0} = {1}", IdentityColumnName, OwningObjectId));

                if (parentRows.Length > 0)
                {
                    return parentRows[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Command for getting abstract item data
        /// </summary>
        /// <param name="db"></param>
        /// <param name="owningObjectId"></param>
        /// <returns></returns>
        protected override DBCommandWrapper CreateAbstractDataCommand(Database db, int owningObjectId)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_GetItem");
            command.AddInParameter(IdentityColumnName, DbType.Int32, owningObjectId);

            return command;
        }

        /// <summary>
        /// Get the name of the text table
        /// </summary>
        public virtual string TextTableName
        {
            get { return "ItemTexts_" + DataTableName; }
        }

        /// <summary>
        /// Create text tables
        /// </summary>
        protected override void InitializeTextTables()
        {
            base.InitializeTextTables();

            DataTable textData = new DataTable { TableName = TextTableName };
            textData.Columns.Add("ItemID", typeof(Int32));
            textData.Columns.Add("TextIDSuffix", typeof(string));
            textData.Columns.Add("TextIDPrefix", typeof(string));
            textData.Columns.Add("ComputedTextID", typeof(string));
            textData.Columns.Add("TextID", typeof(string));
            textData.Columns.Add("ReportableSectionBreak", typeof(bool));
            textData.Columns.Add("LanguageCode", typeof(string));
            textData.Columns.Add("TextValue", typeof(string));
            textData.Columns["ComputedTextID"].Expression = "Iif(ItemID IS NULL OR TextIDSuffix IS NULL OR TextIDPrefix IS NULL, '', '/' + TextIDPrefix + '/' + ItemID + '/' + TextIDSuffix)";

            Tables.Add(textData);
        }
    }
}
