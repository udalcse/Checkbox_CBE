namespace Checkbox.Progress
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProgressProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ProgressData GetProgress(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void ClearProgress(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="errorMessage"></param>
        /// <param name="status"></param>
        /// <param name="currentItem"></param>
        /// <param name="itemCount"></param>
        void SetProgress(string key, string message, string errorMessage, ProgressStatus status,
            int currentItem, int itemCount);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progressData"></param>
        void SetProgress(string key, ProgressData progressData);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="message"></param>
        /// <param name="currentItem"></param>
        void SetProgress(string progressKey, string message, int currentItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="baseMessage"></param>
        /// <param name="currentCounterItem"></param>
        /// <param name="totalCounterItems"></param>
        /// <param name="stepMagnitude"></param>
        /// <param name="stepCompletePercent"></param>
        void SetProgressCounter(string progressKey, string baseMessage, int currentCounterItem, int totalCounterItems,
                                int stepMagnitude, int stepCompletePercent);
    }
}
