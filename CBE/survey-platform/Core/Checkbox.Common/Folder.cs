using System;
using System.Data;

using Prezza.Framework.Data;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Common
{
    /// <summary>
    /// Access Controllable container for objects.
    /// </summary>
    [Serializable]
    public abstract class Folder : AccessControllablePersistedDomainObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="supportedPermissionMasks">List of permission masks supported by the folder.</param>
        /// <param name="supportedPermissions">List of supported permissions for the folder.</param>
        protected Folder(string[] supportedPermissionMasks, string[] supportedPermissions)
            : base(supportedPermissionMasks, supportedPermissions)
        {
        }

        /// <summary>
        /// Get the configuration data container for a folder
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new FolderDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Get object type name
        /// </summary>
        public override string ObjectTypeName { get { return "Folder"; } }

        /// <summary>
        /// Get sproc for loading folders
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Folder_Get"; } }

        /// <summary>
        /// Get/Set the folder description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get the unique identifier of the folder's creator.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set count of children. For the survey it's 0, for the folder -- count of the surveys in it.
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// Get data table name in database
        /// </summary>
        public override string DomainDBTableName
        {
            get { return "ckbx_Folder"; }
        }

        /// <summary>
        /// Get folder id
        /// </summary>
        public override string DomainDBIdentityColumnName
        {
            get { return "FolderID"; }
        }

        /// <summary>
        /// Add an item to the folder.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public abstract void Add(object item);

        /// <summary>
        /// Add an item with the specified id to the folder.
        /// </summary>
        /// <param name="id">ID of the item to add.</param>
        public abstract void Add(int id);

        /// <summary>
        /// Remove the specified item from the folder.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public abstract void Remove(object item);

        /// <summary>
        /// Remove the item with the specified id from the folder.
        /// </summary>
        /// <param name="id">ID of the item to remove.</param>
        public abstract void Remove(int id);

        /// <summary>
        /// Move and item to a folder.
        /// </summary>
        /// <param name="folder">Folder to move item to.</param>
        /// <param name="itemID">Item to move.</param>
        protected virtual void MoveToFolder(Folder folder, int itemID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_MoveToFolder");
            command.AddInParameter("FolderID", DbType.Int32, folder.ID);
            command.AddInParameter("ItemID", DbType.Int32, itemID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Load the folder from a data row.
        /// </summary>
        /// <param name="data">Data row to load folder information from.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            Name = DbUtility.GetValueFromDataRow(data, "Name", string.Empty);
            Description = DbUtility.GetValueFromDataRow(data, "Description", string.Empty);
            CreatedDate = DbUtility.GetValueFromDataRow<DateTime?>(data, "DateCreated", null);
            LastModified = DbUtility.GetValueFromDataRow<DateTime?>(data, "LastModified", null);
            CreatedBy = DbUtility.GetValueFromDataRow(data, "CreatedBy", string.Empty);

            AclID = DbUtility.GetValueFromDataRow<int?>(data, "AclId", null);
            DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(data, "DefaultPolicy", null);
            ChildrenCount = DbUtility.GetValueFromDataRow<int>(data, "ChildrenCount", 0);
        }
        /// <summary>
        /// Copy the folder.
        /// </summary>
        /// <param name="owner">Owner of the copied folder.</param>
        /// <param name="languageCode">Language code used to look up "copy of" naming</param>
        /// <returns>Copy of the folder.</returns>
        public abstract Folder Copy(ExtendedPrincipal owner, string languageCode);
    }
}