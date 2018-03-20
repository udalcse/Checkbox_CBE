using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StatisticsItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get statistics table appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_STATISTICS_TABLE"; }
        }

        public override void SetDefaults()
        {
            this["ShowResponses"] = "true";
            this["ShowMean"] = "true";
            this["ShowMedian"] = "true";
            this["ShowMode"] = "true";
            this["ShowStdDeviation"] = "true";
        }
    }
}
