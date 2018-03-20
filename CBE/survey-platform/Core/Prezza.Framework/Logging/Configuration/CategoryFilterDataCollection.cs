//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// Strongly typed collection of <see cref="CategoryFilterData"/> objects.
	/// </summary>
	[Serializable]
	public class CategoryFilterDataCollection : DataCollection
	{
		/// <summary>
		/// Get/Set the <see cref="CategoryFilterData" /> object with the specified index.
		/// </summary>
		public CategoryFilterData this[int index]
		{
			get{return (CategoryFilterData)BaseGet(index);}
			set{BaseSet(index, value);}
		}

		/// <summary>
		/// Get/Set the <see cref="CategoryFilterData"/> object with the specified name.
		/// </summary>
		public CategoryFilterData this[string name]
		{
			get
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				return (CategoryFilterData)BaseGet(name);
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
		/// Add a new <see cref="CategoryFilterData"/> object to the collection.
		/// </summary>
		/// <param name="exceptionTypeData"></param>
		public void Add(CategoryFilterData exceptionTypeData)
		{
			if(exceptionTypeData == null)
			{
				throw new ArgumentNullException("exceptionTypeData");
			}

			if(exceptionTypeData.Name == null)
			{
				throw new ArgumentNullException("exceptionTypeData.Name");
			}

			BaseAdd(exceptionTypeData.Name, exceptionTypeData);
		}

		/// <summary>
		/// Add a new object to the collection.  An error will result if the object can not
		/// be cast to type <see cref="CategoryFilterData"/>
		/// </summary>
		/// <param name="parameter"></param>
		public void Add(object parameter)
		{
			Add((CategoryFilterData)parameter);
		}
	}
}
