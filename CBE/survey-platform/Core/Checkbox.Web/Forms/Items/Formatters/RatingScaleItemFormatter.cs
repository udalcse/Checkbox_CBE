using System;


namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format rating scale items as HTML or text
    /// </summary>
    [Serializable]
    public class RatingScaleItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "RatingScaleToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "RatingScaleToText.xslt"; } }
    }
}
