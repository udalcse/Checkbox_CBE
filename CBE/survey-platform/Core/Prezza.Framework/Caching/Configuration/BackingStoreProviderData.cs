//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;	
using System.Xml;

using Prezza.Framework.Configuration;
using Prezza.Framework.Common;

namespace Prezza.Framework.Caching.Configuration
{
	/// <summary>
	/// Base configuration data  class for cache backing store providers.
	/// </summary>
	public class BackingStoreProviderData : ConfigurationBase, IXmlConfigurationBase
	{
		private Hashtable backingStores;

		private string defaultBackingStore;

        /// <summary>
        /// Constructor.  Initializes the hashtable that caches the configurations.
        /// </summary>
		public BackingStoreProviderData() : base("BackingStoreProviderData")
		{
			backingStores = new Hashtable();
		}

        /// <summary>
        /// Get the configuration for the default backing store.
        /// </summary>
		public CacheStorageData DefaultCacheStorageData
		{
			get
			{
				if(defaultBackingStore != null)
					return (CacheStorageData)backingStores[defaultBackingStore]; 
				else
					return null;
			}
		}

        /// <summary>
        /// Get the configuration for the specified backing store.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
		public CacheStorageData GetCacheStorageData(string name)
		{
			ArgumentValidation.CheckForEmptyString(name, "name");
			if(backingStores.Contains(name))
				return (CacheStorageData)backingStores[name];
			else
				return null;
		}

		#region IXmlConfigurationBase Members

        /// <summary>
        /// Load the backing store configurations from XML
        /// </summary>
        /// <param name="node"></param>
		public void LoadFromXml(System.Xml.XmlNode node)
		{
			XmlNodeList bStores = node.SelectNodes("/backingStores/backingStore");
			foreach(XmlNode n in bStores)
			{
				string name = XmlUtility.GetAttributeText(n, "name");
				string filePath = XmlUtility.GetAttributeText(n, "filePath");
				string configDataType = XmlUtility.GetAttributeText(n, "configDataType");
				if(XmlUtility.GetAttributeBool(n, "default"))
				{
					defaultBackingStore = name;
				}

				backingStores.Add(name, ConfigurationManager.GetConfiguration(filePath, configDataType, null));
			}
		}

		#endregion
	}
}
