using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Web.Charts;
using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Charts
{
    public partial class CustomColors : SecuredPage
    {
        
        //private SummaryChartItemAppearanceData _appearance;

        //protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        //protected List<string> GraphColors { get; set; }

        //protected override void OnPageInit()
        //{
        //    base.OnPageInit();

        //    if (!Page.IsPostBack)
        //    {
        //        _appearance = (SummaryChartItemAppearanceData)HttpContext.Current.Session["CurrentChartAppearance"];
        //        GraphColors = _appearance.PieGraphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        BindRepeater();
        //    }

        //    _colorRepeater.ItemCommand += ColorRepeater_ItemCommand;
        //    _addColorBtn.Click += AddColorBtn_Click;
        //    _save.Click += SaveBtn_Click;            
        //}

        //private void BindRepeater()
        //{
        //    _colorRepeater.DataSource = GraphColors;
        //    _colorRepeater.DataBind();
        //}

        //private void UpdateColors()
        //{
        //    string graphColors = string.Empty;

        //    foreach (string color in GraphColors)
        //    {
        //        graphColors += color + ",";
        //    }

        //    if(GraphColors.Count > 0)
        //        graphColors = graphColors.Remove(graphColors.Length - 1);

        //    _appearance.PieGraphColors = graphColors;
        //    GraphColors = graphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    HttpContext.Current.Session["CurrentChartAppearance"] = _appearance;

        //    BindRepeater();
        //}

        //void ColorRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        //{
        //    if (Utilities.IsNotNullOrEmpty(e.CommandName) && e.CommandName.Equals("DeleteColor", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        if (e.CommandArgument != null)
        //        {
        //            _appearance = (SummaryChartItemAppearanceData)HttpContext.Current.Session["CurrentChartAppearance"];
        //            GraphColors = _appearance.PieGraphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //            if (GraphColors.Contains(e.CommandArgument.ToString()))
        //            {
        //                GraphColors.Remove(e.CommandArgument.ToString());
        //                UpdateColors();
        //                BindRepeater();
        //            }
        //        }
        //    }

        //}

        //private void AddColorBtn_Click(object sender, ImageClickEventArgs e)
        //{
        //    if (_newColor.SelectedColor != System.Drawing.Color.Empty)
        //    {
        //        _appearance = (SummaryChartItemAppearanceData)HttpContext.Current.Session["CurrentChartAppearance"];
        //        GraphColors = _appearance.PieGraphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //        GraphColors.Add(Utilities.ColorToHex(_newColor.SelectedColor));
        //        string colorList = string.Empty;

        //        foreach (string color in GraphColors)
        //        {
        //            colorList += color + ",";
        //        }

        //        _appearance.PieGraphColors = colorList.Remove(colorList.Length - 1);
        //        HttpContext.Current.Session["CurrentChartAppearance"] = _appearance;

        //        BindRepeater();
        //        _newColor.SelectedColor = System.Drawing.Color.Empty;
        //    }
        //}

        //private void SaveBtn_Click(object sender, EventArgs e)
        //{
        //    _appearance = (SummaryChartItemAppearanceData)HttpContext.Current.Session["CurrentChartAppearance"];
        //    List<string> colors = _appearance.PieGraphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    string graphColors = string.Empty;

        //    foreach (string color in colors)
        //    {
        //        graphColors += color + ",";
        //    }

        //    graphColors = graphColors.Remove(graphColors.Length - 1);
        //    _appearance.PieGraphColors = graphColors;
        //    GraphColors = graphColors.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    HttpContext.Current.Session["CurrentChartAppearance"] = _appearance;

        //    ClientScript.RegisterClientScriptBlock(GetType(), "CloseReload", "CloseAndReload();", true);
        //}

        //protected void NewColorChanged(object sender, EventArgs e)
        //{
        //    if (_newColor.SelectedColor != System.Drawing.Color.Empty)
        //    {
        //        _newColorTxt.Text = Utilities.ColorToHex(_newColor.SelectedColor);
        //    }
        //    else
        //    {
        //        _newColorTxt.Text = string.Empty;
        //    }
        //}
    }
}
