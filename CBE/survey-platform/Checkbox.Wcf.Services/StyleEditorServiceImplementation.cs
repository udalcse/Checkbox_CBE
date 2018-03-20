using System;
using Checkbox.Globalization.Text;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Wcf.Services
{
    public static class StyleEditorServiceImplementation
    {
        public static StyleMetaData GetStyleData(CheckboxPrincipal userPrincipal, int styleId)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var style = StyleTemplateManager.GetStyleTemplate(styleId);

            return new StyleMetaData
            {
                CreatedBy = style.CreatedBy,
                DateCreated = style.DateCreated.HasValue ? style.DateCreated.Value : DateTime.MinValue,
                Id = style.TemplateID.HasValue ? style.TemplateID.Value : -1,
                IsEditable = style.IsEditable,
                IsPublic = style.IsPublic,
                Name = style.Name,
                HeaderTextId = style.HeaderTextID,
                FooterTextId = style.FooterTextID
            };
        }

        public static string GetStyleElementProperty(CheckboxPrincipal userPrincipal, int styleId, string elementName, string propertyName)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var style = StyleTemplateManager.GetStyleTemplate(styleId);

            return style.GetElementProperty(elementName, propertyName);
        }

        public static SimpleNameValueCollection GetStyleElementProperties(CheckboxPrincipal userPrincipal, int styleId, string elementName)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var style = StyleTemplateManager.GetStyleTemplate(styleId);

            var element = style.GetElementStyle(elementName);

            //if background is none don't show it 
            const string bgImageKey = "background-image";
            if (elementName == "body" && element.ContainsKey(bgImageKey) && element[bgImageKey] == "none")
            {
                element[bgImageKey] = string.Empty;
            }

            return new SimpleNameValueCollection(element);
        }

        public static bool SaveFormStyle(CheckboxPrincipal userPrincipal, int styleId, string languageCode, StyleData style)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var lwTemplate = StyleTemplateManager.GetLightweightStyleTemplate(styleId, userPrincipal);
            var template = lwTemplate == null ? StyleTemplateManager.CreateStyleTemplate(userPrincipal) : StyleTemplateManager.GetStyleTemplate(lwTemplate.TemplateId);

            foreach (var element in style.Css)
                template.SetElementStyle(element.Key, element.Value);

            //Save both header and footer html
            TextManager.SetText(template.HeaderTextID, languageCode, style.HeaderHtml);
            TextManager.SetText(template.FooterTextID, languageCode, style.FooterHtml);

            StyleTemplateManager.SaveTemplate(template, userPrincipal);

            //If we make it this far we have succeeded
            return true;
        }
    }
}
