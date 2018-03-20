using System.Web.UI;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    public partial class InvitationList : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Id of survey to list invitations for
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        /// Callback for invitation selected
        /// </summary>
        public string InvitationSelectedClientCallback { get; set; }
    }
}