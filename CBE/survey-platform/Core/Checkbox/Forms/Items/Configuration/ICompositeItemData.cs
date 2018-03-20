using System;
using System.Collections.ObjectModel;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Interface for item data that contain child item datas
    /// </summary>
    public interface ICompositeItemData
    {
        /// <summary>
        /// Get the ids of any child item data objects.
        /// </summary>
        /// <returns></returns>
        ReadOnlyCollection<Int32> GetChildItemDataIDs();
    }
}
