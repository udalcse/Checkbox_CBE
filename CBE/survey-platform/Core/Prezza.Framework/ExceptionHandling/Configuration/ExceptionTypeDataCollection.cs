//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Collection of exception type configuration objects.
	/// </summary>
	public class ExceptionTypeDataCollection : DataCollection
	{
		/// <summary>
		/// Get/Set the exception type configuration object with the specified index.
		/// </summary>
		public ExceptionTypeData this[int index]
		{
			get{return (ExceptionTypeData)BaseGet(index);}
			set{BaseSet(index, value);}
		}

		/// <summary>
		/// Get/Set the exception type configuration object with the specified name.
		/// </summary>
		public ExceptionTypeData this[string name]
		{
			get
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				return BaseGet(name) as ExceptionTypeData;
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
		/// Add an exception type configuration object to the collection.
		/// </summary>
		/// <param name="exceptionTypeData">Exception type configuration object to add to the collection.</param>
		public void Add(ExceptionTypeData exceptionTypeData)
		{
			if(exceptionTypeData == null)
			{
				throw new ArgumentNullException("exceptionTypeData");
			}

			if(exceptionTypeData.Name == null)
			{
				throw new InvalidOperationException("Exception type data was missing name.");
			}

			BaseAdd(exceptionTypeData.Name, exceptionTypeData);
		}

		/// <summary>
		/// Add an object to the collection.  An exception will be thrown if the object can not be cast to 
		/// an <see cref="ExceptionTypeData" /> object.
		/// </summary>
		/// <param name="parameter"></param>
		public void Add(object parameter)
		{
			Add((ExceptionTypeData)parameter);
		}
	}
}
