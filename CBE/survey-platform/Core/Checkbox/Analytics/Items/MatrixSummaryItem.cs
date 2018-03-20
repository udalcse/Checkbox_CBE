using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Computation;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Summary item for calculating matrix summary information, which includes a table
    /// for option results and a table for open-ended question results.
    /// </summary>
    [Serializable]
    public class MatrixSummaryItem : AnalysisItem
    {
        private int _matrixSourceItem;
        private SummaryItemProcessedData _sourceData;

        /// <summary>
        /// Simple data container for caching processed data
        /// </summary>
        [Serializable]
        public class SummaryItemProcessedData
        {
            public readonly List<int> OtherRows;
            public readonly List<int> HeaderRows;

            public readonly Dictionary<int, int> RowItems;
            public readonly Dictionary<int, int> ColumnItems;

            public readonly Dictionary<int, List<int>> ItemOptions;
            public readonly Dictionary<Coordinate, int> MatrixItems;
            public readonly Dictionary<int, string> ItemsTypes;

            public readonly List<int> RatingScaleColumns;
            public readonly List<int> SumTotalColumns;
            public readonly List<int> SliderColumns;

            //Overridden texts for "other" rows
            public readonly Dictionary<int, string> RowTexts;

            public int RowCount;
            public int ColumnCount;

            //Fields used by the item renderer
            public SummaryItemProcessedData()
            {
                OtherRows = new List<int>();
                HeaderRows = new List<int>();
                RowItems = new Dictionary<int, int>();
                ColumnItems = new Dictionary<int, int>();

                ItemOptions = new Dictionary<int, List<int>>();
                MatrixItems = new Dictionary<Coordinate, int>(new CoordinateComparer());
                ItemsTypes = new Dictionary<int, string>();

                RatingScaleColumns = new List<int>();
                SumTotalColumns = new List<int>();
                SliderColumns = new List<int>();

                RowTexts = new Dictionary<int, string>();

            }
        }

        /// <summary>
        /// Get reference to processed data
        /// </summary>
        private SummaryItemProcessedData ProcessedData
        {
            get
            {
                if (_sourceData == null)
                {
                    //Check cache
                    var processedDataCacheItem = AnalysisDataProxy.GetResultFromCache<SummaryItemProcessedData>(
                        ID,
                        LanguageCode,
                        GenerateDataKey() + "_SourceData");

                    if (processedDataCacheItem != null)
                    {
                        _sourceData = processedDataCacheItem.Data;
                    }
                }

                if (_sourceData == null)
                {
                    _sourceData = ProcessSourceData();

                    if (_sourceData != null)
                    {
                        AnalysisDataProxy.AddResultToCache(ID, LanguageCode, GenerateDataKey() + "_SourceData", _sourceData);
                    }
                }

                return _sourceData;
            }

            set
            {
                _sourceData = value;

                if (_sourceData != null)
                {
                    AnalysisDataProxy.AddResultToCache(ID, LanguageCode, GenerateDataKey() + "_SourceData", _sourceData);
                }
            }
        }


        /// <summary>
        /// Configure the item.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            if (((MatrixSummaryItemData)itemData).MatrixSourceItem != null)
            {
                _matrixSourceItem = ((MatrixSummaryItemData)itemData).MatrixSourceItem.Value;
            }

            base.Configure(itemData, languageCode, templateId);
        }

        ///// <summary>
        ///// Get source item ids
        ///// </summary>
        //public override ReadOnlyCollection<int> SourceItemIds
        //{
        //    get
        //    {
        //        var sourceItemIDs = new List<int> { _matrixSourceItem };

        //        return new ReadOnlyCollection<int>(sourceItemIDs);
        //    }
        //}

      


        /// <summary>
        /// Get a boolean indicating if a row is an "other" row.
        /// </summary>
        /// <param name="row">Row number.</param>
        /// <returns>Boolean value</returns>
        public bool IsRowOther(int row)
        {
            return ProcessedData.OtherRows.Contains(row);
        }

        /// <summary>
        /// Get a boolean indicating if a column is a sum total column.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Boolean value</returns>
        public bool IsColumnSumTotal(int column)
        {
            return ProcessedData.SumTotalColumns.Contains(column);
        }

        /// <summary>
        /// Get a boolean indicating if a column is a rating scale column.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Boolean value</returns>
        public bool IsColumnRatingScale(int column)
        {
            return ProcessedData.RatingScaleColumns.Contains(column);
        }

        /// <summary>
        /// Get a boolean indicating if a column is a slider column.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Boolean value</returns>
        public bool IsColumnSlider(int column)
        {
            return ProcessedData.SliderColumns.Contains(column);
        }

        /// <summary>
        /// Get the ID of the item at the specified location in the matrix
        /// </summary>
        /// <param name="row">Row position</param>
        /// <param name="column">Column position</param>
        /// <returns>ID of item at the position.  If no item is found, a negative number is returned</returns>
        public int GetItemIDAt(int row, int column)
        {
            var c = new Coordinate(column, row);

            if (ProcessedData.MatrixItems.ContainsKey(c))
            {
                return ProcessedData.MatrixItems[c];
            }

            return -1;
        }

        /// <summary>
        /// Process the data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return AggregatedData();
        }

        /// <summary>
        /// Generate preview data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return ProcessPreviewData();
        }


        /// <summary>
        /// Process report data
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult ProcessPreviewData()
        {
            //Aggregate and summarize data
            var answerAggregator = new ItemAnswerAggregator(UseAliases);
            var calculator = new MatrixReportDataCalculator();

            //Reprocess source data
            ProcessedData = ProcessSourceData();

            //Build the list of columns to include
            var columnsToInclude = new List<int>(ProcessedData.ColumnItems.Keys);

            //Loop through all rows and columns to add possible answers
            for (var row = 1; row <= ProcessedData.RowCount; row++)
            {
                //Columns
                foreach (int column in columnsToInclude)
                {
                    var coordinate = new Coordinate(column, row);

                    if (!ProcessedData.MatrixItems.ContainsKey(coordinate))
                    {
                        continue;
                    }

                    var itemId = ProcessedData.MatrixItems[coordinate];

                    //Add an answer group for the column
                    //Item text is not needed since the relevant texts are part of the row/colum headers when rendererd

                    var itemOptionIds = GetItemOptionIdsForPreview(itemId);

                    answerAggregator.AddItem(
                        itemId,
                        string.Empty,
                        GetSourceItemTypeName(itemId));

                    foreach (int itemOptionID in itemOptionIds)
                    {
                        //Item text is not needed since the relevant texts are part of the row/colum headers when rendererd
                        answerAggregator.AddItemOption(itemId, itemOptionID, string.Empty,
                                                       GetOptionPoints(itemId, itemOptionID),
                                                       GetOptionIsOther(itemId, itemOptionID));
                    }

                    //If necessary, tell the calculator that an item is a rating scale or sum total
                    if (IsColumnRatingScale(column))
                    {
                        calculator.AddRatingScaleItem(itemId);
                    }
                    else if (IsColumnSumTotal(column))
                    {
                        calculator.AddSumTotalItem(itemId);
                    }
                    else if (IsColumnSlider(column))
                    {
                        calculator.AddSliderItem(itemId);
                    }
                }
            }


            long answerIdSeed = 1000;

            //Loop through all rows and columns to add actual answers
            for (int row = 1; row <= ProcessedData.RowCount; row++)
            {
                //Columns
                foreach (int column in columnsToInclude)
                {
                    var coordinate = new Coordinate(column, row);

                    if (!ProcessedData.MatrixItems.ContainsKey(coordinate))
                    {
                        continue;
                    }

                    var itemId = ProcessedData.MatrixItems[coordinate];

                    List<ItemAnswer> answers = GetItemPreviewAnswers(itemId, answerIdSeed, 1000);
                                                   
                    foreach (ItemAnswer answer in answers)
                    {
                        if (answer.OptionId.HasValue)
                        {
                            answerAggregator.AddAnswer(answer.AnswerId, answer.ResponseId, itemId, answer.OptionId);
                        }
                        else
                        {
                            answerAggregator.AddAnswer(answer.AnswerId, answer.ResponseId, itemId, answer.AnswerText);
                        }
                    }
                }

                answerIdSeed++;
            }

            var result = calculator.GetData(answerAggregator);

            //Store the response count
            ResponseCounts[_matrixSourceItem] = answerAggregator.GetResponseCount(null);

            return result;
        }

        /// <summary>
        /// Build parameter data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetItemsAndOptionsIds(SummaryItemProcessedData data)
        {
            string result = null;
            for (int y = 1; y <= data.RowCount; y++)
            {
                for (int x = 1; x <= data.ColumnCount; x++)
                {
                    var itemId = data.MatrixItems.FirstOrDefault(i => i.Key.X == x && i.Key.Y == y).Value;
                    if (itemId != 0)
                        result += itemId + ",";
                }
            }

            return string.IsNullOrEmpty(result)? string.Empty : result.Remove(result.Length - 1);
        }

        /// <summary>
        /// Aggregate result data 
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        private AggregateResult[] AggregateResult(Dictionary<int, Dictionary<int, int>> answers)
        {
            List<AggregateResult> aggregateResults = new List<AggregateResult>();

            for (int y = 1; y <= ProcessedData.RowCount; y++)
            {
                for (int x = 1; x <= ProcessedData.ColumnCount; x++)
                {
                    var itemId = ProcessedData.MatrixItems.FirstOrDefault(i => i.Key.X == x && i.Key.Y == y).Value;
                    if (itemId != 0 && ProcessedData.ItemOptions.ContainsKey(itemId))
                    {
                        foreach (var optionId in ProcessedData.ItemOptions[itemId])
                        {
                            int count = 0;
                            double total = 1d;
                            if (answers.ContainsKey(itemId) && answers[itemId].ContainsKey(optionId))
                            {
                                count = answers[itemId][optionId];
                                total = answers[itemId].Sum(i => i.Value);
                            }

                            aggregateResults.Add(new AggregateResult
                            {
                                ResultKey = itemId + "_" + optionId,
                                AnswerCount = count,
                                AnswerPercent = count * 100d / total
                            });
                        }
                    }
                }
            }

            return aggregateResults.ToArray();
        }
        
        /// <summary>
        /// Get source rating scale items
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<int> GetRatingScaleID(SummaryItemProcessedData data)
        {
            return (from type in data.ItemsTypes
                    where type.Value.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase)
                    select type.Key).ToList();
        }

        /// <summary>
        /// Get source matrix sum total items
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<int> GetMatrixSumTotalID(SummaryItemProcessedData data)
        {
            return (from type in data.ItemsTypes
                    where type.Value.Equals("MatrixSumTotal", StringComparison.InvariantCultureIgnoreCase)
                    select type.Key).ToList();
        }

        /// <summary>
        /// Get source matrix slider items
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<int> GetMatrixSliderID(SummaryItemProcessedData data)
        {
            return (from type in data.ItemsTypes
                    where type.Value.Equals("Slider", StringComparison.InvariantCultureIgnoreCase)
                    select type.Key).ToList();
        }

        /// <summary>
        /// Calculate rating scale averages
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        private Dictionary<int, double> SliderAverages(IEnumerable<int> itemIds, Dictionary<int, Dictionary<int, int>> answers)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (int itemId in itemIds)
            {
                if (answers.ContainsKey(itemId))
                {
                    var answer = answers[itemId].FirstOrDefault();
                    double totalScore = (double)answer.Key / (answer.Value > 0 ? answer.Value : 1);
                    result.Add(itemId, totalScore);
                }
            }
            return result;
        }


        /// <summary>
        /// Calculate rating scale averages
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        private Dictionary<int, double> RatingScaleAverages(IEnumerable<int> itemIds, Dictionary<int, Dictionary<int, int>> answers)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (int itemId in itemIds)
            {
                if(answers.ContainsKey(itemId))
                {
                    double totalScore = 0;
                    foreach (var optionId in answers[itemId].Keys)
                    {
                        totalScore += answers[itemId][optionId] * GetOptionPoints(itemId, optionId);
                    }
                    int scoreCount = answers[itemId].Where(o => !SurveyMetaDataProxy.GetOptionData(o.Key, itemId, true).IsOther ).Sum(a => a.Value);
                    result.Add(itemId, scoreCount > 0 ? totalScore/scoreCount : 0);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate sum total averages
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        private Dictionary<int, double> SumTotalAverages(IEnumerable<int> itemIds, Dictionary<int, Dictionary<int, int>> answers)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (int itemId in itemIds)
            {
                if (answers.ContainsKey(itemId))
                {
                    double totalScore = 0;

                    string answertext = GetItemText(itemId);
                    if (Utilities.IsNumeric(answertext))
                        totalScore = Convert.ToDouble(answertext);

                    int scoreCount = answers[itemId].Sum(a => a.Value);
                    result.Add(itemId, scoreCount > 0 ? totalScore / scoreCount : 0);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult AggregatedData()
        {
            ProcessedData = ProcessSourceData();

            var itemResponseCounts = new Dictionary<int, int>();
            var itemAnswerCounts = new Dictionary<int, int>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_MatrixSummary");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, SourceResponseTemplateId);
            command.AddInParameter("ItemIDString", DbType.String, GetItemsAndOptionsIds(ProcessedData));
            command.AddInParameter("IncludeIncompleteResponses", DbType.Byte, Report.IncludeIncompleteResponses);
            command.AddInParameter("IncludeTestResponses", DbType.Byte, Report.IncludeTestResponses);
            var filterStrings = BuildFilterStrings();
            if (filterStrings != null)
            {
                foreach (var filterParameter in filterStrings.Keys)
                {
                    command.AddInParameter(filterParameter, DbType.String, filterStrings[filterParameter].ToString());
                }
            }
            command.AddInParameter("StartDate", DbType.DateTime, Report.FilterStartDate);
            command.AddInParameter("EndDate", DbType.DateTime, Report.FilterEndDate);

            //dictionary <itemID,<optionID, answerCount>>
            Dictionary<int, Dictionary<int, int>> answers = new Dictionary<int, Dictionary<int, int>>();

            //execute the function
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //read global values
                    reader.Read();
                    int responseCount = DbUtility.GetValueFromDataReader(reader, "responsecount", 0);
                    itemResponseCounts.Add(_matrixSourceItem, responseCount);

                    //read answers data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int itemId = DbUtility.GetValueFromDataReader(reader, "itemid", -1);
                        int optionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1);
                        int answerCount = DbUtility.GetValueFromDataReader(reader, "answercount", 0);
                        int answersum = DbUtility.GetValueFromDataReader(reader, "answersum", 0);

                        if (!answers.ContainsKey(itemId))
                            answers.Add(itemId, new Dictionary<int, int>());

                        if (optionId > -1)
                            answers[itemId].Add(optionId, answerCount);
                        else
                            answers[itemId].Add(answersum, answerCount);
                    }
                }
                finally 
                {
                    //Close the reader and rethrow the exception
                    reader.Close();
                }
            }
            
            return new MatrixSummaryAnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                AggregateResults = AggregateResult(answers),
                RatingScaleAverages = RatingScaleAverages(GetRatingScaleID(ProcessedData), answers),
                SumTotalAverages = SumTotalAverages(GetMatrixSumTotalID(ProcessedData), answers),
                SliderAverages = SliderAverages(GetMatrixSliderID(ProcessedData), answers)
            };
        }

        /// <summary>
        /// Process source data for item
        /// </summary>
        /// <returns></returns>
        protected virtual SummaryItemProcessedData ProcessSourceData()
        {
            var data = new SummaryItemProcessedData();

            if (_matrixSourceItem <= 0)
            {
                return data;
            }

            LightweightItemMetaData matrixItemData = SurveyMetaDataProxy.GetItemData(_matrixSourceItem, TemplatesValidated);

            if (matrixItemData == null)
            {
                return data;
            }

            //Load child items and options
            LoadChildItemData(matrixItemData, data);

            //Build the list of columns to include
            var columnsToInclude = new List<int>(data.ColumnItems.Keys);

            //Loop through all rows and columns to add possible answers
            for (var row = 1; row <= data.RowCount; row++)
            {
                //Columns
                foreach (int column in columnsToInclude)
                {
                    var coordinate = new Coordinate(column, row);

                    if (!data.MatrixItems.ContainsKey(coordinate))
                    {
                        continue;
                    }

                    var itemId = data.MatrixItems[coordinate];

                    //Add an answer group for the column
                    //Item text is not needed since the relevant texts are part of the row/colum headers when rendererd

                    data.ItemOptions[itemId] = PreviewMode
                                                  ? GetItemOptionIdsForPreview(itemId)
                                                  : GetItemOptionIdsForReport(itemId);

                }
            }

            return data;
        }

        /// <summary>
        /// Load the children of the matrix item and add them to the
        /// internal items collection.
        /// </summary>
        /// <param name="matrixItemData"></param>
        /// <param name="processedData"></param>
        private void LoadChildItemData(LightweightItemMetaData matrixItemData, SummaryItemProcessedData processedData)
        {
            //Store column count, row count, and pk index
            processedData.ColumnCount = matrixItemData.ColumnCount;
            processedData.RowCount = matrixItemData.RowCount;

            processedData.MatrixItems.Clear();

            foreach (int childItemId in matrixItemData.Children)
            {
                Coordinate childCoordinate = matrixItemData.GetChildCoordinate(childItemId);

                if (childCoordinate != null)
                {
                    processedData.MatrixItems[childCoordinate] = childItemId;
                    processedData.ItemsTypes[childItemId] = GetSourceItemTypeName(childItemId);
                }
            }

            processedData.RatingScaleColumns.Clear();
            processedData.SumTotalColumns.Clear();
            processedData.SliderColumns.Clear();

            for (int columnNumber = 1; columnNumber <= processedData.ColumnCount; columnNumber++)
            {
                LoadColumnPrototype(columnNumber, matrixItemData.GetColumnPrototypeID(columnNumber), processedData);
            }

            for (int rowNumber = 1; rowNumber <= processedData.RowCount; rowNumber++)
            {
                LoadRowPkItem(rowNumber, matrixItemData.GetRowPkItemId(rowNumber), matrixItemData.GetRowType(rowNumber), processedData);
            }
        }


        /// <summary>
        /// Load the column prototype for a column
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="columnPrototypeId"></param>
        /// <param name="processedData"></param>
        private void LoadColumnPrototype(int columnNumber, int? columnPrototypeId, SummaryItemProcessedData processedData)
        {
            if (!columnPrototypeId.HasValue
                || columnNumber <= 0)
            {
                return;

            }
            LightweightItemMetaData columnPrototype = SurveyMetaDataProxy.GetItemData(columnPrototypeId.Value, TemplatesValidated);

            if (columnPrototype != null)
            {
                //Set column type
                if ("RadioButtonScale".Equals(columnPrototype.ItemType, StringComparison.InvariantCultureIgnoreCase))
                {
                    processedData.RatingScaleColumns.Add(columnNumber);
                }

                if ("MatrixSumTotal".Equals(columnPrototype.ItemType, StringComparison.InvariantCultureIgnoreCase))
                {
                    processedData.SumTotalColumns.Add(columnNumber);
                }

                if ("Slider".Equals(columnPrototype.ItemType, StringComparison.InvariantCultureIgnoreCase))
                {
                    processedData.SliderColumns.Add(columnNumber);
                }

                //Save all except single line text
                if (!("SingleLineText".Equals(columnPrototype.ItemType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    processedData.ColumnItems[columnNumber] = columnPrototypeId.Value;
                    processedData.ItemOptions[columnPrototype.ItemId] = columnPrototype.Options;
                }
            }
        }

        /// <summary>
        /// Load the pk item for a row
        /// </summary>
        /// <param name="rowItemId"></param>
        /// <param name="rowNumber"></param>
        /// <param name="rowType"></param>
        /// <param name="processedData"></param>
        private void LoadRowPkItem(int rowNumber, int? rowItemId, string rowType, SummaryItemProcessedData processedData)
        {
            if (!rowItemId.HasValue)
            {
                return;
            }

            string rowText = SurveyMetaDataProxy.GetItemText(rowItemId.Value, LanguageCode, UseAliases, TemplatesValidated);

            //Flag other rows
            if ("Other".Equals(rowType, StringComparison.InvariantCultureIgnoreCase))
            {
                processedData.OtherRows.Add(rowNumber);
                rowText = GetText("/controlText/matrixSummaryItem/other");
            }

            //Flag subheading rows
            if ("Subheading".Equals(rowType, StringComparison.InvariantCultureIgnoreCase))
            {
                processedData.HeaderRows.Add(rowNumber);
            }

            processedData.RowTexts[rowItemId.Value] = rowText;

            processedData.RowItems[rowNumber] = rowItemId.Value;
        }

        /// <summary>
        /// Get item text override
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public override string GetItemText(int itemID)
        {
            return ProcessedData.RowTexts.ContainsKey(itemID)
                ? ProcessedData.RowTexts[itemID]
                : base.GetItemText(itemID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var instanceData = base.GetMetaDataValuesForSerialization();

            instanceData["MatrixSourceItem"] = _matrixSourceItem.ToString();

            return instanceData;
        }

        /// <summary>
        /// Add additional data to transfer object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ItemProxyObject)
            {
                ((ItemProxyObject)itemDto).AdditionalData = GetAdditionalData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void PopulateSourceItems(ReportItemInstanceData itemDto)
        {
            base.PopulateSourceItems(itemDto);

            var sourceItemList = new List<ReportItemSourceItemData>(itemDto.SourceItems);

            //Ensure row and column items populated
            foreach (var rowItemEntry in ProcessedData.RowItems)
            {
                var sourceItem = GetSourceItem(rowItemEntry.Value);

                if (sourceItem == null)
                {
                    continue;
                }

                if (ProcessedData.RowTexts.ContainsKey(rowItemEntry.Key))
                {
                    sourceItem.ReportingText = ProcessedData.RowTexts[rowItemEntry.Key];
                }

                sourceItemList.Add(sourceItem);
            }

            //Ensure child items populated
            for (int row = 1; row <= ProcessedData.RowCount; row++)
            {
                for (int column = 1; column <= ProcessedData.ColumnCount; column++)
                {
                    if (ProcessedData.ColumnItems.ContainsKey(column))
                    {
                        var itemId = GetItemIDAt(row, column);

                        if (itemId > 0)
                        {
                            sourceItemList.Add(GetSourceItem(itemId));
                        }
                    }
                }
            }

            sourceItemList.AddRange(ProcessedData.ColumnItems.Select(columnItemEntry => GetSourceItem(columnItemEntry.Value)).Where(sourceItem => sourceItem != null));

            sourceItemList.Add(GetSourceItem(_matrixSourceItem));
            itemDto.SourceItems = sourceItemList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private MatrixSummaryAdditionalData GetAdditionalData()
        {
            var additionalData = new MatrixSummaryAdditionalData
            {
                RowSourceItems = ProcessedData.RowItems.OrderBy(ri => ri.Key).ToDictionary(row => row.Value, row => ProcessedData.HeaderRows.Contains(row.Key)),
                ColumnSourceItems = ProcessedData.ColumnItems.OrderBy(ci => ci.Key).Select(ci => ci.Value).ToArray()
            };

            var columnItemPositions = new SimpleNameValueCollection();

            foreach (var columnSourceEntry in ProcessedData.ColumnItems)
            {
                columnItemPositions[columnSourceEntry.Value.ToString()] = columnSourceEntry.Key.ToString();
            }

            additionalData.ColumnItemPositions = columnItemPositions;

            //Row data
            var resultData = GetResultData();

            //Column data
            if (resultData is MatrixSummaryAnalysisItemResult)
            {
                var scaleAverageValues = new SimpleNameValueCollection();

                foreach (var scaleAverageItem in ((MatrixSummaryAnalysisItemResult) resultData).RatingScaleAverages)
                {
                    scaleAverageValues[scaleAverageItem.Key.ToString()] = string.Format("{0:0.#}", scaleAverageItem.Value);
                }

                var sumTotalAverageValues = new SimpleNameValueCollection();

                foreach (var sumTotalAverageItem in ((MatrixSummaryAnalysisItemResult) resultData).SumTotalAverages)
                {
                    sumTotalAverageValues[sumTotalAverageItem.Key.ToString()] = string.Format("{0:0.#}", sumTotalAverageItem.Value);
                }

                var sliderAverageValues = new SimpleNameValueCollection();

                foreach (var sliderAverageItem in ((MatrixSummaryAnalysisItemResult)resultData).SliderAverages)
                {
                    sliderAverageValues[sliderAverageItem.Key.ToString()] = string.Format("{0:0.#}", sliderAverageItem.Value);
                }

                additionalData.RatingScaleAverages = scaleAverageValues;
                additionalData.SumTotalAverages = sumTotalAverageValues;
                additionalData.SliderAverages = sliderAverageValues;
            }

            additionalData.MatrixChildren = new SimpleNameValueCollection();

            //Child data
            for (int row = 1; row <= ProcessedData.RowCount; row++)
            {
                for (int column = 1; column <= ProcessedData.ColumnCount; column++)
                {
                    additionalData.MatrixChildren[string.Format("{0}_{1}", row, column)] = GetItemIDAt(row, column).ToString();
                }
            }

            return additionalData;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //protected override System.Collections.Specialized.NameValueCollection GetInstanceDataValuesForSerialization()
        //{
        //    var values = base.GetInstanceDataValuesForSerialization();

        //    values["MatrixName"] = SurveyMetaDataProxy.GetItemText(this._matrixSourceItem, LanguageCode, UseAliases, TemplatesValidated);
        //    values["ColCount"] = data.ColumnCount.ToString();
        //    values["RowCount"] = data.RowCount.ToString();
        //    values["RowTexts"] = SerializeRowTexts(data.RowTexts, data.MatrixItems, data.ColumnCount);
        //    values["ColumnItems"] = SerializeColumnsItems(data.ColumnItems, data.ItemOptions);
        //    return values;
        //}

        ///// <summary>
        ///// Serializes rows texts into string
        ///// format:
        ///// number1,string1|number2,string2| ... |
        ///// commas replaced by ~comma~, line replaced by ~line~
        ///// </summary>
        ///// <param name="dict"></param>
        ///// <returns></returns>
        //protected string SerializeRowTexts(Dictionary<int,string> dict, Dictionary<Coordinate, int> items, int colCount)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    StringBuilder sbCells = new StringBuilder();

        //    int row = 1;
        //    foreach (int key in dict.Keys)
        //    {
        //        sbCells.Clear();
        //        for (int col = 1; col <= colCount; col++)
        //        {
        //            sbCells.AppendFormat("{0};", items[new Coordinate(col, row)]);
        //        }

        //        sb.AppendFormat("{0},{1},{2}|", key, dict[key], sbCells);
        //        row++;
        //    }

        //    return sb.ToString();
        //}
        
        ///// <summary>
        ///// Serializes column items into the string
        ///// Format
        /////  column_num1;column_id1;column_name1;option1^option_name1,option2^option_name2,...,|...
        ///// </summary>
        ///// <param name="dict"></param>
        ///// <returns></returns>
        //protected string SerializeColumnsItems(Dictionary<int, int> dictColumns, Dictionary<int, List<int>> dictOptions)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 1; i <= dictColumns.Count + 1; i++)
        //    {
        //        if (dictColumns.ContainsKey(i))
        //        {
        //            if (dictOptions.ContainsKey(dictColumns[i]))
        //            {
        //                List<int> options = dictOptions[dictColumns[i]];
        //                sb.AppendFormat("{0};{1};{2};", i, dictColumns[i], EncodeString(GetItemText(dictColumns[i])));
                        
        //                foreach (int option in options)
        //                {
        //                    sb.AppendFormat("{0}^{1},", option, EncodeString(GetOptionText(dictColumns[i], option)));
        //                }
        //                sb.Append("|");
        //            }
        //        }
        //    }

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Encodes a string
        ///// </summary>
        ///// <param name="src"></param>
        ///// <returns></returns>
        //public static string EncodeString(string src)
        //{
        //    return src.Replace(",", "~c~").Replace("|", "~l~").Replace("^", "~cap~").Replace(";", "~sc~");
        //}

        ///// <summary>
        ///// Decodes a string
        ///// </summary>
        ///// <param name="src"></param>
        ///// <returns></returns>
        //public static string DecodeString(string src)
        //{
        //    return src.Replace("~c~", ",").Replace("~l~", "|").Replace("~cap~", "^").Replace("~sc~", ";");
        //}
    }
}
