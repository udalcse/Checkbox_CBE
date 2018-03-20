//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Prezza.Framework.Configuration;

#if BACKING_STORE_LOG
using Prezza.Framework.Logging;
#endif

namespace Prezza.Framework.Caching.BackingStoreImplementations
{
    /// <summary>
    /// Base class for backing stores.
    /// </summary>
    public abstract class BaseBackingStore : ConfigurationProvider, IBackingStore
    {
        /// <summary>
        /// Event fired when an item is removed from the cache
        /// </summary>
        public event BackingStoreCacheItemRemovedHandler CacheItemRemoved;

        /// <summary>
        /// <para>Gets the current name of the <see cref="CacheManager"/> using this instance.</para>
        /// </summary>
        /// <value>
        /// <para>The current name of the <see cref="CacheManager"/> using this instance.</para>
        /// </value>
        public string CurrentCacheManager { get; set; }

        /// <summary>
        /// Number of objects stored in the backing store
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Removes an item with the given key from the backing store
        /// </summary>
        /// <param name="key">Key to remove. Must not be null.</param>
        /// <remarks>
        /// <p>
        /// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Remove
        /// </p>
        /// </remarks>
        public void Remove(string key)
        {

#if BACKING_STORE_LOG
            Logger.Write(
                 string.Format("Removing cache item with key [{0}] from backing store.  Key hash code is [{1}].", key, key.GetHashCode()),
                "BackingStore",
                1,
                -1,
                Severity.Information);

#endif

            DoRemove(HashString(key));
        }

        /// <summary>
        /// Removes an item with the given storage key from the backing store.
        /// </summary>
        /// <param name="storageKey">Unique storage key for the cache item to be removed</param>
        /// <remarks>
        /// <p>
        /// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Remove
        /// </p>
        /// </remarks>
        protected abstract void DoRemove(string storageKey);

        /// <summary>
        /// Return a boolean indicating if the cache contains an item with the specified key.
        /// </summary>
        /// <param name="key">Key of cache item to check for existence of.  Must not be null.</param>
        /// <returns>True if the cache contains an item with the specified key, false otherwise.</returns>
        public bool Contains(string key)
        {
#if BACKING_STORE_LOG
            Logger.Write(
                 string.Format("Checking cache for item with key [{0}].  Key hash code is [{1}] and result is [{2}].", key, HashString(key), DoContains(HashString(key))),
                "BackingStore",
                1,
                -1,
                Severity.Information);

#endif
            return DoContains(HashString(key));
        }

        /// <summary>
        /// Return a boolean indicating if the cache contains an item with the specified key.
        /// </summary>
        /// <param name="storageKey">Key of cache item to check for existence of.  Must not be null.</param>
        /// <returns>True if the cache contains an item with the specified key, false otherwise.</returns>
        protected abstract bool DoContains(string storageKey);

        /// <summary>
        /// Updates the last accessed time for a cache item.
        /// </summary>
        /// <param name="key">Key to update</param>
        /// <param name="timestamp">Time at which item updated</param>
        /// <remarks>
        /// <p>
        /// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during UpdateLastAccessedTime
        /// </p>
        /// </remarks>
        public void UpdateLastAccessedTime(string key, DateTime timestamp)
        {
#if BACKING_STORE_LOG
            Logger.Write(
                 string.Format("Updating accessed time for cache item with key [{0}].  Key hash code is [{1}].", key, HashString(key)),
                "BackingStore",
                1,
                -1,
                Severity.Information);

#endif
            DoUpdateLastAccessedTime(HashString(key), timestamp);
        }

        /// <summary>
        /// Updates the last accessed time for a cache item referenced by this unique storage key
        /// </summary>
        /// <param name="storageKey">Unique storage key for cache item</param>
        /// <param name="timestamp">Time at which item updated</param>
        protected abstract void DoUpdateLastAccessedTime(string storageKey, DateTime timestamp);

        /// <summary>
        /// Flushes all CacheItems from backing store. This method must meet the Strong Exception Safety guarantee.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Flush
        /// </p>
        /// </remarks>
        public abstract void Flush();

        /// <summary>
        /// <p>
        /// This method is responsible for adding a CacheItem to the BackingStore. This operation must be successful 
        /// even if an item with the same key already exists. This method must also meet the exception safety guarantee
        /// and make sure that all traces of the new or old item are gone if the add fails in any way.
        /// </p> 
        /// </summary>
        /// <param name="newItem">CacheItem to be added</param>
        /// <remarks>
        /// <p>
        /// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Add
        /// </p>
        /// </remarks>
        public virtual void Add(CacheItem newItem)
        {
            try
            {
#if BACKING_STORE_LOG
                Logger.Write(
                     string.Format("Adding cache item with key [{0}] to backing store.  Key hash code is [{1}].", newItem.Key, HashString(newItem.Key)),
                    "BackingStore",
                    1,
                    -1,
                    Severity.Information);

#endif
                RemoveOldItem(HashString(newItem.Key));
                AddNewItem(HashString(newItem.Key), newItem);
            }
            catch
            {
                DoRemove(HashString(newItem.Key));
                throw;
            }
        }

