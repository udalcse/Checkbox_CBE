//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// <para>Represents the method that will handle the <seealso cref="ConfigurationManager.ConfigurationChanged"/> events.</para>
	/// </summary>
	/// <param name="sender">
	/// <para>The source of the event.</para>
	/// </param>
	/// <param name="e">
	/// <para>A <see cref="ConfigurationChangedEventArgs"/> that contains the event data.</para>
	/// </param>
	public delegate void ConfigurationChangedEventHandler(object sender, ConfigurationChangedEventArgs e);

	/// <summary>
	/// Summary description for ConfigurationChangedEventArgs.
	/// </summary>
	[Serializable]
	public class ConfigurationChangedEventArgs : EventArgs
	{
		private readonly string configurationFile;

        /// <summary>
        /// Constructor that accepts the configuration file name.
        /// </summary>
        /// <param name="configurationFile"></param>
		public ConfigurationChangedEventArgs(string configurationFile)
		{
			this.configurationFile = configurationFile;
		}

		/// <summary>
		/// The path of the file that has changed
		/// </summary>
		public string ConfigurationFile
		{
			get { return this.configurationFile; }
		}
	}
}
