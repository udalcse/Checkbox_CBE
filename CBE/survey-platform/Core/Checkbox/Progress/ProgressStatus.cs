using System;

namespace Checkbox.Progress
{
    /// <summary>
    /// Progress status enumerated type
    /// </summary>
    [Serializable]
    public enum ProgressStatus
    {
        /// <summary>
        /// Action has not started and is pending.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Action is in progress
        /// </summary>
        Running,

        /// <summary>
        /// Action completed
        /// </summary>
        Completed,

        /// <summary>
        /// An error occurred
        /// </summary>
        Error
    }
}
