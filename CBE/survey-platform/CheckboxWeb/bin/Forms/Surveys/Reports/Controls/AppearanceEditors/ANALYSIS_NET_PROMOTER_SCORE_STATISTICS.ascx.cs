using System;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class ANALYSIS_NET_PROMOTER_SCORE_STATISTICS : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the appearance data
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (data is NetPromoterScoreStatisticsAppearanceData)
            {
                _showMin.Checked = IsPropertyTrue(data, "ShowMin");
                _showMax.Checked = IsPropertyTrue(data, "ShowMax");
                _showAverage.Checked = IsPropertyTrue(data, "ShowAverage");
                _showVariance.Checked = IsPropertyTrue(data, "ShowVariance");
                _showStdDev.Checked = IsPropertyTrue(data, "ShowStandartDeviation");
                _showResponses.Checked = IsPropertyTrue(data, "ShowTotalResponses");
           //     _showRespondents.Checked = IsPropertyTrue(data, "ShowTotalRespondents");
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

            if (AppearanceData is NetPromoterScoreStatisticsAppearanceData)
            {
                AppearanceData["ShowMin"] = _showMin.Checked.ToString();
                AppearanceData["ShowMax"] = _showMax.Checked.ToString();
                AppearanceData["ShowAverage"] = _showAverage.Checked.ToString();
                AppearanceData["ShowVariance"] = _showVariance.Checked.ToString();
                AppearanceData["ShowStandartDeviation"] = _showStdDev.Checked.ToString();
                AppearanceData["ShowTotalResponses"] = _showResponses.Checked.ToString();
             //   AppearanceData["ShowTotalRespondents"] = _showRespondents.Checked.ToString();
            }
        }
    }
}