using System;
using System.Data;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filters IAnswerRowFilter are capable of filtering answer rows for analysis reports for 
    /// an individual response
    /// </summary>
    interface IAnswerRowFilter
    {
        /// <summary>
        /// Return a boolean indicating if the specified answer rows fulfill the filter parameters.
        /// </summary>
        /// <param name="answerRows"></param>
        /// <returns></returns>
        bool EvaluateFilter(DataRow[] answerRows);
    }
}
