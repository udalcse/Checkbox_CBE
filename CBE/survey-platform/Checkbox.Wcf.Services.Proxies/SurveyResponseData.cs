

using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for completed survey response information.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyResponseData
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int64 ResponseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid ResponseGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Int32 ResponseTemplateId { get; set; }

        /// <summary>
        /// Unique identifier of respondent, if respondent is registered user
        /// </summary>
        [DataMember]
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// Guid identifying anonymous respondent
        /// </summary>
        [DataMember]
        public Guid? AnonymousRespondentGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? DateStarted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? DateCompleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? DateLastEdited { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int CurrentPageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid? WorkflowInstanceId { get; set; }
    }
}