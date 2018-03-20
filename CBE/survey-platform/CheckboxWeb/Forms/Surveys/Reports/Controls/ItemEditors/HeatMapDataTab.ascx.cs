using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Forms.Data;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class HeatMapDataTab : Checkbox.Web.Common.UserControlBase
    {
        public Dictionary<int, string> AllSections { get; set; }

        public Dictionary<int, double> ESigmaValues { get; set; }

        public List<int> SelectedItems { get; set; }

        public int ResponseTemplateId { get; private set; }
        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(HeatMapData itemData, int sourceResponseTemplateId, List<int> selectedItems, Dictionary<int, string> sections)
        {
            ResponseTemplateId = sourceResponseTemplateId;
            AllSections = sections;
            SelectedItems = selectedItems;
            _userMeanValues.Checked = itemData.UseMeanValues;
            _randomizeResponses.Checked = itemData.RandomizeResponses;
            ESigmaValues = itemData.SigmaValues;

            BuildItemLists();
        }

        /// <summary>
        /// Update item data with selected options.
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateItemData(HeatMapData itemData, List<int> selectedItems)
        {
            ESigmaValues = itemData.SigmaValues;
            SelectedItems = selectedItems;
            itemData.UseMeanValues = _userMeanValues.Checked;
            itemData.RandomizeResponses = _randomizeResponses.Checked;

            if (itemData.SigmaValues == null)
            {
                itemData.SigmaValues = new Dictionary<int, double>();
            }

            double sectionValue;
            int sectionId;

            foreach (ListViewItem row in _selectedSections.Items)
            {
                sectionId = int.Parse(((HiddenField)row.FindControl("sectionId")).Value);

                if (double.TryParse(((TextBox)row.FindControl("sectionValue")).Text, out sectionValue))
                {
                    if (itemData.SigmaValues.ContainsKey(sectionId))
                    {
                        itemData.SigmaValues[sectionId] = sectionValue;
                    }
                    else
                    {
                        itemData.SigmaValues.Add(sectionId, sectionValue);
                    }
                }
            }

            BuildItemLists();
        }

        private void BuildItemLists()
        {
            var allItems = ItemConfigurationManager.ListResponseTemplateItems(
                ResponseTemplateId,
                null,
                true,
                true,
                true,
                WebTextManager.GetUserLanguage());

            var selectedSections = FilterOnlySelectedSections(allItems);

            var mappedSections = selectedSections.Select(s => new KeyValuePair<int, KeyValuePair<string, string>>(s.Key, 
                new KeyValuePair<string, string>(s.Value, ESigmaValues.ContainsKey(s.Key) ? Math.Round(ESigmaValues[s.Key], 2).ToString() : string.Empty)));
            
            _selectedSections.DataSource = mappedSections;
            _selectedSections.DataBind();
        }

        private List<KeyValuePair<int, string>> FilterOnlySelectedSections(List<LightweightItemMetaData> allItems)
        {
            allItems = allItems.OrderBy(data => data.PagePosition)
                    .ThenBy(data => data.ItemPosition).ToList();
            var sectionIds = AllSections.Select(s => s.Key).ToList();
            var onlySections = allItems.Where(s => sectionIds.Contains(s.ItemId));
            var onlySelectedItems = allItems.Where(s => SelectedItems.Contains(s.ItemId));

            allItems.Reverse();

            var sectionsWithSelectedElements = new List<int>();

            foreach (var item in onlySelectedItems)
            {
                var sectionInList = allItems.FirstOrDefault(s => s.ItemPosition < item.ItemPosition && s.PagePosition == item.PagePosition && sectionIds.Contains(s.ItemId));
                if (sectionInList != null)
                {
                    sectionsWithSelectedElements.Add(sectionInList.ItemId);
                }
            }

            return AllSections.Where(s => sectionsWithSelectedElements.Contains(s.Key)).ToList();
        }

    }
}