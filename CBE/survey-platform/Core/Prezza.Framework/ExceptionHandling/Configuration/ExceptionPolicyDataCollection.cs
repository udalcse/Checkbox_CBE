//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Collectoin of exception policy configuration objects.
	/// </summary>
	public class ExceptionPolicyDataCollection : ProviderDataCollection
	{
		/// <summary>
		/// Get/Set the collection policy configuration object with the specified index.
		/// </summary>
		public ExceptionPolicyData this[int index]
		{
			get{return (ExceptionPolicyData)GetProvider(index);}
			set{SetProvider(index, value);}
		}
		
		/// <summary>
		/// Get/Set the exception policy configuration object with the specified name.fs
		/// </summary>
		public ExceptionPolicyData this[string name]
		{
			get{return (ExceptionPolicyData)GetProvider(name);}
			set{SetProvider(name, value);}
		}
        
		/// <summary>
		/// Add an exception policy configuration object to the collection.
		/// </summary>
		/// <param name="providerData"></param>
		public void Add(ExceptionPolicyData providerData)
		{
			AddProvider(providerData);
		}
	}
}
