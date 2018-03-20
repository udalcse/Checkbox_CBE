using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format multi-line text to html or text
    /// </summary>
    [Serializable]
    public class MultiLineTextBoxItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "MultiLineTextToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "MultiLineTextToText.xslt"; } }
    }
}
