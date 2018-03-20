using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Answer for the get timeline request
    /// </summary>
    [DataContract]
    public class TimelineAnswer
    {
        /// <summary>
        /// Status. If Pending, the client must rerequest the result later
        /// </summary>
        [DataMember]
        public TimelineRequestStatus Status { get; set; }

        /// <summary>
        /// Results if the request was processed successfuly
        /// </summary>
        [DataMember]
        public TimelineAggregatedResult[] Results { get; set; }

        /// <summary>
        /// Client should cache the recieved results and use them during this period
        /// </summary>
        [DataMember]
        public int CacheTimeoutSeconds;

        /// <summary>
        /// Number of records per page
        /// </summary>
        [DataMember]
        public int RecordsPerPage;

        /// <summary>
        /// Error message
        /// </summary>
        [DataMember]
        public string Message;

        /// <summary>
        /// The ID of the last request being proceeded
        /// </summary>
        [DataMember]
        public long? RequestID { get; set; }
    }
}
