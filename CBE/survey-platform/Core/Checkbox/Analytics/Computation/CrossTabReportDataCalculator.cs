using System;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Perform calculations for cross-tab data.
    /// </summary>
    public class CrossTabReportDataCalculator : TabularReportDataCalculator
    {
        /// <summary>
        /// Aggregate the data
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            var results = new List<AggregateResult>();

            //Get a list of response ids by item
            Dictionary<Int32, Dictionary<long, int>> itemResponseIDs = answerAggregator.ItemResponses;

            //Get a list of response ids by option
            Dictionary<Int32, Dictionary<long, int>> optionResponseIDs = answerAggregator.OptionResponses;

            Int32 columnCount = GetColumnCount();
            Int32 rowCount = GetRowCount();

            //For each column, calculate the data
            //NOTE: For the cross-tab there is no item at position 1,1, so the items start
            //      Position 2,1 for columns and 1,2 for rows, which means the max row option position is (1, RowCount +1)
            //      and max column option position is (ColumnCount + 1, 1)
            for (int i = 1; i <= columnCount + 1; i++)
            {
                int? columnOptionID = GetOptionIDAtPosition(new Coordinate(i, 1));

                if (!columnOptionID.HasValue) 
                {
                    continue;
                }

                Int32 columnItemID = answerAggregator.GetItemID(columnOptionID.Value);

                if (columnItemID <= 0)
                {
                    continue;
                }

                for (int j = 2; j <= rowCount + 1; j++)
                {
                    var rowOptionID = GetOptionIDAtPosition(new Coordinate(1, j));

                    if (!rowOptionID.HasValue)
                    {
                        continue;
                    }

                    Int32 rowItemID = answerAggregator.GetItemID(rowOptionID.Value);

                    if (rowItemID <= 0)
                    {
                        continue;
                    }

                    //Get the response intersection
                    if (itemResponseIDs.ContainsKey(columnItemID) 
                        && itemResponseIDs.ContainsKey(rowItemID)
                        && optionResponseIDs.ContainsKey(columnOptionID.Value) 
                        && optionResponseIDs.ContainsKey(rowOptionID.Value))
                    {
                        List<Int64> optionResponseIDIntersection = Utilities.ListIntersection(new List<long>(optionResponseIDs[columnOptionID.Value].Keys), new List<long>(optionResponseIDs[rowOptionID.Value].Keys));

                        //The percentage is calculated as, of the people who answer =  the independent variable, what percentage chose the dependent variable
                        double answerPercent = optionResponseIDs[columnOptionID.Value].Count > 0
                                                   ? 100*((double) optionResponseIDIntersection.Count/(double) optionResponseIDs[columnOptionID.Value].Count)
                                                   : (double) 0;

                        //Result key is row option id and column option id
                        results.Add(new AggregateResult
                        {
                            ResultKey = string.Format("{0}_{1}", columnOptionID, rowOptionID),
                            AnswerCount = optionResponseIDIntersection.Count,
                            AnswerPercent = answerPercent
                        });
                    }
                }
            }

            return results.ToArray();
        }
    }
}
