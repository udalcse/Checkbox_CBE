using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for progress reporting data.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProgressData
    {
        /// <summary>
        /// Unique identifier of progress session.
        /// </summary>
        [DataMember]
        public string ProgressKey { get; set; }

        /// <summary>
        /// Status of operation being tracked.  Possible values are:
        ///   Pending   - Operation not yet started.
        ///   Running   - Operation in progress.
        ///   Completed - Operation completed successfully.
        ///   Error     - Operation failed.
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Current status message for operation.
        /// </summary>
        [DataMember]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Additional error details about reason for error failure.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Additional data about final result of operation.  
        /// </summary>
        [DataMember]
        public string Result { get; set; }

        /// <summary>
        /// Total percentage of operation that is complete.  Computed using CurrentItem and
        /// TotalItemCount
        /// </summary>
        [DataMember]
        public double PercentComplete
        {
            get
            {
                if (TotalItemCount == 0)
                {
                    return 0;
                }

                return ((double)CurrentItem / TotalItemCount) * 100;
            }

            set { ;}
        }

        /// <summary>
        /// Current "Item" being processed in the tracked operation.
        /// </summary>
        [DataMember]
        public int CurrentItem { get; set; }

        /// <summary>
        /// Total number of "Items" to be processed in the tracked operation.
        /// </summary>
        [DataMember]
        public int TotalItemCount { get; set; }

        [DataMember]
        public string DownloadUrl { get; set; }

    }
}
