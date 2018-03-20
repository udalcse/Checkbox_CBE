using System;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter that checks whether a question has been answered or not.
    /// </summary>
    [Serializable]
    public class AnsweredFilter : ItemQueryFilter
    {
        /// <summary>
        /// This filter requires an answer value to evaluate to true
        /// </summary>
        protected override bool ValueRequired { get { return true; } }

        /// <summary>
        /// Get a where clause suitable to append to a load answer datq query for more efficient report-wide filter evaluation.
        /// </summary>
        /// <returns>String suitable for addition to a SQL where clause.</returns>
        protected override string GetValueFilterClause()
        {
            return " (AnswerText IS NOT NULL AND NOT AnswerText LIKE '') OR OptionID IS NOT NULL";
        }

        /// <summary>
        /// Determines if in/not in should be used when constructing the query used to filter results.
        /// </summary>
        public override bool UseNotIn
        {
            get { return false; }
        }
    }
}
