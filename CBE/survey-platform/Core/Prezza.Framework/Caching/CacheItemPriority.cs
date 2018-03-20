//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Priority of items stored in the cache.
	/// </summary>
	public enum CacheItemPriority
	{
		/// <summary>
		/// Lowest priority, generally won't be used in practice.
		/// </summary>
		None = 0,
		
		/// <summary>
		/// Low priority for scavenging.
		/// </summary>
		Low = 1,
		
		/// <summary>
		/// Normal priority for scavenging.
		/// </summary>
		Normal,

		/// <summary>
		/// High priority for scavenging.
		/// </summary>
		High,

		/// <summary>
		/// Will never be removed by scavenging.
		/// </summary>
		NotRemovable
	}
}
