using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Configuration.Install;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolvingScriptIncludesElement : WebControl
    {
        private const string _scriptFormatString = "<script type=\"text/javascript\" language=\"javascript\" src=\"{0}\"></script>";

        /// <summary>
        /// 
        /// </summary>
        public ResolvingScriptIncludesElement()
        {
            ScriptIncludes = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, string> ScriptIncludes { set; get; }

        /// <summary>
        /// Registers a script resource file for the current page, adds the version 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="url"></param>
        public void RegisterClientScriptInclude(Type type, string key, string url)
        {
            ScriptIncludes[type.FullName + key] = AddVersionParameter(url);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (var script in ScriptIncludes)
            {
                writer.Write(_scriptFormatString, script.Value);
            }
        }

        protected string AddVersionParameter(string url)
        {
            if (url.Contains("?"))
                return url + "&v=" + ApplicationInstaller.ApplicationAssemblyFullVersion;

            return url + "?v=" + ApplicationInstaller.ApplicationAssemblyFullVersion;
        }
    }
}
