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
using Prezza.Framework.ExceptionHandling;

using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching
{
    /// <summary>
    /// Interface for components to create instances of <see cref="CacheManager"/> objects.
    /// </summary>
    public static class CacheFactory
    {
        private static readonly Hashtable _providers;

        /// <summary>
        /// Data context provider
        /// </summary>
        private static IDataContextProvider _contextProvider;

        /// <summary>
        /// 
        /// </summary>
        static CacheFactory()
        {
            lock (typeof(CacheFactory))
            {
                _providers = new Hashtable();
            }
            ConfigurationManager.ConfigurationChanged += ConfigurationManager_ConfigurationChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextProvider"></param>
        public static void Initialize(IDataContextProvider contextProvider)
        {
            //Initialize the cache factory with a context provider
            ArgumentValidation.CheckForNullReference(contextProvider, "ContextProvider");

            _contextProvider = contextProvider;
        }

        /// <summary>
        /// Create and return an instance of the default <see cref="CacheManager"/>.
        /// </summary>
        /// <returns>Instance of the default <see cref="CacheManager"/>.</returns>
        public static CacheManager GetCacheManager()
        {
            try
            {
                lock (_providers.SyncRoot)
                {
                    if (_providers.Contains("[DEFAULT]"))
                    {
                        return (CacheManager)_providers["[DEFAULT]"];
                    }
                }

                //Logger.Write("Getting an instance of the default CacheManager.", "Info", 1, -1, Severity.Information);
                CacheManagerFactory factory = new CacheManagerFactory(
                    _contextProvider,
                    (ICacheConfiguration)ConfigurationManager.GetConfiguration("checkboxCacheConfiguration"));

                CacheManager provider = factory.GetCacheManager();

                lock (_providers.SyncRoot)
                {
                    if (!_providers.Contains("[DEFAULT]"))
                    {
                        _providers.Add("[DEFAULT]", provider);
                    }
                }

                return provider;
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;

                return null;
            }
        }

        /// <summary>
        /// Create and return an instance of the specified <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="cacheManagerName">Name of the <see cref="CacheManager"/> to instantiate.</param>
        /// <returns>Instance of the specified <see cref="CacheManager"/> object.</returns>
        public static CacheManager GetCacheManager(string cacheManagerName)
        {
            try
            {
                lock (_providers.SyncRoot)
                {
                    if (_providers.Contains(cacheManagerName))
                    {
                        return (CacheManager)_providers[cacheManagerName];
                    }
                }

                //Logger.Write("Getting an instance of CacheManager with the name " + cacheManagerName + ".", "Info", 1, -1, Severity.Information);
                CacheManagerFactory factory = new CacheManagerFactory(
                    _contextProvider,
                    (ICacheConfiguration)ConfigurationManager.GetConfiguration("checkboxCacheConfiguration"));
                CacheManager provider = factory.GetCacheManager(cacheManagerName);

                lock (_providers.SyncRoot)
                {
                    if (!_providers.Contains(cacheManagerName))
                    {
                        _providers.Add(cacheManagerName, provider);
                    }
                }

                return provider;
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkInternal");

                if (rethrow)
                    throw;

                return null;
            }
        }

        /// <summary>
        /// Clear providers when cache configuration changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConfigurationManager_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            if (_providers != null)
                _providers.Clear();
        }

        /// <summary>
        /// Get all of the cache managers. This should only be used for debugging
        /// </summary>
        /// <returns></returns>
        public static string[] GetCacheManagers()
        {
            ArrayList list = new ArrayList();

            foreach (string key in _providers.Keys)
            {
                if (key != null)
                {
                    list.Add(key);
                }
            }

            return (string[])list.ToArray(typeof(string));
        }

        /// <summary>
        /// 
        /// </summary>
        public static string CacheType
        {
            get
            {
                return ((new CacheManagerFactory(
                    _contextProvider,
                    (ICacheConfiguration)ConfigurationManager.GetConfiguration("checkboxCacheConfiguration"))).Config as ICacheConfiguration).CacheTypeName;
            }
        }
    
    }
}
