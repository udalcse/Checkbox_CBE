using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Item to display the current score of a survey
    /// </summary>
    [Serializable]
    public class CurrentScoreItemData : MessageItemData
    {
        /// <summary>
        /// Get/set id of page to print score of.  If value is null, all items in survey before
        /// score item are used.
        /// </summary>
        public int? PageId { get; set; }

        /// <summary>
        /// Create an instance of the business object.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CurrentScore();
        }

        /// <summary>
        /// Get name of load sproc.
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetCurrentScore"; } }

        /// <summary>
        /// Load item data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            PageId = DbUtility.GetValueFromDataRow<int?>(data, "PageId", null);
        }

        /// <summary>
        /// Insert data into db
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertCurrentScore");
            command.AddInParameter("ItemId", DbType.Int32, ID);
            command.AddInParameter("MessageTextId", DbType.String, TextID);
            command.AddInParameter("PageId", DbType.Int32, PageId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Update data in db
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateCurrentScore");
            command.AddInParameter("ItemId", DbType.Int32, ID);
            command.AddInParameter("MessageTextId", DbType.String, TextID);
            command.AddInParameter("PageId", DbType.Int32, PageId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementValue("PageId", PageId);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			PageId =  XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("PageId"));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="pageIdMap"></param>
        protected internal override void UpdateImportId(Dictionary<int, ItemData> itemIdMap, Dictionary<int, TemplatePage> pageIdMap)
        {
            if (!PageId.HasValue || PageId.Value == 0)
                return;

            PageId = pageIdMap[PageId.Value].ID;
        }
    }
}
