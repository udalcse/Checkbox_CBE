using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Charts;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportItemEditor : Checkbox.Web.Common.UserControlBase
    {
        private int _sourceItemsCount;
        private string _legendAlignment;
        private string _legendVerticalAlignment;
        
        /// <summary>
        /// Get item data edited
        /// </summary>
        public ItemData ItemData { get; private set; }

        /// <summary>
        /// Get/set appearance data
        /// </summary>
        public AppearanceData AppearanceData { get; private set; }

        /// <summary>
        /// Item editor control
        /// </summary>
        private IItemEditor ItemEditor { get; set; }

        /// <summary>
        /// Appearance editor control
        /// </summary>
        private IAppearanceEditor AppearanceEditor { get; set; }

        /// <summary>
        /// Initialize item with item and appearance data.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="itemData"></param>
        /// <param name="appearanceData"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="reportId"></param>
        /// <param name="hidePreview"></param>
        public void Initialize(int reportId, int pagePosition, ItemData itemData, AppearanceData appearanceData, string currentLanguage, bool isPagePostBack, bool hidePreview)
        {
            //Store inputs
            ItemData = itemData;
            AppearanceData = appearanceData;

            //save some orinal values to calculate automargin values on update
            SummaryChartItemAppearanceData chartAppearanceData = appearanceData as SummaryChartItemAppearanceData;
            FrequencyItemData frequencyItemData = itemData as FrequencyItemData;
            if (chartAppearanceData != null && frequencyItemData != null)
            {
                _sourceItemsCount = frequencyItemData.SourceItemIds.Count;
                _legendAlignment = chartAppearanceData.LegendAlign;
                _legendVerticalAlignment = chartAppearanceData.LegendVerticalAlign;
            }

            //Check values
            if (itemData == null)
            {
                throw new Exception("Report item editor was initialized with NULL item data.");
            }

            //Get an editor for the item data
            ItemEditor = ItemEditorFactory.CreateEditor(itemData.ItemTypeName);

            if (ItemEditor == null)
            {
                throw new Exception("Unable to create item editor for item type: " + itemData.ItemTypeName);
            }

            if (!(ItemEditor is Control))
            {
                throw new Exception("Item editor type: " + ItemEditor.GetType() + " is not a control and can't be added to report item editor control collection.");
            }

            //special case: we need to allow selecting the primary source item for 3 types of graphs
            if (itemData is FrequencyItemData)
            {
                var fid = (FrequencyItemData)itemData;
                if (!fid.PrimarySourceItemID.HasValue)
                {
                    //check the appearance
                    if ("ColumnGraph".Equals(appearanceData["GraphType"], StringComparison.CurrentCultureIgnoreCase)
                        || "LineGraph".Equals(appearanceData["GraphType"], StringComparison.CurrentCultureIgnoreCase)
                        || "BarGraph".Equals(appearanceData["GraphType"], StringComparison.CurrentCultureIgnoreCase))
                        fid.PrimarySourceItemID = 0; //set non-empty value                        
                }
            }

            _itemEditorPlace.Controls.Add((Control)ItemEditor);
            ItemEditor.Initialize(reportId, pagePosition, itemData, currentLanguage, new List<string>{currentLanguage}, isPagePostBack, EditMode.Report, hidePreview);

            //Get an editor for appearance data, if any
            if (appearanceData == null)
            {
                return;
            }

            //Not all items have appearances, so it's ok if there is no appearance editor
            AppearanceEditor = AppearanceEditorFactory.CreateEditor(appearanceData.AppearanceCode);

            if (AppearanceEditor == null)
            {
                _appearanceEditorPlace.Visible = false;
                return;
            }

            //Initialize
            AppearanceEditor.Initialize(appearanceData);

            //Add to appropriate place in control hierarchy
            if (ItemEditor.SupportsEmbeddedAppearanceEditor)
            {
                ItemEditor.EmbedAppearanceEditor(AppearanceEditor);
                _appearanceEditorPlace.Visible = false;
            }
            else
            {
                _appearanceEditorPlace.Visible = true;

                if (!(AppearanceEditor is Control))
                {
                    throw new Exception("Appearance editor type: " + AppearanceEditor.GetType() + " is not a control and can't be added to report item editor control collection.");
                }

                _appearanceEditorPlace.Controls.Add((Control)AppearanceEditor);

            }
        }

        /// <summary>
        /// Validate editor
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //Validate item and appearance data. If appearance editor is null, simply
            // assign a true value for appearance editor validation.
            return ItemEditor.Validate() && (AppearanceEditor == null || AppearanceEditor.Validate());
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="isNew"> </param>
        public void UpdateData(bool isNew)
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.UpdateData();
                AppearanceDataManager.CleanUpAppearanceDataCacheForItem(ItemData.ID.Value);
            }

            ItemEditor.UpdateData();

            if (isNew)
                AdjustAutoMargin();
        }

        /// <summary>
        /// Save data
        /// </summary>
        public void SaveData()
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.SaveData();
                AppearanceDataManager.CleanUpAppearanceDataCacheForItem(ItemData.ID.Value);
            }

            ItemEditor.SaveData();
        }

        private void AdjustAutoMargin()
        {
            SummaryChartItemAppearanceData chartAppearanceData = AppearanceData as SummaryChartItemAppearanceData;
            AnalysisItemData itemData = ItemData as AnalysisItemData;
            FrequencyItemData itemFreqData = ItemData as FrequencyItemData;
            if (chartAppearanceData != null && itemData != null)
            {
                if (_sourceItemsCount != itemData.SourceItemIds.Count)
                {
                    chartAppearanceData.AdjustTopMarginForTitle(itemData.SourceItemIds.Count);
                    chartAppearanceData.AdjustAutoMarginValuesForSpecificItems(itemData.SourceItemIds.Count,
                        (itemFreqData==null || itemFreqData.PrimarySourceItemID == null) ? null : itemFreqData.PrimarySourceItemID);
                    chartAppearanceData.UpdateWrapTitleChars();
                }

                chartAppearanceData.AdjustAutoMarginValues(_legendAlignment, _legendVerticalAlignment);

                AppearanceEditor.Initialize(chartAppearanceData);
            }            
        }
    }
}