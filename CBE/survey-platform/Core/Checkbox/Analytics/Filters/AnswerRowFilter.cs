using System;
using System.Data;

using Checkbox.Forms.Logic;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Answer row filter
    /// </summary>
    [Serializable]
    public abstract class AnswerRowFilter : Filter, IAnswerRowFilter
    {
        /// <summary>
        /// Used to specify whether all answer rows or any answer
        /// rows must pass the filter to succeed
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
        /// Evaluate the filter for the answer rows
        /// </summary>
        /// <param name="answerRows"></param>
        /// <returns></returns>
        public virtual bool EvaluateFilter(DataRow[] answerRows)
        {
            bool allPassed = true;
            bool hasValue = false;

            foreach (DataRow answerRow in answerRows)
            {
                bool rowHasValue = false;
                bool rowFilterPassed = EvaluateFilter(answerRow, out rowHasValue);

                //If only one row is necessary to pass, then return true as soon
                // as filter evaluates to true.
                if(rowFilterPassed && Mode == FilterMode.Any)
                {
                    return true;
                }

                //Update all passed bool
                allPassed = allPassed & rowFilterPassed;

                //Update has values boolean
                hasValue = hasValue | rowHasValue;
            }


            //If this type of filter requires a value present to evaluate to true, then
            // return false if there were no values
            if (!hasValue && ValueRequired)
            {
                return false;
            }
            else
            {
                return allPassed;
            }
        }

        /// <summary>
        /// Evaluate a filter
        /// </summary>
        /// <param name="answerRow"></param>
        /// <param name="rowHasAnswer"></param>
        /// <returns></returns>
        public abstract bool EvaluateFilter(DataRow answerRow, out bool rowHasAnswer);


        /// <summary>
        /// Determine if an answer is required based on the operator
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
                    || Operator == LogicalOperator.Equal)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determine filter mode based on the operator
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
                else
                {
                    return FilterMode.All;
                }
            }
        }
    }
}