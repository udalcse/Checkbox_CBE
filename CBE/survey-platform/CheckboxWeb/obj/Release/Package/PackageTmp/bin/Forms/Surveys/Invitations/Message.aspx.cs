using System;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Security.Principal;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Management;
using System.Text;
using Checkbox.Invitations;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Message : InvitationPropertiesPage
    {

        [QueryParameter("reminder")]
        public bool? Reminder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("scheduleEdit")]
        public bool ScheduleEdit { get; set; }

        private bool IsReminder 
        {
            get { return Reminder.HasValue && Reminder.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (IsReminder)
                _editMessage.InitializeReminder(Invitation);
            else
                _editMessage.Initialize(Invitation);

            Master.PreventEnterKeyBinding();

            Master.OkClick += Master_OkClick;

            if(ScheduleEdit)
            {
                Master.CancelClick += ReturnToScheduleEditList;
            }

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText(IsReminder ?
                "/pageText/forms/surveys/invitations/message.aspx/reminderTitle" :
                "/pageText/forms/surveys/invitations/message.aspx/pageTitle"), Utilities.StripHtml(Invitation.Name, 64)));
        }

        private void ReturnToScheduleEditList(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl(String.Format("~/Forms/Surveys/Invitations/Schedule.aspx?i={0}&scheduleEdit={1}", Invitation.ID, ScheduleEdit)));
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Master_OkClick(object sender, EventArgs e)
        {
            if (IsReminder ? _editMessage.UpdateReminder(Invitation) :
                             _editMessage.UpdateInvitation(Invitation))
            {
                Invitation.Save(Context.User as CheckboxPrincipal);

                ShowStatusMessage(
                    WebTextManager.GetText("/pageText/forms/surveys/invitations/properties.aspx/propertiesUpdated"),
                    StatusMessageType.Success);
                if (ScheduleEdit)
                {
                    Response.Redirect(ResolveUrl(String.Format("~/Forms/Surveys/Invitations/Schedule.aspx?i={0}&scheduleEdit={1}", Invitation.ID, ScheduleEdit)));
                }
                Master.CloseDialog("messageUpdated", false);
            }
        }
    }
}
