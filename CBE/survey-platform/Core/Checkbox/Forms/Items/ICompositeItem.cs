using System;
using System.Collections.ObjectModel;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Interface definition for items comprised of other items.
    /// </summary>
    public interface ICompositeItem
    {
        /// <summary>
        /// Get a read-only collection of the item's children.
        /// </summary>
        ReadOnlyCollection<Item> Items
        {
            get;
        }
    }
}
