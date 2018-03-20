//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Collection of configuration information objects for providers.
	/// </summary>
	[Serializable]
	public abstract class ProviderDataCollection : DataCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ProviderDataCollection()
		{
		}

		/// <summary>
		/// Return the configuration for the provider with the specified name.
		/// </summary>
		/// <param name="name">Name of provider to get.</param>
		/// <returns><see cref="ProviderData" /> configuration for the specified provider.</returns>
		protected internal ProviderData GetProvider(string name)
		{
			ArgumentValidation.CheckForNullReference(name, "name");

			return (ProviderData)BaseGet(name);
		}

		/// <summary>
		/// Return the configuration for the provider with the specified index.
		/// </summary>
		/// <param name="index">Index of the provider configuration object to retrieve.</param>
		/// <returns>ProviderData object for the specified provider.</returns>
		protected internal ProviderData GetProvider(int index)
		{
			return (ProviderData)BaseGet(index);
		}

		/// <summary>
		/// Set the provider configuration for the specified name.
		/// </summary>
		/// <param name="name">Name of provider configuration to set.</param>
		/// <param name="providerData">Configuration for the provider.</param>
		protected internal void SetProvider(string name, ProviderData providerData)
		{
			ArgumentValidation.CheckForNullReference(name, "name");

			BaseSet(name, providerData);
		}

		/// <summary>
		/// Set the provider configuraiton for the specified index.
		/// </summary>
		/// <param name="index">Index of the provider to set the configuration for.</param>
		/// <param name="providerData">Configuration for the provider.</param>
		protected internal void SetProvider(int index, ProviderData providerData)
		{
			BaseSet(index, providerData);
		}

		/// <summary>
		/// Add provider configuration to the collection.
		/// </summary>
		/// <param name="providerData">Provider configuration to add.</param>
		protected internal void AddProvider(ProviderData providerData)
		{
			ArgumentValidation.CheckForNullReference(providerData, "providerData");

			if(providerData.Name == null)
			{
				throw new InvalidOperationException("Provider name is null.");
			}

			BaseAdd(providerData.Name, providerData);
		}

		/// <summary>
		/// Add provider configuration with the specified name to the collection.
		/// </summary>
		/// <param name="name">Name of provider configuration.</param>
		/// <param name="providerData">Provider configuration to add.</param>
		protected internal void AddProvider(string name, ProviderData providerData)
		{
			ArgumentValidation.CheckForNullReference(name, "name");
			ArgumentValidation.CheckForNullReference(providerData, "providerData");

			BaseAdd(name, providerData);
		}

		/// <summary>
		/// Add a collection of provider configuration objects to the collection.
		/// </summary>
		/// <param name="collection">Collection of provider configuration objects to add.</param>
		protected internal void AddProviders(ProviderDataCollection collection)
		{
			foreach(ProviderData data in collection)
			{
				AddProvider(data);
			}
		}
	}
}
