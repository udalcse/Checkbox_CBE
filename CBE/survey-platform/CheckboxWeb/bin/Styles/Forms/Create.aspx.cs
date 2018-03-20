using System;
using System.Xml;
using Checkbox.Common;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.IO;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Create : ApplicationPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "t")]
        public string StyleType { get; set; }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/styles/forms/create.aspx/createStyle");
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            //if there are validation errors we do nothing and the validation errors will be displayed
            if (!Page.IsValid) return;

            StyleTemplate template;

            //if the _styleProperties.StyleTemplate property is null then we must create a new style
            if (_styleProperties.StyleTemplate == null)
            {
                var defaultStyle = new XmlDocument();
                if (_styleProperties.BaseStyleID == 0)
                {
                    string filePath = Server.MapPath("~/Resources/DefaultStylePC.xml");
                    
                    if (!File.Exists(filePath))
                        filePath = Server.MapPath("~/Resources/DefaultStyle.xml");//take template from old file
                    defaultStyle.Load(filePath);
                }
                else
                {
                    //create a template based on another
                    StyleTemplate source = StyleTemplateManager.GetStyleTemplate(_styleProperties.BaseStyleID);
                    defaultStyle = source.ToXml();
                }

                template = StyleTemplateManager.CreateStyleTemplate(defaultStyle, UserManager.GetCurrentPrincipal());
                template.Type = StyleTemplateType.PC;
                template.Name = Utilities.AdvancedHtmlEncode(_styleProperties.StyleName);
                template.IsEditable = _styleProperties.IsEditable;
                template.IsPublic = _styleProperties.IsPublic;

                StyleTemplateManager.SaveTemplate(template, UserManager.GetCurrentPrincipal());
            }
            else
            {
                template = StyleTemplateManager.GetStyleTemplate(_styleProperties.StyleTemplate.TemplateId);
                template.Name = _styleProperties.StyleName;
                template.IsEditable = _styleProperties.IsEditable;
                template.IsPublic = _styleProperties.IsPublic;

                StyleTemplateManager.SaveTemplate(template, UserManager.GetCurrentPrincipal());
            }

            if (template.TemplateID.HasValue)
                CloseAndRedirect(template.TemplateID.Value);
        }

        private void CloseAndRedirect(int id)
        {
            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Redirect",
                string.Format("closeWindowAndRedirectParentPage('', null, 'Edit.aspx?s={0}');", id),
                true);
        }
    }
}