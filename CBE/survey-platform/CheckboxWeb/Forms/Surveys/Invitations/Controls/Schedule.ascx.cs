using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Schedule : Checkbox.Web.Common.UserControlBase
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
        /// Initialize by lightweight invitation
        /// </summary>
        /// <param name="invitationData"></param>
        public void Initialize(InvitationData invitationData)
        {
            InvitationId = invitationData.DatabaseId;
        }

    }
}