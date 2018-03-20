//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Formatters
{
	internal class LogFormatterFactory : ProviderFactory
	{
		public LogFormatterFactory(ConfigurationBase config) : base("logFormatterFactory", typeof(ILogFormatter), config)
		{
		}

		public ILogFormatter CreateFormatter(string formatterName)
		{
			return (ILogFormatter)CreateInstance(formatterName);
		}

		protected override ConfigurationBase GetConfigurationObject(string formatterName)
		{
			LoggingConfiguration loggingConfig = (LoggingConfiguration)base.Config;
			return loggingConfig.GetFormatterData(formatterName);
		}

		protected override Type GetConfigurationType(string formatterName)
		{
			FormatterData data = (FormatterData)GetConfigurationObject(formatterName);
			return GetType(data.TypeName);
		}
	}
}
