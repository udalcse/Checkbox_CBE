using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Data calculator for the statistics report item.  The calculator computes responses, mean,
    /// median, mode and standard deviations for responses to rating scale questions.
    /// </summary>
    [Serializable]
    public class StatisticsItemDataCalculator : ReportDataCalculator
    {
        private readonly StatisticsItemReportingOption _reportOption;

        private double? _minPossibleAnswerValue;
        private double? _maxPossibleAnswerValue;

        /// <summary>
        /// Get minimum possible answer value for set of items in aggregator.
        /// </summary>
        public double MinPossibleAnswerValue
        {
            get { return _minPossibleAnswerValue.Value; }
        }

        /// <summary>
        /// Get maximum possible answer value for set of items in aggregator.
        /// </summary>
        public double MaxPossibleAnswerValue
        {
            get { return _maxPossibleAnswerValue.Value; }
        }

        public StatisticsItemDataCalculator(StatisticsItemReportingOption reportOption)
        {
            _reportOption = reportOption;
        }

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
            var resultList = new List<CalculateResult>();

            List<int> sourceItemIds = answerAggregator.GetItemIDs();

            foreach (int itemId in sourceItemIds)
            {
                StatisticsItemResult result = GetItemResult(itemId, answerAggregator) as StatisticsItemResult;

                if (result != null)
                {
                    AddResultToResultList(resultList, itemId, result.Response, result.Mean, result.Median, result.Mode, result.StandardDeviation);
                }
            }

            //Ensure min/max have value
            if (!_minPossibleAnswerValue.HasValue)
            {
                _minPossibleAnswerValue = 1;
            }

            if (!_maxPossibleAnswerValue.HasValue)
            {
                _maxPossibleAnswerValue = 10;
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// Help function to add calculate result to result list
        /// </summary>
        /// <param name="resultList"></param>
        /// <param name="itemId"></param>
        /// <param name="responses"></param>
        /// <param name="mean"></param>
        /// <param name="median"></param>
        /// <param name="mode"></param>
        /// <param name="stdDev"></param>
        private static void AddResultToResultList(List<CalculateResult> resultList, int itemId, int responses, double mean, double median, double mode, double stdDev)
        {
            CalculateResult result = new CalculateResult();
            result.ResultKey = itemId.ToString();
            result.ResultText = "Responses";
            result.ResultValue = responses;
            resultList.Add(result);

            result = new CalculateResult();
            result.ResultKey = itemId.ToString();
            result.ResultText = "Mean";
            result.ResultValue = mean;
            resultList.Add(result);

            result = new CalculateResult();
            result.ResultKey = itemId.ToString();
            result.ResultText = "Median";
            result.ResultValue = median;
            resultList.Add(result);

            result = new CalculateResult();
            result.ResultKey = itemId.ToString();
            result.ResultText = "Mode";
            result.ResultValue = mode;
            resultList.Add(result);

            result = new CalculateResult();
            result.ResultKey = itemId.ToString();
            result.ResultText = "StdDev";
            result.ResultValue = stdDev;
            resultList.Add(result);
        }

        /// <summary>
        /// Overridden GetItemResult(...) method that returns a custom item result class
        /// with average and standard deviation values.
        /// </summary>
        /// <param name="itemId">ID of item to get result for.</param>
        /// <param name="answerAggregator">Answer aggregator containing survey answer data.</param>
        /// <returns>RatingScaleItemResult data object.</returns>
        protected override ItemResult GetItemResult(int itemId, ItemAnswerAggregator answerAggregator)
        {
            StatisticsItemResult result = new StatisticsItemResult();

            List<double> answerValues = GetAnswerValues(itemId, answerAggregator);

            if (answerValues.Count == 0)
            {
                return result;
            }

            switch (_reportOption)
            {
                case StatisticsItemReportingOption.Responses:
                    result.Response = GetItemAnswerCountWithoutNA(itemId, answerAggregator);
                    break;
                case StatisticsItemReportingOption.Mean:
                    result.Mean = ComputeMean(answerValues);
                    break;
                case StatisticsItemReportingOption.Median:
                    result.Median = ComputeMedian(answerValues);
                    break;
                case StatisticsItemReportingOption.Mode:
                    result.Mode = ComputeMode(answerValues);
                    break;
                case StatisticsItemReportingOption.StdDeviation:
                    result.StandardDeviation = ComputeStandardDeviation(answerValues, result.Mean);
                    break;
                case StatisticsItemReportingOption.All:
                    result.Response = GetItemAnswerCountWithoutNA(itemId, answerAggregator);
                    result.Mean = ComputeMean(answerValues);
                    result.Median = ComputeMedian(answerValues);
                    result.Mode = ComputeMode(answerValues);
                    result.StandardDeviation = ComputeStandardDeviation(answerValues, result.Mean);
                    break;
                default:
                    result.Response = GetItemAnswerCountWithoutNA(itemId, answerAggregator);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get a list of all answers for the item converted to doubles.  Handles cases of
        /// open-ended inputs, rating scales with N/A option, and other select types, such
        /// as radio buttons & checkboxes.
        /// </summary>
        /// <param name="itemId">ID of item to list answers for.</param>
        /// <param name="answerAggregator">Answer aggregator containing item meta data
        /// and response answer information.</param>
        /// <returns>List of answers converted to double format.</returns>
        protected List<double> GetAnswerValues(int itemId, ItemAnswerAggregator answerAggregator)
        {
            int itemAnswerCount = answerAggregator.GetItemAnswerCount(itemId);

            if (itemAnswerCount == 0)
            {
                return new List<double>();
            }

            List<double> answers = new List<double>(itemAnswerCount);

            string itemTypeName = answerAggregator.GetItemType(itemId);

            //Get the options for the item.
            List<int> itemOptionIds = answerAggregator.GetOptionIDs(itemId);

            //Handle open-ended
            if (itemOptionIds.Count == 0)
            {
                //List all text answers and convert to double
                List<string> itemTextAnswers = answerAggregator.GetAnswerTexts(itemId);

                foreach (string itemTextAnswer in itemTextAnswers)
                {
                    double? answerTextAsDouble = Utilities.GetDouble(itemTextAnswer);

                    //If conversion was successful, store value
                    if (answerTextAsDouble.HasValue)
                    {
                        answers.Add(answerTextAsDouble.Value);

                        //Also assign min/max value
                        if (!_minPossibleAnswerValue.HasValue || answerTextAsDouble < MinPossibleAnswerValue)
                        {
                            _minPossibleAnswerValue = answerTextAsDouble.Value;
                        }

                        if (!_maxPossibleAnswerValue.HasValue || answerTextAsDouble > MaxPossibleAnswerValue)
                        {
                            _maxPossibleAnswerValue = answerTextAsDouble.Value;
                        }
                    }
                }
            }
            //Handle selection based
            else
            {
                foreach (int itemOptionId in itemOptionIds)
                {
                    bool optionIsOther = answerAggregator.GetOptionIsOther(itemOptionId);

                    //If this is an "other" option and the question is a rating scale, move on to next item.
                    if (itemTypeName.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase) && optionIsOther)
                    {
                        continue;
                    }

                    //Get a count of the number of answers for the option
                    int optionResultCount = GetOptionResult(itemOptionId, answerAggregator).Count;

                    //Otherwise, try to convert option text to double
                    string optionText = answerAggregator.GetOptionText(itemOptionId);

                    double? optionDoubleValue = answerAggregator.GetOptionPoints(itemOptionId);

                    //If option does not appear to have a numeric value, then move on to the next option.
                    if (!optionDoubleValue.HasValue)
                    {
                        continue;
                    }

                    //Otherwise, add it to the answer values array once for each time it was included
                    for (int i = 0; i < optionResultCount; i++)
                    {
                        answers.Add(optionDoubleValue.Value);
                    }

                    //Also assign min/max value
                    if (!_minPossibleAnswerValue.HasValue || optionDoubleValue < MinPossibleAnswerValue)
                    {
                        _minPossibleAnswerValue = optionDoubleValue.Value;
                    }

                    if (!_maxPossibleAnswerValue.HasValue || optionDoubleValue > MaxPossibleAnswerValue)
                    {
                        _maxPossibleAnswerValue = optionDoubleValue.Value;
                    }
                }
            }

            answers.TrimExcess();

            return answers;
        }

        /// <summary>
        /// Compute the mean for a set of answer values.
        /// </summary>
        /// <param name="answerValues">List of answer values.</param>
        /// <returns></returns>
        private static double ComputeMean(List<double> answerValues)
        {
            double runningTotal = 0;

            foreach (double answerValue in answerValues)
                runningTotal += answerValue;

            return runningTotal / answerValues.Count;
        }

        /// <summary>
        /// Compute standard deviation for result set.
        /// </summary>
        /// <param name="answerValues">List of answer values.</param>
        /// <param name="averageValue">Average value of answers.</param>
        /// <returns>Standard deviation for values.</returns>
        private static double ComputeStandardDeviation(List<double> answerValues, double averageValue)
        {
            double runningTotal = 0;

            foreach (double answerValue in answerValues)
            {
                runningTotal += Math.Pow(answerValue - averageValue, 2);
            }

            return Math.Sqrt(runningTotal / answerValues.Count);
        }

        /// <summary>
        /// Compute the median for the list of answer values
        /// </summary>
        /// <param name="answerValues">List of answer values.</param>
        private static double ComputeMedian(List<double> answerValues)
        {
            List<double> uniqueValues = new List<double>();

            foreach (double answerValue in answerValues)
            {
                if (!uniqueValues.Contains(answerValue))
                {
                    uniqueValues.Add(answerValue);
                }
            }
            uniqueValues.Sort();

            int middleIndex = uniqueValues.Count / 2;

            if (uniqueValues.Count % 2 == 0)
            {
                return (uniqueValues[middleIndex] + uniqueValues[middleIndex - 1]) / 2;
            }

            return uniqueValues[middleIndex];
        }

        /// <summary>
        /// Compute the mode for the list of answer values
        /// </summary>
        /// <param name="answerValues">List of answer values.</param>
        private static double ComputeMode(List<double> answerValues)
        {
            double mode = 0;
            int optionCaunter = 0;
            int maxCounter = 0;

            for (int i = 0; i < answerValues.Count; i++)
            {
                if (i != answerValues.Count - 1)
                {
                    if (answerValues[i] == answerValues[i + 1])
                        optionCaunter++;
                    else
                    {
                        if (++optionCaunter >= maxCounter)
                        {
                            maxCounter = optionCaunter;
                            mode = answerValues[i];
                        }
                        optionCaunter = 0;
                    }
                }
                else
                {
                    if (++optionCaunter >= maxCounter)
                    {
                        maxCounter = optionCaunter;
                        mode = answerValues[i];
                    }
                    optionCaunter = 0;
                }
            }

            return mode;
        }

        /// <summary>
        /// Compute count of answers on item without N/A active options
        /// </summary>
        /// <param name="itemId">ID of item to get result for.</param>
        /// <param name="answerAggregator">Answer aggregator containing item meta data
        /// and response answer information.</param>
        private int GetItemAnswerCountWithoutNA(int itemId, ItemAnswerAggregator answerAggregator)
        {
            int answerCounter = 0;
            List<long> responses = answerAggregator.GetResponseIDs();
            foreach (long response in responses)
            {
                List<long> answers = answerAggregator.ResponseAnswerDictionary[response];
                foreach (long answer in answers)
                {
                    if (answerAggregator.AnswerDictionary[answer].ItemId == itemId) //For our item
                    {
                        bool isOther = false;
                        int? activeOptionId = answerAggregator.AnswerDictionary[answer].OptionId;
                        if (activeOptionId != null)
                        {
                            isOther = answerAggregator.GetOptionIsOther((int)activeOptionId);
                        }

                        if (isOther != true)
                            answerCounter++;
                    }
                }
            }
            return answerCounter;
        }

        #region Alternative Calculation

        ///<summary>
        ///</summary>
        ///<param name="answers"></param>
        ///<param name="option"></param>
        ///<param name="minPossibleAnswerValue"></param>
        ///<param name="maxPossibleAnswerValue"></param>
        ///<returns></returns>
        public static CalculateResult[] Calculate(IEnumerable<ItemAnswer> answers, StatisticsItemReportingOption option,
            out double? minPossibleAnswerValue, out double? maxPossibleAnswerValue)
        {
            minPossibleAnswerValue = null;
            maxPossibleAnswerValue = null;

            var resultList = new List<CalculateResult>();

            List<int> sourceItemIds = answers.Select(a => a.ItemId).Distinct().ToList();

            foreach (int itemId in sourceItemIds)
            {
                double ? min, max;
                var result = GetItemResult(GetItemTypeName(itemId), answers.Where(a => a.ItemId == itemId).ToList(), option, out min, out max);

                if (!minPossibleAnswerValue.HasValue || (min.HasValue && minPossibleAnswerValue.Value > min.Value))
                    minPossibleAnswerValue = min;
                if (!maxPossibleAnswerValue.HasValue || (max.HasValue && maxPossibleAnswerValue.Value > max.Value))
                    maxPossibleAnswerValue = max;

                if (result != null)
                    AddResultToResultList(resultList, itemId, result.Response, result.Mean, result.Median, result.Mode, result.StandardDeviation);
            }

            if (!minPossibleAnswerValue.HasValue)
                minPossibleAnswerValue = 1;
            if (!maxPossibleAnswerValue.HasValue)
                maxPossibleAnswerValue = 10;

            return resultList.ToArray();
        }

        private static StatisticsItemResult GetItemResult(string itemTypeName, IEnumerable<ItemAnswer> answers, StatisticsItemReportingOption option,
            out double? min, out double? max)
        {
            StatisticsItemResult result = new StatisticsItemResult();

            List<double> answerValues = GetAnswerValues(answers, itemTypeName, out min, out max);

            if (answerValues.Count == 0)
            {
                return result;
            }

            switch (option)
            {
                case StatisticsItemReportingOption.Responses:
                    result.Response = GetItemAnswerCountWithoutNA(answers);
                    break;
                case StatisticsItemReportingOption.Mean:
                    result.Mean = ComputeMean(answerValues);
                    break;
                case StatisticsItemReportingOption.Median:
                    result.Median = ComputeMedian(answerValues);
                    break;
                case StatisticsItemReportingOption.Mode:
                    result.Mode = ComputeMode(answerValues);
                    break;
                case StatisticsItemReportingOption.StdDeviation:
                    result.StandardDeviation = ComputeStandardDeviation(answerValues, result.Mean);
                    break;
                case StatisticsItemReportingOption.All:
                    result.Response = GetItemAnswerCountWithoutNA(answers);
                    result.Mean = ComputeMean(answerValues);
                    result.Median = ComputeMedian(answerValues);
                    result.Mode = ComputeMode(answerValues);
                    result.StandardDeviation = ComputeStandardDeviation(answerValues, result.Mean);
                    break;
                default:
                    result.Response = GetItemAnswerCountWithoutNA(answers);
                    break;
            }

            return result;
        }

        private static List<double> GetAnswerValues(IEnumerable<ItemAnswer> answers, string itemTypeName,
            out double? min, out double? max)
        {
            int itemAnswerCount = answers.Count();

            min = null;
            max = null;

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
                    {
                        answerValues.Add(answerTextAsDouble.Value);

                        //Also assign min/max value
                        if (!min.HasValue || min.Value > answerTextAsDouble.Value)
                            min = answerTextAsDouble.Value;

                        if (!max.HasValue || max.Value < answerTextAsDouble.Value)
                            max = answerTextAsDouble.Value;
                    }
                }
            }
            //Handle selection based
            else
            {
                foreach (var answer in answers)
                {
                    //If this is an "other" option and the question is a rating scale, move on to next item.
                    if (itemTypeName.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase) && answer.IsOther)
                        continue;

                    //If option does not appear to have a numeric value, then move on to the next option.
                    if (!answer.Points.HasValue)
                        continue;

                    //Otherwise, add it to the answer values array once for each time it was included
                    for (int i = 0; i < answer.Count; i++)
                    {
                        answerValues.Add(answer.Points.Value);
                    }

                    //Also assign min/max value
                    if (!min.HasValue || min.Value > answer.Points.Value)
                        min = answer.Points.Value;

                    if (!max.HasValue || max.Value < answer.Points.Value)
                        max = answer.Points.Value;
                }
            }

            answerValues.TrimExcess();

            return answerValues;
        }

        private static string GetItemTypeName(int itemID)
        {
            return SurveyMetaDataProxy.GetItemTypeName(itemID, true);
        }

        private static int GetItemAnswerCountWithoutNA(IEnumerable<ItemAnswer> answers)
        {
            return answers.Where(a => !a.IsOther).Sum(a => a.Count);
        }
        
        #endregion
    }
}
