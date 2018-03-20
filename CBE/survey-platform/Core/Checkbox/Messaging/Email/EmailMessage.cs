using System;
using System.Collections.Generic;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Handles operations relating to the sending of Email through the application
    /// </summary>
    public class EmailMessage : IEmailMessage
    {
        private List<IEmailAttachment> _attachments;
        private List<long> _attachmentsByRef;

        #region Properties

        /// <summary>
        /// Gets and sets the From address for this EmailMessage
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets and sets the To address for this EmailMessage
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets and sets the Subject for this EmailMessage
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets and sets the Body for this EmailMessage
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="MailFormat"/> for this EmailMessage
        /// </summary>
        public MailFormat Format { get; set; }

        /// <summary>
        /// Return a boolean indicating if the body is html
        /// </summary>
        public bool IsBodyHtml
        {
            get
            {
                return Format == MailFormat.Html;
            }
        }

        /// <summary>
        /// Get the email message attachments
        /// </summary>
        public List<IEmailAttachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<IEmailAttachment>();
                }

                return _attachments;
            }
        }

        /// <summary>
        /// Message reference attachments
        /// </summary>
        public List<long> AttachmentsByRef
        {
            get
            {
                if (_attachmentsByRef == null)
                {
                    _attachmentsByRef = new List<long>();
                }

                return _attachmentsByRef;
            }
        }

        #endregion

    }

    /// <summary>
    /// Email message format.
    /// </summary>
    public enum MailFormat
    {
        /// <summary>
        /// HTML
        /// </summary>
        Html,

        /// <summary>
        /// Text
        /// </summary>
        Text
    }
}
