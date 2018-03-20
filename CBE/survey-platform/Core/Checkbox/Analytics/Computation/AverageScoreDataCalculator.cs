using System.Linq;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Calculator for getting average score information for reports
    /// </summary>
    public class AverageScoreDataCalculator : ReportDataCalculator
    {
        /// <summary>
        /// Aggregate and return the analysis data
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            return (from groupedAnswer in answerAggregator.GetAggregatedAnswerData().GroupBy(answer => answer.ItemId)
                                    let responseCount = answerAggregator.GetResponseCount(groupedAnswer.Key)
                                    let scoreSum = groupedAnswer.Where(detail => !detail.IsAnswerOther).Sum(detail => detail.AnswerScore)
                                    select new AggregateResult
                                               {
                                                   ResultKey = groupedAnswer.Key.ToString(),
                                                   ResultText = answerAggregator.GetItemText(groupedAnswer.Key), 
                                                   AnswerPercent = responseCount > 0 ? scoreSum/(double) responseCount : 0
                                               }).ToArray();
        }
    }
}
