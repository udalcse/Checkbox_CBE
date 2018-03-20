//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;
using Prezza.Framework.Configuration;
using Prezza.Framework.Caching;

namespace Prezza.Framework.Caching.BackingStoreImplementations
{
	/// <summary>
	/// This class is used when no backing store is needed to support the caching storage policy.
	/// Its job is to provide an implementation of a backing store that does nothing, enabling
	/// the cache to provide a strictly in-memory cache.
	/// </summary>
	public class NullBackingStore : ConfigurationProvider, IBackingStore
	{
        /// <summary>
        /// Support event required by IBackingStore interface to notify subscriber caches that
        /// an item has been removed from the backing store.
        /// </summary>
        public event BackingStoreCacheItemRemovedHandler CacheItemRemoved;

		/// <summary>
		/// Not used
		/// </summary>
		public string CurrentCacheManager
		{
			get { return string.Empty; }
			set { }
		}

		/// <summary>
		/// Always returns 0
		/// </summary>
		public int Count
		{
			get { return 0; }
		}

		/// <summary>
		/// Not used
		/// </summary>
		public NullBackingStore()
		{
		}

		/// <summary>
		/// Not Used
		/// </summary>
		/// <param name="config">Not used</param>
		public override void Initialize(ConfigurationBase config)
		{

		}

		/// <summary>
		/// Not used
		/// </summary>
		/// <param name="newCacheItem">Not used</param>
		public void Add(CacheItem newCacheItem)
		{
		}

		/// <summary>
		/// Not used
		/// </summary>
		/// <param name="key">Not used</param>
		public void Remove(string key)
		{
		}

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public bool Contains(string key)
		{
			return false;
		}

        /// <summary>
        /// Always returns null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public object GetData(string key)
		{
			return null;
		}

        /// <summary>
        /// Always returns null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public CacheItem GetCacheItem(string key)
		{
			return null;
		}

		/// <summary>
		/// Not used
		/// </summary>
		/// <param name="key">Not used</param>
		/// <param name="timestamp">Not used</param>
		public void UpdateLastAccessedTime(string key, DateTime timestamp)
		{
		}

		/// <summary>
		/// Not used
		/// </summary>
		public void Flush()
		{
		}

		/// <summary>
		/// Always returns an empty hash table.
		/// </summary>
		/// <returns>Empty hash table</returns>
		public Hashtable Load()
		{
			return new Hashtable();
		}

		/// <summary>
		/// Empty dispose implementation
		/// </summary>
		public void Dispose()
		{
            
		}
	}
}