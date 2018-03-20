using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Invitations;
using Checkbox.Web;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class InvitationSummary : InvitationPropertiesPage
    {
        private Invitation _currentInvitation;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();                    

            Master.OkClick += new System.EventHandler(Master_OkClick);

            //Set title
            Master.Title = string.Format("{0} - {1}", WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/viewInvitation"), Utilities.StripHtml(Invitation.Name, 64));

            _updateBtn.Click += new System.EventHandler(_updateBtn_Click);

            _recipientEditor.Initialize(Invitation);
            if (!GetCurrentInvitation().InvitationLocked)
            {
                _addRecipients.Initialize(Invitation);
            }
            else
            {
                _addRecipients.Visible = false;
            }
            if (!GetCurrentInvitation().LastSent.HasValue)
            {
                _messageEditor.Initialize(Invitation);
                _propertiesEditor.Initialize(Invitation);
                _sendInvitation.Initialize(Invitation);
            }
            _schedule.Initialize(Invitation);
            _copyInvitation.Initialize(Invitation);
            _sendInvitation.OnRemindClick += new Invitations.Controls.Send.RemindDelegate(_sendInvitation_OnRemindClick);
        }

        /// <summary>
        /// Redirect to the send reminder page
        /// </summary>
        void _sendInvitation_OnRemindClick()
        {
            if(!Checkbox.Messaging.Email.EmailGateway.ProviderSupportsBatches)
                Response.Redirect("SendReminder.aspx?i=" + Invitation.ID);
            else
                Response.Redirect("SendScheduledReminder.aspx?i=" + Invitation.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            //If this is an "Edit smth" tab - show "Save" and "Cancel" buttons, another - hide.
            if (!Invitation.InvitationLocked && (_currentTabIndex.Text == "2" || _currentTabIndex.Text == "3" || _currentTabIndex.Text == "4"))
                Master.ShowDialogButtons();
            else
                Master.HideDialogButtons();   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, System.EventArgs e)
        {
            //reload invitation
            Invitation = GetCurrentInvitation();
            if (!GetCurrentInvitation().InvitationLocked)
            {
                _addRecipients.Initialize(Invitation);
            }
            else
            {
                _addRecipients.Visible = false;
            }
            if (!GetCurrentInvitation().LastSent.HasValue)
            {
                _messageEditor.UpdateInvitation(Invitation);
                _propertiesEditor.UpdateInvitation(Invitation);
                _sendInvitation.Initialize(Invitation);
            }

            Invitation.Save(Context.User as CheckboxPrincipal);

            Master.CloseDialog(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _updateBtn_Click(object sender, System.EventArgs e)
        {
            //reload invitation
            Invitation = GetCurrentInvitation();
            if (!GetCurrentInvitation().InvitationLocked)
            {
                _addRecipients.Initialize(Invitation);
            }
            if (!GetCurrentInvitation().LastSent.HasValue)
            {
                _messageEditor.Initialize(Invitation);
                _propertiesEditor.Initialize(Invitation);
                _sendInvitation.Initialize(Invitation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Invitation GetCurrentInvitation()
        {
            return _currentInvitation ?? (_currentInvitation = InvitationManager.GetInvitation(Invitation.ID.Value));
        }
    }
}