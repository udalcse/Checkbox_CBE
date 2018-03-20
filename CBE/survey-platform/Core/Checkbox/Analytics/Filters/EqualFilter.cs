using System;
using System.Text;

using Checkbox.Common;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter to evaluate whether values are equivalent.
    /// </summary>
    [Serializable]
    public class EqualFilter : ItemQueryFilter
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
            //Handle open-ended and options. If Value is numeric, consider option id case too
            int? intValue = null;

            if (Value != null)
            {
                intValue = Utilities.AsInt(Value.ToString());
            }

            StringBuilder sb = new StringBuilder();

            if (intValue.HasValue)
            {
                sb.Append(" (OptionID IS NULL AND AnswerText Like '");
                sb.Append(GetEscapedValueString());
                sb.Append("') OR (");
                sb.Append(" OptionID IS NOT NULL AND OptionID = ");
                sb.Append(GetEscapedValueString());
                sb.Append(")");
            }
            else
            {
                sb.Append(" (OptionID IS NULL AND AnswerText Like '");
                sb.Append(GetEscapedValueString());
                sb.Append("')");
            }

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
