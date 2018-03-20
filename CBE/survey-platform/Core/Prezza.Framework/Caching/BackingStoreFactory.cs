//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using Prezza.Framework.Caching.Configuration;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Represents a factory for creating <see cref="IBackingStore"/> object
    /// from the configuration data in a specified <see cref="ConfigurationBase"/>.
	/// </summary>
	public class BackingStoreFactory : ProviderFactory
	{
		/// <summary>
		/// <para>Initialize a new instance of the <see cref="BackingStoreFactory"/> class.</para>
		/// </summary>
		public BackingStoreFactory() : this(((ICacheConfiguration)ConfigurationManager.GetConfiguration("cacheConfiguration")).BackingStoreData)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="BackingStoreFactory"/> class.
		/// </summary>
		/// <param name="config">Current configuration context</param>
		public BackingStoreFactory(ConfigurationBase config) : base("BackingStoreFactory", typeof(IBackingStore), config)
		{
		}

		/// <summary>
		/// Creates an <see cref="IBackingStore"/> from the configuration
		/// data associated with the specified Cache Manager.
		/// </summary>
		/// <param name="cacheManagerName">The name of the cache manager that is creating the <see cref="IBackingStore"/>.</param>
		/// <returns>An <see cref="IBackingStore"/>.</returns>
		public IBackingStore CreateBackingStore(string cacheManagerName)
		{
			return (IBackingStore)CreateInstance(cacheManagerName);
		}

		/// <summary>
		/// <para>Gets the <see cref="Type"/> of the <see cref="IBackingStore"/> for the factory to create for a <see cref="CacheManager"/>.</para>
		/// </summary>
		///<param name="backingStoreName">Name of the backing store to use.</param>
		///<returns></returns>
		protected override Type GetConfigurationType(string backingStoreName)
		{ 
			CacheStorageData cacheStorageData = GetCacheStorageData(backingStoreName);
			return GetType(cacheStorageData.TypeName);
		}

		/// <summary>
		/// Get the configuration information for the specified <see cref="CacheManager"/>.
		/// </summary>
		/// <param name="providerName">Name of <see cref="CacheManager"/> to get the configuration for.</param>
		/// <returns><see cref="CacheManagerData"/> object containing <see cref="CacheManager"/> configuration.</returns>
		protected override ConfigurationBase GetConfigurationObject(string providerName)
		{
			BackingStoreProviderData config = (BackingStoreProviderData)Config;
			return config.GetCacheStorageData(providerName);
		}

		/// <summary>
		/// <para>Initialize the <see cref="IConfigurationProvider"/> by invoking the <see cref="IConfigurationProvider.Initialize"/> method.</para>
		/// </summary>
		/// <param name="cacheManagerName">
		/// <para>The name of the <see cref="CacheManager"/>.</para>
		/// </param>
		/// <param name="provider">
		/// <para>The <see cref="IConfigurationProvider"/> to initialize.</para>
		/// </param>
		protected override void InitializeConfigurationProvider(string cacheManagerName, IConfigurationProvider provider)
		{
			CacheStorageData cacheStorageData = GetCacheStorageData(cacheManagerName);
			provider.ConfigurationName = cacheStorageData.Name;
			((IBackingStore)provider).CurrentCacheManager = cacheManagerName;
			base.InitializeConfigurationProvider(cacheManagerName, provider);
		}

		private CacheStorageData GetCacheStorageData(string backingStoreName)
		{
			BackingStoreProviderData backingStoreData = (BackingStoreProviderData)Config;
			return backingStoreData.GetCacheStorageData(backingStoreName);
		}
	}
}