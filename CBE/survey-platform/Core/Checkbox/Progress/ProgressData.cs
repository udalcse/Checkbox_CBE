using System;

namespace Checkbox.Progress
{
    /// <summary>
    /// Simple container class for storing progress information.
    /// </summary>
    [Serializable]
    public class ProgressData
    {
        /// <summary>
        /// Message associated with progress or current status
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message specific to error status
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Get/set status of the operation
        /// </summary>
        public ProgressStatus Status { get; set; }

        /// <summary>
        /// Get/set percentage of current operation completed.
        /// </summary>
        public double PercentComplete
        {
            get
            {
                return ((double)CurrentItem / TotalItemCount) * 100;
            }
        }

        /// <summary>
        /// Get/set the current "item" in the progress
        /// </summary>
        public int CurrentItem { get; set; }

        /// <summary>
        /// Get/set total number of items.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Result of operation.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Additional data that can be used as result of operation
        /// </summary>
        public string AdditionalData { get; set; }
    }
}
