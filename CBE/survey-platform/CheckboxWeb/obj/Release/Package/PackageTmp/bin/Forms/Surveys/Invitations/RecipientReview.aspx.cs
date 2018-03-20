using System;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Web;
using System.Linq;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class RecipientReview : InvitationWizardPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();
            Master.HideDialogButtons();
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/invitations/recipientReview.aspx/title"));

            _pendingRecipientsGrid.PageSize = ApplicationManager.AppSettings.PagingResultsPerPage;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            BindGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void BindGrid()
        {
            var recipients = Invitation.GetPendingRecipients(false);
            recipients.ForEach(r => r.UniqueIdentifier = Utilities.AdvancedHtmlDecode(r.UniqueIdentifier));

            _pendingRecipientsGrid.DataSource =
                recipients
                    .Skip((_pendingRecipientsGrid.PageIndex - 1)*_pendingRecipientsGrid.PageSize)
                    .Take(_pendingRecipientsGrid.PageSize)
                    .ToList();

            _pendingRecipientsGrid.DataBind();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            Master.CloseDialog(null);
        }
    }
}
