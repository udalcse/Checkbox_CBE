using System;
using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ReminderDetails : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("scheduleEdit")]
        public bool ScheduleEdit { get; set; }

        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/invitations/responses.aspx/title") + " - " + Utilities.StripHtml(Invitation.Name, 64));

            _responsesGrid.InitialSortField = "UniqueIdentifier";
            _responsesGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responsesGridListTemplate.html");
            _responsesGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responsesGridListItemTemplate.html");
            _responsesGrid.LoadDataCallback = "loadResponseList";
            _responsesGrid.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/invitations/responses.aspx/noResponses");
            _responsesGrid.ResultsPerPage = 10;

            _backButton.Text = WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/returnToSend");
            _backButton.Click += _closeButton_Click;

            LoadDatePickerLocalized();

            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
               "svcInvitationManagement.js",
               ResolveUrl("~/Services/js/svcInvitationManagement.js"));

            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));
            
            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));
        }

        private void _closeButton_Click(object sender, System.EventArgs e)
        {
            if (ScheduleEdit)
            {
                Response.Redirect(ResolveUrl(String.Format("~/Forms/Surveys/Invitations/Schedule.aspx?i={0}&scheduleEdit={1}", Invitation.ID, ScheduleEdit)));
            }
            else
            {
                Response.Redirect(ResolveUrl("~/Forms/Surveys/Invitations/InvitationSummary.aspx?i=") + Invitation.ID);
            }
        }
    }
}