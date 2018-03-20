//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Common;
using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Remove items from the cache according to the scavenging policy.
	/// </summary>
	internal class ScavengerTask
	{
		/// <summary>
		/// Cache scavenging policy based on the number of items in the cache.
		/// </summary>
		private CacheCapacityScavengingPolicy scavengingPolicy;

		/// <summary>
		/// Caching configuration information.
		/// </summary>
		private readonly ICacheConfiguration configurationData;

		/// <summary>
		/// Cache operations proxy.
		/// </summary>
		private ICacheOperations cacheOperations;

		/// <summary>
		/// Name of the cache manager.
		/// </summary>
		private string cacheManagerName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="cacheManagerName">Name of the cache manager.</param>
		/// <param name="configurationData">Caching configuration.</param>
		/// <param name="scavengingPolicy">Cache scavenging policy.</param>
		/// <param name="cacheOperations">Cache operations proxy.</param>
		internal ScavengerTask(string cacheManagerName, ICacheConfiguration configurationData, CacheCapacityScavengingPolicy scavengingPolicy, ICacheOperations cacheOperations)
		{
			ArgumentValidation.CheckForEmptyString(cacheManagerName, "cacheManagerName");
			ArgumentValidation.CheckForNullReference(configurationData, "configurationData");
			ArgumentValidation.CheckForNullReference(scavengingPolicy, "scavengingPolicy");
			ArgumentValidation.CheckForNullReference(cacheOperations, "cacheOperations");

			this.scavengingPolicy = scavengingPolicy;
			this.configurationData = configurationData;
			this.cacheManagerName = cacheManagerName;
			this.cacheOperations = cacheOperations;
		}

		/// <summary>
		/// Number of items to scavenge when the task is run.
		/// </summary>
		public int NumberOfItemsToBeScavenged
		{
			get { return configurationData.GetCacheManagerConfig(cacheManagerName).NumberToRemoveWhenScavenging; }
		}

		/// <summary>
		/// Perform scavenging operations.
		/// </summary>
		public void DoScavenging()
		{
			Hashtable liveCacheRepresentation = cacheOperations.GetCurrentCacheState();

			int currentNumberOfItemsInCache = liveCacheRepresentation.Count;

			if(scavengingPolicy.IsScavengingNeeded(currentNumberOfItemsInCache))
			{
				ResetScavengingFlagInCacheItems(liveCacheRepresentation);
				SortedList scavengableItems = SortItemsForScavenging(liveCacheRepresentation);
				RemoveScavengableItems(scavengableItems);
			}
		}

		/// <summary>
		/// Reset the scavenging eligibilty flags on the cache items.
		/// </summary>
		/// <param name="liveCacheRepresentation">Current cache state.</param>
		private void ResetScavengingFlagInCacheItems(Hashtable liveCacheRepresentation)
		{
			foreach(CacheItem cacheItem in liveCacheRepresentation.Values)
			{
				lock(cacheItem)
				{
					cacheItem.MakeEligibleForScavenging();
				}
			}
		}

		/// <summary>
		/// Sort the cache items by priority and date.
		/// </summary>
		/// <param name="unsortedItemsInCache"><see cref="CacheItem"/>s to sort.</param>
		/// <returns>Sorted list of cache items.</returns>
		private SortedList SortItemsForScavenging(Hashtable unsortedItemsInCache)
		{
			return new SortedList(unsortedItemsInCache, new  PriorityDateComparer(unsortedItemsInCache));
		}

		/// <summary>
		/// Remove scavengable items from the cache.
		/// </summary>
		/// <param name="scavengableItems">Items to remove.</param>
		private void RemoveScavengableItems(SortedList scavengableItems)
		{
			int scavengedItemCount = 0;

			foreach(CacheItem scavengableItem in scavengableItems.Values)
			{
				bool wasRemoved = RemoveItemFromCache(scavengableItem);
				
				if(wasRemoved)
				{
					scavengedItemCount++;

					if(scavengedItemCount >= NumberOfItemsToBeScavenged)
					{
						break;
					}
				}
			}		

			//CachingServiceCacheScavengedEvent.Fire(scavengedItemCount, scavengingPolicy.MaximumItemsAllowedBeforeScavenging, NumberOfItemsToBeScavenged);
			Logger.Write("Cache scavenged.  " + scavengedItemCount.ToString() + " items scavenged.  Max. allowed items = " + scavengingPolicy.MaximumItemsAllowedBeforeScavenging.ToString() + "   Num. items to scavenge = " + NumberOfItemsToBeScavenged.ToString(), "Info", 1);
		}

		/// <summary>
		/// Remove an item (if eligible) from the cache.
		/// </summary>
		/// <param name="itemToRemove">Item to remove from the cache.</param>
		/// <returns>True if the item was removed, false otherwise.</returns>
		private bool RemoveItemFromCache(CacheItem itemToRemove)
		{
			lock(itemToRemove)
			{
				if(itemToRemove.EligibleForScavenging)
				{
					try
					{
						cacheOperations.RemoveItemFromCache(itemToRemove.Key, CacheItemRemovedReason.Scavenged);
						return true;
					}
					catch(Exception ex)
					{
						Logger.Write("Cache Scavenger unable to remove item with key [" + itemToRemove.Key + "] from the cache.", "Error", 5);
						ExceptionPolicy.HandleException(ex, "FrameworkCache");

						//CachingServiceInternalFailureEvent.Fire("Unable to remove item from the cache.", ex);
					}
				}
			}
			
			return false;			
		}
	}
}
