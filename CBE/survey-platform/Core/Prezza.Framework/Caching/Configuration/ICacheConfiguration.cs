using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prezza.Framework.Caching.Configuration;

namespace Prezza.Framework.Caching.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    interface ICacheConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        BackingStoreProviderData BackingStoreData
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheManagerName"></param>
        /// <returns></returns>
        CacheManagerData GetCacheManagerConfig(string cacheManagerName);

        /// <summary>
        /// 
        /// </summary>
        string DefaultCacheManager
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string CacheTypeName
        {
            get;
        }
    }
}
