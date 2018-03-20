using System;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Reporting option for the statistics report item.
    /// </summary>
    [Serializable]
    public enum StatisticsItemReportingOption
    {
        /// <summary>
        /// Calculate the count of responses for each source item without N/A options
        /// </summary>
        Responses,

        /// <summary>
        /// Calculate the mean for each source item.
        /// </summary>
        Mean,

        /// <summary>
        /// Calculate the median for each source item.
        /// </summary>
        Median,

        /// <summary>
        /// Calculate the mode for each source item.
        /// </summary>
        Mode,

        /// <summary>
        /// Calculate the standard deviation for each source item.
        /// </summary>
        StdDeviation,

        /// <summary>
        /// Calculate all points above described
        /// </summary>
        All
    }
}
