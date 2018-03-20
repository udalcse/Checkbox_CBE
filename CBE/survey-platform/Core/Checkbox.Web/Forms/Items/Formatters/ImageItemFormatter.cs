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
    public class ImageItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// Get the transform file for image to Html
        /// </summary>
        protected override string HtmlTransformFileName
        {
            get { return "ImageToHtml.xslt"; }
        }

        /// <summary>
        /// Get the transform file for image to text
        /// </summary>
        protected override string TextTransformFileName
        {
            get { return "ImageToText.xslt"; }
        }
    }
}