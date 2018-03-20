using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Items;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class StatisticsTableOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(StatisticsItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
            _calculationOption.SelectedValue = itemData.ReportOption.ToString();
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(StatisticsItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
            itemData.ReportOption = (StatisticsItemReportingOption)Enum.Parse(typeof(StatisticsItemReportingOption), _calculationOption.SelectedValue);
        }
    }
}