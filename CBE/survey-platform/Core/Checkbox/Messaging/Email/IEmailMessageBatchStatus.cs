using System;
using System.Collections.Generic;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Get status information for a batch of emails.
    /// </summary>
    public interface IEmailMessageBatchStatus
    {
        /// <summary>
        /// Get a boolean indicating if the message was sent successfully.
        /// </summary>
        bool SuccessfullySent { get; }

        /// <summary>
        /// Get the date/time the last attempt to send the message was started.
        /// </summary>
        DateTime? BatchSendStarted { get; }

        /// <summary>
        /// Get date/time send completed
        /// </summary>
        DateTime? BatchSendCompleted { get; }

        /// <summary>
        /// Get the date/time the next attempt to send the message will occur.  Generally,
        /// this will only apply to messages that have a "Do Not Send Before" time specified.
        /// </summary>
        DateTime? NextSendAttempt { get; }

        /// <summary>
        /// Get the current status of the batch.
        /// </summary>
        BatchStatus CurrentStatus { get; set; }

        /// <summary>
        /// Get a list of identifiers for messages that were not sent successfully.
        /// </summary>
        List<long> FailedMessages { get; }

        /// <summary>
        /// Get the number of message sends attempted.
        /// </summary>
        int AttemptedCount { get; }

        /// <summary>
        /// Get the number of successfully sent messages.
        /// </summary>
        int SucceededCount { get; }

        /// <summary>
        /// Get the number of failed messages.
        /// </summary>
        int FailedCount { get; }
    }
}
