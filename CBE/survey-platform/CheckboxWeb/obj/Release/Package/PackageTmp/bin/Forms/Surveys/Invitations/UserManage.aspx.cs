using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Messaging.Email;
using Checkbox.Forms;
using Checkbox.Invitations;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class UserManage : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("ret")]
        public string ReturnPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("term")]
        public string SearchTerm { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName ="i", DefaultValue="0", IsRequired = false, IsDefaultUsedForInvalid = true)]
        public int InvitationID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Require view responses permission for the page
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Form.Administer"; }
        }

        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        protected override ResponseTemplate ResponseTemplate
        {
            get
            {
                if (ResponseTemplateId == 0 && InvitationID > 0)
                {
                    InvitationData i = InvitationManager.GetInvitationData(InvitationID);
                    InvitationID = i.DatabaseId;
                    this.ResponseTemplateId = i.ResponseTemplateId;
                }

                return base.ResponseTemplate;
            }
        }


        /// <summary>
        /// Initialize controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            _userInvitationList.SurveyId = ResponseTemplateId;
            
            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for uframe
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                 "svcSurveyEditor.js",
                 ResolveUrl("~/Services/js/svcSurveyEditor.js"));

            RegisterClientScriptInclude(
                "jquery.localize.js",
                ResolveUrl("~/Resources/jquery.localize.js"));
        }
    }
}