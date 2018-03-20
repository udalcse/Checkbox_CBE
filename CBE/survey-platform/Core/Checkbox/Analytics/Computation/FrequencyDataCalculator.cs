using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Data calculator for frequency items
    /// </summary>
    public class FrequencyDataCalculator : ReportDataCalculator
    {
        /// <summary>
        /// Aggregate the frequencies
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

                //If the item has no options, assume open-ended, otherwise assume select
                if (optionIDs.Count > 0)
                {
                    results.AddRange(from optionID in optionIDs
                                     let result = GetOptionResult(optionID, answerAggregator)
                                     select new AggregateResult
                                                {
                                                    //ItemId = itemID, 
                                                    //OptionId = optionID, 
                                                    //ItemText = itemText, 
                                                    //OptionText = answerAggregator.GetOptionText(optionID), 
                                                    ResultKey = itemID + "_" + optionID,
                                                    AnswerCount = result.Count, 
                                                    AnswerPercent = Convert.ToDouble(result.Percentage), 
                                                    ResultText = answerAggregator.GetOptionText(optionID)
                                                });
                }
                else
                {
                    //Summarize the result for each answer
                    List<string> answerTexts = answerAggregator.GetAnswerTexts(itemID);

                    results.AddRange(from answerText in answerTexts
                                     let result = GetItemResult(itemID, answerText, answerAggregator)
                                     select new AggregateResult
                                                {
                                                    //ItemId = itemID, 
                                                    //ItemText = itemText, 
                                                    ResultKey = itemID.ToString(),
                                                    ResultText = answerText, 
                                                    AnswerCount = result.Count, 
                                                    AnswerPercent = Convert.ToDouble(result.Percentage)
                                                });
                }
            }

            return results.ToArray();
        }
    }
}
