using System;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Web.Charts;
using Checkbox.Styles;

namespace CheckboxWeb.Styles.Charts.EditorControls
{
    public partial class General : ChartStyleUserControl
    {
        //private LightweightStyleTemplate _style;

        //protected override void OnInit(EventArgs e)
        //{
        //    //int styleId = string.IsNullOrEmpty(Request.QueryString["id"]) ? -1 : Int32.Parse(Request.QueryString["id"]);
        //    //if (styleId > 0)
        //    //    _style = StyleTemplateManager.GetLightweightStyleTemplate(styleId);
        //    //else
        //        _style = (LightweightStyleTemplate)HttpContext.Current.Session["CurrentChartStyleProperties"];

        //        _position.Items.Add(new ListItem(WebTextManager.GetText("/controlText/styles/position/left"), "LEFT"));
        //        _position.Items.Add(new ListItem(WebTextManager.GetText("/controlText/styles/position/center"), "CENTER"));
        //        _position.Items.Add(new ListItem(WebTextManager.GetText("/controlText/styles/position/right"), "RIGHT"));

        //    base.OnInit(e);
        //}

        //protected override void UpdateAppearanceValues()
        //{
        //    _style.Name = _styleName.Text;
        //    Appearance.Height = Int32.Parse(_height.Text);
        //    Appearance.Width = Int32.Parse(_width.Text);
        //    _style.IsPublic = _public.Checked;
        //    _style.IsEditable = _editable.Checked;
        //    Appearance.BackgroundColor = Utilities.ColorToHex(_backgroundColor.SelectedColor);
        //    Appearance.ItemPosition = _position.SelectedValue;

        //    HttpContext.Current.Session["CurrentChartStyleProperties"] = _style;
        //}

        //public void LoadDefaultValues()
        //{
        //    _styleName.Text = string.Empty;
        //    _public.Checked = false;
        //    _editable.Checked = true;

        //    _backgroundColor.SelectedColor = Utilities.GetColor(Appearance.BackgroundColor, false);
        //    _height.Text = Appearance.Height.ToString();
        //    _width.Text = Appearance.Width.ToString();
        //    _position.SelectedValue = Appearance.ItemPosition;
        //}

        //protected override void LoadStyleValues()
        //{
        //    if (_style == null)
        //    {
        //        LoadDefaultValues();
        //        return;
        //    }
        //    _styleName.Text = _style.Name;
        //    _public.Checked = _style.IsPublic;
        //    _editable.Checked = _style.IsEditable;

        //    _backgroundColor.SelectedColor = Utilities.GetColor(Appearance.BackgroundColor, false);
        //    _height.Text = Appearance.Height.ToString();
        //    _width.Text = Appearance.Width.ToString();
        //    _position.SelectedValue = Appearance.ItemPosition;
        //}
    }
}