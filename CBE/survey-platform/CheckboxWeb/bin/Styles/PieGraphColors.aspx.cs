using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Styles
{
    public partial class PieGraphColors : Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Master.SetTitle(WebTextManager.GetText("/pageText/pieGraphColors.aspx/pieGraphColorsLbl"));

            refreshColorListBtn.Click += RefreshColorListBtnClick;
            Master.OkClick += new EventHandler(Master_OkClick);
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Palette(15, 100);

            if (!Page.IsPostBack)
            {
                string[] colorList = System.Web.HttpUtility.UrlDecode(Request["colorList"]).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string color in colorList)
                {
                    Color hexColor = Utilities.GetColor("#" + color, false);
                    _pieGraphColorsTxtList.Text += Utilities.ColorToHex(hexColor) + ", ";
                }
                SelectedColors(_pieGraphColorsTxtList.Text);

            }

            _refreshBtn.Style["font"] = "11px Arial";
            _refreshBtn.Style["COLOR"] = "black";
        }

        /// <summary>
        /// Palette
        /// </summary>
        /// <param name="ColorPickerRow"></param>
        /// <param name="Saturation"></param>
        private void Palette(long ColorPickerRow, long Saturation)
        {
            string RGBColor;
            double Gray;
            double lum;
            double sat;
            double hue;
            HtmlTableRow row;
            HtmlTableCell cell;
            HtmlTable Table1 = new HtmlTable();
            ColorConverter cc = new ColorConverter();

            Table1.CellPadding = 0;
            Table1.CellSpacing = 1;

            sat = Saturation / 100;

            for (hue = 0; hue <= 359; hue = hue + (260 / ColorPickerRow))
            {
                row = new HtmlTableRow();

                for (lum = 0.064; lum <= 1; lum = lum + 0.036)
                {
                    cell = new HtmlTableCell();
                    cell.Width = "35";

                    RGBColor = HlsToRgb(hue, lum, sat);
                    cell.BgColor = RGBColor;
                    cell.Controls.Add(new LinkButton());
                    ((LinkButton)cell.Controls[0]).Text = "__";
                    ((LinkButton)cell.Controls[0]).Width = 15;
                    ((LinkButton)cell.Controls[0]).BackColor = ((LinkButton)cell.Controls[0]).ForeColor = Utilities.HexToColor("#" + RGBColor);

                    ((LinkButton)cell.Controls[0]).Click += new EventHandler(PieGraphColors_Click);

                    row.Cells.Add(cell);
                }

                Table1.Rows.Add(row);
            }

            row = new HtmlTableRow();

            for (Gray = 0; Gray <= 255; Gray = Gray + 10.2)
            {
                cell = new HtmlTableCell();
                cell.Width = "35";

                cell.BgColor = Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2) + Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2) + Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2);
                cell.Controls.Add(new LinkButton());
                ((LinkButton)cell.Controls[0]).Text = "__";
                ((LinkButton)cell.Controls[0]).Width = 15;
                ((LinkButton)cell.Controls[0]).BackColor = ((LinkButton)cell.Controls[0]).ForeColor = Color.FromArgb((int)Gray, (int)Gray, (int)Gray);

                string colorCode = Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2) + Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2) + Utilities.Right("00" + Utilities.DoubleToHex(Gray), 2);

                ((LinkButton)cell.Controls[0]).Click += new EventHandler(PieGraphColors_Click);

                row.Cells.Add(cell);
            }

            Table1.Rows.Add(row);
            Table1.Style["margin-left"] = "4px";
            Table1.Style["border-collapse"] = "separate";
            Table1.Style["border-spacing"] = "1px";
            paletteTable.Controls.Add(Table1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RefreshColorListBtnClick(object sender, ImageClickEventArgs e)
        {
            SelectedColors(_pieGraphColorsTxtList.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            Dictionary<string,string> parameters = new Dictionary<string, string>();
            parameters["result"] = "ok";
            parameters["newColorList"] = _pieGraphColorsTxtList.Text.Replace(" ", "");
            Master.CloseDialog(parameters);
        }

        /// <summary>
        /// To Rgb
        /// </summary>
        /// <param name="H"></param>
        /// <param name="L"></param>
        /// <param name="S"></param>
        /// <returns></returns>
        private string HlsToRgb(double H, double L, double S)
        {
            double p1;
            double p2;
            double r;
            double g;
            double b;

            if (L <= 0.5)
            {
                p2 = L * (1 + S);
            }
            else
            {
                p2 = L + S - L * S;
            }

            p1 = 2 * L - p2;

            if (S == 0)
            {
                r = Convert.ToDouble(L * 255);
                g = Convert.ToDouble(L * 255);
                b = Convert.ToDouble(L * 255);
            }
            else
            {
                r = Convert.ToDouble(QqhToRgb(p1, p2, H + 120) * 255);
                g = Convert.ToDouble(QqhToRgb(p1, p2, H) * 255);
                b = Convert.ToDouble(QqhToRgb(p1, p2, H - 120) * 255);
            }

            return Utilities.Right("00" + Utilities.DoubleToHex(r), 2) + Utilities.Right("00" + Utilities.DoubleToHex(g), 2) + Utilities.Right("00" + Utilities.DoubleToHex(b), 2);
        }

        /// <summary>
        /// to rgb
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="hue"></param>
        /// <returns></returns>
        private double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360)
            {
                hue = hue - 360;
            }
            else if (hue < 0)
            {
                hue = hue + 360;
            }
            if (hue < 60)
            {
                return q1 + (q2 - q1) * hue / 60;
            }
            else if (hue < 180)
            {

                return q2;
            }
            else if (hue < 240)
            {
                return q1 + (q2 - q1) * (240 - hue) / 60;
            }
            else
            {
                return q1;
            }
        }

        /// <summary>
        /// Populates the current selected colors
        /// </summary>
        private void SelectedColors(string colorList)
        {
            HtmlTable SelectedColorsTable = new HtmlTable();
            HtmlTableRow newRow = new HtmlTableRow();
            HtmlTableCell newColorCell = new HtmlTableCell();

            string[] colorArray = colorList.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string color in colorArray)
            {
                newRow = new HtmlTableRow();
                newColorCell = new HtmlTableCell();
                newColorCell.Controls.Add(new Label());

                ((Label)newColorCell.Controls[0]).BackColor = Utilities.GetColor(color.Trim(), false);
                ((Label)newColorCell.Controls[0]).ForeColor = Utilities.GetColor(color.Trim(), false);
                ((Label)newColorCell.Controls[0]).Text = "__";
                ((Label)newColorCell.Controls[0]).Font.Size = FontUnit.XSmall;

                newRow.Cells.Add(newColorCell);
                SelectedColorsTable.Rows.Add(newRow);
            }

            SelectedColorsTable.Style["margin-right"] = "4px";
            SelectedColorsTable.Style["border-collapse"] = "separate";
            SelectedColorsTable.Style["border-spacing"] = "1px";
            _selectedColors.Controls.Clear();
            _selectedColors.Controls.Add(SelectedColorsTable);
        }

        /// <summary>
        /// Populates the submit button javascript when a color is added to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void PieGraphColors_Click(object sender, EventArgs ea)
        {
            _pieGraphColorsTxtList.Text += Utilities.ColorToHex(((LinkButton)sender).BackColor) + ", ";

            SelectedColors(_pieGraphColorsTxtList.Text);
        }
    }
}