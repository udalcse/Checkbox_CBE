using System;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Meta data and factory for filters applied at the response level, rather than at the specific answer level.
    /// </summary>
    [Serializable]
    public class ResponseFilterData : FilterData
    {
        /// <summary>
        /// Get/set the name of the response property to filter on.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Get type name of object
        /// </summary>
        public override string ObjectTypeName { get { return "ResponseFilter"; } }

        /// <summary>
        /// Get the name of the <see cref="DataTable"/> in the filter data configuration <see cref="DataSet"/> containing the configuration information
        /// for this type of filter.
        /// </summary>
        public override string DataTableName { get { return "ResponseFilterData"; } }

        /// <summary>
        /// Get the name of the <see cref="DataColumn"/> in the filter data configuration <see cref="DataTable"/> containing the identities of 
        /// this type of filter.
        /// </summary>
        public override string IdentityColumnName { get { return "FilterId"; } }

        /// <summary>
        /// Get the name of the configuration <see cref="DataColumn"/> containing the name of the response attribute this filter uses for comparisons.
        /// </summary>
        protected virtual string PropertyFieldName { get { return "ResponseProperty"; } }

        /// <summary>
        /// Create an instance of the <see cref="ResponseFilter"/> object.
        /// </summary>
        /// <returns><see cref="ResponseFilter"/> object configured with this object's data.</returns>
        protected override Filter CreateFilterObject()
        {
            return new ResponseFilter();
        }

        /// <summary>
        /// Create data set used to store this object's data.
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new FilterDataSet(ObjectTypeName, DataTableName, IdentityColumnName);
        }

        /// <summary>
        /// Get procedure to load sproc
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Filter_GetResponse"; } }

        /// <summary>
        /// Get the left operand text for displaying the filter as text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        protected override string GetFilterLeftOperandText(string languageCode)
        {
            return PropertyName;
        }

        /// <summary>
        /// Get a <see cref="DBCommandWrapper"/> configured to update the filter configuration int the
        /// database.
        /// </summary>
        /// <param name="db"><see cref="Database"/> object used to create the command wrapper object.</param>
        /// <returns><see cref="DBCommandWrapper"/> configured to update the filter configuration in the
        /// database.</returns>
        protected virtual DBCommandWrapper GetCreateCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_InsertResponse");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);
            command.AddInParameter("ResponseProperty", DbType.String, PropertyName);

            return command;
        }

        /// <summary>
        /// Get a <see cref="DBCommandWrapper"/> configured to delete the filter configuration from the
        /// database.
        /// </summary>
        /// <param name="db"><see cref="Database"/> object used to create the command wrapper object.</param>
        /// <returns><see cref="DBCommandWrapper"/> configured to delete the filter configuration from the
        /// database.</returns>
        protected virtual DBCommandWrapper GetDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_DeleteResponse");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);

            return command;
        }

        /// <summary>
        /// Insert the filter configuration information into the database.
        /// </summary>
        /// <param name="t">Transaction to participate in for the insertion.</param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();

            db.ExecuteNonQuery(GetCreateCommand(db), t);
        }

        /// <summary>
        /// Delete the filter configuration information from the database.
        /// </summary>
        /// <param name="t">Transaction to participate in for the deletion.</param>
        public override void Delete(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            db.ExecuteNonQuery(GetDeleteCommand(db), t);

            base.Delete(t);
        }

        /// <summary>
        /// Load the filter configuration from the specified <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"><see cref="DataRow"/> containing filter configuration information.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            PropertyName = DbUtility.GetValueFromDataRow<string>(data, PropertyFieldName, null);
        }
    }
}
