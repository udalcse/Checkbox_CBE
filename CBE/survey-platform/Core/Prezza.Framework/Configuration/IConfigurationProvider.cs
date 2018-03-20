//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Interface for configuration-based providers.
	/// </summary>
	/// <remarks>
	/// A configuration-based provider operated in a manner specified by its 
	/// configuration.  Configuration information is provided by the creator
	/// of a provider object by passing in a <see cref="ConfigurationBase" />
	/// object of a type specified in the application's configuration file.
	/// </remarks>
	public interface IConfigurationProvider
	{
		/// <summary>
		/// Name of the configuration provider.
		/// </summary>
		string ConfigurationName{get; set;}

		/// <summary>
		/// Initialize the provider with the specified configuration object.
		/// </summary>
		/// <remarks>
		/// <see cref="ConfigurationBase" /> is an abstract class, and as such 
		/// can not be instantiated.  The author of a configuration provider will
		/// also create a class that extends <see cref="ConfigurationBase" /> to
		/// contain configuration information specific to the provider.  An object
		/// of this type will be passed to the Initialize method.
		/// </remarks>
		/// <param name="config"></param>
		void Initialize(ConfigurationBase config);
	}
}
