using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Checkbox.Globalization.Text;
using Checkbox.Management;
using Prezza.Framework.Common;

namespace Checkbox.Web
{
	/// <summary>
	/// XmlResourceManager handles functionality relating to the resource files for the application.  These files include 
	/// string.xml files, xml resource files, language lists, etc... 
	/// </summary>
	public static class XmlResourceManager
	{
        private static readonly Hashtable ResourceFileCache = new Hashtable();
		
		/// <summary>
		/// Loads a resource file into memory and object cache
		/// </summary>
		/// <param name="resource"></param>
		private static void LoadResource(string resource)
		{
            string resourcefile = Configuration.DiscoverApplicationRoot() + string.Format("{0}Resources{0}", System.IO.Path.DirectorySeparatorChar) + resource + ".xml";

			XmlDocument codeDependentResource = new XmlDocument();
			codeDependentResource.Load(resourcefile);
			ResourceFileCache[resource] = codeDependentResource;
            
		}

		/// <summary>
		/// Gets the CodeDependent resource file
		/// </summary>
		private static XmlDocument CodeDependentResource
		{
			get
			{
				if(ResourceFileCache["CodeDependentResources"] == null)
					LoadResource("CodeDependentResources");
				return (XmlDocument)ResourceFileCache["CodeDependentResources"];
			}
		}

		/// <summary>
		/// Gets an element of the CodeDependentResource xml document using arguments specified as an array
		/// </summary>
		/// <param name="args">The xpath arguments used in building the xpath expression</param>
		/// <returns>An <see cref="XmlNode"/> from the CodeDependentResources.xml file</returns>
		public static XmlNode GetCodeDependentResource(params string[] args)
		{
			StringBuilder xpath = new StringBuilder("/CodeDependentResources");
		
			for(int i = 0; i < args.Length; i++)
			{
				xpath.Append("/" + args[i]);
			}
			return CodeDependentResource.SelectSingleNode(xpath.ToString());
		}

        /// <summary>
        /// Get a list of values from the code dependent resource based on a selection
        /// of nodes.
        /// </summary>
        /// <param name="xPath">XPath for select.</param>
        /// <returns>List of node or attribute values, depending on xpath.</returns>
        public static List<string> GetCodeDependentResourceValues(string xPath)
        {
            List<string> values = new List<string>();

            XmlNodeList resourceNodes = CodeDependentResource.SelectNodes("/CodeDependentResources/" + xPath);

            if (resourceNodes != null)
            {
                foreach (XmlNode node in resourceNodes)
                {
                    values.Add(XmlUtility.GetNodeText(node));
                }
            }

            return values;
        }

        /// <summary>
        /// Get the default width for a specified single line text format.
        /// </summary>
        /// <param name="format">The text box's format.</param>
        /// <returns>The default width for a given format.</returns>
        public static int GetSingleLineTextDefaultWidth(string format)
        {
            int defaultWidth = ApplicationManager.AppSettings.DefaultSingleLineTextWidth;

            if (format != null)
            {
                format = format.ToLower().Trim();
                string xpath = string.Format("/SingleLineTextDefaultWidths/{0}", format);
                XmlDocument doc = SelectResource("SingleLineTextDefaultWidths");
                XmlNode node = doc.SelectSingleNode(xpath);

                if (node != null)
                {
                    int value;
                    if (Int32.TryParse(node.InnerText, out value))
                    {
                        defaultWidth = value;
                    }
                }
            }

            return defaultWidth;
        }

		/// <summary>
		/// Gets an element of the CodeDependentResource xml document using an xpath expression
		/// </summary>
		/// <param name="xpath">The XPath expression</param>
		/// <returns>An <see cref="XmlNode"/> from the CodeDependentResources.xml file</returns>
		public static XmlNode GetCodeDependentResourceUsingXPath(string xpath)
		{
			return CodeDependentResource.SelectSingleNode(xpath);
		}

	    /// <summary>
	    /// Gets an <see cref="XmlDocument"/> representation of a resource file given the name of the resource
	    /// </summary>
	    /// <param name="resource">The file name of the resource</param>
	    /// <returns><see cref="XmlDocument"/></returns>
	    private static XmlDocument SelectResource(string resource)
	    {
	        if (ResourceFileCache[resource] == null)
	        {
	            LoadResource(resource);
	        }

	        return (XmlDocument)ResourceFileCache[resource];
	    }

	    /// <summary>
		/// Gets an <see cref="XmlDocument"/> representation of a resource file given the name of the resource
		/// </summary>
		/// <param name="resource">The file name of the resource</param>
		/// <returns><see cref="XmlDocument"/></returns>
		private static XmlDocument SelectLocalizedResource(string resource)
		{
            string language = TextManager.DefaultLanguage;

            if (ResourceFileCache[resource] == null)
            {
                LoadResource(resource + "_" + language);
            }

			return (XmlDocument)ResourceFileCache[resource + "_" + language];
            
		}

	    /// <summary>
		/// Gets an <see cref="XmlNode"/> element from a resource using the resource filename, the child element to search, and the node to find
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="category"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static XmlNode SelectSingleLocalizedResource(string resource, string category, string node)
		{
            string language = TextManager.DefaultLanguage;

			return SelectLocalizedResource(resource).SelectSingleNode("//" + resource + "_" + language + "/" + category + "/" + node);
		}
	}
}
