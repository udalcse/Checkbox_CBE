//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Provides a way to watch for changes to configuration files
	/// </summary>
	public interface IConfigurationChangeWatcher
	{
		/// <summary>
		/// Event raised when the underlying persistence mechanism for configuration notices that
		/// the persistent representation of configuration information has changed.
		/// </summary>
		event ConfigurationChangedEventHandler ConfigurationChanged;

		/// <summary>
		/// When implemented by a subclass, starts the object watching for configuration changes
		/// </summary>
		void StartWatching();

		/// <summary>
		/// When implemented by a subclass, stops the object from watching for configuration changes
		/// </summary>
		void StopWatching();
	}
}
