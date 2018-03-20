using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format single line text items as HTML or text.
    /// </summary>
    [Serializable]
    public class AddressVerifierItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "AddressVerifierToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "AddressVerifierToText.xslt"; } }
    }
}
