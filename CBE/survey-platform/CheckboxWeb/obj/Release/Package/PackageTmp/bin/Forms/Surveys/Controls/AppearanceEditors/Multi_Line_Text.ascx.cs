using System;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for multiline text
    /// </summary>
    public partial class Multi_Line_Text : UserControlAppearanceEditorBase
    {
        const int DefaultWidthColumns = 80;
        const int DefaultWidthRows = 10;
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
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _columnsTxt.Text = data["Columns"] ?? Convert.ToString(DefaultWidthColumns);
            _rowsTxt.Text = data["Rows"] ?? Convert.ToString(DefaultWidthRows);

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }

            if (_labelPositionList.Items.FindByValue(data["LabelPosition"]) != null)
            {
                _labelPositionList.SelectedValue = data["LabelPosition"];
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["Columns"] = _columnsTxt.Text;
            AppearanceData["Rows"] = _rowsTxt.Text;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
            AppearanceData["LabelPosition"] = _labelPositionList.SelectedValue;
        }
    }
}