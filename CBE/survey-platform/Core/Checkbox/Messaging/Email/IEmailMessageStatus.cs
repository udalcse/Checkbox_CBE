using System;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Interface definition for an object that provides status information about email messages.
    /// </summary>
    public interface IEmailMessageStatus
    {
        /// <summary>
        /// Get a boolean indicating if the message was sent successfully.
        /// </summary>
        bool SuccessfullySent { get; }

        /// <summary>
        /// Get the date/time the last attempt to send the message was made.
        /// </summary>
        DateTime? LastSendAttempt { get; }

        /// <summary>
        /// Get the date/time the next attempt to send the message will occur.  Generally,
        /// this will only apply to messages that have a "Do Not Send Before" time specified.
        /// </summary>
        DateTime? NextSendAttempt { get; }

        /// <summary>
        /// Get the error message from an unsuccessful send attempt.
        /// </summary>
        string LastSendError { get; }

        /// <summary>
        /// Get BatchId value.
        /// </summary>
        int BatchId { get; }

        /// <summary>
        /// Get/set when message was queued
        /// </summary>
        DateTime? QueuedDate { get; }
    }
}
