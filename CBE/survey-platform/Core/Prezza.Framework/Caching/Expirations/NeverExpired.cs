//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Caching.Expirations
{
	/// <summary>
	/// This class reflects an expiration policy of never being expired.
	/// </summary>
	[Serializable]
	internal class NeverExpired : ICacheItemExpiration
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public NeverExpired()
		{
		}
		#region ICacheItemExpiration Members

		/// <summary>
		/// Always returns false.
		/// </summary>
		/// <returns></returns>
		public bool HasExpired()
		{
			return false;
		}

		/// <summary>
		/// Do nothing, since there is no expiration.
		/// </summary>
		public void Notify()
		{
		}

		/// <summary>
		/// Nothign to to.
		/// </summary>
		/// <param name="owningCacheItem"></param>
		public void Initialize(CacheItem owningCacheItem)
		{
		}

		#endregion
	}
}
