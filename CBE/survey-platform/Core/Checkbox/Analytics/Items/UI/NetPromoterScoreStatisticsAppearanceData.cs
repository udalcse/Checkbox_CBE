using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreStatisticsAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get net promoter score table appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_NET_PROMOTER_SCORE_STATISTICS"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults()
        {
            this["ShowMin"] = "true";
            this["ShowMax"] = "true";
            this["ShowAverage"] = "true";
            this["ShowVariance"] = "true";
            this["ShowStandartDeviation"] = "true";
            this["ShowTotalResponses"] = "true";
            this["ShowTotalRespondents"] = "false";
        }
    }
}
