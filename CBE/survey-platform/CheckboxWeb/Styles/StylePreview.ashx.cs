using System;
using System.Web;
using System.Web.UI;
using Checkbox.Globalization.Text;
using Checkbox.Styles;

namespace CheckboxWeb.Styles
{
    /// <summary>
    /// Summary description for StylePreview
    /// </summary>
    public class StylePreview : IHttpHandler
    {

        public int StyleId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["s"]))
                    return Int32.Parse(HttpContext.Current.Request.QueryString["s"]);
                return -1;
            }
        }

        public string StyleCss
        {
            get
            {
                var template = StyleTemplateManager.GetStyleTemplate(StyleId);

                return template.GetCss().Replace("body", "#_previewContainer");
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();

            context.Response.ContentType = "text/css";

            context.Response.Write(StyleCss);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}