using System;
using System.Text.RegularExpressions;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Messaging.Email;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Management;
using System.Collections.Generic;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EditMessageControl : Checkbox.Web.Common.UserControlBase
    {
        private bool _isOptOutRequired;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _emailFormatErrorLbl.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string MessageText
        {
            get { return _messageBodyText.Text; }
        }

        ///
        public bool HideFromFields
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation)
        {
            if (invitation.InvitationLocked)
            {
                _fromTxt.Enabled = false;
                _subjectTxt.Enabled = false;
                _fromNameTxt.Enabled = false;
                _formatTxt.Enabled = false;
                _messageBodyText.Enabled = false;

                CheckForInvitationErrors(invitation, true);
            }

            _isOptOutRequired = invitation.Template.IncludeOptOutLink;
            _fromTxt.Text = invitation.Template.FromAddress;
            _subjectTxt.Text = invitation.Template.Subject;
            _fromNameTxt.Text = invitation.Template.FromName;
            _formatTxt.Text = invitation.Template.Format == MailFormat.Html ? "Html" : "Text";

            _defaultFooter.Value = FooterText;
            _optoutLink.Value = UnsubscriptionLinkText;

            var text = WebUtilities.StripAppRelativePathFromUrl(
                invitation.Template.Body,
                Page.AppRelativeTemplateSourceDirectory);

            if (!_isOptOutRequired)
                text = StripUnsubscriptionLink(text);
            else if (!ApplicationManager.AppSettings.FooterEnabled &&
                !text.Contains(InvitationManager.OPT_OUT_URL_PLACEHOLDER))
            {
                if (invitation.Template.Format == MailFormat.Html)
                    text += "<br/><br/>";
                else
                    text += Environment.NewLine + Environment.NewLine;

                text += UnsubscriptionLinkText;                
            }

            _messageBodyText.Text = text;

            _subjectPipeSelector.Initialize(
                null,
                null,
                WebTextManager.GetUserLanguage(),
                _subjectTxt.ClientID);

            _bodyPipeSelector.Initialize(
                null,
                null,
                WebTextManager.GetUserLanguage(),
                _messageBodyText.ClientID);

            _setInvitationText.Visible = false;
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="invitation"></param>
        public void InitializeReminder(Invitation invitation)
        {
            _isOptOutRequired = invitation.Template.IncludeOptOutLink;
            _fromTxt.Text = invitation.Template.FromAddress;
            _subjectTxt.Text = string.IsNullOrEmpty(invitation.Template.ReminderSubject) ? invitation.Template.Subject : invitation.Template.ReminderSubject;
            _fromNameTxt.Text = invitation.Template.FromName;
            _formatTxt.Text = invitation.Template.Format == MailFormat.Html ? "Html" : "Text";

            _defaultFooter.Value = FooterText;
            _optoutLink.Value = UnsubscriptionLinkText;

            var text = WebUtilities.StripAppRelativePathFromUrl(
                string.IsNullOrEmpty(invitation.Template.ReminderBody) ? invitation.Template.Body : invitation.Template.ReminderBody,
                Page.AppRelativeTemplateSourceDirectory);

            if (!_isOptOutRequired)
                text = StripUnsubscriptionLink(text);

            _messageBodyText.Text = text;
            _invitationText.InnerHtml = invitation.Template.Body;

            _setInvitationText.Visible = true;

            _subjectPipeSelector.Initialize(
                null,
                null,
                WebTextManager.GetUserLanguage(),
                _subjectTxt.ClientID);

            _bodyPipeSelector.Initialize(
                null,
                null,
                WebTextManager.GetUserLanguage(),
                _messageBodyText.ClientID);
        }

        private string StripUnsubscriptionLink(string text)
        {
            var regex = new Regex("<a\\s([^>]*\\s)?href=\"" 
                + InvitationManager.OPT_OUT_URL_PLACEHOLDER + "\"(.*?)>(.*?)</a>");

            text = regex.Replace(text, string.Empty);
            text = text.Replace(InvitationManager.OPT_OUT_URL_PLACEHOLDER, string.Empty);

            return regex.Replace(text, string.Empty);
        }

        private CompanyProfile GetCompanyProfileForInvitation(Invitation invitation)
        {
            if (!invitation.CompanyProfileId.HasValue)
            {
                int? companyProfileId = null;

                if (Session["CurrentInvitationCompanyProfile"] != null)
                {
                    companyProfileId = (int)Session["CurrentInvitationCompanyProfile"];
                    Session.Remove("CurrentInvitationCompanyProfile");
                }
                else if (invitation.LastSent.HasValue || invitation.InvitationScheduled.HasValue)
                {
                    companyProfileId = CompanyProfileFacade.GetDefaultCompanyProfileId();
                }

                if (companyProfileId.HasValue)
                {
                    invitation.CompanyProfileId = companyProfileId.Value;
                    invitation.Save(UserManager.GetCurrentPrincipal());
                }
            }

            return invitation.GetCompanyProfile();
        }

        /// <summary>
        /// Determine if user confirmed creating the empty item
        /// </summary>
        public bool IsConfirmed
        {
            get
            {
                bool temp;
                return bool.TryParse(_confirmed.Value, out temp) && temp;
            }
            set { _confirmed.Value = value.ToString(); }
        }


        protected bool ValidateMessageText(bool moveNext, Invitation invitation)
        {
            List<CompanyProfile.Property> missedParams;
            var validationResult = ValidateInvitationText(out missedParams);

            switch (validationResult)
            {
                case InvitationEmailTextValidator.ErrorType.Success:
                    {
                        var profile = GetCompanyProfileForInvitation(invitation);
                        if (ApplicationManager.AppSettings.FooterEnabled && (profile == null|| !profile.IsValid))
                        {
                            if (IsConfirmed && !ApplicationManager.AppSettings.EnableMultiDatabase)
                                return true;
                            InjectRequiredPropertiesMissedJS(moveNext, invitation);
                            return false;
                        }
                        return true;
                    }
                case InvitationEmailTextValidator.ErrorType.SurveyURLMissed:
                    {
                        if (IsConfirmed)
                            return true;
                        InjectSurveyURLMissedJS(moveNext);
                    }
                    break;
                case InvitationEmailTextValidator.ErrorType.FooterMissed:
                    {
                        if (IsConfirmed && !ApplicationManager.AppSettings.EnableMultiDatabase)
                            return true;
                        InjectFooterMissedJS(missedParams, moveNext, invitation);
                    }
                    break;
                case InvitationEmailTextValidator.ErrorType.OptOutLinkMissed:
                    {
                        if (IsConfirmed && !ApplicationManager.AppSettings.EnableMultiDatabase)
                            return true;
                        InjectUnsubscribeLink(moveNext);
                    }
                    break;
            }

            return false;
        }

        InvitationEmailTextValidator.ErrorType ValidateInvitationText(out List<CompanyProfile.Property> missedParams)
        {
            string text = WebUtilities.StripAppRelativePathFromUrl(_messageBodyText.Text.Trim(), Page.AppRelativeTemplateSourceDirectory);
            return InvitationEmailTextValidator.ValidateInvitationText(text, out missedParams, _isOptOutRequired);
        }

        InvitationEmailTextValidator.ErrorType ValidateInvitationText()
        {
            List<CompanyProfile.Property> missedParams;
            return ValidateInvitationText(out missedParams);
        }

        /// <summary>
        /// Injects a script that shows a confirm message about the empty URL
        /// </summary>
        private void InjectRequiredPropertiesMissedJS(bool moveNext, Invitation invitation)
        {
            var action = ApplicationManager.AppSettings.EnableMultiDatabase ? "stopProcessing" : (moveNext ? "moveNext" : "moveBack");
            var message = TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/companyFieldsAreEmpty");
            var companyProfileId = invitation.CompanyProfileId.HasValue ? "," + invitation.CompanyProfileId : string.Empty;
            ShowConfirmMessageScript(message, "function(){ redirectToCompanyProfileSettings(" + moveNext.ToString().ToLower() + companyProfileId + "); }", action);
        }

        /// <summary>
        /// Injects a script that shows a confirm message about the empty URL
        /// </summary>
        private void InjectSurveyURLMissedJS(bool moveNext)
        {
            var message = TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/messageValidation");
            ShowConfirmMessageScript(message, moveNext ? "moveNext" : "moveBack", "stopProcessing");                
        }

        /// <summary>
        /// 
        /// </summary>
        protected string FooterText
        {
            get
            {
                var text = ApplicationManager.AppSettings.FooterText;

                if (_formatTxt.Text == "Html")
                    return text;

                text = text.Replace("<br>", "\\n").Replace("<br />", "\\n").Replace("<br/>", "\\n");
                var regex = new Regex("<a\\s([^>]*\\s)?href=\"" + InvitationManager.OPT_OUT_URL_PLACEHOLDER + "\"(.*?)>(.*?)</a>");
                text = regex.Replace(text, InvitationManager.OPT_OUT_URL_PLACEHOLDER);

                return Utilities.StripHtml(text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string UnsubscriptionLinkText
        {
            get
            {
                if (_formatTxt.Text == "Html")
                    return "===================================<br /><a id=\"unsubscribeLink\" href=\""
                        + InvitationManager.OPT_OUT_URL_PLACEHOLDER +"\">Unsubscribe from this list</a>";

                return InvitationManager.OPT_OUT_URL_PLACEHOLDER;
            }
        }

        /// <summary>
        /// Injects a script that handles the situation with absent company footer
        /// </summary>
        private void InjectFooterMissedJS(IEnumerable<CompanyProfile.Property> missedCompanyParameters, bool moveNext, Invitation invitation)
        {
            var joinedParams = string.Join(", ",
            (from p in missedCompanyParameters
             select
                 WebTextManager.GetText(string.Format("/Forms/Surveys/Invitations/Controls/EditMessageControl/{0}", p))).ToArray());

            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                string message;

                var profile = GetCompanyProfileForInvitation(invitation);

                //check if all required company fields are nonempty
                if (profile != null && profile.IsValid)
                {
                    message = string.Format(TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/messageFooterRequired"), joinedParams);
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage",
                                                                "$(function() { ShowConfirmMessage(\"" +
                                                                message + "\", injectDefaultFooter, stopProcessing); });", true);
                }
                else
                {
                    string function;

                    var result = ValidateInvitationText();
                    bool isFooterMissed = result == InvitationEmailTextValidator.ErrorType.FooterMissed;
                    _isFooterInjected.Value = (!isFooterMissed).ToString().ToLower();

                    if (isFooterMissed)
                    {
                        function = "ShowFooterAndCompanyDialogs";
                        message = string.Format(TextManager.GetText(
                                    "/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/messageFooterRequired"), joinedParams);
                    }
                    else
                    {
                        function = "ShowCompanyDialog";
                        message = string.Format(TextManager.GetText(
                                    "/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/companyFieldsAreEmpty"));
                    }

                    var script = string.Format("$(function() {{ {0}(\"{1}\",{2}); }});", function, message, moveNext.ToString().ToLower());
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage", script, true);
                }
            }
            else if(ApplicationManager.AppSettings.FooterEnabled)
            {
                var message = string.Format(TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/messageFooterValidation"), joinedParams);
                ShowConfirmMessageScript(message, "injectDefaultFooter", moveNext ? "moveNext" : "moveBack");                
            }
        }

        private void ShowConfirmMessageScript(string message, string yesAction, string noAction)
        {
            var script = string.Format("$(function() {{ ShowConfirmMessage(\"{0}\", {1}, {2}); }});", message, yesAction, noAction);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage", script, true);
        }

        /// <summary>
        /// Injects a script that handles the situation with absent company footer
        /// </summary>
        private void InjectUnsubscribeLink(bool moveNext)
        {
            var action = ApplicationManager.AppSettings.EnableMultiDatabase ? "stopProcessing" : (moveNext ? "moveNext" : "moveBack");
            var message = TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/unsubscribeLinkRequired");
            ShowConfirmMessageScript(message, "injectDefaultUnsubscriptionLink", action);
        }

        /// <summary>
        /// Update invitation with values
        /// </summary>
        /// <param name="invitation"></param>
        /// <param name="moveNext"></param>
        public bool UpdateInvitation(Invitation invitation, bool moveNext = true)
        {
            if (!CheckForInvitationErrors(invitation, moveNext))
                return false;
            
            string text = WebUtilities.StripAppRelativePathFromUrl(
                    _messageBodyText.Text.Trim(),
                    Page.AppRelativeTemplateSourceDirectory);

            Page.Validate();

            if (Page.IsValid)
            {
                //Validate email
                var emailValidator = new EmailValidator();

                if (!emailValidator.Validate(_fromTxt.Text.Trim()))
                {
                    _emailFormatErrorLbl.Visible = true;
                    return false;
                }
                
                _emailFormatErrorLbl.Visible = false;
                invitation.Template.FromAddress = _fromTxt.Text.Trim();
                invitation.Template.FromName = _fromNameTxt.Text.Trim();
                invitation.Template.Subject = _subjectTxt.Text.Trim();

                //Browser edit mode adds full path to relative URLs and corrupts them.
                // Attempt to remove by eliminating current folder from path.

                invitation.Template.Body = text;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Update invitation with values
        /// </summary>
        /// <param name="invitation"></param>
        /// <param name="moveNext"></param>
        public bool UpdateReminder(Invitation invitation, bool moveNext = true)
        {
            if (!CheckForReminderErrors(invitation, moveNext))
                return false;

            Page.Validate();

            if (Page.IsValid)
            {
                //Validate email
                var emailValidator = new EmailValidator();

                if (!emailValidator.Validate(_fromTxt.Text.Trim()))
                {
                    _emailFormatErrorLbl.Visible = true;
                    return false;
                }
                
                _emailFormatErrorLbl.Visible = false;
                invitation.Template.FromAddress = _fromTxt.Text.Trim();
                invitation.Template.FromName = _fromNameTxt.Text.Trim();
                invitation.Template.ReminderSubject = _subjectTxt.Text.Trim();

                //Browser edit mode adds full path to relative URLs and corrupts them.
                // Attempt to remove by eliminating current folder from path.

                invitation.Template.ReminderBody = WebUtilities.StripAppRelativePathFromUrl(
                    _messageBodyText.Text.Trim(),
                    Page.AppRelativeTemplateSourceDirectory);

                return true;
            }

            return false;
        }

        private bool CheckForInvitationErrors(Invitation invitation, bool moveNext)
        {
            if (invitation.InvitationLocked)
            {
                int minutes = invitation.NextInitationDispatchInMinutes;

                _statusControl.Message = TextManager.GetText(minutes > 0 ?
                    "/pageText/message.aspx/cannotEditInvitation1" : "/pageText/message.aspx/cannotEditInvitation2");

                _statusControl.MessageType = StatusMessageType.Warning;
                _statusControl.ShowStatus();

                return false;
            }

            if (!ValidateMessageText(moveNext, invitation))
                return false;

            return true;
        }

        private bool CheckForReminderErrors(Invitation invitation, bool moveNext)
        {
            if (invitation.ReminderLocked)
            {
                _statusControl.Message = TextManager.GetText("/pageText/message.aspx/cannotEditReminder");

                _statusControl.MessageType = StatusMessageType.Error;
                _statusControl.ShowStatus();

                return false;
            }

            if (!ValidateMessageText(moveNext, invitation))
                return false;

            return true;
        }
    }
}