using System;
using Checkbox.Analytics.Data;
using Checkbox.Common;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Logic;
using Checkbox.Globalization;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter based on response properties.
    /// </summary>
    public class ResponseFilter : AnswerDataObjectFilter, IQueryFilter
    {
        /// <summary>
        /// Get the profile property name
        /// </summary>
        protected string PropertyName { get; private set; }

        /// <summary>
        /// Configure the filter
        /// </summary>
        /// <param name="filterData"></param>
        /// <param name="languageCode"></param>
        public override void Configure(FilterData filterData, string languageCode)
        {
            base.Configure(filterData, languageCode);

            if (filterData is ResponseFilterData)
            {
                PropertyName = ((ResponseFilterData)filterData).PropertyName;
            }
        }

        /// <summary>
        /// Evaluate the filterf for the row
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties"></param>
        /// <param name="hasValue"></param>
        /// <returns></returns>
        public override bool EvaluateFilter(ItemAnswer answer, AnalysisAnswerData answerData, ResponseProperties responseProperties, out bool hasValue)
        {
            string valueAsString = responseProperties.GetStringValue(PropertyName);
            
            hasValue = Utilities.IsNotNullOrEmpty(valueAsString);

            return CompareValue(valueAsString);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseNotIn
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Filter string to be used in SQL
        /// </summary>
        public virtual string FilterString
        {
            get
            {
                if (Operator == LogicalOperator.Contains)
                {
                    return string.Format("cast(ckbx_Response.{0} as nvarchar(1024)) like '%{1}%'", PropertyName, Value.ToString().Replace("'", ""));
                }
                if (Operator == LogicalOperator.DoesNotContain)
                {
                    return string.Format("not(cast(ckbx_Response.{0} as nvarchar(1024)) like '%{1}%')", PropertyName, Value.ToString().Replace("'", ""));
                }

                if (OperatorAsString == null)
                    return "";
                return ValueRequired ? string.Format("ckbx_Response.{0} {1} {2}", PropertyName, OperatorAsString, WrappedValue)
                    : string.Format("ckbx_Response.{0} {1}", PropertyName, OperatorAsString);                 
            }
        }

        /// <summary>
        /// Operator as SQL string
        /// </summary>
        public string OperatorAsString
        {
            get
            {
                switch (Operator)
                {
                    case LogicalOperator.Equal:
                        return " = ";
                    case LogicalOperator.GreaterThan:
                        return " > ";
                    case LogicalOperator.GreaterThanEqual:
                        return " >= ";
                    case LogicalOperator.IsNotNull:
                        return " is not null ";
                    case LogicalOperator.IsNull:
                        return " is null ";
                    case LogicalOperator.LessThan:
                        return " < ";
                    case LogicalOperator.LessThanEqual:
                        return " <= ";
                    case LogicalOperator.NotEqual:
                        return " <> ";
                }
                return null;
            }
        }

        /// <summary>
        /// Wrapped and purified value
        /// </summary>
        public string WrappedValue 
        {
            get
            {
                var dateTime = Utilities.GetDate(Value.ToString());
                if (dateTime.HasValue)
                    return string.Format("'{0}'", dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                if ("ResponseID".Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase) ||
                    "LastPageViewed".Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Value.ToString().Replace("'", "");
                }
                        
                return string.Format("'{0}'", Value.ToString().Replace("'", ""));
            }
        }
    }
}
