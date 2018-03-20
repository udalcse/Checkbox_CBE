using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Security
{
    public partial class SecurityEditor : SecurityEditorPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override SecuredResourceType SecuredResourceType
        {
            get { return ContextData.SecuredResourceType; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceId
        {
            get { return ContextData.SecuredResourceId; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string SecuredResourceName
        {
            get { return ContextData.SecuredResourceName; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceDefaultPolicyId
        {
            get { return ContextData.SecuredResourceDefaultPolicyId; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override SecurityEditorControl AclEditorControl
        {
            get { return _aclEditor; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override SecurityEditorControl DefaultPolicyEditorControl
        {
            get { return _defaultPolicyEditor; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override SecurityEditorControl GrantAccessControl
        {
            get { return _grantAccess; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            _defaultPolicyEditor.DefaultPolicyId = ContextData.SecuredResourceDefaultPolicyId;
            Master.OkVisible = false;
            Master.CancelTextId = "/controlText/AccessListEditor.ascx/closeEditor";
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
