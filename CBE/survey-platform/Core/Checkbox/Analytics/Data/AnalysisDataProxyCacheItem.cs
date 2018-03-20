using System;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Representation of an analysis data cache item which simply supports storing
    /// an <see cref="AnalysisData"/> object and a last-accessed date.
    /// </summary>
    public class AnalysisDataProxyCacheItem<T>
    {
        /// <summary>
        /// Get/set data for the cache item
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Get/set reference date for item.
        /// </summary>
        public DateTime ReferenceDate { get; set; }
    }
}
