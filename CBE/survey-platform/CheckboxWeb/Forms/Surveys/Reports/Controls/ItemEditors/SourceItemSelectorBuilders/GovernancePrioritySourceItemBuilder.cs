using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;
using Checkbox.Forms;
using Checkbox.Forms.Data;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders
{
    public class GovernancePrioritySourceItemBuilder : ISourceItemBuilder
    {
        public List<LightweightItemMetaData> Build(int responseTemplateId , List<LightweightItemMetaData> items, List<int> selectedItems, List<string> allowedItemTypes, List<string> notAllowedItemTypes)
        {
            //var responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            var sectionHeaders = ResponseTemplateManager.GetResponseTemplateSectionsIds(responseTemplateId);
            var template = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            var contentPages = GetContentPages(template, sectionHeaders);
            ClearNotSectionItems(contentPages, sectionHeaders);

            List<int> allSectionItems = new List<int>();

            foreach (var page in contentPages.Keys)
                allSectionItems.AddRange(contentPages[page]);

            //filter and take only rating scale and net promoter scores items 
            items = 
                items.Where(
                    item =>
                        item.ItemType.ToLower().Equals("radiobuttonscale")).ToList();

            // if all section items does not contain item from items collection , remove it 
            foreach (var item in items.ToList())
            {
                if (!allSectionItems.Contains(item.ItemId))
                    items.Remove(item);
            }


            //Build list of non-selected items
            return items
                .Where(data => !selectedItems.Contains(data.ItemId) && (allowedItemTypes == null || allowedItemTypes.Count == 0 || allowedItemTypes.Contains(data.ItemType))
                        && (notAllowedItemTypes == null || !notAllowedItemTypes.Contains(data.ItemType)))
                .ToList();
        }

        /// <summary>
        /// Gets the content pages.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="sectionHeaders">The section headers.</param>
        /// <returns></returns>
        private Dictionary<int, List<int>> GetContentPages(ResponseTemplate template, Dictionary<int,string> sectionHeaders )
        {
            var result = new Dictionary<int, List<int>>();

            foreach (var key in template.TemplatePages.Keys)
            {
                var page = template.TemplatePages[key];

                // if page has secton item , add this page to filtered list of data
                foreach (var sectionHeader in sectionHeaders.Keys)
                {
                    if (page.ListItemIds().Contains(sectionHeader) && page.PageType == TemplatePageType.ContentPage)
                    {
                        result.Add(page.ID.Value, page.ListItemIds().ToList());
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the not section items.
        /// </summary>
        /// <param name="itemPages">The item pages.</param>
        /// <param name="sectionHeaders">The section headers.</param>
        private void ClearNotSectionItems(Dictionary<int, List<int>> itemPages, Dictionary<int, string> sectionHeaders)
        {
            foreach (var key in itemPages.Keys)
            {
                foreach (var itemId in itemPages[key].ToList())
                {
                    if (!sectionHeaders.Keys.Contains(itemId))
                        itemPages[key].Remove(itemId);
                    else
                        break;
                }
            }
        }
    }
}