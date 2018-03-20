using System;
using System.Collections.Generic;
using Prezza.Framework.Caching;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Provide access to analysis data through reporting service or directly to Checkbox DB depending on application
    /// configuration.
    /// </summary>
    public static class AnalysisDataProxy
    {
        private static readonly CacheManager _analysisItemResultCache;

        /// <summary>
        /// Constructor to initialize the data cache
        /// </summary>
        static AnalysisDataProxy()
        {
            lock (typeof(AnalysisDataProxy))
            {
                _analysisItemResultCache = CacheFactory.GetCacheManager("analysisItemResultCache");
            }
        }

        /// <summary>
        /// Generate a cache key for the results based on item id and language code.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GenerateResultCacheKey(int itemId, string languageCode, string key)
        {
            return string.Format("{0}_{1}_{2}", itemId, languageCode, key);
        }

        /// <summary>
        /// Cache the specified analysis item result 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="analysisItemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="key"></param>
        /// <param name="result"></param>
        public static void AddResultToCache<T>(int analysisItemId, string languageCode, string key,  T result)
        {
            var cacheItem = new AnalysisDataProxyCacheItem<T>();
            cacheItem.Data = result;
            cacheItem.ReferenceDate = DateTime.Now;

            _analysisItemResultCache.Add(
                GenerateResultCacheKey(analysisItemId, languageCode, key),
                cacheItem);
        }

        /// <summary>
        /// Cache the specified analysis item result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="analysisItemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="key"></param>
        public static AnalysisDataProxyCacheItem<T> GetResultFromCache<T>(int analysisItemId, string languageCode, string key)
        {
            string cacheKey = GenerateResultCacheKey(analysisItemId, languageCode, key);

            if (_analysisItemResultCache.Contains(cacheKey))
            {
                //Get the result and validate
                return _analysisItemResultCache[cacheKey] as AnalysisDataProxyCacheItem<T>;
            }

            return null;
        }

        /// <summary>
        /// Validate that the result data associated with this item is
        /// current.  Returns false if data not in cache or data's reference
        /// date is earlier than specified date. 
        /// If reference date arg. is null, result is always considered
        /// valid if it is in the cache. 
        /// </summary>
        /// <param name="analysisItemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="dataKey"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static bool ValidateItemResultData(int analysisItemId, string languageCode, string dataKey, DateTime? referenceDate)
        {
            //Item with negative id is never value
            if (analysisItemId <= 0)
            {
                return false;
            }

            //Check the cache for data
            AnalysisDataProxyCacheItem<AnalysisItemResult> cachedData = GetResultFromCache<AnalysisItemResult>(
                analysisItemId,
                languageCode,
                dataKey);

            //If no data, then this item's data is not valid
            if (cachedData == null)
            {
                return false;
            }

            //Item in cache and no ref. date, then return true.
            if (!referenceDate.HasValue)
            {
                return true;
            }

            return cachedData.ReferenceDate >= referenceDate;
        }

        /// <summary>
        /// For the given list of response templates, return a reference date to use for freshness-checking
        /// report item data.  Freshness date will be latest modified response date among all specified
        /// templates.
        /// </summary>
        /// <param name="sourceResponseTemplateIds"></param>
        /// <returns></returns>
        public static DateTime? GetSurveyReferenceDate(IEnumerable<int> sourceResponseTemplateIds)
        {
            DateTime? curMaxDate = null;

            //For each response template, get max modified date.
            DateTime? responseMinDate;
            DateTime? responseMaxDate;

            foreach (int responseTemplateId in sourceResponseTemplateIds)
            {
                //Get min/max response dates
                ResponseManager.GetMinMaxResponseDates(responseTemplateId, null, out responseMinDate, out responseMaxDate);

                if (responseMaxDate.HasValue && (!curMaxDate.HasValue || responseMaxDate > curMaxDate))
                {
                    curMaxDate = responseMaxDate;
                }
            }

            return curMaxDate;
        }
    }
}
