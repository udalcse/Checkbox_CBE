
using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for data related to source items for analysis items
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportItemSourceItemData
    {
        /// <summary>
        /// Get/set id of source item
        /// </summary>
        [DataMember]
        public int ItemId { get; set; }

        [DataMember]
        public string ItemType { get; set; }

        /// <summary>
        /// Get/set text to display in reports for item
        /// </summary>
        [DataMember]
        public string ReportingText { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public int PagePosition { get; set; }

        [DataMember]
        public int ItemPosition { get; set; }

        [DataMember]
        public int? ParentRowNumber { get; set; }

        [DataMember]
        public int? ParentColumnNumber { get; set; }

        [DataMember]
        public ReportItemSourceOptionData[] Options { get; set; }

        [DataMember]
        public int ResponseCount { get; set; }

        [DataMember]
        public int AnswerCount { get; set; }
    }
}
