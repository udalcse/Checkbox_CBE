using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Metadata specific to items in surveys.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SurveyItemMetaData : ItemMetaData
    {
        /// <summary>
        /// Ids of items that are children of this item.  Currently, only matrix questions have children
        /// </summary>
        [DataMember]
        public int[] ChildItemIds { get; set; }

        /// <summary>
        /// Row containing item.  Only valid if item is child of a matrix question.
        /// </summary>
        [DataMember]
        public int RowPosition { get; set; }

        /// <summary>
        /// Column containing item.  Only valid if item is child of a matrix question.
        /// </summary>
        [DataMember]
        public int ColumnPosition { get; set; }

        /// <summary>
        /// Options contained in survey item
        /// </summary>
        [DataMember]
        public SurveyOptionMetaData[] Options { get; set; }

        /// <summary>
        /// Indicator of whether the item accepts answers or is display only.
        /// </summary>
        [DataMember]
        public bool IsAnswerable { get; set; }

        /// <summary>
        /// Indicates whether an answer is required for this item.  Applies only to
        /// items that are marked as Answerable
        /// </summary>
        [DataMember]
        public bool IsAnswerRequired { get; set; }

        
    }
}
