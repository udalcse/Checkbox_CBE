using System;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Option for handling "other" responses in a report.
    /// </summary>
    [Serializable]
    public enum OtherOption
    {
        /// <summary>
        /// Aggregate all other responses into one "other" option line item.
        /// </summary>
        Aggregate = 1,

        /// <summary>
        /// Display "other" answers separately but not with the rest of the answers.
        /// </summary>
        Display,

        /// <summary>
        /// Aggregate all other responses into one "other" option line item
        /// and display "other" answers separately but not with the rest of the answers.
        /// </summary>
        AggregateAndDisplay
    }
}
