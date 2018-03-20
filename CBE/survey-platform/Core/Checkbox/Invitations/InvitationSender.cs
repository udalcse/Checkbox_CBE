using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Checkbox.Security;
using Checkbox.Messaging.Email;
using Checkbox.Management;
using Prezza.Framework.Data;
using Checkbox.Forms;
using Checkbox.Security.Principal;
using Checkbox.SystemMode;
using Checkbox.Users;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Sends an invitation using the Email Gateway
    /// 
    /// Contains cross-context logic to handle the postponed sending
    /// </summary>
    public static class InvitationSender
    {
        #region Sending
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <param name="batchSize"></param>
        /// <exception cref="Exception"></exception>
        public static int RequestBatchMessages(int scheduleID, int? batchSize)
        {
            InvitationSchedule schedule = new InvitationSchedule();
            schedule.InvitationScheduleID = scheduleID;
            schedule.Load();
            schedule.ProcessingStarted = DateTime.Now;
            schedule.Save(null);
            if (schedule.InvitationID == null)
                throw new Exception("Invitation ID is NULL");
            Invitation invitation = InvitationManager.GetInvitation(schedule.InvitationID.Value);
            return Send(invitation, schedule, null, null, batchSize);
        }
        /// <summary>
        /// Type for the callback for message sending notification
        /// </summary>
        /// <param name="r"></param>
        /// <param name="number"></param>
        /// <param name="count"></param>
        public delegate void MessageSentDelegate(Recipient r, int number, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitation"></param>
        /// <param name="scheduleRecord"></param>
        /// <param name="recipientsToHandleCount"></param>
        public static void HandleRecipients(Invitation invitation, InvitationSchedule scheduleRecord, int? recipientsToHandleCount)
        {
            var recipientList = GetRecipients(invitation, scheduleRecord.InvitationActivityType);
        }

        /// <summary>
        /// Sends a scheduled invitation
        /// </summary>
        /// <param name="invitation"></param>
        /// <param name="scheduleRecord"></param>
        /// <param name="sentCallback"></param>
        /// <param name="context"> </param>
        /// <param name="batchSize"> </param>
        public static int Send(Invitation invitation, InvitationSchedule scheduleRecord, MessageSentDelegate sentCallback, string context, int? batchSize = null)
        {
            int processedInvitationCount = 0;

            //Test message should be already created
            if (scheduleRecord.InvitationActivityType == InvitationActivityType.Test)
                return processedInvitationCount;

            var customUserFieldNames = ProfileManager.ListPropertyNames();

            Guid? surveyGuid = ResponseTemplateManager.GetResponseTemplateGUID(invitation.ParentID);
            var baseSurveyUrl = InvitationPipeMediator.GetBaseSurveyUrl(surveyGuid);

            long? messageBatchId = scheduleRecord.BatchID;

            //preload recipients if we need not pending
            Dictionary<long, Recipient> preparedRecipientsData = null;
            if (scheduleRecord.InvitationActivityType != InvitationActivityType.Invitation)
                preparedRecipientsData = invitation.LoadRecipientData(scheduleRecord.BatchID, batchSize);

            //Figure out recipient list and message text
            if (!messageBatchId.HasValue && EmailGateway.ProviderSupportsBatches)
            {
                //initialize credentials for callbacks from the Messaging Service
                EnsureMessagingServiceCredentialsInitialized();

                //Create an email batch
                messageBatchId = InvitationManager.CreateInvitationEmailBatch(
                    invitation.ID.Value,
                    scheduleRecord.InvitationActivityType,
                    invitation.GetPendingRecipientsCount(),
                    scheduleRecord.Creator,
                    scheduleRecord.Scheduled,
                    scheduleRecord.InvitationScheduleID.Value);
                scheduleRecord.BatchID = messageBatchId;
                scheduleRecord.Save(null);

                if (scheduleRecord.Scheduled > DateTime.Now)
                    return processedInvitationCount;

                scheduleRecord.ProcessingStarted = DateTime.Now;
                scheduleRecord.Save(null);
            }

            var recipientList = new List<Recipient>();
            var recipients = GetRecipients(invitation, scheduleRecord.InvitationActivityType, batchSize,
                preparedRecipientsData);

            foreach (var recipient in recipients)
            {
                if (recipientList.Any(x => x.UniqueIdentifier == recipient.UniqueIdentifier))
                {
                    recipientList.First(x => x.UniqueIdentifier == recipient.UniqueIdentifier)
                        .EmailList.Add(recipient.EmailToAddress);
                }
                else
                {
                    recipient.EmailList = new List<string> { recipient.EmailToAddress };
                    recipientList.Add(recipient);
                }
            }

            //mark recipient that were filtered out to sort them out from the next batch
            if (messageBatchId != null && preparedRecipientsData != null)
            {
                var filteredIDs = (from r in recipientList where r.ID.HasValue select r.ID).ToList();
                if (filteredIDs.Any())
                {
                    var toBeMarkedAsProcessed = string.Join(",", (from r in preparedRecipientsData where !filteredIDs.Contains(r.Key) select r.Key.ToString()).ToArray());

                    InvitationManager.MarkRecipientAsProcessed(toBeMarkedAsProcessed, messageBatchId.Value);
                }
            }

            //save starting time
            if (!EmailGateway.ProviderSupportsBatches)
            {
                scheduleRecord.ProcessingStarted = DateTime.Now;
                scheduleRecord.Save(null);
            }



            if (ApplicationManager.AppSettings.IsPrepMode)
            {
                SendEmailsToPrepModeUsers(recipientList, baseSurveyUrl, invitation, scheduleRecord, customUserFieldNames,
                    messageBatchId, surveyGuid, sentCallback);
            }
            else
            {
                SendEmailsToRecipients(recipientList, baseSurveyUrl, invitation, scheduleRecord, customUserFieldNames, messageBatchId, surveyGuid, sentCallback);
            }
            

            if (batchSize.HasValue)
            {
                int processedRecipientsCount = preparedRecipientsData != null
                                                  ? preparedRecipientsData.Count
                                                  : recipientList.Count;

                bool allRecipientsAreProcessed = batchSize.Value > processedRecipientsCount;
            
                //Mark the batch as ready
                if (messageBatchId.HasValue)
                {
                    processedInvitationCount = invitation.GetProcessedRecipientsCount(messageBatchId.Value);

                    if (allRecipientsAreProcessed)
                    {
                        InvitationManager.CloseInvitationEmailBatch(messageBatchId.Value);

                        //Mark recipients as sent only if batch successfully marked as ready
                        InvitationManager.SetSuccessfulSentStatusForRecipients(invitation.ID.Value);

                        //mark invitation as sent 
                        if (processedInvitationCount == 0)
                        {
                            scheduleRecord.ProcessingFinished = DateTime.Now;
                            scheduleRecord.ErrorMessage = "No recipients has been found.";
                            scheduleRecord.Save(null);
                        }
                        InvitationManager.UpdateInvitationSentDate(invitation.ID.Value);
                    }
                }
            }

            //Mark invitation as sent
            if (!EmailGateway.ProviderSupportsBatches)
            {
                scheduleRecord.ProcessingFinished = DateTime.Now;
                scheduleRecord.Save(null);
                InvitationManager.UpdateInvitationSentDate(invitation.ID.Value);
            }

            return processedInvitationCount;
        }

        private static void SendEmailsToPrepModeUsers(List<Recipient> recipientList, string baseSurveyUrl,
            Invitation invitation, InvitationSchedule scheduleRecord, List<string> customUserFieldNames,
            long? messageBatchId, Guid? surveyGuid, MessageSentDelegate sentCallback)
        {

            var prepModeUsesGuids = SystemModeManager.GetPrepModeUserGuids();
            List<CheckboxPrincipal> prepModeUsers =
                prepModeUsesGuids.Select(UserManager.GetUserByGuid)
                    .Where(item => !string.IsNullOrEmpty(item.Email))
                    .ToList();

            foreach (var user in prepModeUsers)
              SendInvitations(recipientList,baseSurveyUrl,invitation,scheduleRecord,customUserFieldNames,messageBatchId,surveyGuid,sentCallback, user.Email);
        }


        private static void SendEmailsToRecipients(List<Recipient> recipientList, string baseSurveyUrl,Invitation invitation, InvitationSchedule scheduleRecord, List<string> customUserFieldNames, long? messageBatchId, Guid? surveyGuid, MessageSentDelegate sentCallback)
        {
            SendInvitations(recipientList, baseSurveyUrl, invitation, scheduleRecord, customUserFieldNames, messageBatchId, surveyGuid, sentCallback, string.Empty);
        }

        private static void SendInvitations(List<Recipient> recipientList, string baseSurveyUrl,
           Invitation invitation, InvitationSchedule scheduleRecord, List<string> customUserFieldNames,
           long? messageBatchId, Guid? surveyGuid, MessageSentDelegate sentCallback, string emailSubstitution)
        {
            for (int recipientNumber = 0; recipientNumber < recipientList.Count; recipientNumber++)
            {
                Recipient recipient = recipientList[recipientNumber];

                recipient.ProcessingBatchId = scheduleRecord.BatchID;

                if (recipient.OptedOut)
                {
                    recipient.SuccessfullySent = false;
                    recipient.Save();
                    continue;
                }

                if (messageBatchId.HasValue)
                {
                    bool createMessage = true;
                    if (recipient.BatchMessageId.HasValue)
                    {
                        //if this recipient already had the message, which was created within current batch, do not create a duplicate,
                        var messageStatus = EmailGateway.GetMessageStatus(recipient.BatchMessageId.Value);
                        if (messageStatus != null)
                        {
                            createMessage = messageStatus.BatchId != messageBatchId.Value;
                        }
                    }

                    if (createMessage)
                    {
                        foreach (var email in recipient.EmailList)
                        {
                            recipient.EmailToAddress = email;

                            EmailMessage message = CreateMessage(invitation, recipient, customUserFieldNames,
                                baseSurveyUrl, surveyGuid,
                                scheduleRecord.InvitationActivityType);

                            //Do not try/catch here since queuing failure is critical
                            recipient.BatchMessageId = EmailGateway.AddEmailMessageToBatch(messageBatchId.Value, message);
                        }
                    }
                    recipient.Save();
                }
                else
                {
                    //Try catch so that errors can be reported
                    try
                    {
                        foreach (var email in recipient.EmailList)
                        {
                            recipient.EmailToAddress = email;
                            EmailMessage message = CreateMessage(invitation, recipient, customUserFieldNames,
                                baseSurveyUrl, surveyGuid, scheduleRecord.InvitationActivityType);

                            if (!string.IsNullOrWhiteSpace(emailSubstitution))
                                message.To = emailSubstitution;

                            EmailGateway.Send(message);
                        }

                        //Save the recipient send information
                        recipient.LastSent = DateTime.Now;
                        recipient.SuccessfullySent = true;
                        recipient.Save();
                    }
                    catch (Exception ex)
                    {
                        //Save the recipient send information
                        recipient.LastSent = DateTime.Now;
                        recipient.SuccessfullySent = false;
                        recipient.Error = ex.Message;
                        recipient.Save();
                    }
                }

                if (sentCallback != null)
                    sentCallback(recipient, recipientNumber, recipientList.Count);
            }
        }

        private static EmailMessage CreateMessage(Invitation invitation, Recipient recipient,
            List<string> customUserFieldNames, string baseSurveyUrl, Guid? surveyGuid, InvitationActivityType type)
        {
            InvitationTemplate template = invitation.Template.Copy();
            recipient.PersonalizeTemplate(invitation, template, customUserFieldNames, baseSurveyUrl, surveyGuid);

            return CreateEmailMessage(recipient, template, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <param name="Status"></param>
        /// <param name="ErrorText"></param>
        /// <exception cref="Exception"></exception>
        public static void UpdateBatchStatus(int scheduleID, string Status, string ErrorText)
        {
            InvitationSchedule schedule = new InvitationSchedule();
            schedule.InvitationScheduleID = scheduleID;
            schedule.Load();
            if (schedule.InvitationID == null)
                throw new Exception("Invitation ID is NULL");
            schedule.ProcessingFinished = DateTime.Now;
            schedule.ErrorMessage = string.Format("{0}. {1}", Status, ErrorText);
            schedule.Save(null);
        }

        #endregion Sending

        #region Security
        static object _lockObj = new object();
        /// <summary>
        /// Checks and initializes the user name and the password for the messaging service
        /// </summary>
        public static void EnsureMessagingServiceCredentialsInitialized()
        {
            lock (_lockObj)
            {
                if (string.IsNullOrEmpty(ApplicationManager.AppSettings.MessagingServiceUserName))
                {
                    ApplicationManager.AppSettings.MessagingServiceUserName = string.Format("cmsusr{0}", Guid.NewGuid().ToString().Substring(0, 5));
                }
                if (string.IsNullOrEmpty(ApplicationManager.AppSettings.MessagingServicePassword))
                {
                    ApplicationManager.AppSettings.MessagingServicePassword = Guid.NewGuid().ToString();
                }
            }
        }
        #endregion Security

        #region Private Methods
        /// <summary>
        /// Returns recipient list by the sending mode
        /// </summary>
        private static List<Recipient> GetRecipients(Invitation invitation, InvitationActivityType mode,
            int? batchSize = null, Dictionary<long, Recipient> preparedRecipientsData = null)
        {
            switch (mode)
            {
                case InvitationActivityType.Invitation :
                    return invitation.GetRecipients(RecipientFilter.Pending, false, ApplicationManager.AppSettings.EnableOptOutScreen, batchSize, preparedRecipientsData);
                case InvitationActivityType.Reminder:
                    return invitation.GetRecipients(RecipientFilter.NotResponded, false, ApplicationManager.AppSettings.EnableOptOutScreen, batchSize, preparedRecipientsData);
                case InvitationActivityType.Test:
                case InvitationActivityType.Resend:
                    return invitation.GetRecipients(RecipientFilter.All, false, ApplicationManager.AppSettings.EnableOptOutScreen, batchSize, preparedRecipientsData);
            }
            //Return no recipients if no mode specified
            return new List<Recipient>(new List<Recipient>());
        }

        /// <summary>
        /// Creates an EmailMessage for sending to a Recipient
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static EmailMessage CreateEmailMessage(Recipient recipient, InvitationTemplate template, InvitationActivityType mode)
        {
            var msg = new EmailMessage
            {
                To = recipient.EmailToAddress,
                From = template.FromName + "<" + template.FromAddress + ">",
                Subject = mode == InvitationActivityType.Reminder ? template.ReminderSubject : template.Subject,
                Format = template.Format,
                Body = mode == InvitationActivityType.Reminder ? template.ReminderBody : template.Body
            };
            if (template.Format == MailFormat.Html)
            {
                msg.Body = FixIssueWithOutlook(msg.Body);
                msg.Body = FixRelativeUrls(msg.Body);
            }
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        private static string FixRelativeUrls(string messageBody)
        {
            if (string.IsNullOrEmpty(messageBody))
                return string.Empty;

            const string relative = "../";

            int index = messageBody.IndexOf(relative, 0);

            if (index >= 0)
            {
                //lets find open quotes
                int end = index;
                do
                {
                    end += relative.Length;
                } while (end < messageBody.Length && messageBody.Substring(end, relative.Length) == relative);

                if (end < messageBody.Length)
                {
                    string relativePath = messageBody.Substring(index, end - index);

                    messageBody = messageBody.Replace(relativePath, ApplicationManager.ApplicationPath + "/");
                    messageBody = messageBody.Replace(relative, string.Empty);
                }
            }
            
            return messageBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        private static string FixIssueWithOutlook(string messageBody)
        {
            int index = 0;
            const string textAlign = "text-align: ";
            while ((index = messageBody.IndexOf(textAlign, index)) != -1)
            {
                //parse alignment direction
                int directionStartIndex = index + textAlign.Length;
                int directionEndIndex = messageBody.IndexOfAny(new[] { '"', ';', ' ' }, directionStartIndex);
                string direction = messageBody.Substring(directionStartIndex, directionEndIndex - directionStartIndex);

                //inline style closing tag
                int tagClosingQuotesInd = messageBody.IndexOf('"', index);
                if (tagClosingQuotesInd != -1 && (direction == "right" || direction == "left" || direction == "center"))
                {
                    //add html alignment tag
                    string align = " align=\"" + direction + "\"";
                    messageBody = messageBody.Insert(tagClosingQuotesInd + 1, align);

                    //remove css inline style
                    messageBody = messageBody.Replace(textAlign + direction, string.Empty);

                    index += align.Length;
                }
                else
                    index++;
            }

            index = 0;
            //replace automargin with center tag
            const string autoMargin = "margin-left: auto; margin-right: auto";
            while ((index = messageBody.IndexOf(autoMargin, index)) != -1)
            {
                //inline style closing tag
                int parentTagInd, lastInd = 0;
                do
                {
                    parentTagInd = lastInd;
                    lastInd = messageBody.IndexOf("<p", lastInd + 1);

                } while (lastInd != -1 && lastInd < index);

                //doesn't work
                //parentTagInd = messageBody.LastIndexOf("<p", 0, index - 1); 

                if (parentTagInd != -1)
                {
                    //add html alignment tag
                    const string align = " align=\"center\"";
                    messageBody = messageBody.Insert(parentTagInd + 2, align);

                    //remove css inline style
                    messageBody = messageBody.Replace(autoMargin, string.Empty);

                    index += align.Length;
                }
                else
                    index++;
            }

            index = 0;
            //replace float with tag
            const string floating = "float: ";
            while ((index = messageBody.IndexOf(floating, index)) != -1)
            {
                //parse alignment direction
                int directionStartIndex = index + floating.Length;
                int directionEndIndex = messageBody.IndexOfAny(new[] { '"', ';', ' ' }, directionStartIndex);
                string direction = messageBody.Substring(directionStartIndex, directionEndIndex - directionStartIndex);

                //inline style closing tag
                int parentTagInd, lastInd = 0;
                do
                {
                    parentTagInd = lastInd;
                    lastInd = messageBody.IndexOf("<p", lastInd + 1);

                } while (lastInd != -1 && lastInd < index);

                //doesn't work
                //parentTagInd = messageBody.LastIndexOf("<p", 0, index - 1); 

                if (parentTagInd != -1 && (direction == "right" || direction == "left"))
                {
                    //add html alignment tag
                    string align = " align=\"" + direction + "\"";
                    messageBody = messageBody.Insert(parentTagInd + 2, align);

                    //remove css inline style
                    messageBody = messageBody.Replace(floating + direction, string.Empty);

                    index += align.Length;
                }
                else
                    index++;
            }

            const string bodyFormat = "<div style=\"display: inline-block;\">{0}</div>";
            return string.Format(bodyFormat, messageBody);
        }

        #endregion Private Methods
    }
}
