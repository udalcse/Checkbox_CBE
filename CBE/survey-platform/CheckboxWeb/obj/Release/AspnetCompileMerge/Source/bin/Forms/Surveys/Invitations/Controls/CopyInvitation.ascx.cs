using System.Web.UI;
using Checkbox.Invitations;
using Checkbox.Security.Principal;
using System;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CopyInvitation : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected int InvitationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation)
        {
            InvitationId = invitation.ID.Value;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(_invitationName.Text))
            {
                _invitationName.Text = string.Format("Invitation #{0} Copy", InvitationId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _copyBtn_Click(object sender, System.EventArgs e)
        {
            try
            {
                Invitation res = InvitationManager.CopyInvitation(InvitationId, _invitationName.Text, _useDefaultText.Checked, _copyRecipientsList.Checked, Context.User as CheckboxPrincipal);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "setCopyStatus", 
                    "<script>$(document).ready(function() {setCopyStatus('Info', '" + 
                      string.Format(WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/copied"), res.Name) + "');});</script>");

                Response.Redirect(ResolveUrl("~/Forms/Surveys/Invitations/InvitationSummary.aspx?i=") + res.ID);
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "setCopyStatus", 
                    "<script>$(document).ready(function() {setCopyStatus('Error', '" + 
                    WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/copyingFailed") + "');});</script>");
            }
        }
    }
}