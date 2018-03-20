using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Web.Page;

namespace Checkbox.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class UserControlBase : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T FindFirstChildControl<T>() where T : Control
        {
            return FindChildControlsRecursive<T>(this, true).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected List<T> FindChildControls<T>() where T : Control
        {
            return FindChildControlsRecursive<T>(this, false);
        }

        private List<T> FindChildControlsRecursive<T>(Control control, bool first) where T : Control
        {
            List<T> result = new List<T>();

            foreach (Control childControl in control.Controls)
            {
                if (childControl is T)
                {
                    result.Add((T)childControl);
                    if (first)
                        return result;
                }
                else
                {
                    result.AddRange(FindChildControlsRecursive<T>(childControl, first));
                }
            }

            return result;
        }

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
