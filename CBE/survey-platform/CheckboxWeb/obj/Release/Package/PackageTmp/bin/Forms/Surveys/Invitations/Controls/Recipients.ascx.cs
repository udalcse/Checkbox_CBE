using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Recipients : Checkbox.Web.Common.UserControlBase
    {
        protected int InvitationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PendingMode { get; set; }

        /// <summary>
        /// URL to return to when return link is active
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PendingInvitationMode { get; set; }

        /// <summary>
        /// Get CSS for grid class
        /// </summary>
        public string GridCssClass
        {
            get { return _recipientsGrid.GridCssClass; }
            set { _recipientsGrid.GridCssClass = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            _returnLink.Visible = !string.IsNullOrEmpty(ReturnUrl);

            if (_returnLink.Visible)
            {
                _returnLink.NavigateUrl = ReturnUrl;
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (PendingMode == "Ungrouped")
            {
                var pendingItem = _filterList.Items.FindByValue("Pending");
                if (pendingItem != null)
                    pendingItem.Value = "PendingUngrouped";
            }
        }

        protected string PendingValue
        {
            get { return PendingMode == "Ungrouped"? "PendingUngrouped" : "Pending"; }
        }

        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation)
        {
            InvitationId = invitation.ID.Value;

            if(PendingInvitationMode)
            {
                _filterList.SelectedValue = PendingValue;
                _filterPanel.Style[HtmlTextWriterStyle.Display] = "none";
                return;
            }

            /*0012721: Item # 527 – Default Filter Selection for Invitation Recipients View*/
            /*When viewing recipients for an invitation, the recipient filter is always set to “Current” by default. For better usability, this should default to “Pending” if there are any “Pending” recipients. Otherwise, it should default to “Current.” */
            List<Recipient> pendingRecipients = invitation.GetRecipients(RecipientFilter.Pending);
            _filterList.SelectedValue = pendingRecipients.Count == 0 
                ? "Current"
                : PendingValue;

            if ("pending".Equals(Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                _filterList.SelectedValue = PendingValue;
                _filterPanel.Style["display"] = "none";
            }

            if ("notResponded".Equals(Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                _filterList.SelectedValue = "NotResponded";
                _filterPanel.Style["display"] = "none";
            }
        }
    }
}