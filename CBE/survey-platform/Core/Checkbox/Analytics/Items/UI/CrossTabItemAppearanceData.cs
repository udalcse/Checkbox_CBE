using System;
using System.Data;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CrossTabItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_CROSSTAB"; }
        }
    }
}
