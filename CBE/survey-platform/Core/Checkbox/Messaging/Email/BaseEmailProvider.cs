using System;
using System.Text;
using Checkbox.Management;
using Prezza.Framework.Configuration;

namespace Checkbox.Messaging.Email
{
    ///<summary>
    ///Base email provider class.
    ///</summary>
    public abstract class BaseEmailProvider : IEmailProvider
    {
        /// <summary>
        /// Get/set configuration name for the email provider
        /// </summary>
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Initialize the provider with its configuration
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ConfigurationBase config)
        {
            DoInitialize(config);
        }

        /// <summary>
        /// Add line breaks to a string using the max line length application setting if it is enabled.
        /// </summary>
        /// <param name="stringToBreak"></param>
        /// <returns></returns>
        protected static string AddLineBreaks(string stringToBreak)
        {
            if (!ApplicationManager.AppSettings.LimitEmailMessageLineLength)
            {
                return stringToBreak;
            }

            StringBuilder sb = new StringBuilder();

            int rangeStart = 0;

            while (rangeStart >= 0)
            {
                int newlineIndex = stringToBreak.IndexOf(Environment.NewLine, rangeStart);

                //Grab the first MaxLineLength chars, or up to the first newline, whichever amount is smaller
                if (newlineIndex > 0 && newlineIndex < rangeStart + ApplicationManager.AppSettings.MaxEmailMessageLineLength)
                {
                    sb.Append(stringToBreak.Substring(rangeStart, newlineIndex - rangeStart + 1));
                    rangeStart = newlineIndex + 1;
                }
                else
                {
                    if (stringToBreak.Length >= rangeStart + ApplicationManager.AppSettings.MaxEmailMessageLineLength)
                    {
                        sb.Append(stringToBreak.Substring(rangeStart, ApplicationManager.AppSettings.MaxEmailMessageLineLength));
                        rangeStart = rangeStart + ApplicationManager.AppSettings.MaxEmailMessageLineLength;
                    }
                    else
                    {
                        sb.Append(stringToBreak.Substring(rangeStart));
                        rangeStart = -1;
                    }
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        #region IEmailProvider Batch Members

        /// <summary>
        /// Return a boolean indicating if the email provider supports batches or not.
        /// </summary>
        public abstract bool SupportsBatches { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="message"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        public long SendMessage(string name, string createdBy, IEmailMessage message, DateTime? earliestSendDate)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return DoSendMessage(name, createdBy, message, earliestSendDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        public long CreateEmailMessageBatch(string name, string createdBy, DateTime? earliestSendDate, int scheduleID)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return DoCreateEmailMessageBatch(name, createdBy, earliestSendDate, scheduleID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long AddEmailMessageToBatch(long batchId, IEmailMessage message)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return DoAddEmailMessageToBatch(batchId, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void CloseEmailMessageBatch(long batchId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            DoCloseEmailMessageBatch(batchId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public IEmailMessageStatus GetEmailMessageStatus(long emailId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return DoGetEmailMessageStatus(emailId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public IEmailMessageBatchStatus GetEmailMessageBatchStatus(long batchId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return GetEmailMessageBatchStatus(batchId);
        }

        /// <summary>
        /// Delete a queued message.
        /// </summary>
        /// <param name="messageId"></param>
        public void DeleteEmailMessage(long messageId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            DoDeleteEmailMessage(messageId);
        }

        /// <summary>
        /// Delete a queued message batch.
        /// </summary>
        /// <param name="batchId"></param>
        public void DeleteEmailMessageBatch(long batchId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            DoDeleteEmailMessageBatch(batchId);
        }

        /// <summary>
        /// Register a message attachment for inclusion with email messages.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public long RegisterMessageAttachment(IEmailAttachment attachment)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            return DoRegisterEmailAttachment(attachment);
        }

        /// <summary>
        /// Delete an email message attachment
        /// </summary>
        /// <param name="attachmentId"></param>
        public void DeleteMessageAttachment(long attachmentId)
        {
            if (!SupportsBatches)
            {
                throw new EmailProviderDoesNotSupportBatchesException();
            }

            DoDeleteMessageAttachment(attachmentId);
        }

        /// <summary>
        /// Retrieve an email message.
        /// </summary>
        /// <param name="messageId">Id of message to retrieve.</param>
        /// <returns>Message values.</returns>
        public IEmailMessage GetMessage(long messageId)
        {
            if (!SupportsBatches)
            {
                return null;
            }

            return DoGetMessage(messageId);
        }

        /// <summary>
        /// Changes the scheduled date for the batch
        /// </summary>
        /// <param name="batchId">Id of the batch</param>
        /// <param name="scheduledDate">New date when the batch should be sent</param>
        public abstract void SetMessageBatchDate(long batchId, DateTime scheduledDate);

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Overridable configuration method
        /// </summary>
        /// <param name="config"></param>
        protected virtual void DoInitialize(ConfigurationBase config) { }

        /// <summary>
        /// Perform the work of sending the email message.
        /// </summary>
        /// <param name="message"></param>
        public long? Send(IEmailMessage message)
        {
            return DoSendMessage(message);
        }

        /// <summary>
        /// Overridable method to send the message
        /// </summary>
        /// <param name="message"></param>
        protected abstract long? DoSendMessage(IEmailMessage message);


        /// <summary>
        /// Send message for specific providers to override.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="message"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        protected virtual long DoSendMessage(string name, string createdBy, IEmailMessage message, DateTime? earliestSendDate) { return -1; }

        /// <summary>
        /// Create batch  for specific providers to override.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        protected virtual long DoCreateEmailMessageBatch(string name, string createdBy, DateTime? earliestSendDate, int scheduleID) { return -1; }

        /// <summary>
        /// Add message to batch  for specific providers to override.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        protected virtual long DoAddEmailMessageToBatch(long batchId, IEmailMessage message) { return -1; }

        /// <summary>
        /// Close batch  for specific providers to override.
        /// </summary>
        /// <param name="batchId"></param>
        protected virtual void DoCloseEmailMessageBatch(long batchId) { }

        /// <summary>
        /// Get message status for specific providers to override.
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        protected virtual IEmailMessageStatus DoGetEmailMessageStatus(long emailId) { return null; }

        /// <summary>
        /// Get message batch status  for specific providers to override.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        protected virtual IEmailMessageBatchStatus DoGetEmailMessageBatchStatus(long batchId) { return null; }

        /// <summary>
        /// Delete an email message
        /// </summary>
        /// <param name="messageId"></param>
        protected virtual void DoDeleteEmailMessage(long messageId) { }

        /// <summary>
        /// Delete an email batch for specific providers to override.
        /// </summary>
        /// <param name="batchId"></param>
        public virtual void DoDeleteEmailMessageBatch(long batchId) { }

        /// <summary>
        /// Register an attachment for specific providers to override.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        protected virtual long DoRegisterEmailAttachment(IEmailAttachment attachment) { return -1; }

        /// <summary>
        /// Delete a previously-registered email attachment for specific providers to override.
        /// </summary>
        /// <param name="attachmentId"></param>
        protected virtual void DoDeleteMessageAttachment(long attachmentId) { }

        /// <summary>
        /// Overridable method to get an email message.
        /// </summary>
        /// <param name="messageId"></param>
        protected virtual IEmailMessage DoGetMessage(long messageId) { return null; }

        /// <summary>
        /// Returns recent status of the batch
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public virtual string GetBatchErrorText(int scheduleID)
        {
            return string.Empty;
        }
        
        /// <summary>
        /// Returns an array of bounced emails
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public virtual string[] GetBouncedEmails(int scheduleID)
        {
            return new string[] { };
        }

        public virtual string[] GetBouncedEmails()
        {
            return new string[] { };
        }

        #endregion
    }
}
