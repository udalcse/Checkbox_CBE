//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;

using Prezza.Framework.Logging.Distributor;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// Configuration information for InProcDistributionStrategy.
	/// 
	/// </summary>
	public class InProcDistributionStrategyData : DistributionStrategyData
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public InProcDistributionStrategyData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log distribution strategy this configuration represents.</param>
		public InProcDistributionStrategyData(string name) : base(name, typeof(InProcLogDistributionStrategy).AssemblyQualifiedName)
		{
		}
	}
}
