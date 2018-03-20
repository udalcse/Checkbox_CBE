//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Distributor
{
	/// <summary>
	/// Interface for log distribution strategies.
	/// </summary>
	public interface ILogDistributionStrategy : IConfigurationProvider
	{
		/// <summary>
		/// Send a log entry to the appropriate destination.
		/// </summary>
		/// <param name="log">Log entry to distribute.</param>
		void SendLog(LogEntry log);
	}
}
