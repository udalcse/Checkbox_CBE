namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Enumerated type defining email message batch status.
    /// </summary>
    public enum BatchStatus
    {
        /// <summary>
        /// Unknown, reserved for error cases.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Batch is pending.  It has been created and can still accept new email messages.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Batch is closed.  It is ready for sending and no messages can be accepted.
        /// </summary>
        Closed = 2,

        /// <summary>
        /// Messages in the batch are currently sending.
        /// </summary>
        Sending = 3,

        /// <summary>
        /// All messages have been sent for delivery.  Completed status indicates only that an attempt
        /// to send each message was made, not whether all messages were sent successfully.
        /// </summary>
        Completed = 4
    }
}
