using System;
using System.Text;

using Checkbox.Common;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter to evaluate whether values are equivalent.
    /// </summary>
    [Serializable]
    public class LessFilter : ItemQueryFilter
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

            string val = GetEscapedValueString();
            if (intValue.HasValue)
            {
                sb.AppendFormat(@" 
                    (OptionID IS NULL AND (isnumeric(cast(AnswerText as varchar(20))) = 1 AND convert(decimal, cast(AnswerText as varchar(20))) < {0})                 
                                         OR OptionID IS NULL AND (isnumeric(cast(AnswerText as varchar(20))) = 0 AND cast(AnswerText as varchar(255)) < '{0}'))", val);
                sb.AppendFormat("OR (OptionID IS NOT NULL AND OptionID < {0})", val);
            }
            else
            {
                sb.Append(" (OptionID IS NULL AND cast(AnswerText as varchar(255)) < '");
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
