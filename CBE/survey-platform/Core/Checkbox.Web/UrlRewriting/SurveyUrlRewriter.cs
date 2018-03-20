using System;
using System.Collections.Generic;

using Checkbox.Forms;
using Checkbox.Management;


namespace Checkbox.Web.UrlRewriting
{
    /// <summary>
    /// Rewriter for survey Urls.  Only matches URLs based on path w/out query string.
    /// </summary>
    public class SurveyUrlRewriter : UrlRewriter
    {
        /// <summary>
        /// Rewrite the survey URL based on the portion of the requested path w/out the query string.  Any query
        /// string parameters that are part of the requestedPath will be appended to the survey url.
        /// </summary>
        /// <param name="requestedPath">URL requested.</param>
        /// <param name="app">HttpApplication context.</param>
        protected override void RewriteUrl(string requestedPath, System.Web.HttpApplication app)
        {
            if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                string newPath = UrlMapper.GetMapping(requestedPath);

                if (newPath != requestedPath) 
                {
                    //Add query string and expect that new path has query param for survey attached.
                    if (app.Context.Request.ServerVariables["QUERY_STRING"] != null && app.Context.Request.ServerVariables["QUERY_STRING"].Trim() != string.Empty)
                    {
                        newPath += "&" + app.Context.Request.ServerVariables["QUERY_STRING"];
                    }

                    app.Context.RewritePath(newPath);
                }
            }
        }
    }
}
