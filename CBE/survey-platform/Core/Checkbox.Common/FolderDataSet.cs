namespace Checkbox.Common
{
    /// <summary>
    /// Data container for folders
    /// </summary>
    public class FolderDataSet : PersistedDomainObjectDataSet
    {
        public override string DataTableName { get { return "Folder"; } }
        public override string IdentityColumnName { get { return "FolderId"; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FolderDataSet(string owningObjectTypeName)
            : base(owningObjectTypeName)
        {
        }
    }
}
