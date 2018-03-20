using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Search results
    /// </summary>
    [DataContract]
    [Serializable]
    public class SearchAnswer
    {
        /// <summary>
        /// Request ID
        /// </summary>
        [DataMember]
        public Guid RequestID
        {
            get;
            set;
        }

        /// <summary>
        /// Is request totally processed
        /// </summary>
        [DataMember]
        public bool Completed
        {
            get;
            set;
        }

        /// <summary>
        /// Is request in progress, nevertheless it can contain some results
        /// </summary>
        [DataMember]
        public bool Pending
        {
            get;
            set;
        }

        /// <summary>
        /// Is request is collecting objects available for the given user
        /// </summary>
        [DataMember]
        public bool CollectingObjects
        {
            get;
            set;
        }

        /// <summary>
        /// Results that have been already found 
        /// </summary>
        [DataMember]
        public SearchAnswerData[] Results
        {
            get;
            set;
        }
    }
}
