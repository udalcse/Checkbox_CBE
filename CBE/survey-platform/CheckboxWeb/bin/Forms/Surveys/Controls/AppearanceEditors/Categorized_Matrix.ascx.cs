using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    public partial class Categorized_Matrix : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize editor with data to edit
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_gridLinesList.Items.FindByValue(data["GridLines"]) != null)
            {
                _gridLinesList.SelectedValue = data["GridLines"];
            }

            _widthTxt.Text = data["Width"] ?? string.Empty;

            if (_labelPositionList.Items.FindByValue(data["LabelPosition"]) != null)
            {
                _labelPositionList.SelectedValue = data["LabelPosition"];
            }

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }

        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            //TODO: Validate
            return base.Validate();
        }

        /// <summary>
        /// Update data with control inputs
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["GridLines"] = _gridLinesList.SelectedValue;
            AppearanceData["Width"] = _widthTxt.Text;
            AppearanceData["LabelPosition"] = _labelPositionList.SelectedValue;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;

        }
    }
}