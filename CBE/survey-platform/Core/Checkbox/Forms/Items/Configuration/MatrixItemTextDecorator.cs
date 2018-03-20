using System;
using System.Collections.Generic;
using Checkbox.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for a matrix item
    /// </summary>
    [Serializable]
    public class MatrixItemTextDecorator : LabelledItemTextDecorator
    {
        readonly Dictionary<int, string> _rowTexts;
        readonly Dictionary<int, string> _columnTexts;

        readonly Dictionary<int, bool> _rowTextModified;
        readonly Dictionary<int, bool> _columnTextModified;

        //Option Text Key is a concatenation of column & option position
        readonly Dictionary<string, string> _optionTexts;
        readonly Dictionary<string, bool> _optionTextsModified;

        //Scale Text key is a concatenation of column & text pos (start, middle, end, na);
        readonly Dictionary<string, string> _scaleTexts;
        readonly Dictionary<string, bool> _scaleTextsModified;

        //The default texts are for text items
        //Column Default Text Key is the column position
        readonly Dictionary<Int32, string> _columnDefaultTexts;
        readonly Dictionary<Int32, bool> _columnDefaultTextsModified;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="language"></param>
        public MatrixItemTextDecorator(MatrixItemData itemData, string language)
            : base(itemData, language)
        {
            _columnTextModified = new Dictionary<int, bool>();
            _rowTextModified = new Dictionary<int, bool>();
            _columnTexts = new Dictionary<int, string>();
            _rowTexts = new Dictionary<int, string>();
            _optionTexts = new Dictionary<string, string>();
            _optionTextsModified = new Dictionary<string, bool>();
            _scaleTextsModified = new Dictionary<string, bool>();
            _scaleTexts = new Dictionary<string, string>();
            _columnDefaultTexts = new Dictionary<int, string>();
            _columnDefaultTextsModified = new Dictionary<int, bool>();
        }

        /// <summary>
        /// Get the data
        /// </summary>
        new public MatrixItemData Data
        {
            get { return (MatrixItemData)base.Data; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetData(MatrixItemData data)
        {
            base.Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private ItemData GetColumnPrototypeItemData(int columnNumber)
        {
            if (columnNumber == Data.PrimaryKeyColumnIndex)
            {
                return null;
            }

            int columnPrototypeId = Data.GetColumnPrototypeId(columnNumber);

            if (columnPrototypeId <= 0)
            {
                return null;
            }

            return ItemConfigurationManager.GetConfigurationData(columnPrototypeId);
        }

        /// <summary>
        /// Set localized texts associated with this item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            //Set the text/subtext for the item
            base.SetLocalizedTexts();

            //Set texts for the column headers and the row messages

            //Column headers
            for (int columnNumber = 1; columnNumber <= Data.ColumnCount; columnNumber++)
            {
                if (columnNumber == Data.PrimaryKeyColumnIndex)
                {
                    continue;
                }

                ItemData columnPrototypeItemData = GetColumnPrototypeItemData(columnNumber);

                if (columnPrototypeItemData == null)
                {
                    continue;
                }

                SetColumnPrototypeItemTexts(columnPrototypeItemData, columnNumber);
            }

            //Row texts
            for (int rowNumber = 1; rowNumber <= Data.RowCount; rowNumber++)
            {
                int? itemId = Data.GetItemIdAt(rowNumber, Data.PrimaryKeyColumnIndex);

                if (!itemId.HasValue)
                {
                    continue;
                }

                ItemData rowHeaderItemData = ItemConfigurationManager.GetConfigurationData(itemId.Value);

                if (rowHeaderItemData == null)
                {
                    continue;
                }

                if (rowHeaderItemData is MessageItemData)
                {
                    SetText(((MessageItemData)rowHeaderItemData).TextID, GetRowText(rowNumber));
                }

                if (rowHeaderItemData is TextItemData)
                {
                    SetText(((TextItemData)rowHeaderItemData).DefaultTextID, GetRowText(rowNumber));
                }
            }
        }

        /// <summary>
        /// Set the texts for the item data
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="column"></param>
        protected virtual void SetColumnPrototypeItemTexts(ItemData itemData, int column)
        {
            if (itemData is LabelledItemData)
            {
                SetText(((LabelledItemData)itemData).TextID, GetColumnText(column));
            }

            //Set the scale texts
            if (itemData is RatingScaleItemData)
            {
                SetText(((RatingScaleItemData)itemData).StartTextID, GetScaleText(column, "start"));
                SetText(((RatingScaleItemData)itemData).MidTextID, GetScaleText(column, "middle"));
                SetText(((RatingScaleItemData)itemData).EndTextID, GetScaleText(column, "end"));
                SetText(((RatingScaleItemData)itemData).NotApplicableTextID, GetScaleText(column, "na"));
            }

            //Set default texts
            if (itemData is TextItemData)
            {
                SetText(((TextItemData)itemData).DefaultTextID, GetColumnDefaultText(column));
            }

            //Option texts
            if (itemData is SelectItemData)
            {
                foreach (ListOptionData option in ((SelectItemData)itemData).Options)
                {
                    if (!String.IsNullOrEmpty(option.TextID))
                        SetText(option.TextID, GetOptionText(column, option.Position));
                }
            }
        }

        /// <summary>
        /// Set the default text for a column, useful only for text item columns
        /// </summary>
        /// <param name="column"></param>
        /// <param name="text"></param>
        public void SetColumnDefaultText(int column, string text)
        {
            _columnDefaultTexts[column] = text;
            _columnDefaultTextsModified[column] = true;
        }

        /// <summary>
        /// Get the default text for a text item column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetColumnDefaultText(int column)
        {
            if (column == Data.PrimaryKeyColumnIndex)
            {
                return string.Empty;
            }

            if (_columnDefaultTextsModified.ContainsKey(column) && _columnDefaultTextsModified[column])
            {
                return _columnDefaultTexts[column];
            }

            ItemData data = GetColumnPrototypeItemData(column);

            if (data != null && data is TextItemData)
            {
                string text = GetText(((TextItemData)data).DefaultTextID);
                _columnDefaultTexts[column] = text;

                return text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Set text for a column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="text"></param>
        public void SetColumnText(int column, string text)
        {
            _columnTexts[column] = text;
            _columnTextModified[column] = true;
        }

        /// <summary>
        /// Get the text for a column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetColumnText(int column)
        {
            if (column == Data.PrimaryKeyColumnIndex)
            {
                return string.Empty;
            }

            if (_columnTextModified.ContainsKey(column) && _columnTextModified[column])
            {
                return _columnTexts[column];
            }

            ItemData data = GetColumnPrototypeItemData(column);

            if (data is LabelledItemData)
            {
                string text = GetText(((LabelledItemData)data).TextID);
                _columnTexts[column] = text;

                return text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Set the text for a row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetRowText(int row, string text)
        {
            _rowTexts[row] = text;
            _rowTextModified[row] = true;
        }

        /// <summary>
        /// Get the text for a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetRowText(int row)
        {
            if (_rowTextModified.ContainsKey(row) && _rowTextModified[row])
            {
                return _rowTexts[row];
            }

            int? itemID = Data.GetItemIdAt(row, Data.PrimaryKeyColumnIndex);

            if (itemID.HasValue)
            {
                if (Data.IsRowOther(row))
                {
                    var data = new SingleLineTextItemData();
                    data.Load(itemID.Value);
                    string text = GetText(data.DefaultTextID);
                    _rowTexts[row] = text;
                    return text;
                }
                else
                {
                    var data = new MessageItemData();
                    data.Load(itemID.Value);
                    string text = GetText(data.TextID);
                    _rowTexts[row] = text;
                    return text;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Set the text for a scale
        /// </summary>
        /// <param name="column"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public void SetScaleText(int column, string position, string text)
        {
            string key = column + "_" + position;
            _scaleTexts[key] = text;
            _scaleTextsModified[key] = true;
        }

        /// <summary>
        /// Get the text for a scale
        /// </summary>
        /// <param name="column"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public string GetScaleText(int column, string position)
        {
            string key = column + "_" + position;

            if (_scaleTextsModified.ContainsKey(key))
            {
                return _scaleTexts[key];
            }

            ItemData data = GetColumnPrototypeItemData(column);

            if (data != null && data is RatingScaleItemData)
            {
                string text = string.Empty;

                if (position.ToLower() == "start")
                {
                    text = GetText(((RatingScaleItemData)data).StartTextID);
                }
                else if (position.ToLower() == "middle")
                {
                    text = GetText(((RatingScaleItemData)data).MidTextID);
                }
                else if (position.ToLower() == "end")
                {
                    text = GetText(((RatingScaleItemData)data).EndTextID);
                }
                else if (position.ToLower() == "na")
                {
                    text = GetText(((RatingScaleItemData)data).NotApplicableTextID);
                }

                _scaleTexts[key] = text;

                return text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Set the text of the option at specified column / option position
        /// </summary>
        /// <param name="column"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetOptionText(int column, int position, string text)
        {
            string key = column + "_" + position;

            _optionTexts[key] = text;
            _optionTextsModified[key] = true;
        }

        /// <summary>
        /// Get the text for the specified option
        /// </summary>
        /// <param name="column"></param>
        /// <param name="position"></param>
        public string GetOptionText(int column, int position)
        {
            string key = column + "_" + position;

            if (_optionTextsModified.ContainsKey(key) && _optionTextsModified[key])
            {
                return _optionTexts[key];
            }
            ItemData data = GetColumnPrototypeItemData(column);

            if (data != null && data is SelectItemData)
            {
                if (((SelectItemData)data).OptionsList != null)
                {
                    ListOptionData option = ((SelectItemData)data).OptionsList.GetOptionAt(position);

                    if (option != null)
                    {
                        string text;

                        if (data is RatingScaleItemData)
                        {
                            text = option.IsOther ? GetText(((RatingScaleItemData)data).NotApplicableTextID) : option.Points.ToString();

                            if (Utilities.IsNullOrEmpty(text))
                            {
                                text = "n/a";
                            }
                        }
                        else
                        {
                            text = GetText(option.TextID);
                        }

                        _optionTexts[key] = text;

                        return text;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the text for the specified option
        /// </summary>
        /// <param name="column"></param>
        public List<string> GetOptionTexts(int column)
        {
            string columnKey = column + "_";

            var optionTextKeys = new Dictionary<string, string>();

            //Find the keys for this column
            ItemData data = GetColumnPrototypeItemData(column);

            if (data != null && data is SelectItemData)
            {
                foreach (ListOptionData option in ((SelectItemData)data).Options)
                {
                    optionTextKeys[columnKey + option.Position] = option.TextID;
                }
            }

            var optionTexts = new List<string>();

            foreach (string textKey in optionTextKeys.Keys)
            {
                if (_optionTextsModified.ContainsKey(textKey) && _optionTextsModified[textKey])
                {
                    optionTexts.Add(_optionTexts[textKey]);
                }
                else
                {
                    string text = GetText(optionTextKeys[textKey]);
                    _optionTexts[textKey] = text;
                    optionTexts.Add(text);
                }
            }

            return optionTexts;
        }

        /// <summary>
        /// Remove a row from the matrix
        /// </summary>
        /// <param name="row"></param>
        public void RemoveRow(int row)
        {
            if (row > 0)
            {
                Data.RemoveRow(row);

                for (int i = row; i <= Data.RowCount; i++)
                {
                    SwapRowTextValues(i, i + 1);
                }

                if (_rowTextModified.ContainsKey(Data.RowCount + 1))
                {
                    _rowTextModified.Remove(Data.RowCount + 1);
                }

                if (_rowTexts.ContainsKey(Data.RowCount + 1))
                {
                    _rowTexts.Remove(Data.RowCount + 1);
                }
            }
        }

        /// <summary>
        /// Move a row from one position to another.
        /// </summary>
        /// <param name="currentRowPosition"></param>
        /// <param name="newRowPosition"></param>
        public void MoveRow(int currentRowPosition, int newRowPosition)
        {
            //Moving a row by more than one position is new to 5.0.  Due to time constraints, simply use existing
            // move by one methods for now.
            if (currentRowPosition < newRowPosition)
            {
                for (int rowPosition = currentRowPosition; rowPosition < newRowPosition; rowPosition++)
                {
                    MoveRowDown(rowPosition);
                }
            }

            if (currentRowPosition > newRowPosition)
            {
                for (int rowPosition = currentRowPosition; rowPosition > newRowPosition; rowPosition--)
                {
                    MoveRowUp(rowPosition);
                }
            }
        }

        /// <summary>
        /// Move a column from one position to another.
        /// </summary>
        /// <param name="currentColumnPosition"></param>
        /// <param name="newColumnPosition"></param>
        public void MoveColumn(int currentColumnPosition, int newColumnPosition)
        {
            //Moving a row by more than one position is new to 5.0.  Due to time constraints, simply use existing
            // move by one methods for now.
            if (currentColumnPosition < newColumnPosition)
            {
                for (int columnPosition = currentColumnPosition; columnPosition < newColumnPosition; columnPosition++)
                {
                    MoveColumnRight(columnPosition);
                }
            }

            if (currentColumnPosition > newColumnPosition)
            {
                for (int columnPosition = currentColumnPosition; columnPosition > newColumnPosition; columnPosition--)
                {
                    MoveColumnLeft(columnPosition);
                }
            }
        }


        /// <summary>
        /// Move the specified row up one position
        /// </summary>
        /// <param name="row"></param>
        public void MoveRowUp(int row)
        {
            if (row > 1)
            {
                //Change the item in the data
                Data.MoveRow(row, row - 1);
                SwapRowTextValues(row, row - 1);
            }
        }

        /// <summary>
        /// Move the specified row down one position
        /// </summary>
        /// <param name="row"></param>
        public void MoveRowDown(int row)
        {
            if (row < Data.RowCount)
            {
                Data.MoveRow(row, row + 1);
                SwapRowTextValues(row, row + 1);
            }
        }

        /// <summary>
        /// Move the column to the right and associated text data
        /// </summary>
        /// <param name="column"></param>
        public void MoveColumnRight(int column)
        {
            if (column < Data.ColumnCount)
            {
                Data.MoveColumn(column, column + 1);
                SwapColumnTextValues(column, column + 1);
            }
        }

        /// <summary>
        /// Move the colmn to the left and associated text data
        /// </summary>
        /// <param name="column"></param>
        public void MoveColumnLeft(int column)
        {
            if (column > 1)
            {
                Data.MoveColumn(column, column - 1);
                SwapColumnTextValues(column, column - 1);
            }
        }

        /// <summary>
        /// Set order of columns as function of current positions.  For example, in a 5 column matrix, passing order of
        /// 1,2,3,5,4 would have effect of swapping last two column positions.
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumnOrder(List<int> columns)
        {
            //Populate list of current column positions. We'll update this list so we can
            // easily tell where to move columns
            var currentColumnList = new List<int>();

            for (int i = 1; i <= Data.ColumnCount; i++)
            {
                currentColumnList.Add(i);
            }


            //Now handle moving columnns
            for (int i = 1; i <= columns.Count; i++)
            {
                var columnToMove = columns[i - 1];

                //Locate current position of this column, which may have changed due to movement of 
                // other columns.
                var currentColumnPosition = currentColumnList.IndexOf(columnToMove) + 1;

                //Since items in columns array are in order, new column position is i
                MoveColumn(currentColumnPosition, i);

                //Update current column list.  Account for fact that column positiosn are 1-based, but
                // indexes for list elements are 0 based.
                Utilities.MoveListElement(currentColumnList, currentColumnPosition - 1, i - 1);
            }
        }

        /// <summary>
        /// Remove the specified column
        /// </summary>
        /// <param name="column"></param>
        public void RemoveColumn(int column)
        {
            if (column > 0)
            {
                //Bubble the text for columns over
                for (int i = column; i <= Data.ColumnCount; i++)
                {
                    SwapColumnTextValues(i, i + 1);
                }

                //Remove the last column, which now contains the text for the deleted column
                if (_columnTextModified.ContainsKey(Data.ColumnCount))
                {
                    _columnTextModified.Remove(Data.ColumnCount);
                }

                if (_columnTexts.ContainsKey(Data.ColumnCount))
                {
                    _columnTexts.Remove(Data.ColumnCount);
                }

                if (_columnDefaultTextsModified.ContainsKey(Data.ColumnCount))
                {
                    _columnDefaultTextsModified.Remove(Data.ColumnCount);
                }

                if (_columnDefaultTexts.ContainsKey(Data.ColumnCount))
                {
                    _columnDefaultTexts.Remove(Data.ColumnCount);
                }

                Data.RemoveColumn(column);
            }
        }

        /// <summary>
        /// Swap text data for the specified rows
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        private void SwapRowTextValues(int row1, int row2)
        {
            SwapTextValues(row1, row2, _rowTexts, _rowTextModified);
        }


        /// <summary>
        /// Swap the texts for two columns
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        private void SwapColumnTextValues(int column1, int column2)
        {
            SwapTextValues(column1, column2, _columnTexts, _columnTextModified);
            SwapTextValues(column1, column2, _columnDefaultTexts, _columnDefaultTextsModified);
            SwapOptionTextValues(column1, column2);
            SwapRatingScaleTextValues(column1, column2);
        }

        /// <summary>
        /// Swap option texts
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        private void SwapOptionTextValues(int column1, int column2)
        {
            //Step 1:  Put all options from column 1 into a temp dictionary
            var tempDict1 = new Dictionary<string, string>();
            var tempModDict1 = new Dictionary<string, bool>();

            foreach (string key in _optionTexts.Keys)
            {
                if (key.StartsWith(column1 + "_"))
                {
                    tempDict1[key] = _optionTexts[key];

                    if (_optionTextsModified.ContainsKey(key))
                    {
                        tempModDict1[key] = _optionTextsModified[key];
                    }
                }
            }

            //Step 2:  Remove the existing column 1 options
            foreach (string key in tempDict1.Keys)
            {
                if (_optionTexts.ContainsKey(key))
                {
                    _optionTexts.Remove(key);
                }

                if (_optionTextsModified.ContainsKey(key))
                {
                    _optionTextsModified.Remove(key);
                }
            }

            //Step 3: Get the texts to move to column 1
            var tempDict2 = new Dictionary<string, string>();
            var tempModDict2 = new Dictionary<string, bool>();

            foreach (string key in _optionTexts.Keys)
            {
                if (key.StartsWith(column2 + "_"))
                {
                    tempDict2[key] = _optionTexts[key];

                    if (_optionTextsModified.ContainsKey(key))
                    {
                        tempModDict2[key] = _optionTextsModified[key];
                    }
                }
            }

            //Step 4: Move the texts into column 1
            foreach (string key in tempDict2.Keys)
            {
                string newKey = key.Replace(column2 + "_", column1 + "_");

                _optionTexts[newKey] = _optionTexts[key];

                if (_optionTextsModified.ContainsKey(key))
                {
                    _optionTextsModified[newKey] = _optionTextsModified[key];
                }
            }

            //Step 5:  Remove the old column 2 texts
            foreach (string key in tempDict2.Keys)
            {
                if (_optionTexts.ContainsKey(key))
                {
                    _optionTexts.Remove(key);
                }

                if (_optionTextsModified.ContainsKey(key))
                {
                    _optionTextsModified.Remove(key);
                }
            }

            //Step 6:  Move the texts from the temp dictionary into column 2
            foreach (string key in tempDict1.Keys)
            {
                string newKey = key.Replace(column1 + "_", column2 + "_");

                _optionTexts[newKey] = tempDict1[key];

                if (tempModDict1.ContainsKey(key))
                {
                    _optionTextsModified[newKey] = tempModDict1[key];
                }
            }
        }

        /// <summary>
        /// Swap text values for rating scales
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        private void SwapRatingScaleTextValues(int column1, int column2)
        {
            //Start Text
            string key1 = column1 + "_start";
            string key2 = column2 + "_start";
            SwapTextValues(key1, key2, _scaleTexts, _scaleTextsModified);

            key1 = column1 + "_middle";
            key2 = column2 + "_middle";
            SwapTextValues(key1, key2, _scaleTexts, _scaleTextsModified);

            key1 = column1 + "_end";
            key2 = column2 + "_end";
            SwapTextValues(key1, key2, _scaleTexts, _scaleTextsModified);
        }

        /// <summary>
        /// Swap text values
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="textDict"></param>
        /// <param name="modDict"></param>
        private static void SwapTextValues<T>(T key1, T key2, IDictionary<T, string> textDict, IDictionary<T, bool> modDict)
        {
            SwapTextValues(key1, key2, textDict);
            SwapTextValues(key1, key2, modDict);
        }

        /// <summary>
        /// Do the swap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="dict"></param>
        private static void SwapTextValues<T, U>(T key1, T key2, IDictionary<T, U> dict)
        {
            if (dict.ContainsKey(key1) && !dict.ContainsKey(key2))
            {
                dict[key2] = dict[key1];
                dict.Remove(key1);
            }
            else if (!dict.ContainsKey(key1) && dict.ContainsKey(key2))
            {
                dict[key1] = dict[key2];
                dict.Remove(key2);
            }
            else if (dict.ContainsKey(key1) && dict.ContainsKey(key2))
            {
                U tmp = dict[key1];
                dict[key1] = dict[key2];
                dict[key2] = tmp;
            }
        }

        /// <summary>
        /// Sets new order for the Matrix Item Data and for the temporary structures like rowTexts
        /// </summary>
        /// <param name="newOrder"></param>
        public void SetRowOrder(List<int> newOrder)
        {
            Data.SetRowOrder(newOrder);

            Dictionary<int, string> rowTextsUpdated = new Dictionary<int, string>();

            for (int i = 0; i < newOrder.Count; i++)
            {
                if (newOrder[i] - 1 != i)
                    _rowTextModified[i + 1] = true;

                string text = _rowTexts[newOrder[i]];
                if (Utilities.IsHtmlFormattedText(text))
                    text = Utilities.AdvancedHtmlDecode(text);

                rowTextsUpdated[i + 1] = text;
            }

            _rowTexts.Clear();
            foreach (int k in rowTextsUpdated.Keys)
            {
                _rowTexts[k] = rowTextsUpdated[k];
            }            
        }
    }
}
