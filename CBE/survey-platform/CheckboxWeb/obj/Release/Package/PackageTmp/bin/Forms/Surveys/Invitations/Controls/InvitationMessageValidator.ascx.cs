using Checkbox.Globalization.Text;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    public partial class InvitationMessageValidator : UserControlBase
    {
        /// <summary>
        /// Determine if user confirm creating the empty item
        /// </summary>
        public bool IsConfirmed
        {
            get
            {
                bool temp;
                return bool.TryParse(_confirmed.Value, out temp) && temp;
            }
            set { _confirmed.Value = value.ToString(); }
        }

        public string NextButtonId { set; get; }

        public bool ValidateInvitationText(string invitationText)
        {
            if (invitationText.Contains("SURVEY_URL_PLACEHOLDER__DO_NOT_ERASE") || IsConfirmed)
                return true;
            return false;
        }
    }
}