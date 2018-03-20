using System;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    public partial class DropDown_List : UserControlAppearanceEditorBase
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

            _numberLabelsChk.Checked =
                data["ShowNumberLabels"] != null
                && data["ShowNumberLabels"].Equals("True", StringComparison.InvariantCultureIgnoreCase);

            _showAsComboboxChk.Checked =
                data["ShowAsCombobox"] != null
                && data["ShowAsCombobox"].Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Update data with control inputs
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["LabelPosition"] = _labelPositionList.SelectedValue;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
            AppearanceData["ShowNumberLabels"] = _numberLabelsChk.Checked.ToString();
            AppearanceData["ShowAsCombobox"] = _showAsComboboxChk.Checked.ToString();
        }
    }
}