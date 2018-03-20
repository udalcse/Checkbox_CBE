using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format select 1 items, such as radio buttons or dropdown lists
    /// to html or text
    /// </summary>
    [Serializable]
    public class Select1ItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "Select1ToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "Select1ToText.xslt"; } }
    }
}
