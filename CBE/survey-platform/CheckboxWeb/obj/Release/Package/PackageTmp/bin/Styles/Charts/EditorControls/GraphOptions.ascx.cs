using System;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Web.Charts;
using System.Drawing;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GraphOptions : ChartStyleUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _pieGraphColorsPlace.Visible = _colorPalette.SelectedValue == "CUSTOM";

            //Show pie options when pie/doughnut selected, or when specified to always show
            _pieOptionsPanel.Visible =
                ShowPieOptions
                || _graphTypeList.SelectedValue.Equals("PieGraph", StringComparison.InvariantCultureIgnoreCase)
                || _graphTypeList.SelectedValue.Equals("Doughnut", StringComparison.InvariantCultureIgnoreCase);

            //Show bar options for bar/line/column charts or when specified to always show.
            _barOptionsPanel.Visible = ShowBarOptions || !_pieOptionsPanel.Visible;

            //Show doughnut options when doughnut selected or when specified to always show.
            _donutPlace.Visible = ShowDoughnutOptions || _graphTypeList.SelectedValue.Equals("Doughnut", StringComparison.InvariantCultureIgnoreCase);

            _chartTypePanel.Visible = ShowGraphTypeList;

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
            Appearance.DoughnutRadius = Int32.Parse(_donutRadius.Text);
            Appearance.PieGraphColors = UpdatePaletteColors();
            Appearance.Animation = _animation.Checked;
            Appearance.ShowTitle = _showTitle.Checked;
            Appearance.BackgroundColor = _backgroundColor.Text;
            Appearance.PlotAreaBackgroundColor = _plotBackgroundColor.Text;
            Appearance.LegendBackgroundColor = _legendBackgroundColor.Text;
            Appearance.PieBorderColor = _pieBorderColor.Text;

            if (ShowGraphTypeList)
            {
                Appearance["GraphType"] = _graphTypeList.SelectedValue;
            }

			Appearance.Width = int.Parse(_widthTxt.Text);
			Appearance.Height = int.Parse(_heightTxt.Text);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string UpdatePaletteColors()
        {
            string colorCsv = string.Empty;

            if (_colorPalette.SelectedValue == "CUSTOM")
            {
                foreach (string hexColor in _customPalette.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        Utilities.GetColor(hexColor, true);
                        colorCsv += hexColor + ",";
                    }
                    catch 
                    {
                    }
                }
            }
            else
            {
                Color[] paletteColors = ChartPaletteColors.GetColorList(_colorPalette.SelectedValue);

                colorCsv = paletteColors.Aggregate(colorCsv, (current, color) => current + (Utilities.ColorToHex(color) + ","));
            }

            return colorCsv.Remove(colorCsv.Length - 1);
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

            LoadPaletteList();
            _customPalette.Value = Appearance.PieGraphColors;
            _donutRadius.Text = Appearance.DoughnutRadius.ToString();

            if (_graphTypeList.Items.FindByValue(Appearance["GraphType"]) != null)
            {
                _graphTypeList.SelectedValue = Appearance["GraphType"];
            }

			_widthTxt.Text = Appearance.Width.ToString();
			_heightTxt.Text = Appearance.Height.ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadPaletteList()
        {
            string selectedPalette;

            if (Utilities.IsNotNullOrEmpty(Appearance.PieGraphColors))
            {
                BindList(_colorPalette, GetEnumListItems(typeof(ChartColorPalette)), string.Empty);
                selectedPalette = "CUSTOM";
            }
            else
            {
                BindList(_colorPalette, GetEnumListItems(typeof(ChartColorPalette)), string.Empty);
                selectedPalette = "Excel";
            }

            //Remove the "None", "Default", "GrayScale", "Pastel", "Bright", "BrightPastel" palette options
            var itemsToRemove = new List<ListItem>();

            foreach (ListItem item in _colorPalette.Items)
            {
                if (item.Value.Equals("None", StringComparison.InvariantCultureIgnoreCase)
                    || item.Value.Equals("Default", StringComparison.InvariantCultureIgnoreCase)
                    || item.Value.Equals("GrayScale", StringComparison.InvariantCultureIgnoreCase) 
                    || item.Value.Equals("Pastel", StringComparison.InvariantCultureIgnoreCase)
                    || item.Value.Equals("Bright", StringComparison.InvariantCultureIgnoreCase)
                    || item.Value.Equals("BrightPastel", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (ListItem item in itemsToRemove)
            {
                _colorPalette.Items.Remove(item);
            }

            //Add the "Custom" palette option
            _colorPalette.Items.Add(new ListItem(WebTextManager.GetText("/controlText/dundasFrequencyOptionsEditor.ascx/custom"), "CUSTOM"));

            //Now set the selected palette
            if (_colorPalette.Items.FindByValue(selectedPalette) != null)
            {
                _colorPalette.SelectedValue = selectedPalette;
            }
        }
    }
}