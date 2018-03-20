using System;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class SummaryChartBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(AnalysisItemData itemData)
        {
            _multiOptionsPlace.Enabled = itemData.SourceItemIds.Count > 0;
            _aliasesChk.Checked = itemData.UseAliases;            

            if (itemData is FrequencyItemData)
            {
                if (_otherOptionList.Items.FindByValue(((FrequencyItemData)itemData).OtherOption.ToString()) != null)
                {
                    _otherOptionList.SelectedValue = ((FrequencyItemData)itemData).OtherOption.ToString();
                }
                _displayStatistics.Checked = ((FrequencyItemData)itemData).DisplayStatistics;
                _displayAnswers.Checked = ((FrequencyItemData)itemData).DisplayAnswers;
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
                ((FrequencyItemData)itemData).DisplayStatistics = _displayStatistics.Checked;
                ((FrequencyItemData)itemData).DisplayAnswers = _displayAnswers.Checked;
            }
        }
    }
}