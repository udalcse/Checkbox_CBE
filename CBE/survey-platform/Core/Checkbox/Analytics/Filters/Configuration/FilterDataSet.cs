using System.Data;
using Checkbox.Common;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Data container for filter data objects
    /// </summary>
    public class FilterDataSet : PersistedDomainObjectDataSet
    {
        private readonly string _dataTableName;
        private readonly string _identityColumnName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectTypeName"></param>
        /// <param name="dataTableName"></param>
        /// <param name="identityColumnName"></param>
        public FilterDataSet(string objectTypeName, string dataTableName, string identityColumnName)
            : base(objectTypeName)
        {
            _dataTableName = dataTableName;
            _identityColumnName = identityColumnName;
        }

        /// <summary>
        /// Get data table name
        /// </summary>
        public override string DataTableName { get { return _dataTableName; } }

        /// <summary>
        /// Get identity column name
        /// </summary>
        public override string IdentityColumnName { get { return _identityColumnName; } }
    }
}
