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
	/// Collection of destination information for log messages.
	/// </summary>
	[Serializable]
	public class DestinationDataCollection : DataCollection
	{
		/// <summary>
		/// Get/Set the destination data with the specified index.
		/// </summary>
		public DestinationData this[int index]
		{
			get{return (DestinationData)BaseGet(index);}
			set{BaseSet(index, value);}
		}

		/// <summary>
		/// Get/Set the destination data with the specified name.
		/// </summary>
		public DestinationData this[string name]
		{
			get
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				return BaseGet(name) as DestinationData;
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
		/// Add the specified destination data to the collection.
		/// </summary>
		/// <param name="destinationData">Destination data to add to the collection.</param>
		public void Add(DestinationData destinationData)
		{
			if(destinationData == null)
			{
				throw new ArgumentNullException("destinationData");
			}

			if(destinationData.Name == null)
			{
				throw new InvalidOperationException("Destination data name was null.");
			}

			BaseAdd(destinationData.Name, destinationData);
		}


		/// <summary>
		/// Add the specified object to the collection.
		/// </summary>
		/// <param name="parameter">Object to add to the collection.  An exception will be thrown if the object cannot be cast to a <see cref="DestinationData"/>.</param>
		public void Add(object parameter)
		{
			Add((DestinationData)parameter);
		}
	}
}
