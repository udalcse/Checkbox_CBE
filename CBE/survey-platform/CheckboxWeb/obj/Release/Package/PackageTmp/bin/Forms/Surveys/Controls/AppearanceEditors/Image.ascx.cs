using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for image control
    /// </summary>
    public partial class Image : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the editor
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
        }
    }
}