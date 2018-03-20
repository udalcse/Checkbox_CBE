using System;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Container class used to provide message status information.
    /// </summary>
    [Serializable]
    public class EmailMessageStatus : IEmailMessageStatus
    {
        #region IEmailMessageStatus Members

        /// <summary>
        /// Get/set whether the email was sent successfully.
        /// </summary>
        public bool SuccessfullySent { get; set; }

        /// <summary>
        /// Get/set the date/time of the last send attempt
        /// </summary>
        public DateTime? LastSendAttempt { get; set; }

        /// <summary>
        /// Get/set the date/time of the next send attempt.
        /// </summary>
        public DateTime? NextSendAttempt { get; set; }

        /// <summary>
        /// Get/set the text of the last send error.
        /// </summary>
        public string LastSendError { get; set; }

        /// <summary>
        /// Get BatchId value.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Get/set when message was queued
        /// </summary>
        public DateTime? QueuedDate { get; set; }

        #endregion
    }
}