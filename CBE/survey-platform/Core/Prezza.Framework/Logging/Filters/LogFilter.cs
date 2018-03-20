//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Collections;

using Prezza.Framework.Logging;
using Prezza.Framework.Logging.Configuration;

namespace Prezza.Framework.Logging.Filters
{
	internal class LogFilter
	{
		private Hashtable filters = new Hashtable();

		public LogFilter(LoggingConfiguration config)
		{

			RegisterFilters(config);
		}

		public bool CheckFilters(LogEntry log)
		{
			bool passFilters = true;

			foreach(ILogFilter filter in filters.Values)
			{
				passFilters = filter.Filter(log) && passFilters;
			}

			return passFilters;
		}

		private void RegisterFilters(LoggingConfiguration config)
		{
			filters.Add("categoryFilter", new CategoryFilter(config));
			filters.Add("priorityFilter", new PriorityFilter(config));
		}
	}
}
