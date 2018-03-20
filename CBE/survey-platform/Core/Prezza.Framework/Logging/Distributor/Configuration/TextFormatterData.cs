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
	/// Container for the <see cref="TextFormatter"/>'s configuration information.
	/// </summary>
	public class TextFormatterData : FormatterData, IXmlConfigurationBase
	{
		/// <summary>
		/// Template for text log entries
		/// </summary>
		private XmlCDataSection templateData;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TextFormatterData() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the formatter.</param>
		/// <param name="templateData">Template for text-formatted log entries.</param>
		public TextFormatterData(string name, string templateData) : base(name, typeof(TextFormatter).AssemblyQualifiedName)
		{
			Template.Value = templateData;
		}

		/// <summary>
		/// Get/Set the text format template.
		/// </summary>
		public XmlCDataSection Template
		{
			get
			{
				if(templateData == null)
				{
					XmlDocument doc = new XmlDocument();
					this.templateData = doc.CreateCDataSection(string.Empty);
				}

				return this.templateData;
			}

			set
			{
				this.templateData = value;
			}
		}

		/// <summary>
		/// Type name of the log formatter.
		/// </summary>
		public override string TypeName
		{
			get{return base.TypeName;}
		}
		#region IXmlConfigurationBase Members

		/// <summary>
		/// Load the configuration for the <see cref="TextFormatter"/>.
		/// </summary>
		/// <param name="node"></param>
		public void LoadFromXml(XmlNode node)
		{
			ArgumentValidation.CheckForNullReference(node, "node");

			Template.Value = XmlUtility.GetNodeText(node.SelectSingleNode("/textLogFormatterConfiguration/templateData"), false);
		}

		#endregion
	}
}
