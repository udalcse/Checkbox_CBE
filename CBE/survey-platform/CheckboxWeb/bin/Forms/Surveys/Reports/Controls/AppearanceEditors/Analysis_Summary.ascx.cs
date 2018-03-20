using System;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for summary table items.
    /// </summary>
    public partial class Analysis_Summary : UserControlAppearanceEditorBase
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
        /// Initialize
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (data is AnalysisItemAppearanceData)
            {
                _showEmptyChk.Checked = ((AnalysisItemAppearanceData)data).ShowDataLabelZeroValues;
            }

			_widthTxt.Text = data["Width"] ?? string.Empty;
			_heightTxt.Text = data["Height"] ?? string.Empty;
            _optionsOrder.SelectedIndex = "Survey".Equals(data["OptionsOrder"]) ? 0 : 1;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (AppearanceData is AnalysisItemAppearanceData)
            {
                ((AnalysisItemAppearanceData)AppearanceData).ShowDataLabelZeroValues = _showEmptyChk.Checked;
            }

			AppearanceData["Width"] = _widthTxt.Text;
			AppearanceData["Height"] = _heightTxt.Text;
            AppearanceData["OptionsOrder"] = _optionsOrder.SelectedValue;
        }
    }
}