using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Representation of a "Detail Result", which is essentially the answer to a question
    /// </summary>
    [DataContract]
    [Serializable]
    public class DetailResult : ReportResult
    {
        /// <summary>
        /// Answer id associated with result
        /// </summary>
        [DataMember]
        public long AnswerId { get; set; }

        /// <summary>
        /// Response id associated with result
        /// </summary>
        [DataMember]
        public long ResponseId { get; set; }

        /// <summary>
        /// GUID of response associated with result
        /// </summary>
        [DataMember]
        public Guid? ResponseGuid { get; set; }
        
        /// <summary>
        /// Id of survey item answered
        /// </summary>
        [DataMember]
        public int ItemId { get; set; }

        /// <summary>
        /// Id of item option, if any, corresponding to chosen answer
        /// </summary>
        [DataMember]
        public int? OptionId { get; set; }

        /// <summary>
        /// Score associated with answer
        /// </summary>
        [DataMember]
        public double AnswerScore { get; set; }

        /// <summary>
        /// Answer is "other" option
        /// </summary>
        [DataMember]
        public bool IsAnswerOther { get; set; }

        /// <summary>
        /// Id of survey page whre item answered
        /// </summary>
        [DataMember]
        public int? PageId { get; set; }
    }
}
