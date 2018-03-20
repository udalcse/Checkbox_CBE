using Checkbox.Security;
using Prezza.Framework.Security;

namespace Checkbox.Analytics.Security
{
	/// <summary>
	/// Summary description for AnalysisSecurityEditor.
	/// </summary>
	public class AnalysisSecurityEditor : AccessControllablePDOSecurityEditor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="analysis">Analysis object to edit security for.</param>
        public AnalysisSecurityEditor(IAccessControllable analysis)
            : base(analysis)
        {
        }

        /// <summary>
        /// Get required permission to manage report security
        /// </summary>
        protected override string RequiredEditorPermission { get { return "Analysis.Administer"; } }
	}
}
