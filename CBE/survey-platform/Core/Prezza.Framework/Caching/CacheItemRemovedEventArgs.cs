using System;
using System.Collections.Generic;
using System.Text;

namespace Prezza.Framework.Caching
{
    /// <summary>
    /// Container for event arguments when an item is removed from a cache.
    /// </summary>
    public class CacheItemRemovedEventArgs : EventArgs
    {
        private string _key;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        public CacheItemRemovedEventArgs(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Get the key of the item that has been removed
        /// </summary>
        public string Key
        {
            get { return _key; }
        }
    }
}
