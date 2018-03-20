using System;
using System.Data;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using System.Collections.Specialized;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Matrix summary data
    /// </summary>
    [Serializable]
    public class MatrixSummaryItemData : AnalysisItemData
    {
        private int? _matrixSourceItem;

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetMatSummary"; } }

        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "MatrixSummaryItemData"; } }

        /// <summary>
        /// Get the matrix source item
        /// </summary>
        public int? MatrixSourceItem
        {
            get { return _matrixSourceItem; }
            set
            {
                _matrixSourceItem = value;
                //AddSourceItem(_matrixSourceItem.Value);
            }
        }

        /// <summary>
        /// Only allow one source item at a time
        /// </summary>
        /// <param name="sourceItemID"></param>
        public override void AddSourceItem(int sourceItemID)
        {
            if (!SourceItemIds.Contains(sourceItemID))
            {
                //Clear all other source items
                foreach (Int32 id in SourceItemIds)
                {
                    RemoveSourceItem(id);
                }

                ItemData itemData = ItemConfigurationManager.GetConfigurationData(sourceItemID);

                if (itemData != null)
                {
                    if (itemData is ICompositeItemData)
                    {
                        //Only add a matrix item
                        base.AddSourceItem(sourceItemID);

                        foreach (Int32 itemID in ((ICompositeItemData)itemData).GetChildItemDataIDs())
                        {
                            AddSourceItem(itemID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            if (data != null)
            {
                _matrixSourceItem = DbUtility.GetValueFromDataRow<int?>(data, "MatrixSourceItem", null);
                UseAliases = DbUtility.GetValueFromDataRow(data, "UseAliases", false);
            }
        }

        /// <summary>
        /// Create an instance of the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to save matrix item data.  Data ID was null or <= zero.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMatSummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MatrixSourceItem", DbType.Int32, _matrixSourceItem);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            //Execute the query
            db.ExecuteNonQuery(command, t);

            //Update the data tables
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update an instance of the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to save matrix item data.  Data ID was null or <= zero.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMatSummary");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MatrixSourceItem", DbType.Int32, _matrixSourceItem);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            //Execute the query
            db.ExecuteNonQuery(command, t);

            //Update the data tables
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create a matrix summary item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new MatrixSummaryItem();
        }

        protected override void BuildDataTransferObject(Wcf.Services.Proxies.IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);
            
            //Get text of matrix item
            var textDataList = ((ItemMetaData)itemDto).TextData;

            //
            foreach (var textData in textDataList)
            {
                if (MatrixSourceItem.HasValue && MatrixSourceItem.Value > 0)
                {
                    textData.TextValues["NavText"] =
                        Utilities.StripHtml(
                            ItemConfigurationManager.GetItemText(
                                MatrixSourceItem.Value,
                                textData.LanguageCode,
                                null,
                                UseAliases,
                                false),
                            50);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("MatrixSourceItem", MatrixSourceItem.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(System.Xml.XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            MatrixSourceItem = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MatrixSourceItem"));
        }
    }
}
