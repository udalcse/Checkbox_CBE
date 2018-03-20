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
	/// Configuration information for the <see cref="MultiLanguageTextProvider"/>.
	/// </summary>
	public class MultiLanguageTextProviderData : ProviderData, IXmlConfigurationBase
	{
		/// <summary>
		/// Enum to define export format
		/// </summary>
		public enum ExportFormat
		{
			XML = 1,
			CSV
		}

		/// <summary>
		/// Export format for exported text
		/// </summary>
		private ExportFormat textExportFormat;

		/// <summary>
		/// Langage to get/set text for
		/// </summary>
		private string defaultLanguage;

		/// <summary>
		/// Name of the cache to use for text
		/// </summary>
		private string cacheManagerName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="providerName">Name of the provider instance.</param>
		public MultiLanguageTextProviderData(string providerName) : base(providerName, typeof(MultiLanguageTextProvider).AssemblyQualifiedName)
		{
			//Validate argument
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
		}

		/// <summary>
		/// Load authentication provider configuration from the supplied Xml node.
		/// </summary>
		/// <param name="providerNode"><see cref="XmlNode"/> containing <see cref="MultiLanguageTextProvider"/> configuration information.</param>
		public void LoadFromXml(XmlNode providerNode)
		{
			ArgumentValidation.CheckForNullReference(providerNode, "providerNode");

			XmlNode defaultLanguageNode = providerNode.SelectSingleNode("defaultLanguage");

			if(defaultLanguageNode == null || defaultLanguageNode.InnerText == string.Empty)
				throw new Exception("No default langauge was specified in configuration for text provider:  " + providerNode.Name);

			defaultLanguage = defaultLanguageNode.InnerText;

			XmlNode cacheManagerNameNode = providerNode.SelectSingleNode("cacheManager");

			if(cacheManagerNameNode == null)
				cacheManagerName = string.Empty;
			else
				cacheManagerName = cacheManagerNameNode.InnerText;

			XmlNode exportFormatNode = providerNode.SelectSingleNode("exportFormat");

			if(exportFormatNode == null)
			{
				this.textExportFormat = ExportFormat.XML;
			}
			else
			{
				this.textExportFormat = (ExportFormat)Enum.Parse(typeof(ExportFormat), exportFormatNode.InnerText, true);
			}
			
		}

        /// <summary>
		/// Get the default text language
		/// </summary>
		public string DefaultLanguage
		{
			get{ return this.defaultLanguage; }
			set{ this.defaultLanguage = value; }
		}
		
		/// <summary>
		/// Get/Set the name of the cache manager to use
		/// </summary>
		public string CacheManagerName
		{
			get{ return this.cacheManagerName; }
			set{this.cacheManagerName = value; }
		}	

		/// <summary>
		/// Get/Set the export format for text data
		/// </summary>
		public ExportFormat TextExportFormat
		{
			get{ return this.textExportFormat;}
			set{ this.textExportFormat = value; }
		}
	}
}
