using System;

using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms.Logic;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter
    /// </summary>
    [Serializable]
    public abstract class Filter
    {
        ///<summary>
        ///</summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Get the filter id
        /// </summary>
        public int FilterId { get; private set; }

        /// <summary>
        /// Get the object value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Get/set the logical operator
        /// </summary>
        public LogicalOperator Operator { get; set; }

        /// <summary>
        /// Get the filter text
        /// </summary>
        public string FilterText { get; private set; }

        /// <summary>
        /// Configure the filter with its data
        /// </summary>
        /// <param name="filterData"></param>
        /// <param name="languageCode"></param>
        public virtual void Configure(FilterData filterData, string languageCode)
        {
            FilterId = filterData.ID.Value;
            Value = filterData.Value;
            Operator = filterData.Operator;
            LanguageCode = languageCode;
            FilterText = filterData.ToString(languageCode);
        }

        /// <summary>
        /// Compare the filter value against the specified value to compare
        /// </summary>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        protected virtual bool CompareValue(object valueToCompare)
        {
            string valueToCompareAsString = null;
            string valueAsString = null;

            if (Value != null)
            {
                valueAsString = Value.ToString();
            }

            if (valueToCompare != null)
            {
                valueToCompareAsString = valueToCompare.ToString();
            }

            StringOperand left = new StringOperand(valueToCompareAsString);
            StringOperand right = new StringOperand(valueAsString);

            return OperandComparer.Compare(left, Operator, right, null);
        }

        /// <summary>
        /// Parameter name for stored procedure to extract data
        /// </summary>
        public virtual string FilterParameter
        {
            get
            {
                return "FilterString";
            }
        }
    }
}
