using System;
using System.Web;

using Checkbox.Management;
using Checkbox.Forms.Items;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Web-specific implementation of an item formatter
    /// </summary>
    [Serializable]
    public abstract class WebItemFormatter : XsltItemFormatter
    {
        /// <summary>
        /// Get the xsl file path to use for formatting
        /// </summary>
        /// <param name="format">Desired format for output.</param>
        /// <returns>File location of xsl tranform file</returns>
        protected override string GetXslFilePath(string format)
        {
            string basePath = HttpContext.Current.Server.MapPath(
                ApplicationManager.ApplicationRoot + "/Forms/Xsl/");

            if (format.Equals("Html", StringComparison.InvariantCultureIgnoreCase))
            {
                return basePath + HtmlTransformFileName;
            }
            
            if (format.Equals("Text", StringComparison.InvariantCultureIgnoreCase))
            {
                return basePath + TextTransformFileName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the name of the Xsl file to transform item into
        /// HTML format.  This file is expected to live in the
        /// Forms/Xsl folder of the web application.
        /// </summary>
        protected abstract string HtmlTransformFileName { get; }

        /// <summary>
        /// Get the name of the Xsl file to transform item into
        /// text format.  This file is expected to live in the
        /// Forms/Xsl folder of the web application.
        /// </summary>
        protected abstract string TextTransformFileName { get; }
    }
}
