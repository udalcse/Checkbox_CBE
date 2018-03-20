using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Common;

namespace Checkbox.Forms
{
    /// <summary>
    /// Data container for raw data associated with a template.
    /// </summary>
    public class TemplateDataSet : LocalizedPersistedDomainObjectDataSet
    {
        #region Utility Accessors

        /// <summary>
        /// Return pages table name
        /// </summary>
        protected const string PageDataTableName = "Pages";

        /// <summary>
        /// Return template data table name
        /// </summary>
        public override string DataTableName { get { return "TemplateData"; } }

        /// <summary>
        /// Get identity column name for template data
        /// </summary>
        public override string IdentityColumnName { get { return "TemplateId"; } }

        /// <summary>
        /// Get the name of the table containing page items
        /// </summary>
        protected const string PageItemsTableName = "PageItems";

        /// <summary>
        /// Get the name of the template items table
        /// </summary>
        protected const string TemplateItemsTableName = "Items"; 

        /// <summary>
        /// Get the name of the item appearances table
        /// </summary>
        protected const string ItemAppearanceTableName = "AppearanceData";

        /// <summary>
        /// Get the name of the table containing item appearances
        /// </summary>
        protected const string ItemAppearanceMapTableName = "ItemAppearanceMap";
        
        /// <summary>
        /// Get the table containing item/appearance mappings
        /// </summary>
        public DataTable ItemAppearanceMapTable { get { return Tables[ItemAppearanceMapTableName]; } }

        /// <summary>
        /// Item appearance table
        /// </summary>
        public DataTable ItemAppearanceTable { get { return Tables[ItemAppearanceTableName]; } }

        /// <summary>
        /// Get the table containing page data
        /// </summary>
        public DataTable PageDataTable { get { return Tables[PageDataTableName]; } }

        /// <summary>
        /// Get the page items table
        /// </summary>
        public DataTable PageItemsTable { get { return Tables[PageItemsTableName]; } }
        /// <summary>
        /// Get the template items table
        /// </summary>
        public DataTable TemplateItemsTable { get { return Tables[TemplateItemsTableName]; } }

        public override string ParentDataTableName { get { return string.Empty; } }

        protected override Prezza.Framework.Data.DBCommandWrapper CreateAbstractDataCommand(Prezza.Framework.Data.Database db, int owningObjectId)
        {
            return null;
        }

        #endregion

     

        ///<summary>
        ///</summary>
        ///<param name="owningObjectTypeName"></param>
        public TemplateDataSet(string owningObjectTypeName)
            : base(owningObjectTypeName)
        {
        }

        /// <summary>
        /// Initialize the template data
        /// </summary>
        protected override void InitializeDataTables()
        {
            InitializeTemplateData();
            InitializeItemData();
            InitializePageData();
        }

        /// <summary>
        /// Initialize the template data table
        /// </summary>
        protected virtual void InitializeTemplateData()
        {
            var templateDataTable = new DataTable { TableName = DataTableName };
            templateDataTable.Columns.Add(IdentityColumnName, typeof(Int32));
            templateDataTable.Columns.Add("ModifiedDate", typeof(DateTime));
            templateDataTable.Columns.Add("Deleted", typeof(bool));
            templateDataTable.Columns.Add("DefaultPolicy", typeof(Int32));
            templateDataTable.Columns.Add("AclID", typeof(Int32));
            templateDataTable.Columns.Add("CreatedDate", typeof(DateTime));
            templateDataTable.Columns.Add("CreatedBy", typeof(string));

            Tables.Add(templateDataTable);
        }

        /// <summary>
        /// Initialize page data
        /// </summary>
        protected virtual void InitializePageData()
        {
            //Page Data Table
            //Note: Auto-Increment and primary key are defined in CreatePrimaryKeys
            var pageDataTable = new DataTable();
            pageDataTable.Columns.Add(IdentityColumnName, typeof(Int32));
            pageDataTable.Columns.Add("PageID", typeof(Int32));
            pageDataTable.Columns.Add("PagePosition", typeof(Int32));
            pageDataTable.Columns.Add("RandomizeItems", typeof(Int32));
            pageDataTable.Columns.Add("LayoutTemplateID", typeof(Int32));
            pageDataTable.TableName = PageDataTableName;

            //Page Items Table
            var pageItemsTable = new DataTable();
            pageItemsTable.Columns.Add("PageID", typeof(Int32));
            pageItemsTable.Columns.Add("ItemID", typeof(Int32));
            pageItemsTable.Columns.Add("Position", typeof(Int32));
            pageItemsTable.TableName = PageItemsTableName;

            Tables.Add(pageDataTable);
            Tables.Add(pageItemsTable);
        }

