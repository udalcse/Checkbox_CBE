using System;
using System.Drawing;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class ANALYSIS_HEAT_MAP_SUMMARY : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Gets the appearance data.
        /// </summary>
        /// <value>
        /// The appearance data.
        /// </value>
        private HeatMapSummaryAppearanceData HeatMapAppearanceData
            => this.AppearanceData as HeatMapSummaryAppearanceData;

        /// <summary>
        /// Initialize control and children
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _font.Text = this.HeatMapAppearanceData.FontFamily;
            _titleSize.Text = this.HeatMapAppearanceData.LableFontSize.ToString();
            _gridLine.Checked = this.HeatMapAppearanceData.GridLine;
            _respondentLabels.Checked = this.HeatMapAppearanceData.RespondentLabels;
            Utilities.BindList(_titleSize, Utilities.GetFontSizeListItems(8, 36, 1, " pt"), HeatMapAppearanceData.LableFontSize.ToString());
            Utilities.BindList(_font, Utilities.GetFontFamilyNameListItems(FontStyle.Bold, FontStyle.Regular), HeatMapAppearanceData.FontFamily);
        }

        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            HeatMapAppearanceData.FontFamily = _font.Text;
            HeatMapAppearanceData.LableFontSize = int.Parse(_titleSize.Text);
            HeatMapAppearanceData.GridLine = _gridLine.Checked;
            HeatMapAppearanceData.RespondentLabels = _respondentLabels.Checked;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }
    }
}