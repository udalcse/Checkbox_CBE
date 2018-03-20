using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Editor for score message item.
    /// </summary>
    public partial class Score_Message : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
             "colorPicker.js",
             ResolveUrl("~/Resources/mColorPicker.min.js"));

            RegisterClientScriptInclude(
               "jquery.numeric.js",
               ResolveUrl("~/Resources/jquery.numeric.js"));
        }
         /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _selectedColor.Text = data["FontColor"];

            _fontSizeTxt.Text = data["FontSize"];

            if (_itemPositionList.Items.FindByValue(data["ItemPosition"]) != null)
            {
                _itemPositionList.SelectedValue = data["ItemPosition"];
            }
        }

        /// <summary>
        /// Update appearance data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["FontSize"] = _fontSizeTxt.Text.Trim();
            AppearanceData["FontColor"] = _selectedColor.Text;
            AppearanceData["ItemPosition"] = _itemPositionList.SelectedValue;
        }
    }
}