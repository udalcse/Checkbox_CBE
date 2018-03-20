//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Filters;
using Prezza.Framework.Logging.Distributor;
using Prezza.Framework.Logging.Configuration;

namespace Prezza.Framework.Logging
{
	/// <summary>
	/// Instantiate the configured log distribution strategy and cause it to send the log entry
	/// to configured sinks and destinations.
	/// </summary>
	public class LogWriter
	{
		/// <summary>
		/// Framework logging configuration object.
		/// </summary>
		private LoggingConfiguration loggingConfiguration;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config">Object containing the framework logging configuration.</param>
		public LogWriter(ConfigurationBase config)
		{
			ArgumentValidation.CheckExpectedType(config, typeof(LoggingConfiguration));
			this.loggingConfiguration = (LoggingConfiguration)config;
		}

/*		Context items are used for remoting, which is not currently supported 
 * 		public void SetContextItem(object key, object value)
		{
			ArgumentValidation.CheckForNullReference(key, "key");
			ArgumentValidation.CheckForNullReference(value, "value");

			ContextItems items = new ContextItems();
			items.SetContextItem(key, value);
		}

		public void FlushContextItems()
		{
		}
		*/

		/// <summary>
		/// Write the log entry by using a distribution strategy to send it to configured log sinks, but only if the logging configuration specifies that logging is enabled.
		/// </summary>
		/// <param name="log">Log entry to write.</param>
		public void Write(LogEntry log)
		{
			if(!loggingConfiguration.LoggingEnabled)
			{
				return;
			}

			LogFilter filters = new LogFilter(loggingConfiguration);

			if(filters.CheckFilters(log))
			{
				SendMessage(log);
			}
		}

		/// <summary>
		/// Use a distribution strategy to send a log entry to configured log sinks.
		/// </summary>
		/// <param name="logEntry">Log entry to send.</param>
		private void SendMessage(LogEntry logEntry)
		{
			LogDistributionStrategyFactory factory = new LogDistributionStrategyFactory(loggingConfiguration);
			ILogDistributionStrategy strategy = factory.CreateDistributionStrategy(loggingConfiguration.DistributionStrategy);
			strategy.SendLog(logEntry);
		}
	}
}
