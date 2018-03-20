using System;
using Checkbox.Analytics.Computation;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreItem : NetPromoterScoreItemBase
    {
        protected override ReportDataCalculator CreateCalculator()
        {
            return new NetPromoterScoreItemDataCalculator();
        }
    }
}
