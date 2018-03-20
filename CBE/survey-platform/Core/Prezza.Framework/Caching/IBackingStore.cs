//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Caching
{
    /// <summary>
    /// Delegate for event fired when backing store cache items are removed from the backing store.
    /// </summary>
    /// <param name="e"></param>
    public delegate void BackingStoreCacheItemRemovedHandler(CacheItemRemovedEventArgs e);

	/// <summary>
	/// This interface defines the contract that must be implemented by all backing stores.  Implementors 
	/// of this interface are responsible for interacting with all underlying persistence mechanisms to store 
	/// and retrieve <see cref="CacheItem"/> objects.
	/// </summary>
	/// <remarks>
	/// The Cache class handles all thread-safety for backing stores
	/// </remarks>
	public interface IBackingStore : IConfigurationProvider, IDisposable
	{
		/// <summary>
		/// <para>When implemented by a class, gets the current name of the <see cref="CacheManager"/> using this instance.</para>
		/// </summary>
		/// <value>
		/// <para>The current name of the <see cref="CacheManager"/> using this instance.</para>
		/// </value>
		string CurrentCacheManager { get; set; }

		/// <summary>
		/// Number of objects stored in the backing store
		/// </summary>
		int Count { get; }

		/// <summary>
		/// <p>
		/// This method is responsible for adding a CacheItem to the BackingStore. This operation must be successful 
		/// even if an item with the same key already exists. This method must also meet the Weak Exception Safety guarantee
		/// and remove the item from the backing store if any part of the Add fails.
		/// </p> 
		/// </summary>
		/// <param name="newCacheItem">CacheItem to be added</param>
		/// <remarks>
		/// <p>
		/// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Add
		/// </p>
		/// </remarks>
		void Add(CacheItem newCacheItem);

		/// <summary>
		/// Removes an item with the given key from the backing store
		/// </summary>
		/// <param name="key">Key to remove. Must not be null.</param>
		/// <remarks>
		/// <p>
		/// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Remove
		/// </p>
		/// </remarks>
		void Remove(string key);

		/// <summary>
		/// Return a boolean indicating if the cache contains an item with the specified key.
		/// </summary>
		/// <param name="key">Key of cache item to check for existence of.  Must not be null.</param>
		/// <returns>True if the cache contains an item with the specified key, false otherwise.</returns>
		bool Contains(string key);

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
		void UpdateLastAccessedTime(string key, DateTime timestamp);

		/// <summary>
		/// Flushes all CacheItems from backing store. This method must meet the Weak Exception Safety guarantee.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Flush
		/// </p>
		/// </remarks>
		void Flush();

		/// <summary>
		/// Retrieve the item with the specified key from the cache.
		/// </summary>
		/// <param name="key">Key of the item to retrieve.</param>
		/// <returns>Object with specified key.</returns>
		object GetData(string key);

		/// <summary>
		/// Retrieve the <see cref="CacheItem"/> with the specified key from the cache.
		/// </summary>
		/// <param name="key">Key of the CacheItem to retrieve.</param>
		/// <returns>Object with specified key.</returns>
		CacheItem GetCacheItem(string key);


		/// <summary>
		/// Loads all CacheItems from backing store. 
		/// </summary>
		/// <returns>Hashtable filled with all existing CacheItems.</returns>
		/// <remarks>
		/// <p>
		/// Other exceptions can be thrown, depending on what individual Backing Store implementations throw during Load
		/// </p>
		/// </remarks>
		Hashtable Load();

        /// <summary>
        /// Event fired when an item is removed from the caching store.  This allows listeners to remove
        /// the item from their own caches.
        /// </summary>
        event BackingStoreCacheItemRemovedHandler CacheItemRemoved;
	}
}
