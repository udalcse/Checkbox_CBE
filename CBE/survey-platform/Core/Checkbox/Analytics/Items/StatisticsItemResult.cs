using System;
using Checkbox.Analytics.Computation;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Result for statistics item
    /// </summary>
    [Serializable]
    public class StatisticsItemResult : ItemResult
    {
        /// <summary>
        /// Get/set response value for the result
        /// </summary>
        public int Response { get; set; }

        /// <summary>
        /// Get/set mean value for the result
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Get/set median value for the result
        /// </summary>
        public double Median { get; set; }

        /// <summary>
        /// Get/set mode value for the result
        /// </summary>
        public double Mode { get; set; }

        /// <summary>
        /// Get/set standard deviation value for the result
        /// </summary>
        public double StandardDeviation { get; set; }
    }
}
