using System;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;

namespace Checkbox.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class WebControlBase : WebControl
    {
        /// <summary>
        /// Registers a script resource file for the current page, adds the version 
        /// </summary>
        protected void RegisterClientScriptInclude(string key, string url)
        {
            RegisterClientScriptInclude(GetType(), key, url);
        }

        /// <summary>
        /// Registers a script resource file for the current page, adds the version 
        /// </summary>
        protected void RegisterClientScriptInclude(Type type, string key, string url)
        {
            var page = Page as BasePage;
            if (page != null)
                page.RegisterClientScriptInclude(type, key, url);
            else
                Page.ClientScript.RegisterClientScriptInclude(type, key, url);
        }
    }
}
