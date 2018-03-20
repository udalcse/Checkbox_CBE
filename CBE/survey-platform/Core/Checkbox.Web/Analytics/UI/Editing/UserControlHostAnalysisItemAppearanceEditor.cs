using Checkbox.Web.Forms.UI.Editing;

namespace Checkbox.Web.Analytics.UI.Editing
{
    /// <summary>
    /// Editor for report item appearance.
    /// </summary>
    public class UserControlHostAnalysisItemAppearanceEditor : UserControlHostAppearanceEditor
    {
        /// <summary>
        /// Get path to folder containing appearance editors
        /// </summary>
        protected override string BaseControlPath { get { return "/Forms/Surveys/Reports/Controls/AppearanceEditors"; } }
    }
}
