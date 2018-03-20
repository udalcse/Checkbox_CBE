//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Common;
using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Cache scavenging policy based on cache capacity.
	/// </summary>
	internal class CacheCapacityScavengingPolicy
	{
		/// <summary>
		/// Cache configuration information
		/// </summary>
		private readonly ICacheConfiguration configurationData;

		/// <summary>
		/// Name of the cache manager.
		/// </summary>
		private readonly string cacheManagerName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="cacheManagerName">Name of the cache manager.</param>
		/// <param name="configurationData">Cach configuration object.</param>
		public CacheCapacityScavengingPolicy(string cacheManagerName, ICacheConfiguration configurationData)
		{
			ArgumentValidation.CheckForEmptyString(cacheManagerName, "cacheManagerName");
			ArgumentValidation.CheckForNullReference(configurationData, "configurationData");
			
			this.configurationData = configurationData;
			this.cacheManagerName = cacheManagerName;
		}

		/// <summary>
		/// Get the number of items that have to be added to the cache before scavenging occurred.
		/// </summary>
		public int MaximumItemsAllowedBeforeScavenging
		{
			get { return configurationData.GetCacheManagerConfig(cacheManagerName).MaximumElementsInCacheBeforeScavenging; }
		}

		/// <summary>
		/// Determine if cache scavenging is needed.
		/// </summary>
		/// <param name="currentCacheItemCount">Current number of items in the cache.</param>
		/// <returns>True if the scavenging is necessary.</returns>
		public bool IsScavengingNeeded(int currentCacheItemCount)
		{
			if(currentCacheItemCount > MaximumItemsAllowedBeforeScavenging)
				return true;
			else
				return false;
		}
	}
}
