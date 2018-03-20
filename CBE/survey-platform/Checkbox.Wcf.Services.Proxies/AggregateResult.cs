using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Base container for analysis item results
    /// </summary>
    [DataContract]
    [Serializable]
    public class AggregateResult : ReportResult
    {
        /// <summary>
        /// Get/set # of answers
        /// </summary>
        [DataMember]
        public int AnswerCount { get; set; }

        /// <summary>
        /// Get/set percent
        /// </summary>
        [DataMember]
        public double AnswerPercent { get; set; }

        /// <summary>
        /// Get answer percent as decimal value. Will be AnswerPercent / 100.
        /// </summary>
        public double AnswerPercentDecimal { get { return AnswerPercent/(double)100; } }
    }
}
