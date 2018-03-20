using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Aggregated result for the timeline
    /// </summary>
    [DataContract]
    public class TimelineAggregatedResult
    {
        /// <summary>
        /// Event ID
        /// </summary>
        [DataMember]
        public long EventID { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        [DataMember]
        public string UserID { get; set; }
        /// <summary>
        /// Object (i.e. survey, item or response) ID
        /// </summary>
        [DataMember]
        public string ObjectID { get; set; }
        /// <summary>
        /// Object (i.e. survey, item or response) Guid
        /// </summary>
        [DataMember]
        public Guid? ObjectGUID { get; set; }
        /// <summary>
        /// Count of events occured
        /// </summary>
        [DataMember]
        public long Count { get; set; }
        /// <summary>
        /// Date when the event was occured
        /// </summary>
        [DataMember]
        public DateTime? Date { get; set; }
        /// <summary>
        /// Period: 1-single event, 2-daily, 3-weekly, 4-monthly
        /// </summary>
        [DataMember]
        public int Period { get; set; }
        /// <summary>
        /// Event name
        /// </summary>
        [DataMember]
        public string EventName { get; set; }
        /// <summary>
        /// Image that must be displayed at the event record
        /// </summary>
        [DataMember]
        public string Image { get; set; }
        /// <summary>
        /// URL to trace event object(s)
        /// </summary>
        [DataMember]
        public string Url { get; set; }
        /// <summary>
        /// Parent ID
        /// </summary>
        [DataMember]
        public long? ObjectParentID { get; set; }

        /// <summary>
        /// Parent object name
        /// </summary>
        [DataMember]
        public string ObjectParentName { get; set; }
    }
}
