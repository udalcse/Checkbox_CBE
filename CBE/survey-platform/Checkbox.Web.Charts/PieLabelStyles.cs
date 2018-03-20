namespace Checkbox.Web.Charts
{
    /// <summary>
    /// Possible label positions for pie and doughnut charts
    /// </summary>
    public enum PieLabelStyles
    {
        /// <summary>
        /// Labels are placed inside the chart values.
        /// </summary>
        Inside = 1,

        /// <summary>
        /// Labels are placed outside the chart values. The connecting lines are straight lines.
        /// </summary>
        Outside,

        /// <summary>
        /// Labels are not shown.
        /// </summary>
        Disabled
    }
}
