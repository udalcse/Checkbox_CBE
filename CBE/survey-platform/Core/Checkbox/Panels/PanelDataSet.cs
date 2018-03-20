using Checkbox.Common;

namespace Checkbox.Panels
{
    /// <summary>
    /// Data set container for invitation panel data
    /// </summary>
    public class PanelDataSet : PersistedDomainObjectDataSet
    {
        /// <summary>
        /// Object type name for panel data
        /// </summary>
        /// <param name="objectTypeName"></param>
        public PanelDataSet(string objectTypeName)
            : base(objectTypeName)
        {
        }

        /// <summary>
        /// Get panel data
        /// </summary>
        public override string DataTableName { get { return "PanelData"; } }

        /// <summary>
        /// Get identity column for panel data
        /// </summary>
        public override string IdentityColumnName { get { return "PanelId"; } }
    }
}
