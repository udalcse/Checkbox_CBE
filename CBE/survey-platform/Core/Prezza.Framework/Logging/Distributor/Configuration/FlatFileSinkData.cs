//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Common;
using Prezza.Framework.Logging.Sinks;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Configuration for the flat file log sink.
	/// </summary>
	public class FlatFileSinkData : SinkData, IXmlConfigurationBase
	{
		/// <summary>
		/// Log file path.
		/// </summary>
		private string fileName = string.Empty;

		/// <summary>
		/// Header for messages written to the log.
		/// </summary>
		private string header = string.Empty;

		/// <summary>
		/// Footer for messages written to the log.
		/// </summary>
		private string footer = string.Empty;

		/// <summary>
		/// Constructor.
		/// </summary>
		public FlatFileSinkData()
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the configuration instance.</param>
		public FlatFileSinkData(string name) : this(name, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the configuration instance.</param>
		/// <param name="fileName">Log file path.</param>
		public FlatFileSinkData(string name, string fileName) : this(name, string.Empty, string.Empty, string.Empty)
		{
			this.fileName = fileName;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the configuration instance.</param>
		/// <param name="fileName">Log file path.</param>
		/// <param name="header">Log message header.</param>
		/// <param name="footer">Log message footer.</param>
		public FlatFileSinkData(string name, string fileName, string header, string footer) : base(name, typeof(FlatFileSink).AssemblyQualifiedName)
		{
			this.fileName = fileName;
			this.header = header;
			this.footer = footer;
		}

		/// <summary>
		/// Log file path.
		/// </summary>
		public string FileName
		{
			get{return fileName;}
			set{fileName = value;}
		}

		/// <summary>
		/// Log message header.
		/// </summary>
		public string Header
		{
			get{return header;}
			set{header = value;}
		}

		/// <summary>
		/// Log message footer.
		/// </summary>
		public string Footer
		{
			get{return footer;}
			set{footer = value;}
		}
		#region IXmlConfigurationBase Members

		/// <summary>
		/// Load the sink configuration from the specified Xml node.
		/// </summary>
		/// <param name="node">XmlNode containing flat file sink configuration information.</param>
		public void LoadFromXml(System.Xml.XmlNode node)
		{
			fileName = XmlUtility.GetNodeText(node.SelectSingleNode("/flatFileSinkConfiguration/logFile"), true);
			header = XmlUtility.GetNodeText(node.SelectSingleNode("/flatFileSinkConfiguration/header"));
			footer = XmlUtility.GetNodeText(node.SelectSingleNode("/flatFileSinkConfiguration/footer"));
		}

		#endregion
	}
}
