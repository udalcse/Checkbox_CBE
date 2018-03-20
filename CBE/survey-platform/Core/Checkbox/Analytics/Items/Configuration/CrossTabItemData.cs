using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class CrossTabItemData : AnalysisItemData
    {
        private List<int> _yAxisItemIds;
        private List<int> _xAxisItemIds;

        /// <summary>
        /// Get name of load stored procedure
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetCrossTab"; } }

        /// <summary>
        /// Get list of items on y axis of cross tab
        /// </summary>
        public List<int> YAxisItemIds
        {
            get { return _yAxisItemIds ?? (_yAxisItemIds = new List<int>()); }
        }

        /// <summary>
        /// Get list of items on x axis of cross tab
        /// </summary>
        public List<int> XAxisItemIds
        {
            get { return _xAxisItemIds ?? (_xAxisItemIds = new List<int>()); }
        }

        /// <summary>
        /// Create data container for cross tab item data
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new CrossTabItemDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Load item to axis mappings
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            XAxisItemIds.Clear();
            YAxisItemIds.Clear();

            base.LoadAdditionalData(data);

            //X-Axis
            DataRow[] axisRows = ((CrossTabItemDataSet)data).GetXAxisItems();

            foreach (DataRow axisRow in axisRows)
            {
                var itemId = DbUtility.GetValueFromDataRow<int?>(axisRow, "ItemId", null);

                if (itemId.HasValue)
                {
                    XAxisItemIds.Add(itemId.Value);
                }
            }

            //Y-Axis
            axisRows = ((CrossTabItemDataSet)data).GetYAxisItems();

            foreach (DataRow axisRow in axisRows)
            {
                var itemId = DbUtility.GetValueFromDataRow<int?>(axisRow, "ItemId", null);

                if (itemId.HasValue)
                {
                    YAxisItemIds.Add(itemId.Value);
                }
            }
        }


        /// <summary>
        /// Add an item to the x-axis of the cross tab
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void AddXAxisItem(Int32 itemID)
        {
            if (!XAxisItemIds.Contains(itemID))
            {
                XAxisItemIds.Add(itemID);
            }
        }

        /// <summary>
        /// Add an item to the y-axis of the cross tab
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void AddYAxisItem(Int32 itemID)
        {
            if (!YAxisItemIds.Contains(itemID))
            {
                YAxisItemIds.Add(itemID);
            }
        }

        /// <summary>
        /// Remove an item from the x-axis
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void RemoveXAxisItem(Int32 itemID)
        {
            if (XAxisItemIds.Contains(itemID))
            {
                XAxisItemIds.Remove(itemID);
            }
        }

        /// <summary>
        /// Remove an item from the y-axis
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void RemoveYAxisItem(Int32 itemID)
        {
            if (YAxisItemIds.Contains(itemID))
            {
                YAxisItemIds.Remove(itemID);
            }
        }

        /// <summary>
        /// Create an instance of a crosstab item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            //Save crosstab data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertCrosstab");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            //Save source and item mappings
            SaveSourceAndAxisMappings(t);
        }

        /// <summary>
        /// Create an instance of a crosstab item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to update crosstab item.  DataID is <= 0.");
            }

            //Save crosstab data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateCrosstab");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            db.ExecuteNonQuery(command, t);

            //Save source and item mappings
            SaveSourceAndAxisMappings(t);
        }

        /// <summary>
        /// Save source item and axis mappings
        /// </summary>
        /// <param name="t"></param>
        protected virtual void SaveSourceAndAxisMappings(IDbTransaction t)
        {
            //Update source items collection to include x-axis and y-axis items.
            //First clear items no longer included.
            var itemsToRemove = SourceItemIds.Where(itemId => !XAxisItemIds.Contains(itemId) && !YAxisItemIds.Contains(itemId)).ToList();

            foreach (var itemId in itemsToRemove)
            {
                RemoveSourceItem(itemId);
            }


            //Now ensure that each item is x & y axes are included.  The AddSourceItem
            // method of base class will perform contains check, so no need to do that here.
            foreach (int itemId in XAxisItemIds)
            {
                AddSourceItem(itemId);
            }

            foreach (int itemId in YAxisItemIds)
            {
                AddSourceItem(itemId);
            }

            //Update source items and RT tables
            UpdateSourceItemTables(t);

            //Now store item to axis mappings
            // a) clear mappings, to be safe
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = GetDeleteAxisItemCommand(db);
            db.ExecuteNonQuery(command, t);

            // b) insert mappings
            foreach (int itemId in XAxisItemIds)
            {
                command = GetInsertAxisItemCommand(db);
                command.AddInParameter("Axis", DbType.String, "X");
                command.AddInParameter("ItemId", DbType.Int32, itemId);

                db.ExecuteNonQuery(command, t);
            }

            foreach (int itemId in YAxisItemIds)
            {
                command = GetInsertAxisItemCommand(db);
                command.AddInParameter("Axis", DbType.String, "Y");
                command.AddInParameter("ItemId", DbType.Int32, itemId);

                db.ExecuteNonQuery(command, t);
            }
        }
           

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CrossTabItem();
        }

        /// <summary>
        /// Get a command
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetInsertAxisItemCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_CrossTab_InsertMap");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

     
        /// <summary>
        /// Get a command
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeleteAxisItemCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_CrossTab_DeleteMap");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);
         
            return command;
        }

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("YAxisItemIds", String.Join(",", YAxisItemIds));
            writer.WriteElementString("XAxisItemIds", String.Join(",", XAxisItemIds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            
            var y = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("YAxisItemIds"));
            if (!String.IsNullOrEmpty(y))
                _yAxisItemIds = y.Split(',').Select(int.Parse).ToList();

            var x =  XmlUtility.GetNodeText(xmlNode.SelectSingleNode("XAxisItemIds"));
            if (!String.IsNullOrEmpty(x))
                _xAxisItemIds = x.Split(',').Select(int.Parse).ToList();
        }

        #endregion

    }
}
