using System;
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
    public class NetPromoterScoreStatisticsItemDataCalculator : ReportDataCalculator
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalResponsesCount { set; get; }

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
        /// </summary>
        /// <param name="answerAggregator">Aggregated answers to rating scale items
        /// to use for the computations.</param>
        /// <returns>DataTable containing calculated results.</returns>
        protected override CalculateResult[] Calculate(ItemAnswerAggregator answerAggregator)
        {
            var sourceItemIds = answerAggregator.AnswerDictionary.Values.Select(a => a.ItemId).Distinct().ToList();
            foreach (var itemId in sourceItemIds)
            {
                TotalResponsesCount += answerAggregator.GetResponseCount(itemId);
            }

            return Calculate(answerAggregator.AnswerDictionary.Values);
        }

        /// <summary>
        /// </summary>
        /// <param name="answers"></param>
        /// <returns>DataTable containing calculated results.</returns>
        protected override CalculateResult[] Calculate(IEnumerable<ItemAnswer> answers)
        {
            var resultList = new List<CalculateResult>();
            var sourceItemIds = answers.Select(a => a.ItemId).Distinct().ToList();

            foreach (int itemId in sourceItemIds)
            {
                var result = GetItemResult(answers.Where(a => a.ItemId == itemId)) as NetPromoterScoreStatisticsItemResult;
                if (result != null)
                    AddResultToResultList(resultList, itemId, result.MinValue, result.MaxValue, result.AverageValue, result.Variance, result.StandardDeviation);
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// Help function to add calculate result to result list
        /// </summary>
        private void AddResultToResultList(List<CalculateResult> resultList, int itemId, int min, int max, double average, double variance, double deviation)
        {
            var result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "MinValue",
                ResultValue = min
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "MaxValue",
                ResultValue = max
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "AverageValue",
                ResultValue = average
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "Variance",
                ResultValue = variance
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "StandardDeviation",
                ResultValue = deviation
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "TotalResponses",
                ResultValue = TotalResponsesCount
            };
            resultList.Add(result);

            result = new CalculateResult
            {
                ResultKey = itemId.ToString(),
                ResultText = "TotalRespondents",
                ResultValue = 0
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
            var result = new NetPromoterScoreStatisticsItemResult();

            var answerValues = GetAnswerValues(answers);
            if (answerValues.Count == 0)
                return result;

            var min = answerValues.Min();
            var max = answerValues.Max();
            var average = answerValues.Average();
            var variance = ComputeVariance(answerValues, 0, answerValues.Count);
            var deviation = ComputeStandardDeviation(answerValues, 0, answerValues.Count);

            result.MinValue = min;
            result.MaxValue = max;
            result.AverageValue = average;
            result.Variance = variance;
            result.StandardDeviation = deviation;

            return result;
        }

        private double ComputeMean(List<int> answerValues, int start, int end)
        {
            var s = 0d;

            for (int i = start; i < end; i++)
            {
                s += answerValues[i];
            }

            return s / (end - start);
        }

        private double ComputeVariance(List<int> answerValues, int start, int end)
        {
            double mean = ComputeMean(answerValues, start, end);
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((answerValues[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        /// <summary>
        /// Compute standard deviation for result set.
        /// </summary>
        private double ComputeStandardDeviation(List<int> answerValues, int start, int end)
        {
            var variance = ComputeVariance(answerValues, start, end);
            return Math.Sqrt(variance);
        }

        private List<int> GetAnswerValues(IEnumerable<ItemAnswer> answers)
        {
            int itemAnswerCount = answers.Count();

            if (itemAnswerCount == 0)
                return new List<int>();

            var answerValues = new List<int>(itemAnswerCount);

            //Get the options for the item.
            List<int?> itemOptionIds = answers.Where(a => a.OptionId > -1).Select(a => a.OptionId).Distinct().ToList();

            //Handle open-ended
            if (itemOptionIds.Count == 0)
            {
                //List all text answers and convert to double
                List<string> itemTextAnswers = answers.Select(a => a.AnswerText).ToList();

                foreach (string itemTextAnswer in itemTextAnswers)
                {
                    var answerTextAsInt = Utilities.GetInt32(itemTextAnswer);

                    //If conversion was successful, store value
                    if (answerTextAsInt.HasValue)
                        answerValues.Add(answerTextAsInt.Value);
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
                        answerValues.Add((int)answer.Points.Value);
                    }
                }
            }

            answerValues.TrimExcess();

            return answerValues;
        }

    }
}
