using System;
using System.Collections.ObjectModel;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Represents an item that contains child items in a table (full or sparse) format.
    /// </summary>
    [Serializable]
    public abstract class TabularItemData : LabelledItemData, ICompositeItemData
    {
        /// <summary>
        /// Get the number of rows for the table
        /// </summary>
        public abstract Int32 RowCount { get; }

        /// <summary>
        /// Get the number of columns for the table
        /// </summary>
        public abstract Int32 ColumnCount { get; }

        /// <summary>
        /// Get the item data for the specific location in the table
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public abstract int? GetItemIdAt(Int32 row, Int32 column);

        /// <summary>
        /// Gets the coordinate position of an item with a table where the x-coordinate is the 
        /// column index of the item and the y-coordinate is the row index
        /// </summary>
        /// <param name="itemId">the id of the item</param>
        /// <returns>the location within the table as an Coordinate</returns>
        public abstract TableCoordinate GetItemCoordinate(int itemId);

        #region ICompositeItemData Members

        /// <summary>
        /// Get the ids of any child item data objects.
        /// </summary>
        /// <returns></returns>
        public abstract ReadOnlyCollection<int> GetChildItemDataIDs();
     
        #endregion
    }
}