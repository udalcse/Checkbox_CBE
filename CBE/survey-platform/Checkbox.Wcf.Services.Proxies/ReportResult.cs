using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for a single result data point for an analysis item
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReportResult
    {
        /// <summary>
        /// Get/set result index used to order results
        /// </summary>
        [DataMember]
        public int ResultIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ResultKey { get; set; }

        /// <summary>
        /// Get/set text of answer
        /// </summary>
        [DataMember]
        public string ResultText { get; set; }

        /// <summary>
        /// Get/set points
        /// </summary>
        [DataMember]
        public double Points { get; set; }
    }
}
