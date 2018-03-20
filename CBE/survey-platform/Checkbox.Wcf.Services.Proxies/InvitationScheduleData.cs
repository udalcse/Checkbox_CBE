using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Basic container of information for scheduled invitations suitable for serialization to web services.
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvitationScheduleData
    {
        /// <summary>
        /// Get/set the database id of the invitation schedule item
        /// </summary>
        /// <remarks>This field is used as the key to look up the invitation schedule item to modify when making changes
        /// via the Invitation Management Service UpdateInvitationSchedule(...) method.</remarks>
        [DataMember]
        public int? InvitationScheduleID { get; set; }

        /// <summary>
        /// Get/set the database id of the invitation item
        /// </summary>
        [DataMember]
        public int? InvitationID { get; set; }

        /// <summary>
        /// Date to send the invitation
        /// </summary>
        [DataMember]
        public DateTime? Scheduled { get; set; }

        /// <summary>
        /// Date of when the process was started
        /// </summary>
        [DataMember]
        public DateTime? ProcessingStarted { get; set; }

        /// <summary>
        /// Date of when the process was finished
        /// </summary>
        [DataMember]
        public DateTime? ProcessingFinished { get; set; }

        /// <summary>
        /// Date of when the process was finished
        /// </summary>
        [DataMember]
        public bool CanBeDeleted { get; set; }

        /// <summary>
        /// Displays the type of the sending: is it a reminder or an invitation
        /// </summary>
        [DataMember]
        public string InvitationActivityType { get; set; }

        /// <summary>
        /// Error text of the batch
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Bounced emails
        /// </summary>
        [DataMember]
        public string[] BouncedEmails { get; set; }
    }
}
