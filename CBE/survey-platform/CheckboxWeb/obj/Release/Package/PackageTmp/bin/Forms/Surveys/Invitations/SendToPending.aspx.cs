using System;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Messaging.Email;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SendToPending : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/sendInvite") + " - " + Utilities.StripHtml(Invitation.Name, 64));

            int pendingCount = Invitation.GetPendingRecipientsCount();

            _subjectReview.Text = Invitation.Template.Subject;
            _messageReview.Text = Invitation.Template.Body;
            _recipientCountReview.Text = String.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/sendReminder.aspx/recipientReviewCount"), "<b>" + pendingCount + "</b>");

            //Check if emails provided by the license are enough.
            String errorMsg;
            if (!CheckForEmailsEnough(pendingCount, out errorMsg))
            {
                _emailsNotEnoughWarning.Text = errorMsg;
                _emailsNotEnoughWarningPanel.Visible = true;
                _recipientCountReview.Visible = _recipientReviewLink.Visible = false;
                _sendNowButton.Visible = false;
            }

            _recipientReviewLink.NavigateUrl = string.Format("Recipients.aspx?i={0}&m=pending", Invitation.ID);

            _invitationSender.OnRemindClick += new Invitations.Controls.Send.RemindDelegate(_invitationSender_OnRemindClick);
        }

        /// <summary>
        /// Start sending process
        /// </summary>
        void _invitationSender_OnRemindClick()
        {
            _invitationSender.UpdateSchedule(Checkbox.Invitations.InvitationActivityType.Reminder);
            if (!EmailGateway.ProviderSupportsBatches)
                _invitationSender.StartProgress("remind");
            else
                Master.CloseDialog(null, false);
        }

        /// <summary>
        /// Check if emails count is enough.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool CheckForEmailsEnough(int count, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (Page is LicenseProtectedPage)
            {
                var emailLimit = (Page as LicenseProtectedPage).ActiveLicense.EmailLimit;
                long? currentEmailsValue = emailLimit.CurrentValue;

                if (!currentEmailsValue.HasValue)
                {
                    return true;
                }

                if (currentEmailsValue.Value < count)
                {
                    switch (currentEmailsValue.Value)
                    {
                        case 0: errorMsg = WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/noAvailableEmailsWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.");
                            break;
                        case 1:
                            errorMsg = WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/onlyOneEmailAvailableWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.");
                            break;
                        default:
                            errorMsg = WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/availableEmailsNotEnoughWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.").Replace("{0}", currentEmailsValue.Value.ToString());
                            break;
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendButton_Click(object sender, EventArgs e)
        {
            _sendPanel.Visible = true;
            _reviewPanel.Visible = false;
            _sendNowButton.Visible = false;

            _invitationSender.AutoSend = true;

            _invitationSender.Initialize(Invitation);
        }
    }
}