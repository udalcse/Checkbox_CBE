using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Charts;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Analytics.Items.UI;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance Editor for rank order summary item.
    /// </summary>
    public partial class ANALYSIS_RANK_ORDER_SUMMARY_TABLE : UserControlAppearanceEditorBase
    {
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
    }
}