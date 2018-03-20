using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for summary of responses to survey
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResponseSummaryData
    {
        /// <summary>
        /// The id of the survey.
        /// </summary>
        [DataMember]
        public int SurveyId { get; set; }

        /// <summary>
        /// The number of completed responses.
        /// </summary>
        [DataMember]
        public int CompletedResponseCount { get; set; }

        /// <summary>
        /// The number of incomplete responses.
        /// </summary>
        [DataMember]
        public int IncompleteResponseCount { get; set; }

        /// <summary>
        /// The number of test responses.
        /// </summary>
        [DataMember]
        public int TestResponseCount { get; set; }
    }
}
