using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prezza.Framework.Caching.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MemcachedPoolConfiguration
    {
        public MemcachedPoolConfiguration()
        {
            CacheManagers = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsDefault
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int InitialConnections
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinSpareConnections
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int MaxSpareConnections
        {
            get;
            set;
        }
          
        /// <summary>
        /// 
        /// </summary>
        public long MaxIdleTime
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public long MaxBusyTime
        {
            get;
            set;
        }
          
        /// <summary>
        /// 
        /// </summary>
        public long MainThreadSleep
        {
            get;
            set;
        }
          
        /// <summary>
        /// 
        /// </summary>
        public int SocketTimeOut
        {
            get;
            set;
        }
          
        /// <summary>
        /// 
        /// </summary>
        public int SocketConnectTO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Failover
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NagleAlg
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ServerNames
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int[] ServerWeights
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> CacheManagers
        {
            get;
            set;
        }
    }
}
