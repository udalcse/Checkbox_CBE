using System;
using System.Data;
using System.Net.Mime;
using System.Collections.Specialized;

using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Checkbox.Messaging.Email;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Styles;
using System.Text;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item that encapsulates sending an email.
    /// </summary>
    [Serializable]
    public class EmailItem : ResponseItem
    {
        private const string MESSAGE_BODY_TOKEN = "[[MESSAGE_BODY]]";
        private string _from;
        private string _messageFormat;
        private int? _styleTemplateID;
        private string _to;
        private string _bcc;
        private string _subject;
        private string _body;

        private bool _sendOnlyOnce;

        private Int32 _myPage;

        #region Properties

        /// <summary>
        /// Get the from email address
        /// </summary>
        public virtual string From
        {
            get
            {
                string from = GetPipedText("From", _from) ?? string.Empty;
                string[] fromAddresses = from.Split(',', ';');

                //Find the first none empty email address
                //Note: It is not possible to enter a from of this format in Checkbox
                //this logic is in place to support Ultimate Survey upgrades.
                if (fromAddresses.Length > 0)
                {
                    foreach (string address in fromAddresses)
                    {
                        if (Utilities.IsNotNullOrEmpty(address))
                        {
                            from = address;
                            break;
                        }
                    }
                }

                if(string.IsNullOrEmpty(from))
                    from = ApplicationManager.AppSettings.SystemEmailAddress;

                return from;
            }
        }

        /// <summary>
        /// Get the message format
        /// </summary>
        public virtual string MessageFormat
        {
            get { return _messageFormat; }
        }

        /// <summary>
        /// Get the email message mime type when sending email as attachments (not
        /// currently part of the product, but will be eventually).
        /// </summary>
        public virtual ContentType MessageContentType
        {
            get
            {
                var contentType = new ContentType();

                if ("Html".Equals(MessageFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    contentType.MediaType = "text/html";
                }
                else if ("Xml".Equals(MessageFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    contentType.MediaType = "text/xml";
                }
                else
                {
                    contentType.MediaType = "text/plain";
                }

                contentType.Parameters["charset"] = "utf-8";

                return contentType;
            }
        }

        /// <summary>
        /// Gets the StyleTemplate ID associated with this email
        /// </summary>
        public virtual int? StyleTemplateID
        {
            get { return _styleTemplateID; }
        }

        /// <summary>
        /// Get the To address
        /// </summary>
        public virtual string To
        {
            get { return GetPipedText("To", _to); }
        }

        /// <summary>
        /// Get the bcc address
        /// </summary>
        public virtual string BCC
        {
            get { return GetPipedText("Bcc", _bcc); }
        }

        /// <summary>
        /// Get the email subject
        /// </summary>
        public virtual string Subject
        {
            get { return GetPipedText("Subject", _subject); }
        }

        /// <summary>
        /// Get the email body
        /// </summary>
        public virtual string Body
        {  
            get { return GetPipedText("Body", _body); }
        }

        /// <summary>
        /// Get whether the item is visible
        /// </summary>
        private bool _visible = true;
        public override bool Visible
        {
            get
            {
                if (_visible)
                    return (Response != null) ? false : true;

                return _visible;
            }

            protected set { _visible = value; }
        }

        #endregion

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)        
        {
            Visible = ExportMode != ExportMode.Pdf;
            ArgumentValidation.CheckExpectedType(configuration, typeof(EmailItemData));
            var config = (EmailItemData)configuration;

            _from = config.From;
            _messageFormat = config.MessageFormat;
            _styleTemplateID = config.StyleTemplateID;
            _to = config.To;
            _bcc = config.BCC;
            _subject = TextManager.GetText(config.SubjectTextID, languageCode);
            _body = TextManager.GetText(config.BodyTextID, languageCode);
            _sendOnlyOnce = config.SendOnce;
            _myPage = -1;

            base.Configure(config, languageCode, templateId);
        }

        /// <summary>
        /// Get the template for a message.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMessageTemplate()
        {
            if (MessageFormat.Equals("Html", StringComparison.InvariantCultureIgnoreCase)
                || MessageFormat.Equals("SurveyHtml", StringComparison.InvariantCultureIgnoreCase))
            {
                StyleTemplate st = null;

                if (StyleTemplateID.HasValue)
                {
                    st = StyleTemplateManager.GetStyleTemplate(StyleTemplateID.Value);
                }

                var sb = new StringBuilder();

                //Open html
                sb.Append("<html>");
                sb.Append("<head>");

                //Add style css
                if (st != null)
                {
                    sb.Append("<style type=\"text/css\">");
                    sb.Append(st.GetCss());
                    sb.Append("</style>");
                }

                sb.Append("</head>");

                //Open body
                sb.Append("<body>");

                //Add style header
                if (st != null)
                {
                    sb.Append(TextManager.GetText(st.HeaderTextID, LanguageCode));
                }

                //Add placeholder for body text
                sb.Append(MESSAGE_BODY_TOKEN);

                //Add style footer
                if (st != null)
                {
                    sb.Append(TextManager.GetText(st.FooterTextID, LanguageCode));
                }

                //Close body and html
                sb.Append("</body>");
                sb.Append("</html>");

                return sb.ToString();

            }

            return MESSAGE_BODY_TOKEN;
        }

        /// <summary>
        /// Get Message Body
        /// </summary>
        /// <returns></returns>
        protected virtual string GetBodyText()
        {
            if (Response != null)
            {
                return GetPipedText("Body", _body);
            }

            return Body;
        }

        /// <summary>
        /// Create the message body
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMessageBody()
        {
            return GetMessageTemplate().Replace(MESSAGE_BODY_TOKEN, GetBodyText());
        }

        /// <summary>
        /// Send the email when the page is loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            try
            {
                if (Response != null)
                {
                    _myPage = Response.CurrentPage.Position;

                    Response.PageChanged -= Response_PageChanged;
                    Response.PageChanged += Response_PageChanged;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
            }
        }

        /// <summary>
        /// Send message when page is advanced
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Response_PageChanged(object sender, ResponsePageChangedEventArgs e)
        {
            if (e.NewPage == _myPage && e.NewPage != e.PreviousPage)
            {
                //Handle and log error, but don't cause exception to be thrown
                try
                {
                    SendEmail();

                    //If only sending once, unhook the event handler
                    if (_sendOnlyOnce)
                    {
                        Response.PageChanged -= Response_PageChanged;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");
                }
            }
        }

        /// <summary>
        /// Send the email message
        /// </summary>
        protected void SendEmail()
        {
            if (OkToSend)
            {
                //Send the basic message
                var message = new EmailMessage
                {
                    From = From,
                    To = To,
                    Subject = Subject,
                    Body = GetMessageBody()
                };

                /* BEGIN MESSAGE AS ATTACHMENT CODE FOR FUTURE USE
                //message.Body = GetPipedText("MessageBody", Body);

                //Add other as an attachment
                message.Attachments.Add(new EmailAttachment(
                    MessageContentType,
                    GetMessageBody()));
                 * 
                 * 
                 * END MESSAGE AS ATTACHMENT CODE */

                if (MessageFormat == null || MessageFormat.ToLower() != "html")
                {
                    message.Format = MailFormat.Text;
                }

                //send message to each individual in the to: field
                string[] addresses = message.To.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string address in addresses)
                {
                    message.To = address.Trim();

                    if (!string.IsNullOrEmpty(message.To) && message.To.Contains("@") && message.To.Contains("."))
                    {
                        try
                        {
                            EmailGateway.Send(message);
                        }
                        catch (Exception ex)
                        {
                            ExceptionPolicy.HandleException(ex, "BusinessPublic");
                            continue;
                        }
                    }
                }

                //Now send the bcc
                if (BCC != null)
                {
                    addresses = BCC.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string address in addresses)
                    {
                        message.To = address.Trim();

                        if (!string.IsNullOrEmpty(message.To) && message.To.Contains("@") && message.To.Contains("."))
                        {
                            try
                            {
                                EmailGateway.Send(message);
                            }
                            catch (Exception ex)
                            {
                                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                                continue;
                            }
                        }
                    }
                }

                MarkEmailSent();
            }
        }

        /// <summary>
        /// Mark that an email has been sent by storing an answer
        /// </summary>
        protected virtual void MarkEmailSent()
        {
            if (_sendOnlyOnce && Response != null)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_MarkEmailSent");
                command.AddInParameter("ResponseID", DbType.Int32, Response.ID);
                command.AddInParameter("EmailItemID", DbType.Int32, ID);
                command.AddInParameter("EmailDate", DbType.DateTime, DateTime.Now);

                db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Check if the email has been sent
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckEmailSent()
        {
            if (Response != null)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetEmailSent");
                command.AddInParameter("ResponseID", DbType.Int32, Response.ID);
                command.AddInParameter("EmailItemID", DbType.Int32, ID);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        return reader.Read();
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determine whether it's ok to send the message by checking the response state
        /// and the send once parameter.
        /// </summary>
        protected virtual bool OkToSend
        {
            get
            {
                if (!Excluded && Response != null)
                {
                    if (!_sendOnlyOnce)
                    {
                        return true;
                    }

                    return !CheckEmailSent();
                }

                return false;

            }
        }

        /// <summary>
        /// Get metadata/configuration related values
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["MessageFormat"] = MessageFormat;
            values["StyleTemplateID"] = StyleTemplateID.HasValue ? StyleTemplateID.ToString() : null;
            values["SendOnce"] = _sendOnlyOnce.ToString();

            return values;
        }

        /// <summary>
        /// Get data for the item
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["From"] = From;
            values["To"] = To;
            values["Bcc"] = BCC;
            values["Subject"] = Subject;
            values["Body"] = Body;

            return values;
        }
    }
}
