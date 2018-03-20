using System;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    /// <summary>
    /// Options editor for summary table
    /// </summary>
    public partial class SummaryTableOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(AnalysisItemData itemData)
        {
            _multiOptionsPlace.Enabled = itemData.SourceItemIds.Count > 1;
            _aliasesChk.Checked = itemData.UseAliases;

            if (itemData is FrequencyItemData)
            {
                if (_otherOptionList.Items.FindByValue(((FrequencyItemData)itemData).OtherOption.ToString()) != null)
                {
                    _otherOptionList.SelectedValue = ((FrequencyItemData)itemData).OtherOption.ToString();
                }
            }
        }

        /// <summary>
        /// Update item data with selected options.
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateItemData(AnalysisItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;

            if (itemData is FrequencyItemData)
            {
                ((FrequencyItemData)itemData).OtherOption = (OtherOption)Enum.Parse(typeof(OtherOption), _otherOptionList.SelectedValue);
            }
        }
    }
}