using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.Adapters;

namespace Checkbox.Web.UrlRewriting
{
    /// <summary>
    /// Form rewriter to preserve rewritten URLS on postback
    /// </summary>
    public class FormRewriterControlAdapter : ControlAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new FormRewriterHtmlTextWriter(writer));
        }
    }

    /// <summary>
    /// Html Text writer for writing form action
    /// </summary>
    public class FormRewriterHtmlTextWriter : HtmlTextWriter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer"></param>
        public FormRewriterHtmlTextWriter(TextWriter writer)
            : base(writer)
        {
        }
       
        /// <summary>
        /// Override write attribute for action tag
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="fEncode"></param>
        public override void WriteAttribute(string name, string value, bool fEncode)
        {
            if (string.Compare(name, "action", true) == 0)
            {
                value = HttpContext.Current.Request.RawUrl;
            }

            base.WriteAttribute(name, value, fEncode);
        }
    }
}
