using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format upload items as text or html
    /// </summary>
    [Serializable]
    public class UploadItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName { get { return "UploadItemToHtml.xslt"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName { get { return "UploadItemToText.xslt"; } }
    }
}
