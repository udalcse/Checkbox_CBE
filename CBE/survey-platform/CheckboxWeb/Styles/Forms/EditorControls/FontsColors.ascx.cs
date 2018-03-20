using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Web;
using Checkbox.Common;
using Telerik.Web.UI;
using System.Text;
using CheckboxWeb.Styles.Controls;

namespace CheckboxWeb.Styles.Forms.EditorControls
{
    public partial class FontsColors : UserControl
    {
        private StyleTemplate template;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _fontsColorsAccordion.FindControl("nothing");

            template = (StyleTemplate)HttpContext.Current.Session["CurrentStyleTemplate"];
            
            LoadStyleValues();

            bodyStylesLbl.Click += new EventHandler(EditStyle);
            _changeBgImageLnk.Click += new ImageClickEventHandler(_changeBgImageLnk_Click);
            _deleteBgImageLnk.Click += new ImageClickEventHandler(_deleteBgImageLnk_Click);
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            template.SetElementStyleProperty("body", "background-color", Utilities.ColorToHex(_backgroundColorPicker.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix", "border-color", Utilities.ColorToHex(_matrixBorderColorPicker.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .header", "background-color", Utilities.ColorToHex(_matrixHeaderRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .header td", "background-color", Utilities.ColorToHex(_matrixHeaderRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .header th", "background-color", Utilities.ColorToHex(_matrixHeaderRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .subheader td", "background-color", Utilities.ColorToHex(_matrixSubHeadingRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .subheader", "background-color", Utilities.ColorToHex(_matrixSubHeadingRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .Item", "background-color", Utilities.ColorToHex(_matrixDefaultRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .Item td", "background-color", Utilities.ColorToHex(_matrixDefaultRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .AlternatingItem", "background-color", Utilities.ColorToHex(_matrixAltRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Matrix .AlternatingItem td", "background-color", Utilities.ColorToHex(_matrixAltRowColor.SelectedColor, false));
            template.SetElementStyleProperty(".Page", "height", _itemSpace.Text + "px");
            template.SetElementStyleProperty(".Matrix", "border-style", _matrixBorderStyle.SelectedValue);
            template.SetElementStyleProperty(".Matrix", "border-width", _matrixBorderWidth.SelectedValue);

            _baseFont.UpdateDataSource(template);
            _titleFont.UpdateDataSource(template);
            _pageNumberFont.UpdateDataSource(template);
            _questionFont.UpdateDataSource(template);
            _subtextFont.UpdateDataSource(template);
            _errorMessage.UpdateDataSource(template);
            _matrixAltRowFont.UpdateDataSource(template);
            _matrixHeaderAnswerFont.UpdateDataSource(template);
            _matrixHeaderQuestionFont.UpdateDataSource(template);
            _matrixRowFont.UpdateDataSource(template);
            _matrixSubheaderFont.UpdateDataSource(template);

            HttpContext.Current.Session["CurrentStyleTemplate"] = template;
        }

        private void LoadStyleValues()
        {
            string spacer = template.GetElementProperty(".Page", "height");
            _itemSpace.Text = String.IsNullOrEmpty(spacer) ? "25" : spacer.Replace("px", string.Empty);

            string backgroundImg = template.GetElementProperty("body", "background-image").TrimStart("url(\"".ToCharArray()).TrimEnd("\")".ToCharArray());
            if (Utilities.IsNullOrEmpty(backgroundImg))
            {
                _deleteBgImageLnk.Visible = false;
                _backgroundImageUrl.Text = WebTextManager.GetText("/pageText/styles/forms/editor.aspx/noImage");
            }
            else
            {
                _deleteBgImageLnk.Visible = true;
                _backgroundImageUrl.Text = backgroundImg;
            }

            //Set initial values for matrix border
            SetDropDownListSelectedValue(_matrixBorderStyle, template.GetElementProperty(".Matrix", "border-style"));
            SetDropDownListSelectedValue(_matrixBorderWidth, template.GetElementProperty(".Matrix", "border-width").Replace(" ", string.Empty));

            _titleFont.BindToDataSource(template);
            _pageNumberFont.BindToDataSource(template);
            _errorMessage.BindToDataSource(template);
            _questionFont.BindToDataSource(template);
            _subtextFont.BindToDataSource(template);
            _answerFont.BindToDataSource(template);
            _matrixRowFont.BindToDataSource(template);
            _matrixAltRowFont.BindToDataSource(template);
            _matrixHeaderQuestionFont.BindToDataSource(template);
            _matrixHeaderAnswerFont.BindToDataSource(template);
            _matrixSubheaderFont.BindToDataSource(template);
            _baseFont.BindToDataSource(template);

            _backgroundColorPicker.SelectedColor = Utilities.HexToColor(template.GetElementProperty("body", "background-color"));
            _matrixBorderColorPicker.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".Matrix", "border-color"));
            _matrixDefaultRowColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".Matrix .Item", "background-color"));
            _matrixAltRowColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".Matrix .AlternatingItem", "background-color"));
            _matrixSubHeadingRowColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".Matrix .subheader", "background-color"));
            _matrixHeaderRowColor.SelectedColor = Utilities.HexToColor(template.GetElementProperty(".Matrix .header", "background-color"));
        }

        void _deleteBgImageLnk_Click(object sender, ImageClickEventArgs e)
        {
            template.SetElementStyleProperty("body", "background-image", "url(\"\")");
            StyleTemplateManager.SaveTemplate(template, HttpContext.Current.User as CheckboxPrincipal);

            _backgroundImageUrl.Text = WebTextManager.GetText("/pageText/styles/forms/editor.aspx/noImage");
            _deleteBgImageLnk.Visible = false;
        }

        void _changeBgImageLnk_Click(object sender, ImageClickEventArgs e)
        {
            _radWindowMgr.Windows[0].NavigateUrl = "~/Styles/Forms/BackgroundImage.aspx?StyleTemplateID=" + template.TemplateID.ToString();
            _radWindowMgr.Windows[0].VisibleOnPageLoad = true;
        }

        public void EditStyle(object sender, EventArgs e)
        {
            if (sender is ImageButton)
            {
                _radWindowMgr.Windows[0].NavigateUrl = string.Format(
                    "~/Styles/Forms/StyleBuilder.aspx?StyleTemplateID={0}&ElementName={1}",
                    template.TemplateID,
                    Server.UrlEncode(((ImageButton)sender).CommandArgument));

                _radWindowMgr.Windows[0].VisibleOnPageLoad = true;
            }
            else if (sender is LinkButton)
            {
                _radWindowMgr.Windows[0].NavigateUrl = string.Format(
                    "~/Styles/Forms/StyleBuilder.aspx?StyleTemplateID={0}&ElementName={1}",
                    template.TemplateID,
                    Server.UrlEncode(((LinkButton)sender).CommandArgument));

                _radWindowMgr.Windows[0].VisibleOnPageLoad = true;
            }
            else if (sender is FontSelector)
            {
                string elementName = ((FontSelector)sender).ElementName;

                if (Utilities.IsNotNullOrEmpty(((FontSelector)sender).AltElementName))
                {
                    elementName += "," + ((FontSelector)sender).AltElementName;
                }

                _radWindowMgr.Windows[0].NavigateUrl = string.Format(
                    "~/Styles/Forms/StyleBuilder.aspx?StyleTemplateID={0}&ElementName={1}",
                    template.TemplateID,
                    Server.UrlEncode(elementName));

                _radWindowMgr.Windows[0].VisibleOnPageLoad = true;
            }
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