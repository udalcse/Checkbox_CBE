using System;
using System.Data;
using System.Collections.Generic;

using Prezza.Framework.Data;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Content
{
    /// <summary>
    /// Representation of a content folder
    /// </summary>
    [FetchProcedure("ckbx_sp_Content_GetFolder")]
    [InsertProcedure("ckbx_sp_Content_CreateFolder")]
    [UpdateProcedure("ckbx_sp_Content_UpdateFolder")]
    [DeleteProcedure("ckbx_sp_Content_DeleteFolder")]
    public class DBContentFolder : ContentFolder
    {
        private readonly ExtendedPrincipal _currentUser;

        /// <summary>
        /// Construct a new folder object
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="folderPath"></param>
        /// <param name="currentUser"></param>
        public DBContentFolder(int? folderID, string folderPath, ExtendedPrincipal currentUser)
        {
            FolderID = folderID;
            _currentUser = currentUser;
            FolderPath = folderPath;
        }

        /// <summary>
        /// Get/set the folder id
        /// </summary>
        [FetchParameter(Name="FolderID", DbType=DbType.Int32, Direction=ParameterDirection.Input), UpdateParameter(Name="FolderID", DbType=DbType.Int32, Direction=ParameterDirection.Input), InsertParameter(Name="FolderID", DbType=DbType.Int32, Direction=ParameterDirection.Output,Size=4), DeleteParameter(Name="FolderID", DbType=DbType.Int32, Direction=ParameterDirection.Input)]
        public int? FolderID { get; set; }

        /// <summary>
        /// Get/set the folder path
        /// </summary>
        [FetchParameter(Name = "FolderPath", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "FolderPath", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "FolderPath", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public override string FolderPath
        {
            get { return base.FolderPath; }
            set { base.FolderPath = value; }
        }

        /// <summary>
        /// Get/set parent folder id
        /// </summary>
        [FetchParameter(Name = "ParentFolderID", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue), UpdateParameter(Name = "ParentFolderID", DbType = DbType.Int32, Direction = ParameterDirection.Input), InsertParameter(Name = "ParentFolderID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public int? ParentFolderID { get; set; }

        /// <summary>
        /// Get/set folder name
        /// </summary>
        [FetchParameter(Name = "FolderName", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "FolderName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "FolderName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public override string FolderName
        {
            get { return base.FolderName; }
            set { base.FolderName = value; }
        }

        /// <summary>
        /// Get/set is public
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
        /// Get/set created by
        /// </summary>
        [FetchParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.ReturnValue), UpdateParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.Input), InsertParameter(Name = "CreatedBy", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Store the item
        /// </summary>
        /// <param name="item"></param>
        protected override void StoreContentItem(ContentItem item)
        {
            if (item is DBContentItem)
            {
                ((DBContentItem)item).FolderID = FolderID;
                item.LastUpdated = DateTime.Now;
                ((DBContentItem)item).Save();
            }
        }

        /// <summary>
        /// Delete the item
        /// </summary>
        /// <param name="item"></param>
        protected override void DeleteContentItem(ContentItem item)
        {
            if (item is DBContentItem)
            {
                ((DBContentItem)item).Delete();
            }
        }

        /// <summary>
        /// Load child items
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, ContentItem> LoadItems()
        {
            return DBContentManager.ListItems(FolderPath, _currentUser, new string[] { });
        }

        /// <summary>
        /// Load child folders
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, ContentFolder> LoadFolders()
        {
            return DBContentManager.ListFolders(FolderPath, _currentUser);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        public void Save()
        {
            StoredProcedureCommandExtractor.ExecuteProcedure(FolderID.HasValue ? ProcedureType.Update : ProcedureType.Insert, this);
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
            //If necessary, get the folder id
            if (!FolderID.HasValue)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Content_GetFolderByPath");
                command.AddInParameter("FolderPath", DbType.String, FolderPath);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            if (reader["FolderID"] != DBNull.Value)
                            {
                                FolderID = (int)reader["FolderID"];
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, this);
        }
    }
}
