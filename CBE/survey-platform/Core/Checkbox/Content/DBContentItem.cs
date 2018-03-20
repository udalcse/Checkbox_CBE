using System;
using System.Data;

using Prezza.Framework.Data;
using Prezza.Framework.Data.Sprocs;

namespace Checkbox.Content
{
    /// <summary>
    /// Database persisted content item
    /// </summary>
    [FetchProcedure("ckbx_sp_Content_GetItem")]
    [InsertProcedure("ckbx_sp_Content_CreateItem")]
    [UpdateProcedure("ckbx_sp_Content_UpdateItem")]
    [DeleteProcedure("ckbx_sp_Content_DeleteItem")]
    public class DBContentItem : ContentItem
    {
        /// <summary>
        /// Construct a new content item
        /// </summary>
        /// <param name="itemID"></param>
        public DBContentItem(int? itemID)
        {
            ItemID = itemID;
        }

        /// <summary>
        /// Get/set the item database id
        /// </summary>
        [FetchParameter(Name="ItemID", DbType=DbType.Int32, Direction=ParameterDirection.Input), DeleteParameter(Name = "ItemID", DbType = DbType.Int32, Direction = ParameterDirection.Input), UpdateParameter(Name = "ItemID", DbType = DbType.Int32, Direction = ParameterDirection.Input), InsertParameter(Name = "ItemID", DbType = DbType.Int32, Direction = ParameterDirection.Output, Size=4)]
        public int? ItemID { get; set; }

        /// <summary>
        /// Get/set creator of item
        /// </summary>
        [FetchParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.ReturnValue), UpdateParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.Input), InsertParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set item url
        /// </summary>
        [FetchParameter(Name = "ItemUrl", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "ItemUrl", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "ItemUrl", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public override string ItemUrl
        {
            get { return base.ItemUrl; }
            set { base.ItemUrl = value; }
        }

        /// <summary>
        /// Get/set parent folder db id
        /// </summary>
        [FetchParameter(Name = "FolderID", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue), UpdateParameter(Name = "FolderID", DbType = DbType.Int32, Direction = ParameterDirection.Input), InsertParameter(Name = "FolderID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public int? FolderID { get; set; }

        /// <summary>
        /// Get/set content type
        /// </summary>
        [FetchParameter(Name = "MIMEContentType", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "MIMEContentType", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "MIMEContentType", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public override string ContentType
        {
            get { return base.ContentType; }
            set { base.ContentType = value; }
        }

        /// <summary>
        /// Get/set whether the item is public
        /// </summary>
        [FetchParameter(Name = "IsPublic", DbType = DbType.Boolean, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "IsPublic", DbType = DbType.Boolean, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "IsPublic", DbType = DbType.Boolean, Direction = ParameterDirection.Input)]
        public override bool IsPublic
        {
            get { return base.IsPublic; }
            set { base.IsPublic = value; }
        }
        
        /// <summary>
        /// Get/set the name of the item
        /// </summary>
        [FetchParameter(Name = "ItemName", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "ItemName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "ItemName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public override string ItemName
        {
            get { return base.ItemName; }
            set { base.ItemName = value; }
        }

        /// <summary>
        /// Get/set when the item was last updated
        /// </summary>
        [FetchParameter(Name = "LastUpdated", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "LastUpdated", DbType = DbType.DateTime, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "LastUpdated", DbType = DbType.DateTime, Direction = ParameterDirection.Input)]
        public override DateTime LastUpdated
        {
            get { return base.LastUpdated; }
            set { base.LastUpdated = value; }
        }

        /// <summary>
        /// Get/set the item data
        /// </summary>
        [UpdateParameter(Name = "ItemData", DbType = DbType.Binary, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "ItemData", DbType = DbType.Binary, Direction = ParameterDirection.Input)]
        public override byte[] Data
        {
            get { return base.Data; }
            set { base.Data = value; }
        }

        /// <summary>
        /// Load the item's data from the database
        /// </summary>
        /// <returns></returns>
        protected override byte[] LoadItemData()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Content_GetItemData");
            command.AddInParameter("ItemID", DbType.Int32, ItemID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read() && reader["ItemData"] != DBNull.Value)
                    {
                        return (byte[])reader["ItemData"];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Save the item
        /// </summary>
        public void Save()
        {
            StoredProcedureCommandExtractor.ExecuteProcedure(ItemID.HasValue ? ProcedureType.Update : ProcedureType.Insert, this);
        }

        /// <summary>
        /// Delete the item
        /// </summary>
        public void Delete()
        {
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Delete, this);
        }

        /// <summary>
        /// Load the item
        /// </summary>
        public void Load()
        {
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, this);
        }
    }
}
