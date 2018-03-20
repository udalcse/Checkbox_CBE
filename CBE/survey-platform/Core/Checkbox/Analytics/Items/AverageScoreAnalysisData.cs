using System;
using System.Data;
using System.Collections.Generic;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Container for processed average score analysis data
    /// </summary>
    [Serializable]
    public class AverageScoreAnalysisData
    {
        private readonly Dictionary<Int32, Dictionary<Int32, Int32>> _scores;
        private readonly Dictionary<Int32, string> _questions;
        private readonly Dictionary<Int32, Int32> _responseCounts;

        /// <summary>
        /// Constructor
        /// </summary>
        internal AverageScoreAnalysisData()
        {
            _scores = new Dictionary<int, Dictionary<int, int>>();
            _questions = new Dictionary<int, string>();
            _responseCounts = new Dictionary<int, int>();
        }

        /// <summary>
        /// Add a score to the collection
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="questionText"></param>
        /// <param name="score"></param>
        internal void AddScore(Int32 itemID, string questionText, Int32 score)
        {
            _questions[itemID] = questionText;

            if (!_scores.ContainsKey(itemID))
            {
                _scores[itemID] = new Dictionary<int, int>();
            }

            if (!_scores[itemID].ContainsKey(score))
            {
                _scores[itemID][score] = 1;
            }
            else
            {
                _scores[itemID][score]++;
            }

            if (!_responseCounts.ContainsKey(itemID))
            {
                _responseCounts[itemID] = 1;
            }
            else
            {
                _responseCounts[itemID]++;
            }
        }

        /// <summary>
        /// Get the aggregated average score, weighted by response count, etc.
        /// </summary>
        public double AggregateAverage
        {
            get
            {
                double totalScore = 0;
                double scoreCount = 0;

                foreach (Int32 itemID in _scores.Keys)
                {
                    foreach (Int32 score in _scores[itemID].Keys)
                    {
                        //Total = score * count
                        totalScore += Convert.ToDouble(score * _scores[itemID][score]);
                        scoreCount += Convert.ToDouble(_scores[itemID][score]);
                    }
                }

                return totalScore / scoreCount;
            }
        }

        /// <summary>
        /// Average score of item totals
        /// </summary>
        public double AverageOfItemTotals
        {
            get
            {
                double total = 0;
                double count = 0;

                foreach (Int32 itemID in _scores.Keys)
                {
                    double itemTotal;
                    double itemScoreCount;
                    double itemAverage;

                    GetItemScoreSummary(itemID, out itemTotal, out itemAverage, out itemScoreCount);

                    total += itemTotal;
                    count += itemScoreCount;
                }

                return total / count;
            }
        }

        /// <summary>
        /// Get summary information about item scores
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="total"></param>
        /// <param name="average"></param>
        /// <param name="responseCount"></param>
        public void GetItemScoreSummary(Int32 itemID, out double total, out double average, out double responseCount)
        {

            total = 0;
            average = 0;
            responseCount = 0;

            if (_scores.ContainsKey(itemID))
            {
                foreach (Int32 score in _scores[itemID].Keys)
                {
                    //Total = score * count
                    total += Convert.ToDouble(score * _scores[itemID][score]);
                    responseCount += Convert.ToDouble(_scores[itemID][score]);
                }

                average = total / responseCount;
            }
        }

        /// <summary>
        /// Get a datatable containing item totals
        /// </summary>
        public DataTable ItemAveragesTable
        {
            get
            {
                DataTable t = new DataTable();
                t.Columns.Add("ItemID", typeof(Int32));
                t.Columns.Add("QuestionText", typeof(string));
                t.Columns.Add("ResponseCount", typeof(Int32));
                t.Columns.Add("ItemTotal", typeof(Int32));
                t.Columns.Add("ItemAverage", typeof(double));

                foreach (Int32 itemID in _scores.Keys)
                {
                    double itemTotal;
                    double itemAverage;
                    double responseCount;

                    GetItemScoreSummary(itemID, out itemTotal, out itemAverage, out responseCount);

                    DataRow newRow = t.NewRow();
                    newRow["ItemID"] = itemID;
                    newRow["QuestionText"] = (_questions.ContainsKey(itemID)) ? _questions[itemID] : itemID.ToString();
                    newRow["ResponseCount"] = Convert.ToInt32(responseCount);
                    newRow["ItemTotal"] = Convert.ToInt32(itemTotal);
                    newRow["ItemAverage"] =itemAverage;
                    t.Rows.Add(newRow);                    
                }

                return t;
            }
        }

        /// <summary>
        /// Get a list of all responses to items, grouped by score
        /// </summary>
        public DataTable GroupedItemScoresTable
        {
            get
            {
                DataTable t = new DataTable();
                t.Columns.Add("ItemID", typeof(Int32));
                t.Columns.Add("QuestionText", typeof(string));
                t.Columns.Add("Score", typeof(Int32));
                t.Columns.Add("ResponseCount", typeof(Int32));

                foreach (Int32 itemID in _scores.Keys)
                {
                    foreach (Int32 score in _scores[itemID].Keys)
                    {
                        DataRow newRow = t.NewRow();
                        newRow["ItemID"] = itemID;
                        newRow["QuestionText"] = (_questions.ContainsKey(itemID)) ? _questions[itemID] : itemID.ToString();
                        newRow["Score"] = score;
                        newRow["ResponseCount"] = _scores[itemID][score];
                        t.Rows.Add(newRow);
                    }
                }

                return t;
            }
        }

        /// <summary>
        /// Get a datatable with all items for the survey
        /// </summary>
        public DataTable AllItemScoresTable
        {
            get
            {
                DataTable t = new DataTable();
                t.Columns.Add("ItemID", typeof(Int32));
                t.Columns.Add("QuestionText", typeof(string));
                t.Columns.Add("Score", typeof(Int32));

                foreach (Int32 itemID in _scores.Keys)
                {
                    foreach (Int32 score in _scores[itemID].Keys)
                    {
                        for (Int32 i = 0; i < _scores[itemID][score]; i++)
                        {
                            DataRow newRow = t.NewRow();
                            newRow["ItemID"] = itemID;
                            newRow["QuestionText"] = (_questions.ContainsKey(itemID)) ? _questions[itemID] : itemID.ToString();
                            newRow["Score"] = score;
                            newRow["ResponseCount"] = _scores[itemID][score];
                            t.Rows.Add(newRow);
                        }
                    }
                }

                return t;
            }
        }
    }
}
