using Checkbox.Web.Page;
using Checkbox.Invitations;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public abstract class InvitationPropertiesPage : SecuredPage, IStatusPage
    {
        private Invitation _invitation;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Create the page title with the invitation name and survey name as a link
            var titlePlace = new PlaceHolder();
            var invitationLabel = new Label();
            var surveyLink = new HyperLink();
            invitationLabel.Text = " - " + Invitation.Name;
            surveyLink.NavigateUrl = ResolveUrl("~/Forms/Surveys/Survey.aspx") + "?s=" + Invitation.ParentID;
            surveyLink.Text = Utilities.StripHtml(ResponseTemplateManager.GetLightweightResponseTemplate(Invitation.ParentID).Name, null);

            titlePlace.Controls.Add(surveyLink);
            titlePlace.Controls.Add(invitationLabel);


            if (Page.Master is BaseMasterPage)
            {
                ((BaseMasterPage)Page.Master).SetTitleControl(titlePlace);
            }

            if (SendLink == null)
            {
                return;
            }

            //Determine if the send link should send the invitation or a reminder
            if (Invitation.LastSent.HasValue)
            {
                SendLink.Text = WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/sendReminder");
                SendLink.NavigateUrl = ResolveUrl("~/Forms/Surveys/Invitations/SendReminder.aspx");
            }
            else
            {
                if (Invitation.GetRecipients(RecipientFilter.Pending).Count > 0)
                {
                    ShowStatusMessage(WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/messageNotSent"), StatusMessageType.Warning);
                    SendLink.Text = WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/sendInvite");
                    SendLink.NavigateUrl = ResolveUrl("~/Forms/Surveys/Invitations/Progress.aspx") + "?mode=invite";
                }
                else
                {
                    ShowStatusMessage(WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/noRecipients"), StatusMessageType.Warning);
                    SendLink.Visible = false;
                }
            }
        }


        #region Properties

        /// <summary>
        /// Form administer permission
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Administer"; } }

        /// <summary>
        /// Get the current invitation
        /// </summary>
        public Invitation Invitation
        {
            get
            {
                if (_invitation == null && IsPostBack)
                {
                    _invitation = GetSessionValue<Invitation>("CurrentInvitation", false, null);
                }

                if (_invitation == null
                    && !string.IsNullOrEmpty(Request.QueryString["i"]))
                {
                    _invitation = InvitationManager.GetInvitation(int.Parse(Request.QueryString["i"]));
                    Session["CurrentInvitation"] = _invitation;
                }

                if (_invitation == null)
                {
                    Response.Redirect(UserDefaultRedirectUrl);
                }

                return _invitation;
            }

            protected set
            {
                _invitation = value;
                Session["CurrentInvitation"] = value;
            }
        }

        protected virtual HyperLink SendLink { get; set; }

        #endregion

        #region IStatusPage Members

        public virtual void WireStatusControl(Control sourceControl) { }

        public virtual void WireUndoControl(Control sourceControl) { }

        public virtual void ShowStatusMessage(string message, StatusMessageType messageType) { }

        public virtual void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument) { }

        #endregion
    }
}
