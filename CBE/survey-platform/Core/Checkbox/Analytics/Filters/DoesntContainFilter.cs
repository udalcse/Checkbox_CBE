using System;
using System.Text;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter to evaluate whether values are equivalent.
    /// </summary>
    [Serializable]
    public class DoesntContainFilter : ItemQueryFilter
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
            string value = GetEscapedValueString();
            long tmp = 0;
            bool isKey = long.TryParse(value, out tmp);

            StringBuilder sb = new StringBuilder();

            //sb.AppendFormat(isKey ? " (ResponseID not in (Select ResponseID FROM ckbx_ResponseAnswers ra where OptionID = {0})) "
            //    : " (ResponseID not in (Select ResponseID FROM ckbx_ResponseAnswers ra where AnswerText like '%{0}%')) ", value);
            sb.AppendFormat(isKey ? " OptionID = {0}"
                : " AnswerText like '%{0}%' ", value);

            return sb.ToString();
        }

        /// <summary>
        /// Specify whether filter is a "NOT IN" or "IN" filter for purposes of construction a SQL where clause.
        /// </summary>
        public override bool UseNotIn
        {
            get { return true; }
        }
    }
}
