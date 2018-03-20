using System.IO;
using System.Text;
using System.Net.Mime;

using Checkbox.Common;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Email attachment implementation.
    /// </summary>
    public class EmailAttachment : IEmailAttachment
    {
        private readonly string _contentString;
        private readonly byte[] _contentBytes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EmailAttachment()
        {
        }

        /// <summary>
        /// Construct an email attachment with string content
        /// </summary>
        /// <param name="contentType">Attachment content type.</param>
        /// <param name="content"></param>
        public EmailAttachment(string contentType, string content)
        {
            MimeContentTypeString = contentType;
            _contentString = content;
        }

        /// <summary>
        /// Construct an email attachment with binary content
        /// </summary>
        /// <param name="contentType">Attachment content type.</param>
        /// <param name="content"></param>
        public EmailAttachment(string contentType, byte[] content)
        {
            MimeContentTypeString = contentType;
            _contentBytes = content;
        }

        #region IEmailAttachment Members

        /// <summary>
        /// Get/set file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Get/set content type
        /// </summary>
        public string MimeContentTypeString { get; set; }

        /// <summary>
        /// Get a stream for the content
        /// </summary>
        /// <returns></returns>
        public Stream GetContentStream()
        {
            if (Utilities.IsNotNullOrEmpty(_contentString))
            {
                return GetContentStream(_contentString);
            }

            if (_contentBytes != null)
            {
                return GetContentStream(_contentBytes);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Get a stream for reading string content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Stream GetContentStream(string content)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(Encoding.UTF8.GetBytes(content), 0, content.Length);

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        /// <summary>
        /// Get a stream for byte content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Stream GetContentStream(byte[] content)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(content, 0, content.Length);

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
