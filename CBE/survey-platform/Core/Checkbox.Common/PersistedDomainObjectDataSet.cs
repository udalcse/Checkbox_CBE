using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Prezza.Framework.Data;

namespace Checkbox.Common
{
    /// <summary>
    /// Base class for persisted domain object data.
    /// </summary>
    public class PersistedDomainObjectDataSet : DataSet
    {
        /// <summary>
        /// Construct the data set.
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        public PersistedDomainObjectDataSet(string owningObjectTypeName)
            : this(owningObjectTypeName, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        /// <param name="dataTableName"></param>
        /// <param name="identityColumnName"></param>
        /// <param name="additionalTableNames"></param>
        public PersistedDomainObjectDataSet(string owningObjectTypeName, string dataTableName, string identityColumnName, params string[] additionalTableNames)
        {
            OwningObjectTypeName = owningObjectTypeName;
            IdentityColumnName = identityColumnName;
            DataTableName = dataTableName;
            AdditionalTableNames = new List<string>(additionalTableNames);
        }

        /// <summary>
        /// Get a name of the pdo that owns this data.
        /// </summary>
        public string OwningObjectTypeName { get; private set; }

        /// <summary>
        /// Get id of object owning this item.
        /// </summary>
        public int? OwningObjectId { get; set; }

        /// <summary>
        /// Get list of data table names for the object.  The object's base data table
        /// should always be the first name in this list.  The PDO base class implicitly
        /// creates a list with data table as only entry if not overridden.
        /// </summary>
        public virtual List<string> ObjectDataTableNames
        {
            get 
            {
                var tableNames = new List<string>{DataTableName};

                if (AdditionalTableNames.Count > 0)
                {
                    tableNames.AddRange(AdditionalTableNames);
                }

                return tableNames;
            }
        }

        /// <summary>
        /// Get names of "Additional" data tables
        /// </summary>
        public List<String> AdditionalTableNames { get; private set; }

        /// <summary>
        /// Get the name of the datatable containing configuration for this PersistedDomainObject.  This name will
        /// be used when searching for object data within a dataset.
        /// </summary>
        public virtual string DataTableName { get; private set; }

        /// <summary>
        /// Get the name of the column of the <see cref="DataTable"/> with a name of <see cref="DataTableName"/> that
        /// contains identities for persisted objects.
        /// </summary>
        public virtual string IdentityColumnName { get; private set; }

        /// <summary>
        /// Get data table with domain object base data.
        /// </summary>
        public DataTable DomainObjectDataTable
        {
            get
            {
                if (Tables.Contains(DataTableName))
                {
                    return Tables[DataTableName];
                }

                return null;
            }
        }

        /// <summary>
        /// Get data row associated with domain object
        /// </summary>
        public DataRow DomainObjectDataRow
        {
            get
            {
                if (DomainObjectDataTable != null
                    && Utilities.IsNotNullOrEmpty(IdentityColumnName)
                    && OwningObjectId.HasValue
                    && DomainObjectDataTable.Columns.Contains(IdentityColumnName))
                {
                    DataRow[] rows = DomainObjectDataTable.Select(IdentityColumnName + " = " + OwningObjectId);

                    if (rows.Length > 0)
                    {
                        return rows[0];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningObjectId"></param>
        /// <param name="loadSprocName"></param>
        /// <param name="parameters"></param>
        public void LoadDataSet(int owningObjectId, string loadSprocName, List<DbParameter> parameters)
        {
            OwningObjectId = owningObjectId;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper(loadSprocName);

            foreach (DbParameter parameter in parameters)
            {
                command.AddInParameter(parameter.ParameterName, parameter.DbType, parameter.Value);
            }

            db.LoadDataSet(command, this, ObjectDataTableNames.ToArray());
        }

        /// <summary>
        /// Load the object from the persistent data store
        /// </summary>
        /// <param name="owningObjectId"></param>
        /// <param name="loadSprocName"></param>
        /// <param name="parameters"></param>
        public virtual void Load(int owningObjectId, string loadSprocName, List<DbParameter> parameters)
        {
            LoadDataSet(owningObjectId, loadSprocName, parameters);
        }
    }
}
