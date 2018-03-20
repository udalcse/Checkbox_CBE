using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Html and text formatter for categorized matrix questions
    /// </summary>
    [Serializable]   
    class CategorizedMatrixItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// Get the transform file for matrix to Html
        /// </summary>
        protected override string HtmlTransformFileName { get { return "CategorizedMatrixToHtml.xslt"; } }

        /// <summary>
        /// Get the transform file for text to Html
        /// </summary>
        protected override string TextTransformFileName { get { return "CategorizedMatrixToText.xslt"; } }
    }
}
