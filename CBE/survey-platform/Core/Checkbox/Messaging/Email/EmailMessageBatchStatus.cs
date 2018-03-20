using System;
using System.Collections.Generic;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Container class for providing email message batch status
    /// information.
    /// </summary>
    [Serializable]
    public class EmailMessageBatchStatus : IEmailMessageBatchStatus
    {

        #region IEmailMessageBatchStatus Members

        /// <summary>
        /// Get/set indicator of whether the batch, as a whole, was succesful.
        /// </summary>
        public bool SuccessfullySent { get; set; }

        /// <summary>
        /// Get/set the time the batch started
        /// </summary>
        public DateTime? BatchSendStarted { get; set; }

        /// <summary>
        /// Get/set the time the batch ended
        /// </summary>
        public DateTime? BatchSendCompleted { get; set; }

        /// <summary>
        /// Get/set the next send attempt for the batch.
        /// </summary>
        public DateTime? NextSendAttempt { get; set; }

        /// <summary>
        /// Get/set the current batch status.
        /// </summary>
        public BatchStatus CurrentStatus { get; set; }

        /// <summary>
        /// Get/set list of identifiers for failed messages.
        /// </summary>
        public List<long> FailedMessages { get; set; }

        /// <summary>
        /// Get/set number of messages attempted to send as part of batch
        /// </summary>
        public int AttemptedCount { get; set; }

        /// <summary>
        /// Get/set number of messages sent successfully
        /// </summary>
        public int SucceededCount { get; set; }

        /// <summary>
        /// Get/set number of messages failed
        /// </summary>
        public int FailedCount { get; set; }

        #endregion
    }
}