using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for invitation recipient summary data
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvitationRecipientSummary
    {
        /// <summary>
        /// The number of recipients that have been added to an invitation but have not yet been emailed a copy.
        /// </summary>
        [DataMember]
        public int PendingCount { get; set; }

        /// <summary>
        /// The number of recipients that have responded.
        /// </summary>
        [DataMember]
        public int RespondedCount { get; set; }

        /// <summary>
        /// The number of recipients that have not responded.
        /// </summary>
        [DataMember]
        public int NotRespondedCount { get; set; }

        /// <summary>
        /// The number of recipients that have opted out.
        /// </summary>
        [DataMember]
        public int OptedOutCount { get; set; }

        /// <summary>
        ///The total number recipients that not been removed from the invitation or opted out.
        /// </summary>
        [DataMember]
        public int CurrentCount { get; set; }

        /// <summary>
        ///The total number recipients which emails have been bounced
        /// </summary>
        [DataMember]
        public int BouncedCount { get; set; }
    }
}
