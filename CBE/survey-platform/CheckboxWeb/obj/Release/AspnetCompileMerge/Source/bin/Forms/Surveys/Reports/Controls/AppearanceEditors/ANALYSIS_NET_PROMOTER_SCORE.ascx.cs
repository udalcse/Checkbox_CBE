using System;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class ANALYSIS_NET_PROMOTER_SCORE : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the appearance data
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (data is NetPromoterScoreAppearanceData)
            {
                _showDetractors.Checked = IsPropertyTrue(data, "ShowDetractors");
                _showPromoters.Checked = IsPropertyTrue(data, "ShowPromoters");
                _showPassive.Checked = IsPropertyTrue(data, "ShowPassive");
                _showNps.Checked = IsPropertyTrue(data, "ShowNetPromoterScore");
            }
        }

        /// <summary>
        /// Determine if the specified property is "True" or not.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsPropertyTrue(AppearanceData data, string property)
        {
            return data[property] != null && data[property].Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Update the appearance data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (AppearanceData is NetPromoterScoreAppearanceData)
            {
                AppearanceData["ShowDetractors"] = _showDetractors.Checked.ToString();
                AppearanceData["ShowPromoters"] = _showPromoters.Checked.ToString();
                AppearanceData["ShowPassive"] = _showPassive.Checked.ToString();
                AppearanceData["ShowNetPromoterScore"] = _showNps.Checked.ToString();
            }
        }
    }
}