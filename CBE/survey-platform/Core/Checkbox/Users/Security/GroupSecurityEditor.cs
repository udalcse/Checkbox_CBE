//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using Checkbox.Security;

namespace Checkbox.Users.Security
{
	/// <summary>
	/// Policy and ACL editor class for user <see cref="Group"/> objects.
	/// </summary>
    public class GroupSecurityEditor : AccessControllablePDOSecurityEditor
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="group"><see cref="Group"/> this editor will modify.</param>
        public GroupSecurityEditor(Group group)
            : base(group)
        {
        }

        /// <summary>
        /// Get the required permission to edit user group security
        /// </summary>
        protected override string RequiredEditorPermission { get { return "Group.Edit"; } }
	}
}
