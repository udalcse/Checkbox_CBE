//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using Checkbox.Security;
using Prezza.Framework.Security;

namespace Checkbox.Panels.Security
{
	/// <summary>
	/// Editor implementation for editing email list security policies.
	/// </summary>
    public class EmailListSecurityEditor : AccessControllablePDOSecurityEditor
	{
		/// <summary>
		/// Constructor.
		/// </summary>
        /// <param name="emailListPanel"><see cref="IAccessControllable"/> ControllableResource to edit security of.</param>
        public EmailListSecurityEditor(EmailListPanel emailListPanel)
            : base(emailListPanel)
		{
		}

        /// <summary>
        /// Get the permission required to edit email list security
        /// </summary>
        protected override string RequiredEditorPermission { get { return "EmailList.Edit"; } }
	}
}
