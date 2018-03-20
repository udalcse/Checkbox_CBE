using System;

namespace Checkbox.Web.UI.Controls.Security
{
    /// <summary>
    /// Mode for security editor controls
    /// </summary>
    [Flags()]
    public enum SecurityEditorMode
    {
        /// <summary>
        /// Granting/denying access based on a specified permission
        /// </summary>
        GrantAccess = 1,

        /// <summary>
        /// Full ACL Editing
        /// </summary>
        ACLEdit = 2,

        /// <summary>
        /// Default Policy Editing
        /// </summary>
        DefaultPolicyEdit = 4,

        /// <summary>
        /// Allow acl editing and default policy editing
        /// </summary>
        AclAndDefaultPolicyEdit = ACLEdit | DefaultPolicyEdit
    }
}
