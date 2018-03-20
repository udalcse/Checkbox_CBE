using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Html and text formatter for matrix questions
    /// </summary>
    [Serializable]
    public class MatrixItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// Get the transform file for matrix to Html
        /// </summary>
        protected override string HtmlTransformFileName { get { return "MatrixToHtml.xslt"; } }

        /// <summary>
        /// Get the transform file for text to Html
        /// </summary>
        protected override string TextTransformFileName { get { return "MatrixToText.xslt"; } }
    }
}
