//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Collection of log sink configuration objects.
	/// </summary>
	[Serializable]
	public class SinkDataCollection : ProviderDataCollection
	{
		/// <summary>
		/// Get/Set the configuration object with the specified index.
		/// </summary>
		public SinkData this[int index]
		{
			get{return (SinkData)GetProvider(index);}
			set{SetProvider(index, value);}
		}

		/// <summary>
		/// Get/Set the configuration object with the specified name.
		/// </summary>
		public SinkData this[string name]
		{
			get{return (SinkData)GetProvider(name);}
			set{SetProvider(name, value);}
		}

		/// <summary>
		/// Add the specified log sink configuration object to the collection.
		/// </summary>
		/// <param name="providerData">Log sink configuration object to add to the collection.</param>
		public void Add(SinkData providerData)
		{
			AddProvider(providerData);
		}
	}
}
