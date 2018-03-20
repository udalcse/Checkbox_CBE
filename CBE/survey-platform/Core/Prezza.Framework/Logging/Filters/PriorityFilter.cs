//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;

namespace Prezza.Framework.Logging.Filters
{
	internal class PriorityFilter : ILogFilter
	{
		private LoggingConfiguration loggingConfiguration;

		public PriorityFilter(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(LoggingConfiguration));
			
			loggingConfiguration = (LoggingConfiguration)config;
		}

		#region ILogFilter Members

		public bool Filter(LogEntry log)
		{
			int minPriority = loggingConfiguration.MinimumPriority;

			if(log.Priority < 0)
			{
				log.Priority = minPriority;
			}

			return(log.Priority >= minPriority);
		}

		#endregion
	}
}
