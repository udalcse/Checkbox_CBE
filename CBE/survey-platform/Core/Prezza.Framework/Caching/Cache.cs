//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Threading;
using System.Collections;

using Prezza.Framework.Logging;
using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Thread-safe caching class.  All cache items are locked before modification in the cache.  In the case where a backing
    /// store is present, the backing store is used as the data store instead of the in-memory cache.
	/// </summary>
	internal class Cache : ICacheOperations, IDisposable
	{
		/// <summary>
		/// In-memory store of cached items.
		/// </summary>
		private readonly Hashtable _inMemoryCache;

		/// <summary>
		/// Cache scavenger.
		/// </summary>
		private ICacheScavenger _cacheScavenger;

		/// <summary>
		/// Implementation of a cache storage
		/// </summary>
		private IBackingStore _backingStore;

		/// <summary>
		/// Policy to define operation of the cache scavenger.
		/// </summary>
		private readonly CacheCapacityScavengingPolicy _scavengingPolicy;

		/// <summary>
		/// Dummy flag used when addition of an item to the cache is in progress.
		/// </summary>
		private const string _addInProgressFlag = "Dummy variable used to flag temp cache item added during Add";

        /// <summary>
        /// Cache name used when syncing multiple caches to the same backing store.
        /// </summary>
        private readonly string _cacheName;

		/// <summary>
		/// Constructor.
		/// </summary>
        public Cache(IBackingStore backingStore, CacheCapacityScavengingPolicy scavengingPolicy, string cacheName, ICacheConfiguration config)
		{
			//The synchronized hashtable is a thread-safe wrapper for the 
			// hash table.
			_inMemoryCache = Hashtable.Synchronized(new Hashtable());

            _cacheName = cacheName;

			_scavengingPolicy = scavengingPolicy;
			
			_backingStore = backingStore;

            //if(_backingStore != null)
            //{
            //    Logger.Write("Creating cache instance with a backing store.", "Cache", 1, -1, Severity.Information);
            //}
            //else
            //{
            //    Logger.Write("Creating cache instance with no backing store.", "Cache", 1, -1, Severity.Information);
            //}
		}

        /// <summary>
        /// Get the key to use for the backing store based on this cache's key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string CacheKeyToBackingStoreKey(string key)
        {
            if (_cacheName != null && _cacheName.Trim() != string.Empty)
            {
                return _cacheName + "_" + key;
            }
            
            return _cacheName;
        }

	    /// <summary>
        /// Based on the backing store key, determine the local cache key
        /// </summary>
        /// <param name="backingStoreKey"></param>
        /// <returns></returns>
        private string BackingStoreKeyToCacheKey(string backingStoreKey)
	    {
	        if (_cacheName != null && _cacheName.Trim() != string.Empty)
            {
                return backingStoreKey.Replace(_cacheName + "_", string.Empty);
            }
	        
            return backingStoreKey;
	    }

	    /// <summary>
		/// Number of items stored in the cache.
		/// </summary>
		public int Count
		{
			get{return _inMemoryCache.Count;}
		}

		/// <summary>
		/// Determine if an item with the specified key exists in the cache.
		/// </summary>
		/// <param name="key">Key of item to check.</param>
		/// <returns>True if the item with the specified key is contained in the cache, false otherwise.</returns>
		public bool Contains(string key)
		{
            //Ensure key is valid
			ValidateKey(key);

            if (_backingStore != null)
            {
                return _backingStore.Contains(CacheKeyToBackingStoreKey(key));
            }

            return _inMemoryCache.Contains(key);
		}

		/// <summary>
		/// Initialize the cache with a scavenger.
		/// </summary>
		/// <param name="cacheScavenger">Cache scavenger.</param>
		public void Initialize(ICacheScavenger cacheScavenger)
		{
			_cacheScavenger = cacheScavenger;
		}

		/// <summary>
		/// Add the object with the specified key to the cache.
		/// </summary>
		/// <param name="key">Key of the object to add.</param>
		/// <param name="value">Object to add to the cache.</param>
		public void Add(string key, object value)
		{
			Add(key, value, CacheItemPriority.Normal, null);
		}

		/// <summary>
		/// Add the object with the specified key to the cache.
		/// </summary>
		/// <param name="key">Key of the object to add.</param>
		/// <param name="value">Object to add.</param>
		/// <param name="scavengingPriority">Scavenging priority associated with the item.</param>
		/// <param name="refreshAction">Refresh action to take upon item expiration.</param>
		/// <param name="expirations">Expirations</param>
		public void Add(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
		{
			ValidateKey(key);
			EnsureCacheInitialized();

			CacheItem cacheItemBeforeLock;
			bool lockWasSuccessful;

            //If backing store present, add item to store
            if (_backingStore != null)
            {
                _backingStore.Add(new CacheItem(
                    CacheKeyToBackingStoreKey(key),
                    value,
                    scavengingPriority,
                    refreshAction,
                    expirations));

                return;
            }

            //Otherwise, use in-memory cache
			do
			{
                //Lock in-memory cache for thread-safety
				lock(_inMemoryCache.SyncRoot)
				{
					if(!_inMemoryCache.Contains(key))
					{
						cacheItemBeforeLock = new CacheItem(key, _addInProgressFlag, CacheItemPriority.NotRemovable, null);
						_inMemoryCache[key] = cacheItemBeforeLock;
					} 
					else
					{
						cacheItemBeforeLock = (CacheItem)_inMemoryCache[key];
					}

					lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);

					if(lockWasSuccessful == false)
					{
						Thread.Sleep(0);
					}
				}
			}  while(lockWasSuccessful == false);

			try
			{
				//Signal that this item has been changed and is being removed (replaced, actually)
				cacheItemBeforeLock.TouchedByUserAction(true);

                try
				{
					cacheItemBeforeLock.Replace(value, refreshAction, scavengingPriority, expirations);
					_inMemoryCache[key] = cacheItemBeforeLock;
				} 
				catch(Exception ex)
				{
                    //Something went wrong, so remove the item from the memory cache.  Later
					// we'd remove it from a backing store as well.
					_inMemoryCache.Remove(key);
					throw new Exception(string.Format("An unexpected error occurred while attempting to add an object to the cache.  Key was {0} and object was {1}.", key, value), ex);
				}
				
				if(_scavengingPolicy.IsScavengingNeeded(_inMemoryCache.Count))
				{
					_cacheScavenger.StartScavenging();
				}
			}
			finally
			{
				//Release the lock on the cache item
				Monitor.Exit(cacheItemBeforeLock);
			}
		}

		/// <summary>
		/// Manually remove the item with the specified key from the cache.  
		/// </summary>
		/// <param name="key">Key of the item to remove from the cache.</param>
		public void Remove(string key)
		{
			Remove(key, CacheItemRemovedReason.Removed);
		}

		/// <summary>
		/// Remove the item with the specified key from the cache.  The removal reason is used to trigger certain
		/// events on item removal.
		/// </summary>
		/// <param name="key">Key of the item to remove from the cache.</param>
		/// <param name="removalReason">Reason for removing item from the cache.</param>
		public void Remove(string key, CacheItemRemovedReason removalReason)
		{
			//Validate key
			ValidateKey(key);

            //If backing store present, use it
            if (_backingStore != null)
            {
                _backingStore.Remove(CacheKeyToBackingStoreKey(key));
                return;
            }

            //Otherwise, use in-memory cache
			CacheItem cacheItemBeforeLock;

			bool lockWasSuccessful;

			do
			{
                //Lock cache for thread safety
				lock(_inMemoryCache.SyncRoot)
				{
					cacheItemBeforeLock = (CacheItem)_inMemoryCache[key];

                    //Contrary to it's name, IsObjectInCache returns true when the cacheItemBeforeLock
                    // passed in is null or the item is in the process of being added to the cache
					if(IsObjectInCache(cacheItemBeforeLock))
					{
						return;
					}

					lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);
				}

				if(lockWasSuccessful == false)
				{
					Thread.Sleep(0);
				}
			} while(lockWasSuccessful == false);

			try
			{
				cacheItemBeforeLock.TouchedByUserAction(true);

				_inMemoryCache.Remove(key);

				RefreshActionInvoker.InvokeRefreshAction(cacheItemBeforeLock, removalReason);
			}
			finally
			{
				Monitor.Exit(cacheItemBeforeLock);

				//Logger.Write("Item removed from cache.", "Cache", 1, -1, Severity.Information);
			}
		}

		/// <summary>
		/// Remove the item with the specified key from the cache.  The removal reason is used to trigger certain
		/// events on item removal.
		/// </summary>
		/// <param name="key">Key of the item to remove from the cache.</param>
		/// <param name="removalReason">Reason for removing item from the cache.</param>
		/// <remarks> This seemingly redundant method is here to be called through the ICacheOperations 
		/// interface. I put this in place to break any dependency from any other class onto 
		/// the Cache class</remarks>
		public void RemoveItemFromCache(string key, CacheItemRemovedReason removalReason)
		{
			Remove(key, removalReason);
		}

		/// <summary>
		/// Retrieve the item with the specified key from the cache.
		/// </summary>
		/// <param name="key">Key of the item to retrieve.</param>
		/// <returns>Object with specified key.</returns>
		public object GetData(string key)
		{
            //Validate cache key
			ValidateKey(key);

            //If backing store in use, return value from backing store
            if (_backingStore != null)
            {
                return _backingStore.GetData(CacheKeyToBackingStoreKey(key));
            }

			CacheItem cacheItemBeforeLock;
			bool lockWasSuccessful;

			do
			{
                //Lock cache to ensure cache isn't modified while checking for item
				lock(_inMemoryCache.SyncRoot)
				{
					cacheItemBeforeLock = (CacheItem)_inMemoryCache[key];

                    //Contrary to it's name, IsObjectInCache returns true when the cacheItemBeforeLock
                    // passed in is null or the item is in the process of being added to the cache.  In both
                    // cases the object is "not in cache"
					if(IsObjectInCache(cacheItemBeforeLock))
					{
                        return null;
                    }

					lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);
				}

				if(lockWasSuccessful == false)
				{
					Thread.Sleep(0);
				}

			} while(lockWasSuccessful == false);

			try
			{
                //If item was found but has expired, remove the item and return null.
				if(cacheItemBeforeLock.HasExpired())
				{
					//Logger.Write("Item has expired, removing from cache.", "Cache", 1, -1, Severity.Information);

					cacheItemBeforeLock.TouchedByUserAction(true);

					_inMemoryCache.Remove(key);

					RefreshActionInvoker.InvokeRefreshAction(cacheItemBeforeLock, CacheItemRemovedReason.Expired);

                    return null;
				}

                //Otherwise, mark item as accessed and return value
				cacheItemBeforeLock.TouchedByUserAction(false);

				return cacheItemBeforeLock.Value;
			}
			finally
			{
				Monitor.Exit(cacheItemBeforeLock);
			}	
		}

		/// <summary>
		/// Flush the cache and remove all items.
		/// </summary>
		/// <remarks>
		/// There may still be thread safety issues in this class with respect to expirations
		/// and scavenging, but I really doubt that either of those will be happening while
		/// a Flush is in progress. It seems that the most likely scenario for a flush
		/// to be called is at the very start of a program, or when absolutely nothing else
		/// is going on. Calling flush in the middle of an application would seem to be
		/// an "interesting" thing to do in normal circumstances.
		/// </remarks>
		public void Flush()
		{
            //Do nothing in backing-store only case.  Flush is currently only used for debug purposes anyway.
            if (_backingStore != null)
            {
                return;
            }

			RestartFlushAlgorithm:

			lock(_inMemoryCache.SyncRoot)
			{
				foreach(string key in _inMemoryCache.Keys)
				{
					bool lockWasSuccessful = false;
					CacheItem itemToRemove = (CacheItem)_inMemoryCache[key];
					try
					{
						if(lockWasSuccessful = Monitor.TryEnter(itemToRemove))
						{
							itemToRemove.TouchedByUserAction(true);
						} 
						else 
						{
							goto RestartFlushAlgorithm;
						}

					}
					finally
					{
						if(lockWasSuccessful)
							Monitor.Exit(itemToRemove);
					}
				}

				int countBeforeFlushing = _inMemoryCache.Count;

                _inMemoryCache.Clear();
			}
		}

		/// <summary>
		/// Get a list of keys currently in the cache.
		/// </summary>
		public string[] ListKeys()
		{
            //If backing store, in memory cache contains no keys
            if (_backingStore != null)
            {
                return new string[] { };
            }

            //Otherwise, list keys in in-memory cache
			ArrayList keys = new ArrayList();

			lock(_inMemoryCache.SyncRoot)
			{
				foreach(string key in _inMemoryCache.Keys)
				{
					keys.Add(key);
				}
			}

			return (string[])keys.ToArray(typeof(string));
		}

		/// <summary>
		/// Get a copy of the current state of the cache.  This copy represents the
		/// state of the cache at the time the copy is created, and will not 
		/// reflect any future changes.
		/// </summary>
		/// <returns><see cref="Hashtable"/> containing a copy of the cache.</returns>
		public Hashtable GetCurrentCacheState()
		{
			return (Hashtable)_inMemoryCache.Clone();
		}

		/// <summary>
		/// Validate that the specified key is neither null nor empty.  Throws an
		/// <see cref="ArgumentNullException"/> if the key is null or an <see cref="ArgumentException"/>
		///  if the key is String.Empty;
		/// </summary>
		/// <param name="key">Key to validate.</param>
		private static void ValidateKey(string key)
		{
			if(key == null)
				throw new ArgumentNullException("key", "An attempt to get or set cache data was made with a null key.  A cache key must be non-null and contain a value.");

			if(key.Length == 0)
				throw new ArgumentException("An attempt to get or set cache data was made with a blank key.  A cache key must be non-null and contain a value.", "key");

		}

        /// <summary>
        /// Ensure cache has been initialized w/a cache scavenger
        /// </summary>
		private void EnsureCacheInitialized()
		{
			if(_cacheScavenger == null)
			{
				throw new InvalidOperationException("Attempt to add an item to the cache before it is initialized.");
			}
		}

		/// <summary>
		/// Internal method used by cache operations that lock the cache item
		/// before modifying it.
		/// </summary>
		/// <param name="cacheItemBeforeLock"><see cref="CacheItem"/> to check.</param>
		/// <returns>True if the item is null or equal to the <see cref="_addInProgressFlag"/></returns>
		private static bool IsObjectInCache(CacheItem cacheItemBeforeLock)
		{
			return cacheItemBeforeLock == null || ReferenceEquals(cacheItemBeforeLock.Value, _addInProgressFlag);
		}

		/// <summary>
		/// Destructor, calls the Dispose method.
		/// </summary>
		~Cache()
		{
			Dispose();
		}

		/// <summary>
		/// Implement the IDisposable interface and calls Dispose on any member objects.
		/// </summary>
		public void Dispose()
		{
            Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
                if (_backingStore != null)
                {
                    _backingStore.Dispose();
                }
	
				_backingStore = null;
			}
		}
	}
}
