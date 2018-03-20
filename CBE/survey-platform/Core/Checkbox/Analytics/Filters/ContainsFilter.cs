using System;
using System.Text;

using Checkbox.Common;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter to evaluate whether values are equivalent.
    /// </summary>
    [Serializable]
    public class ContainsFilter : ItemQueryFilter
    {
        /// <summary>
        /// Any answer can match for "equal" to be true
        /// </summary>
        protected override FilterMode Mode
        {
            get { return FilterMode.Any; }
        }
        /// <summary>
        /// Get the value filter clause for the query filter.
        /// </summary>
        /// <returns>String clause suitable for addition to a SQL where clause.</returns>
        protected override string GetValueFilterClause()
        {

            var value = GetEscapedValueString();
            long tmp = 0;
            var isKey = long.TryParse(value, out tmp);

            var sb = new StringBuilder();

            sb.AppendFormat(isKey
                                ? " OptionID = {0}"
                                : "AnswerText like '%{0}%' ", value);

            return sb.ToString();
        }

        /// <summary>
        /// Specify whether filter is a "NOT IN" or "IN" filter for purposes of construction a SQL where clause.
        /// </summary>
        public override bool UseNotIn
        {
            get { return false; }
        }
    }
}
