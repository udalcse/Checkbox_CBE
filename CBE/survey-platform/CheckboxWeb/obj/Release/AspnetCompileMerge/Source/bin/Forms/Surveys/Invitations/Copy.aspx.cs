using System;
using System.Collections.Generic;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Security.Principal;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class Copy : SecuredPage
    {
        [QueryParameter("i")]
        public int InvitationId { get; set; }

        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        private Invitation _invitation;
        private ResponseTemplate _responseTemplate;

        public Invitation Invitation
        {
            get
            {
                return _invitation ?? (_invitation = InvitationManager.GetInvitation(InvitationId));
            }
        }

        public ResponseTemplate ResponseTemplate
        {
            get
            {
                return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText(
                "/pageText/forms/surveys/invitations/controls/copy.aspx/copy"), Invitation != null ? Invitation.Name : string.Empty));

            if (string.IsNullOrEmpty(_invitationName.Text))
                _invitationName.Text = string.Format("Invitation #{0} Copy", InvitationId);

            Master.SetTitle(WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/copyInvitation"));
            Master.OkClick += Master_OkClick;
        }

        public void Master_OkClick(object sender, EventArgs e)
        {
            Invitation res = InvitationManager.CopyInvitation(InvitationId, _invitationName.Text, _useDefaultText.Checked, _copyRecipientsList.Checked, Context.User as CheckboxPrincipal);
            if (res != null)
            {
                var args = new Dictionary<string, string>
                           {
                               {"op", "copyInvitation"},
                               {"invitationId", res.ID.ToString()},
                               {"surveyId", ResponseTemplateId.ToString()}
                           };

                Master.CloseDialog(args);
            }
        }
    }
}