using System;
using System.Linq;
using Checkbox.Globalization;
using Checkbox.Invitations;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Security.Principal;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;
using System.Collections.Generic;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Schedule : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter]
        public bool IsReturn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("i")]
        public int InvitationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            InvitationData invitation = InvitationManager.GetInvitationData(int.Parse(Request["i"]));
            _scheduleView.Initialize(invitation);
            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/title");
            Master.OkVisible = false;
            Master.CancelVisible = false;
            Master.CancelTextId = "/common/close";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _addToSchedule.ServerClick += _addToSchedule_ServerClick;

            if (!IsPostBack)
            {
                _scheduledDate.DateTime = DateTime.Now.AddHours(1);
            }

            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();

            //Date Utils
            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));
        }

        void _addToSchedule_ServerClick(object sender, EventArgs e)
        {
            UpdateSchedule(InvitationActivityType.Reminder);
            
        }

        /// <summary>
        /// Validates reminder message text
        /// </summary>
        /// <param name="invitation"></param>
        /// <returns></returns>
        private bool ValidateReminderText(Invitation invitation)
        {
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                List<CompanyProfile.Property> missedParams;
                var validationResult = InvitationEmailTextValidator.ValidateInvitationText(invitation.Template.ReminderBody, out missedParams, invitation.Template.IncludeOptOutLink, false);

                if (validationResult == InvitationEmailTextValidator.ErrorType.OptOutLinkMissed)
                {
                    _msgDivError.InnerText = WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/unsubscribeLinkMustBeSpecified");
                    _successPanel.Visible = false;
                    _errorPanel.Visible = true;
                    return false;
                }
                
                if (validationResult == InvitationEmailTextValidator.ErrorType.FooterMissed)
                {
                    var joinedParams = string.Join(", ",
                    (from p in missedParams
                     select
                         WebTextManager.GetText(string.Format("/Forms/Surveys/Invitations/Controls/EditMessageControl/{0}", p.ToString()))).ToArray());

                    _msgDivError.InnerText = string.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/footerMustBeSpecified"), joinedParams);
                    _successPanel.Visible = false;
                    _errorPanel.Visible = true;
                    return false;
                }

                var profile = GetCompanyProfileForInvitation(invitation);
                if (profile == null || !profile.IsValid)
                {
                    _msgDivError.InnerText = string.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/companyFieldsMustBeSpecified"));
                    _successPanel.Visible = false;
                    _errorPanel.Visible = true;
                    return false;
                }
            }
            return true;
        }

        private CompanyProfile GetCompanyProfileForInvitation(Invitation invitation)
        {
            if (!invitation.CompanyProfileId.HasValue &&
                (invitation.LastSent.HasValue || invitation.InvitationScheduled.HasValue))
            {
                invitation.CompanyProfileId = CompanyProfileFacade.GetDefaultCompanyProfileId();
                invitation.Save(UserManager.GetCurrentPrincipal());
            }

            return invitation.GetCompanyProfile();
        }

        private void UpdateSchedule(InvitationActivityType mode)
        {
            try
            {
                Invitation invitation = InvitationManager.GetInvitation(InvitationId);
                if (!ValidateReminderText(invitation))
                {
                    return;
                }
                
                InvitationSchedule scheduleItem = new InvitationSchedule();
                scheduleItem.InvitationID = InvitationId;
                scheduleItem.InvitationActivityType = mode;
                scheduleItem.Scheduled = _scheduledDate.DateTime ?? DateTime.Now.AddHours(1);
                scheduleItem.Save(Context.User as CheckboxPrincipal);
                invitation.CheckForEmptyReminder();
                invitation.AddScheduleItem(scheduleItem);
                InvitationSender.Send(invitation, scheduleItem, null, ApplicationManager.ApplicationDataContext);

                _successPanel.Visible = true;
                _msgDivSuccess.InnerText = WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/send.ascx/scheduleItemAdded");
            }
            catch (Exception ex)
            {
                _errorPanel.Visible = true;
                _msgDivError.InnerText = string.Format(WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/send.ascx/scheduleItemAdditionFailed"), ex.Message);
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
            }
        }
    }
}
