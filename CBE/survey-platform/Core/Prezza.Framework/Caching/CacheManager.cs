//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Prezza.Framework.Caching.Expirations;
using Prezza.Framework.Data;
using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Caching
{
    /// <summary>
    /// Provides the public interface to caching operations.
    /// </summary>
    public class CacheManager : IDisposable
    {
        /// <summary>
        /// Real cache object containing <see cref="CacheItem"/>s.
        /// </summary>
        private ICacheOperations _realCache;

        /// <summary>
        /// Scheduler for background tasks, such as cache scavenging.
        /// </summary>
        private BackgroundScheduler _scheduler;

        /// <summary>
        /// Timer set to trigger cache expiration polling.
        /// </summary>
        private ExpirationPollTimer _pollTimer;

        /// <summary>
        /// Context provider to use for storing cache data.
        /// </summary>
        private readonly IDataContextProvider _contextProvider;

        /// <summary>
        /// Expiration period in seconds 
        /// </summary>
        public int? ExpirationTime { get; set; }

        /// <summary>
        /// Expiration Mode
        /// </summary>
        public string ExpirationMode { get; set; }

        /// <summary>
        ///  Contructor. Called by the <see cref="CacheManagerFactory"/> when it instantiates a CacheManager.
        /// </summary>
        /// <param name="realCache"><see cref="Cache"/> object to store the <see cref="CacheItem"/>s.</param>
        /// <param name="scheduler"><see cref="BackgroundScheduler"/> to schedule tasks such as the scavenging task.</param>
        /// <param name="pollTimer"><see cref="ExpirationPollTimer"/> to trigger checks of cache item expiration.</param>
        /// <param name="contextProvider">Context provider for data access.</param>
        internal CacheManager(ICacheOperations realCache, BackgroundScheduler scheduler, ExpirationPollTimer pollTimer, IDataContextProvider contextProvider, int? expirationTime, string expirationMode)
        {
            _realCache = realCache;
            _scheduler = scheduler;
            _pollTimer = pollTimer;
            _contextProvider = contextProvider;
            ExpirationTime = expirationTime;
            ExpirationMode = expirationMode;
        }

        /// <summary>
        /// Number of items in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                try
                {
                    return ListKeys().Length;
                }
                catch (Exception ex)
                {
                    bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                    if (rethrow)
                    {
                        throw;
                    }

                    //An error occurred, but was not rethrown.  Report an empty cache.
                    Logger.Write("An error occurred without rethrow while getting the item count from a cache, so default value of 0 was returned.", "Warning", 2, -1, Severity.Warning);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Determine if the cache contains an item with the specified key.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the cache contains the item, false otherwise.</returns>
        public bool Contains(string key)
        {
            try
            {
                return _realCache.Contains(GetContextSpecificKey(key));
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                {
                    throw;
                }

                Logger.Write("An error occurred without rethrow while determining if the cache contained a particular item, so default value of false was returned.", "Warning", 2, -1, Severity.Warning);
                return false;
            }
        }

        /// <summary>
        /// Indexer for getting cache items.
        /// </summary>
        public object this[string key]
        {
            get
            {
                try
                {
                    return _realCache.GetData(GetContextSpecificKey(key));
                }
                catch (Exception ex)
                {
                    bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                    if (rethrow)
                    {
                        throw;
                    }

                    //An error occurred, but was not rethrown, return a null item.
                    Logger.Write("An error occurred withouth rethrow while getting an item from a cache, so default value of null was returned.", "Warning", 2, -1, Severity.Warning);
                    return null;
                }
            }
        }

        /// <summary>
        /// Add a value with the specified key to the cache.  If an item with the specified key exists in the cache, it will
        /// be replaced.  If an error occurs during the add, the item will NOT be in the cache.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <param name="value">Item value</param>
        public void Add(string key, object value)
        {
            try
            {
                if (ExpirationTime != null)
                {
                    var timeSpan = new TimeSpan(0, 0, 0, (int)ExpirationTime);
                    var expiration = Activator.CreateInstance(Type.GetType("Prezza.Framework.Caching.Expirations." + ExpirationMode + "Time"), new object[] { timeSpan }) as ICacheItemExpiration;
                    Add(key, value, CacheItemPriority.Normal, null, expiration);
                }
                else
                {
                    Add(key, value, CacheItemPriority.Normal, null);
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }
        /// <summary>
        /// Add a value with the specified key to the cache.  If an item with the specified key exists in the cache, it will
        /// be replaced.  If an error occurs during the add, the item will NOT be in the cache.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <param name="value">Item value</param>
        /// <param name="scavengingPriority">Specifies the new item's scavenging priority. 
        /// See <see cref="CacheItemPriority" /> for more information.</param>
        /// <param name="refreshAction">Object provided to allow the cache to refresh a cache item that has been expired. May be null.</param>
        /// <param name="expirations">Param array specifying the expiration policies to be applied to this item. May be null or omitted.</param>
        public void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
        {
            try
            {
                _realCache.Add(GetContextSpecificKey(key), value, scavengingPriority, refreshAction, expirations);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Remove the item with the specified key from the cache.
        /// </summary>
        /// <param name="key">Item key.</param>
        public void Remove(string key)
        {
            try
            {
                _realCache.Remove(GetContextSpecificKey(key));
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Get the value of the specified item from the cache.
        /// </summary>
        /// <param name="key">Key of item to retrieve value of.</param>
        /// <returns>Value of item associated with the specified key.</returns>
        public object GetData(string key)
        {
            try
            {
                return _realCache.GetData(GetContextSpecificKey(key));
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                {
                    throw;
                }

                //An error occurrred, but was not rethrown, so return null
                Logger.Write("An error occurred withouth rethrow while getting item data from a cache, so default value of null was returned.", "Warning", 2, -1, Severity.Warning);
                return null;
            }
        }

        /// <summary>
        /// Empty the cache.  Should only be called on startup or when no processing is occurring, including
        /// cache expirations (not yet supported) or scavenging (also not yet supported)
        /// </summary>
        public void Flush()
        {
            try
            {
                string[] cacheKeys = ListKeys(false);

                if (cacheKeys.Length > 0)
                {
                    foreach (string key in cacheKeys)
                    {
                        Remove(key);
                    }
                }
                else
                {
                    _realCache.Flush();
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// List keys, optionally with app context information stripped.
        /// </summary>
        /// <returns></returns>
        public string[] ListKeys()
        {
            return ListKeys(true);
        }

        /// <summary>
        /// Get a list of keys in the cache.
        /// </summary>
        /// <param name="stripContext">Specify whether to strip application context information from the key.</param>
        /// <returns>String array representation of cache keys.</returns>
        private string[] ListKeys(bool stripContext)
        {
            try
            {
                string[] keyList = _realCache.ListKeys();
                List<string> appKeyList = new List<string>();

                foreach (string key in keyList)
                {
                    if (KeyIsValidForContext(key))
                    {
                        if (stripContext)
                        {
                            appKeyList.Add(StripContextSpecificKey(key));
                        }
                        else
                        {
                            appKeyList.Add(key);
                        }
                    }
                }

                return appKeyList.ToArray();
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
        /// Get a key specific to the current application context, if any
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetContextSpecificKey(string key)
        {
            if (_contextProvider != null)
            {
                return _contextProvider.ApplicationContext + "%%" + key;
            }

            return key;
        }

        /// <summary>
        /// Strip context information from the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string StripContextSpecificKey(string key)
        {
            if (_contextProvider != null)
            {
                return Regex.Replace(key, _contextProvider.ApplicationContext + "%%", string.Empty, RegexOptions.IgnoreCase);
            }

            return key;
        }

        /// <summary>
        /// Return a boolean indicating the key is valid for the current application context.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool KeyIsValidForContext(string key)
        {
            if (_contextProvider != null)
            {
                return key.ToLower().StartsWith(_contextProvider.ApplicationContext.ToLower());
            }

            return true;
        }

        #region IDisposable Members
        /// <summary>
        /// Implementation of IDisposable interface.  Calls the internal <see cref="Cache"/>'s Dispose method.
        /// </summary>
        public void Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);

                if (_scheduler != null)
                {
                    _scheduler.Stop();
                    _scheduler = null;
                }

                if (_pollTimer != null)
                {
                    _pollTimer.StopPolling();
                    _pollTimer = null;
                }

                if (_realCache != null)
                {
                    (_realCache as IDisposable).Dispose();
                    _realCache = null;
                }

            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }
        #endregion
    }
}
