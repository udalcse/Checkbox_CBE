using Checkbox.Security;

namespace Checkbox.Forms.Security
{
	/// <summary>
	/// Security controller for FormFolders
	/// </summary>
    public class FormFolderSecurityEditor : AccessControllablePDOSecurityEditor
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="folder"></param>
        public FormFolderSecurityEditor(FormFolder folder)
            : base(folder)
        {
        }

        /// <summary>
        /// Required folder permission
        /// </summary>
        protected override string RequiredEditorPermission { get { return "FormFolder.FullControl"; } }
	}
}
