using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Security;
using Checkbox.Web;
using Checkbox.Invitations;
using Checkbox.Messaging.Email;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class MessageReview : InvitationWizardPage
    {
        private Recipient _recipient;
        private Dictionary<long, IEmailMessage> _batchMessages;
        private Boolean _recipientPreview;


        /// <summary>
        /// Load batch messages and populate select message drop down
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            Page.Title = WebTextManager.GetText("/pageText/forms/surveys/invitations/messageReview.aspx/title");
            Master.HideDialogButtons();
            long recipientID;

            if (long.TryParse(Request.QueryString["r"], out recipientID))
            {
                //We're looking at an invitation for a specific recipient
                _recipientPreview = true;
                List<Recipient> recipients = Invitation.GetRecipients(new long[] { recipientID });

                if (recipients.Count > 0)
                {
                    _recipient = recipients[0];

                    if (EmailGateway.ProviderSupportsBatches)
                    {
                        PopulateMessageList(_recipient);
                    }
                }
            }
            else
            {
                //We're in the wizard, previewing the general message
                _recipientPreview = false;
            }
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            ShowPreview();
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            Master.CloseDialog(null);
        }

        
        
        /// <summary>
        /// Show the preview of the invitation message.
        /// </summary>
        private void ShowPreview()
        {
            _bodyPreviewPanel.Controls.Clear();
            _subjectPreviewPanel.Controls.Clear();

            if (_recipientPreview && _recipient != null)
            {
                if (EmailGateway.ProviderSupportsBatches)
                {
                    ShowBatchMessagePreview(_recipient);
                }
                else
                {
                    ShowStandardPreview(_recipient);
                }

            }
            else
            {
                //Hide the message selection controls
                _messageSelectPanel.Visible = false;

                GeneratePreview(Invitation.Template.Format, Invitation.Template.Subject, Invitation.Template.Body);

            }

        }

        /// <summary>
        /// Populate the list if messages available for the recipient
        /// </summary>
        /// <param name="recipient"></param>
        private void PopulateMessageList(Recipient recipient)
        {
            _batchMessages = new Dictionary<long, IEmailMessage>();
            _messageList.Items.Clear();

            //Get a list of invitations for the recipient
            List<long> recipientMessageIds = InvitationManager.ListRecipientQueueMessages(recipient.ID.Value);

            //If no messages, go w/standard preview in case message batches aren't tracked or invitation
            // was sent before batches enabled.
            if (recipientMessageIds.Count == 0)
            {
                return;
            }

            foreach (long recipientMessageId in recipientMessageIds)
            {
                IEmailMessage message = EmailGateway.GetMessageFromBatch(recipientMessageId);

                if (message != null)
                {
                    IEmailMessageStatus messageStatus = EmailGateway.GetMessageStatus(recipientMessageId);

                    if (messageStatus != null && messageStatus.QueuedDate.HasValue)
                    {
                        _batchMessages[recipientMessageId] = message;
                        _messageList.Items.Add(new ListItem(
                            messageStatus.QueuedDate.ToString(),
                            recipientMessageId.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Show the message preview for batch messages
        /// </summary>
        /// <param name="recipient"></param>
        private void ShowBatchMessagePreview(Recipient recipient)
        {
            //If message data could not be retrieved from email gateway, which may be
            // possible if provider removes messages, go with standard preview
            long selectedMessage;

            if (long.TryParse(_messageList.SelectedValue, out selectedMessage))
            {
                if (_batchMessages != null && _batchMessages.ContainsKey(selectedMessage))
                {
                    IEmailMessage message = _batchMessages[selectedMessage];

                    //Show preview
                    GeneratePreview(
                        message.IsBodyHtml ? MailFormat.Html : MailFormat.Text,
                        message.Subject,
                        message.Body);

                    //Update status
                    IEmailMessageStatus status = EmailGateway.GetMessageStatus(selectedMessage);

                    if (status != null)
                    {
                        if (status.SuccessfullySent)
                        {
                            _yesNoLbl.Text = WebTextManager.GetText("/common/yes");
                            _yesNoLbl.CssClass = "Message";
                            _errorMessageLbl.Visible = false;
                        }
                        else
                        {
                            _yesNoLbl.Text = WebTextManager.GetText("/common/no");
                            _yesNoLbl.CssClass = "ErrorMessage";
                            _errorMessageLbl.Visible = true;
                            _errorMessageLbl.Text = "[" + status.LastSendError + "]";
                        }
                    }

                    _messageSelectPanel.Visible = _messageList.Items.Count > 0;
                }
            }
            else
            {
                ShowStandardPreview(recipient);
            }
        }

        /// <summary>
        /// Show the standard message preview
        /// </summary>
        /// <param name="recipient"></param>
        private void ShowStandardPreview(Recipient recipient)
        {
            _messageSelectPanel.Visible = false;

            //Copy the template
            InvitationTemplate template = Invitation.Template.Copy();

            //Personalize for the recipient
            throw new NotImplementedException();
            //recipient.PersonalizeTemplate(Invitation, template);

            GeneratePreview(template.Format, template.Subject, template.Body);
        }

        /// <summary>
        /// Generate the message preview
        /// </summary>
        /// <param name="mailFormat"></param>
        /// <param name="messageSubject"></param>
        /// <param name="messageBody"></param>
        private void GeneratePreview(MailFormat mailFormat, string messageSubject, string messageBody)
        {
            _subjectPreviewPanel.Controls.Clear();
            _bodyPreviewPanel.Controls.Clear();

            //Update the preview
            //Replace newlines with breaks for text-format invitations for better display
            if (mailFormat == MailFormat.Html)
            {
                _bodyPreviewPanel.Controls.Add(new LiteralControl(messageBody));
                _subjectPreviewPanel.Controls.Add(new LiteralControl(messageSubject));
            }
            else
            {
                _bodyPreviewPanel.Controls.Add(new LiteralControl(messageBody.Replace(Environment.NewLine, "<br />")));
                _subjectPreviewPanel.Controls.Add(new LiteralControl(messageSubject.Replace(Environment.NewLine, "<br />")));
            }
        }
    }
}
