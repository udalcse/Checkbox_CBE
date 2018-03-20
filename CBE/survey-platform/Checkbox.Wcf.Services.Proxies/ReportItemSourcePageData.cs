using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for data related to source pages for analysis items
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportItemSourcePageData
    {
        [DataMember]
        public int? PageId { get; set; }

        [DataMember]
        public int? Position { get; set; }

        /// <summary>
        /// Get/set text to display in reports for item
        /// </summary>
        [DataMember]
        public string ReportingText { get; set; }
    }
}
