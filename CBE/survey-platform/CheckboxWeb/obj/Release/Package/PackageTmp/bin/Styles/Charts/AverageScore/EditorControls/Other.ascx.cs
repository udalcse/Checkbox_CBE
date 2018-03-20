using System;
using System.Web.UI.DataVisualization.Charting;
using Checkbox.Web.Charts;
using CheckboxWeb.Styles.Charts.EditorControls;

namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    public partial class Other : AverageScoreStyleUserControl
    {
        /// <summary>
        /// Show hatching settings
        /// </summary>
        public bool ShowHatchingSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.ShowAnswerCount = _showAnswerCount.Checked;
            Appearance.AllowExporting = _allowExporting.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _showAnswerCount.Checked = Appearance.ShowAnswerCount;
            _allowExporting.Checked = Appearance.AllowExporting;
        }
    }
}
