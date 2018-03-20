using System;
using System.Web.UI.DataVisualization.Charting;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Border : ChartStyleUserControl
    {

        /// <summary>
        /// 
        /// </summary>
        public bool ShowBorderFrameStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowBorderLineStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowFrameBgColor
        {
            get;
            set;
        }

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
            Appearance.BorderSkinStyle = (BorderSkinStyle)Enum.Parse(typeof(BorderSkinStyle), _borderStyle.SelectedValue);
            Appearance.BorderLineStyle = (ChartDashStyle)Enum.Parse(typeof(ChartDashStyle), _lineStyle.SelectedValue);
            Appearance.BorderLineWidth = Int32.Parse(_lineWidth.Text);
            Appearance.BorderRadius = Int32.Parse(_borderRadius.Text);
            Appearance.BorderFrameBackgroundColor = _frameBgColor.Text;
            Appearance.BorderLineColor = _lineColor.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _borderStyle.Items.Clear();
            _lineStyle.Items.Clear();

            BindList(_borderStyle, GetEnumListItems(typeof(BorderSkinStyle)), Appearance.BorderSkinStyle.ToString());
            BindList(_lineStyle, GetEnumListItems(typeof(ChartDashStyle)), Appearance.BorderLineStyle.ToString());
            
            _lineWidth.Text = Appearance.BorderLineWidth.ToString();
            _borderRadius.Text = Appearance.BorderRadius.ToString();
            _frameBgColor.Text = Appearance.BorderFrameBackgroundColor;
            _lineColor.Text = Appearance.BorderLineColor;
        }
    }
}