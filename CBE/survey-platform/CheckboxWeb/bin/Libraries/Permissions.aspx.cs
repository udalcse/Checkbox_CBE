using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using Checkbox.Management;

namespace CheckboxWeb.Libraries
{
    public partial class Permissions : SecurityEditorPage
    {
        private LibraryTemplate _libraryTemplate;

        [QueryParameter("lib")]
        public int? LibraryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override SecuredResourceType SecuredResourceType
        {
            get { return SecuredResourceType.Library; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceId
        {
            get { return Library.ID.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string SecuredResourceName
        {
            get { return Library.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int SecuredResourceDefaultPolicyId
        {
            get { return Library.DefaultPolicyID.Value; }
        }

        /// <summary>
        /// Get boolean indicating this page redirects to shared security editor page.
        /// </summary>
        protected override bool IsRedirect { get { return true; } }

        /// <summary>
        /// 
        /// </summary>
        public LibraryTemplate Library
        {
            get
            {
                if (_libraryTemplate == null && LibraryId.HasValue && LibraryId.Value > 0)
                {
                    _libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(LibraryId.Value);
                    _libraryTemplate.Name = WebTextManager.GetText(_libraryTemplate.NameTextID);
                }

                return _libraryTemplate;
            }
        }

        /// <summary>
        /// Get controllable entity for security checks
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return Library;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Library.Edit"; } }

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
        /// <returns></returns>
        protected override string GetAclPermissionsToGrant()
        {
            return "Library.Create,Library.Delete,Library.Edit,Library.View";
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Response.Redirect(ResolveUrl("~/Security/SecurityEditor.aspx"), false);
        }
    }
}
