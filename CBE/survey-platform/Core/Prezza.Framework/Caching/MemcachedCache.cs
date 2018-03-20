//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Enyim.Caching;
using Prezza.Framework.Logging;
using Prezza.Framework.Caching.Configuration;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System.Net;
using Prezza.Framework.Caching.Expirations;


namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Memcached based cache
    /// 
    /// Supports only absolute time expiration.
	/// </summary>
	internal class MemcachedCache : ICacheOperations, IDisposable
	{
		/// <summary>
		/// Cache scavenger.
		/// </summary>
		private ICacheScavenger _cacheScavenger;

		/// <summary>
		/// Implementation of a cache storage
		/// </summary>
		private IBackingStore _backingStore;


        /// <summary>
        /// 
        /// </summary>
        private string _serverEndpoint;

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
        /// Config data
        /// </summary>
        private MemcachedCacheConfiguration _config;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MemcachedCache(IBackingStore backingStore, CacheCapacityScavengingPolicy scavengingPolicy, string cacheName, ICacheConfiguration config)
		{
			//The synchronized hashtable is a thread-safe wrapper for the 
			// hash table.
			//_inMemoryCache = Hashtable.Synchronized(new Hashtable());

            _cacheName = cacheName;

            var mconfig = (config as MemcachedCacheConfiguration);
            if (mconfig == null)
            {
                throw new Exception("MemcachedCacheConfiguration instance expected." );
            }
            _config = mconfig;

			_scavengingPolicy = scavengingPolicy;
			
			_backingStore = backingStore;
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

        static object _syncRoot = new object();
        /*private SockIOPool _pool;
        /// <summary>
        /// Memcached connection pool
        /// </summary>
        private SockIOPool Pool
        {
            get
            {
                ensurePoolCreated();
                return _pool;
            }
        }*/

        /// <summary>
        /// Creates a pool if necessary
        /// </summary>
        /*private void ensurePoolCreated()
        {
            if (_pool != null)
                return;
            lock (_syncRoot)
            {
                if (_pool != null && _pool.Initialized)
                {
                    return;
                }

                var pool = getPoolFromConfig();

                if (pool == null)
                    throw new Exception(string.Format("Cannot find default pool or a pool assigned to the cache {0}", _cacheName));

                _pool = SockIOPool.GetInstance(pool.Name);

                _pool.Failover = pool.Failover;
                _pool.InitConnections = pool.InitialConnections;
                _pool.MaintenanceSleep = pool.MainThreadSleep;
                _pool.MaxBusy = pool.MaxBusyTime;
                _pool.MaxConnections = pool.MaxSpareConnections;
                _pool.MaxIdle = pool.MaxIdleTime;
                _pool.MinConnections = pool.MinSpareConnections;
                _pool.Nagle = pool.NagleAlg;
                _pool.SocketConnectTimeout = pool.SocketConnectTO;
                _pool.SocketTimeout = pool.SocketTimeOut;
                var endpoints = (from s in _config.Servers where pool.ServerNames.Contains(s.Name) select s.Endpoint).ToArray();
                _pool.SetServers(endpoints);
                _pool.SetWeights(pool.ServerWeights);
                _pool.Initialize();
            }
        }*/

        /// <summary>
        /// Gets a pool from the configuration
        /// </summary>
        /// <returns></returns>
        private MemcachedPoolConfiguration getPoolFromConfig()
        {
            var pool = (from p in _config.Pools where p.CacheManagers.Contains(_cacheName) select p).FirstOrDefault();

            if (pool == null)
            {
                pool = (from p in _config.Pools where p.IsDefault select p).FirstOrDefault();
            }
            return pool;
        }



        private MemcachedClient _client = null;
        private static object _lockObj = new object();
        /// <summary>
        /// Memchached client class
        /// </summary>
        private MemcachedClient Client
        {
            get
            {
                if (_client != null)
                    return _client;

                lock (_lockObj)
                {
                    if (_client != null)
                        return _client;
                    
                    // try to hit all lines in the config classes
                    MemcachedClientConfiguration mcc = new MemcachedClientConfiguration();


                    var pool = getPoolFromConfig();

                    if (pool == null)
                        throw new Exception(string.Format("Cannot find default pool or a pool assigned to the cache {0}", _cacheName));


                    var endpoints = (from s in _config.Servers where pool.ServerNames.Contains(s.Name) select s.Endpoint).ToArray();
                    foreach (var ep in endpoints)
                    {
                        mcc.AddServer(ep);
                    }

                    mcc.NodeLocator = typeof(DefaultNodeLocator);
                    mcc.KeyTransformer = new SHA1KeyTransformer();
                    mcc.Transcoder = new DefaultTranscoder();

                    mcc.SocketPool.MinPoolSize = pool.MinSpareConnections;
                    mcc.SocketPool.MaxPoolSize = pool.MaxSpareConnections;
                    mcc.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 0, 0, pool.SocketConnectTO);
                    mcc.SocketPool.DeadTimeout = new TimeSpan(0, 0, 0, 0, pool.SocketTimeOut);

                    //ensurePoolCreated();
                    MemcachedClient client = new MemcachedClient(mcc);

                    _client = client;

                    return client;
                }
            }
        }
        
	    /// <summary>
		/// Number of items stored in the cache.
		/// </summary>
		public int Count
		{
			get
            {
                return  0;
            }
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

            return Client.Append(_cacheName + "|" + key, new ArraySegment<byte>(new byte[0]));                
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

            //find expiration date
            DateTime? expiresAt = null;
            if (expirations != null)
            {
                foreach (var exp in expirations)
                {
                    var absTimeExp = exp as AbsoluteTime;
                    if (absTimeExp != null)
                    {
                        if (expiresAt == null)
                        {
                            expiresAt = absTimeExp.AbsoluteExpirationTime;
                        }
                        else
                        {
                            if (expiresAt.Value > absTimeExp.AbsoluteExpirationTime)
                            {
                                expiresAt = absTimeExp.AbsoluteExpirationTime;
                            }
                        }
                    }
                }
            }

            if (expiresAt != null)
            {
                Client.Store(StoreMode.Set, _cacheName + "|" + key, value, expiresAt.Value);
            }
            else
            {
                Client.Store(StoreMode.Set, _cacheName + "|" + key, value);
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

            //System.Diagnostics.Trace.WriteLine(string.Format("[{0}] removed from cache", _cacheName + "|" + key));
            if (!Client.Remove(_cacheName + "|" + key))
                System.Diagnostics.Trace.WriteLine(string.Format("Failed to remove [{0}] from cache", _cacheName + "|" + key)); ;
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
            DateTime dtStart = DateTime.Now;
            //Validate cache key
			ValidateKey(key);

            //If backing store in use, return value from backing store
            if (_backingStore != null)
            {
                return _backingStore.GetData(CacheKeyToBackingStoreKey(key));
            }

            var res = Client.Get(_cacheName + "|" + key);
            var totalMS = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            if (DateTime.Now.Subtract(dtStart).TotalMilliseconds > 20)
            {
                //System.Diagnostics.Trace.WriteLine(string.Format("{1} ms. Request from the cache {3} of key {0}. Results are {2}", key, totalMS, res, _cacheName));
            }
            return res;
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
            Client.FlushAll();
		}

		/// <summary>
		/// Get a list of keys currently in the cache.
		/// </summary>
		public string[] ListKeys()
		{
            return new string[]{};
		}

		/// <summary>
		/// Get a copy of the current state of the cache.  This copy represents the
		/// state of the cache at the time the copy is created, and will not 
		/// reflect any future changes.
		/// </summary>
		/// <returns><see cref="Hashtable"/> containing a copy of the cache.</returns>
		public Hashtable GetCurrentCacheState()
		{
			return new Hashtable();
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
        ~MemcachedCache()
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
                //Pool.Shutdown();

                if (_backingStore != null)
                {
                    _backingStore.Dispose();
                }
	
				_backingStore = null;
			}
		}
	}
}
