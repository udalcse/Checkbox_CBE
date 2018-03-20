using System;
using System.Web.UI.DataVisualization.Charting;

namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Border : AverageScoreStyleUserControl
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
            int width, radius;
            Int32.TryParse(_lineWidth.Text, out width);
            Int32.TryParse(_borderRadius.Text, out radius);

            Appearance.BorderLineWidth = width;
            Appearance.BorderRadius = radius;
            Appearance.BorderLineColor = _lineColor.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _lineWidth.Text = Appearance.BorderLineWidth.ToString();
            _borderRadius.Text = Appearance.BorderRadius.ToString();
            _lineColor.Text = Appearance.BorderLineColor;
        }
    }
}