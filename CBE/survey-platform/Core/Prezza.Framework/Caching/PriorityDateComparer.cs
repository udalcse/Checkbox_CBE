//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Collections;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// <see cref="IComparer"/> implementation for cache items.
	/// </summary>
	internal class PriorityDateComparer : IComparer
	{
		private Hashtable unsortedItems;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="unsortedItems">Items to be sorted.</param>
		public PriorityDateComparer(Hashtable unsortedItems)
		{
			this.unsortedItems = unsortedItems;
		}

		/// <summary>
		/// Compare two cache items.
		/// </summary>
		/// <param name="left">1st item to compare.</param>
		/// <param name="right">2nd item to compare.</param>
		/// <returns>0 if the items are equal, -1 if the left item is less than the right item, 1 if the left item is greater than the right item.</returns>
		public int Compare(object left, object right)
		{
			CacheItem leftCacheItem = (CacheItem)unsortedItems[(string)left];
			CacheItem rightCacheItem = (CacheItem)unsortedItems[(string)right];

			lock(rightCacheItem)
			{
				lock(leftCacheItem)
				{
					if(rightCacheItem == null && leftCacheItem == null)
					{
						return 0;
					}
					if(leftCacheItem == null)
					{
						return -1;
					}
					if(rightCacheItem == null)
					{
						return 1;
					}

					return leftCacheItem.ScavengingPriority == rightCacheItem.ScavengingPriority
						? leftCacheItem.LastAccessedTime.CompareTo(rightCacheItem.LastAccessedTime)
						: leftCacheItem.ScavengingPriority - rightCacheItem.ScavengingPriority;
				}
			}
		}
	}
}
