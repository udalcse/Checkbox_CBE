using System;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Type of averaging to perform on scores.
    /// </summary>
    [Serializable]
    public enum AverageScoreCalculation
    {
        /// <summary>
        /// Add value of all selected options for all items together and divide by total responses
        /// </summary>
        Aggregate,

        /// <summary>
        /// For each item, add value of all selected options together and divide by total responses, then divide result by number of items.
        /// </summary>
        AverageOfItemScores,

        /// <summary>
        /// For each item, add value of all selected options together and divide by total responses.
        /// </summary>
        ItemAverages,

        /// <summary>
        /// For each page, add value of sum of all points on the page and divide by total responses.
        /// </summary>
        PageAverages,

        /// <summary>
        /// For each page, add value of sum of all points on the page and divide by total responses.
        /// And calculate total survey score 
        /// </summary>
        PageAveragesWithTotalScore
    }
}
