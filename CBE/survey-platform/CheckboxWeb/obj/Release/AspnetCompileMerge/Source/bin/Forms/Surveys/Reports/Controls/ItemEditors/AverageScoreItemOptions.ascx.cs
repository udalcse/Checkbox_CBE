using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class AverageScoreItemOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(AverageScoreItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(AverageScoreItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}