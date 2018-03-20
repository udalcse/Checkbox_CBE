using System;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Invitations;
using CheckboxWeb.Forms.Surveys.Invitations.Controls;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public class InvitationWizardPage : SecuredPage
    {
        private Invitation _invitation;

        #region Properties

        [QueryParameter("s")]
        public int SurveyId { get; set; }

        [QueryParameter("i")]
        public int? InvitationId { get; set; }

        /// <summary>
        /// Get the current invitation
        /// </summary>
        protected Invitation Invitation
        {
            get
            {
                if(_invitation == null && InvitationId.HasValue && InvitationId.Value > 0)
                {
                    _invitation = InvitationManager.GetInvitation(InvitationId.Value);
                }

                return _invitation;
            }
        }

        #endregion

        protected bool ValidateInvitationText()
        {
            if (Validator == null || Validator.IsConfirmed || (Validator != null && Validator.ValidateInvitationText(Invitation.Template.Body)))
                return true;

            var message = TextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/messageValidation");

            ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage",
                                                        "$(function() { ShowConfirmMessage(\"" +
                                                        message + "\"); });", true);
            return false;
        }

        protected InvitationMessageValidator Validator { set; get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Validator = FindFirstChildControl<InvitationMessageValidator>();
        }
    }
}
