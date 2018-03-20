using System;
using System.Data;

using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Common
{
//    /// <summary>
//    /// Abstract extension of persisted domain object that includes basic functionality for a two-table
//    /// data hierarchy.
//    /// </summary>
//    [Serializable]
//    public abstract class AbstractPersistedDomainObject : PersistedDomainObject
//    {
//        /// <summary>
//        /// The name of the ParentData table
//        /// </summary>
//        public abstract string ParentDataTableName { get; }

//        /// <summary>
//        /// Create data relations within the provided data set.
//        /// </summary>
//        /// <param name="ds">DataSet to create data relations in.</param>
//        /// <remarks>Specifically, relations are defined between the abstract "parent" table and concrete child table.</remarks>
//        protected override void CreateDataRelationsInternal(DataSet ds)
//        {
//            if (ds.Tables.Contains(DataTableName))
//            {
//                DataTable concreteItemTable = ds.Tables[DataTableName];
//                DataTable abstractItemTable = ds.Tables[ParentDataTableName];

//                if (string.Compare(DataTableName, ParentDataTableName, true) != 0)
//                {
//                    if (!ds.Relations.Contains(abstractItemTable.TableName + "_" + concreteItemTable.TableName))
//                    {
//                        try
//                        {
//                            DataRelation relation = new DataRelation(
//                                abstractItemTable.TableName + "_" + concreteItemTable.TableName,
//                                abstractItemTable.Columns[IdentityColumnName],
//                                concreteItemTable.Columns[IdentityColumnName], false);

//                            concreteItemTable.Constraints.Add(new ForeignKeyConstraint(
//                                                                  abstractItemTable.Columns[IdentityColumnName],
//                                                                  concreteItemTable.Columns[IdentityColumnName]));

//                            ds.Relations.Add(relation);
//                        }
//                        catch (Exception ex)
//                        {
//                            bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

//                            if (rethrow)
//                            {
//                                throw;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Get a DB Command wrapper to retrieve the object's configuration data set.
//        /// </summary>
//        protected virtual DBCommandWrapper ConfigurationDataSetCommand
//        {
//            get { return null; }
//        }

//        /// <summary>
//        /// Get the configuration data set for the object.
//        /// </summary>
//        /// <returns>Returns a DataSet containing the object's configuration, including any parent and child data.</returns>
//        public override DataSet GetConfigurationDataSet()
//        {
//            if (ID <= 0)
//            {
//                throw new ApplicationException("No DataID specified.");
//            }

//            try
//            {
//                Database db = DatabaseFactory.CreateDatabase();
//                DBCommandWrapper command = ConfigurationDataSetCommand;

//                DataSet ds = new DataSet();
//                db.LoadDataSet(command, ds, new[] { ParentDataTableName });

//                DataSet concreteData = GetConcreteConfigurationDataSet();
//                if (concreteData != null)
//                {
//                    ds.Merge(concreteData);
//                }

//                return ds;
//            }
//            catch (Exception ex)
//            {
//                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

//                if (rethrow)
//                {
//                    throw;
//                }

//                return null;
//            }
//        }

//        /// <summary>
//        /// Get the concrete configuration for the object.  This would include any data specific to the object type, but
//        /// not generic to all persisted domain objects.
//        /// </summary>
//        /// <returns></returns>
//        protected virtual DataSet GetConcreteConfigurationDataSet()
//        {
//            return null;
//        }

//        /// <summary>
//        /// Load the object from the specified data.
//        /// </summary>
//        /// <param name="data">DataSet containing data for the object.</param>
//        public override void Load(DataSet data)
//        {
//            if (data != null && ID.HasValue)
//            {
//                //Attempt to load from passed data set if parent data is present, otherwise load configuration
//                string selectFilter = IdentityColumnName + " = " + ID;

//                if (data.Tables.Contains(ParentDataTableName) && data.Tables[ParentDataTableName].Select(selectFilter).Length > 0)
//                {
//                    Load(data.Tables[ParentDataTableName]);
//                }
//                else
//                {
//                    //Get the parent data
//                    DataSet objectData = GetConfigurationDataSet();

//                    if (objectData.Tables.Contains(ParentDataTableName) && objectData.Tables[ParentDataTableName].Select(selectFilter).Length > 0)
//                    {
//                        Load(data.Tables[ParentDataTableName]);
//                    }

//                    //Merge the parent data
//                    data.Merge(objectData);
//                }


//                //Get the child data, if necessary
//                if (!data.Tables.Contains(DataTableName) || data.Tables[DataTableName].Select(selectFilter).Length == 0)
//                {
//                    DataSet childData = GetConcreteConfigurationDataSet();

//                    //Merge the child data
//                    if (childData != null)
//                    {
//                        data.Merge(childData);
//                    }
//                }
//            }

//            base.Load(data);
//        }

//        /// <summary>
//        /// Load the object's data from the specified data row.  The base method does nothing.
//        /// </summary>
//        /// <param name="data">DataRow containing data for the object.</param>
//        protected virtual void LoadFromDataRow(DataRow data) { }
//    }
}