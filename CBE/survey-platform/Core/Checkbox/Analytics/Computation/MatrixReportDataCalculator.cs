using System;
using System.Linq;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Data calculator for matrix analysis that includes average scores
    /// </summary>
    public class MatrixReportDataCalculator : FrequencyDataCalculator
    {
        //List of rating scale items
        private readonly List<int> _ratingScaleItems;

        //List of sum total items
        private readonly List<int> _sumTotalItems;

        //List of slider items
        private readonly List<int> _sliderItems;

        /// <summary>
        /// Constructor -- Initializes internal data store
        /// </summary>
        public MatrixReportDataCalculator()
        {
            _ratingScaleItems = new List<int>();
            _sumTotalItems = new List<int>();
            _sliderItems = new List<int>();
        }

        /// <summary>
        /// Get the name of the table containing rating scale item averages
        /// </summary>
        public string RatingScaleAveragesTableName
        {
            get { return "RatingScaleItemAverages"; }
        }

        /// <summary>
        /// Get the name of the table containing sum total item averages
        /// </summary>
        public string SumTotalAveragesTableName
        {
            get { return "SumTotalItemAverages"; }
        }
        
        /// <summary>
        /// Get the calculated data for the matrix summary
        /// </summary>
        /// <param name="aggregator">Item answer aggregator</param>
        /// <returns>DataSet containing data</returns>
        public override AnalysisItemResult GetData(ItemAnswerAggregator aggregator)
        {
            return new MatrixSummaryAnalysisItemResult
            {
                ItemResponseCounts = CountResponses(aggregator),
                ItemAnswerCounts = CountAnswers(aggregator),
                AggregateResults = Aggregate(aggregator),
                RatingScaleAverages = CalculateRatingScaleAverages(aggregator),
                SumTotalAverages = CalculateSumTotalAverages(aggregator),
                SliderAverages = CalculateSliderAverages(aggregator)
            };
        }


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

                string itemText = answerAggregator.GetItemText(itemID);

                //If the item has no options, assume open-ended, otherwise assume select
                if (optionIDs.Count > 0)
                {
                    results.AddRange(from optionID in optionIDs
                                     let result = GetOptionResult(itemID, optionID, answerAggregator)
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

        /// <summary>
        /// Get the answer count/percentage for a particular option id.
        /// </summary>
        /// <param name="optionID">option ID</param>
        /// <param name="answerAggregator">Aggregated data</param>
        /// <returns>Result value</returns>
        protected virtual ItemResult GetOptionResult(Int32 itemID, Int32 optionID, ItemAnswerAggregator answerAggregator)
        {
            int answerCount = answerAggregator.GetOptionAnswerCount(itemID, optionID);

            int totalResponseCount = answerAggregator.GetItemAnswerCount(itemID);

            if (totalResponseCount > 0)
            {
                return new ItemResult(answerCount, 100 * ((decimal)answerCount / totalResponseCount));
            }

            return new ItemResult(0, 0);
        }


        /// <summary>
        /// Calculate the average scores for the rating scale items in the matrix
        /// </summary>
        /// <param name="aggregator">Answer aggregator</param>
        /// <returns>DataTable with average score information</returns>
        protected virtual Dictionary<int, double> CalculateRatingScaleAverages(ItemAnswerAggregator aggregator)
        {
            var answerData = aggregator.GetAggregatedAnswerData();

            var averages = new Dictionary<int, double>();

            foreach (int ratingScaleItemID in _ratingScaleItems)
            {
                var itemResults = answerData.Where(result => result.ItemId == ratingScaleItemID);

                double scoreCount = Convert.ToDouble(itemResults.Count());
                double totalScore = itemResults.Sum(result => result.AnswerScore);

                averages[ratingScaleItemID] = scoreCount > 0 ? totalScore/scoreCount : 0;
            }

            return averages;
        }

        /// <summary>
        /// Calculate the averages for the sum total items in the matrix
        /// </summary>
        /// <param name="aggregator">Answer aggregator</param>
        /// <returns>DataTable with average score information</returns>
        protected virtual Dictionary<int, double> CalculateSumTotalAverages(ItemAnswerAggregator aggregator)
        {
            var answerData = aggregator.GetAggregatedAnswerData();

            var averages = new Dictionary<int, double>();

            foreach (int sumTotalItemID in _sumTotalItems)
            {
                var itemResults = answerData.Where(
                    result => 
                        result.ItemId == sumTotalItemID
                        && Utilities.IsNumeric(result.ResultText)
                );

                double valueCount = Convert.ToDouble(itemResults.Count());
                double totalValue = itemResults.Sum(result => result.AnswerScore);

                averages[sumTotalItemID] = valueCount > 0 ? totalValue / valueCount : 0;
            }

            return averages;
        }

        /// <summary>
        /// Calculate the averages for the slider items in the matrix
        /// </summary>
        /// <param name="aggregator">Answer aggregator</param>
        /// <returns>DataTable with average score information</returns>
        protected virtual Dictionary<int, double> CalculateSliderAverages(ItemAnswerAggregator aggregator)
        {
            var answerData = aggregator.GetAggregatedAnswerData();

            var averages = new Dictionary<int, double>();

            foreach (int sliderItemID in _sliderItems)
            {
                var itemResults = answerData.Where(
                    result =>
                        result.ItemId == sliderItemID
                        && Utilities.IsNumeric(result.ResultText)
                );

                double valueCount = Convert.ToDouble(itemResults.Count());
                double totalValue = itemResults.Sum(result => result.AnswerScore);

                averages[sliderItemID] = valueCount > 0 ? totalValue / valueCount : 0;
            }

            return averages;
        }

        /// <summary>
        /// Add an item id to the list of rating scale item ids
        /// </summary>
        /// <param name="itemID">ID of the rating scale item</param>
        public void AddRatingScaleItem(int itemID)
        {
            if (!_ratingScaleItems.Contains(itemID))
            {
                _ratingScaleItems.Add(itemID);
            }
        }

        /// <summary>
        /// Add an item id to the list of sum total item ids
        /// </summary>
        /// <param name="itemID">ID of the item</param>
        public void AddSumTotalItem(int itemID)
        {
            if (!_sumTotalItems.Contains(itemID))
            {
                _sumTotalItems.Add(itemID);
            }
        }

        /// <summary>
        /// Add an item id to the list of slider item ids
        /// </summary>
        /// <param name="itemID">ID of the item</param>
        public void AddSliderItem(int itemID)
        {
            if (!_sliderItems.Contains(itemID))
            {
                _sliderItems.Add(itemID);
            }
        }
    }
}
