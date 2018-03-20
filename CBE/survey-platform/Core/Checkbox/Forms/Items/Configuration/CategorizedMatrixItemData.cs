using System;
using System.Collections.Generic;
using System.Data;

using Checkbox.Common;
using Checkbox.Forms.Items.UI;

using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Class to represent categorized matrix item data
    /// </summary>
    [Serializable]
    public class CategorizedMatrixItemData : MatrixItemData
    {
        /// <summary>
        /// Create an instance of a matrix item based on this item type
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CategorizedMatrixItem();
        }

        /// <summary>
        /// Create a text decorator for an item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new CategorizedMatrixItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// Returns the list of column indexes for all questions which can be categorized
        /// </summary>
        /// <returns></returns>
        public List<int> GetCategorizedColumnIndexes()
        {
            List<int> indexes = new List<int>();

            for (int columnIndex = 1; columnIndex <= ColumnCount; columnIndex++)
            {
                if (columnIndex != PrimaryKeyColumnIndex)
                {
                    ItemData itemData = ItemConfigurationManager.GetConfigurationData(GetColumnPrototypeId(columnIndex));
                    if (itemData is CategorizedSelectOneData || itemData is CategorizedSelectManyData)
                    {
                        indexes.Add(columnIndex);
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Returns the distinct list of categories contained within a specified column.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDistinctCategoriesByColumn(int columnIndex)
        {
            List<string> categories = new List<string>();

            if (columnIndex != PrimaryKeyColumnIndex)
            {
                ItemData item = ItemConfigurationManager.GetConfigurationData(GetColumnPrototypeId(columnIndex));

                if (item is ICategorizedItemData)
                {
                    foreach (ListOptionData option in ((SelectItemData)item).Options)
                    {
                        string category = option.Category;
                        if (Utilities.IsNotNullOrEmpty(category))
                        {
                            if (!categories.Contains(category))
                            {
                                categories.Add(category);
                            }
                        }
                    }
                }                
            }

            return categories;
        }
        
    }
}