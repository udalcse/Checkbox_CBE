using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Invite : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _inviteAdditional.Initialize(Invitation);

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText("/pageText/forms/surveys/invitations/invite.aspx/pageTitle"), Utilities.StripHtml(Invitation.Name, 64)));

            Master.HideDialogButtons();
        }
    }
}