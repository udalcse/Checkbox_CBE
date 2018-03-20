using System;


namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AverageScoreMargins : AverageScoreStyleUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.ChartMarginTop = Int32.Parse(_chartTopMargin.Text);
            Appearance.ChartMarginBottom = Int32.Parse(_chartBottomMargin.Text);
            Appearance.ChartMarginRight = Int32.Parse(_chartRightMargin.Text);
            Appearance.ChartMarginLeft = Int32.Parse(_chartLeftMargin.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _chartTopMargin.Text = Appearance.ChartMarginTop.ToString();
            _chartBottomMargin.Text = Appearance.ChartMarginBottom.ToString();
            _chartRightMargin.Text = Appearance.ChartMarginRight.ToString();
            _chartLeftMargin.Text = Appearance.ChartMarginLeft.ToString();
        }
    }
}