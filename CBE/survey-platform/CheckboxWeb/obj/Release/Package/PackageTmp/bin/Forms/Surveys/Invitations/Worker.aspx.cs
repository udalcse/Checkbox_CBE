using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Messaging.Email;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class Worker : ProgressReportingEnabledPage
    {
        private Invitation _invitation;

        /// <summary>
        /// Mode for the page, whether it is send, invite, etc.
        /// </summary>
        [QueryParameter("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("i")]
        public int InvitationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("s")]
        public int InvitationScheduleID { get; set; }

        /// <summary>
        /// Get key to use for progress tracking
        /// </summary>
        protected override string ProgressKey
        {
            get { return string.Format("Invitation_{0}_{1}", Mode, Invitation.ID); }
        }

        /// <summary>
        /// Get invitation to send
        /// </summary>
        private Invitation Invitation
        {
            get
            {
                if (_invitation == null
                    && InvitationId > 0)
                {
                    _invitation = InvitationManager.GetInvitation(InvitationId);
                }

                return _invitation;
            }
        }

        

        /// <summary>
        /// Start sending invitations
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Set pending status
            ProgressProvider.SetProgress(
                ProgressKey,
                WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationWorker.aspx/preparing"),
                string.Empty,
                ProgressStatus.Pending,
                0,
                100);

            //Check invitation
            if (Invitation == null)
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    "An error occurred while sending the invitation.",
                    "Invitation ID was not specified or invitation could not be loaded.",
                    ProgressStatus.Error,
                    0,
                    100);

                WriteResult(new {success = false, errorMessage = "Unable to load invitation to send."});
                return;
            }

            //Do work
            SendInvitations();
        }

        private string _queuingString;
        private string QueuingString
        {
            get
            {
                if (string.IsNullOrEmpty(_queuingString))
                {
                    var languageCode = WebTextManager.GetUserLanguage();
                    _queuingString = TextManager.GetText("/pageText/batchSendSummary.aspx/queueingText", languageCode);
                }
                return _queuingString;
            }
        }

        /// <summary>
        /// Worker thread for queueing/sending the invitations
        /// </summary>
        public void SendInvitations()
        {
            try
            {
                //Set pending status
                ProgressProvider.SetProgress(
                    ProgressKey,
                    string.Empty,
                    string.Empty,
                    ProgressStatus.Pending,
                    0,
                    0);

                if (Invitation == null)
                    return;

                InvitationSchedule s = Session["InvitationScheduleItem"] as InvitationSchedule;//  (from i in Invitation.Schedule where i.InvitationScheduleID == InvitationScheduleID select i).FirstOrDefault();
                if (s != null)
                {
                    Session.Remove("InvitationScheduleItem");
                    InvitationSender.Send(Invitation, s, new InvitationSender.MessageSentDelegate(OnMessageSent), ApplicationManager.ApplicationDataContext);
                    WriteResult(new { success = true });
                }

                //Set progress status
                ProgressProvider.SetProgress(
                    ProgressKey,
                    QueuingString,
                    string.Empty,
                    ProgressStatus.Completed,
                    100,
                    100);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                WriteResult(new { success = false, error = ex.Message });

                //Set progress to errr
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Error,
                        Message = "An error occurred while sending the invitations.",
                        ErrorMessage = ex.Message,
                        TotalItemCount = 100
                    }
                );
            }
        }

        /// <summary>
        /// Updates the progress
        /// </summary>
        /// <param name="r"></param>
        /// <param name="num"></param>
        private void OnMessageSent(Recipient r, int num, int count)
        {

            string queueMsg = QueuingString;

            if (Utilities.IsNotNullOrEmpty(QueuingString)
                && QueuingString.Contains("{0}")
                && QueuingString.Contains("{1}"))
            {
                queueMsg = string.Format(QueuingString, num + 1, count);
            }


            ProgressProvider.SetProgressCounter(
                            ProgressKey,
                            queueMsg,
                            num + 1,
                            count,
                            100,
                            100);
        }

    }
}