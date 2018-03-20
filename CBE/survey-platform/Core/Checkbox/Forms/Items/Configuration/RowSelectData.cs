using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RowSelectData : SelectItemData
    {
        /// <summary>
        /// Determine if this row selector allows multiple selection or not.
        /// </summary>
        public bool AllowMultipleSelection { get; set; }

        /// <summary>
        /// Get/set the minimum number of items to select.
        /// </summary>
        public int? MinToSelect { get; set; }
        /// <summary>
        /// Get/set the maximum number of items to select.
        /// </summary>
        public int? MaxToSelect { get; set; }

        /// <summary>
        /// Load procedure name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetMatrixRowSelect"; } }

        /// <summary>
        /// Create an instance of a Select1 configuration in the persistent store
        /// </summary>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMatrixRowSelect");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("AllowMultipleSelection", DbType.Boolean, AllowMultipleSelection);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("MinToSelect", DbType.Int32, MinToSelect);
            command.AddInParameter("MaxToSelect", DbType.Int32, MaxToSelect);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Update an instance of a Select1 configuration in the persistent store
        /// </summary>
        protected override void Update(IDbTransaction transaction)
        {
            base.Update(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMatrixRowSelect");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("AllowMultipleSelection", DbType.Boolean, AllowMultipleSelection);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("MinToSelect", DbType.Int32, MinToSelect);
            command.AddInParameter("MaxToSelect", DbType.Int32, MaxToSelect);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }


        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            AllowMultipleSelection = DbUtility.GetValueFromDataRow(data, "AllowMultipleSelection", false);
            MinToSelect = DbUtility.GetValueFromDataRow<int?>(data, "MinToSelect", null);
            MaxToSelect = DbUtility.GetValueFromDataRow<int?>(data, "MaxToSelect", null);
        }

        public override string ItemDataTableName
        {
            get { return "RowSelectData"; }
        }

        /// <summary>
        /// Create an instance of a row select item based on this configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new RowSelect();
        }

        /// <summary>
        /// Create a text decorator for the item that can be used to localize texts for the item
        /// and it's options.
        /// </summary>
        /// <param name="languageCode">Language for the text decorator.</param>
        /// <returns><see cref="SelectItemTextDecorator"/></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new SelectItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("AllowMultipleSelection", AllowMultipleSelection.ToString());
            writer.WriteElementValue("MinToSelect", MinToSelect);
            writer.WriteElementValue("MaxToSelect", MaxToSelect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            AllowMultipleSelection =XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("AllowMultipleSelection"));
            MinToSelect = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MinToSelect"));
            MaxToSelect = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxToSelect"));
        }

    }
}
