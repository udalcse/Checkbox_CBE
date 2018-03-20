//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Distributor;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// Abstract base class for <see cref="ILogDistributionStrategy" /> configuration data.
	/// </summary>
	public abstract class DistributionStrategyData : ProviderData
	{
		private LoggingConfiguration loggingConfiguration;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the distribution strategy.</param>
		/// <param name="typeName">Type name of the implmentation of <see cref="ILogDistributionStrategy"/>.</param>
		protected DistributionStrategyData(string name, string typeName) : base(name, typeName)
		{
		}

		/// <summary>
		/// Get/Set the configuration object for the distribution strategy data.
		/// </summary>
		public virtual LoggingConfiguration LoggingConfiguration
		{
			get{return loggingConfiguration;}
			set{loggingConfiguration = value;}
		}

	}
}
