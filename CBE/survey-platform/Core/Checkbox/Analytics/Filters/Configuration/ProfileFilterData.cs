using System;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Meta data container and factory for filters that compare respondent profile attribute values.
    /// </summary>
    [Serializable]
    public class ProfileFilterData : ResponseFilterData
    {
        /// <summary>
        /// Get the name of the <see cref="DataTable"/> in the filter data configuration <see cref="DataSet"/> containing the configuration information
        /// for this type of filter.
        /// </summary>
        public override string DataTableName { get { return "ProfileFilterData"; } }

        /// <summary>
        /// Get the name of the <see cref="DataColumn"/> in the filter data configuration <see cref="DataTable"/> containing the identities of 
        /// this type of filter.
        /// </summary>
        public override string IdentityColumnName { get { return "FilterID"; } }

        /// <summary>
        /// Get the name of the configuration <see cref="DataColumn"/> containing the name of the profile attribute this filter uses to compare.
        /// </summary>
        protected override string PropertyFieldName{ get { return "ProfileField"; } }

        /// <summary>
        /// Load filter sproc.
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Filter_GetProfile"; } }

        /// <summary>
        /// Create an instance of a <see cref="ProfileFilter"/> object.
        /// </summary>
        /// <returns><see cref="ProfileFilter"/> object configured with this object's data.</returns>
        protected override Filter CreateFilterObject()
        {
            return new ProfileFilter();
        }

        /// <summary>
        /// Get a <see cref="DBCommandWrapper"/> configured to update the filter configuration int the
        /// database.
        /// </summary>
        /// <param name="db"><see cref="Database"/> object used to create the command wrapper object.</param>
        /// <returns><see cref="DBCommandWrapper"/> configured to update the filter configuration in the
        /// database.</returns>
        protected override DBCommandWrapper GetCreateCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_InsertProfile");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);
            command.AddInParameter("ProfileField", DbType.String, PropertyName);

            return command;
        }

        /// <summary>
        /// Get a <see cref="DBCommandWrapper"/> configured to delete the filter configuration from the
        /// database.
        /// </summary>
        /// <param name="db"><see cref="Database"/> object used to create the command wrapper object.</param>
        /// <returns><see cref="DBCommandWrapper"/> configured to delete the filter configuration from the
        /// database.</returns>
        protected override DBCommandWrapper GetDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_DeleteProfile");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);

            return command;
        }


       
    }
}
