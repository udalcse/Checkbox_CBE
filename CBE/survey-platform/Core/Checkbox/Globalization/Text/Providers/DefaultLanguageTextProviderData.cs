//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Xml;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Checkbox.Globalization.Text.Providers
{
	/// <summary>
	/// Configuration information for the <see cref="DefaultLanguageTextProvider"/>.
	/// </summary>
	public class DefaultLanguageTextProviderData : ProviderData, IXmlConfigurationBase
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="providerName">Name of the provider instance.</param>
		public DefaultLanguageTextProviderData(string providerName) : base(providerName, typeof(DefaultLanguageTextProvider).AssemblyQualifiedName)
		{
			//Validate argument
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
		}

		/// <summary>
		/// Load authentication provider configuration from the supplied Xml node.
		/// </summary>
		/// <param name="providerNode"><see cref="XmlNode"/> containing <see cref="DefaultLanguageTextProvider"/> configuration information.</param>
		public void LoadFromXml(XmlNode providerNode)
		{
			ArgumentValidation.CheckForNullReference(providerNode, "providerNode");

			XmlNode defaultLanguageNode = providerNode.SelectSingleNode("defaultLanguage");

			if(defaultLanguageNode == null || DefaultLanguage == string.Empty)
				throw new Exception("No default language was specified in configuration for text provider:  " + providerNode.Name);

			DefaultLanguage = defaultLanguageNode.InnerText;

			XmlNode cacheManagerNameNode = providerNode.SelectSingleNode("cacheManager");

			CacheManagerName = cacheManagerNameNode == null ? string.Empty : cacheManagerNameNode.InnerText;
			
		}

	    /// <summary>
	    /// Get the default text language
	    /// </summary>
	    public string DefaultLanguage { get; private set; }

	    /// <summary>
	    /// Get/Set the name of the cache manager to use
	    /// </summary>
	    public string CacheManagerName { get; set; }
	}
}