        /// <summary>
        /// Get the data associated with the cache item with the specified key.
        /// </summary>
        /// <param name="key">Key of cache item.</param>
        /// <returns>Cache item data.</returns>
        public object GetData(string key)
        {
#if BACKING_STORE_LOG
            Logger.Write(
                 string.Format("Getting data for cache item with key [{0}] from backing store.  Key hash code is [{1}].", key, HashString(key)),
                "BackingStore",
                1,
                -1,
                Severity.Information);

#endif
            return DoGetData(HashString(key));
        }

        /// <summary>
        /// Get the data associated with the cache item with the specified key.
        /// </summary>
        /// <param name="storageKey">Key of cache item.</param>
        /// <returns>Cache item data.</returns>
        protected abstract object DoGetData(string storageKey);

        /// <summary>
        /// Get the cache item associated with the specified key.
        /// </summary>
        /// <param name="key">Key of cache item.</param>
        /// <returns>Cache Item associated with the specified key.</returns>
        public CacheItem GetCacheItem(string key)
        {
#if BACKING_STORE_LOG
            Logger.Write(
                string.Format("Getting cache item with key [{0}] from backing store.  Key hash code is [{1}].", key, HashString(key)),
                "BackingStore",
                1,
                -1,
                Severity.Information);
#endif

            return DoGetCacheItem(HashString(key));
        }

        /// <summary>
        /// Get the cache item associated with the specified key.
        /// </summary>
        /// <param name="storageKey">Key of cache item.</param>
        /// <returns>Cache Item associated with the specified key.</returns>
        protected abstract CacheItem DoGetCacheItem(string storageKey);

        /// <summary>
        /// Loads all CacheItems from underlying database.
        /// </summary>
        /// <returns>Hashtable containing all existing CacheItems.</returns>
        /// <remarks>Exceptions thrown depend on the implementation of the underlying database.</remarks>
        public virtual Hashtable Load()
        {
            return LoadDataFromStore();
        }

        /// <summary>
        /// Removed existing item stored in persistence store with same key as new item
        /// </summary>
        /// <param name="storageKey">Unique key for cache item</param>
        protected abstract void RemoveOldItem(string storageKey);

        /// <summary>
        /// Adds new item to persistence store
        /// </summary>
        /// <param name="storageKey">Unique key for cache item</param>
        /// <param name="newItem">Item to be added to cache. May not be null.</param>
        protected abstract void AddNewItem(string storageKey, CacheItem newItem);

        /// <summary>
        /// Responsible for loading items from underlying persistence store. This method should do
        /// no filtering to remove expired items.
        /// </summary>
        /// <returns>Hash table of all items loaded from persistence store</returns>
        protected abstract Hashtable LoadDataFromStore();

        /// <summary>
        /// Touch the cache item.  Allows child classes to access internal cache item methods
        /// </summary>
        /// <param name="item"></param>
        /// <param name="removedFromCache"></param>
        protected void TouchCacheItem(CacheItem item, bool removedFromCache)
        {
            if (removedFromCache && CacheItemRemoved != null && item != null)
            {
                CacheItemRemoved(new CacheItemRemovedEventArgs(item.Key));
            }

            if (item != null)
            {
                item.TouchedByUserAction(removedFromCache);
            }
        }

        /// <summary>
        /// Replace the value of a cache item
        /// </summary>
        /// <param name="cacheItem"></param>
        /// <param name="value"></param>
        /// <param name="refreshAction"></param>
        /// <param name="scavengingPriority"></param>
        /// <param name="expirations"></param>
        protected static void ReplaceCacheItem(CacheItem cacheItem, object value, ICacheItemRefreshAction refreshAction, CacheItemPriority scavengingPriority, params ICacheItemExpiration[] expirations)
        {
            cacheItem.Replace(value, refreshAction, scavengingPriority, expirations);
        }

        /// <summary>
        /// Invoke a refresh action when a cache item has been updated
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason"></param>
        protected static void InvokeRefreshAction(CacheItem item, CacheItemRemovedReason reason)
        {
            RefreshActionInvoker.InvokeRefreshAction(item, reason);
        }

        /// <summary>
        /// Hash a plaintext string using the MD5 hash algorithm
        /// </summary>
        /// <param name="plaintext">A plaintext string to encrypt</param>
        /// <returns>An encrypted string</returns>
        private static string HashString(string plaintext)
        {
            MD5CryptoServiceProvider sha1 = new MD5CryptoServiceProvider();
            Byte[] hashedArray = sha1.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
            string hashedString = Encoding.ASCII.GetString(hashedArray);
            return hashedString;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseBackingStore()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose method for all backing stores. This implementation is sufficient for any class that does not need any finalizer behavior
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing method as used in the Dispose pattern
        /// </summary>
        /// <param name="disposing">True if we are called during Dispose. False if we are called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
