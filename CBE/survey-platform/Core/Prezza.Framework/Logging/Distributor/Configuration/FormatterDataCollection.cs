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
	/// Collection of log formatter configuration objects.
	/// </summary>
	[Serializable]
	public class FormatterDataCollection : ProviderDataCollection
	{
		/// <summary>
		/// Get/Set the formatter data object with the specified index.
		/// </summary>
		public FormatterData this[int index]
		{
			get{return (FormatterData)GetProvider(index);}
			set{SetProvider(index, value);}
		}

		/// <summary>
		/// Get/Set the formatter data object with the specified name.
		/// </summary>
		public FormatterData this[string name]
		{
			get{return (FormatterData)GetProvider(name);}
			set{SetProvider(name, value);}
		}

		/// <summary>
		/// Add a formatter data object to the collection.
		/// </summary>
		/// <param name="formatterData">Formatter data object to add to the collection.</param>
		public void Add(FormatterData formatterData)
		{
			AddProvider(formatterData);
		}
	}
}
