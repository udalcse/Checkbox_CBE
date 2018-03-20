using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Globalization.Text;
using Checkbox.Styles;
using Checkbox.Web.UI.Controls.RadExtensions;

namespace CheckboxWeb.Styles.Forms.EditorControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class HeaderFooter : UserControl
    {
        private StyleTemplate _template;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if (_editorType.Items.Count == 0)
            {
                _editorType.Items.Add(new ListItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/header"), "HEADER"));
                _editorType.Items.Add(new ListItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/footer"), "FOOTER"));
            }

            EditorFontLoader.LoadEditorFonts(_header);
            EditorFontLoader.LoadEditorFonts(_footer);

            _template = (StyleTemplate)HttpContext.Current.Session["CurrentStyleTemplate"];

            LoadStyleValues();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            UpdateStyle();
            
            if (_editorType.SelectedValue == "HEADER")
            {
                _header.Visible = true;
                _footer.Visible = false;
            }
            else
            {
                _footer.Visible = true;
                _header.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadStyleValues()
        {
            _header.Content = TextManager.GetText(_template.HeaderTextID, TextManager.DefaultLanguage);
            _footer.Content = TextManager.GetText(_template.FooterTextID, TextManager.DefaultLanguage);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStyle()
        {
            if (_template.HeaderTextID != null)
                TextManager.SetText(_template.HeaderTextID, TextManager.DefaultLanguage, _header.Content);
            if (_template.FooterTextID != null)
                TextManager.SetText(_template.FooterTextID, TextManager.DefaultLanguage, _footer.Content);

            HttpContext.Current.Session["CurrentStyleTemplate"] = _template;
        }
    }
}