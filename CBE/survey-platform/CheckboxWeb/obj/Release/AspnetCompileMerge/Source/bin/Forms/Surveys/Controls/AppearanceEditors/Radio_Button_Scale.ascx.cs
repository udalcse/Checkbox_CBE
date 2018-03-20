using System;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for rating scale
    /// </summary>
    public partial class Radio_Button_Scale : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize editor with data to edit
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_layoutList.Items.FindByValue(data["Layout"]) != null)
            {
                _layoutList.SelectedValue = data["Layout"];
            }

            if (_labelPositionList.Items.FindByValue(data["LabelPosition"]) != null)
            {
                _labelPositionList.SelectedValue = data["LabelPosition"];
            }

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }

            if (_showSeparatorList.Items.FindByValue(data["ShowSeparator"]) != null)
            {
                _showSeparatorList.SelectedValue = data["ShowSeparator"];
            }

            _optionWidth.Text = data["OptionWidth"];
        }

        /// <summary>
        /// Update data with control inputs
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["Layout"] = _layoutList.SelectedValue;
            AppearanceData["LabelPosition"] = _labelPositionList.SelectedValue;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
            AppearanceData["ShowSeparator"] = _showSeparatorList.SelectedValue;
            AppearanceData["OptionWidth"] = _optionWidth.Text;
        }
    }
}