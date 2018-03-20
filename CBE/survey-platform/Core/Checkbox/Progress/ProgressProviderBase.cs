using Checkbox.Common;
using Checkbox.Management;

namespace Checkbox.Progress
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ProgressProviderBase : IProgressProvider
    {
        /// <summary>
        /// Get the cache key for progress data.
        /// </summary>
        /// <param name="progressKey">Progress key.</param>
        /// <returns>Cache key.</returns>
        protected string GetCacheKey(string progressKey)
        {
            if (Utilities.IsNotNullOrEmpty(ApplicationManager.ApplicationDataContext))
            {
                return string.Format("PROGRESS::{0}::{1}", ApplicationManager.ApplicationDataContext, progressKey);
            }

            return string.Format("PROGRESS::{0}", progressKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalCounterItems"></param>
        /// <param name="stepCompletePercent"></param>
        /// <param name="stepMagnitude"></param>
        /// <param name="currentCounterItem"></param>
        /// <returns></returns>
        protected int CalculateProgressItem(int currentCounterItem, int totalCounterItems, int stepMagnitude, int stepCompletePercent)
        {
            return totalCounterItems > 0
                ? (int)(stepCompletePercent - (stepMagnitude * (totalCounterItems - currentCounterItem) / (double)totalCounterItems))
                : stepCompletePercent;
        }

        public abstract ProgressData GetProgress(string key);
        public abstract void ClearProgress(string key);
        public abstract void SetProgress(string key, string message, string errorMessage, ProgressStatus status, int currentItem, int itemCount);
        public abstract void SetProgress(string key, ProgressData progressData);
        public abstract void SetProgress(string progressKey, string message, int currentItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="baseMessage"></param>
        /// <param name="currentCounterItem"></param>
        /// <param name="totalCounterItems"></param>
        /// <param name="stepMagnitude"></param>
        /// <param name="stepCompletePercent"></param>
        public void SetProgressCounter(string progressKey, string baseMessage, int currentCounterItem, int totalCounterItems, int stepMagnitude, int stepCompletePercent)
        {
            if (string.IsNullOrEmpty(progressKey)
                || string.IsNullOrEmpty(baseMessage))
            {
                return;
            }

            string progressMessage = baseMessage.Contains("{0}") && baseMessage.Contains("{1}")
                ? string.Format(baseMessage, currentCounterItem, totalCounterItems)
                : baseMessage;

            int currentProgressItem = CalculateProgressItem(currentCounterItem, totalCounterItems, stepMagnitude, stepCompletePercent);

            SetProgress(progressKey, progressMessage, currentProgressItem);
        }
    }
}
