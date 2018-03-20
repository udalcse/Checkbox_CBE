using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Security.Principal;
using Checkbox.Pagination;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddUsers : InvitationWizardPage
    {
        private ResponseTemplate _responseTemplate;


        /// <summary>
        /// Get current response template
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Form.Administer"; }
        }



        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();
            Master.SetTitle("Generate links for users");
        }

        /// <summary>
        /// Bind events to store cursor position for pipe insertion
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            _addRecipients.Initialize(null, SurveyId);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }


        /// <summary>
        /// Handles the finish event of the wizard
        /// - Controls what options are available depending on the state of the invitation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvitationWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            var count = Invitation.GetPendingRecipientsCount();

            String errorMsg;
        }

        /// <summary>
        /// Handles the 'Close Window' event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseWindowButton_Click(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>
                           {
                               {"op", "createInvitation"},
                               {"invitationId", Invitation.ID.ToString()}
                           };

            Master.CloseDialog(args);
        }
    }
}
