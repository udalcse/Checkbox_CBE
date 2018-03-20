using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format slider items as html or text
    /// </summary>
    [Serializable]
    public class SliderItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName
        {
            get { return "SliderToHtml.xslt"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName
        {
            get { return "SliderToText.xslt"; }
        }
    }
}
