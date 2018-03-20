using System;
using System.Threading;
using Checkbox.Management;
using Checkbox.Management.Licensing.Limits;
using Prezza.Framework.ExceptionHandling;
using Checkbox.LicenseLibrary;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Public gateway for sending email messages.  Handles some batch support as well as other operations.
    /// </summary>
    public static class EmailGateway
    {
        /// <summary>
        /// Return a boolean indicating if the designated email provider supports email batching
        /// </summary>
        public static bool ProviderSupportsBatches
        {
            get
            {
                return EmailFactory.GetEmailProvider().SupportsBatches;
            }
        }

        /// <summary>
        /// Return a boolean indicating if the configuration contains the provider that supports batches
        /// </summary>
        public static bool HasBatchSupportiveProvider
        {
            get
            {
                return EmailFactory.HasBatchSupportiveProvider;
            }
        }

        /// <summary>
        /// Changes the default e-mail provider
        /// </summary>
        /// <param name="providerName"></param>
        public static void ChangeEmailProvider(string providerName)
        {
            EmailFactory.ChangeEmailProvider(providerName);            
        }



        /// <summary>
        /// If application email is enabled, instantiate an email provider and use it to send the specified message.
        /// If email is not enabled, no email sending occurs.
        /// If email test mode is enabled, the executing thread sleeps for a configurable amount of time. <see cref="AppSettings.EmailTestModeSleepTime"/>
        /// </summary>
        /// <param name="message">Message to send.</param>
        public static long? Send(IEmailMessage message)
        {
            try
            {
                //Only send if email is enabled
                if (ApplicationManager.AppSettings.EmailEnabled)
                {
                    //Check if the license allows to send an email.
                    EmailLimit emailLimit = new EmailLimit();
                    String errorMsg;
                    if (emailLimit.Validate(out errorMsg) == LimitValidationResult.LimitNotReached)
                    {
                        long? result = EmailFactory.GetEmailProvider().Send(message);
                        
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return null;
        }

        #region Batch Operations


        /// <summary>
        /// Create a new batch for sending emails.  
        /// </summary>
        /// <param name="batchName">Name of new batch.</param>
        /// <param name="createdBy">Creator of the new batch.</param>
        /// <param name="earliestSendDate">Earliest date/time batch should be sent, subject to support by the 
        /// configured email provider.</param>
        /// <returns>Identifer for the batch.</returns>
        /// <remarks>Configured email provider must support batch operations or an exception will be thrown.</remarks>
        public static long? CreateEmailBatch(string batchName, string createdBy, DateTime? earliestSendDate, int scheduleID)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().CreateEmailMessageBatch(batchName, createdBy, earliestSendDate, scheduleID);
            }

            return null;
        }

        /// <summary>
        /// Mark a previously created email batch as ready for sending.
        /// </summary>
        /// <param name="batchId">Database Id of batch to send.</param>
        public static void MarkEmailBatchReady(long batchId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                EmailFactory.GetEmailProvider().CloseEmailMessageBatch(batchId);
            }
        }

        /// <summary>
        /// Add the specified email message to a previously created message batch.
        /// </summary>
        /// <param name="batchId">Id of batch to add the message to.</param>
        /// <param name="message">Message to add.</param>
        /// <returns>Identifier for the added message.</returns>
        public static long? AddEmailMessageToBatch(long batchId, IEmailMessage message)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().AddEmailMessageToBatch(batchId, message);
            }

            return null;
        }

        /// <summary>
        /// Register an attachment with the email provider for inclusion "by reference" in separate emails.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static long? RegisterAttachment(IEmailAttachment attachment)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().RegisterMessageAttachment(attachment);
            }

            return null;
        }

        /// <summary>
        /// Get the status of a previously created email batch.
        /// </summary>
        /// <param name="batchId">ID of batch to get status of.</param>
        /// <returns>Object containing batch status information.</returns>
        public static IEmailMessageBatchStatus GetMessageBatchStatus(long batchId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetEmailMessageBatchStatus(batchId);
            }

            return null;
        }

        /// <summary>
        /// Get the status of a previously created email message.
        /// </summary>
        /// <param name="messageId">ID of message to get status of.</param>
        /// <returns>Object containing message status information.</returns>
        public static IEmailMessageStatus GetMessageStatus(long messageId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetEmailMessageStatus(messageId);
            }

            return null;
        }

        /// <summary>
        /// Delete an email message from the message Queue.
        /// </summary>
        /// <param name="messageId">Id of message to delete.</param>
        /// <remarks>This will remove a message from the messaging queue.</remarks>
        public static void DeleteMessage(long messageId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                EmailFactory.GetEmailProvider().DeleteEmailMessage(messageId);
            }
        }

        /// <summary>
        /// Delete an email message from the message Queue.
        /// </summary>
        /// <param name="batchId">Id of batch to delete.</param>
        /// <remarks>This will remove a batch and associated messages from the messaging queue.</remarks>
        public static void DeleteMessageBatch(long batchId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                EmailFactory.GetEmailProvider().DeleteEmailMessage(batchId);
            }
        }

        /// <summary>
        /// Retrieve a message that was previously submitted to the message Queue.  Not all providers
        /// support this method, so null values may be returned when the message can't be found AND
        /// when the provider does not support retrieving messages at a later date.
        /// </summary>
        /// <param name="messageId">Id of message to retrieve.</param>
        /// <returns>Message object representing email message.</returns>
        public static IEmailMessage GetMessageFromBatch(long messageId)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetMessage(messageId);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        public static void SetMessageBatchDate(long batchId, DateTime scheduledDate)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                EmailFactory.GetEmailProvider().SetMessageBatchDate(batchId, scheduledDate);
            }
        }


        /// <summary>
        /// Get the status of a previously created email batch.
        /// </summary>
        /// <param name="batchId">ID of batch to get status of.</param>
        /// <returns>Object containing batch status information.</returns>
        public static string GetBatchErrorText(int scheduleID)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetBatchErrorText(scheduleID);
            }

            return null;
        }

        /// <summary>
        /// Returns an array of bounced emails
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public static string[] GetBouncedEmails(int scheduleID)
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetBouncedEmails(scheduleID);
            }

            return null;
        }

        /// <summary>
        /// Returns an array of bounced emails
        /// </summary>
        /// <returns></returns>
        public static string[] GetBouncedEmails()
        {
            if (ApplicationManager.AppSettings.EmailEnabled)
            {
                return EmailFactory.GetEmailProvider().GetBouncedEmails();
            }

            return null;
        }

        #endregion
    }
}
