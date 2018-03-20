using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Web.Charts
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RankOrderSummaryItemAppearanceData : SummaryChartItemAppearanceData
    {
        /// <summary>
        /// Get appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_RANK_ORDER_SUMMARY_CHART"; }
        }
    }
}
