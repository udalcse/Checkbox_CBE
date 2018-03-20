using System;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Messaging.Email;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Recipients : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("m")]
        public string Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText("/pageText/invitations/recipients.aspx/title"), Utilities.StripHtml(Invitation.Name, 64)));

            _recipients.Mode = Mode;

            if ("pending".Equals(Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                _recipients.ReturnUrl = ResolveUrl("~/Forms/Surveys/Invitations/SendToPending.aspx?i=" + Invitation.ID);
            }

            if ("notResponded".Equals(Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                if (EmailGateway.ProviderSupportsBatches)
                    _recipients.ReturnUrl = ResolveUrl("~/Forms/Surveys/Invitations/SendScheduledReminder.aspx?i=" + Invitation.ID + "&isReturn=true");
                else
                    _recipients.ReturnUrl = ResolveUrl("~/Forms/Surveys/Invitations/SendReminder.aspx?i=" + Invitation.ID + "&isReturn=true");
            }

            _recipients.Initialize(Invitation);

            Master.PreventEnterKeyBinding();
            Master.HideDialogButtons();
        }
    }
}
