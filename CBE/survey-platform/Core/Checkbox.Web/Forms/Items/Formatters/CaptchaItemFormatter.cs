using System;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Provide text and html formatting for captcha items
    /// </summary>
    [Serializable]
    public class CaptchaItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "CaptchaToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string  TextTransformFileName { get { return "CaptchaToText.xslt"; } }
    }
}
