using System;
using System.Data;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Data container for analysis items
    /// </summary>
    public class AnalysisItemDataSet : ItemDataSet
    {
        public const string SourceItemsTableName = "SourceItems";
        public const string ResponseTemplatesTableName = "ResponseTemplates";

        /// <summary>
        /// Dataset contstructore
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <param name="dataTableName"></param>
        public AnalysisItemDataSet(string itemTypeName, string dataTableName)
            : this(itemTypeName, dataTableName, string.Empty, SourceItemsTableName, ResponseTemplatesTableName)
        {
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        /// <param name="dataTableName"></param>
        /// <param name="identityColumnName"></param>
        /// <param name="additionalTableNames"></param>
        protected AnalysisItemDataSet(string owningObjectTypeName, string dataTableName, string identityColumnName, params string[] additionalTableNames)
            : base(owningObjectTypeName, dataTableName, identityColumnName, additionalTableNames)
        {
        }

        /// <summary>
        /// Identity column of analysis template.
        /// </summary>
        public override string IdentityColumnName { get { return "ItemId"; } }

        /// <summary>
        /// Identity column of analysis template.
        /// </summary>
        public string AnalysisItemIdentityColumnName { get { return "AnalysisItemId"; } }

        /// <summary>
        /// Get the table containing source items for this analysis item
        /// </summary>
        protected DataTable SourceItemsTable
        {
            get { return Tables[SourceItemsTableName]; }
        }

        /// <summary>
        /// Get the table containing response template data
        /// </summary>
        protected DataTable ResponseTemplatesTable
        {
            get { return Tables[ResponseTemplatesTableName]; }
        }

        /// <summary>
        /// Initialize data store
        /// </summary>
        protected override void InitializeDataTables()
        {
            base.InitializeDataTables();

            DataTable sourceItems = new DataTable();
            sourceItems.Columns.Add("AnalysisItemID", typeof(Int32));
            sourceItems.Columns.Add("SourceItemID", typeof(Int32));
            sourceItems.TableName = SourceItemsTableName;

            DataTable responseTemplates = new DataTable();
            responseTemplates.Columns.Add("AnalysisItemID", typeof(Int32));
            responseTemplates.Columns.Add("ResponseTemplateID", typeof(Int32));
            responseTemplates.TableName = ResponseTemplatesTableName;

            Tables.Add(sourceItems);
            Tables.Add(responseTemplates);
        }

        /// <summary>
        /// Get data rows for source items in order of position
        /// </summary>
        public DataRow[] GetSourceItems()
        {
            return SourceItemsTable.Select();
        }

        /// <summary>
        /// Get dta rows for templates
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetResponseTemplates()
        {
            return ResponseTemplatesTable.Select();
        }
    }
}
