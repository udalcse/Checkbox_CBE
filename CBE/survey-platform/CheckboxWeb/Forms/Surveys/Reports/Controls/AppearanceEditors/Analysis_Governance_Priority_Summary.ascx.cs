using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    /// <summary>
    /// Analysis_Governance_Priority_Summary
    /// </summary>
    /// <seealso cref="Checkbox.Web.Forms.UI.Editing.UserControlAppearanceEditorBase" />
    public partial class Analysis_Governance_Priority_Summary : UserControlAppearanceEditorBase
    {

        /// <summary>
        /// Gets the appearance data.
        /// </summary>
        /// <value>
        /// The appearance data.
        /// </value>
        private GovernancePrioritySummaryAppearanceData GovernancAppearanceData
            => this.AppearanceData as GovernancePrioritySummaryAppearanceData;

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _barColor.Text = GovernancAppearanceData.BarColor;
            _gridLine.Checked = GovernancAppearanceData.GridLine;
            _showValuesOnBars.Checked = GovernancAppearanceData.ShowValuesOnBars;
            _titleSize.Text = GovernancAppearanceData.TitleFontSize.ToString();
            Utilities.BindList(_titleSize, Utilities.GetFontSizeListItems(8, 36, 1, " pt"), GovernancAppearanceData.TitleFontSize.ToString());
            Utilities.BindList(_font, Utilities.GetFontFamilyNameListItems(FontStyle.Bold, FontStyle.Regular), GovernancAppearanceData.FontFamily);
        }


        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            GovernancAppearanceData.BarColor = this._barColor.Text;
            GovernancAppearanceData.GridLine = _gridLine.Checked;
            GovernancAppearanceData.ShowValuesOnBars = _showValuesOnBars.Checked;
            GovernancAppearanceData.TitleFontSize = int.Parse(_titleSize.Text.Replace("pt",string.Empty));
            GovernancAppearanceData.FontFamily = _font.Text;
        }
    }
}