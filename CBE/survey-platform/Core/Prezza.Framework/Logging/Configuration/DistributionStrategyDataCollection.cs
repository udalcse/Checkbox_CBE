//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// STrongly Typed collection of <see cref="DistributionStrategyData"/> objects.
	/// </summary>
	[Serializable]
	public class DistributionStrategyDataCollection : ProviderDataCollection
	{
		/// <summary>
		/// Get/Set the <see cref="DistributionStrategyData"/> object with the specified index.
		/// </summary>
		public DistributionStrategyData this[int index]
		{
			get{return (DistributionStrategyData)GetProvider(index);}
			set{SetProvider(index, value);}
		}

		/// <summary>
		/// Get/Set the <see cref="DistributionStrategyData" /> object with the specified name.
		/// </summary>
		public DistributionStrategyData this[string name]
		{
			get{return (DistributionStrategyData)GetProvider(name);}
			set{SetProvider(name, value);}
		}
        
		/// <summary>
		/// Add a new <see cref="DistributionStrategyData"/> object to the collection.
		/// </summary>
		/// <param name="providerData"></param>
		public void Add(DistributionStrategyData providerData)
		{
			AddProvider(providerData);
		}
	}
}
