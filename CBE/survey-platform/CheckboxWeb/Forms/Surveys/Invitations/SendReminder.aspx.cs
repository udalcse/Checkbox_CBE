using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using CheckboxWeb.Controls.Wizard.WizardControls;
using CheckboxWeb.Forms.Surveys.Invitations.Controls;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SendReminder : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter]
        public bool IsReturn { get; set; }

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

            _recipientReviewLink.NavigateUrl = string.Format("Recipients.aspx?i={0}&m=notresponded", Invitation.ID);

            //Initialize message editor
            if (Invitation != null)
            {
                _editMessage.InitializeReminder(Invitation);
            }            

            if (IsReturn && !IsPostBack)
            {
                _invitationWizard.ActiveStepIndex = 1;
                UpdateReview();
            }

            _invitationSender.OnRemindClick += _invitationSender_OnRemindClick;
        }

        /// <summary>
        /// Start sending process
        /// </summary>
        void _invitationSender_OnRemindClick()
        {
            _invitationSender.UpdateSchedule(InvitationActivityType.Reminder);
            if (!EmailGateway.ProviderSupportsBatches)
                _invitationSender.StartProgress("remind");
            else
                Master.CloseDialog(null, false);
        }

        #region Wizard event handlers


        /// <summary>
        /// Handles the finish event of the wizard
        /// - Redirects to the progress screen if invitations are to be sent immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid || !UpdateReview())
                e.Cancel = true;
        }

        /// <summary>
        /// Get navigation buttons
        /// </summary>
        protected WizardButtons NavigationButtons
        {
            get
            {
                return _invitationWizard.FindControl("FinishNavigationTemplateContainerID").FindControl("_finishNavigationButtons") as
                       WizardButtons;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsReviewStep
        {
            get { return _invitationWizard.ActiveStepIndex == 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool UpdateReview()
        {
            if (!_editMessage.UpdateReminder(Invitation))
                return false;

            Invitation.Save(UserManager.GetCurrentPrincipal());

            //Set up the review panel
            _subjectReview.Text = Invitation.Template.ReminderSubject;
            _messageReview.Text = Invitation.Template.ReminderBody;

            int remindersCount = Invitation.GetRecipients(RecipientFilter.NotResponded).Count;
            _recipientCountReview.Text = String.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/sendReminder.aspx/recipientReviewCount"), "<b>" + remindersCount + "</b>");

            //Check for license limit exceeded
            String errorMsg;
            if (!CheckForEmailsEnough(remindersCount, out errorMsg))
            {
                _emailsNotEnoughWarning.Text = errorMsg;
                _emailsNotEnoughWarningPanel.Visible = true;
                _recipientCountReview.Visible = _recipientReviewLink.Visible = false;
                NavigationButtons.NextButton.Visible = false;
            }

            if (remindersCount <= 0)
                NavigationButtons.NextButton.Visible = false;

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

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendButton_Click(object sender, EventArgs e)
        {
            _sendPanel.Visible = true;
            _reviewPanel.Visible = false;
            NavigationButtons.NextButton.Visible = false;

            _invitationSender.AutoSend = true;
            _invitationSender.Initialize(Invitation);
            if (EmailGateway.ProviderSupportsBatches)
            {
        //        _closeWizardButton.Visible = true;
            }
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

        protected void ChangeWizardStepToPrevious(object sender, EventArgs e)
        {
            _invitationWizard.ActiveStepIndex -= 1;
        }
    }
}
