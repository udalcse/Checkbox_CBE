using System;
using System.Web.UI.DataVisualization.Charting;
using System.Linq;
using Checkbox.Web.Charts;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    public partial class Other : ChartStyleUserControl
    {
        /// <summary>
        /// Show hatching settings
        /// </summary>
        public bool ShowHatchingSettings { get; set; }

        /// <summary>
        /// Set autopostback property of 3d check
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //If settings not forced on, postback on change so settings can be enabled/disabled
            // depending 3d check.
            _show3d.AutoPostBack = !Force3DSettingsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                Enable3dSettings();
            }

            _hatchContainer.Visible = !_show3d.Checked;

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.ShowPercent = _showPercent.Checked;
            Appearance.ShowAnswerCount = _showAnswerCount.Checked;
            Appearance.ShowDataLabelZeroValues = _includeAllAnswers.Checked;
            Appearance.AllowExporting = _allowExporting.Checked;
            Appearance.HatchStyle = (ChartHatchStyle)Enum.Parse(typeof(ChartHatchStyle), _hatchStyle.SelectedValue);
            Appearance.OptionsOrder = (OptionsOrder)Enum.Parse(typeof(OptionsOrder), _optionsOrder.SelectedValue);
            Appearance.HatchBackgroundColor = _hatchColor.Text;
            Appearance.Enable3D = _show3d.Checked;
            Appearance.XAngle = Int32.Parse(_xAngle.Text);
            Appearance.YAngle = Int32.Parse(_yAngle.Text);
            Appearance.Perspective = Int32.Parse(_perspective.Text);
            Appearance.Transparency = Int32.Parse(_opacity.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _hatchStyle.Items.Clear();

            _showPercent.Checked = Appearance.ShowPercent;
            _showAnswerCount.Checked = Appearance.ShowAnswerCount;
            _includeAllAnswers.Checked = Appearance.ShowDataLabelZeroValues;
            _allowExporting.Checked = Appearance.AllowExporting;
            
            BindList(_hatchStyle, GetEnumListItems(typeof(ChartHatchStyle)), Appearance.HatchStyle.ToString());

            _optionsOrder.SelectedIndex = Appearance.OptionsOrder == OptionsOrder.Default ? 1 : 0;

            _hatchColor.Text = Appearance.HatchBackgroundColor;
            _show3d.Checked = Appearance.Enable3D;
            _xAngle.Text = Appearance.XAngle.ToString();
            _yAngle.Text = Appearance.YAngle.ToString();
            _perspective.Text = Appearance.Perspective.ToString();
            _opacity.Text = Appearance.Transparency.ToString();

            Enable3dSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Enable3dSettings()
        {
            xAngleLbl.Enabled = _show3d.Checked || Force3DSettingsEnabled;
            _xAngle.Enabled = _show3d.Checked || Force3DSettingsEnabled;

            yAngleLbl.Enabled = _show3d.Checked || Force3DSettingsEnabled;
            _yAngle.Enabled = _show3d.Checked || Force3DSettingsEnabled;

            perspectiveLbl.Enabled = _show3d.Checked || Force3DSettingsEnabled;
            _perspective.Enabled = _show3d.Checked || Force3DSettingsEnabled;

            //opacityLbl.Enabled = _show3d.Checked || Force3DSettingsEnabled;
            //_opacity.Enabled = _show3d.Checked || Force3DSettingsEnabled;

        }
    }
}
