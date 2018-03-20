using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get net promoter score table appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_NET_PROMOTER_SCORE"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults()
        {
            this["ShowDetractors"] = "true";
            this["ShowPassive"] = "true";
            this["ShowPromoters"] = "true";
            this["ShowNetPromoterScore"] = "true";
        }
    }
}
