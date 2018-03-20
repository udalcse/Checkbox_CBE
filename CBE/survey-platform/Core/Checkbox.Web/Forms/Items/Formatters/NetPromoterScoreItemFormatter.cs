using System;


namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format Net Promoter Score items as HTML or text
    /// </summary>
    [Serializable]
    public class NetPromoterScoreItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "NetPromoterScoreToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "NetPromoterScoreToText.xslt"; } }
    }
}
