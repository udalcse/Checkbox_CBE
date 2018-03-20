//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Text;
using System.Collections;
using System.Globalization;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Formatter for text log entries.
	/// </summary>
	public class TextFormatter : ConfigurationProvider, ILogFormatter
	{
		/// <summary>
		/// Configuration information for the text formatter.
		/// </summary>
		private TextFormatterData textFormatterData;

		/// <summary>
		/// Builder for the text format template.
		/// </summary>
		private StringBuilder templateBuilder;

		/// <summary>
		/// List of token functions used by the text formatter.
		/// </summary>
		private ArrayList tokenFunctions;

		/// <summary>
		/// System time the log entry was created.
		/// </summary>
		private const string timeStampToken = "{timestamp}";

		/// <summary>
		/// Message to log.
		/// </summary>
		private const string messageToken = "{message}";

		/// <summary>
		/// Category of message.
		/// </summary>
		private const string categoryToken = "{category}";

		/// <summary>
		/// Priority of message.
		/// </summary>
		private const string priorityToken = "{priority}";

		/// <summary>
		/// Event Id associated with the log entry.
		/// </summary>
		private const string eventIdToken = "{eventId}";

		/// <summary>
		/// Severity of the log entry.
		/// </summary>
		private const string severityToken = "{severity}";

		/// <summary>
		/// Title to write for the log entry.
		/// </summary>
		private const string titleToken = "{title}";

		/// <summary>
		/// Machine name the log entry was created on.
		/// </summary>
		private const string machineToken = "{machine}";

		/// <summary>
		/// Application domain.
		/// </summary>
		private const string appDomainToken = "{appDomain}";

		/// <summary>
		/// Process Id.
		/// </summary>
		private const string processIdToken = "{processId}";

		/// <summary>
		/// Process Name.
		/// </summary>
		private const string processNameToken = "{processName}";

		/// <summary>
		/// Thread Name.
		/// </summary>
		private const string threadNameToken = "{threadName}";

		/// <summary>
		/// Win32 Thread Id
		/// </summary>
		private const string win32ThreadIdToken = "{win32ThreadId}";

		/// <summary>
		/// New line.
		/// </summary>
		private const string NewLineToken = "{newline}";

		/// <summary>
		/// Tab.
		/// </summary>
		private const string TabToken = "{tab}";

		/// <summary>
		/// Constructor.
		/// </summary>
		public TextFormatter() : this(new TextFormatterData())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="textFormatterData">Text log configuration information.</param>
		public TextFormatter(TextFormatterData textFormatterData)
		{
			ArgumentValidation.CheckForNullReference(textFormatterData, "textFormatterData");

			this.textFormatterData = textFormatterData;	
			RegisterTemplate();
		}

		/// <summary>
		/// Get the builder to use for formatting the message.
		/// </summary>
		internal StringBuilder TemplateBuilder
		{
			get{return templateBuilder;}
			set{templateBuilder = value;}
		}

		/// <summary>
		/// Initialize the text formatter with its configuration.
		/// </summary>
		/// <param name="config">Configuration object for the text formatter.</param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(TextFormatterData));

			textFormatterData = (TextFormatterData)config;
			RegisterTemplate();
		}

		/// <summary>
		/// Create a new builder based on the text formatter's template and register the token functions.
		/// </summary>
		private void RegisterTemplate()
		{
			templateBuilder = new StringBuilder(textFormatterData.Template.Value);
			RegisterTokenFunctions();
		}

		/// <summary>
		/// Get the configuration of the text formatter.
		/// </summary>
		protected TextFormatterData FormatterData
		{
			get{return textFormatterData;}
		}

		#region ILogFormatter Members

		/// <summary>
		/// Format the log entry based on the formatter configuration.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		/// <returns>String containing the formatted log entry.</returns>
		public virtual string Format(LogEntry log)
		{
			if(templateBuilder.Length == 0)
			{
				templateBuilder = new StringBuilder("Timestamp: {timestamp}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventId}{newline}Severity: {severity}{newline}Title:{title}{newline}Machine: {machine}{newline}Application Domain: {appDomain}{newline}Process Id: {processId}{newline}Process Name: {processName}{newline}Win32 Thread Id: {win32ThreadId}{newline}Thread Name: {threadName}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");
			}

			templateBuilder.Replace(timeStampToken, log.TimeStampString);
			templateBuilder.Replace(titleToken,  log.Title);
			templateBuilder.Replace(messageToken, log.Message);
			templateBuilder.Replace(categoryToken, log.Category);
			templateBuilder.Replace(eventIdToken, log.EventId.ToString());
			templateBuilder.Replace(priorityToken, log.Priority.ToString());
			templateBuilder.Replace(severityToken, log.Severity.ToString());
				
			templateBuilder.Replace(machineToken, log.MachineName);
			templateBuilder.Replace(appDomainToken, log.AppDomainName);
			templateBuilder.Replace(processIdToken, log.ProcessId);
			templateBuilder.Replace(processNameToken, log.ProcessName);
			templateBuilder.Replace(win32ThreadIdToken, log.Win32ThreadId);
			templateBuilder.Replace(threadNameToken, log.ManagedThreadName);
			
			FormatTokenFunctions(log);

			templateBuilder.Replace(NewLineToken, Environment.NewLine);
			templateBuilder.Replace(TabToken, "\t");

			return templateBuilder.ToString();
		}

		/// <summary>
		/// Run each of the token functions.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		private void FormatTokenFunctions(LogEntry log)
		{
			foreach(TokenFunction token in tokenFunctions)
			{
				token.Format(templateBuilder, log);
			}
		}

		/// <summary>
		/// Set the token functions for the formatter. Currently, these are <see cref="DictionaryToken"/>,
		/// <see cref="KeyValueToken"/>, and <see cref="TimeStampToken"/>.
		/// </summary>
		private void RegisterTokenFunctions()
		{
			tokenFunctions = new ArrayList();

			tokenFunctions.Add(new DictionaryToken());
			tokenFunctions.Add(new KeyValueToken());
			tokenFunctions.Add(new TimeStampToken());
		}

		#endregion
	}
}
