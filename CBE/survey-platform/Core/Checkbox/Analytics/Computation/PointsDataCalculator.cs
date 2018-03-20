using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Data calculator for RankOrderSummary item.
    /// </summary>
    public class PointsDataCalculator : ReportDataCalculator
    {
        /// <summary>
        /// Aggregate the points
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            var results = new List<AggregateResult>();
            List<int> itemIDs = answerAggregator.GetItemIDs();

            foreach (int itemID in itemIDs)
            {
                List<int> optionIDs = answerAggregator.GetOptionIDs(itemID);

                double totalOptionsForItem = answerAggregator.GetItemSumPoints(itemID);

                results.AddRange(from optionID in optionIDs
                                 let result = GetOptionSumPoints(optionID, answerAggregator, totalOptionsForItem)
                                 select new AggregateResult
                                            {
                                                //ItemId = itemID, 
                                                //OptionId = optionID, 
                                                //ItemText = itemText, 
                                                //OptionText = answerAggregator.GetOptionText(optionID), 
                                                ResultKey = itemID + "_" + optionID,
                                                //The points for rankOrder item are integers actually. So we can cast double to int.
                                                AnswerCount = Convert.ToInt32(result.SumPoints),
                                                Points = result.SumPoints,
                                                AnswerPercent = Convert.ToDouble(result.Percentage),
                                                ResultText = answerAggregator.GetOptionText(optionID)
                                            });


            }

            return results.ToArray();
        }
    }
}
