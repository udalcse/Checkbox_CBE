using System;
using System.Data;
using System.Collections.Generic;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Container/computer for matrix summary analysis data.
    /// </summary>
    [Serializable]
    public class MatrixSummaryAnalysisData
    {
        /// <summary>
        /// Container for answers in a matrix summary item.
        /// </summary>
        [Serializable]
        protected class MatrixAnswer
        {
            /// <summary>
            /// Row position for result
            /// </summary>
            public Int32 Row;

            /// <summary>
            /// Column position for result
            /// </summary>
            public Int32 Column;

            /// <summary>
            /// Answer value
            /// </summary>
            public string Answer;

            /// <summary>
            /// Number of answers with same value.
            /// </summary>
            public Int32 Count;
        }

        private DataTable _otherData;
        private Dictionary<string, string> _otherTexts;

        private Dictionary<Int32, string> _rowTexts;
        private Dictionary<Int32, string> _columnTexts;
        private Dictionary<Int32, List<string>> _columnOptions;
        private Dictionary<string, MatrixAnswer> _matrixAnswers;
        private Dictionary<string, double> _scaleAverages;
        private Dictionary<Int32, string> _defaultOtherTexts;

        private String _averageKey;

        /// <summary>
        /// Constructor
        /// </summary>
        public MatrixSummaryAnalysisData()
        {
            InitializeData();
        }

        /// <summary>
        /// Initialize internal data store
        /// </summary>
        private void InitializeData()
        {
            _rowTexts = new Dictionary<int, string>();
            _columnOptions = new Dictionary<int, List<string>>();
            _columnTexts = new Dictionary<int, string>();
            _matrixAnswers = new Dictionary<string, MatrixAnswer>();
            _otherTexts = new Dictionary<string,string>();
            _defaultOtherTexts = new Dictionary<int, string>();
            _scaleAverages = new Dictionary<string,double>();
            _averageKey = String.Empty;

            _otherData = new DataTable {TableName = "OtherData"};
            _otherData.Columns.Add("ResponseID", typeof(Int64));
            _otherData.Columns.Add("Row", typeof(Int32));
            _otherData.Columns.Add("Column", typeof(Int32));
            _otherData.Columns.Add("AnswerText", typeof(string));
        }

        /// <summary>
        /// Add a column question text
        /// </summary>
        /// <param name="column"></param>
        /// <param name="text"></param>
        public virtual void AddColumnQuestionText(Int32 column, string text)
        {
            _columnTexts[column] = text;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="column"></param>
        /// <param name="optionTexts"></param>
        public virtual void AddColumnOptionTexts(Int32 column, List<string> optionTexts)
        {
            _columnOptions[column] = optionTexts;
        }

        /// <summary>
        /// Add a row text
        /// </summary>
        /// <param name="row"></param>
        /// <param name="text"></param>
        public virtual void AddRowText(Int32 row, string text)
        {
            _rowTexts[row] = text;
        }

        /// <summary>
        /// Add an answer set
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="column"></param>
        /// <param name="count"></param>
        /// <param name="row"></param>
        public virtual void AddAnswer(Int32 row, Int32 column, string answer, int count)
        {
            string key = row + "_" + column + "_" + answer;

            if (_matrixAnswers.ContainsKey(key))
            {
                _matrixAnswers[key].Count += count;
            }
            else
            {
                MatrixAnswer matrixAnswer = new MatrixAnswer 
                {
                    Row = row, 
                    Column = column, 
                    Answer = answer, 
                    Count = count
                };

                _matrixAnswers[key] = matrixAnswer;
            }
        }

        /// <summary>
        /// Add a scale average
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="averageKey"></param>
        /// <param name="average"></param>
        public virtual void AddScaleAverage(Int32 row, Int32 column, String averageKey, Double average)
        {
            string key = row + "_" + column + "_" + averageKey;
            _averageKey = averageKey;
            _scaleAverages[key] = average;
        }

        /// <summary>
        /// Add an "other" text
        /// </summary>
        /// <param name="responseID"></param>
        /// <param name="row"></param>
        /// <param name="otherText"></param>
        public virtual void AddOtherText(Int64 responseID, Int32 row, string otherText)
        {
            string key = responseID + "_" + row;

            _otherTexts[key] = otherText;
        }

        /// <summary>
        /// Add an other answer set
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="column"></param>
        /// <param name="responseID"></param>
        /// <param name="row"></param>
        public virtual void AddOtherAnswer(Int64 responseID, Int32 row, Int32 column, string answer)
        {
            DataRow newRow = _otherData.NewRow();

            newRow["ResponseID"] = responseID;
            newRow["Row"] = row;
            newRow["Column"] = column;
            newRow["AnswerText"] = answer;

            _otherData.Rows.Add(newRow);
        }

        /// <summary>
        /// Set a default other text
        /// </summary>
        /// <param name="row"></param>
        /// <param name="otherText"></param>
        /// <returns></returns>
        public virtual void AddDefaultOtherText(Int32 row, string otherText)
        {
            _defaultOtherTexts[row] = otherText;
        }

        /// <summary>
        /// Get other texts for the row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetDefaultOtherText(Int32 row)
        {
            if (_defaultOtherTexts.ContainsKey(row))
            {
                return _defaultOtherTexts[row];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the number of responses for the item
        /// </summary>
        public virtual Int32 ResponseCount { get; set; }

        /// <summary>
        /// Get the number of responses for the item
        /// </summary>
        public virtual Int32 AnswerCount { get; set; }

        /// <summary>
        /// Get matrix category other data, grouped by "other" text
        /// </summary>
        public virtual DataTable OtherData
        {
            get
            {
                //Create table
                DataTable outputTable = new DataTable();

                outputTable.Columns.Add("Row", typeof(Int32));
                outputTable.Columns.Add("ResponseID", typeof(Int64));
                outputTable.Columns.Add("OtherText", typeof(string));

                //Add columns
                foreach (Int32 key in _columnTexts.Keys)
                {
                    string columnName = key + "_" + _columnTexts[key];

                    if (!outputTable.Columns.Contains(columnName))
                    {
                        outputTable.Columns.Add(columnName, typeof(string));
                    }
                }


                //Get a list of rows & response ids
                List<Int64> responseIDs = new List<long>();
                List<Int32> rows = new List<int>();

                foreach (string key in _otherTexts.Keys)
                {
                    string[] splitVal = key.Split('_');

                    if (splitVal.Length == 2)
                    {
                        Int64 responseID = Convert.ToInt64(splitVal[0]);
                        Int32 row = Convert.ToInt32(splitVal[1]);

                        if (!responseIDs.Contains(responseID))
                        {
                            responseIDs.Add(responseID);
                        }

                        if (!rows.Contains(row))
                        {
                            rows.Add(row);
                        }
                    }
                }

                //Now that the list of responses and rows has been created, get the output values
                foreach (Int64 responseID in responseIDs)
                {
                    foreach (Int32 row in rows)
                    {
                        DataRow[] otherRows = _otherData.Select("ResponseID = " + responseID + " AND Row = " + row, null, DataViewRowState.CurrentRows);

                        DataRow newRow = outputTable.NewRow();

                        string key = responseID + "_" + row;

                        if (_otherTexts.ContainsKey(key))
                        {
                            newRow["OtherText"] = _otherTexts[key];
                        }
                        else
                        {
                            newRow["OtherText"] = string.Empty;
                        }

                        newRow["ResponseID"] = responseID;
                        newRow["Row"] = row;

                        foreach (DataRow otherRow in otherRows)
                        {
                            if (otherRow["Column"] != DBNull.Value)
                            {
                                Int32 column = (Int32)otherRow["Column"];

                                if (_columnTexts.ContainsKey(column))
                                {
                                    string columnName = column + "_" + _columnTexts[column];

                                    if (outputTable.Columns.Contains(columnName))
                                    {
                                        if (newRow[columnName] != DBNull.Value && (string)newRow[columnName] != string.Empty)
                                        {
                                            newRow[columnName] += ", " + otherRow["AnswerText"];
                                        }
                                        else
                                        {
                                            newRow[columnName] = otherRow["AnswerText"];
                                        }
                                    }
                                }
                            }
                        }

                        outputTable.Rows.Add(newRow);
                    }
                }

                return outputTable;
            }
        }

        /// <summary>
        /// Get the "other" data table
        /// </summary>
        /// <returns></returns>
        protected virtual DataTable GetOtherDataTable()
        {
            DataTable table = new DataTable();
            
            //Add the columns
            foreach (Int32 key in _columnTexts.Keys)
            {
                table.Columns.Add(key.ToString(), typeof(string));
            }

            return table;
        }

       
        /// <summary>
        /// Get a datatable with the results data
        /// </summary>
        public virtual DataTable ResultsData
        {
            get
            {
                DataTable resultsData = new DataTable {TableName = "MatrixSummaryData"};
                resultsData.Columns.Add("RowNumber", typeof(string));
                resultsData.Columns.Add("RowText", typeof(string));

                //Add the columns
                foreach (Int32 key in _columnTexts.Keys)
                {
                    if (_columnOptions.ContainsKey(key) && _columnOptions[key].Count > 0)
                    {
                        foreach (string option in _columnOptions[key])
                        {
                            if (!resultsData.Columns.Contains(key + "_" + option))
                            {
                                resultsData.Columns.Add(key + "_" + option, typeof(string));
                            }
                        }
                    }
                    else
                    {
                        if (!resultsData.Columns.Contains(key + "_" + _columnTexts[key]))
                        {
                            resultsData.Columns.Add(key + "_", typeof(string));
                        }
                    }
                }

                //Add the answers
                foreach (MatrixAnswer answer in _matrixAnswers.Values)
                {
                    DataRow[] resultsRows = resultsData.Select("RowNumber = " + answer.Row, null, DataViewRowState.CurrentRows);

                    if (resultsRows.Length > 0)
                    {
                        if (resultsData.Columns.Contains(answer.Column + "_" + answer.Answer))
                        {
                            double percent = 0;

                            if (AnswerCount > 0)
                            {
                                percent = (double)100 * ((double)answer.Count / (double)AnswerCount);
                            }

                            resultsRows[0][answer.Column + "_" + answer.Answer] = answer.Count + "  (" + percent.ToString("N2") + "%)";
                        }
                    }
                    else
                    {
                        if (resultsData.Columns.Contains(answer.Column + "_" + answer.Answer))
                        {
                            DataRow newRow = resultsData.NewRow();
                            double percent = 0;

                            if (AnswerCount > 0)
                            {
                                percent = (double)100 * ((double)answer.Count / (double)AnswerCount);
                            }

                            newRow[answer.Column + "_" + answer.Answer] = answer.Count + "  (" + percent.ToString("N2") + "%)";

                            newRow["RowNumber"] = answer.Row;
                            
                            if (_rowTexts.ContainsKey(answer.Row))
                            {
                                newRow["RowText"] = _rowTexts[answer.Row];
                            }

                            resultsData.Rows.Add(newRow);
                        }                        
                    }
                }

                //Add item averages for rating scale items
                foreach (DataColumn column in resultsData.Columns)
                {
                    if (_averageKey != null && _averageKey.Trim() != string.Empty && column.ColumnName.Contains(_averageKey))
                    {
                        //Get the column number from the column name
                        if (column.ColumnName.Contains("_"))
                        {
                            string columnIndex = column.ColumnName.Substring(0, column.ColumnName.IndexOf('_'));

                            //Now loop the rows and insert the average value into the appropriate cells
                            for (Int32 rowIndex = 1; rowIndex <= resultsData.Rows.Count; rowIndex++)
                            {
                                string key = rowIndex + "_" + columnIndex + "_" + _averageKey;

                                if (_scaleAverages.ContainsKey(key))
                                {
                                    Double cellAverage = _scaleAverages[key];
                                    resultsData.Rows[rowIndex - 1][column] = cellAverage.ToString("N2");
                                }
                            }
                        }
                    }
                }


                resultsData.Columns.Remove("RowNumber");
                
                return resultsData;
            }
        }

        /// <summary>
        /// Get the texts for a column
        /// </summary>
        public Dictionary<Int32, string> GetColumnTexts()
        {
            return _columnTexts;
        }

        /// <summary>
        /// Get the options for a column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<string> GetColumnOptions(Int32 column)
        {
            if (_columnOptions.ContainsKey(column))
            {
                return _columnOptions[column];
            }
            
            return new List<string>();
        }

        /// <summary>
        /// Get the key column for the data
        /// </summary>
        public string KeyColumn
        {
            get { return "RowText"; }
        }
    }
}
