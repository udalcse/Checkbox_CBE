using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Forms;
using Checkbox.Forms.Logic;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Base class representation of a filter that is capable of operating on <see cref="ItemAnswer"/> objects directly.
    /// </summary>
    [Serializable]
    public abstract class AnswerDataObjectFilter : Filter, IAnswerDataObjectFilter
    {
        /// <summary>
        /// Used to specify whether all answer objects or any answer
        /// object must pass the filter to succeed
        /// </summary>
        [Serializable]
        protected enum FilterMode
        {
            /// <summary>
            /// All answer rows must match filter
            /// </summary>
            All,

            /// <summary>
            /// Any answer row match is sufficient
            /// </summary>
            Any
        }

        /// <summary>
        /// Evaluate the filter for the specified answer objects.
        /// </summary>
        /// <param name="answers">Answers to evaluate.</param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties">Response properties to use for evaluating response property filters or to look up the respondent information
        /// to evaluate user profile filters.</param>
        /// <returns>Boolean value indicating if the provided answers meet the filter's criteria.</returns>
        public virtual bool EvaluateFilter(List<ItemAnswer> answers, AnalysisAnswerData answerData, Dictionary<long, ResponseProperties> responseProperties)
        {
            bool allPassed = true;
            bool hasValue = false;

            foreach (ItemAnswer answer in answers)
            {
                ResponseProperties props = responseProperties.ContainsKey(answer.ResponseId) ? responseProperties[answer.ResponseId] : null;
                bool answerHasValue;
                bool answerFilterPassed = EvaluateFilter(answer, answerData, props, out answerHasValue);

                //If only one row is necessary to pass, then return true as soon
                // as filter evaluates to true.
                if (answerFilterPassed && Mode == FilterMode.Any)
                {
                    return true;
                }

                //Update all passed bool
                allPassed = allPassed & answerFilterPassed;

                //Update has values boolean
                hasValue = hasValue | answerHasValue;
            }


            //If this type of filter requires a value present to evaluate to true, then
            // return false if there were no answer values
            if (!hasValue && ValueRequired)
            {
                return false;
            }

            return allPassed;
        }

        /// <summary>
        /// Evaluate a filter for a single answer and provide an indication whether the provided answer had a value or not.
        /// </summary>
        /// <param name="answer">Answer to evaluate.</param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties">Response properties to use for evaluating response property filters or to look up the respondent information
        /// to evaluate user profile filters.</param>
        /// <param name="answerHasValue">Boolean value indicating whether the input answer parameter contained an answer value or not.</param>
        /// <returns></returns>
        public abstract bool EvaluateFilter(ItemAnswer answer, AnalysisAnswerData answerData, ResponseProperties responseProperties, out bool answerHasValue);


        /// <summary>
        /// Determine if the filter can only evaluate to "true" (i.e. success) if the provided answer object has an answer value.
        /// </summary>
        protected virtual bool ValueRequired
        {
            get
            {
                if (Operator == LogicalOperator.Contains
                    || Operator == LogicalOperator.GreaterThan
                    || Operator == LogicalOperator.GreaterThanEqual
                    || Operator == LogicalOperator.LessThan
                    || Operator == LogicalOperator.LessThanEqual
                    || Operator == LogicalOperator.Equal
                    || Operator == LogicalOperator.NotEqual)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Determine filter mode based on the logical operator associated with the filter.
        /// </summary>
        protected virtual FilterMode Mode
        {
            get
            {
                if (Operator == LogicalOperator.Equal
                    || Operator == LogicalOperator.Answered
                    || Operator == LogicalOperator.Contains
                    || Operator == LogicalOperator.GreaterThan
                    || Operator == LogicalOperator.GreaterThanEqual
                    || Operator == LogicalOperator.LessThan
                    || Operator == LogicalOperator.LessThanEqual)
                {
                    return FilterMode.Any;
                }

                return FilterMode.All;
            }
        }
    }
}
