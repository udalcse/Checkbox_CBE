using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    [Serializable]
    public class MessageItemFormatter : WebItemFormatter
    {
        protected override string HtmlTransformFileName
        {
            get { return "MessageToHtml.xslt"; }
        }

        protected override string TextTransformFileName
        {
            get { return "MessageToText.xslt"; }
        }
    }
}