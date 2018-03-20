//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;

using Checkbox.Management;
using Checkbox.Messaging.Email;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Template for invitations
    /// </summary>
    [Serializable]
    public class InvitationTemplate
    {
        ///<summary>
        ///</summary>
        public InvitationTemplate()
        {
            FromAddress = ApplicationManager.AppSettings.SystemEmailAddress;
            FromName = ApplicationManager.AppSettings.DefaultEmailFromName;
            Body = ApplicationManager.AppSettings.DefaultEmailInvitationBody;
            Subject = ApplicationManager.AppSettings.DefaultEmailInvitationSubject;
            ReminderBody = Body;
            ReminderSubject = Subject;
        }

        /// <summary>
        /// Gets or sets the email subject for the Invitation
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email body for the Invitation
        /// </summary>
        public string Body { get; set; }

        ///<summary>
        ///</summary>
        public string ReminderSubject { get; set; }

        ///<summary>
        ///</summary>
        public string ReminderBody { get; set; }

        /// <summary>
        /// Get/set whether the invitation should include an opt-out link
        /// </summary>
        public bool IncludeOptOutLink { get; set; }

        /// <summary>
        /// Get/set the opt out text
        /// </summary>
        public string OptOutText { get; set; }

        /// <summary>
        /// Get the opt out URL
        /// </summary>
        public string OptOutURL
        {
            get { return string.Format("{0}{1}/OptOut.aspx?i={2}", 
                ApplicationManager.ApplicationURL, 
                ApplicationManager.ApplicationRoot, 
                ApplicationManager.AppSettings.PipePrefix); }
        }

        /// <summary>
        /// Gets or sets the email From Address for the Invitation
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the email From Name for the Invitation
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MailFormat"/> for the Invitation
        /// </summary>
        public MailFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the text of the Form url as it will appear in the body of the email
        /// </summary>
        public string LinkText { get; set; }

        ///<summary>
        ///</summary>
        public LoginOption LoginOption { get; set; }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public InvitationTemplate Copy()
        {
            var template = new InvitationTemplate {
                FromAddress = FromAddress, 
                FromName = FromName, 
                Subject = Subject, 
                Body = Body, 
                LinkText = LinkText, 
                LoginOption = LoginOption, 
                Format = Format, 
                IncludeOptOutLink = IncludeOptOutLink, 
                ReminderBody =  ReminderBody,
                ReminderSubject =  ReminderSubject,
                OptOutText = OptOutText};

            return template;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateReminder()
        {
            bool updated = false;
            if (string.IsNullOrEmpty(ReminderSubject))
            {
                updated = true;
                ReminderSubject = Subject;
            }

            if (string.IsNullOrEmpty(ReminderBody))
            {
                updated = true;
                ReminderBody = Body;
            }

            return updated;
        }
    }
}