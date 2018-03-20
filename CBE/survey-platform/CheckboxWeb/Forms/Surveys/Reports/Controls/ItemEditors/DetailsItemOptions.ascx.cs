using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    /// <summary>
    /// Editor for details item options
    /// </summary>
    public partial class DetailsItemOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(DetailsItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
            _groupAnswersChk.Checked = itemData.GroupAnswers;
            _linkChk.Checked = itemData.LinkToResponseDetails;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(DetailsItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
            itemData.GroupAnswers = _groupAnswersChk.Checked;
            itemData.LinkToResponseDetails = _linkChk.Checked;
        }
    }
}