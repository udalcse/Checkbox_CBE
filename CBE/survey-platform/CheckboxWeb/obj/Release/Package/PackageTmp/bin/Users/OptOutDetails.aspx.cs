using System;
using Checkbox.Wcf.Services;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Users
{
    public partial class OptOutDetails : SecuredPage
    {
        [QueryParameter("s")]
        public int SurveyId { get; set; }

        [QueryParameter("name")]
        public string SurveyName { get; set; }

        [QueryParameter("email")]
        public string Email { get; set; }

        public DateTime Date { set; get; }

        public string UserComment { set; get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Master.OkVisible = false;
            Master.CancelTextId = "/common/close";

            InitData();
        }

        private void InitData()
        {
            var data = InvitationManagementServiceImplementation.GetEmailOptOutDetails(
                CurrentPrincipal, Email, SurveyId, 0);

            Date = data.Date;
            SurveyName = data.ResponseTemplateName;
            UserComment = data.UserComment;
        }
    }
}