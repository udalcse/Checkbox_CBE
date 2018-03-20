using System;
using System.IO;
using System.Net.Mime;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Interface definiation for email attachments for the Checkbox
    /// email gateway.
    /// </summary>
    public interface IEmailAttachment
    {
        /// <summary>
        /// Attachment file name
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Get the email attachment mime content type
        /// </summary>
        string MimeContentTypeString { get; set; }

        /// <summary>
        /// Get a stream for reading the attachment content.
        /// </summary>
        /// <returns>Content stream.</returns>
        Stream GetContentStream();
    }
}