        /// <summary>
        /// Initialize item data tables
        /// </summary>
        protected virtual void InitializeItemData()
        {
            var itemTable = new DataTable();
            itemTable.Columns.Add(IdentityColumnName, typeof(Int32));
            itemTable.Columns.Add("ItemID", typeof(Int32));
            itemTable.Columns.Add("ItemDataClassName", typeof(string));
            itemTable.Columns.Add("ItemDataAssemblyName", typeof(string));
            itemTable.Columns.Add("ItemName", typeof(string));
            itemTable.Columns.Add("ItemTypeID", typeof(Int32));
            itemTable.TableName = TemplateItemsTableName;

            var itemAppearanceMap = new DataTable();
            itemAppearanceMap.Columns.Add("ItemID", typeof(Int32));
            itemAppearanceMap.Columns.Add("AppearanceID", typeof(Int32));
            itemAppearanceMap.TableName = ItemAppearanceMapTableName;

            var itemAppearanceTable = new DataTable();
            itemAppearanceTable.Columns.Add("AppearanceID", typeof(Int32));
            itemAppearanceTable.Columns.Add("AppearanceCode", typeof(string));
            itemAppearanceTable.Columns.Add("LayoutStyle", typeof(string));
            itemAppearanceTable.Columns.Add("Columns", typeof(Int32));
            itemAppearanceTable.Columns.Add("Width", typeof(Int32));
            itemAppearanceTable.Columns.Add("Height", typeof(Int32));
            itemAppearanceTable.Columns.Add("ShowNumberLabels", typeof(bool));
            itemAppearanceTable.Columns.Add("FontColor", typeof(string));
            itemAppearanceTable.Columns.Add("FontSize", typeof(string));
            itemAppearanceTable.Columns.Add("ItemPosition", typeof(string));
            itemAppearanceTable.Columns.Add("Rows", typeof(Int32));
            itemAppearanceTable.Columns.Add("LabelPosition", typeof(string));
            itemAppearanceTable.Columns.Add("UseAliases", typeof(bool));
            itemAppearanceTable.Columns.Add("GraphType", typeof(string));
            itemAppearanceTable.TableName = ItemAppearanceTableName;

            Tables.Add(itemTable);
            Tables.Add(itemAppearanceTable);
            Tables.Add(itemAppearanceMap);
        }

        /// <summary>
        /// Create primary keys for the data set
        /// </summary>
        /// <param name="templateData"></param>
        public virtual void CreatePrimaryKeys(DataSet templateData)
        {
            //Create pk for page data
            if (PageDataTable != null
                && PageDataTable.Columns.Contains("PageID")
                && PageDataTable.PrimaryKey.Length == 0)
            {
                PageDataTable.PrimaryKey = new[] { PageDataTable.Columns["PageID"] };

                PageDataTable.Columns["PageID"].AutoIncrement = true;
                PageDataTable.Columns["PageID"].AutoIncrementSeed = -1;
                PageDataTable.Columns["PageID"].AutoIncrementStep = -1;
            }

            //Create pk for template items
            if (TemplateItemsTable != null
                && TemplateItemsTable.Columns.Contains("ItemID")
                && TemplateItemsTable.PrimaryKey.Length == 0)
            {
                TemplateItemsTable.PrimaryKey = new[] { TemplateItemsTable.Columns["ItemID"] };
            }

            //itemTable.PrimaryKey = new DataColumn[] {itemTable.Columns["ItemID"]};
        }


        /// <summary>
        /// Get names of data tables
        /// </summary>
        public override List<string> ObjectDataTableNames
        {
            get
            {
                return new List<string> { DataTableName, TemplateItemsTableName, PageDataTableName, PageItemsTableName, ItemAppearanceMapTableName, ItemAppearanceTableName };
            }
        }
    }
}
