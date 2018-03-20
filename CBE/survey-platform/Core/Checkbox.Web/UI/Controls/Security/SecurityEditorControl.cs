using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.UI.Controls.Security
{
    /// <summary>
    /// Base class for controls that allow users to edit security
    /// </summary>
    public abstract class SecurityEditorControl : MementoEnabledUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionToGrant"></param>
        public virtual void Initialize(SecuredResourceType resourceType, int resourceId, string permissionToGrant)
        {
            SecuredResourceType = resourceType;
            SecuredResourceId = resourceId;
            PermissionToGrant = permissionToGrant;
        }

        /// <summary>
        /// Type of resource secured
        /// </summary>
        protected SecuredResourceType SecuredResourceType { get; set; }

        /// <summary>
        /// ID of resource secured
        /// </summary>
        protected int SecuredResourceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string PermissionToGrant { get; set; }

    }
}
