using System;
using System.Data;
using System.Collections.Generic;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Container/computer for cross-tab analysis data
    /// </summary>
    [Serializable]
    public class CrossTabAnalysisData
    {
        private DataTable _data;
        private Dictionary<Int32, string> _questionTexts;
        private Dictionary<Int32, List<string>> _xAxisOptions;
        private Dictionary<Int32, List<string>> _yAxisOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrossTabAnalysisData()
        {
            InitializeData();
        }

        /// <summary>
        /// Initialize internal data store
        /// </summary>
        private void InitializeData()
        {
            _data = new DataTable {TableName = "CrossTabData"};
            _data.Columns.Add("XAxisItemID", typeof(Int32));
            _data.Columns.Add("XAxisItem", typeof(string));
            _data.Columns.Add("YAxisItemID", typeof(Int32));
            _data.Columns.Add("YAxisItem", typeof(string));
            _data.Columns.Add("AnswerCount", typeof(Int32));

            _questionTexts = new Dictionary<int, string>();
            _xAxisOptions = new Dictionary<int,List<string>>();
            _yAxisOptions = new Dictionary<int,List<string>>();
        }

        /// <summary>
        /// Add a question text
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="questionText"></param>
        public virtual void AddQuestionText(Int32 itemID, string questionText)
        {
            _questionTexts[itemID] = questionText;
        }

        /// <summary>
        /// Add question options
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="options"></param>
        public virtual void AddXOptions(Int32 itemID, List<string> options)
        {
            _xAxisOptions[itemID] = options;
        }

        /// <summary>
        /// Add question options
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="options"></param>
        public virtual void AddYOptions(Int32 itemID, List<string> options)
        {
            _yAxisOptions[itemID] = options;
        }

        /// <summary>
        /// Add an anser to the data collection.
        /// </summary>
        /// <param name="xAxisItemID"></param>
        /// <param name="xAxisItem"></param>
        /// <param name="yAxisItemID"></param>
        /// <param name="yAxisItem"></param>
        /// <param name="answerCount"></param>
        public virtual void AddAnswer(Int32 xAxisItemID, string xAxisItem, Int32 yAxisItemID, string yAxisItem, Int32 answerCount)
        {
            string filter = string.Format("XAxisItemID = {0} AND XAxisItem = '{1}'", xAxisItemID, xAxisItem.Replace("'", "''"));
            filter+= string.Format(" AND  YAxisItemID = {0} AND YAxisItem = '{1}'", yAxisItemID, yAxisItem.Replace("'", "''"));

            DataRow[] rows = _data.Select(filter, null, DataViewRowState.CurrentRows);

            if (rows.Length == 0)
            {
                DataRow newRow = _data.NewRow();
                newRow["XAxisItemID"] = xAxisItemID;
                newRow["XAxisItem"] = xAxisItem;
                newRow["YAxisItemID"] = yAxisItemID;
                newRow["YAxisItem"] = yAxisItem;
                newRow["AnswerCount"] = answerCount;
                _data.Rows.Add(newRow);
            }
            else
            {
                Int32 count = Convert.ToInt32(rows[0]["AnswerCount"]) + answerCount;
                rows[0]["AnswerCount"] = count;
            }
        }

        /// <summary>
        /// Get the number of responses for the item
        /// </summary>
        public virtual Int32 ResponseCount { get; set; }

        /// <summary>
        /// Get a datatable with the results data
        /// </summary>
        public virtual DataTable ResultsData
        {
            get
            {
                DataTable resultsData = new DataTable();
                resultsData.Columns.Add("YAxisItem", typeof(string));
                resultsData.Columns.Add(KeyColumn, typeof(string));
                           
                //Get the data for the rows/columns
                foreach (Int32 xKey in _xAxisOptions.Keys)
                {
                    foreach (string xValue in _xAxisOptions[xKey])
                    {
                        string theKey = xKey + "_" + xValue;

                        int loopCount = 1;

                        while (resultsData.Columns.Contains(theKey))
                        {
                            theKey = xKey + "_" + xValue + loopCount;
                            loopCount++;
                        }

                        resultsData.Columns.Add(theKey, typeof(string));

                        foreach (Int32 yKey in _yAxisOptions.Keys)
                        {
                            foreach (string yValue in _yAxisOptions[yKey])
                            {
                                string xValueEscaped = xValue.Replace("'", "''");
                                string yValueEscaped = yValue.Replace("'", "''");

                                object oCount = _data.Compute("Sum(AnswerCount)", string.Format("XAxisItemID = {0} AND XAxisItem = '{1}' AND YAxisItemID = {2} AND YAxisItem = '{3}'", xKey, xValueEscaped, yKey, yValueEscaped));

                                double responseCount = oCount != DBNull.Value ? Convert.ToDouble(oCount) : 0;

                                double responsePercent = 0;

                                //Get the percentage by dividing the total number of answers for this combination by the Total Number of Responses to this Question
                                if (ResponseCount > 0)
                                {
                                    responsePercent = (responseCount / (double)ResponseCount) * (double)100;
                                }

                                string valueString = responseCount + "  (" + responsePercent.ToString("0.00") + "%)  ";

                                DataRow[] yAxisRows = resultsData.Select(KeyColumn + " = '" + yKey + "_" + yValueEscaped + "'", null, DataViewRowState.CurrentRows);

                                if (yAxisRows.Length == 0)
                                {
                                    DataRow newRow = resultsData.NewRow();
                                    newRow[theKey] = valueString;
                                    newRow["YAxisItem"] = yValue;
                                    newRow[KeyColumn] = yKey + "_" + yValue;
                                    resultsData.Rows.Add(newRow);
                                }
                                else
                                {
                                    yAxisRows[0][theKey] = valueString;
                                }
                            }
                        }
                    }
                }

                return resultsData;
            }
        }

        /// <summary>
        /// Get the texts for the questions on the xaxis
        /// </summary>
        public Dictionary<Int32, string> GetXQuestionTexts()
        {
            Dictionary<Int32, string> texts = new Dictionary<int, string>();

            foreach (Int32 key in _questionTexts.Keys)
            {
                DataRow[] xRows = _data.Select("XAxisItemID = " + key, null, DataViewRowState.CurrentRows);

                if (xRows.Length > 0)
                {
                    texts[key] = _questionTexts[key];
                }
            }

            return texts;
        }

        /// <summary>
        /// Get the texts for questions on the y axis
        /// </summary>
        /// <returns></returns>
        public Dictionary<Int32, string> GetYQuestionTexts()
        {
            Dictionary<Int32, string> texts = new Dictionary<int, string>();

            foreach (Int32 key in _questionTexts.Keys)
            {
                DataRow[] yRows = _data.Select("YAxisItemID = " + key, null, DataViewRowState.CurrentRows);

                if (yRows.Length > 0)
                {
                    texts[key] = _questionTexts[key];
                }
            }

            return texts;
        }

        /// <summary>
        /// Get the option texts for questions
        /// </summary>
        /// <returns></returns>
        public Dictionary<Int32, List<string>> GetOptionTexts()
        {
            Dictionary<Int32, List<string>> texts = new Dictionary<Int32, List<string>>();

            foreach (Int32 key in _xAxisOptions.Keys)
            {
                texts[key] = _xAxisOptions[key];
            }

            foreach (Int32 key in _yAxisOptions.Keys)
            {
                texts[key] = _yAxisOptions[key];
            }

            return texts;
        }

        /// <summary>
        /// Get the key column for the data
        /// </summary>
        public string KeyColumn
        {
            get { return "YAxisKey"; }
        }
    }
}
