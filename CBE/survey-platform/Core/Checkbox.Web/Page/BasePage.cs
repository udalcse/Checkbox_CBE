using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for pages in Checkbox Web
    /// </summary>
    public class BasePage : System.Web.UI.Page
    {
        /// <summary>
        /// Handle the error event to provide custom error handling.  If this method is reached in a normal control flow
        /// it indicates a failure of a page to gracefully handle an error condition.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnError(EventArgs e)
        {
            Exception ex = Server.GetLastError();

            //Ignore thread aborts
            if (!(ex is System.Threading.ThreadAbortException))
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Handle the specified exception through use of the framework exception handler.
        /// </summary>
        /// <param name="ex">Exception to handle.</param>
        protected virtual void HandleException(Exception ex)
        {
            HandleException(ex, null);
        }

        /// <summary>
        /// Handle the specified exception and redirect to the specified Url
        /// </summary>
        /// <param name="ex">Exception to handle.</param>
        /// <param name="redirectUrl">URL to redirect to.</param>
        protected virtual void HandleException(Exception ex, string redirectUrl)
        {
            ExceptionPolicy.HandleException(ex, "UIProcess");

            if (redirectUrl != null && redirectUrl.Trim() != string.Empty)
            {
                Response.Redirect(redirectUrl, true);
            }
        }

        /// <summary>
        /// Check for Safari browser to force use of "uplevel" asp:menu
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPreInit(EventArgs e)
        {
            if (Request != null
                 && Request.ServerVariables != null
                 && Request.ServerVariables["http_user_agent"] != null
                 && Request.ServerVariables["http_user_agent"].ToLower().Equals("safari")
                 && Page != null)
            {
                Page.ClientTarget = "uplevel";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T FindFirstChildControl<T>() where T : Control
        {
            return FindChildControlsRecursive<T>(this, null, true).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T FindFirstChildControl<T>(string name) where T : Control
        {
            return FindChildControlsRecursive<T>(this, name, true).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected List<T> FindChildControls<T>() where T : Control
        {
            return FindChildControlsRecursive<T>(this, null, false);
        }

        private List<T> FindChildControlsRecursive<T>(Control control, string name, bool first) where T : Control
        {
            List<T> result = new List<T>();

            foreach (Control childControl in control.Controls)
            {
                if (childControl is T && (string.IsNullOrEmpty(name) || childControl.UniqueID == name))
                {
                    result.Add((T)childControl);
                    if (first)
                        return result;
                }
                else
                {
                    result.AddRange(FindChildControlsRecursive<T>(childControl, name, first));
                }
            }

            return result;
        }

        /// <summary>
        /// Registers a script resource file for the current page, adds the version 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="url"></param>
        public void RegisterClientScriptInclude(string key, string url)
        {
            RegisterClientScriptInclude(GetType(), key, url);
        }

        /// <summary>
        /// Registers a script resource file for the current page, adds the version 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="url"></param>
        public void RegisterClientScriptInclude(Type type, string key, string url)
        {
            //let's look if there is the predefined script include placeholder
            PlaceHolder scriptIncludesPlace =
                Header.Controls.OfType<PlaceHolder>().FirstOrDefault(c => c.ID == "_scriptIncludesPlace");

            //try to find any resolving script elements
            var resolvingScriptIncludes = Header.Controls.OfType<ResolvingScriptIncludesElement>().FirstOrDefault();

            //try to find it in the placeholder
            if (resolvingScriptIncludes == null && scriptIncludesPlace != null)
                resolvingScriptIncludes = scriptIncludesPlace.Controls.OfType<ResolvingScriptIncludesElement>().FirstOrDefault();
            
            //if there are no any
            if (resolvingScriptIncludes == null)
            {
                //create a new one
                resolvingScriptIncludes = new ResolvingScriptIncludesElement { ID = "_resolvingScriptIncludes" };

                //add it in to the placeholder or directly into the header
                if (scriptIncludesPlace != null)
                    scriptIncludesPlace.Controls.Add(resolvingScriptIncludes);
                else
                    Header.Controls.Add(resolvingScriptIncludes);
            }

            //include the script
            resolvingScriptIncludes.RegisterClientScriptInclude(type, key, url);
        }
    }
}
