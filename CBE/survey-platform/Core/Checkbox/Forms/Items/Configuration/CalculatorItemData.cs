using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using Prezza.Framework.Data;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information associated with Calculator items.
    /// </summary>
    [Serializable]
    public class CalculatorItemData : TextItemData
    {
        /// <summary>
        /// Data table name for Calculator item configuration data
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "CalculatorItemData"; }
        }

        /// <summary>
        /// Load procedure name
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetCalculator"; }
        }

        /// <summary>
        /// Get/Set Round to decimal places
        /// </summary>
        public int RoundToPlaces { get; set; }

        /// <summary>
        /// Get/Set Formula
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            RoundToPlaces = DbUtility.GetValueFromDataRow<int>(data, "RoundToPlaces", 0);
            Formula = DbUtility.GetValueFromDataRow<string>(data, "Formula", null);
        }

        /// <summary>
        /// Create an instance of a Calculator item based on this configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new Calculator();
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertCalculator");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("RoundToPlaces", DbType.Int32, RoundToPlaces);
            command.AddInParameter("Formula", DbType.String, Formula);

            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Update an instance of a Calculator configuration in the persistent store
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateCalculator");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("RoundToPlaces", DbType.Int32, RoundToPlaces);
            command.AddInParameter("Formula", DbType.String, Formula);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Writes an XML to the given writer
        /// </summary>
        /// <param name="writer">XML Writer</param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("RoundToPlaces", RoundToPlaces.ToString());
            writer.WriteElementString("Formula", Formula);
        }


        /// <summary>
        /// Reads the data from XML Node
        /// </summary>
        /// <param name="xmlNode">XML Node</param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            RoundToPlaces = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("RoundToPlaces")) ?? 0;
            Formula = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Formula"));

            Save();
        }
      
    }

}
