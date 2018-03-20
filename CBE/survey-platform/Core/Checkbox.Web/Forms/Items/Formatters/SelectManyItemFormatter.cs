using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Formatter select many items (such as checkboxes) to html and text.
    /// </summary>
    [Serializable]
    public class SelectManyItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "SelectManyToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "SelectManyToText.xslt"; } }
    }
}
