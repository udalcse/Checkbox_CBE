using System;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for single line text items
    /// </summary>
    public partial class Display_Response : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }

			_widthTxt.Text = data["Width"] ?? string.Empty;
			_heightTxt.Text = data["Height"] ?? string.Empty;

        }

        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;

			AppearanceData["Width"] = _widthTxt.Text;
			AppearanceData["Height"] = _heightTxt.Text;

        }
    }
}