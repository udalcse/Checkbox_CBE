using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Edit survey permissions
    /// </summary>
    public partial class Permissions : ResponseTemplateSecurityEditorPage
    {
        /// <summary>
        /// Get page specific title
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/title"); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Prezza.Framework.Security.IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Form.Administer"; } }

        /// <summary>
        /// Initialize child controls
        /// </summary>
        protected override void OnPageInit()
        {
            //StoreSecurityEditorData();
            base.OnPageInit();

            Master.SetTitle(PageSpecificTitle);
            Master.OkVisible = false;
            Master.CancelTextId = "/controlText/AccessListEditor.ascx/closeEditor";

            if (IsDialog)
                Master.CancelVisible = false;

            if (ApplicationManager.UseSimpleSecurity)
            {
                _aclEditor.Visible = false;
                _defaultPolicyEditor.Visible = false;
            }
            else
            {
                _aclEditor.Initialize(SecuredResourceType.Survey, ResponseTemplate.ID.Value, ContextData.AclPermissionsToGrant);
                _defaultPolicyEditor.Initialize(SecuredResourceType.Survey, ResponseTemplate.ID.Value, ContextData.AclPermissionsToGrant);
                _defaultPolicyEditor.DefaultPolicyId = ResponseTemplate.DefaultPolicyID.Value;
            }

            _grantAccess.Initialize(SecuredResourceType.Survey, ResponseTemplate.ID.Value, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }
    }
}
