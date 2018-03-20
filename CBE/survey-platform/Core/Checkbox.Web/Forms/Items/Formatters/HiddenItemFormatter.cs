using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// HTML and text formatter for hidden items
    /// </summary>
    [Serializable]
    public class HiddenItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "HiddenToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "HiddenToText.xslt"; } }
    }
}
