using System;
using System.Data;
using System.Text;

using Checkbox.Common;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter for not equal
    /// </summary>
    [Serializable()]
    public class NotEqualFilter : ItemQueryFilter
    {
        /// <summary>
        /// Get the value filter clause for the query filter.
        /// </summary>
        /// <returns></returns>
        protected override string GetValueFilterClause()
        {
            //Handle open-ended and options. If Value is numeric, consider option id case too
            Nullable<int> intValue = null;

            if (Value != null)
            {
                intValue = Utilities.AsInt(Value.ToString());
            }

            StringBuilder sb = new StringBuilder();

            if (intValue.HasValue)
            {
                sb.Append(" (OptionID IS NULL AND AnswerText LIKE '");
                sb.Append(GetEscapedValueString());
                sb.Append("') OR (");
                sb.Append(" OptionID IS NOT NULL AND OptionID = ");
                sb.Append(GetEscapedValueString());
                sb.Append(")");
            }
            else
            {
                sb.Append(" (OptionID IS NULL AND AnswerText LIKE '");
                sb.Append(GetEscapedValueString());
                sb.Append("')");
            }

            return sb.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        public override bool UseNotIn
        {
            get { return true; }
        }
    }
}
