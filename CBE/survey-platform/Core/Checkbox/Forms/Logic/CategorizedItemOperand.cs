using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Categorized Matrix item contains all functionality of a standard MatrixItem as well as additional functionality
    /// requested by the Greater Wellington Regional Council. The Categorized Matrix Item supports both categorizations
    /// of options and Row Select item types.
    /// </summary>
    [Serializable]
    public class CategorizedItemOperand : Operand
    {
        private int ItemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="columnIndex"></param>
        /// <param name="category"></param>
        public CategorizedItemOperand(int itemId, int columnIndex, String category)
        {
            ItemId = itemId;
            ColumnIndex = columnIndex;
            Category = category;
        }

        protected override object GetValue(Response response)
        {
            return GetResponseCountByColumnAndCategory(response);
        }

        /// <summary>
        /// Returns the number of answered item options in a given column which have the specified category.
        /// Rows what are not enabled are excluded.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private int GetResponseCountByColumnAndCategory(Response response)
        {
            int result = 0;

            CategorizedMatrixItem categorizedMatrix = response.GetItem(ItemId) as CategorizedMatrixItem;

            foreach (MatrixRowInfo matrixRowInfo in categorizedMatrix.ListRows())
            {
                Item item = categorizedMatrix.GetItemAt(matrixRowInfo.RowNumber, ColumnIndex);
                if (item != null && !item.Excluded && item is SelectItem)
                {
                    foreach (ListOption option in ((SelectItem)item).Options)
                    {
                        if (option.IsSelected && Category.Equals(option.Category))
                            result++;
                    }
                }
            }

            return result;
        }
    }
}
