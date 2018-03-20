//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Collection of log category information.
	/// </summary>
	[Serializable]
	public class CategoryDataCollection : DataCollection
	{
		/// <summary>
		/// Get/Set the category information with the specified index.
		/// </summary>
		public CategoryData this[int index]
		{
			get{return (CategoryData)BaseGet(index);}
			set{BaseSet(index, value);}
		}

		/// <summary>
		/// Get/Set the category information with the specified name.
		/// </summary>
		public CategoryData this[string name]
		{
			get
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				return (CategoryData)BaseGet(name);
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
		/// Add the specified category data to the collection.
		/// </summary>
		/// <param name="categoryData">Category data to add to the collection.</param>
		public void Add(CategoryData categoryData)
		{
			if(categoryData == null)
			{
				throw new ArgumentNullException("categoryData");
			}

			if(categoryData.Name == null)
			{
				throw new InvalidOperationException("Category data name is null.");
			}
			
			BaseAdd(categoryData.Name, categoryData);
		}

		/// <summary>
		/// Add the specified object to the collection. 
		/// </summary>
		/// <param name="parameter">Object to add to the collection.  An exception will be thrown if it cannot be cast to type <see cref="CategoryData"/>.</param>
		public void Add(object parameter)
		{
			Add((CategoryData)parameter);
		}
	}
}
