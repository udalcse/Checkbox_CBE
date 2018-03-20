//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Threading;

using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Encapsulates invokation of cache item refresh handlers.
	/// </summary>
	internal class RefreshActionInvoker
	{
		/// <summary>
		/// Invoke the refresh action for the specified cache item.
		/// </summary>
		/// <param name="removedCacheItem">Item to invoke the refresh action for.</param>
		/// <param name="removalReason">Reason item was removed from the cache.</param>
		public static void InvokeRefreshAction(CacheItem removedCacheItem, CacheItemRemovedReason removalReason)
		{
			if(removedCacheItem.RefreshAction == null)
			{
				return;
			}

			try
			{
				RefreshActionData refreshActionData = new RefreshActionData(removedCacheItem.RefreshAction, removedCacheItem.Key, removedCacheItem.Value, removalReason);

				refreshActionData.InvokeOnThreadPoolThread();

			}
			catch(Exception ex)
			{
				Logger.Write("An error occurred while invoking a refresh action for a cache item.", "Error", 5);
				ExceptionPolicy.HandleException(ex, "FrameworkCache");
			}
		}

		/// <summary>
		/// Invokes refresh action on a thread pool thread.
		/// </summary>
		private class RefreshActionData
		{
			private ICacheItemRefreshAction refreshAction;
			private string keyToRefresh;
			private object removedData;
			private CacheItemRemovedReason removalReason;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="refreshAction">Cache item refresh action.</param>
			/// <param name="keyToRefresh">Key of cache item</param>
			/// <param name="removedData">Value of cache item.</param>
			/// <param name="removalReason">Reason item was removed from cache.</param>
			public RefreshActionData(ICacheItemRefreshAction refreshAction, string keyToRefresh, object removedData, CacheItemRemovedReason removalReason)
			{
				this.refreshAction = refreshAction;
				this.keyToRefresh = keyToRefresh;
				this.removedData = removedData;
				this.removalReason = removalReason;
			}

			/// <summary>
			/// Get the cache item refresh action.
			/// </summary>
			public ICacheItemRefreshAction RefreshAction
			{
				get{ return refreshAction;}
			}

			/// <summary>
			/// Get the key of the cache item.
			/// </summary>
			public string KeyToRefresh
			{
				get{ return keyToRefresh;}
			}

			/// <summary>
			/// Get the value of the cache item.
			/// </summary>
			public object RemovedData
			{
				get{ return removedData;}
			}

			/// <summary>
			/// Get the reason the cache item was removed from the cache.
			/// </summary>
			public object RemovalReason
			{
				get{ return removalReason;}
			}

			/// <summary>
			/// Invoke the refresh action on a thread pool thread.
			/// </summary>
			public void InvokeOnThreadPoolThread()
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolRefreshActionInvoker));
			}

			/// <summary>
			/// Refresh action invoker callback.
			/// </summary>
			private void ThreadPoolRefreshActionInvoker(object notUsed)
			{
				try
				{
					RefreshAction.Refresh(keyToRefresh, removedData, removalReason);
				}
				catch(Exception ex)
				{
					Logger.Write("An error occurred in the refresh action invoker.", "Error", 5);

					ExceptionPolicy.HandleException(ex, "FrameworkCache");
				}
			}
		}
	}
}
