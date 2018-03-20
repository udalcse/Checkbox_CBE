using System;
using Prezza.Framework.Configuration;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Interface for email providers used by Checkbox.
    /// </summary>
    public interface IEmailProvider : IConfigurationProvider
    {
        /// <summary>
        /// Get a boolean value indicating if the email provider supports batch operations.
        /// </summary>
        bool SupportsBatches { get; }

        /// <summary>
        /// Send the specified email message and return an identifier that can be used later
        /// to check the status of the sent message.  For email providers that don't support
        /// later querying for message status, a NULL value will be returned.  Messages sent using
        /// this method are not part of any email batches.
        /// </summary>
        /// <param name="message">Message to send.</param>
        long? Send(IEmailMessage message);


        #region Email Batch Operations

        /// <summary>
        /// Queue an email to be sent outside the context of a batch.
        /// </summary>
        /// <param name="name">Name associated with the message.</param>
        /// <param name="createdBy">Creator of the message.</param>
        /// <param name="message">Message to sent.</param>
        /// <param name="earliestSendDate">Optional earliest send date for the message.  If the value is null,
        /// the message will be queued for immediate delivery.</param>
        /// <returns>Identifier representing the message.</returns>
        long SendMessage(
            string name,
            string createdBy,
            IEmailMessage message,
            DateTime? earliestSendDate);

        /// <summary>
        /// Create an email message batch.  
        /// </summary>
        /// <param name="name">Name of batch.</param>
        /// <param name="createdBy">Creator of the batch.</param>
        /// <param name="earliestSendDate">Earliest delivery date for the batch.  A NULL value indicates that the batch should be sent
        /// as soon as it is "closed".</param>
        /// <param name="scheduleID">Id of the schedule record</param>
        /// <returns>Identifier of newly created batch.</returns>
        long CreateEmailMessageBatch(
            string name,
            string createdBy,
            DateTime? earliestSendDate,
            int scheduleID);


        /// <summary>
        /// Add an email message to the specified batch.
        /// </summary>
        /// <param name="batchId">Identifier of batch to add.</param>
        /// <param name="message">Email message to add to the batch.</param>
        /// <returns>Identifier for the email message.</returns>
        long AddEmailMessageToBatch(long batchId, IEmailMessage message);

        /// <summary>
        /// "Close" a batch, which means all emails have been added and it is now ready to be sent.
        /// </summary>
        /// <param name="batchId">Identifier of the batch to close.</param>
        void CloseEmailMessageBatch(long batchId);

        /// <summary>
        /// Get the status of an email message.
        /// </summary>
        /// <param name="emailId">Identifier of email message to get the status of.</param>
        /// <returns></returns>
        IEmailMessageStatus GetEmailMessageStatus(long emailId);

        /// <summary>
        /// Get the contents of an email message.
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns>IEmailMessage object containing message information.</returns>
        /// <remarks>Not all email providers will support this method.  Providers that do not support this method
        /// should return null values.</remarks>
        IEmailMessage GetMessage(long emailId);

        /// <summary>
        /// Get the status of an email message batch.
        /// </summary>
        /// <param name="batchId">Identifier of message batch to get status of.</param>
        /// <returns></returns>
        IEmailMessageBatchStatus GetEmailMessageBatchStatus(long batchId);

        /// <summary>
        /// Register a message attachment to be used a a "by ref" attachment.  This allows multiple
        /// emails to have the same attachment without the need to upload the attachment to the queue
        /// database for each message.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        long RegisterMessageAttachment(IEmailAttachment attachment);

        /// <summary>
        /// Delete an email message from the queue of messages to send.
        /// </summary>
        /// <param name="messageId">Id of message to delete.</param>
        void DeleteEmailMessage(long messageId);

        /// <summary>
        /// Delete a message batch and all messages that are part of the batch.
        /// </summary>
        /// <param name="batchId">Id of batch to delete.</param>
        /// <remarks>Batches that are in-progress can be deleted.  Depending on the underlying provider implementation,
        /// the deleting the batch mid-send may not abort the batch.</remarks>
        void DeleteEmailMessageBatch(long batchId);

        /// <summary>
        /// Remove a message attachment from the database.  If the attachment is in use by any existing messages, it will
        /// be removed from those messages as well.
        /// </summary>
        /// <param name="attachmentId">Id of attachment to delete.</param>
        void DeleteMessageAttachment(long attachmentId);

        /// <summary>
        /// Changes the scheduled date for the batch
        /// </summary>
        /// <param name="batchId">Id of the batch</param>
        /// <param name="scheduledDate">New date when the batch should be sent</param>
        void SetMessageBatchDate(long batchId, DateTime scheduledDate);


        /// <summary>
        /// Returns recent status of the batch
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        string GetBatchErrorText(int scheduleID);

        /// <summary>
        /// Returns an array of bounced emails
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        string[] GetBouncedEmails(int scheduleID);

        /// <summary>
        /// Returns all bounced emails
        /// </summary>
        /// <returns></returns>
        string[] GetBouncedEmails();

        #endregion
    }
}
