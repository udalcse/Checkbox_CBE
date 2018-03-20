using CheckboxWeb.Styles.Charts.EditorControls;

namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    public partial class Legend : AverageScoreStyleUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.ShowLegend = _showLegend.Checked;
            Appearance.LegendLayout = _legendLayout.SelectedValue;
            Appearance.LegendAlign = _legendAlign.SelectedValue;
            Appearance.LegendVerticalAlign = _legendVerticalAlign.SelectedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _showLegend.Checked = Appearance.ShowLegend;
            _legendLayout.SelectedValue = Appearance.LegendLayout;
            _legendAlign.SelectedValue = Appearance.LegendAlign;
            _legendVerticalAlign.SelectedValue = Appearance.LegendVerticalAlign;
        }
    }
}