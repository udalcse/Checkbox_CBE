//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.ComponentModel;


namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Destination information for log categories.
	/// </summary>
	public class DestinationData
	{
		/// <summary>
		/// Name of the destination.
		/// </summary>
		private string name;

		/// <summary>
		/// Log Sink to route messages to.
		/// </summary>
		private string sink;

		/// <summary>
		/// Format of messages.
		/// </summary>
		private string format;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DestinationData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the destination.</param>
		public DestinationData(string name) : this(name, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the destination.</param>
		/// <param name="sink">Name of the sink for the destination.</param>
		public DestinationData(string name, string sink) : this(name, sink, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the destination.</param>
		/// <param name="sink">Name of the sink for the destination.</param>
		/// <param name="format">Format of messages for this destination.</param>
		public DestinationData(string name, string sink, string format)
		{
			this.name = name;
			this.sink = sink;
			this.format = format;
		}

		/// <summary>
		/// Name of the destination.
		/// </summary>
		public string Name
		{
			get{return name;}
			set{name = value;}
		}

		/// <summary>
		/// Name of the Log Sink for the destination.
		/// </summary>
		public string Sink
		{
			get{return sink;}
			set{sink = value;}
		}

		/// <summary>
		/// Format of the messages for the destination.
		/// </summary>
		public string Format
		{
			get{return format;}
			set{format = value;}
		}
	}
}
