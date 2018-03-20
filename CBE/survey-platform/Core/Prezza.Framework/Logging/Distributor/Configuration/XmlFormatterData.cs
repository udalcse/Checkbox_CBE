//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Xml;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Formatters;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Configuration information for the Xml log formatter.
	/// </summary>
	public class XmlFormatterData : FormatterData, IXmlConfigurationBase
	{
		/// <summary>
		/// Template for text log entries
		/// </summary>
		private XmlCDataSection templateData;


		/// <summary>
		/// Constructor.
		/// </summary>
		public XmlFormatterData() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log formatter configuration.</param>
		/// <param name="templateData">Template for log entries.</param>
		public XmlFormatterData(string name, string templateData) : base(name, typeof(XmlFormatter).AssemblyQualifiedName)
		{
			Template.Value = templateData;
		}

		/// <summary>
		/// Get/Set the template for log entries.
		/// </summary>
		public XmlCDataSection Template
		{
			get
			{
				if(templateData == null)
				{
					XmlDocument doc = new XmlDocument();
					templateData = doc.CreateCDataSection(string.Empty);
				}

				return this.templateData;
			}
			set
			{
				this.templateData = value;
			}
		}

		#region IXmlConfigurationBase Members

		/// <summary>
		/// Load the configuration of the formatter from the specified Xml node.
		/// </summary>
		/// <param name="node">XmlNode containing Xml log formatter configuration information.</param>
		public void LoadFromXml(System.Xml.XmlNode node)
		{
			ArgumentValidation.CheckForNullReference(node, "node");

			Template.Value = XmlUtility.GetNodeText(node.SelectSingleNode("/xmlLogFormatterConfiguration/templateData"), false);
		}

		#endregion
	}
}
