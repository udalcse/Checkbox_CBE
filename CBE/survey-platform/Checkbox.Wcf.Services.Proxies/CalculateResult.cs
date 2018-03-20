using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Representation of a "Calculate result".
    /// </summary>
    [DataContract]
    [Serializable]
    public class CalculateResult : ReportResult
    {
        /// <summary>
        /// Get/set result value
        /// </summary>
        [DataMember]
        public double ResultValue { get; set; }

        /// <summary>
        /// Get/set result value
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        [DataMember]
        public double MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        [DataMember]
        public double MaxValue { get; set; }
    }
}