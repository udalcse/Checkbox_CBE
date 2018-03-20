using System;
using System.Collections.Generic;
using Prezza.Framework.Caching;
using Prezza.Framework.Caching.Expirations;

namespace Checkbox.Progress
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheableProgressProvider : ProgressProviderBase
    {
        //Cache manager to use when tracking progress for fully-configured Checkbox
        private readonly CacheManager _progressCacheManager;

        //Cache manager to use when performing installation or other tasks on unconfigured Checkbox
        private readonly Dictionary<string, ProgressData> _progressDictionary;

        /// <summary>
        /// Default constructor to initialize the cache
        /// </summary>
        public CacheableProgressProvider()
        {
            //Use the default cache manager
            lock (typeof(ProgressProvider))
            {
                try
                {
                    _progressCacheManager = CacheFactory.GetCacheManager();
                }
                catch (Exception)
                {
                    _progressDictionary = new Dictionary<string, ProgressData>();
                }
            }
        }

        /// <summary>
        /// Get cached progress item from cache manager or static dictionary, depending
        /// on context.
        /// </summary>
        /// <param name="progressKey"></param>
        /// <returns></returns>
        private ProgressData GetCacheItem(string progressKey)
        {
            var cacheKey = GetCacheKey(progressKey);
            ProgressData progressData = null;

            if (_progressCacheManager != null)
            {
                progressData = _progressCacheManager[cacheKey] as ProgressData;
            }
            else if (_progressDictionary != null
                && _progressDictionary.ContainsKey(cacheKey))
            {
                progressData = _progressDictionary[cacheKey];
            }

            return progressData;
        }

        /// <summary>
        /// Add item to progress cache
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="progressData"></param>
        private void AddItemToCache(string progressKey, ProgressData progressData)
        {
            var cacheKey = GetCacheKey(progressKey);

            if (_progressCacheManager != null)
            {
                _progressCacheManager.Add(cacheKey,
                    progressData,
                    CacheItemPriority.Normal,
                    null,
                    new SlidingTime(new TimeSpan(0, 10, 0)));
            }
            else if (_progressDictionary != null)
            {
                _progressDictionary[cacheKey] = progressData;
            }
        }

        /// <summary>
        /// Remove item from progress cache
        /// </summary>
        /// <param name="progressKey"></param>
        private void RemoveCacheItem(string progressKey)
        {
            var cacheKey = GetCacheKey(progressKey);

            if (_progressCacheManager != null)
            {
                _progressCacheManager.Remove(cacheKey);
            }
            else if (_progressDictionary != null
                && _progressDictionary.ContainsKey(cacheKey))
            {
                _progressDictionary.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Get progress data associated with the specified key.
        /// </summary>
        /// <param name="key">Key representing progress to delete.</param>
        /// <returns></returns>
        public override ProgressData GetProgress(string key)
        {
            return GetCacheItem(key);
        }

        /// <summary>
        /// Clear/delete progress data associated with the specified key.
        /// </summary>
        /// <param name="key">Key representing progress data to remove.</param>
        public override void ClearProgress(string key)
        {
            //Remove the cache item
            RemoveCacheItem(key);
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
        public override void SetProgress(string key,
                                       string message,
                                       string errorMessage,
                                       ProgressStatus status,
                                       int currentItem,
                                       int itemCount)
        {
            SetProgress(
                key,
                new ProgressData
                {
                    Message = message,
                    ErrorMessage = errorMessage,
                    Status = status,
                    CurrentItem = currentItem,
                    TotalItemCount = itemCount
                }
            );
        }

        /// <summary>
        /// Set current progress for the specified progress key
        /// </summary>
        /// <param name="key">Key to uniquely identify progress data.</param>
        /// <param name="progressData">Progress data to store</param>
        public override void SetProgress(string key, ProgressData progressData)
        {
            AddItemToCache(key, progressData);
        }

        /// <summary>
        /// Set import progress
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="message"></param>
        /// <param name="currentItem"></param>
        public override void SetProgress(string progressKey, string message, int currentItem)
        {
            if (string.IsNullOrEmpty(progressKey))
            {
                return;
            }

            SetProgress(
                progressKey,
                message,
                string.Empty,
                ProgressStatus.Running,
                currentItem,
                100
            );
        }
    }
}
