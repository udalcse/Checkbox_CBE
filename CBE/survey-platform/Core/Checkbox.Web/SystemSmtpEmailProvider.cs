using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Checkbox.Content;
using Checkbox.Management;
using Checkbox.Messaging.Email;

namespace Checkbox.Web.Mail
{
    ///<summary>
    ///Email provider that sends messages via SMTP;
    ///</summary>
    public class SystemSmtpEmailProvider : BaseEmailProvider
    {
        /// <summary>
        /// The company signature email identifier
        /// </summary>
        private const string CompanySignatureEmailId = "companySignature";

        /// <summary>
        /// The content URL parameter
        /// </summary>
        private const string ContentUrlParameter = "contentID";

        /// <summary>
        /// The company signature tag
        /// </summary>
        private const string CompanySignatureTag = "<img style = 'display:block' src=\"cid:" + CompanySignatureEmailId + "\">";

        /// <summary>
        /// Compose and send an SMTP messgae.
        /// </summary>
        /// <param name="message"></param>
        protected override long? DoSendMessage(IEmailMessage message)
        {
            SmtpClient client = CreateSmtpClient();
            client.Send(CreateMessage(message));

            return null;
        }

        /// <summary>
        /// Create an System.Net.Mail.MailMessage object from a Checkbox IEmailMessage object.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static MailMessage CreateMessage(IEmailMessage message)
        {
            MailMessage netMailMessage;

            if (ApplicationManager.AppSettings.CompanySignatureEnabled &&
                !string.IsNullOrWhiteSpace(ApplicationManager.AppSettings.CompanySignatureImageUrl) &&
                message.IsBodyHtml)
            {
                netMailMessage = CreateAlternativeViewMessage(message) ?? CreateBasicMessage(message);
            }
            else 
            {
               netMailMessage = CreateBasicMessage(message);
            }

            //Add attachments
            foreach (IEmailAttachment attachment in message.Attachments)
            {
                netMailMessage.Attachments.Add(new Attachment(
                    attachment.GetContentStream(),
                    attachment.FileName,
                    attachment.MimeContentTypeString));
            }

            return netMailMessage;
        }

        private static MailMessage CreateAlternativeViewMessage(IEmailMessage message)
        {
            MailMessage netMailMessage = null;

            var signatureImage = GetSignatureImage();

            if (signatureImage != null)
            {
                LinkedResource companySignature = new LinkedResource(new MemoryStream(signatureImage.Data),
                    MediaTypeNames.Image.Jpeg)
                {ContentId = CompanySignatureEmailId};

                AlternateView view =
                    AlternateView.CreateAlternateViewFromString(message.Body + CompanySignatureTag, null,
                        MediaTypeNames.Text.Html);

                view.LinkedResources.Add(companySignature);

                netMailMessage = new MailMessage(
                    message.From,
                    message.To
                    )
                {Subject = message.Subject, IsBodyHtml = message.IsBodyHtml};

                netMailMessage.AlternateViews.Add(view);
            }

            return netMailMessage;
        }

        private static MailMessage CreateBasicMessage(IEmailMessage message)
        {

            var emailMessage = new MailMessage(
                 message.From,
                 message.To,
                 message.Subject,
                 AddLineBreaks(message.Body))
            { IsBodyHtml = message.IsBodyHtml };

            if (!message.IsBodyHtml)
            {
                var image = GetSignatureImage();

                if (image != null)
                {
                    emailMessage.Attachments.Add(new Attachment(new MemoryStream(image.Data), "CompanySignature.jpeg", MediaTypeNames.Image.Jpeg));
                }
            }

            return emailMessage;
        }

        private static DBContentItem GetSignatureImage()
        {
            var imageUrl = ApplicationManager.AppSettings.CompanySignatureImageUrl;
            var isAbsolute = WebUtilities.IsAbsoluteUrl(imageUrl);

            if (WebUtilities.IsValidUri(imageUrl, isAbsolute ? UriKind.Absolute : UriKind.Relative))
            {
                Uri uri = !isAbsolute ? WebUtilities.ConvertRelativeUrlToAbsoluteUrl(imageUrl) : new Uri(imageUrl);

                var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
                int contentId;
                var contentParameter = queryDictionary.Get(ContentUrlParameter);

                if (!string.IsNullOrWhiteSpace(contentParameter) && int.TryParse(contentParameter, out contentId))
                {
                    var item = DBContentManager.GetItem(contentId);

                    if (item != null)
                        return item;
                }
            }

            return null;
        }


        /// <summary>
        /// Create an instance of the smtp client to send the message
        /// </summary>
        /// <returns></returns>
        private static
            SmtpClient CreateSmtpClient()
        {
            //Create the client
            SmtpClient client = new SmtpClient(
                ApplicationManager.AppSettings.SmtpServer,
                ApplicationManager.AppSettings.SmtpPort) {EnableSsl = ApplicationManager.AppSettings.EnableSmtpSsl};

            //Set credentials, if necessary
            SetClientCredentials(client);

            return client;
        }

        /// <summary>
        /// Set client credentials for the smtp client
        /// </summary>
        /// <param name="client"></param>
        private static void SetClientCredentials(SmtpClient client)
        {
            //Do nothing if SMTP authentication not enabled
            if (!ApplicationManager.AppSettings.EnableSmtpAuthentication)
            {
                return;
            }

            client.Credentials = GetSmtpAuthCredentials();
        }

        /// <summary>
        /// Get credentials to use for smtp authentication
        /// </summary>
        /// <returns></returns>
        private static NetworkCredential GetSmtpAuthCredentials()
        {
            string userName = ApplicationManager.AppSettings.SmtpUserName;
            string newUserName = string.Empty;
            string domain = string.Empty;

            if (userName.Contains("/") || userName.Contains("\\"))
            {
                int indexOfForwardSlash = userName.IndexOf("/");
                int indexOfBackSlash = userName.IndexOf("\\");

                if (indexOfForwardSlash > 0 && indexOfForwardSlash < userName.Length - 1)
                {
                    newUserName = userName.Substring(0, indexOfForwardSlash);
                    domain = userName.Substring(indexOfForwardSlash + 1);
                }
                else if (indexOfBackSlash > 0 && indexOfBackSlash < userName.Length - 1)
                {
                    newUserName = userName.Substring(0, indexOfBackSlash);
                    domain = userName.Substring(indexOfBackSlash + 1);

                }
            }

            if (string.IsNullOrEmpty(domain)
                || string.IsNullOrEmpty(newUserName))
            {
                return new NetworkCredential(
                    ApplicationManager.AppSettings.SmtpUserName,
                    ApplicationManager.AppSettings.SmtpPassword);
            }

            return new NetworkCredential(
                newUserName,
                ApplicationManager.AppSettings.SmtpPassword,
                domain);
        }

        /// <summary>
        /// Provider does not support batches, so always return false.
        /// </summary>
        public override bool SupportsBatches
        {
            get { return false; }
        }

        /// <summary>
        /// Changes the scheduled date for the batch
        /// </summary>
        /// <param name="batchId">Id of the batch</param>
        /// <param name="scheduledDate">New date when the batch should be sent</param>
        public override void SetMessageBatchDate(long batchId, System.DateTime scheduledDate)
        {
        }
    }
}
