namespace Checkbox.Common
{
    /// <summary>
    /// Data set container for persisted domain objects that handle associated text data
    /// </summary>
    public abstract class LocalizedPersistedDomainObjectDataSet : AbstractPersistedDomainObjectDataSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectTypeName"></param>
        protected LocalizedPersistedDomainObjectDataSet(string objectTypeName)
            : this(objectTypeName, string.Empty, string.Empty)
        {
        }

          ///<summary>
        ///</summary>
        ///<param name="owningObjectTypeName"></param>
        ///<param name="dataTableName"></param>
        ///<param name="identityColumnName"></param>
        ///<param name="additionalTableNames"></param>
        protected LocalizedPersistedDomainObjectDataSet(string owningObjectTypeName, string dataTableName, string identityColumnName, params string[] additionalTableNames)
            : base(owningObjectTypeName, dataTableName, identityColumnName, additionalTableNames)
        {
            InitializeDataTables();
            InitializeTextTables();
        }


        /// <summary>
        /// Initialize data tables
        /// </summary>
        protected virtual void InitializeDataTables()
        {
        }

        /// <summary>
        /// Initialize assoicated text data tables
        /// </summary>
        protected virtual void InitializeTextTables()
        {
        }
    }
}
