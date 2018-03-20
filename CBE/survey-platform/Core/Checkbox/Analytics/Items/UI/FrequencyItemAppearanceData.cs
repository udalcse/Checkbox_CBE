using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FrequencyItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// 
        /// </summary>
        public FrequencyItemAppearanceData()
        {
            SetPropertyValue("GraphType", GraphType.SummaryTable.ToString());
        }

        /// <summary>
        /// Get the summary item appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_SUMMARY"; }
        }
    }
}
