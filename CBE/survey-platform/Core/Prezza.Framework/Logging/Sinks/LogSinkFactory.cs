//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Sinks
{
	internal class LogSinkFactory : ProviderFactory
	{
		public LogSinkFactory(ConfigurationBase config) : base("logSinkFactory", typeof(ILogSink), config)
		{
		}

		public ILogSink CreateSink(string sinkName)
		{
			return (ILogSink)CreateInstance(sinkName);
		}

		protected override ConfigurationBase GetConfigurationObject(string providerName)
		{
			LoggingConfiguration config = (LoggingConfiguration)base.Config;
			SinkData data = config.GetSinkData(providerName);
			return data;
		}

		protected override Type GetConfigurationType(string configurationName)
		{
			SinkData data = (SinkData)GetConfigurationObject(configurationName);
			return GetType(data.TypeName);
		}


	}
}
