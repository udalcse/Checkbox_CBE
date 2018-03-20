using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{    
    public partial class Analysis_Statistics_Table : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the appearance data
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (data is StatisticsItemAppearanceData)
            {
                _showResponses.Checked = IsPropertyTrue(data, "ShowResponses");
                _showMean.Checked = IsPropertyTrue(data, "ShowMean");
                _showMedian.Checked = IsPropertyTrue(data, "ShowMedian");
                _showMode.Checked = IsPropertyTrue(data, "ShowMode");
                _showStdDeviation.Checked = IsPropertyTrue(data, "ShowStdDeviation");
            }
        }

        /// <summary>
        /// Determine if the specified property is "True" or not.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsPropertyTrue(AppearanceData data, String property)
        {
            return data[property] != null && data[property].Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Update the appearance data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (AppearanceData is StatisticsItemAppearanceData)
            {
                AppearanceData["ShowResponses"] = _showResponses.Checked.ToString();
                AppearanceData["ShowMean"] = _showMean.Checked.ToString();
                AppearanceData["ShowMedian"] = _showMedian.Checked.ToString();
                AppearanceData["ShowMode"] = _showMode.Checked.ToString();
                AppearanceData["ShowStdDeviation"] = _showStdDeviation.Checked.ToString();
            }
        }
    }
}