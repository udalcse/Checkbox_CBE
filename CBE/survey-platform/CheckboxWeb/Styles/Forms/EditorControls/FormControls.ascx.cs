using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Styles;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Styles.Forms.EditorControls
{
    public partial class FormControls : UserControl
    {
        private StyleTemplate template;

        protected override void OnInit(EventArgs e)
        {
            template = (StyleTemplate)HttpContext.Current.Session["CurrentStyleTemplate"];
            LoadStyleValues();
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateStyle();
        }

        private void LoadStyleValues()
        {
            _buttonFont.BindToDataSource(template);

            //Progress bar height and width
            _progressBarWidth.Text = template.GetElementProperty(".ProgressBar", "width").Replace("px", string.Empty);
            _progressBarHeight.Text = template.GetElementProperty(".ProgressBar", "height").Replace("px", string.Empty);

            //Progress bar border
            SetDropDownListSelectedValue(_progressBarBorderSize, template.GetElementProperty(".ProgressBar", "border-width"));

            //Progress bar border
            SetDropDownListSelectedValue(_progressBarBorderStyle, template.GetElementProperty(".ProgressBar", "border-style"));

            //Button border
            SetDropDownListSelectedValue(_buttonBorderStyle, template.GetElementProperty(".button", "border-style"));

            //Button border width
            SetDropDownListSelectedValue(_buttonBorderSize, template.GetElementProperty(".button", "border-width"));

            _buttonBorderColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".button", "border-color"));
            _buttonBackgroundColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".button", "background-color"));

            _progressBarBorderColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".ProgressBar", "border-color"));
            _progressBarBackColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".ProgressBar", "background-color"));
            _progressBarForeColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".ProgressBarInner", "background-color"));
        }

        private void UpdateStyle()
        {
            template.SetElementStyleProperty(".button", "border-width", _buttonBorderSize.SelectedValue);
            template.SetElementStyleProperty(".button", "border-style", _buttonBorderStyle.SelectedValue);
            template.SetElementStyleProperty(".ProgressBar", "border-width", _progressBarBorderSize.SelectedValue);
            template.SetElementStyleProperty(".ProgressBar", "border-style", _progressBarBorderStyle.SelectedValue);
            template.SetElementStyleProperty(".ProgressBar", "height", _progressBarHeight.Text + "px");
            template.SetElementStyleProperty(".ProgressBar", "width", _progressBarWidth.Text + "px");

            template.SetElementStyleProperty(".button", "border-color", Utilities.ColorToHex(_buttonBorderColor.SelectedColor, false));
            template.SetElementStyleProperty(".button", "background-color", Utilities.ColorToHex(_buttonBackgroundColor.SelectedColor, false));
            template.SetElementStyleProperty(".ProgressBar", "border-color", Utilities.ColorToHex(_progressBarBorderColor.SelectedColor, false));
            template.SetElementStyleProperty(".ProgressBar", "background-color", Utilities.ColorToHex(_progressBarBackColor.SelectedColor, false));
            template.SetElementStyleProperty(".ProgressBarInner", "background-color", Utilities.ColorToHex(_progressBarForeColor.SelectedColor, false));

            _buttonFont.UpdateDataSource(template);
        }

        protected void SetDropDownListSelectedValue(DropDownList list, string value)
        {
            if (list != null && value != null)
            {
                //Bind list, if necessary
                if (Utilities.IsNotNullOrEmpty(list.DataSourceID) && list.Items.Count == 0)
                {
                    list.DataBind();
                }

                if (list.Items.FindByValue(value) != null)
                {
                    list.SelectedValue = value;
                }
            }
        }
    }
}