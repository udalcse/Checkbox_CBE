using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Lightweight recipient class designed for serialization to web services.
    /// </summary>
    [Serializable]
    [DataContract]
    public class RecipientData
    {
        /// <summary>
        /// Get/set the recipient database id
        /// </summary>
        [DataMember]
        public long DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets a unique Guid
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        [DataMember]
        public string EmailToAddress { get; set; }

        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets an group id
        /// </summary>
        [DataMember]
        public int? GroupId { get; set; }

        /// <summary>
        /// Is group member
        /// </summary>
        [DataMember]
        public bool PanelMember { get; set; }

        /// <summary>
        /// Gets or sets an group id
        /// </summary>
        [DataMember]
        public int? EmailListId { get; set; }

        /// <summary>
        /// Gets or sets whether this Recipient has responded to an email invitation
        /// </summary>
        [DataMember]
        public bool HasResponded { get; set; }

        /// <summary>
        /// Gets or sets whether this Recipient has been sent an Invitation
        /// </summary>
        [DataMember]
        public bool Sent { get; set; }

        /// <summary>
        /// Gets or sets the date this Recipient was last sent an invitation
        /// </summary>
        [DataMember]
        public DateTime? LastSent { get; set; }

        /// <summary>
        /// Gets or sets whether the last Invitation was successfully sent to this Recipient
        /// </summary>
        [DataMember]
        public bool SuccessfullySent { get; set; }

        /// <summary>
        /// Gets or sets any error message associated with a failed send
        /// </summary>
        [DataMember]
        public string Error { get; set; }

        /// <summary>
        /// Number of messages sent to email address
        /// </summary>
        [DataMember]
        public int MessageCount { get; set; }

        /// <summary>
        /// True if this recipient has opted out of receiving further invitations
        /// </summary>
        [DataMember]
        public bool OptedOut { get; set; }

        /// <summary>
        /// True if this recipient email has been bounced
        /// </summary>
        [DataMember]
        public bool Bounced { get; set; }

        /// <summary>
        /// Survey id
        /// </summary>
        [DataMember]
        public int ResponseTemplateId { get; set; }
    }
}
