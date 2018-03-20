using System;
using System.IO;
using System.Data;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using System.Collections.Generic;
using Checkbox.Management;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Custom email provider that operates by placing sent mail data into a database for later retrieval and 
    /// sending by an external process.
    /// </summary>
    /// <remarks>Uses the default or master database depending on whether multi db mode is enabled.</remarks>
    public class DatabaseRelayEmailProvider : BaseEmailProvider
    {
        private string _mailDbInstanceName;

        /// <summary>
        /// Provider supports batches, so always return true
        /// </summary>
        public override bool SupportsBatches { get { return true; } }

        /// <summary>
        /// Send a message outside of a batch
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override long? DoSendMessage(IEmailMessage message)
        {
            return DoSendMessage(
                "Ad Hoc Email Message " + DateTime.Now,
                "DatabaseRelayEmailProvider",
                message,
                null);
        }

        /// <summary>
        /// Send a message outside of a batch.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="message"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        protected override long DoSendMessage(string name, string createdBy, IEmailMessage message, DateTime? earliestSendDate)
        {
            long batchId = CreateEmailMessageBatch(
               "Ad Hoc Message Batch " + DateTime.Now,
               "DatabaseRelayEmailProvider",
               earliestSendDate,
               0);

            if (batchId <= 0)
            {
                throw new Exception("Unable to send email:  Create batch returned an invalid batch id [" + batchId + "].");
            }

            //Queue the message
            long messageId = QueueEmailMessage(batchId, message);

            //Close the batch
            DoCloseEmailMessageBatch(batchId);

            //Return message id
            return messageId;
        }

        /// <summary>
        /// Initialize the provider with its connection string
        /// </summary>
        /// <param name="config"></param>
        protected override void DoInitialize(ConfigurationBase config)
        {
            //Validate config type
            ArgumentValidation.CheckExpectedType(config, typeof(DatabaseRelayEmailProviderData));

            _mailDbInstanceName = ((DatabaseRelayEmailProviderData)config).MailDbInstanceName;
        }

        /// <summary>
        /// Returns recent status of the batch
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public override string GetBatchErrorText(int scheduleID)
        {
            try
            {
                Database db = GetDatabaseProvider();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetBatchErrorText");
                command.AddInParameter("ContextName", DbType.String, ApplicationManager.ApplicationDataContext);
                command.AddInParameter("ScheduleId", DbType.Int32, scheduleID);

                //Execute command and get batch id
                var reader = db.ExecuteReader(command);

                try
                {
                    if (reader.Read())
                    {
                        return reader[0] is DBNull ? null : (string)reader[0];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPrivate");
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns an array of bounced emails
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public override string[] GetBouncedEmails(int scheduleID)
        {
            try
            {
                Database db = GetDatabaseProvider();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetBatchBouncedEmails");
                command.AddInParameter("ContextName", DbType.String, ApplicationManager.ApplicationDataContext);
                command.AddInParameter("ScheduleId", DbType.Int32, scheduleID);

                //Execute command and get batch id
                var reader = db.ExecuteReader(command);
                var res = new List<string>();

                while (reader.Read())
                {
                    res.Add((string)reader[0]);
                }

                reader.Close();

                return res.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPrivate");
            }
            return new string[]{};
        }

        public override string[] GetBouncedEmails()
        {
            try
            {
                Database db = GetDatabaseProvider();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetAllBouncedEmails");

                //Execute command and get batch id
                var reader = db.ExecuteReader(command);
                var res = new List<string>();

                while (reader.Read())
                {
                    res.Add((string)reader[0]);
                }

                reader.Close();

                return res.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPrivate");
            }
            return new string[] { };
        }

        /// <summary>
        /// Create an email message batch
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        protected override long DoCreateEmailMessageBatch(string name, string createdBy, DateTime? earliestSendDate, int scheduleID)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_CreateBatch");
            command.AddInParameter("BatchName", DbType.String, name);
            command.AddInParameter("CreatedBy", DbType.String, createdBy);
            command.AddInParameter("EarliestSendDate", DbType.DateTime, earliestSendDate);
            command.AddInParameter("ContextName", DbType.String, ApplicationManager.ApplicationDataContext);
            command.AddInParameter("WebServiceURL", DbType.String, ApplicationManager.ApplicationPath);
            command.AddInParameter("WebServiceUser", DbType.String, ApplicationManager.AppSettings.MessagingServiceUserName);
            command.AddInParameter("WebServiceKey", DbType.String, ApplicationManager.AppSettings.MessagingServicePassword);
            command.AddInParameter("ScheduleId", DbType.Int32, scheduleID);
            command.AddInParameter("Status", DbType.String, "Postponed"); 
            command.AddOutParameter("BatchId", DbType.Int64, 8);

            //Execute command and get batch id
            db.ExecuteNonQuery(command);

            object val = command.GetParameterValue("BatchId");

            if (val != null && val != DBNull.Value)
            {
                return (long)val;
            }

            return -1;
        }

        /// <summary>
        /// Mark a batch as closed
        /// </summary>
        /// <param name="batchId"></param>
        protected override void DoCloseEmailMessageBatch(long batchId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_CloseBatch");
            command.AddInParameter("BatchId", DbType.Int64, batchId);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Add an email message to a batch.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override long DoAddEmailMessageToBatch(long batchId, IEmailMessage message)
        {
            return QueueEmailMessage(batchId, message);
        }

        /// <summary>
        /// Register an email attachment.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        protected override long DoRegisterEmailAttachment(IEmailAttachment attachment)
        {
            return RegisterEmailMessageAttachment(null, attachment, null);
        }

        /// <summary>
        /// Delete an email message
        /// </summary>
        /// <param name="messageId"></param>
        protected override void DoDeleteEmailMessage(long messageId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_DeleteMessage");
            command.AddInParameter("MessageId", DbType.Int64, messageId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete a message batch
        /// </summary>
        /// <param name="batchId"></param>
        public override void DoDeleteEmailMessageBatch(long batchId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_DeleteMessageBatch");
            command.AddInParameter("BatchId", DbType.Int64, batchId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete a message attachment
        /// </summary>
        /// <param name="attachmentId"></param>
        protected override void DoDeleteMessageAttachment(long attachmentId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_DeleteMessageAttachment");
            command.AddInParameter("AttachmentId", DbType.Int64, attachmentId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Get the status of an email message batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        protected override IEmailMessageBatchStatus DoGetEmailMessageBatchStatus(long batchId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetBatchStatus");

            EmailMessageBatchStatus status = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        status = new EmailMessageBatchStatus
                        {
                            CurrentStatus = ((BatchStatus)Enum.Parse(typeof(BatchStatus), DbUtility.GetValueFromDataReader(reader, "BatchStatus", BatchStatus.Unknown.ToString()))),
                            BatchSendStarted = DbUtility.GetValueFromDataReader<DateTime?>(reader, "BatchSendStarted", null),
                            BatchSendCompleted = DbUtility.GetValueFromDataReader<DateTime?>(reader, "BatchSendCompleted", null),
                            NextSendAttempt = DbUtility.GetValueFromDataReader<DateTime?>(reader, "NextSendAttempt", null),
                            SuccessfullySent = DbUtility.GetValueFromDataReader(reader, "SuccessfullySent", false),
                            AttemptedCount = DbUtility.GetValueFromDataReader(reader, "AttemptedCount", 0),
                            SucceededCount = DbUtility.GetValueFromDataReader(reader, "SucceededCount", 0),
                            FailedCount = DbUtility.GetValueFromDataReader(reader, "FailedCount", 0)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //If the batch has failures, get more information
            if (status != null
                && status.FailedCount > 0)
            {
                List<long> failureIds = new List<long>();

                DBCommandWrapper comand = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetBatchFailures");
                comand.AddInParameter("BatchId", DbType.Int64, batchId);

                using (IDataReader reader = db.ExecuteReader(comand))
                {
                    while (reader.Read())
                    {
                        long failureId = DbUtility.GetValueFromDataReader(reader, "MessageId", (long)-1);

                        if (failureId > 0)
                        {
                            failureIds.Add(failureId);
                        }
                    }
                }

                status.FailedMessages = failureIds;
            }

            return status;
        }

        /// <summary>
        /// Get the status of an email message
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        protected override IEmailMessageStatus DoGetEmailMessageStatus(long emailId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetMessageStatus");
            command.AddInParameter("MessageId", DbType.Int64, emailId);

            EmailMessageStatus status = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        status = new EmailMessageStatus
                        {
                            LastSendAttempt = DbUtility.GetValueFromDataReader<DateTime?>(reader, "LastSendAttempt", null),
                            NextSendAttempt = DbUtility.GetValueFromDataReader<DateTime?>(reader, "NextSendAttempt", null),
                            SuccessfullySent = DbUtility.GetValueFromDataReader(reader, "SuccessfullySent", false),
                            LastSendError = DbUtility.GetValueFromDataReader(reader, "LastSendError", string.Empty),
                            QueuedDate = DbUtility.GetValueFromDataReader<DateTime?>(reader, "DateCreated", null),
                            BatchId = DbUtility.GetValueFromDataReader(reader, "BatchId", 0)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return status;
        }

        /// <summary>
        /// Get data for a particular email message.
        /// </summary>
        /// <param name="messageId">Id of message to get.</param>
        /// <returns></returns>
        protected override IEmailMessage DoGetMessage(long messageId)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_GetMessage");
            command.AddInParameter("MessageId", DbType.Int64, messageId);

            EmailMessage message = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        //Build message object
                        message = new EmailMessage
                        {
                            From = DbUtility.GetValueFromDataReader(reader, "FromEmail", string.Empty),
                            To = DbUtility.GetValueFromDataReader(reader, "ToEmail", string.Empty),
                            Body = DbUtility.GetValueFromDataReader(reader, "MessageBody", string.Empty),
                            Subject = DbUtility.GetValueFromDataReader(reader, "MessageSubject", string.Empty),
                            Format = (DbUtility.GetValueFromDataReader(reader, "MessageIsHtml", false) ? MailFormat.Html : MailFormat.Text)
                        };

                        //Get attachments, if any
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                EmailAttachment attachment = new EmailAttachment
                                {
                                    MimeContentTypeString = DbUtility.GetValueFromDataReader(reader, "MimeContentType", string.Empty),
                                    FileName = DbUtility.GetValueFromDataReader(reader, "AttachmentName", string.Empty)
                                };

                                message.Attachments.Add(attachment);
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return message;
        }



        #region Work Methods

        /// <summary>
        /// Get a reference to a database provider for the mail queue.
        /// </summary>
        /// <returns></returns>
        private Database GetDatabaseProvider()
        {
            return DatabaseFactory.CreateDatabase(_mailDbInstanceName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="message"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private long QueueEmailMessage(long? batchId, IEmailMessage message, IDbTransaction transaction)
        {
            Database db = GetDatabaseProvider();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_InsertMessage");

            command.AddInParameter("BatchId", DbType.Int64, batchId);
            command.AddInParameter("FromName", DbType.String, message.From);
            command.AddInParameter("FromEmail", DbType.String, message.From);
            command.AddInParameter("ToEmail", DbType.String, message.To);
            command.AddInParameter("MessageSubject", DbType.String, message.Subject);
            command.AddInParameter("MessageBody", DbType.String, message.Body);
            command.AddInParameter("MessageIsHTML", DbType.Boolean, message.IsBodyHtml);
            command.AddOutParameter("MessageId", DbType.Int64, 8);

            //Execute command and get message id
            db.ExecuteNonQuery(command, transaction);

            object val = command.GetParameterValue("MessageId");

            if (val != null && val != DBNull.Value)
            {
                return (long)val;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="attachments"></param>
        /// <param name="transaction"></param>
        private void QueueEmailMessageAttachments(long messageId, List<long> attachments, IDbTransaction transaction)
        {
            foreach (long attachmentId in attachments)
            {
                AddAttachmentToMessage(messageId, attachmentId, transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="attachments"></param>
        /// <param name="transaction"></param>
        private void QueueEmailMessageAttachments(long messageId, List<IEmailAttachment> attachments, IDbTransaction transaction)
        {
            //Register attachments on the email itself
            foreach (IEmailAttachment attachment in attachments)
            {
                long attachmentId = RegisterEmailMessageAttachment(messageId, attachment, transaction);

                if (attachmentId > 0)
                {
                    AddAttachmentToMessage(messageId, attachmentId, transaction);
                }
            }
        }

        /// <summary>
        /// Add a reference to an attachment to a message.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="attachmentId"></param>
        /// <param name="transaction"></param>
        private void AddAttachmentToMessage(long messageId, long attachmentId, IDbTransaction transaction)
        {
            Database db = GetDatabaseProvider();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_AddAttachmentToMessage");
            command.AddInParameter("MessageId", DbType.Int64, messageId);
            command.AddInParameter("AttachmentId", DbType.Int64, attachmentId);

            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Store an email attachment and return its database id
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="attachment"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private long RegisterEmailMessageAttachment(long? messageId, IEmailAttachment attachment, IDbTransaction transaction)
        {
            MemoryStream ms = attachment.GetContentStream() as MemoryStream;

            if (ms != null)
            {
                Database db = GetDatabaseProvider();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_RegisterMessageAttachment");
                command.AddInParameter("AttachmentName", DbType.String, attachment.FileName);
                command.AddInParameter("MimeContentType", DbType.String, attachment.MimeContentTypeString);
                command.AddInParameter("AttachmentBytes", DbType.Binary, ms.ToArray());
                command.AddInParameter("MessageId", DbType.Int64, messageId);
                command.AddOutParameter("AttachmentId", DbType.Int64, 8);

                if (transaction != null)
                {
                    db.ExecuteNonQuery(command, transaction);
                }
                else
                {
                    db.ExecuteNonQuery(command);
                }

                //Get id
                object val = command.GetParameterValue("AttachmentId");

                if (val != null && val != DBNull.Value)
                {
                    return (long)val;
                }
            }

            return -1;
        }

        /// <summary>
        /// Queue an email message for sending
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private long QueueEmailMessage(long? batchId, IEmailMessage message)
        {
            Database db = GetDatabaseProvider();
            long emailMessageId = -1;

            //Open a connection
            using (IDbConnection connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    //Create a transaction
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //Queue the message
                            emailMessageId = QueueEmailMessage(batchId, message, transaction);

                            //Queue attachments that are phsyically on the message
                            QueueEmailMessageAttachments(emailMessageId, message.Attachments, transaction);

                            //Add "by ref" attachments
                            QueueEmailMessageAttachments(emailMessageId, message.AttachmentsByRef, transaction);

                            //Commit the transaction
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return emailMessageId;
        }

        /// <summary>
        /// Changes the scheduled date for the batch
        /// </summary>
        /// <param name="batchId">Id of the batch</param>
        /// <param name="scheduledDate">New date when the batch should be sent</param>
        public override void SetMessageBatchDate(long batchId, DateTime scheduledDate)
        {
            Database db = GetDatabaseProvider();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_DatabaseRelayEmail_SetScheduledDate");
            command.AddInParameter("BatchId", DbType.Int64, batchId);
            command.AddInParameter("EarliestSendDate", DbType.DateTime, scheduledDate);

            db.ExecuteNonQuery(command);
        }

        #endregion
    }
}
