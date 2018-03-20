//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Xml;
using System.Configuration;

using Prezza.Framework.Common;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Implementation of <see cref="IConfigurationSectionHandler"/> that supports instantiating and loading
	/// self-loading configuration objects.
	/// </summary>
	public sealed class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		/// <summary>
		/// Instantiate an object of type specified by the configuration file and cause it to be
		/// loaded.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns>Configuration object of type specified in Xml configuration.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			//Get the path of the security configuration file
			string filePath = XmlUtility.GetAttributeText(section, "filePath", true);
			string typeName = XmlUtility.GetAttributeText(section, "configDataType", true);
                				
			//Create the configuration object and load it
			object config =  ConfigurationManager.GetConfiguration(filePath, typeName, null);

			return config;
		}
		#endregion
	}
}
