using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using System;

namespace CheckboxWeb.Forms.Folders
{
    public partial class Permissions : SecurityEditorPage
    {
        private FormFolder _folder;

        [QueryParameter("f")]
        public int? FolderId { get; set; }

        /// <summary>
        /// Get boolean indicating this page redirects to shared security editor page.
        /// </summary>
        protected override bool IsRedirect { get { return true; } }

        /// <summary>
        /// 
        /// </summary>
        protected override SecuredResourceType SecuredResourceType
        {
            get { return SecuredResourceType.Folder; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceId
        {
            get { return FolderId.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string SecuredResourceName
        {
            get { return Folder.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceDefaultPolicyId
        {
            get { return Folder.DefaultPolicyID.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FormFolder Folder
        {
            get
            {
                if(_folder == null && FolderId.HasValue && FolderId.Value > 0)
                    _folder = FolderManager.GetFolder(FolderId.Value);

                return _folder;
            }
        }

      
        /// <summary>
        /// Get controllable entity for security checks
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return Folder;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "FormFolder.FullControl"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetRequiredPermissionForAclEdit()
        {
            return ControllableEntityRequiredPermission;
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            /* 12832: Simple security limit implementation */
            if (Checkbox.Management.ApplicationManager.UseSimpleSecurity)
            {
                Response.ClearContent();
                Response.Write("This functionality is not allowed in simple security mode.");
                Master.HideDialogButtons();
                return;
            }

            base.OnPageInit();

            Response.Redirect(ResolveUrl("~/Security/SecurityEditor.aspx"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetAclPermissionsToGrant()
        {
            return "FormFolder.Read";
        }
    }
}
