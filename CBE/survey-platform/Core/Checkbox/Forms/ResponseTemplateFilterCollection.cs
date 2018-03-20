using System;

using Checkbox.Analytics.Filters;
using Checkbox.Analytics.Filters.Configuration;

using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// Collection of report filters associated with a response template.
    /// </summary>
    [Serializable]
    public class ResponseTemplateFilterCollection : FilterDataCollection
    {
        /// <summary>
        /// Get type of object filters are associated with.
        /// </summary>
        public override string ParentType
        {
            get { return "ResponseTemplate";}
            set {;}
        }

        /// <summary>
        /// Delete a filter
        /// </summary>
        /// <param name="filter"></param>
        public override void DeleteFilter(FilterData filter)
        {
            base.DeleteFilter(filter);

            //Save the maps to avoid an FK error
            Database db = DatabaseFactory.CreateDatabase();
            
            db.UpdateDataSet(
                FilterData,
                FilterMapTableName,
                GetInsertFilterMapCommand(db),
                null,
                GetDeleteFilterMapCommand(db),
                UpdateBehavior.Standard);

            //Delete the filter
            filter.Delete();
        }
    }
}
