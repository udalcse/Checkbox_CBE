
using System;
using System.Web;
using System.Collections;
using System.Threading;

using Prezza.Framework.Logging;
using Prezza.Framework.Caching;
using Prezza.Framework.Caching.BackingStoreImplementations;

namespace Checkbox.Web.Caching
{
    /// <summary>
    /// Implementation of IBackingStore that stores its CacheItems into the ASP.NET Session object for the current IPrincipal.
    /// </summary>
    /// <remarks>
    /// This backing store can be used in an ASP.NET web application to cache user specific information that can 
    /// expire along with the Session.
    /// </remarks>
    public class AspNetSessionBackingStore : BaseBackingStore
    {
        /// <summary>
        /// Dummy flag used when addition of an item to the cache is in progress.
        /// </summary>
        private const string addInProgressFlag = "Dummy variable used to flag temp cache item added during Add";

        private static System.Web.SessionState.HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Session;
                }

                return null;
            }
        }

        private static Hashtable InSessionCache
        {
            get
            {
                if (Session != null)
                {
                    if (Session["aspSessionCache"] == null)
                    {
                        Session["aspSessionCache"] = Hashtable.Synchronized(new Hashtable());
                    }
                    return (Hashtable)Session["aspSessionCache"];
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="newItem"></param>
        protected override void AddNewItem(string storageKey, CacheItem newItem)
        {
            //Logger.Write("Attempting to add item with storage key [" + storageKey.ToString() + "] to cache.", "ASPNET Session Cache", 1, -1, Severity.Information);
            if (Session != null)
            {
                CacheItem cacheItemBeforeLock;
                bool lockWasSuccessful;
                //For the first part, lock the InSessionCache and try to get an exclusive lock (TryEnter)
                // on the cache item to add / update.
                do
                {
                    if (InSessionCache == null)
                    {
                        Logger.Write("An attempt to add an item to the ASPNET session cache was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
                        return;
                    }

                    lock (InSessionCache.SyncRoot)
                    {
                        if (InSessionCache.Contains(storageKey) == false)
                        {
                            //							Logger.Write("Item does not exst in cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                            cacheItemBeforeLock = new CacheItem(newItem.Key, addInProgressFlag, CacheItemPriority.NotRemovable, null);
                            InSessionCache[storageKey] = cacheItemBeforeLock;
                        }
                        else
                        {
                            //							Logger.Write("Item exists in cache. ", "ASPNET Session Cache", 1, -1, Severity.Information);

                            cacheItemBeforeLock = (CacheItem)InSessionCache[storageKey];
                        }

                        lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);

                        if (lockWasSuccessful == false)
                        {
                            Thread.Sleep(0);
                        }
                    }
                } while (lockWasSuccessful == false);

                try
                {
                    //Signal that this item has been changed and is being removed (replaced, actually)

                    TouchCacheItem(cacheItemBeforeLock, true);

                    try
                    {
                        ReplaceCacheItem(cacheItemBeforeLock, newItem.Value, newItem.RefreshAction, newItem.ScavengingPriority, newItem.Expirations);

                        InSessionCache[storageKey] = cacheItemBeforeLock;

                        //						Logger.Write("Item added to cache.", "ASPNET Session Cache", 1, -1, Severity.Information);
                    }
                    catch (Exception ex)
                    {
                        //Something went wrong, so remove the item from the session cache.  Later
                        // we'd remove it from a backing store as well.
                        InSessionCache.Remove(storageKey);
                        throw new Exception(string.Format("An unexpected error occurred while attempting to add an object to the cache.  Key was {0} and object was {1}.", storageKey, cacheItemBeforeLock.Value), ex);
                    }
                }
                finally
                {
                    //Release the lock on the cache item
                    Monitor.Exit(cacheItemBeforeLock);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <returns></returns>
        protected override CacheItem DoGetCacheItem(string storageKey)
        {
            CacheItem cacheItemBeforeLock;
            bool lockWasSuccessful;

            //			Logger.Write("Attempting get item with storage key [" + storageKey.ToString() + "] from cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

            do
            {
                if (InSessionCache == null)
                {
                    Logger.Write("An attempt to get an item from the ASPNET session cache was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
                    return null;
                }

                lock (InSessionCache.SyncRoot)
                {
                    cacheItemBeforeLock = (CacheItem)InSessionCache[storageKey];

                    if (IsObjectInCache(cacheItemBeforeLock))
                    {
                        //Logger.Write("Item not found in cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                        return null;
                    }

                    lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);
                }

                if (lockWasSuccessful == false)
                {
                    Thread.Sleep(0);
                }

            } while (lockWasSuccessful == false);

            try
            {
                if (cacheItemBeforeLock.HasExpired())
                {
                    //					Logger.Write("Item has expired, removing from cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                    TouchCacheItem(cacheItemBeforeLock, true);
                    InSessionCache.Remove(storageKey);
                    InvokeRefreshAction(cacheItemBeforeLock, CacheItemRemovedReason.Expired);
                    return null;
                }

                TouchCacheItem(cacheItemBeforeLock, false);

                //				Logger.Write("Item found in cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                return cacheItemBeforeLock;
            }
            finally
            {
                Monitor.Exit(cacheItemBeforeLock);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <returns></returns>
        protected override object DoGetData(string storageKey)
        {
            CacheItem cacheItemBeforeLock;
            bool lockWasSuccessful;

            //			Logger.Write("Attempting to get data for item with storage key [" + storageKey.ToString() + "] to cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

            do
            {
                if (InSessionCache == null)
                {
                    Logger.Write("An attempt to get data from the ASPNET session cache was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
                    return null;
                }

                lock (InSessionCache.SyncRoot)
                {
                    cacheItemBeforeLock = (CacheItem)InSessionCache[storageKey];

                    if (IsObjectInCache(cacheItemBeforeLock))
                    {
                        //						Logger.Write("Item not found in cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                        return null;
                    }

                    lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);
                }

                if (lockWasSuccessful == false)
                {
                    Thread.Sleep(0);
                }

            } while (lockWasSuccessful == false);

            try
            {
                if (cacheItemBeforeLock.HasExpired())
                {
                    //					Logger.Write("Item has expired, removing from cache.", "ASPNET Session Cache", 1, -1, Severity.Information);
                    TouchCacheItem(cacheItemBeforeLock, true);
                    InSessionCache.Remove(storageKey);
                    InvokeRefreshAction(cacheItemBeforeLock, CacheItemRemovedReason.Expired);
                    return null;
                }

                TouchCacheItem(cacheItemBeforeLock, false);

                //				Logger.Write("Item found in cache.", "ASPNET Session Cache", 1, -1, Severity.Information);

                return cacheItemBeforeLock.Value;
            }
            finally
            {
                Monitor.Exit(cacheItemBeforeLock);
            }
        }

        /// <summary>
        /// Internal method used by cache operations that lock the cache item
        /// before modifying it.
        /// </summary>
        /// <param name="cacheItemBeforeLock"><see cref="CacheItem"/> to check.</param>
        /// <returns>True if the item is null or equal to the <see cref="addInProgressFlag"/></returns>
        private static bool IsObjectInCache(CacheItem cacheItemBeforeLock)
        {
            return cacheItemBeforeLock == null || ReferenceEquals(cacheItemBeforeLock.Value, addInProgressFlag);
        }


        /// <summary>
        /// Count of items in the cache
        /// </summary>
        public override int Count
        {
            get
            {
                if (InSessionCache != null)
                {
                    return InSessionCache.Count;
                }

                Logger.Write("An attempt to get the count of items from the ASPNET session cache was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Flush();

            if (Session != null)
            {
                Session.Remove("aspSessionCache");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
        // touch all items before clearing
        RestartFlushAlgorithm:

            if (InSessionCache != null)
            {
                lock (InSessionCache.SyncRoot)
                {
                    foreach (string key in InSessionCache.Keys)
                    {
                        bool lockWasSuccessful = false;
                        CacheItem itemToRemove = (CacheItem)InSessionCache[key];
                        try
                        {
                            if (lockWasSuccessful = Monitor.TryEnter(itemToRemove))
                            {
                                TouchCacheItem(itemToRemove, true);
                            }
                            else
                            {
                                goto RestartFlushAlgorithm;
                            }

                        }
                        finally
                        {
                            if (lockWasSuccessful)
                                Monitor.Exit(itemToRemove);
                        }
                    }

                    int countBeforeFlushing = InSessionCache.Count;

                    InSessionCache.Clear();

                    Logger.Write(string.Format("Cache flush event.  {0} items flushed from cache.", countBeforeFlushing), "ASPNET Session Cache", 1, -1, Severity.Information);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(Prezza.Framework.Configuration.ConfigurationBase config)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Hashtable LoadDataFromStore()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected override void DoRemove(string storageKey)
        {
            //Like the add, we'll attempt to lock things up before removing the item
            CacheItem cacheItemBeforeLock;

            bool lockWasSuccessful;

            do
            {
                if (InSessionCache == null)
                {
                    Logger.Write("An attempt to get an item from the ASPNET session cache was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
                    return;
                }

                lock (InSessionCache.SyncRoot)
                {
                    cacheItemBeforeLock = (CacheItem)InSessionCache[storageKey];

                    if (IsObjectInCache(cacheItemBeforeLock))
                    {
                        return;
                    }

                    lockWasSuccessful = Monitor.TryEnter(cacheItemBeforeLock);
                }

                if (lockWasSuccessful == false)
                {
                    Thread.Sleep(0);
                }
            } while (lockWasSuccessful == false);

            try
            {
                TouchCacheItem(cacheItemBeforeLock, true);

                InSessionCache.Remove(storageKey);

                InvokeRefreshAction(cacheItemBeforeLock, CacheItemRemovedReason.Unknown);
            }
            finally
            {
                Monitor.Exit(cacheItemBeforeLock);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        protected override void RemoveOldItem(string storageKey)
        {
            DoRemove(storageKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="timestamp"></param>
        protected override void DoUpdateLastAccessedTime(string storageKey, DateTime timestamp)
        {
            //((CacheItem)InSessionCache[storageKey]).TouchedByUserAction( = timestamp;
        }

        /// <summary>
        /// Return a boolean indicating if the cache contains an item with the specified key.
        /// </summary>
        /// <param name="key">Key of cache item to check for existence of.  Must not be null.</param>
        /// <returns>True if the cache contains an item with the specified key, false otherwise.</returns>
        protected override bool DoContains(string key)
        {
            if (InSessionCache != null)
            {
                return InSessionCache.Contains(key);
            }

            Logger.Write("An attempt to determine if the ASPNET session contains an item was made, but the cache was null.", "ASPNET Session Cache", 1, -1, Severity.Information);
            return false;
        }
    }
}

