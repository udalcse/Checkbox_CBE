using System;
using System.Collections;

namespace Checkbox.Common
{
	/// <summary>
	/// An OrderedDictionary is a Dictionary class that retains the ordering information of key/value pairs added to it.
	/// </summary>
	public class OrderedDictionary : DictionaryBase
	{
		// ArrayList that keeps the value of the hashtable in the order it was added.
		private ArrayList orderedKeys = new ArrayList();

		/// <summary>
		/// Gets or sets the value of a given key
		/// </summary>
		public object this[ string key ]
		{
			get
			{
				return Dictionary[key];
			}
			set
			{
				Dictionary[key] = value;
				if(!orderedKeys.Contains(key))
					orderedKeys.Add(key);
			}
		}

		/// <summary>
		/// Gets or sets the value of a key at the given index
		/// </summary>
		public object this[ int index ]
		{
			get
			{
				return Dictionary[orderedKeys[index]];
			}

			set
			{
				Dictionary[orderedKeys[index]] = value;
			}
		}

		/// <summary>
		/// Gets a collection of the Keys in the dictionary
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return orderedKeys;
			}
		}

		/// <summary>
		/// Gets a collection of the Values in the dictionary
		/// </summary>
		public ICollection Values
		{
			get
			{
				ArrayList values = new ArrayList();
				foreach(object o in orderedKeys)
				{
					values.Add(Dictionary[o]);
				}
				return values;
			}
		}

		/// <summary>
		/// Inserts a key/value pair at the given index
		/// </summary>
		/// <param name="index">The int index</param>
		/// <param name="key">the key</param>
		/// <param name="val">the value</param>
		public void Insert(int index, object key, object val)
		{
			Dictionary.Add(key, val);
			orderedKeys.Insert(index, key);
		}

		/// <summary>
		/// Removes a key/value pair from the dictionary
		/// </summary>
		/// <param name="key">The key</param>
		public void Remove(object key)
		{
			Dictionary.Remove(key);
			orderedKeys.Remove(key);
		}

		/// <summary>
		/// Gets whether the dictionary contains the object with the specified key.
		/// </summary>
		/// <param name="o"></param>
		/// <returns>true if contains, otherwise false</returns>
		public bool Contains(object o)
		{
			return Dictionary.Contains(o);
		}

		/// <summary>
		/// Gets the index of a given key object
		/// </summary>
		/// <param name="o">The key</param>
		/// <returns>The int index</returns>
		public int IndexOf(object o)
		{
			foreach(object key in Dictionary.Keys)
			{
				if(Dictionary[key] == o)
					return orderedKeys.IndexOf(key);				
			}
			return -1;
		}

		/// <summary>
		/// Adds a key/value pair to the end of the dictionary
		/// </summary>
		/// <param name="key">the key</param>
		/// <param name="val">the value</param>
		/// <returns>the int index of the added key/value pair</returns>
		public int Add(object key, object val)
		{
			Dictionary.Add(key, val);
			orderedKeys.Add(key);

			return IndexOf(val);
		}

	}
}
