using System;


namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NotAnsweredFilter : AnsweredFilter
    {
        /// <summary>
        /// Query is same as answered, but using NOT IN instead of IN
        /// </summary>
        public override bool UseNotIn
        {
            get { return true; }
        }

        /// <summary>
        /// Get whether a value is required or not
        /// </summary>
        protected override bool ValueRequired { get { return false; } }
    }
}
