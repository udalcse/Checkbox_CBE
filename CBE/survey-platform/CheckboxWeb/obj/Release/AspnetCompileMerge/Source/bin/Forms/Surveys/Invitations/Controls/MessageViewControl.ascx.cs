using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Invitations;
using System.Text;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    public partial class MessageViewControl : Checkbox.Web.Common.UserControlBase
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Invitation currentInvitation = ((InvitationPropertiesPage)Page).Invitation;

            StringBuilder from = new StringBuilder();
            from.Append(currentInvitation.Template.FromName);
            from.Append(" <");
            from.Append(currentInvitation.Template.FromAddress);
            from.Append(">");

            _fromName.Text = from.ToString();

            _subject.Text = currentInvitation.Template.Subject;

            _messageBodyContents.Text = currentInvitation.Template.Body;

        }
    }
}