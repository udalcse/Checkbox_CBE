using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Properties : ApplicationPage
    {
        //set max length of "style properties dialog" title 
        const int StylePropertyTextLength = 36;
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;
           
            //Set page title
            String title = WebTextManager.GetText("/pageText/styles/forms/properties.aspx/title");
            title = String.Format(title, Utilities.TruncateText(_styleProperties.StyleTemplate.Name, StylePropertyTextLength));
            Master.Title = title;
            Master.OkVisible = _styleProperties.StyleTemplate.CanBeEdited;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (CanEdit)
            {
                var template = StyleTemplateManager.GetStyleTemplate(_styleProperties.StyleTemplate.TemplateId);

                template.Name = Utilities.AdvancedHtmlEncode(_styleProperties.StyleName);
                template.IsEditable = _styleProperties.IsEditable;
                template.IsPublic = _styleProperties.IsPublic;
                template.Type = StyleTemplateType.PC;

                StyleTemplateManager.SaveTemplate(template, UserManager.GetCurrentPrincipal());

                Dictionary<String, String> arg = new Dictionary<string, string>();
                arg.Add("p", "properties");
                Master.CloseDialog(arg);
            }
            Master.CloseDialog(null);
        }

        protected bool CanEdit
        {
            get
            {
                return _styleProperties.StyleTemplate.CanBeEdited;
            }
        }
    }
}