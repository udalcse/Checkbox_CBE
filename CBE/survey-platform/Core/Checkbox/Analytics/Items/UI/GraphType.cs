using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Table of values
        /// </summary>
        SummaryTable = 1,

        /// <summary>
        /// Column graph
        /// </summary>
        ColumnGraph,

        /// <summary>
        /// Pie graph
        /// </summary>
        PieGraph,

        /// <summary>
        /// Line graph
        /// </summary>
        LineGraph,

        /// <summary>
        /// Bar graph
        /// </summary>
        BarGraph,

        /// <summary>
        /// Cross tabulation
        /// </summary>
        CrossTab,

        /// <summary>
        /// Doughnut
        /// </summary>
        Doughnut,

        /// <summary>
        /// Statistics table
        /// </summary>
        StatisticsTable
    }
}
