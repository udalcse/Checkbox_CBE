using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Prezza.Framework.Data;

namespace Checkbox.Common
{
    /// <summary>
    /// Data container for abstract pdo, which is a pdo that has a parent mapping in another
    /// table that lists a number of like pdos.
    /// </summary>
    public abstract class AbstractPersistedDomainObjectDataSet : PersistedDomainObjectDataSet
    {
        /// <summary>
        /// Get name of parent table for relationship.
        /// </summary>
        public abstract string ParentDataTableName { get; }

        /// <summary>
        /// Get object data table names
        /// </summary>
        public override List<string> ObjectDataTableNames 
        {
            get
            {
                List<string> names = base.ObjectDataTableNames;

                if (!names.Contains(ParentDataTableName, StringComparer.InvariantCultureIgnoreCase)
                    && Utilities.IsNotNullOrEmpty(ParentDataTableName))
                {
                    names.Insert(0, ParentDataTableName);
                }

                return names;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="owningObjectTypeName"></param>
        ///<param name="dataTableName"></param>
        ///<param name="identityColumnName"></param>
        ///<param name="additionalTableNames"></param>
        protected AbstractPersistedDomainObjectDataSet(string owningObjectTypeName, string dataTableName, string identityColumnName, params string[] additionalTableNames)
            : base(owningObjectTypeName, dataTableName, identityColumnName, additionalTableNames)
        {
        }

        /// <summary>
        /// Get a command wrapper for loading "abstract" data for the object.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="owningObjectId"></param>
        /// <returns></returns>
        protected abstract DBCommandWrapper CreateAbstractDataCommand(Database db, int owningObjectId);

        /// <summary>
        /// Load abstract data
        /// </summary>
        /// <param name="owningObjectId"></param>
        /// <param name="loadSprocName"></param>
        /// <param name="parameters"></param>
        public override void Load(int owningObjectId, string loadSprocName, List<DbParameter> parameters)
        {
            //Load abstract data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper abstractDataCommand = CreateAbstractDataCommand(db, owningObjectId);

            if (abstractDataCommand != null
                && Utilities.IsNotNullOrEmpty(ParentDataTableName))
            {
                db.LoadDataSet(abstractDataCommand, this, ParentDataTableName);
            }

            //Load concrete data
            if (Utilities.IsNotNullOrEmpty(loadSprocName))
            {
                base.Load(owningObjectId, loadSprocName, parameters);
            }
        }
    }
}
