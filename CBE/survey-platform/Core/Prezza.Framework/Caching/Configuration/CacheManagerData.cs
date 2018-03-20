//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Caching.Configuration
{
	/// <summary>
	/// Container of configuration information for <see cref="CacheManager"/> objects.
	/// </summary>
	public class CacheManagerData : ConfigurationBase, IXmlConfigurationBase
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the cache manager.</param>
		public CacheManagerData(string name) : base(name)
		{
			TypeName = typeof(CacheManager).AssemblyQualifiedName;
		}

		/// <summary>
		/// Populate configuration object based on Xml data.
		/// </summary>
		/// <param name="node"><see cref="XmlNode"/> containing cache manager configuration information.</param>
		public void LoadFromXml(XmlNode node)
		{
			ExpirationPollFrequencyInSeconds = XmlUtility.GetNodeInt(node.SelectSingleNode("/cacheManagerConfiguration/expirationPollFrequency")) ?? 60;
			MaximumElementsInCacheBeforeScavenging = XmlUtility.GetNodeInt(node.SelectSingleNode("/cacheManagerConfiguration/maxElementsBeforeScavenging")) ?? 100;
			NumberToRemoveWhenScavenging = XmlUtility.GetNodeInt(node.SelectSingleNode("/cacheManagerConfiguration/scavengeNumberToRemove")) ?? 20;
			BackingStoreName = XmlUtility.GetNodeText(node.SelectSingleNode("/cacheManagerConfiguration/backingStoreName"));
            ExpirationTimeInSeconds = XmlUtility.GetNodeInt(node.SelectSingleNode("/cacheManagerConfiguration/expirationTime")) ?? null;
            ExpirationMode = XmlUtility.GetNodeText(node.SelectSingleNode("/cacheManagerConfiguration/expirationMode"));
		}

	    /// <summary>
	    /// Type name of the <see cref="CacheManager"/>.
	    /// </summary>
	    public string TypeName { get; private set; }

	    /// <summary>
	    /// Name of the backing store
	    /// </summary>
	    public string BackingStoreName { get; private set; }

        /// <summary>
        /// Mode of expiration (Sliding or Absolute)
        /// </summary>
        public string ExpirationMode { get; private set; }

        /// <summary>
	    /// Get/Set the frequency with which to poll the cache for expired items.
	    /// </summary>
	    public int ExpirationPollFrequencyInSeconds { get; set; }

        /// <summary>
        /// Get/Set the expiration time in seconds for cached items.
        /// </summary>
        public int? ExpirationTimeInSeconds { get; set; }

	    /// <summary>
	    /// Get/Set the number of elements that can be added to the cache before the scavenging task runs
	    /// </summary>
	    public int MaximumElementsInCacheBeforeScavenging { get; set; }

	    /// <summary>
	    /// Number of cache elements to remove when the scavenging task runs.
	    /// </summary>
	    public int NumberToRemoveWhenScavenging { get; set; }
	}
}
