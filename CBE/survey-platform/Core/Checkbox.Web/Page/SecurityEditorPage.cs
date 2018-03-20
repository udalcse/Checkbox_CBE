using System.Web;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Security;
using Checkbox.Web.UI.Controls.Security;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for pages that edit security
    /// </summary>
    public abstract class SecurityEditorPage : SecuredPage
    {
        /// <summary>
        /// Security editor context
        /// </summary>
        private SecurityEditorData _contextData;

        /// <summary>
        /// Get the security context data
        /// </summary>
        protected SecurityEditorData ContextData
        {
            get {
                return _contextData ??
                       (_contextData = GetSessionValue<SecurityEditorData>("SecurityContextData", true, null));
            }
        }

        /// <summary>
        /// Get/set whether this page is a redirect to the actual security editor
        /// </summary>
        protected virtual bool IsRedirect { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        protected abstract SecuredResourceType SecuredResourceType { get; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract int SecuredResourceId { get; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract int SecuredResourceDefaultPolicyId { get; }

        ///
        protected abstract string SecuredResourceName { get; }

        /// <summary>
        /// Get permission required to edit acl of controllable entity
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return ContextData.RequiredPermission; }
        }

        /// <summary>
        /// Get permission required to access page, which depends on object being edited
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return ControllableEntityRequiredPermission; }
        }

        /// <summary>
        /// Page initialization
        /// </summary>
        protected override void OnPageInit()
        {
            //Only store data if this is a "source" page that redirects to shared security
            // editor page.
            if (IsRedirect)
            {
                StoreSecurityEditorData();
            }

            base.OnPageInit();

            //Init editor controls
            InitializeEditorControls();

            
            //Set title
            if (Master != null && Master is BaseMasterPage)
            {
                var pageTitle = string.Format("{0} - {1}", WebTextManager.GetText("/pageText/security/securityEditor.aspx/emptyTitle"), Utilities.StripHtml(ContextData.SecuredResourceName, 64));
                ((BaseMasterPage)Master).SetTitle(pageTitle);
            }
        }


        /// <summary>
        /// Initialize editor controls
        /// </summary>
        protected virtual void InitializeEditorControls()
        {
            if (AclEditorControl != null)
            {
                AclEditorControl.Initialize(ContextData.SecuredResourceType, ContextData.SecuredResourceId, ContextData.AclPermissionsToGrant);
            }

            if (DefaultPolicyEditorControl != null)
            {
                DefaultPolicyEditorControl.Initialize(ContextData.SecuredResourceType, ContextData.SecuredResourceId, ContextData.AclPermissionsToGrant);
            }

            if (GrantAccessControl != null)
            {
                GrantAccessControl.Initialize(ContextData.SecuredResourceType, ContextData.SecuredResourceId, ContextData.AclPermissionsToGrant);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual SecurityEditorControl AclEditorControl { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        protected virtual SecurityEditorControl DefaultPolicyEditorControl { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        protected virtual SecurityEditorControl GrantAccessControl { get { return null; } }

        /// <summary>
        /// Get CSV list of permissions to grant on ACL
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAclPermissionsToGrant()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRequiredPermissionForAclEdit()
        {
            //Return non-existent permission to prevent access w/no session
            return "System.Administrator";
        }

        /// <summary>
        /// Put the SecurityEditorData into the Session for pages that need to access more than one ACL Editor
        /// </summary>
        protected virtual void StoreSecurityEditorData()
        {
            var editorData = new SecurityEditorData
            {
                SecuredResourceType = SecuredResourceType,
                SecuredResourceId = SecuredResourceId,
                AclPermissionsToGrant = GetAclPermissionsToGrant(),
                Context = SiteMap.CurrentNode != null ? SiteMap.CurrentNode.Url : string.Empty,
                RequiredPermission = GetRequiredPermissionForAclEdit(),
                SecuredResourceName = SecuredResourceName,
                SecuredResourceDefaultPolicyId = SecuredResourceDefaultPolicyId
            };

            Session["SecurityContextData"] = editorData;
        }
    }
}
