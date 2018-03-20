//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Base class for configuration-based providers.
	/// </summary>
	public abstract class ConfigurationProvider: IConfigurationProvider
	{
		/// <summary>
		/// Name of the configuration.
		/// </summary>
		private string configurationName;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigurationProvider()
		{
		}

		/// <summary>
		/// Get/Set the name of the configuration.
		/// </summary>
		public string ConfigurationName
		{
			get{return configurationName;}
			set{configurationName = value;}
		}

		/// <summary>
		/// Initialize the configuration-based provider with configuration information.
		/// </summary>
		/// <param name="config">Configuration information for the provider.</param>
		public abstract void Initialize(ConfigurationBase config);
	}
}
