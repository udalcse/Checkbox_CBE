using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Html and text formatter for images
    /// </summary>
    [Serializable]
    public class HtmlItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// Get the transform file for image to Html
        /// </summary>
        protected override string HtmlTransformFileName
        {
            get { return "HtmlToHtml.xslt"; }
        }

        /// <summary>
        /// Get the transform file for image to text
        /// </summary>
        protected override string TextTransformFileName
        {
            get { return "HtmlToText.xslt"; }
        }
    }
}