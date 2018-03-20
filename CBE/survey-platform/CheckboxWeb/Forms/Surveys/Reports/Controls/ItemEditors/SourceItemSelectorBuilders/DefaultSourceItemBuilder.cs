using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms.Data;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders
{
    public class DefaultSourceItemBuilder : ISourceItemBuilder
    {
        /// <summary>
        /// Builds the specified response template identifier.
        /// </summary>
        /// <param name="responseTemplateId">The response template identifier.</param>
        /// <param name="items">The items.</param>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="allowedItemTypes">The allowed item types.</param>
        /// <param name="notAllowedItemTypes">The not allowed item types.</param>
        /// <returns></returns>
        public List<LightweightItemMetaData> Build(int responseTemplateId, List<LightweightItemMetaData> items, List<int> selectedItems, List<string> allowedItemTypes, List<string> notAllowedItemTypes)
        {
        
            //Build list of non-selected items
            return items
                .Where(data => !selectedItems.Contains(data.ItemId) && (allowedItemTypes == null || allowedItemTypes.Count == 0 || allowedItemTypes.Contains(data.ItemType))
                        && (notAllowedItemTypes == null || !notAllowedItemTypes.Contains(data.ItemType)))
                .ToList();

        }
    }
}