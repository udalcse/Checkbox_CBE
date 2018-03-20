using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Aggregate data and return the information in tabular format, that is, data has row/column information associated
    /// with it.
    /// </summary>
    public class TabularReportDataCalculator : ReportDataCalculator
    {
        private readonly Dictionary<int, List<Coordinate>> _resultsPositions;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TabularReportDataCalculator()
        {
            _resultsPositions = new Dictionary<int, List<Coordinate>>();

        }
        
        /// <summary>
        /// Aggregate the totals and return the result.
        /// </summary>
        /// <param name="answerAggregator">Aggregated answer data.</param>
        /// <returns>Summed and totalled data</returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            return (from optionID in _resultsPositions.Keys
                    let itemID = answerAggregator.GetItemID(optionID)
                    let result = GetOptionResult(optionID, answerAggregator)
                    select new AggregateResult
                               {
                                   //ItemId = itemID, 
                                   //ItemText = answerAggregator.GetItemText(itemID), 
                                   //OptionId = optionID, 
                                   ResultKey = optionID.ToString(),
                                   ResultText = answerAggregator.GetOptionText(optionID),
                                   AnswerCount = result.Count,
                                   AnswerPercent = Convert.ToDouble(result.Percentage)
                               }).ToArray();
        }

        /// <summary>
        /// Set the position of an option's results in the grid.
        /// </summary>
        /// <param name="optionID">Option ID</param>
        /// <param name="position">Results position</param>
        public void SetOptionResultsPosition(int optionID, Coordinate position)
        {
            if (!_resultsPositions.Keys.Contains(optionID))
                _resultsPositions[optionID] = new List<Coordinate>();

            _resultsPositions[optionID].Add(position);
        }

        /// <summary>
        /// Get the position of an option's results in the grid.
        /// </summary>
        /// <param name="optionID">Option to get the position of.</param>
        /// <returns>Position of the result</returns>
        public List<Coordinate> GetOptionResultPosition(int optionID)
        {
            return _resultsPositions.ContainsKey(optionID) ? _resultsPositions[optionID] : null;
        }

        /// <summary>
        /// Get the position of an option's results in the grid.
        /// </summary>
        /// <param name="optionID">Option to get the position of.</param>
        /// <returns>Position of the result</returns>
        private Coordinate GetOptionResultPositionA(int optionID)
        {
            return _resultsPositions.ContainsKey(optionID) ? _resultsPositions[optionID][0] : null;
        }
        
        /// <summary>
        /// Get the ID of the option at the specified position
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>Option ID</returns>
        protected int? GetOptionIDAtPosition(Coordinate position)
        {
            foreach (int optionID in _resultsPositions.Keys.Where(optionID => _resultsPositions[optionID].Contains(position)))
            {
                return optionID;
            }

            return null;
        }

        /// <summary>
        /// Get the number of rows
        /// </summary>
        /// <returns>Number of rows</returns>
        protected Int32 GetRowCount()
        {
            var rows = new List<int>();

            foreach (List<Coordinate> list in _resultsPositions.Values)
            {
                foreach (Coordinate c in list.Where(c => !rows.Contains(c.Y)))
                {
                    rows.Add(c.Y);
                }
            }

            return rows.Count;
        }

        /// <summary>
        /// Get the number of columns.
        /// </summary>
        /// <returns>Number of columns.</returns>
        protected Int32 GetColumnCount()
        {
            var columns = new List<int>();
            foreach (List<Coordinate> list in _resultsPositions.Values)
            {
                foreach (Coordinate c in list.Where(c => !columns.Contains(c.X)))
                {
                    columns.Add(c.X);
                }
            }

            return columns.Count;
        }
    }
}
