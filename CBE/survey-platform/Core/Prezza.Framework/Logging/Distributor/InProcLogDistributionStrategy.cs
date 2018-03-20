//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Formatters;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Distributor
{
	/// <summary>
	/// Log distribution strategy that supports sending logging messages to in process components.
	/// </summary>
	public class InProcLogDistributionStrategy : ConfigurationProvider, ILogDistributionStrategy 
	{
		/// <summary>
		/// Log distributor to process the log and send the messages to the appropriate destinations.
		/// </summary>
		private LogDistributor logDistributor;

		/// <summary>
		/// Constructor.
		/// </summary>
		public InProcLogDistributionStrategy()
		{
		}

		/// <summary>
		/// Initialize the log strategy with the specified InProcDistributionStrategyData object.
		/// </summary>
		/// <param name="config">Configuration information for the InProcDistributionStrategy.</param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(InProcDistributionStrategyData));
			
			this.logDistributor = new LogDistributor((LoggingConfiguration)((InProcDistributionStrategyData)config).LoggingConfiguration);
		}

		#region ILogDistributionStrategy Members

		/// <summary>
		/// Distribute the log to configured receivers.
		/// </summary>
		/// <param name="logEntry">Log entry to distribute.</param>
		public void SendLog(LogEntry logEntry)
		{
			logDistributor.ProcessLog(logEntry);
			
			//In the future, this is where we could add
			// support for logging distributed events 
		}

		#endregion
	}
}
