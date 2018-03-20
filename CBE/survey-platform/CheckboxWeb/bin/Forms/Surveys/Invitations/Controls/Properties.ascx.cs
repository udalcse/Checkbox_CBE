using System.Web.UI;
using Checkbox.Invitations;
using Checkbox.Management;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Properties : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation)
        {
            if (invitation.InvitationLocked)
            {
                _optOutOn.Enabled = false;
                _optOutOff.Enabled = false;
                _autoLoginStatus.Enabled = false;
            }

            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                _optOutOn.Checked = invitation.Template.IncludeOptOutLink;
                _optOutOff.Checked = !_optOutOn.Checked;
            }
            
            _autoLoginStatus.Checked = invitation.Template.LoginOption == LoginOption.Auto;
        }

        /// <summary>
        /// Update invitation
        /// </summary>
        /// <param name="invitation"></param>
        public void UpdateInvitation(Invitation invitation)
        {
            if (invitation.InvitationLocked)
                return;

            invitation.Template.LoginOption = _autoLoginStatus.Checked
                ? LoginOption.Auto
                : LoginOption.None;

            invitation.Template.IncludeOptOutLink = ApplicationManager.AppSettings.EnableMultiDatabase || _optOutOn.Checked;
        }
    }
}