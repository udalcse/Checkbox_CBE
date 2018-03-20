using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using CheckboxWeb.Forms.Surveys.Invitations.Controls;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SendScheduledReminder : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter]
        public bool IsReturn { get; set; }

        protected override void OnPageLoad()
        {
            if (!IsPostBack)
            {
                _scheduledDate.DateTime = DateTime.Now.AddMinutes(1);
                _scheduledDate.MinDateTime = Invitation.InvitationScheduled;
            }

            //Date Utils
            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));

            base.OnPageLoad();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _invitationWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/forms/surveys/invitations/sendReminder.aspx/wizardStepTitle/{0}", step.ID));
            }

            Master.HideDialogButtons();
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/invitations/sendReminder.aspx/title") + " - " + Utilities.StripHtml(Invitation.Name, 64));

            if (Invitation.SuccessfullySent.HasValue && Invitation.SuccessfullySent.Value)
                _recipientReviewLink.NavigateUrl = string.Format("Recipients.aspx?i={0}&m=notresponded", Invitation.ID);
            else
                _recipientReviewLink.Visible = false;

            //Initialize message editor
            if (Invitation != null)
            {
                _editMessage.InitializeReminder(Invitation);
            }
            
            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();

            //
            if (IsReturn)
            {
                _invitationWizard.ActiveStepIndex = 1;
                UpdateReview();
            }

            //
            if (!Invitation.SuccessfullySent.HasValue || !Invitation.SuccessfullySent.Value)
            {
                _activationWarningLbl.Text = string.Format(WebTextManager.GetText("/pageText/reviewInvitation.aspx/invitationNotSent"));
                _invitationNotSentWarning.Visible = true;
            }
            else
                _invitationNotSentWarning.Visible = false;
        }

        #region Wizard event handlers

        /// <summary>
        /// Move to review Step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid || !UpdateReview())
                e.Cancel = true;
        }

        /// <summary>
        /// Handles the finish event of the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            var scheduleItem = new InvitationSchedule
            {
                Scheduled = _postponed.Checked ? default(DateTime?) : (_schedule.Checked && _scheduledDate.DateTime.HasValue ? _scheduledDate.DateTime : DateTime.Now.AddMinutes(1))
            };

            //avoid async errors
            if (!_postponed.Checked && EmailGateway.ProviderSupportsBatches 
                && scheduleItem.Scheduled < DateTime.Now.AddMinutes(1))
            {
                scheduleItem.Scheduled = DateTime.Now.AddMinutes(1);
            }

            scheduleItem.InvitationID = Invitation.ID;
            scheduleItem.InvitationActivityType = InvitationActivityType.Reminder;
            scheduleItem.Save(Context.User as CheckboxPrincipal);

            Invitation.AddScheduleItem(scheduleItem);
            
            if (!_postponed.Checked)
                InvitationSender.Send(Invitation, scheduleItem, null, ApplicationManager.ApplicationDataContext);

            CloseAndRefreshInvitations();
        }

        /// <summary>
        /// Register a javascript that will close the window and update invitation list
        /// </summary>
        private void CloseAndRefreshInvitations()
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeAndRefreshInvitations", "<script>$(document).ready(function(){closeAndRefreshInvitations();});</script>");                                   
        }

        /// <summary>
        /// 
        /// </summary>
        private bool UpdateReview()
        {
            var count = Invitation.GetPendingRecipientsCount();

            String errorMsg;

            if (CheckForEmailsEnough(count, out errorMsg))
            {
                _emailsNotEnoughWarningPanel.Visible = false;
            }
            else
            {
                _emailsNotEnoughWarning.Text = errorMsg;
                _emailsNotEnoughWarningPanel.Visible = true;
            }

            //Set updated invitation properties on the in-memory invitation and save
            if (!_editMessage.UpdateReminder(Invitation))
                return false;

            Invitation.Save(UserManager.GetCurrentPrincipal());

            //Set up the review panel
            _subjectReview.Text = Invitation.Template.ReminderSubject;
            _messageReview.Text = Invitation.Template.ReminderBody;

            int remindersCount = Invitation.SuccessfullySent.HasValue && Invitation.SuccessfullySent.Value ?
                Invitation.GetRecipients(RecipientFilter.NotResponded).Count
                : Invitation.GetPendingRecipientsCount();

            _recipientCountReview.Text = String.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/sendReminder.aspx/recipientReviewCount"), "<b>" + remindersCount + "</b>");

            //Check for license limit exceeded
            if (!CheckForEmailsEnough(remindersCount, out errorMsg))
            {
                _emailsNotEnoughWarning.Text = errorMsg;
                _emailsNotEnoughWarningPanel.Visible = true;
                _recipientCountReview.Visible = _recipientReviewLink.Visible = false;
            }
            return true;
        }

        /// <summary>
        /// Handles the cancel event of the wizard;
        /// - redirects back to the user manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_CancelButtonClick(object sender, EventArgs e)
        {
            Master.CloseDialog("cancelReminder", false);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlClientID"></param>
        /// <returns></returns>
        protected String MakeButtonEventTarget(String controlClientID)
        {
            String eventTarget = Checkbox.Web.UI.Controls.Adapters.WebControlAdapterExtender.MakeNameFromId(controlClientID);
            return eventTarget;
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
    }
}
