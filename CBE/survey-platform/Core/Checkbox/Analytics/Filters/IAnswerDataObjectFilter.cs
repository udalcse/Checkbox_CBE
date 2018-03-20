using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Forms;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// IAnswerDataObjectFilter are capable of filtering answer data objects for analysis reports for 
    /// an individual response
    /// </summary>
    interface IAnswerDataObjectFilter
    {
        /// <summary>
        /// Return a boolean indicating if the specified answers for a response fulfill the filter parameters.
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties"></param>
        /// <returns></returns>
        bool EvaluateFilter(List<ItemAnswer> answers, AnalysisAnswerData answerData, Dictionary<long, ResponseProperties> responseProperties);
    }
}
