using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// Matrix summary item appearance
    /// </summary>
    [Serializable]
    public class MatrixSummaryItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_MATRIX_SUMMARY"; }
        }
    }
}
