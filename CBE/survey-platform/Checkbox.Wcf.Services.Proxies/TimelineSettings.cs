using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Timeline settings
    /// </summary>
    [DataContract]
    public class TimelineSettings
    {
        /// <summary>
        /// Event name as it will be displayed in list of settings
        /// </summary>
        [DataMember]
        public string FullEventName { get; set; }
        /// <summary>
        /// Event name
        /// </summary>
        [DataMember]
        public string EventName { get; set; }

        /// <summary>
        /// Manager name
        /// </summary>
        [DataMember]
        public string Manager { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        [DataMember]
        public bool Single { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        [DataMember]
        public bool Daily { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        [DataMember]
        public bool Weekly { get; set; }

        /// <summary>
        /// Setting
        /// </summary>
        [DataMember]
        public bool Monthly { get; set; }

        /// <summary>
        /// Order of event will be displayed in timeline
        /// </summary>
        [DataMember]
        public int EventOrder { get; set; }
    }
}
