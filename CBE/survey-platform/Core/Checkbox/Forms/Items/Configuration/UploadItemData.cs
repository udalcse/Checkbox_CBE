using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// MetaData for Upload Items
    /// </summary>
    [Serializable]
    public class UploadItemData : LabelledItemData
    {
        private List<string> _allowedFileTypes;

        #region Properties

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetFileUpload"; } }

        /// <summary>
        /// Get/Set the file size of an uploaded file in bytes.
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// Get/Set the list of files types which are approved for upload.
        /// </summary>
        public List<string> AllowedFileTypes
        {
            get { return _allowedFileTypes ?? (_allowedFileTypes = new List<string>()); }
        }

        /// <summary>
        /// Get the list of files types which are approved for upload as a comma separated list.
        /// </summary>
        public string AllowedFileTypesCSV
        {
            get { return string.Join(", ", AllowedFileTypes.ToArray()); }
        }

        /// <summary>
        /// Get an indication of whether the item created by CreateItem(...) supports
        /// the IAnswerable interface.
        /// </summary>
        public override bool ItemIsIAnswerable
        {
            get { return true; }
        }
        #endregion

        /// <summary>
        /// Data table name
        /// </summary>
        public override string ItemDataTableName { get { return "UploadItemData"; } }

        /// <summary>
        /// Create an instance of a hidden item based on this configuration.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new UploadItem();
        }

        /// <summary>
        /// Load the item's configuration from the specified data row.
        /// </summary>
        /// <param name="data">DataRow containing configuration information for this item.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            if (data == null)
            {
                throw new Exception("Unable to load Upload Item data from a null data row.");
            }

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;

            AllowedFileTypes.Clear();

            if (data["AllowedFileTypes"] != DBNull.Value)
            {
                AllowedFileTypes.AddRange(
                    ((string) data["AllowedFileTypes"])
                        .Replace(" ", string.Empty)
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// Create a new entry for upload item data in the database.
        /// </summary>
        /// <param name="t">Database transaction to participate in for database updates.</param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to create configuration data data for File Upload Item with null or negative id.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertFileUpload");
            AddParams(command);
            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an entry for Update Item data in the database.
        /// </summary>
        /// <param name="t">Database transaction to participate in for database updates.</param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to update configuration data data for File Upload Item with null or negative id.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateFileUpload");
            AddParams(command);
            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Add parameters to the 
        /// </summary>
        /// <param name="command"></param>
        private void AddParams(DBCommandWrapper command)
        {
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, base.TextID);
            command.AddInParameter("SubTextID", DbType.String, base.SubTextID);
            command.AddInParameter("IsRequired", DbType.Boolean, base.IsRequired);
            command.AddInParameter("AllowedFileTypes", DbType.String, AllowedFileTypesCSV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("AllowedFileTypes", AllowedFileTypesCSV);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			string types = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AllowedFileTypes"));

            if (types != null)
            {
                AllowedFileTypes.AddRange(types.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}
