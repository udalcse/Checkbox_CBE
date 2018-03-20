using System;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Web.Charts;
using System.Drawing;

namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Options : AverageScoreStyleUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load color picker script
            RegisterClientScriptInclude(
                "colorPicker.js",
                ResolveUrl("~/Resources/mColorPicker.min.js"));

            
            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.BarColor = _selectedBarColor.Text;
            Appearance.Animation = _animation.Checked;
            Appearance.ShowTitle = _showTitle.Checked;
            Appearance.BackgroundColor = _backgroundColor.Text;
            Appearance.PlotAreaBackgroundColor = _plotBackgroundColor.Text;
            Appearance.LegendBackgroundColor = _legendBackgroundColor.Text;
            Appearance.PieBorderColor = _pieBorderColor.Text;

			Appearance.Width = int.Parse(_widthTxt.Text);
			Appearance.Height = int.Parse(_heightTxt.Text);

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _selectedBarColor.Text = Appearance.BarColor;
            _animation.Checked = Appearance.Animation;
            _showTitle.Checked = Appearance.ShowTitle;
            _backgroundColor.Text = Appearance.BackgroundColor;
            _plotBackgroundColor.Text = Appearance.PlotAreaBackgroundColor;
            _legendBackgroundColor.Text = Appearance.LegendBackgroundColor;
            _pieBorderColor.Text = Appearance.PieBorderColor;

			_widthTxt.Text = Appearance.Width.ToString();
			_heightTxt.Text = Appearance.Height.ToString();

        }
    }
}