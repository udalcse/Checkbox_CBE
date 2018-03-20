//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging
{
	/// <summary>
	/// Logger of messages for the framework.
	/// </summary>
	public sealed class Logger
	{
		/// <summary>
		/// Sync object to ensure only one thread creates a writer object.
		/// </summary>
		private static object sync = new object();

		/// <summary>
		/// Writer to use to write log messages.
		/// </summary>
		private static volatile LogWriter writer;
		
		/// <summary>
		/// Default priority for log messages.
		/// </summary>
		private const int DefaultPriority = -1;

		/// <summary>
		/// Default severity for log messages.
		/// </summary>
		private const Severity DefaultSeverity = Severity.Unspecified;

		/// <summary>
		/// Default event id to associate with log messages.
		/// </summary>
		private const int DefaultEventId = 1;

		/// <summary>
		/// Default title for log messages.
		/// </summary>
		private const string DefaultTitle = "";

		/// <summary>
		/// Constructor.  Since all methods are static, this is private.
		/// </summary>
		private Logger()
		{
		}

		static Logger()
		{
			ConfigurationManager.ConfigurationChanged += new ConfigurationChangedEventHandler(ConfigurationManager_ConfigurationChanged);
		}


/*		public static void SetContextItem(object key, object value)
		{
		}

		public static void FlushContextItems()
		{
		} */

		/// <summary>
		/// Write a message using default parameters.
		/// </summary>
		/// <param name="message">Message to write.</param>
		public static void Write(object message)
		{
			Write(message, "", DefaultPriority, DefaultEventId, DefaultSeverity, DefaultTitle, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		public static void Write(object message, string category)
		{
			Write(message, category, DefaultPriority, DefaultEventId, DefaultSeverity, DefaultTitle, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the message.</param>
		public static void Write(object message, string category, int priority)
		{
			Write(message, category, priority, DefaultEventId, DefaultSeverity, DefaultTitle, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the message.</param>
		/// <param name="eventId">Event Id associated with the message.</param>
		public static void Write(object message, string category, int priority, int eventId)
		{
			Write(message, category, priority, eventId, DefaultSeverity, DefaultTitle, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the message.</param>
		/// <param name="eventId">Event Id associated with the message.</param>
		/// <param name="severity">Severity of the message.</param>
		public static void Write(object message, string category, int priority, int eventId, Severity severity)
		{
			Write(message, category, priority, eventId, severity, DefaultTitle, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the messgae.</param>
		/// <param name="eventId">Event Id associated with the message.</param>
		/// <param name="severity">Severity of the message.</param>
		/// <param name="title">Title of the message.</param>
		public static void Write(object message, string category, int priority, int eventId, Severity severity, string title)
		{
			Write(message, category, priority, eventId, severity, title, null);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="properties">Properties dictionary to containing properties and values to write.</param>
		public static void Write(object message, IDictionary properties)
		{
			Write(message, "", DefaultPriority, DefaultEventId, DefaultSeverity, DefaultTitle, properties);
		}
        
		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the message.</param>
		/// <param name="properties">Additional properties to write with the message.</param>
		public static void Write(object message, string category, int priority, IDictionary properties)
		{
			Write(message, category, priority, DefaultEventId, DefaultSeverity, DefaultTitle, properties);
		}

		/// <summary>
		/// Write a log message.
		/// </summary>
		/// <param name="message">Message to write.</param>
		/// <param name="category">Category of the message.</param>
		/// <param name="priority">Priority of the message.</param>
		/// <param name="eventId">Event Id associated with the message.</param>
		/// <param name="severity">Severity of the message.</param>
		/// <param name="title">Title of the message.</param>
		/// <param name="properties">Additional properties associated with the message.</param>
		public static void Write(object message, string category, int priority, int eventId, Severity severity, string title, IDictionary properties)
		{
			LogEntry entry = new LogEntry();
			entry.Message = message.ToString();
			entry.Category = category;
			entry.Priority = priority;
			entry.EventId = eventId;
			entry.Severity = severity;
			entry.Title = title;
			entry.ExtendedProperties = properties;

			Write(entry);
		}

		/// <summary>
		/// Write a log entry.
		/// </summary>
		/// <param name="log">Log entry to write.</param>
		public static void Write(LogEntry log)
		{
			try
			{
				Writer.Write(log);
			}
			catch
			{
			}
		}
		
		/// <summary>
		/// Get a log writer instance.
		/// </summary>
		private static LogWriter Writer
		{
			get
			{
				if(writer == null)
				{
					lock(sync)
					{
						if(writer == null)
						{
							writer = new LogWriter((ConfigurationBase)ConfigurationManager.GetConfiguration("checkboxLoggingConfiguration"));
						}
					}
				}

				return writer;
			}
		}

		private static void ConfigurationManager_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
		{
			writer = null;
		}
	}
}
