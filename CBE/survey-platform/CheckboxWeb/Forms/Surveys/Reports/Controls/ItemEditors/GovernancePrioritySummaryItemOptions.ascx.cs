using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class GovernancePrioritySummaryItemOptions : UserControlBase
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(GovernancePriorityGraphData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(GovernancePriorityGraphData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}