using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Data container for item appearance
    /// </summary>
    public class AppearanceDataSet : AbstractPersistedDomainObjectDataSet
    {
        /// <summary>
        /// Get name of properties table
        /// </summary>
        protected const string PropertiesTableName = "Properties";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owningObjectType"></param>
        public AppearanceDataSet(string owningObjectType)
            : base(owningObjectType, "AppearanceData", "AppearanceId", PropertiesTableName)
        {
        }

        /// <summary>
        /// Get name of parent data talbe
        /// </summary>
        public override string ParentDataTableName { get { return "AppearanceData"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="owningObjectId"></param>
        /// <returns></returns>
        protected override DBCommandWrapper CreateAbstractDataCommand(Database db, int owningObjectId)
        {
            return null;
        }

        /// <summary>
        /// Get property rows
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetPropertyRows()
        {
            return Tables[PropertiesTableName].Select();
        }
    }
}
