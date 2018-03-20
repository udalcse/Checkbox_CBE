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
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	///Put log entries in Xml format.
	/// </summary>
	public class XmlFormatter : ConfigurationProvider, ILogFormatter
	{
		/// <summary>
		/// Configuration information for the Xml formatter.
		/// </summary>
		private XmlFormatterData xmlFormatterData;

		/// <summary>
		/// Builder for formatting the messages.
		/// </summary>
		private StringBuilder templateBuilder;

		/// <summary>
		/// List of token functions to run when formatting the message.
		/// </summary>
		private ArrayList tokenFunctions;

		/// <summary>
		/// Timestamp.
		/// </summary>
		private const string timeStampToken = "{timestamp}";

		/// <summary>
		/// Message.
		/// </summary>
		private const string messageToken = "{message}";

		/// <summary>
		/// Category of the message.
		/// </summary>
		private const string categoryToken = "{category}";

		/// <summary>
		/// Priority of the message.
		/// </summary>
		private const string priorityToken = "{priority}";

		/// <summary>
		/// Event Id associated with the log entry.
		/// </summary>
		private const string eventIdToken = "{eventId}";

		/// <summary>
		/// Severity of the entry.
		/// </summary>
		private const string severityToken = "{severity}";

		/// <summary>
		/// Title to write before the log entry.
		/// </summary>
		private const string titleToken = "{title}";

		/// <summary>
		/// Machine Name.
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
		/// Thread Id.
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
		/// Default constructor.
		/// </summary>
		public XmlFormatter() : this(new XmlFormatterData())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="xmlFormatterData">Xml formatter configuration information.</param>
		public XmlFormatter(XmlFormatterData xmlFormatterData)
		{
			ArgumentValidation.CheckForNullReference(xmlFormatterData, "xmlFormatterData");

			this.xmlFormatterData = xmlFormatterData;
			RegisterTemplate();
		}

		/// <summary>
		/// Get/Set the builder used to format the message.
		/// </summary>
		internal StringBuilder TemplateBuilder
		{
			get{return templateBuilder;}
			set{templateBuilder = value;}
		}

		/// <summary>
		/// Initialize the xml formatter with its configuration information.
		/// </summary>
		/// <param name="config">Xml formatter configuration object.</param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(XmlFormatterData));

			xmlFormatterData = (XmlFormatterData)config;
			RegisterTemplate();
		}

		/// <summary>
		/// Create the builder based on the template specified in the configuration and register
		/// the token functions.
		/// </summary>
		private void RegisterTemplate()
		{
			templateBuilder = new StringBuilder(xmlFormatterData.Template.Value);
			RegisterTokenFunctions();
		}

		/// <summary>
		/// Get the formatter configuration.
		/// </summary>
		protected XmlFormatterData FormatterData
		{
			get{return xmlFormatterData;}
		}

		#region ILogFormatter Members

		/// <summary>
		/// Format the log entry according to the configuration of the formatter.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		/// <returns>Formatted log message.</returns>
		public virtual string Format(LogEntry log)
		{
			if(templateBuilder.Length == 0)
			{
				templateBuilder = new StringBuilder("<logMessageBlob><![CDATA[Timestamp: {timestamp}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventId}{newline}Severity: {severity}{newline}Title:{title}{newline}Machine: {machine}{newline}Application Domain: {appDomain}{newline}Process Id: {processId}{newline}Process Name: {processName}{newline}Win32 Thread Id: {win32ThreadId}{newline}Thread Name: {threadName}{newline}Extended Properties: {dictionary({key} - {value}{newline})}]]></logMessageBlob>");
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
		/// Run the token format functions on the log entry.
		/// </summary>
		/// <param name="log"></param>
		private void FormatTokenFunctions(LogEntry log)
		{
			foreach(TokenFunction token in tokenFunctions)
			{
				token.Format(templateBuilder, log);
			}
		}

		/// <summary>
		/// Create the list of token functions to run.  Includes <see cref="DictionaryToken"/>, <see cref="KeyValueToken"/>, 
		/// and <see cref="TimeStampToken"/>.
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
