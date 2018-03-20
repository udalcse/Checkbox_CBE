using System.Web.UI;
using Checkbox.Common;
using Checkbox.Invitations;
using Checkbox.Progress;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class Progress : InvitationPropertiesPage
    {
        /// <summary>
        /// Mode for the page, whether it is send, invite, etc.
        /// </summary>
        [QueryParameter("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// Get key to use for progress tracking
        /// </summary>
        protected override string ProgressKey
        {
            get { return string.Format("Invitation_{0}_{1}", Mode, Invitation.ID); }
        }

        /// <summary>
        /// Start the process when page is loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (Utilities.IsNotNullOrEmpty(Request.QueryString["complete"]))
            {
                //reload invitation
                Session["CurrentInvitation"] = InvitationManager.GetInvitation(Invitation.ID.Value);

                //Show return links
                _returnDiv.Style[HtmlTextWriterStyle.Display] = "block";
                _queuingCompletedLbl.Style[HtmlTextWriterStyle.Display] = "inline";
                _queueingLbl.Visible = false;


                return;
            }

            //Seed progress cache so it's not empty on first request to get progress
            SetProgressStatus(
                ProgressStatus.Pending,
                0,
                100,
                WebTextManager.GetText("/pageText/batchSendSummary.aspx/preparing"));

            //Start export and progress monitoring
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "ProgressStart",
                "$(document).ready(function(){startProgress('" + ProgressKey + "');});",
                true);
        }
    }
}
