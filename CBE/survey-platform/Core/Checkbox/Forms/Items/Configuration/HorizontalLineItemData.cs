using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data container for horizontal line items
    /// </summary>
    [Serializable]
    public class HorizontalLineItemData : ResponseItemData
    {
        /// <summary>
        /// Get name of data table containing configuration information for the item.
        /// </summary>
        public override string ItemDataTableName { get { return "HorizontalLineItemData"; } }

        /// <summary>
        /// Get name of load item sproc.
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetHR"; } }

        /// <summary>
        /// Get/set the width of the line
        /// </summary>
        public int? LineWidth { get; set; }

        /// <summary>
        /// Get set the units to use for line width
        /// </summary>
        public string WidthUnit { get; set; }

        /// <summary>
        /// Get set the color of line
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Get/set the thickness of the line
        /// </summary>
        public int? Thickness { get; set; }

        /// <summary>
        /// Create the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("DataID must be set to create item data.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertHR");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Width", DbType.Int32, LineWidth);
            command.AddInParameter("Unit", DbType.String, WidthUnit);
            command.AddInParameter("Color", DbType.String, Color);
            command.AddInParameter("Thickness", DbType.Int32, Thickness);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an existing instance of the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("DataID must be set to update item.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateHR");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Width", DbType.Int32, LineWidth);
            command.AddInParameter("Unit", DbType.String, WidthUnit);
            command.AddInParameter("Color", DbType.String, Color);
            command.AddInParameter("Thickness", DbType.Int32, Thickness);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementValue("LineWidth", LineWidth);
			writer.WriteElementString("WidthUnit", WidthUnit);
			writer.WriteElementString("Color", Color);
			writer.WriteElementValue("Thickness", Thickness);
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			LineWidth =XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("LineWidth"));
			WidthUnit = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("WidthUnit"));
			Color = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Color"));
			Thickness = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("Thickness"));
		}

        /// <summary>
        /// Load this item data from the datarow
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            LineWidth = DbUtility.GetValueFromDataRow<int?>(data, "Width", 100);
            WidthUnit = DbUtility.GetValueFromDataRow(data, "Unit", "Percent");
            Thickness = DbUtility.GetValueFromDataRow<int?>(data, "Thickness", 1);
            Color = DbUtility.GetValueFromDataRow(data, "Color", string.Empty);
        }


        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new HorizontalLine();
        }
    }
}
