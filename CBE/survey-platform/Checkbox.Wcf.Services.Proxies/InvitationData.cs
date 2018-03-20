using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Basic container of information for invitations suitable for serialization to web services.
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvitationData
    {
        /// <summary>
        /// Get/set the database id of the invitation
        /// </summary>
        /// <remarks>This field is used as the key to look up the invitation to modify when making changes
        /// via the Invitation Management Service UpdateInvitation(...) method.</remarks>
        [DataMember]
        public int DatabaseId { get; set; }

        /// <summary>
        /// Get invitation name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get/set the id of the survey this is an invitation for.
        /// </summary>
        /// <remarks>This field value is not persisted to the Checkbox database when using
        /// the Invitation Management Service UpdateInvitation(...) method.</remarks>
        [DataMember]
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Get/set the date the invitation was created.
        /// </summary>
        /// <remarks>This field value is not persisted to the Checkbox database when using
        /// the Invitation Management Service UpdateInvitation(...) method.</remarks>
        [DataMember]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Get/set the creator of the invitation
        /// </summary>
        /// <remarks>This field value is not persisted to the Checkbox database when using
        /// the Invitation Management Service UpdateInvitation(...) method.</remarks>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set the invitation guid
        /// </summary>
        /// <remarks>This field value is not persisted to the Checkbox database when using
        /// the Invitation Management Service UpdateInvitation(...) method.</remarks>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Get/set the invitation mail format.  Valid values are "Html" and "Text"
        /// </summary>
        [DataMember]
        public string MailFormat { get; set; }

        /// <summary>
        /// Get/set the invitation subject.
        /// </summary>
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Get/set the invitation body
        /// </summary>
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// Get/set the from address for invitation emails.
        /// </summary>
        [DataMember]
        public string FromAddress { get; set; }

        /// <summary>
        /// Get/set the from name for invitation emails.
        /// </summary>
        [DataMember]
        public string FromName { get; set; }

        /// <summary>
        /// Get/set the link text for the survey link in HTML invitations.
        /// </summary>
        [DataMember]
        public string LinkText { get; set; }

        /// <summary>
        /// Get/set the login option for the invitation.  Valid values are "None" and "Auto"
        /// </summary>
        [DataMember]
        public string LoginOption { get; set; }

        /// <summary>
        /// Get/set whether to include an opt out link
        /// </summary>
        [DataMember]
        public bool IncludeOptOut { get; set; }

        /// <summary>
        /// Get/set the text for the opt out link in HTML invitations
        /// </summary>
        [DataMember]
        public string OptOutText { get; set; }

        /// <summary>
        /// Date time invitation was last sent to recipients.   Value will be null for unsent invitations.
        /// </summary>
        [DataMember]
        public DateTime? LastSent { get; set; }

        /// <summary>
        /// Date time invitation when the invitation should be set
        /// </summary>
        [DataMember]
        public DateTime? Scheduled { get; set; }

        /// <summary>
        /// Date time invitation when the invitation should be set
        /// </summary>
        [DataMember]
        public int? ScheduleID { get; set; }

        /// <summary>
        /// Defines whether it is a reminder
        /// </summary>
        [DataMember]
        public string InvitationActivityType { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Status description
        /// </summary>
        [DataMember]
        public string StatusDescription { get; set; }

        /// <summary>
        /// Status of the last sent invitation
        /// </summary>
        [DataMember]
        public bool? SuccessfullySent { get; set; }

        /// <summary>
        /// Error message of the last sent invitation
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
