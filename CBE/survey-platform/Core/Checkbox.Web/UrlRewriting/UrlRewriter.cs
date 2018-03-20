using System;
using System.Web;

namespace Checkbox.Web.UrlRewriting
{
    /// <summary>
    /// Base class for http module to handle URL rewriting tasks
    /// </summary>
    public abstract class UrlRewriter : IHttpModule
    {
        #region IHttpModule Members

        /// <summary>
        /// Required member
        /// </summary>
        public virtual void Dispose() {}

        /// <summary>
        /// Initialize the handler
        /// </summary>
        /// <param name="context">Application context for the handler</param>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += context_AuthenticateRequest;
        }

        /// <summary>
        /// When the request starts, rewrite the URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void context_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                HttpApplication app = (HttpApplication)sender;
                RewriteUrl(app.Request.Path, app);
            }
            catch
            {
            }
        }

        #endregion

        /// <summary>
        /// Rewrite the URL
        /// </summary>
        /// <param name="requestedPath">URL requested</param>
        /// <param name="app">Application context.</param>
        protected abstract void RewriteUrl(string requestedPath, HttpApplication app);
        
        /// <summary>
        /// Rewrite the provided source url to be the destination path with the same query parameters.
        /// </summary>
        /// <param name="sourcePathAndQuery">Source path including query string parameters.</param>
        /// <param name="destinationPath">Destination path without query parameters.</param>
        /// <returns></returns>
        public static string RewriteUrlWithParameters(Uri sourcePathAndQuery, string destinationPath)
        {
            return destinationPath + "?" + sourcePathAndQuery.Query;
        }        
    }
}
