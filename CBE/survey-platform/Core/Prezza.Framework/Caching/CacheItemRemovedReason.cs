//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Enumeration of reasons a <see cref="CacheItem"/> may be removed from the <see cref="Cache"/>.
	/// </summary>
	public enum CacheItemRemovedReason
	{
		/// <summary>
		/// The item has expired (NOT CURRENTLY USED).
		/// </summary>
		Expired,

		/// <summary>
		/// The item was manually removed from the cache.
		/// </summary>
		Removed,
        
		/// <summary>
		/// The item was removed by the scavenger because it had a lower priority that any other item in the cache (NOT CURRENTLY USED).
		/// </summary>
		Scavenged, 

        /// <summary>
        /// The item has been removed from teh backing store, and thus will be removed from the cache
        /// </summary>
        RemovedFromBackingStore,
        
		/// <summary>
		/// Reserved. Do not use.
		/// </summary>
		Unknown = 9999
	}
}
