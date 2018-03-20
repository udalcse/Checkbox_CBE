using System;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    /// <summary>
    /// Options editor for summary table
    /// </summary>
    public partial class MatrixSummaryOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(AnalysisItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update item data with selected options.
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateItemData(AnalysisItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}