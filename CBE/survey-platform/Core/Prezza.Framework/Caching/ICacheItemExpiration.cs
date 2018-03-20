//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Interface for generation of custom cache expiration schemas.
	/// </summary>
	public interface ICacheItemExpiration
	{
		/// <summary>
		/// Specifies if the item has expired or not.
		/// </summary>
		/// <returns>Returns true if the item has expired.</returns>
		bool HasExpired();

		/// <summary>
		/// Inform the expiration that the cache item has been touched by a user.
		/// </summary>
		void Notify();

		/// <summary>
		/// Allow the expiration instance to initialize itself with the cache item. 
		/// </summary>
		/// <param name="owningCacheItem"></param>
		void Initialize(CacheItem owningCacheItem);
	}
}
