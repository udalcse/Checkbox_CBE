using System;
using Checkbox.Forms.Items;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Item data for on-the-fly CSV Exporting
    /// </summary>
    [Serializable]
    public class CSVExportItemData : ExportItemData
    {
        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CSVExportItem();
        }
    }
}
