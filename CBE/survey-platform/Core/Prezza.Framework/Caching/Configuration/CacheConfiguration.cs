//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;
using System.Collections;

using Prezza.Framework.Common;
using Prezza.Framework.Caching;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Caching.Configuration
{
	/// <summary>
	/// Container for caching configuration information.  Used by the <see cref="CacheManagerFactory"/>
	/// when instantiating <see cref="CacheManager"/> objects.
	/// </summary>
    public class CacheConfiguration : ConfigurationBase, IXmlConfigurationBase, ICacheConfiguration
	{
        /// <summary>
        /// 
        /// </summary>
        public string CacheTypeName
        {
            get
            {
                return "Prezza.Framework.Caching.Cache";
            }
        }

        /// <summary>
		/// Name of the default <see cref="CacheManager"/>.
		/// </summary>
		private string defaultCacheManager;

		/// <summary>
		/// Collection of configuration information for various <see cref="CacheManager"/>.
		/// </summary>
		private Hashtable cacheManagers;

		/// <summary>
		/// The configuration data for all backing stores
		/// </summary>
		private BackingStoreProviderData backingStoreData;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CacheConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the cache configuration.</param>
		public CacheConfiguration(string name) : base(name)
		{
			cacheManagers = new Hashtable();
		}

		/// <summary>
		/// Load caching configuration information from Xml.
		/// </summary>
		/// <param name="node"><see cref="XmlNode"/> containing caching configuration.</param>
		public void LoadFromXml(XmlNode node)
		{
			try
			{
				ArgumentValidation.CheckForNullReference(node, "node");

				XmlNodeList cacheManagerList = node.SelectNodes("/cacheConfiguration/cacheManagers/cacheManager");

				foreach(XmlNode cacheManagerNode in cacheManagerList)
				{
					string managerName = XmlUtility.GetAttributeText(cacheManagerNode, "name");
					string configDataType = XmlUtility.GetAttributeText(cacheManagerNode, "configDataType");
					string filePath = XmlUtility.GetAttributeText(cacheManagerNode, "filePath");
					bool isDefault = XmlUtility.GetAttributeBool(cacheManagerNode, "default");

					if(managerName == string.Empty)
						throw new Exception("A cache manager was defined in the configuration file, but a name was not specified.");

					if(configDataType == string.Empty)
                        throw new Exception("A cache manager was defined in the configuration file with name: " + managerName + " but no configuration object data type was specified.");

					if(filePath == string.Empty)
                        throw new Exception("A cache manager was defined in the configuration file with name: " + managerName + " but no configuration file was specified.");

					object[] extraParams = {managerName};
					CacheManagerData config = (CacheManagerData)ConfigurationManager.GetConfiguration(filePath, configDataType, extraParams);

					if(config == null)
					{
                        throw new Exception("A cache manager was defined in the configuration file with name: " + managerName + " but no configuration data could be loaded.");
					}

					cacheManagers.Add(config.Name, config);

					if(isDefault)
						defaultCacheManager = config.Name;
				}

				XmlNode backingStoreConfiguration = node.SelectSingleNode("/cacheConfiguration/backingStoresConfiguration");
				if(backingStoreConfiguration != null)
				{
					string backingStoreConfigDataType = XmlUtility.GetAttributeText(backingStoreConfiguration, "configDataType");
					string backingStoreConfigFilePath = XmlUtility.GetAttributeText(backingStoreConfiguration, "filePath");

					backingStoreData = (BackingStoreProviderData)ConfigurationManager.GetConfiguration(backingStoreConfigFilePath, 
						backingStoreConfigDataType, null);
				}
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}

		/// <summary>
		/// Name of the default <see cref="CacheManager"/> configuration.
		/// </summary>
		public string DefaultCacheManager
		{
			get{return defaultCacheManager;}
		}

        /// <summary>
        /// Get the data for the backing store used by the cache
        /// </summary>
		public BackingStoreProviderData BackingStoreData
		{
			get { return backingStoreData; }
		}

		/// <summary>
		/// Get the <see cref="CacheManager"/> configuration with the specified name.
		/// </summary>
		/// <param name="cacheManagerName">Name of configuration object.</param>
		/// <returns><see cref="CacheManagerData"/> object containing configuration information for a <see cref="CacheManager"/>.</returns>
		public CacheManagerData GetCacheManagerConfig(string cacheManagerName)
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(cacheManagerName, "cacheManagerName");

				if(!cacheManagers.Contains(cacheManagerName))
					throw new Exception("An attempt was made to retrieve the configuration for a cache manager with name " + cacheManagerName + ", but no configuration was found.");

				return (CacheManagerData)cacheManagers[cacheManagerName];
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
				else
					return null;
			}
		}
	}
}
