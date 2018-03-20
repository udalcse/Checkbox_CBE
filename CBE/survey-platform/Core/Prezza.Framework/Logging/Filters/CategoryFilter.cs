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

	internal class CategoryFilter : ILogFilter
	{
		private LoggingConfiguration loggingConfiguration;

		public CategoryFilter(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(LoggingConfiguration));

			loggingConfiguration = (LoggingConfiguration)config;
		}
		#region ILogFilter Members

		public bool Filter(LogEntry log)
		{
			bool passFilter = true;
			CategoryFilterData categoryFilterData;
			string category = null;

			if(loggingConfiguration.CategoryFilters.Contains(log.Category))
			{
				categoryFilterData = loggingConfiguration.CategoryFilters[log.Category];
			}
			else
			{
				categoryFilterData = null;
			}
			

			if(categoryFilterData != null)
			{
				category = categoryFilterData.Name;
			}

			CategoryFilterMode mode = loggingConfiguration.CategoryFilterMode;

			//In deny all except mode, if the category is in the hashtable, then deny
			if((category == null) && (mode == CategoryFilterMode.DenyAllExceptAllowed))
			{
				passFilter = false;
			}
			//In allow all except mode, if the category is in the hashtable, then deny
			else if((category != null) && mode == CategoryFilterMode.AllowAllExceptDenied)
			{
				passFilter = false;
			}

			return passFilter;
		}

		#endregion
	}
}
