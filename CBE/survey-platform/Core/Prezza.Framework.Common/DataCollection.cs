//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;


namespace Prezza.Framework.Common
{
    /// <summary>
    /// Base class for strongly typed, serializable collection.
    /// </summary>
    [XmlType(IncludeInSchema = false)]
    [Serializable]
    public abstract class DataCollection : ICollection, ISerializable, IDeserializationCallback
    {
        private const int _defaultCapacity = 8;
        private bool _readOnly;
        private ArrayList _entries;
        private IEqualityComparer _equalityComparer;
        private Hashtable _entriesTable;
        private DataObjectEntry _nullKeyEntry;
        private KeysCollection _keys;
        private SerializationInfo _serializationInfo;
        private int _version;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DataCollection()
            : this(_defaultCapacity)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capacity">Number of _entries the collection can contain.</param>
        protected DataCollection(int capacity)
            : this(capacity, StringComparer.InvariantCulture)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="equalityComparer"></param>
        protected DataCollection(int capacity, IEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;

            Reset(capacity);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DataCollection(SerializationInfo info, StreamingContext context)
        {
            _serializationInfo = info;
        }

        /// <summary>
        /// Collection of _keys for items in the collection.
        /// </summary>
        public KeysCollection Keys
        {
            get
            {
                if (_keys == null)
                {
                    _keys = new KeysCollection(this);
                }

                return _keys;
            }
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Indicates if the collection is synchronized
        /// </summary>
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Get the data for a specific object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentValidation.CheckForNullReference(info, "info");

            info.AddValue("ReadOnly", _readOnly);
            info.AddValue("EqualityComparer", _equalityComparer, typeof(IEqualityComparer));

            int count = _entries.Count;
            info.AddValue("Count", count);

            String[] keys = new String[count];
            Object[] values = new Object[count];

            for (int i = 0; i < count; i++)
            {
                DataObjectEntry entry = (DataObjectEntry)_entries[i];
                keys[i] = entry.Key;
                values[i] = entry.Value;
            }

            info.AddValue("Keys", keys, typeof(String[]));
            info.AddValue("Values", values, typeof(Object[]));
        }

        /// <summary>
        /// Deserialize the collection.
        /// </summary>
        /// <param name="sender"></param>
        public virtual void OnDeserialization(Object sender)
        {
            if (_equalityComparer != null)
            {
                return;
            }

            if (_serializationInfo == null)
            {
                throw new SerializationException();
            }

            SerializationInfo info = _serializationInfo;
            _serializationInfo = null;

            bool isReadOnly = info.GetBoolean("ReadOnly");
            _equalityComparer = (IEqualityComparer)info.GetValue("EqualityComparer", typeof(IEqualityComparer));
            int count = info.GetInt32("Count");

            String[] theKeys = (String[])info.GetValue("Keys", typeof(String[]));
            Object[] values = (Object[])info.GetValue("Values", typeof(Object[]));

            if (_equalityComparer == null || theKeys == null || values == null)
            {
                throw new SerializationException();
            }

            Reset(count);

            for (int i = 0; i < count; i++)
            {
                BaseAdd(theKeys[i], values[i]);
            }

            _readOnly = isReadOnly;
            _version++;
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="name">Name of the item to remove.</param>
        public void Remove(string name)
        {
            ArgumentValidation.CheckForNullReference(name, "name");
            BaseRemove(name);
        }

        /// <summary>
        /// Remove the item at the specified index from the collection.
        /// </summary>
        /// <param name="index">Index of item to remove.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Get indication of whether item with specified name exists in the collection.
        /// </summary>
        /// <param name="name">Nam of item.</param>
        /// <returns>True if item is contained in the collection, otherwise false is returned.</returns>
        public bool Contains(string name)
        {
            return _entriesTable.Contains(name);
        }

        /// <summary>
        /// Clear the collection and remove all items.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Get an enumerator for iteration through items in the collection.
        /// </summary>
        /// <returns>Enumerator for iteration through items in the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return new DataCollectionValuesEnumerator(this);
        }

        /// <summary>
        /// Coppy the contents of the collection to an array.
        /// </summary>
        /// <param name="array">Array to copy contents to.</param>
        /// <param name="index">Index of item in array.</param>
        public void CopyTo(Array array, int index)
        {
            for (IEnumerator e = GetEnumerator(); e.MoveNext(); )
            {
                array.SetValue(e.Current, index++);
            }
        }

        /// <summary>
        /// Indicates if the collection is read-only.
        /// </summary>
        protected bool IsReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        /// <summary>
        /// Determine if there are any _keys.
        /// </summary>
        /// <returns>True if the _entriesTable contains any _entries.</returns>
        protected bool BaseHasKeys()
        {
            return (_entriesTable.Count > 0);
        }

        /// <summary>
        /// Add an object to the collection.
        /// </summary>
        /// <param name="name">Name of object to add.</param>
        /// <param name="value">Object to add.</param>
        protected void BaseAdd(string name, Object value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            DataObjectEntry entry = new DataObjectEntry(name, value);

            if (name != null)
            {
                if (_entriesTable[name] == null)
                {
                    _entriesTable.Add(name, entry);
                }
                else
                {
                    throw new InvalidOperationException("An entry with this key [" + name + "] already exists in the collection.");
                }
            }
            else
            {
                if (_nullKeyEntry == null)
                {
                    _nullKeyEntry = entry;
                }
            }

            _entries.Add(entry);

            _version++;
        }

        /// <summary>
        /// Remove the specified item from the collection.
        /// </summary>
        /// <param name="name">Name of item to remove from the collection.</param>
        protected void BaseRemove(string name)
        {
            CheckIfReadOnly();

            if (name != null)
            {
                _entriesTable.Remove(name);

                for (int i = _entries.Count - 1; i >= 0; i--)
                {
                    if (_equalityComparer.Equals(name, BaseGetKey(i)))
                    {
                        _entries.RemoveAt(i);
                    }
                }
            }
            else
            {
                _nullKeyEntry = null;

                for (int i = _entries.Count - 1; i >= 0; i--)
                {
                    if (BaseGetKey(i) == null)
                    {
                        _entries.RemoveAt(i);
                    }
                }
            }

            _version++;
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="index">Index of the item to remove from the collection.</param>
        protected void BaseRemoveAt(int index)
        {
            CheckIfReadOnly();

            String key = BaseGetKey(index);

            if (key != null)
            {
                _entriesTable.Remove(key);
            }
            else
            {
                _nullKeyEntry = null;
            }

            _entries.RemoveAt(index);

            _version++;
        }

        /// <summary>
        /// Clear the collection and remove all items.
        /// </summary>
        protected void BaseClear()
        {
            CheckIfReadOnly();

            Reset();
        }

        /// <summary>
        /// Get an item in the collection.
        /// </summary>
        /// <param name="index">Index of item to get.</param>
        /// <returns>Requested item.</returns>
        protected object BaseGet(int index)
        {
            DataObjectEntry entry = (DataObjectEntry)_entries[index];
            return entry.Value;
        }

        /// <summary>
        /// Get an item in the collection.
        /// </summary>
        /// <param name="name">Name of item to get.</param>
        /// <returns>Requested item.</returns>
        protected object BaseGet(string name)
        {
            DataObjectEntry entry = FindEntry(name);

            if (entry == null)
            {
                return null;
            }

            return entry.Value;
        }

        /// <summary>
        /// Set the value of an item in the collection.
        /// </summary>
        /// <param name="index">Index of item to set.</param>
        /// <param name="value">New value for item.</param>
        protected void BaseSet(int index, object value)
        {
            CheckIfReadOnly();

            DataObjectEntry entry = (DataObjectEntry)_entries[index];
            entry.Value = value;
            _version++;
        }

        /// <summary>
        /// Set the value of an item in the collection.
        /// </summary>
        /// <param name="name">Name of item to set value of.</param>
        /// <param name="value">New value of item.</param>
        protected void BaseSet(string name, object value)
        {
            CheckIfReadOnly();
            DataObjectEntry entry = FindEntry(name);

            if (entry != null)
            {
                entry.Value = value;
                _version++;
            }
            else
            {
                BaseAdd(name, value);
            }
        }

        /// <summary>
        /// Get the key of the item with the specified index.
        /// </summary>
        /// <param name="index">Index of item.</param>
        /// <returns>Key of the specified item.</returns>
        protected string BaseGetKey(int index)
        {
            DataObjectEntry entry = (DataObjectEntry)_entries[index];
            return entry.Key;
        }

        /// <summary>
        /// Get a string array containing the _keys of all the items in the collection.
        /// </summary>
        /// <returns>String array with all _keys.</returns>
        protected string[] BaseGetAllKeys()
        {
            int n = _entries.Count;
            String[] allKeys = new String[n];

            for (int i = 0; i < n; i++)
            {
                allKeys[i] = BaseGetKey(i);
            }

            return allKeys;
        }

        /// <summary>
        /// Get an object array containing all object values stored in the collection.
        /// </summary>
        /// <returns>Array of objects.</returns>
        protected Object[] BaseGetAllValues()
        {
            int n = _entries.Count;
            Object[] allValues = new Object[n];

            for (int i = 0; i < n; i++)
            {
                allValues[i] = BaseGet(i);
            }

            return allValues;
        }

        /// <summary>
        /// Get all objects in the collection cast to a specific type.
        /// </summary>
        /// <param name="type">Type of objects</param>
        /// <returns>Array of objects.</returns>
        protected Object[] BaseGetAllValues(Type type)
        {
            int n = _entries.Count;
            object[] allValues = (object[])Array.CreateInstance(type, n);

            for (int i = 0; i < n; i++)
            {
                allValues[i] = BaseGet(i);
            }

            return allValues;
        }


        /// <summary>
        /// Clear the collection.
        /// </summary>
        private void Reset()
        {
            _entries = new ArrayList();
            _entriesTable = new Hashtable(_equalityComparer);
            _nullKeyEntry = null;
            _version++;
        }

        /// <summary>
        /// Clear the collection and set the capacity.
        /// </summary>
        /// <param name="capacity">Capacity of the collection.</param>
        private void Reset(int capacity)
        {
            _entries = new ArrayList(capacity);
            _entriesTable = new Hashtable(capacity, _equalityComparer);
            _nullKeyEntry = null;
            _version++;
        }

        /// <summary>
        /// Find the item entry with the specified key.
        /// </summary>
        /// <param name="key">Key of item to find.</param>
        /// <returns><see cref="DataObjectEntry"/> with the specified key.</returns>
        private DataObjectEntry FindEntry(String key)
        {
            if (key != null)
            {
                return (DataObjectEntry)_entriesTable[key];
            }

            return _nullKeyEntry;
        }

        /// <summary>
        /// Throw an exception if the collection is read-only.
        /// </summary>
        private void CheckIfReadOnly()
        {
            if (_readOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
        }

        /// <summary>
        /// Simple container for collection data objects.  Consists of a key and value.
        /// </summary>
        internal class DataObjectEntry
        {
            /// <summary>
            /// Key of item.
            /// </summary>
            internal String Key;

            /// <summary>
            /// Value of item
            /// </summary>
            internal Object Value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">Name (key) of item.</param>
            /// <param name="value">Value of item</param>
            internal DataObjectEntry(String name, Object value)
            {
                Key = name;
                Value = value;
            }
        }

        /// <summary>
        /// <see cref="IEnumerator"/> implementation for the data _collection.
        /// </summary>
        [Serializable]
        internal class DataCollectionValuesEnumerator : IEnumerator
        {
            /// <summary>
            /// Current enumerator _position.
            /// </summary>
            private int _position;

            /// <summary>
            /// Data _collection to enumerate.
            /// </summary>
            private readonly DataCollection _collection;

            /// <summary>
            /// Version to keep track of changes.
            /// </summary>
            private readonly int _version;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="collection">Collection of items to enumerate</param>
            internal DataCollectionValuesEnumerator(DataCollection collection)
            {
                _collection = collection;
                _version = collection._version;
                _position = -1;
            }

            #region IEnumerator Members

            /// <summary>
            /// Current enumerator item.
            /// </summary>
            public object Current
            {
                get
                {
                    if (!_collection._readOnly && _version != _collection._version)
                    {
                        throw new InvalidOperationException("Collection was modified.");
                    }

                    if (_position >= 0 && _position < _collection.Count)
                    {
                        return _collection.BaseGet(_position);
                    }

                    throw new InvalidOperationException("Collection was modified.");
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>Returns true if the move was successful.</returns>
            public bool MoveNext()
            {
                if (!_collection._readOnly && _version != _collection._version)
                {
                    throw new InvalidOperationException("Collection was modified.");
                }

                if (_position < _collection.Count - 1)
                {
                    ++_position;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Reset the enumerator _position.
            /// </summary>
            public void Reset()
            {
                if (!_collection._readOnly && _version != _collection._version)
                {
                    throw new InvalidOperationException("Collection was modified.");
                }

                _position = -1;
            }

            #endregion

        }

        /// <summary>
        /// Simple <see cref="ICollection"/> container for item _keys.
        /// </summary>
        [Serializable]
        public sealed class KeysCollection : ICollection
        {
            /// <summary>
            /// Collection of _keys items.
            /// </summary>
            private readonly DataCollection _collection;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="collection">Collection of _keys.</param>
            internal KeysCollection(DataCollection collection)
            {
                _collection = collection;
            }

            /// <summary>
            /// Get the key with the specified index.
            /// </summary>
            /// <param name="index">Index of key</param>
            /// <returns>Key</returns>
            public string Get(int index)
            {
                return _collection.BaseGetKey(index);
            }

            /// <summary>
            /// Get the key with the specified index.
            /// </summary>
            public string this[int index]
            {
                get { return Get(index); }
            }

            /// <summary>
            /// Get an enumerator for the key.
            /// </summary>
            /// <returns><see cref="IEnumerator"/> instance.</returns>
            public IEnumerator GetEnumerator()
            {
                return new DataCollectionValuesEnumerator(_collection);
            }

            /// <summary>
            /// Number of items in the _collection.
            /// </summary>
            public int Count
            {
                get { return _collection.Count; }
            }

            /// <summary>
            /// Copy items to an array.
            /// </summary>
            /// <param name="array">Array to copy items to.</param>
            /// <param name="index">Index in array.</param>
            void ICollection.CopyTo(Array array, int index)
            {
                string[] stringArray = array as string[];

                if (stringArray == null)
                {
                    throw new ArgumentException("Array is not string array.", "array");
                }

                CopyTo(stringArray, index);
            }

            /// <summary>
            /// Copy items to an array.
            /// </summary>
            /// <param name="array">Array to copy items to.</param>
            /// <param name="index">Index of array.</param>
            public void CopyTo(string[] array, int index)
            {
                for (IEnumerator e = GetEnumerator(); e.MoveNext(); )
                {
                    array.SetValue(e.Current, index++);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            object ICollection.SyncRoot
            {
                get { return _collection; }
            }

            /// <summary>
            /// 
            /// </summary>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }
        }

    }
}
