using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Configuration.Install;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolvingCssElement : Common.WebControlBase
    {
        private const string ScriptFormatString = "<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}?v={1}\" />";
        private const string ScriptFormatMediaString = "<link rel=\"Stylesheet\" media=\"{0}\" type=\"text/css\" href=\"{1}?v={2}\" />";
        private const string ErrorFormatString = "<!-- [{0}] unable to emit link css tag because source was not set. -->";

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Media { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(Source))
            {
                writer.Write(ErrorFormatString, ID);
            }
            else
            {
                writer.Write(string.IsNullOrEmpty(Media)
                                ? string.Format(ScriptFormatString, ResolveUrl(Source), ApplicationInstaller.ApplicationAssemblyFullVersion)
                                : string.Format(ScriptFormatMediaString, Media, ResolveUrl(Source), ApplicationInstaller.ApplicationAssemblyFullVersion));
            }
        }
    }
}
