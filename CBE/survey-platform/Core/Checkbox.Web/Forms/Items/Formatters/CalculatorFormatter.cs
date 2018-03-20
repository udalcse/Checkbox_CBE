using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format single line text items as HTML or text.
    /// </summary>
    [Serializable]
    public class CalculatorFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "CalculatorToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "CalculatorToText.xslt"; } }
    }
}
