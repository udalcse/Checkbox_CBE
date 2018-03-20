//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Formatters;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Sinks
{
	/// <summary>
	/// Base class for log sinks.
	/// </summary>
	public abstract class LogSink : ConfigurationProvider, ILogSink
	{
		/// <summary>
		/// Formatter for the log sink.
		/// </summary>
		private ILogFormatter formatter;

		/// <summary>
		/// Prepare and send a log message to the configured destinations.
		/// </summary>
		/// <param name="entry">Log entry to send</param>
		public void SendMessage(LogEntry entry)
		{
			PrepareMessage(entry);
			SendMessageCore(entry);
		}

		/// <summary>
		/// Get the log formatter for the sink.  It it does not exist (is null), create it.
		/// </summary>
		public virtual ILogFormatter Formatter
		{
			get
			{
				if(formatter == null)
				{
					formatter = new TextFormatter(new TextFormatterData());
				}

				return formatter;
			}
			set{formatter = value;}
		}

		/// <summary>
		/// Format the log entry according to the sink configuration.
		/// </summary>
		/// <param name="entry">Log entry to format.</param>
		/// <returns>Formatted log message.</returns>
		protected virtual string FormatEntry(LogEntry entry)
		{
			string formattedMessage = formatter.Format(entry);

			if(entry.ErrorMessages != null)
			{
				formattedMessage = entry.ErrorMessages.ToString() + "Message:  " + Environment.NewLine + formattedMessage;
			}

			return formattedMessage;
		}

		/// <summary>
		/// Prepare the message.
		/// </summary>
		/// <param name="entry"></param>
		protected virtual void PrepareMessage(LogEntry entry)
		{
			AddActivityIdToLogEntry(entry);
		}

		/// <summary>
		/// Send/Write the message.
		/// </summary>
		/// <param name="entry">Log entry to send/write.</param>
		protected abstract void SendMessageCore(LogEntry entry);

		/// <summary>
		/// Add an activity Id to the log message.
		/// </summary>
		/// <param name="entry">Log entry to add an activity Id to</param>
		protected void AddActivityIdToLogEntry(LogEntry entry)
		{
			//Do nothing until this can be figured out.
		}
	}
}
