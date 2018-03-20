using System;
using Checkbox.Analytics.Computation;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreStatisticsItemResult : ItemResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double AverageValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Variance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double StandardDeviation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalResponses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalRespondents { get; set; }
    }
}
