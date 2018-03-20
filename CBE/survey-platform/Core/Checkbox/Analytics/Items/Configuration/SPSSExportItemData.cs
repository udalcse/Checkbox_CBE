using System;

using Checkbox.Forms.Items;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Item data for spss export
    /// </summary>
    [Serializable]
    public class SPSSExportItemData : ExportItemData
    {
        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new SpssExportItem();
        }
    }
}
