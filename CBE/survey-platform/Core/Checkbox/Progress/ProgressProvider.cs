namespace Checkbox.Progress
{
    /// <summary>
    /// Support for storing and retrieving progress-related information.
    /// </summary>
    public static class ProgressProvider
    {
        private static readonly CacheableProgressProvider _cacheableProgressProvider;

        /// <summary>
        /// 
        /// </summary>
        static ProgressProvider()
        {
            _cacheableProgressProvider = new CacheableProgressProvider();
        }

        /// <summary>
        /// Set import progress
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="baseMessage"></param>
        /// <param name="currentCounterItem"></param>
        /// <param name="totalCounterItems"></param>
        /// <param name="stepMagnitude">Total percentage of process represented by current steps</param>
        /// <param name="stepCompletePercent">Total percentage complete when current step is done</param>
        public static void SetProgressCounter(string progressKey, string baseMessage, int currentCounterItem, int totalCounterItems, int stepMagnitude, int stepCompletePercent)
        {
            _cacheableProgressProvider.SetProgressCounter(progressKey, baseMessage, currentCounterItem, totalCounterItems, stepCompletePercent, stepCompletePercent);
        }


        /// <summary>
        /// Set import progress
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="message"></param>
        /// <param name="currentItem"></param>
        public static void SetProgress(string progressKey, string message, int currentItem)
        {
            _cacheableProgressProvider.SetProgress(progressKey, message, currentItem);
        }


        /// <summary>
        /// Set current progress for the specified progress key
        /// </summary>
        /// <param name="key">Key to uniquely identify progress data.</param>
        /// <param name="progressData">Progress data to store</param>
        public static void SetProgress(string key, ProgressData progressData)
        {
            _cacheableProgressProvider.SetProgress(key, progressData);
        }

        /// <summary>
        /// Set current progress for the specified progress key
        /// </summary>
        /// <param name="key">Key to uniquely identify progress data.</param>
        /// <param name="message">Progress status message</param>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="status">Status of progress.</param>
        /// <param name="currentItem">Current item number in progress batch.</param>
        /// <param name="itemCount">Total number of items in progress batch.</param>
        public static void SetProgress(string key, 
                                       string message,
                                       string errorMessage,
                                       ProgressStatus status,
                                       int currentItem,
                                       int itemCount)
        {
            _cacheableProgressProvider.SetProgress(key, message, errorMessage, status, currentItem, itemCount);
        }

        /// <summary>
        /// Clear/delete progress data associated with the specified key.
        /// </summary>
        /// <param name="key">Key representing progress data to remove.</param>
        public static void ClearProgress(string key)
        {
            //Remove the cache item
            _cacheableProgressProvider.ClearProgress(key);
        }

        /// <summary>
        /// Get progress data associated with the specified key.
        /// </summary>
        /// <param name="key">Key representing progress to delete.</param>
        /// <returns></returns>
        public static ProgressData GetProgress(string key)
        {
            return _cacheableProgressProvider.GetProgress(key);
        }
    }
}