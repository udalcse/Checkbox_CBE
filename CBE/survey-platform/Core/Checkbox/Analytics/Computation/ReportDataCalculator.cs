using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Aggregate report data based on item answers.
    /// </summary>
    public abstract class ReportDataCalculator
    {
        /// <summary>
        /// Get the report data which includes details data and report data
        /// </summary>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        public virtual AnalysisItemResult GetData(ItemAnswerAggregator aggregator)
        {
            return new AnalysisItemResult
            {
                ItemResponseCounts = CountResponses(aggregator),
                ItemAnswerCounts = CountAnswers(aggregator),
                AggregateResults = Aggregate(aggregator),
                CalculateResults = Calculate(aggregator)
            };
        }

        /// <summary>
        /// Get the report data which includes details data and report data
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public virtual AnalysisItemResult GetData(IEnumerable<ItemAnswer> answers)
        {
            return new AnalysisItemResult
            {
                AggregateResults = Aggregate(answers),
                CalculateResults = Calculate(answers)
            };
        }

        /// <summary>
        /// Aggregate report data based on the provided answers. 
        /// </summary>
        /// <param name="answerAggregator">Item Answer aggregator with answers</param>
        /// <returns>Array of aggregated data.</returns>
        protected abstract AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator);

        /// <summary>
        /// Aggregate report data based on the provided answers. 
        /// </summary>
        /// <returns>Array of aggregated data.</returns>
        protected virtual AggregateResult[] Aggregate(IEnumerable<ItemAnswer> answers)
        {
            return new AggregateResult[0];
        }

        /// <summary>
        /// Calculate report data based on the provided answers. 
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected virtual CalculateResult[] Calculate(ItemAnswerAggregator answerAggregator)
        {
            return new CalculateResult[0];
        }

        /// <summary>
        /// Calculate report data based on the provided answers. 
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        protected virtual CalculateResult[] Calculate(IEnumerable<ItemAnswer> answers)
        {
            return new CalculateResult[0];
        }

        /// <summary>
        /// Count all responses for items.
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected virtual Dictionary<int, int> CountResponses(ItemAnswerAggregator answerAggregator)
        {
            var responseCounts = new Dictionary<int, int>();
            List<int> itemIds = answerAggregator.GetItemIDs();

            foreach (int itemId in itemIds)
            {
                responseCounts[itemId] = answerAggregator.GetResponseCount(itemId);
            }

            return responseCounts;
        }

        /// <summary>
        /// Count all answers for items.
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected virtual Dictionary<int, int> CountAnswers(ItemAnswerAggregator answerAggregator)
        {
            var answerCounts = new Dictionary<int, int>();
            List<int> itemIds = answerAggregator.GetItemIDs();

            foreach (int itemId in itemIds)
            {
                answerCounts[itemId] = answerAggregator.GetItemAnswerCount(itemId);
            }

            return answerCounts;
        }

        /// <summary>
        /// Get the answer count/percentage for a particular item id.
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="answerAggregator">Aggregated data</param>
        /// <returns>Result value</returns>
        protected virtual ItemResult GetItemResult(Int32 itemID, ItemAnswerAggregator answerAggregator)
        {
            //Get answer count
            int answerCount = answerAggregator.GetItemAnswerCount(itemID);

            //Get response count
            int totalResponseCount = answerAggregator.GetResponseCount(itemID);

            if (totalResponseCount > 0)
            {
                return new ItemResult(answerCount, 100 * ((decimal)answerCount / totalResponseCount));
            }

            return new ItemResult(0, 0);
        }

        /// <summary>
        /// Get the answer count/percentage for a particular item id and answer text
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="answerText"></param>
        /// <param name="answerAggregator">Aggregated data</param>
        /// <returns>Result value</returns>
        protected virtual ItemResult GetItemResult(Int32 itemID, string answerText, ItemAnswerAggregator answerAggregator)
        {
            int answerCount = answerAggregator.GetItemAnswerCount(itemID, answerText);

            int totalResponseCount = answerAggregator.GetResponseCount(itemID);

            if (totalResponseCount > 0)
            {
                return new ItemResult(answerCount, 100 * ((decimal)answerCount / totalResponseCount));
            }

            return new ItemResult(0, 0);
        }

        /// <summary>
        /// Get the answer count/percentage for a particular option id.
        /// </summary>
        /// <param name="optionID">option ID</param>
        /// <param name="answerAggregator">Aggregated data</param>
        /// <returns>Result value</returns>
        protected virtual ItemResult GetOptionResult(Int32 optionID, ItemAnswerAggregator answerAggregator)
        {
            int answerCount = answerAggregator.GetOptionAnswerCount(optionID);

            int totalResponseCount = answerAggregator.GetResponseCount(null);

            if (totalResponseCount > 0)
            {
                return new ItemResult(answerCount, 100 * ((decimal)answerCount / totalResponseCount));
            }

            return new ItemResult(0, 0);
        }

        /// <summary>
        /// Get the sum points of the options across all the answers.
        /// </summary>
        /// <param name="optionID"></param>
        /// <param name="answerAggregator"></param>
        /// <param name="totalSumOptionsForItem"></param>
        /// <returns></returns>
        protected virtual ItemResult GetOptionSumPoints(Int32 optionID, ItemAnswerAggregator answerAggregator, double totalSumOptionsForItem)
        {
            if (totalSumOptionsForItem > 0)
            {
                double optionSumPoints = answerAggregator.GetOptionSumPoints(optionID);
                return new ItemResult(optionSumPoints, 100 * ((decimal)(optionSumPoints / totalSumOptionsForItem)));
            }

            return new ItemResult(0, 0);
        }
    }


    /// <summary>
    /// Item result
    /// </summary>
    public class ItemResult
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemResult()
        {
        }

        /// <summary>
        /// Construct an item result with a count and percentage.
        /// </summary>
        /// <param name="count">Number of answers.</param>
        /// <param name="percentage">Percentage.</param>
        public ItemResult(Int32 count, decimal percentage)
        {
            Count = count;
            Percentage = percentage;
        }

        /// <summary>
        /// Construct an item result with a sum points
        /// </summary>
        /// <param name="sumPoints">Sum Points</param>
        /// <param name="percentage">Percentage.</param>
        public ItemResult(double sumPoints, decimal percentage)
        {
            SumPoints = sumPoints;
            Percentage = percentage;
        }

        /// <summary>
        /// Number of answers for this result.
        /// </summary>
        public Int32 Count { get; set; }

        /// <summary>
        /// Get/Set sum point for this result.
        /// </summary>
        public double SumPoints { get; set; }

        /// <summary>
        /// Answer percentage for the result.
        /// </summary>
        public Decimal Percentage { get; set; }
    }
}
