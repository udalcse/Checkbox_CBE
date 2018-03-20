using System;

namespace Prezza.Framework.Common
{
    /// <summary>
    /// Simple container class representing a date range
    /// </summary>
    public class DateRange
    {
        private DateTime _low;
        private DateTime _high;

        /// <summary>
        /// Constructor accepting low and high value for date range.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public DateRange(DateTime low, DateTime high)
        {
            _low = low;
            _high = high;
        }

        /// <summary>
        /// Low end of date range
        /// </summary>
        public DateTime Low
        {
            get { return _low; }
        }

        /// <summary>
        /// High end of date range
        /// </summary>
        public DateTime High
        {
            get { return _high; }
        }
    }
}
