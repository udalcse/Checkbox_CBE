using System;
using System.Collections.Generic;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Interface definition for email messages distributed by Checkbox.
    /// </summary>
    public interface IEmailMessage
    {
        /// <summary>
        /// Get the email message "From" address.
        /// </summary>
        string From { get; }

        /// <summary>
        /// Get the address of the email recipient.
        /// </summary>
        string To { get; }

        /// <summary>
        /// Get the email message subject
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// Get the email message body.
        /// </summary>
        string Body { get; }

        /// <summary>
        /// Get a boolean indicating if the message body contains html
        /// </summary>
        bool IsBodyHtml { get; }

        /// <summary>
        /// Get a list of attachments for the email
        /// </summary>
        List<IEmailAttachment> Attachments { get; }

        /// <summary>
        /// For sake of efficiency, also support adding attachments by "reference" 
        /// with attachment id.  This feature is only used by batch email providers.
        /// </summary>
        List<long> AttachmentsByRef { get; }
    }
}
