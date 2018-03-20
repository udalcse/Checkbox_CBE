using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    public partial class Legend : ChartStyleUserControl
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
            Appearance.correctChartMargins();
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