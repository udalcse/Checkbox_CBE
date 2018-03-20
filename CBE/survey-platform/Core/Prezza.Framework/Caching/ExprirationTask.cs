//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Runs the expiration task to mark cacheItems as expired and remove those items from the cache.
	/// </summary>
	internal class ExpirationTask
	{
		/// <summary>
		/// Cache operations object used as a proxy to the cache.
		/// </summary>
		private ICacheOperations cacheOperations;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="cacheOperations">Proxy operations object to the cache.</param>
		internal ExpirationTask(ICacheOperations cacheOperations)
		{
			this.cacheOperations = cacheOperations;
		}

		/// <summary>
		/// Run the expiration process.
		/// </summary>
		public void DoExpirations()
		{
			Hashtable liveCacheRepresentation = cacheOperations.GetCurrentCacheState();
			MarkAsExpired(liveCacheRepresentation);
			PrepareForSweep();
			SweepExpiredItemsFromCache(liveCacheRepresentation);
		}

		/// <summary>
		/// Mark the cache items as expired.
		/// </summary>
		/// <param name="liveCacheRepresentation">Current state of the cache.</param>
		/// <returns>Number of items marked for expiration.</returns>
		public virtual int MarkAsExpired(Hashtable liveCacheRepresentation)
		{
			int markedCount = 0;

			foreach(CacheItem cacheItem in liveCacheRepresentation.Values)
			{
				lock(cacheItem)
				{
					if(cacheItem.HasExpired())
					{
						markedCount++;
						cacheItem.WillBeExpired = true;
					}
				}
			}
			return markedCount;
		}

		/// <summary>
		/// Remove the expired items from the cache.
		/// </summary>
		/// <param name="liveCacheRepresentation">Current state of the cache.</param>
		public virtual void SweepExpiredItemsFromCache(Hashtable liveCacheRepresentation)
		{
			foreach(CacheItem cacheItem in liveCacheRepresentation.Values)
			{
				RemoveItemFromCache(cacheItem);
			}
		}

		/// <summary>
		/// Place holder for future functionality.
		/// </summary>
		public virtual void PrepareForSweep()
		{
		}

		/// <summary>
		/// Remove the specified item from the cache.
		/// </summary>
		/// <param name="itemToRemove">Remove the specified item from the cache.</param>
		private void RemoveItemFromCache(CacheItem itemToRemove)
		{
			lock(itemToRemove)
			{
				if(itemToRemove.WillBeExpired)
				{
					try
					{
						cacheOperations.RemoveItemFromCache(itemToRemove.Key, CacheItemRemovedReason.Expired);
					}
					catch(Exception ex)
					{
						//CachingServiceInternalFailureEvent.Fire("Unable to remove expired item from cache.", ex);
						Logger.Write("Unable to remove expired item with key [" + itemToRemove.Key + "] from cache.", "Error", 5);
						ExceptionPolicy.HandleException(ex, "FrameworkCache");
					}
				}
			}
		}
	}
}
