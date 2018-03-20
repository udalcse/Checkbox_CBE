//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;

namespace Prezza.Framework.Logging.Distributor
{
	/// <summary>
	/// Factory object for creating and initialized implementations of <see cref="ILogDistributionStrategy"/>.
	/// </summary>
	public class LogDistributionStrategyFactory : ProviderFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LogDistributionStrategyFactory() : this((ConfigurationBase)ConfigurationManager.GetConfiguration("logDistributionStrategyConfiguration"))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config">Log distribution strategy configuration file containing information about the <see cref="ILogDistributionStrategy"/> implementing objects that the factory can create.</param>
		public LogDistributionStrategyFactory(ConfigurationBase config) : base("logDistributionStrategyFactory", typeof(ILogDistributionStrategy), config)
		{
		}

		/// <summary>
		/// Create the distribution strategy object with the specified name.
		/// </summary>
		/// <param name="distributionName">Name of the distribution strategy object to create.</param>
		/// <returns></returns>
		public ILogDistributionStrategy CreateDistributionStrategy(string distributionName)
		{
			ArgumentValidation.CheckForEmptyString(distributionName, "distributionName");

			return (ILogDistributionStrategy)base.CreateInstance(distributionName);
		}

		/// <summary>
		/// Get the DistributionStrategyData object with the specified name.
		/// </summary>
		/// <param name="providerName">Name of the distribution strategy data object to retrieve.</param>
		/// <returns></returns>
		protected override ConfigurationBase GetConfigurationObject(string providerName)
		{
			LoggingConfiguration loggingConfig = (LoggingConfiguration)base.Config;
			DistributionStrategyData data = loggingConfig.GetDistributionStrategyData(providerName);
			return data;
		}

		/// <summary>
		/// For the specified distribution strategy name, get the <see cref="System.Type"/> of the corresponding
		/// <see cref="ILogDistributionStrategy"/> configuration.
		/// </summary>
		/// <param name="configurationName"></param>
		/// <returns></returns>
		protected override Type GetConfigurationType(string configurationName)
		{
			DistributionStrategyData data = (DistributionStrategyData)GetConfigurationObject(configurationName);
			return GetType(data.TypeName);
		}
	}
}
