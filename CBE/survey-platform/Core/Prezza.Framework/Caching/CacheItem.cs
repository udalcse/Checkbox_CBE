//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Caching.Expirations;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Items stored by the framework <see cref="Cache"/>.
	/// </summary>
	[Serializable]
	public class CacheItem
	{
		/// <summary>
		/// Item key
		/// </summary>
		private string key;

		/// <summary>
		/// Item value
		/// </summary>
		private object value;

		private ICacheItemRefreshAction refreshAction;
		private ICacheItemExpiration[] expirations;
		private CacheItemPriority scavengingPriority;

		private DateTime lastAccessedTime;
		private bool willBeExpired = false;
		private bool eligibleForScavenging = false;


		/// <summary>
		/// Create a new cache item with the specified key and value.
		/// </summary>
		/// <param name="key">Key of the cache item.</param>
		/// <param name="value">Value to store in the cache item.</param>
		/// <param name="scavengingPriority">Scavenging priority for the cache item.</param>
		/// <param name="refreshAction">Item refresh action.</param>
		/// <param name="expirations">Expiration evaluators.</param>
		public CacheItem(string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
		{
			Initialize(key, value, refreshAction, scavengingPriority, expirations);
			
            TouchedByUserAction(false);
            InitializeExpirations(this);
		}

		/// <summary>
		/// Create a new cache item with the specified key and value.
		/// </summary>
		/// <param name="lastAccessedTime">Last accessed time for the item.</param>
		/// <param name="key">Key of the cache item.</param>
		/// <param name="value">Value to store in the cache item.</param>
		/// <param name="scavengingPriority">Scavenging priority for the cache item.</param>
		/// <param name="refreshAction">Item refresh action.</param>
		/// <param name="expirations">Expiration evaluators.</param>
		public CacheItem(DateTime lastAccessedTime, string key, object value, CacheItemPriority scavengingPriority, ICacheItemRefreshAction refreshAction, params ICacheItemExpiration[] expirations)
		{
			Initialize(key, value, refreshAction, scavengingPriority, expirations);

			TouchedByUserAction(false, lastAccessedTime);
			InitializeExpirations(this);
		}

		/// <summary>
		/// Replace the value of the CacheItem.
		/// </summary>
		/// <param name="scavengingPriority">Cache item scavenging priority.</param>
		/// <param name="refreshAction">Refresh action.</param>
		/// <param name="expirations">Expiration evaluators.</param>
		/// <param name="value">New value of CacheItem</param>
		internal void Replace(object value, ICacheItemRefreshAction refreshAction, CacheItemPriority scavengingPriority, params ICacheItemExpiration[] expirations)
		{
			Initialize(this.key, value, refreshAction, scavengingPriority, expirations);
			
            TouchedByUserAction(false);
            InitializeExpirations(this);
		}

		/// <summary>
		/// Cache scavenging priority.
		/// </summary>
		public CacheItemPriority ScavengingPriority
		{
			get{return this.scavengingPriority;}
		}

		/// <summary>
		/// Expiration evaluators for the item.
		/// </summary>
		public ICacheItemExpiration[] Expirations
		{
			get{return this.expirations;}
		}

		/// <summary>
		/// Time the cache item was last read or modified.
		/// </summary>
		public DateTime LastAccessedTime
		{
			get{return this.lastAccessedTime;}
		}

		/// <summary>
		/// Indicates the cache item is elibible to be expired.  It should only be used internally by framework code.
		/// </summary>
		public bool WillBeExpired
		{
			get{return this.willBeExpired;}
			set{this.willBeExpired = value;}
		}

		/// <summary>
		/// Indicates whether the cache item is eligible for scavenging.  It should only be used internally by framework code.
		/// </summary>
		public bool EligibleForScavenging
		{
			get{ return this.eligibleForScavenging && (ScavengingPriority != CacheItemPriority.NotRemovable);}
		}


		/// <summary>
		/// Value of the CacheItem.
		/// </summary>
		public object Value
		{
			get{return value;}
		}

		/// <summary>
		/// Key of the CacheItem
		/// </summary>
		public string Key
		{
			get{return key;}

            //Only used for backing stores
            internal set { key = value; }
		}

		/// <summary>
		/// Get the cache item refresh action.
		/// </summary>
		public ICacheItemRefreshAction RefreshAction
		{
			get { return refreshAction;}
		}

		/// <summary>
		/// Evaluate whether the cache item has expired or not.
		/// </summary>
		/// <returns></returns>
		public bool HasExpired()
		{
			foreach(ICacheItemExpiration expiration in expirations)
			{
				if(expiration.HasExpired())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Indicate that this cache item has been affected by the action of a <see cref="Cache"/> user.  For internal framework use only.
		/// </summary>
		/// <param name="objectRemovedFromCache">True if the item has been removed from the cache.</param>
		internal void TouchedByUserAction(bool objectRemovedFromCache)
		{
			TouchedByUserAction(objectRemovedFromCache, DateTime.Now);
		}

		/// <summary>
		/// Indicate that this cache item has been affected by the action of a <see cref="Cache"/> user.  For internal framework use only.
		/// </summary>
		/// <param name="objectRemovedFromCache">True if the item has been removed from the cache.</param>
		/// <param name="timestamp">Timestamp when the item was "touched."</param>
		internal void TouchedByUserAction(bool objectRemovedFromCache, DateTime timestamp)
		{
			lastAccessedTime = timestamp;
			eligibleForScavenging = false;

			foreach(ICacheItemExpiration expiration in expirations)
			{
				expiration.Notify();
			}

			if(objectRemovedFromCache)
			{
				willBeExpired = false;
			}
			else
			{
				willBeExpired = HasExpired();
			}
		}

		/// <summary>
		/// Set eligible for scavenging flag to true.
		/// </summary>
		internal void MakeEligibleForScavenging()
		{
			this.eligibleForScavenging = true;
		}

		/// <summary>
		/// Set the eligible for scavenging flag to false.
		/// </summary>
		internal void MakeNotEligibleForScavenging()
		{
			this.eligibleForScavenging = false;
		}

        /// <summary>
        /// Expiration flush
        /// </summary>
        /// <param name="cacheItem"></param>
		private void InitializeExpirations(CacheItem cacheItem)
		{
			foreach(ICacheItemExpiration expiration in cacheItem.Expirations)
			{
				expiration.Initialize(cacheItem);
			}
		}

		/// <summary>
		/// Initialize the CacheItem.
		/// </summary>
		/// <param name="key">Item Key.</param>
		/// <param name="value">Item Value.</param>
		/// <param name="refreshAction">Cache item refresh action.</param>
		/// <param name="scavengingPriority">Cache item scavenging priority.</param>
		/// <param name="expirations">Expiration evaluators.</param>
		private void Initialize(string key, object value, ICacheItemRefreshAction refreshAction, CacheItemPriority scavengingPriority, ICacheItemExpiration[] expirations)
		{
			this.key = key;
			this.value = value;
			this.refreshAction = refreshAction;
			this.scavengingPriority = scavengingPriority;

			if(expirations == null)
			{
				this.expirations = new ICacheItemExpiration[1] { new NeverExpired()};
			}
			else
			{
				this.expirations = expirations;
			}
		}
	}
}
