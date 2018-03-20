using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TotalScoreItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_TOTAL_SCORE"; }
        }
    }
}
