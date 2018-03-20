//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Logging;
using Prezza.Framework.Configuration;
using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching
{
    /// <summary>
    /// Factory to create <see cref="CacheManager"/> objects.
    /// </summary>
    internal class CacheManagerFactory : ConfigurationFactory
    {
        /// <summary>
        /// Internal collection of created <see cref="CacheManager"/> objects.
        /// </summary>
        private readonly Hashtable _cacheManagers = Hashtable.Synchronized(new Hashtable());
        private readonly BackingStoreFactory _backingStoreFactory;
        private readonly IDataContextProvider _contextProvider;



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextProvider">Application context provider.</param>
        /// <param name="config">Container of cache configuration settings.</param>
        public CacheManagerFactory(IDataContextProvider contextProvider, ICacheConfiguration config)
            : base("cacheManagerFactory", config)
        {
            //Logger.Write("Initializing the CacheManagerFactory.", "Info", 1, -1, Severity.Information);
            ArgumentValidation.CheckForNullReference(config, "config");

            _contextProvider = contextProvider;

            if (config.BackingStoreData != null)
            {
                _backingStoreFactory = new BackingStoreFactory(config.BackingStoreData);
            }
        }

        /// <summary>
        /// Get an instance of the default <see cref="CacheManager"/>.
        /// </summary>
        /// <returns>Instance of the default <see cref="CacheManager"/>.</returns>
        public CacheManager GetCacheManager()
        {
            return (CacheManager)CreateInstance(GetDefaultInstanceName());
        }

        /// <summary>
        /// Get an instance of the specified <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="cacheManagerName">Name of <see cref="CacheManager"/> to instantiate and return.</param>
        /// <returns>Instance of specified <see cref="CacheManager"/>.</returns>
        public CacheManager GetCacheManager(string cacheManagerName)
        {
            ArgumentValidation.CheckForEmptyString(cacheManagerName, "cacheManagerName");
            return (CacheManager)CreateInstance(cacheManagerName);
        }

        /// <summary>
        /// Create an object of the specified type and name.
        /// </summary>
        /// <param name="cacheManagerName">Name of <see cref="CacheManager"/> object to create.</param>
        /// <param name="type"><see cref="System.Type"/> of object to create.</param> 
        /// <returns>Instance of a <see cref="CacheManager"/> object.</returns>
        protected override object CreateObject(string cacheManagerName, Type type)
        {
            return CreateCacheManager(cacheManagerName);
        }

        /// <summary>
        /// Get the configuration information for the specified <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="providerName">Name of <see cref="CacheManager"/> to get the configuration for.</param>
        /// <returns><see cref="CacheManagerData"/> object containing <see cref="CacheManager"/> configuration.</returns>
        protected CacheManagerData GetConfigurationObject(string providerName)
        {
            ICacheConfiguration config = (ICacheConfiguration)Config;
            return config.GetCacheManagerConfig(providerName);
        }

        /// <summary>
        /// Get the <see cref="System.Type"/> of the specified <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="configurationName">Name of <see cref="CacheManager"/> to get the <see cref="System.Type"/> of.</param>
        /// <returns><see cref="System.Type"/> of the specified <see cref="CacheManager"/>.</returns>
        protected override Type GetConfigurationType(string configurationName)
        {
            CacheManagerData config = GetConfigurationObject(configurationName);
            return GetType(config.TypeName);
        }

        /// <summary>
        /// Create an instance of the specified <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="cacheManagerName">Name of the <see cref="CacheManager"/> to create.</param>
        /// <returns>Instance of the specified <see cref="CacheManager"/>.</returns>
        private CacheManager CreateCacheManager(string cacheManagerName)
        {
            CacheManager cacheManager = _cacheManagers[cacheManagerName] as CacheManager;  //The 'as' operator is like a cast except that it yields null on conversion failure instead of raising an exception

            if (cacheManager != null)
            {
                return cacheManager;
            }

            CacheManagerData config = GetConfigurationObject(cacheManagerName);

            CacheCapacityScavengingPolicy scavengingPolicy = new CacheCapacityScavengingPolicy(cacheManagerName, Config as ICacheConfiguration);

            IBackingStore backingStore; 

            if (config.BackingStoreName != string.Empty && _backingStoreFactory != null)
                backingStore = _backingStoreFactory.CreateBackingStore(config.BackingStoreName);
            else
                backingStore = null;

            Type cacheType = Type.GetType((Config as ICacheConfiguration).CacheTypeName);
            ICacheOperations cache = Activator.CreateInstance(cacheType, new object[] { backingStore, scavengingPolicy, cacheManagerName, Config }) as ICacheOperations;

            ExpirationPollTimer pollTimer = new ExpirationPollTimer();
            ExpirationTask expirationTask = new ExpirationTask(cache);
            ScavengerTask scavengerTask = new ScavengerTask(cacheManagerName, Config as ICacheConfiguration, scavengingPolicy, cache);
            BackgroundScheduler scheduler = new BackgroundScheduler(expirationTask, scavengerTask);

            cache.Initialize(scheduler);

            scheduler.Start();
            pollTimer.StartPolling(scheduler.ExpirationTimeoutExpired, ((ICacheConfiguration)Config).GetCacheManagerConfig(cacheManagerName).ExpirationPollFrequencyInSeconds * 1000);
            int? expirationTime = config.ExpirationTimeInSeconds;
            string expirationMode = config.ExpirationMode;
            cacheManager = new CacheManager(cache, scheduler, pollTimer, _contextProvider, expirationTime, expirationMode);
            _cacheManagers.Add(cacheManagerName, cacheManager);

            //Logger.Write("Returning a new instance of a cache manager [" + cacheManagerName + "].", "Info", 1, -1, Severity.Information);

            return cacheManager;
        }

        /// <summary>
        /// Get the name of the default <see cref="CacheManager"/>.
        /// </summary>
        /// <returns>Name of the default <see cref="CacheManager"/>.</returns>
        private string GetDefaultInstanceName()
        {
            ICacheConfiguration config = (ICacheConfiguration)Config;
            return config.DefaultCacheManager;
        }
    }
}
