//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Sinks;
using Prezza.Framework.Logging.Formatters;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Distributor
{
	/// <summary>
	/// Distribute log entries to designated (by configuration) log sinks.
	/// </summary>
	public class LogDistributor
	{
		/// <summary>
		/// Logging configuration information.
		/// </summary>
		private readonly LoggingConfiguration loggingConfig;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="loggingConfig">Logging configuration information.</param>
		public LogDistributor(LoggingConfiguration loggingConfig)
		{
			this.loggingConfig = loggingConfig;
		}

		/// <summary>
		/// Get the configuration object for the LogDistributor.
		/// </summary>
		protected ConfigurationBase ConfigurationObject
		{
			get{return loggingConfig;}
		}

		/// <summary>
		/// Process the specified log entry but verifying that filter constraints are met, creating the 
		/// appropriate log sink and log formatter, and sending the message to the log sink.
		/// </summary>
		/// <param name="log"></param>
		public void ProcessLog(LogEntry log)
		{
			CategoryData category = GetCategory(log);
			
			if(category == null)
			{
				return;
			}
			DistributeLogEntry(log, category);
		}

		/// <summary>
		/// Distribute the log entry to each destination of the specified category.
		/// </summary>
		/// <param name="log">Log entry to distribute.</param>
		/// <param name="category">Category of log entry.</param>
		public void DistributeLogEntry(LogEntry log, CategoryData category)
		{
			foreach(DestinationData destination in category.DestinationDataCollection)
			{
				ILogSink sink = CreateSink(destination.Sink);
				sink.Formatter = CreateFormatter(destination.Format);

				sink.SendMessage(log);
			}
		}

		/// <summary>
		/// Get the category of the log entry.  If the log entry category is not set, attempt to set the category
		/// to the default category specified in the configuration.
		/// </summary>
		/// <param name="log">Log entry to get the category of.</param>
		/// <returns>Category data.</returns>
		private CategoryData GetCategory(LogEntry log)
		{
			CategoryData data = null;

			if(log.Category.Length == 0)
			{
				data = loggingConfig.GetDefaultCategoryData();
				log.Category = data.Name;
			}

			CategoryData categoryData = loggingConfig.GetCategoryData(log.Category);

			if(categoryData == null)
			{
				categoryData = loggingConfig.GetDefaultCategoryData();
			}

			return categoryData;
		}

		/// <summary>
		/// Create an instance of the specified log formatter.
		/// </summary>
		/// <param name="formatterName">Name of the log formatter to create.</param>
		/// <returns>Instance of specified log formatter.</returns>
		private ILogFormatter CreateFormatter(string formatterName)
		{
			ILogFormatter formatter = null;
			FormatterData formatterData = null;

			if(formatterName != null && formatterName.Length > 0)
			{
				formatterData = loggingConfig.GetFormatterData(formatterName);
			}

			if(formatterData == null)
			{
				formatterData = loggingConfig.GetDefaultFormatterData();
			}

			if(formatterData != null)
			{
				LogFormatterFactory factory = new LogFormatterFactory(loggingConfig);
				formatter = factory.CreateFormatter(formatterData.Name);
			}

			return formatter;
		}

		/// <summary>
		/// Create an instance of the specified log sink.
		/// </summary>
		/// <param name="sinkName">Name of the log sink to create.</param>
		/// <returns>Instance of the specified log sink.</returns>
		public ILogSink CreateSink(string sinkName)
		{
			LogSinkFactory factory = new LogSinkFactory(loggingConfig);
			return factory.CreateSink(sinkName);
		}
	}
}
