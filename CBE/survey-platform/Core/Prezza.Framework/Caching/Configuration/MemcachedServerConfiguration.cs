using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prezza.Framework.Caching.Configuration
{
    /// <summary>
    /// Memcached Server Data 
    /// </summary>
    public class MemcachedServerConfiguration
    {
        /// <summary>
        /// Endpoint
        /// </summary>
        public string Endpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Server name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
