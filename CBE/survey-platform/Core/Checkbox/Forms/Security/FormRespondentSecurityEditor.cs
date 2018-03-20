using System.Collections.Generic;

using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
    /// <summary>
    /// Summary description for FormRespondentSecurityEditor.
    /// </summary>
    public class FormRespondentSecurityEditor : FormSecurityEditor
    {
        /// <summary>
        /// Editor for selecting respondents
        /// </summary>
        /// <param name="form"></param>
        public FormRespondentSecurityEditor(IAccessControllable form)
            : base(form)
        {
        }

        /// <summary>
        /// Get a list of access permissible entities that can be added to the form acl.
        /// </summary>
        /// <param name="permissions"></param>
        public override List<IAccessControlEntry> GetAccessPermissible(params string[] permissions)
        {
            return GetMergedAccessPermissible(new[] { "Form.Fill" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override void GrantAccess(IAccessControlEntry[] pendingEntries, params string[] permissions)
        {
            UpdateAccess(
                pendingEntries,
                new[] { "Form.Fill" },
                false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <returns></returns>
        public override void RemoveAccess(IAccessControlEntry[] pendingEntries)
        {
            UpdateAccess(
                pendingEntries,
                new[] { "Form.Fill" },
                true);
        }
    }
}
