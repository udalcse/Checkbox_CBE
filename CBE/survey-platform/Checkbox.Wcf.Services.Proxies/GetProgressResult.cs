using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for progress data queries
    /// </summary>
    [Serializable]
    [DataContract]
    public class GetProgressResult
    {
        /// <summary>
        /// Get/set whether "Get" attempt was successful.
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Error message associated with failed get attempt
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Progress data associated with query
        /// </summary>
        [DataMember]
        public ProgressData ProgressData { get; set; }
    }
}
