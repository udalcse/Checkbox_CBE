using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Forms.Items.UI;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    public partial class Rank_Order : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize editor with data to edit
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_labelPositionList.Items.FindByValue(data["LabelPosition"]) != null)
            {
                _labelPositionList.SelectedValue = data["LabelPosition"];
            }

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }

            if (_optionLabelOrientationList.Items.FindByValue(data["OptionLabelOrientation"]) != null)
            {
                _optionLabelOrientationList.SelectedValue = data["OptionLabelOrientation"];
            }
        }

        /// <summary>
        /// Update data with control inputs
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["LabelPosition"] = _labelPositionList.SelectedValue;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
            AppearanceData["OptionLabelOrientation"] = _optionLabelOrientationList.SelectedValue;
        }
    }
}