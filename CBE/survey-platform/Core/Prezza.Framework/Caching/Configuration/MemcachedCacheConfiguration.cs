//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
    public class MemcachedCacheConfiguration : ConfigurationBase, IXmlConfigurationBase, ICacheConfiguration
	{
        /// <summary>
        /// 
        /// </summary>
        public string CacheTypeName
        {
            get
            {
                return "Prezza.Framework.Caching.MemcachedCache";
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
		public MemcachedCacheConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the cache configuration.</param>
        public MemcachedCacheConfiguration(string name)
            : base(name)
		{
			cacheManagers = new Hashtable();
		}

        /// <summary>
        /// All available pools
        /// </summary>
        public MemcachedPoolConfiguration[] Pools
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public MemcachedServerConfiguration[] Servers
        {
            get;
            private set;
        }

        /// <summary>
		/// Load caching configuration information from Xml.
		/// </summary>
		/// <param name="node"><see cref="XmlNode"/> containing caching configuration.</param>
		public void LoadFromXml(XmlNode node)
		{
			try
			{
                XmlNodeList serversList = node.SelectNodes("/cacheConfiguration/servers/server");
                var servers = new List<MemcachedServerConfiguration>();
                foreach (XmlNode n in serversList)
                {
                    MemcachedServerConfiguration server = new MemcachedServerConfiguration();
                    server.Endpoint = XmlUtility.GetAttributeText(n, "endpoint");
                    if (server.Endpoint == string.Empty)
                        throw new Exception("A cache server was defined in the configuration file, but an endpoint was not specified.");
                    server.Name = XmlUtility.GetAttributeText(n, "name");
                    if (server.Name == string.Empty)
                        throw new Exception("A cache server was defined in the configuration file, but a name was not specified.");
                    servers.Add(server);
                }

                this.Servers = servers.ToArray();

                XmlNodeList poolsList = node.SelectNodes("/cacheConfiguration/pools/pool");
                var pools = new List<MemcachedPoolConfiguration>();
                foreach (XmlNode n in poolsList)
                {
                    MemcachedPoolConfiguration pool = new MemcachedPoolConfiguration();
                    pool.Name = XmlUtility.GetAttributeText(n, "name");
                    if (pool.Name == string.Empty)
                        throw new Exception("A pool was defined in the configuration file, but a name was not specified.");
                    bool b = false;
                    if (bool.TryParse(XmlUtility.GetAttributeText(n, "default"), out b))
                    {
                        pool.IsDefault = b;
                    }
                    int i = 0;
                    if (int.TryParse(XmlUtility.GetAttributeText(n, "initialConnections"), out i))
                    {
                        pool.InitialConnections = i;
                    }
                    i = 0;
                    if (int.TryParse(XmlUtility.GetAttributeText(n, "minSpareConnections"), out i))
                    {
                        pool.MinSpareConnections = i;
                    }
                    i = 0;
                    if (int.TryParse(XmlUtility.GetAttributeText(n, "maxSpareConnections"), out i))
                    {
                        pool.MaxSpareConnections = i;
                    }
                    long l = 0;
                    if (long.TryParse(XmlUtility.GetAttributeText(n, "maxIdleTime"), out l))
                    {
                        pool.MaxIdleTime = l;
                    }
                    l = 0;
                    if (long.TryParse(XmlUtility.GetAttributeText(n, "maxBusyTime"), out l))
                    {
                        pool.MaxBusyTime = l;
                    }
                    l = 0;
                    if (long.TryParse(XmlUtility.GetAttributeText(n, "mainThreadSleep"), out l))
                    {
                        pool.MainThreadSleep = l;
                    }
                    i = 0;
                    if (int.TryParse(XmlUtility.GetAttributeText(n, "socketTimeOut"), out i))
                    {
                        pool.SocketTimeOut = i;
                    }
                    i = 0;
                    if (int.TryParse(XmlUtility.GetAttributeText(n, "socketConnectTO"), out i))
                    {
                        pool.SocketConnectTO = i;
                    }
                    b = false;
                    if (bool.TryParse(XmlUtility.GetAttributeText(n, "failover"), out b))
                    {
                        pool.Failover = b;
                    }
                    b = false;
                    if (bool.TryParse(XmlUtility.GetAttributeText(n, "nagleAlg"), out b))
                    {
                        pool.NagleAlg = b;
                    }
                                        
                    List<string> serverNames = new List<string>();
                    List<int> weights = new List<int>();
                    foreach (XmlNode ns in n.ChildNodes)
                    {
                        string serverName = XmlUtility.GetAttributeText(ns, "name");
                        serverNames.Add(serverName);
                        i = 0;
                        if (int.TryParse(XmlUtility.GetAttributeText(ns, "weight"), out i))
                        {
                            weights.Add(i);
                        }
                    }

                    pool.ServerNames = serverNames.ToArray();
                    pool.ServerWeights = weights.ToArray();
                    pools.Add(pool);
                }
                Pools = pools.ToArray();

				ArgumentValidation.CheckForNullReference(node, "node");

				XmlNodeList cacheManagerList = node.SelectNodes("/cacheConfiguration/cacheManagers/cacheManager");

				foreach(XmlNode cacheManagerNode in cacheManagerList)
				{
					string managerName = XmlUtility.GetAttributeText(cacheManagerNode, "name");
					string configDataType = XmlUtility.GetAttributeText(cacheManagerNode, "configDataType");
					string filePath = XmlUtility.GetAttributeText(cacheManagerNode, "filePath");
					bool isDefault = XmlUtility.GetAttributeBool(cacheManagerNode, "default");
                    string poolName = XmlUtility.GetAttributeText(cacheManagerNode, "pool");

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

                    bool poolDefined = false;

                    if (!string.IsNullOrEmpty(poolName))
                    {
                        var pool = (from p in Pools where p.Name.Equals(poolName) select p).FirstOrDefault();
                        if (pool != null)
                        {
                            poolDefined = true;
                            pool.CacheManagers.Add(managerName);
                        }
                    }

                    if (!poolDefined)
                    {
                        //add a cache into default pool
                        var pool = (from p in Pools where p.IsDefault select p).First();
                        pool.CacheManagers.Add(managerName);
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
