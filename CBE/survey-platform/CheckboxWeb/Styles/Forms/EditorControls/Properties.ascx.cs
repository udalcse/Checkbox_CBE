using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Styles;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Styles.Forms.EditorControls
{
    public partial class Properties : UserControl
    {
        private StyleTemplate template;

        protected override void OnInit(EventArgs e)
        {
            template = (StyleTemplate)HttpContext.Current.Session["CurrentStyleTemplate"];

            inUseValidator.StyleTemplateID = template.TemplateID;

            LoadStyleValues();
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateStyle();
        }

        private void LoadStyleValues()
        {
            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
                _styleName.Text = Server.HtmlEncode(template.Name);
            else
                _styleName.Text = template.Name;

            _editableStyle.Checked = template.IsEditable;
            _publicStyle.Checked = template.IsPublic;
        }

        private void UpdateStyle()
        {
            template.Name = _styleName.Text;
            template.IsEditable = _editableStyle.Checked;
            template.IsPublic = _publicStyle.Checked;

            HttpContext.Current.Session["CurrentStyleTemplate"] = template;
        }
    }
}