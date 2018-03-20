using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Encapsulation of summary data associated with survey period.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyPeriod
    {
        /// <summary>
        /// Survey period ID
        /// </summary>
        [DataMember]
        public int SurveyPeriodID { get; set; }

        /// <summary>
        /// Date when the period starts
        /// </summary>
        [DataMember]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Date when the period finishes
        /// </summary>
        [DataMember]
        public DateTime? Finish { get; set; }

        /// <summary>
        /// Period name
        /// </summary>
        [DataMember]
        public string PeriodName { get; set; }

        /// <summary>
        /// ID of the parent survey
        /// </summary>
        [DataMember]
        public int ResponseTemplateID { get; set; }
    }
}
