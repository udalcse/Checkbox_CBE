//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Logging.Formatters;

namespace Prezza.Framework.Logging.Sinks
{
	/// <summary>
	/// All log sinks used by the logging framework implement this interface.
	/// </summary>
	public interface ILogSink
	{
		/// <summary>
		/// Send the log entry to the sink.
		/// </summary>
		/// <param name="log">Log entry to send.</param>
		void SendMessage(LogEntry log);

		/// <summary>
		/// Get/Set the log formatter for the sink.
		/// </summary>
		ILogFormatter Formatter{get; set;}
	}
}
