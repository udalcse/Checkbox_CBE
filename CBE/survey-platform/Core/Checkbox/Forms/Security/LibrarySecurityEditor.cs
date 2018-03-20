using Checkbox.Security;
using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
	/// <summary>
	/// Summary description for ItemLibrarySecurityEditor.
	/// </summary>
    public class LibrarySecurityEditor : AccessControllablePDOSecurityEditor
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="library"></param>
        public LibrarySecurityEditor(IAccessControllable library)
            : base(library)
        {
        }

        /// <summary>
        /// Get the permission required to edit item library security
        /// </summary>
        protected override string RequiredEditorPermission { get { return "Library.Edit"; } }
	}
}
