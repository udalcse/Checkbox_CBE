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
	/// Collection of exception handler configuration information.
	/// </summary>
	public class ExceptionHandlerDataCollection : ProviderDataCollection
	{
		/// <summary>
		/// Get/Set the configuration information with the specified index.
		/// </summary>
		public ExceptionHandlerData this[int index]
		{
			get{return (ExceptionHandlerData)GetProvider(index);}
			set{SetProvider(index, value);}
		}

		/// <summary>
		/// Get/Set the configuration information information with the specified name.
		/// </summary>
		public ExceptionHandlerData this[string name]
		{
			get
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				return BaseGet(name) as ExceptionHandlerData;
			}

			set
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				BaseSet(name, value);
			}
		}

		/// <summary>
		/// Add an exception handling configuration to the collection. 
		/// </summary>
		/// <param name="exceptionHandlerData">Configuration object to add to the collection.</param>
		public void Add(ExceptionHandlerData exceptionHandlerData)
		{
			AddProvider(exceptionHandlerData);
		}
	}
}
