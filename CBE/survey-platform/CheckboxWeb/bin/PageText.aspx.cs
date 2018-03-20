using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb
{
    /// <summary>
    /// Simple helper to emit page text to javascript helper class.
    /// </summary>
    public partial class PageText : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string pagePath = Request.QueryString["p"];

            if (string.IsNullOrEmpty(pagePath))
            {
                return;
            }

            Dictionary<string, string> pageTexts = TextManager.GetPageTexts(pagePath, WebTextManager.GetUserLanguage());

            //Write headers
            Response.Expires = -1;
            Response.BufferOutput = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/javascript";
            Response.ContentEncoding = Encoding.UTF8;

            //Initialize helper object with debug mode
            Response.Write(string.Format("textHelper.initialize({0});", (ApplicationManager.AppSettings.LanguageDebugMode ? "true": "false")));
            Response.Write(Environment.NewLine);

            foreach (var key in pageTexts.Keys)
            {
                Response.Write(string.Format("textHelper.setTextValue('{0}', '{1}');", key, pageTexts[key].Replace("'", "\\'")));
                Response.Write(Environment.NewLine);
            }

            Response.End();
        }
    }
}