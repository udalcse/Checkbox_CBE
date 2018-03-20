using System;
using System.Drawing;
using Checkbox.Web;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Text : ChartStyleUserControl
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
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateAppearanceValues()
        {
            Appearance.FontFamily = _font.SelectedValue;
            
            //Until UI Added, copy font to legend and title
            Appearance.LegendFont = _font.SelectedValue;
            Appearance.TitleFont = _font.SelectedValue;

            Appearance.TitleFontSize = Int32.Parse(_titleSize.SelectedValue.Replace("pt", string.Empty).Trim());
            Appearance.LegendFontSize = Int32.Parse(_legendSize.SelectedValue.Replace("pt", string.Empty).Trim());
            Appearance.LabelFontSize = Int32.Parse(_labelSize.SelectedValue.Replace("pt", string.Empty).Trim());
            Appearance.WrapTitleChars = string.IsNullOrEmpty(_wrapTitleChars.Text) ? 0 : Int32.Parse(_wrapTitleChars.Text.Trim());
            Appearance.TextColor = _textColor.Text;
            Appearance.LegendTextColor = _legendTextColor.Text;
            Appearance.HintTextColor = _hintTextColor.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadStyleValues()
        {
            _font.Items.Clear();
            _titleSize.Items.Clear();
            _legendSize.Items.Clear();
            _labelSize.Items.Clear();

            BindList(_font, GetFontFamilyNameListItems(FontStyle.Bold, FontStyle.Regular), Appearance.FontFamily);
            _font.Enabled = Appearance.ShowTitle;
            BindList(_titleSize, GetFontSizeListItems(8, 36, 1, " pt"), Appearance.TitleFontSize.ToString());
            _titleSize.Enabled = Appearance.ShowTitle;
            BindList(_legendSize, GetFontSizeListItems(8, 14, 1, " pt"), Appearance.LegendFontSize.ToString());
            BindList(_labelSize, GetFontSizeListItems(8, 14, 1, " pt"), Appearance.LabelFontSize.ToString());
            _wrapTitleChars.Text = Appearance.WrapTitleChars > 0 ? Appearance.WrapTitleChars.ToString() : "";
            _wrapTitleChars.Attributes.Add("title", WebTextManager.GetText("/pageText/styles/charts/text.ascx/wrapTitleCharsTooltip"));
            _wrapTitleChars.Enabled = Appearance.ShowTitle;
            _textColor.Text = Appearance.TextColor;
            _legendTextColor.Text = Appearance.LegendTextColor;
            _hintTextColor.Text = Appearance.HintTextColor;
        }
    }
}