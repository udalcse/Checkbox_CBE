//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Collections;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Defines interfaces that cache objects must implement.
	/// </summary>
	internal interface ICacheOperations
	{
		/// <summary>
		/// Get the current state of the cache.  A copy of the cache is returned, so it will not
		/// remain synchronized with the real cache.
		/// </summary>
		/// <returns><see cref="Hashtable"/> containing a copy of the hash.</returns>
		Hashtable GetCurrentCacheState();

		/// <summary>
		/// Remove the item with the specified key from the cache.  The removal reason will be used to fire
		/// events for specific reasons.
		/// </summary>
		/// <param name="keyToRemove">Key of the cache item to remove.</param>
		/// <param name="removalReason"><see cref="CacheItemRemovedReason"/> for the item's removal from the cache.</param>
		void RemoveItemFromCache(string keyToRemove, CacheItemRemovedReason removalReason);

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheScavenger"></param>
        void Initialize(ICacheScavenger cacheScavenger);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(string key, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="scavengingPriority"></param>
        /// <param name="refreshAction"></param>
        /// <param name="expirations"></param>
        void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
		void Remove(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removalReason"></param>
        void Remove(string key, CacheItemRemovedReason removalReason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetData(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string[] ListKeys();
        

        /// <summary>
        /// Clear che cache
        /// </summary>
        void Flush();

    }

}
