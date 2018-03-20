using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// 
    /// </summary>
    public class NetPromoterScoreItemDataCalculator : ReportDataCalculator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            return new AggregateResult[0];
        }

        /// <summary>
        /// Calculate responses, means, medians, modes and standard deviations and set 
        /// result into answer aggregator.
        /// </summary>
        /// <param name="answerAggregator">Aggregated answers to rating scale items
        /// to use for the computations.</param>
        /// <returns>DataTable containing calculated results.</returns>
        protected override CalculateResult[] Calculate(ItemAnswerAggregator answerAggregator)
        {
            return Calculate(answerAggregator.AnswerDictionary.Values);
        }

        /// <summary>
        /// Calculate responses, means, medians, modes and standard deviations and set 
        /// result into answer aggregator.
        /// </summary>
        /// <param name="answers"></param>
        /// <returns>DataTable containing calculated results.</returns>
        protected override CalculateResult[] Calculate(IEnumerable<ItemAnswer> answers)
        {
            var resultList = new List<CalculateResult>();
            var sourceItemIds = answers.Select(a => a.ItemId).Distinct().ToList();

            foreach (int itemId in sourceItemIds)
            {
                var result = GetItemResult(answers.Where(a => a.ItemId == itemId)) as NetPromoterScoreItemResult;
                if (result != null)
                    AddResultToResultList(resultList, itemId, result.Detractors, result.Passive, result.Promoters, result.NetPromoterScore);
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// Help function to add calculate result to result list
        /// </summary>
        /// <param name="resultList"></param>
        /// <param name="itemId"></param>
        /// <param name="detractors"></param>
        /// <param name="passive"></param>
        /// <param name="promoters"></param>
        /// <param name="score"></param>
        private void AddResultToResultList(List<CalculateResult> resultList, int itemId, int detractors, int passive, int promoters, double score)
        {
            var result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "Detractors",
                ResultValue = detractors
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "Passives",
                ResultValue = passive
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "Promoters",
                ResultValue = promoters
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "Net Promoter Score",
                ResultValue = score
            };
            resultList.Add(result);
        }

        /// <summary>
        /// Overridden GetItemResult(...) method that returns a custom item result class
        /// with average and standard deviation values.
        /// </summary>
        /// <returns>RatingScaleItemResult data object.</returns>
        protected ItemResult GetItemResult(IEnumerable<ItemAnswer> answers)
        {
            var result = new NetPromoterScoreItemResult();

            var answerValues = GetAnswerValues(answers);
            if (answerValues.Count == 0)
                return result;

            var totalCount = answers.Where(a => a.Points >= 0 && a.Points <= 10).Sum(a => a.Count);
            var detractors = answers.Where(a => a.Points >= 0 && a.Points <= 6).Sum(a => a.Count);
            var passive = answers.Where(a => a.Points >= 7 && a.Points <= 8).Sum(a => a.Count);
            var promoters = answers.Where(a => a.Points >= 9 && a.Points <= 10).Sum(a => a.Count);

            result.TotalAnswers = totalCount;
            result.Detractors = detractors;
            result.Passive = passive;
            result.Promoters = promoters;
            result.NetPromoterScore = ComputeNetPromoterScore(totalCount, promoters, detractors);

            return result;
        }

        private double ComputeNetPromoterScore(int total, int promoters, int detractors)
        {
            return ((promoters*100d)/total - (detractors*100d)/total);
        }

        private List<double> GetAnswerValues(IEnumerable<ItemAnswer> answers)
        {
            int itemAnswerCount = answers.Count();

            if (itemAnswerCount == 0)
                return new List<double>();

            List<double> answerValues = new List<double>(itemAnswerCount);

            //Get the options for the item.
            List<int?> itemOptionIds = answers.Where(a => a.OptionId > -1).Select(a => a.OptionId).Distinct().ToList();

            //Handle open-ended
            if (itemOptionIds.Count == 0)
            {
                //List all text answers and convert to double
                List<string> itemTextAnswers = answers.Select(a => a.AnswerText).ToList();

                foreach (string itemTextAnswer in itemTextAnswers)
                {
                    double? answerTextAsDouble = Utilities.GetDouble(itemTextAnswer);

                    //If conversion was successful, store value
                    if (answerTextAsDouble.HasValue)
                        answerValues.Add(answerTextAsDouble.Value);
                }
            }
            //Handle selection based
            else
            {
                foreach (var answer in answers)
                {
                    //If option does not appear to have a numeric value, then move on to the next option.
                    if (!answer.Points.HasValue)
                        continue;

                    //Otherwise, add it to the answer values array once for each time it was included
                    for (int i = 0; i < answer.Count; i++)
                    {
                        answerValues.Add(answer.Points.Value);
                    }
                }
            }

            answerValues.TrimExcess();

            return answerValues;
        }


    }
}
