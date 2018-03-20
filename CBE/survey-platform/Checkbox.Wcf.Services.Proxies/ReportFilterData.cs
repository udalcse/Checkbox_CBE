using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for filter data proxy object
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportFilterData
    {
        /// <summary>
        /// Get database id of the filter
        /// </summary>
        [DataMember]
        public int FilterId { get; set; }

        /// <summary>
        /// Get the "type" of the filter source, such as Answer, ProfileProperty, ResponseProperty, etc.
        /// </summary>
        [DataMember]
        public string SourceType { get; set; }

        /// <summary>
        /// Get the "comparison" used by the filter.
        /// </summary>
        [DataMember]
        public string Comparison { get; set; }

        /// <summary>
        /// Get the "value" to use for filter comparison.  For answer values, this could be a string or the 
        /// database id of an item option.
        /// </summary>
        [DataMember]
        public string ValueAsString { get; set; }

        /// <summary>
        /// Get a human-readable version of the filter including source question text or value option
        /// text, if any.
        /// </summary>
        [DataMember]
        public string FilterText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ShortText { get; set; }
    }
}
