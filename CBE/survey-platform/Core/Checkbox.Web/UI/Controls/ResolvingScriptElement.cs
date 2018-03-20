using System.Web.UI;
using Checkbox.Configuration.Install;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Simple element that emits a script control with URL resolution.
    /// </summary>
    public class ResolvingScriptElement : Common.WebControlBase
    {
        private const string ScriptFormatString = "<script type=\"text/javascript\" language=\"javascript\" src=\"{0}?v={1}\"></script>";
        private const string ErrorFormatString = "<!-- [{0}] unable to emit script tag because source was not set. -->";

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(string.IsNullOrEmpty(Source)
                             ? string.Format(ErrorFormatString, ID)
                             : string.Format(ScriptFormatString, ResolveUrl(Source), ApplicationInstaller.ApplicationAssemblyFullVersion));
        }
    }
}
