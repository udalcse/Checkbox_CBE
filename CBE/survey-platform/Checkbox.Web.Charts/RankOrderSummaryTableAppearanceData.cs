using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Analytics.Items.UI;

namespace Checkbox.Web.Charts
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RankOrderSummaryTableAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_RANK_ORDER_SUMMARY_TABLE"; }
        }
    }
}
