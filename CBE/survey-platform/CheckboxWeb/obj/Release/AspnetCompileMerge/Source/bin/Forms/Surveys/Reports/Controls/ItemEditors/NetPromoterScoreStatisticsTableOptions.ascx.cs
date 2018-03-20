using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class NetPromoterScoreStatisticsTableOptions : UserControlBase
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(NetPromoterScoreStatisticsItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(NetPromoterScoreStatisticsItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}