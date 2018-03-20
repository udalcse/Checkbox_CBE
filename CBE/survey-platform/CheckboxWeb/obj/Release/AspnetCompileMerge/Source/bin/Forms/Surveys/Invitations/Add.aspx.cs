using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Security.Principal;
using Checkbox.Pagination;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Add : InvitationWizardPage
    {
        private ResponseTemplate _responseTemplate;


        /// <summary>
        /// Get current response template
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Form.Administer"; }
        }

        public bool IsPrepMode {
            get { return ApplicationManager.AppSettings.IsPrepMode; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int ScheduleId
        {
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
        /// 
        /// </summary>
        protected bool? IsBlankEmail
        {
            set { HttpContext.Current.Session["_IsBlankEmail"] = value; }
            get { return (bool?)HttpContext.Current.Session["_IsBlankEmail"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int ExistingInvitationValue
        {
            set { Session["_existingInvitationSourceId"] = value; }
            get { return Session["_existingInvitationSourceId"] == null ? -1 : (int)Session["_existingInvitationSourceId"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();
            Master.SetTitle("Share via Email Invitation");

            _invitationWizard.NextButtonClick += InvitationWizard_NextButtonClick;
            _invitationWizard.FinishButtonClick += InvitationWizard_FinishButtonClick;
            _invitationWizard.CancelButtonClick += InvitationWizard_CancelButtonClick;
            _invitationWizard.PreviousButtonClick += InvitationWizard_PreviousButtonClick;


            _messageReviewbutton.NavigateUrl = "javascript:showDialog('MessageReview.aspx?i=" + InvitationId + "', '_reviewMessageWindow');";
            _recipientReviewLink.NavigateUrl = "javascript:showDialog('RecipientReview.aspx?i=" + InvitationId + "', '_reviewRecipientsWindow');";

            if (ApplicationManager.AppSettings.EnableMultiDatabase || ApplicationManager.AppSettings.FooterEnabled)
            {
                var profiles = CompanyProfileFacade.ListProfiles();
                if (profiles.Count > 1)
                {
                    _selectProfilePanel.Visible = true;
                    FillProfilesDropdown(profiles);                    
                }
            }

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _invitationWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/forms/surveys/invitations/add.aspx/wizardStepTitle/{0}", step.ID));
            }

            //Initialize message editor
            if (Invitation != null)
            {
                _editMessage.Initialize(Invitation);
            }
            else
            {
                IsBlankEmail = null;
                ExistingInvitationValue = -1;
            }

            //opt out settings should be visible only for server users
            if(ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                HideOptOutSettings();
            }

            
            if (ApplicationManager.AppSettings.IsPrepMode)
            {
                //_emailAdresses.Visible = false;
                //_groups.Visible = false;
                //_emailLists.Visible = false;
                _addEmailTab.Visible = false;
                _addGroupsTab.Visible = false;
                _addEmailListsTab.Visible = false;
            }

            //Bind the other invitations drop down
            if (!Page.IsPostBack)
            {
                var paginationContext = new PaginationContext
                {
                    CurrentPage = 1,
                    PageSize = -1,
                    Permissions = new List<string> { "Form.Administer" },
                    FilterField = String.Empty,
                    FilterValue = String.Empty,
                    SortField = "Name",
                    SortAscending = true
                };

                _existingInvitationSource.DataTextField = "Name";
                _existingInvitationSource.DataValueField = "DatabaseId";
                _existingInvitationSource.DataSource =
                    InvitationManager
                        .ListLightWeightInvitationDetailsForSurvey(HttpContext.Current.User as CheckboxPrincipal, SurveyId, paginationContext)
                        .Where(invitation => Invitation == null || invitation.DatabaseId != Invitation.ID);
                _existingInvitationSource.DataBind();

                if (ExistingInvitationValue >= 0)
                    _existingInvitationSource.SelectedValue = ExistingInvitationValue.ToString();
                
                if (_existingInvitationSource.Items.Count == 0)
                {
                    _invitationSourcePlace.Visible = false;
                    _newInvitationRad.Checked = true;
                    _copyInvitationRad.Checked = false;
                    _existingInvitationSource.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/noExistingInvitations")));
                }

                //If redirected from the wizard itself, set appropriate tab
                if(InvitationId.HasValue)
                {
                    _invitationWizard.ActiveStepIndex = 1;
                }
            }
        }

        private void FillProfilesDropdown(Dictionary<int, string> profiles)
        {
            foreach (var profile in profiles)
            {
                _companyProfileList.Items.Add(new ListItem(profile.Value, profile.Key.ToString()));
            }

            var defaultProfileId = CompanyProfileFacade.GetDefaultCompanyProfileId();
            if (defaultProfileId.HasValue)
                _companyProfileList.SelectedValue = defaultProfileId.ToString();
            else
                _companyProfileList.SelectedIndex = _companyProfileList.Items.Count - 1;
        }

        private void HideOptOutSettings()
        {
            _optOutChk.Visible = false;
            _optOutLbl.Visible = false;
        }

        /// <summary>
        /// Return true if the invitation has respondents
        /// </summary>
        protected bool HasRecipients
        {
            get
            {
                try
                {
                    if (!InvitationId.HasValue)
                        return false;
                    var inv = InvitationManager.GetInvitation(InvitationId.Value);
                    return inv.GetRecipients(RecipientFilter.Pending).Count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Bind events to store cursor position for pipe insertion
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

           

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            if (Invitation != null)
            {
                _recipientEditor.Initialize(Invitation);
                _addRecipients.Initialize(Invitation);
                _addRecipients.ViewRecipientListCallback = "showPendingRecipientsView";

                //If we redirected to this page, reset control inputs on first page
                if (!Page.IsPostBack)
                {
                    _name.Text = Invitation.Name;
                    _formatHtmlRad.Checked = Invitation.Template.Format == MailFormat.Html;
                    _formatTextRad.Checked = !_formatHtmlRad.Checked;
                    _autoLoginChk.Checked = Invitation.Template.LoginOption == LoginOption.Auto;

                    if (!ApplicationManager.AppSettings.EnableMultiDatabase)
                    {
                        _optOutChk.Checked = Invitation.Template.IncludeOptOutLink;
                    }

                    _newInvitationRad.Checked = IsBlankEmail.HasValue && IsBlankEmail.Value;
                    _copyInvitationRad.Checked = !_newInvitationRad.Checked; 
                }
            }

            string reason;
            if (!ResponseTemplate.BehaviorSettings.GetIsActiveOnDate(DateTime.Now, out reason))
            {
                _activationWarning.Visible = true;
                _activationWarningLbl.Text = string.Format(WebTextManager.GetText("/pageText/reviewInvitation.aspx/activeWarning"), ResponseTemplate.Name);
            }
        }


        #region Wizard event handlers

        /// <summary>
        /// Handles the finish event of the wizard
        /// - Controls what options are available depending on the state of the invitation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            var count = Invitation.GetPendingRecipientsCount();

            String errorMsg;

            if (CheckForEmailsEnough(count, out errorMsg))
            {
                _completionInstructions.Visible = count > 0;
                _completionInstructionsNoRecipients.Visible = count == 0;
                _emailsNotEnoughToInviteWarningPanel.Visible = false;
                UpdateButtonsPanel(count > 0);
            }
            else
            {
                _completionInstructions.Visible = _completionInstructionsNoRecipients.Visible = false;
                _emailsNotEnoughToInviteWarning.Text = errorMsg;
                _emailsNotEnoughToInviteWarningPanel.Visible = true;
                UpdateButtonsPanel(false);
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
        protected void InvitationWizard_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (e.CurrentStepIndex >= 1)
            {
                if (e.CurrentStepIndex == 1)
                    UpdateMessage(e, false);

                _invitationWizard.ActiveStepIndex = e.CurrentStepIndex - 1;
            }
        }

        /// <summary>
        /// Handles the next button click of the wizard
        /// - Validates input at each step, saves intermediate data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            //Set up the message screen depending on the message type

            var currentStep = _invitationWizard.WizardSteps[e.CurrentStepIndex].ID;

            if ("StartStep".Equals(currentStep, StringComparison.InvariantCultureIgnoreCase))
            {

                //Create the invitation at this point so we can use it as the backing data store for all the properties 
                //and recipients we'll create and so that it's still available if the user quits the wizard
                var mailFormat = _formatHtmlRad.Checked ? MailFormat.Html : MailFormat.Text;

                LightweightResponseTemplate survey = ResponseTemplateManager.GetLightweightResponseTemplate(SurveyId);
                if (survey == null)
                {
                    ShowError("Wrong survey ID.", null);
                    return;
                }

                var invitation = 
                        Invitation 
                            ?? InvitationManager.CreateInvitationForSurvey(
                                survey,                                 
                                Context.User as CheckboxPrincipal);

                invitation.Name = _name.Text.Trim();
                invitation.Template.LoginOption = _autoLoginChk.Checked ? LoginOption.Auto : LoginOption.None;
                invitation.Template.IncludeOptOutLink = ApplicationManager.AppSettings.EnableMultiDatabase || _optOutChk.Checked;
                invitation.Save(Context.User as CheckboxPrincipal);

                //Set the default values for the text fields from either the invitation defaults or the template invitation if it exists
                if (_copyInvitationRad.Checked && (!IsBlankEmail.HasValue || IsBlankEmail.Value))
                {
                    IsBlankEmail = false;
                    ExistingInvitationValue = Convert.ToInt32(_existingInvitationSource.SelectedValue);

                    Invitation templateInvitation = InvitationManager.GetInvitation(ExistingInvitationValue);
                    invitation.Template.FromAddress = templateInvitation.Template.FromAddress;
                    invitation.Template.FromName = templateInvitation.Template.FromName;
                    invitation.Template.Subject = templateInvitation.Template.Subject;
                    invitation.Template.Body = templateInvitation.Template.Body;
                    invitation.Template.Format = templateInvitation.Template.Format;
                    invitation.Template.OptOutText = templateInvitation.Template.OptOutText;
                }
                else if (_newInvitationRad.Checked && (!IsBlankEmail.HasValue || !IsBlankEmail.Value || mailFormat != invitation.Template.Format))
                {
                    IsBlankEmail = true;
                    InvitationManager.PopulateInvitationWithDefaults(mailFormat, survey, invitation);
                }

                int profileId;
                if (_companyProfileList.Visible && int.TryParse(_companyProfileList.SelectedValue, out profileId))
                    invitation.CompanyProfileId = profileId;
                else if (ApplicationManager.AppSettings.EnableMultiDatabase || ApplicationManager.AppSettings.FooterEnabled)
                    invitation.CompanyProfileId = CompanyProfileFacade.GetDefaultCompanyProfileId();

                invitation.Save(Context.User as CheckboxPrincipal);

                //Now redirect back to page to put invitation ID in URL.  That will remove reliance on storing invitation in session as was done before
                // and hopefully make things more resistant to session timeout
                Response.Redirect("Add.aspx?s=" + SurveyId + "&i=" + invitation.ID + "&onClose=" + Request["onClose"], false);
                return;
            }

            //Text validation
            if ("TextStep".Equals(currentStep, StringComparison.InvariantCultureIgnoreCase))
            {
                UpdateMessage(e, true);
            }

            if ("RecipientStep".Equals(currentStep, StringComparison.InvariantCultureIgnoreCase))
            {
                //Set up the review panel
                _fromReview.Text = 
                    string.IsNullOrEmpty(Invitation.Template.FromName) 
                        ? Invitation.Template.FromAddress.Trim()
                        : Invitation.Template.FromName.Trim() + " &lt;" + Invitation.Template.FromAddress.Trim() + "&gt;";
                _subjectReview.Text = Invitation.Template.Subject;
                _messageTypeReview.Text =
                    Invitation.Template.Format == MailFormat.Html
                        ? WebTextManager.GetText("/controlText/htmlItemEditor/htmlMode")
                        : WebTextManager.GetText("/controlText/htmlItemEditor/textMode");

                if (!ApplicationManager.AppSettings.EnableMultiDatabase)
                {
                    _optOutReview.Text =
                        WebTextManager.GetText(_optOutChk.Checked
                                                   ? "/pageText/forms/surveys/invitations/add.aspx/optOutReviewOn"
                                                   : "/pageText/forms/surveys/invitations/add.aspx/optOutReviewOff");
                }

                _autoLoginReview.Text = WebTextManager.GetText(_autoLoginChk.Checked ? "/pageText/forms/surveys/invitations/add.aspx/autoLoginReviewOn" : "/pageText/forms/surveys/invitations/add.aspx/autoLoginReviewOff");

                _recipientCountReview.Text = String.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/recipientReviewCount"), Invitation.GetPendingRecipientsCount());

                _noRecipientsPanel.Visible = Invitation.GetPendingRecipientsCount() == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateMessage(WizardNavigationEventArgs e, bool moveNext)
        {
            //Update message & Save
            if (!_editMessage.UpdateInvitation(Invitation, moveNext) || !ValidateInvitationText())
            {
                e.Cancel = true;
                return;
            }

            Invitation.Save(Context.User as CheckboxPrincipal);
        }

        /// <summary>
        /// Handles the cancel event of the wizard;
        /// - redirects back to the user manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_CancelButtonClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Forms/Surveys/Invitations/Manage.aspx?s=" + SurveyId));
        }

        /// <summary>
        /// Handles the 'Close Window' event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseWindowButton_Click(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>
                           {
                               {"op", "createInvitation"},
                               {"invitationId", Invitation.ID.ToString()}
                           };

            Master.CloseDialog(args);
        }

        /// <summary>
        /// Handles the 'Edit Invitation' event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditInvitationButton_Click(object sender, EventArgs e)
        {
            if (Invitation != null && ResponseTemplate != null)
                Master.CloseDialog("openInvitation", new Dictionary<string, string>
                                                   {
                                                       { "surveyId", ResponseTemplate.ID.ToString() },
                                                       { "invitationId", Invitation.ID.ToString() }
                                                   });
        }

        #endregion

        #region Control event handlers

        /// <summary>
        /// Update design of buttons panel
        /// </summary>
        /// <param name="canSend"></param>
        private void UpdateButtonsPanel(bool canSend)
        {
            _sendNowButton.Visible = canSend;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendButton_Click(object sender, EventArgs e)
        {
            //design changing
            UpdateButtonsPanel(false);

            const string mode = "invite";
            _sendPanel.Visible = true;

            var progressKey = string.Format("Invitation_{0}_{1}", mode, Invitation.ID);

            var scheduleItem = new InvitationSchedule
                                   {
                                       InvitationID = Invitation.ID,
                                       InvitationActivityType = InvitationActivityType.Invitation,
                                       Scheduled = DateTime.Now
                                   };
            scheduleItem.Save(Context.User as CheckboxPrincipal);
            ScheduleId = scheduleItem.InvitationScheduleID.Value;

            //if invitation is just created then there also must be registered 'invitation_sent' event
            scheduleItem.Save(Context.User as CheckboxPrincipal);
            
            Invitation.AddScheduleItem(scheduleItem);
            Session["InvitationScheduleItem"] = scheduleItem;

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


        /// <summary>
        /// Send test invitation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendTestEmailButton_Click(object sender, EventArgs e)
        {
            try
            {
                Invitation.Test(_testEmail.Text, Context.User as CheckboxPrincipal);
                ClientScript.RegisterClientScriptBlock(GetType(), "setEmailStatus", "<script>$(document).ready(function() {setEmailStatus('Info', '" + WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/testInvitationHasBeenSent") + "');});</script>");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                ClientScript.RegisterClientScriptBlock(GetType(), "setEmailStatus", "<script>$(document).ready(function() {setEmailStatus('Error', '" + string.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/errorOccured"), ex.Message) + "');});</script>");
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
            var eventTarget = Checkbox.Web.UI.Controls.Adapters.WebControlAdapterExtender.MakeNameFromId(controlClientID);
            return eventTarget;
        }
    }
}
