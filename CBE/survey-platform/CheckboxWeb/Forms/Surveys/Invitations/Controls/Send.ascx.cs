using System;
using System.Linq;
using System.Web.UI;
using Checkbox.Globalization;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Messaging.Email;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Send : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected int InvitationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int ScheduleId { 
            get
            {
                return ViewState["ScheduleId"] == null ? 0 : (int)ViewState["ScheduleId"]; 
            } 
            set
            {
                ViewState["ScheduleId"] = value;
            }
        }

        /// <summary>
        /// Set send mode, options are "invite", "remind", "both"
        /// </summary>
        public string SendMode { get; set; }

        /// <summary>
        /// Get/set whether to send once initialized
        /// </summary>
        public bool AutoSend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Send()
        {
            SendMode = "both";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            _inviteBtn.Click += _inviteBtn_Click;
            _remindBtn.Click += _remindBtn_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                _scheduledDate.DateTime = DateTime.Now.AddHours(1);
            }

            //Ensure necessary javascript loaded
            RegisterClientScriptInclude(
                "AjaxProgress.js",
                ResolveUrl("~/Resources/AjaxProgress.js"));

            RegisterClientScriptInclude(
                "jquery.progressbar.min.js",
                ResolveUrl("~/Resources/jquery.progressbar.min.js"));

            //Date Utils
            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));
        
            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation)
        {
            //Ensure messages hidden by default
            _noRecipientsPanel.Visible = false;
            _emailsNotEnoughToInviteWarningPanel.Visible = false;
            _emailsNotEnoughToRemindWarningPanel.Visible = false;
            _recipientsPanel.Visible = true;
            _sendPanel.Visible = false;
            _closeWizardContainer.Visible = false;

            InvitationId = invitation.ID.Value;

            var pendingCount = invitation.GetRecipients(RecipientFilter.Pending).Count;
            var reminderCount = invitation.GetRecipients(RecipientFilter.NotResponded).Count;

            //Show message if no recipients
            if (pendingCount == 0 && reminderCount == 0)
            {
                _noRecipientsPanel.Visible = true;
                _recipientsPanel.Visible = false;
                return;
            }

            //Check for available emails
            String errorMsg;
            if (!CheckForEmailsEnough(pendingCount, out errorMsg))
            {
                _emailsNotEnoughToInviteWarning.Text = errorMsg;
                _emailsNotEnoughToInviteWarningPanel.Visible = true;
                _inviteBtn.Visible = false;
            }

            if (!CheckForEmailsEnough(reminderCount, out errorMsg))
            {
                _emailsNotEnoughToRemindWarning.Text = errorMsg;
                _emailsNotEnoughToRemindWarningPanel.Visible = true;
                _remindBtn.Visible = false;
            }

            _errorPanel.Visible = false;
            _successPanel.Visible = false;

            //If explicit send mode, use that
            if ("invite".Equals(SendMode, StringComparison.InvariantCultureIgnoreCase))
            {
                //Make control not visible.  Using .Visible prevents .NET from rendering control at all, which would
                // cause problems with javascript, so simply set display to none.
                _remindBtn.Style["display"] = "none";
                _inviteBtn.Style["display"] = "none";

                if (AutoSend)
                {
                    _inviteBtn_Click(this, new EventArgs());
                }
            }

            if ("remind".Equals(SendMode, StringComparison.InvariantCultureIgnoreCase))
            {
                _remindBtn.Style["display"] = "none";
                _inviteBtn.Style["display"] = "none";


                if (AutoSend)
                {
                    _remindBtn_Click(this, new EventArgs());
                }
            }

            if ("both".Equals(SendMode, StringComparison.InvariantCultureIgnoreCase))
            {
                //If no pending recipients, set reminder as default action
                if (pendingCount == 0)
                {
                    _invitePanel.Style["display"] = "none";
                    _remindPanel.Style.Remove("display");
                }

                if (reminderCount == 0)
                {
                    _remindPanel.Style["display"] = "none";
                    _invitePanel.Style.Remove("display");
                }
            }
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
                        case 0: errorMsg = WebTextManager.GetText("/controlText/send.ascx/noAvailableEmailsWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.");
                            break;
                        case 1:
                            errorMsg = WebTextManager.GetText("/controlText/send.ascx/onlyOneEmailAvailableWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.");
                            break;
                        default:
                            errorMsg = WebTextManager.GetText("/controlText/send.ascx/availableEmailsNotEnoughWarning", null, "Count of available emails provided by the license isn't enough. Please, try to decrease the email''s count to send or try to do it later.").Replace("{0}", currentEmailsValue.Value.ToString());
                            break;
                    }
                    
                    return false;
                }
            }

            return true;
        }


        public delegate void RemindDelegate();
        public event RemindDelegate OnRemindClick;

        /// <summary>
        /// Saves Scheduled Date to session because of cross-forms calls
        /// </summary>
        private void saveScheduledDate()
        {
            if (_schedule.Checked)
                Session["InvitationScheduledDate"] = _scheduledDate.DateTime;
            else
                Session["InvitationScheduledDate"] = null;
        }

        public void UpdateSchedule(InvitationActivityType mode)
        {
            try
            {
                DateTime? scheduled = Session["InvitationScheduledDate"] as DateTime?;
                scheduled = scheduled ?? DateTime.Now;

                //find existing schedule dates 
                Invitation invitation = InvitationManager.GetInvitation(InvitationId);
                var lastSchedule = invitation.Schedule.FirstOrDefault(s => s.Scheduled.HasValue 
                    && s.InvitationActivityType == mode 
                    && s.Scheduled.Value > scheduled);

                InvitationSchedule scheduleItem = lastSchedule ?? new InvitationSchedule
                                       {
                                           InvitationID = InvitationId,
                                           InvitationActivityType = mode,
                                           Scheduled = scheduled
                                       };

                if (AutoSend)
                    scheduleItem.Scheduled = scheduled;

                //avoid async errors
                if (EmailGateway.ProviderSupportsBatches)
                {
                    if (scheduleItem.Scheduled < DateTime.Now.AddMinutes(1))
                        scheduleItem.Scheduled = DateTime.Now.AddMinutes(1);
                }
                scheduleItem.Save(Context.User as CheckboxPrincipal);
                Session["InvitationScheduleItem"] = scheduleItem;

                ScheduleId = scheduleItem.InvitationScheduleID.Value;

                if (lastSchedule == null)
                    invitation.AddScheduleItem(scheduleItem);
                else if (scheduleItem.BatchID.HasValue)
                    EmailGateway.SetMessageBatchDate(scheduleItem.BatchID.Value, scheduleItem.Scheduled.Value);

                //don't create batch in SMTP mode
                if (EmailGateway.ProviderSupportsBatches)
                {
                    InvitationSender.Send(invitation, scheduleItem, null, ApplicationManager.ApplicationDataContext);
                    _successPanel.Visible = true;
                    _msgDivSuccess.InnerText = WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/send.ascx/scheduleItemAdded");
                    _immediate.Checked = true;
                }
            }
            catch (Exception ex)
            {
                _errorPanel.Visible = true;
                _msgDivError.InnerText = string.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/send.ascx/scheduleItemAdditionFailed"), ex.Message);
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _remindBtn_Click(object sender, System.EventArgs e)
        {
            if (!AutoSend)
            {
                saveScheduledDate();
            }
            if (OnRemindClick != null)
            {
                OnRemindClick();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _inviteBtn_Click(object sender, System.EventArgs e)
        {
            //show close button
            _closeWizardContainer.Visible = true;

            if (!AutoSend)
            {
                saveScheduledDate();
            }
            UpdateSchedule(InvitationActivityType.Invitation);            
            if (!EmailGateway.ProviderSupportsBatches && !_schedule.Checked)
            {
                StartProgress("invite");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void StartProgress(string mode)
        {
            _sendPanel.Visible = true;
            _inviteBtn.Visible = false;
            _remindBtn.Visible = false;

            var progressKey = string.Format("Invitation_{0}_{1}", mode, InvitationId);

            //Seed progress cache so it's not empty on first request to get progress
            ProgressProvider.SetProgress(
                progressKey,
                WebTextManager.GetText("/pageText/batchSendSummary.aspx/preparing"),
                string.Empty,
                ProgressStatus.Pending,
                0,
                100);

            //Start export and progress monitoring
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "ProgressStart",
                "$(document).ready(function(){startProgress('" + progressKey + "', '" + mode + "');});",
                true);
        }
    }
}