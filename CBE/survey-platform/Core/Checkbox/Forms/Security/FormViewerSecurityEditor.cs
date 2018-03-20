using System.Data;
using System.Collections.Generic;

using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
    /// <summary>
    /// Summary description for FormViewerSecurityEditor.
    /// </summary>
    public class FormViewerSecurityEditor : FormSecurityEditor
    {
        /// <summary>
        /// Specify users that can create/view reports and survey data
        /// </summary>
        /// <param name="form"></param>
        public FormViewerSecurityEditor(IAccessControllable form)
            : base(form)
        {
        }

        /// <summary>
        /// Get a list of access permissible entities that can be added to the form acl.
        /// </summary>
        /// <returns><see cref="DataTable"/> object.</returns>
        public override List<IAccessControlEntry> GetAccessPermissible(string[] permissions)
        {
            return GetMergedAccessPermissible("Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Run");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <returns></returns>
        public override void GrantAccess(IAccessControlEntry[] pendingEntries, params string[] permissions)
        {
            UpdateAccess(
                pendingEntries,
                new[] { "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Run" },
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
               new[] { "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Run" },
               true);
        }
    }
}
