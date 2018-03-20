using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResponseAggregatedData
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public AggregateResult[] AggregateResults { set; get; }

        /// <summary>
        /// The number of completed responses.
        /// </summary>
        [DataMember]
        public int CompletedResponseCount { get; set; }
    }
}
