using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using System.Collections.Specialized;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Cross-tab analysis item
    /// </summary>
    [Serializable]
    public class CrossTabItem : AnalysisItem
    {
        /// <summary>
        /// Configuration for a cross tab item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);
            InitializeData(itemData);
        }

        /// <summary>
        /// Initialize data
        /// </summary>
        protected virtual void InitializeData(ItemData itemData)
        {
            XAxisItemIDs = ((CrossTabItemData)itemData).XAxisItemIds;
            YAxisItemIDs = ((CrossTabItemData)itemData).YAxisItemIds;
        }

        /// <summary>
        /// Get item ids for x axis
        /// </summary>
        public List<Int32> XAxisItemIDs { get; private set; }

        /// <summary>
        /// Get item ids for y axis
        /// </summary>
        public List<Int32> YAxisItemIDs { get; private set; }

        /// <summary>
        /// Process report data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return AggregatedData();
        }

        /// <summary>
        /// Generate data for report preview
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return ProcessPreviewData();
        }

        /// <summary>
        /// Process the data and calculate summaries
        /// </summary>
        /// <returns>Data object.</returns>
        protected virtual AnalysisItemResult ProcessPreviewData()
        {
            try
            {
                var answerAggregator = new ItemAnswerAggregator(UseAliases);
                var calculator = new CrossTabReportDataCalculator();

                //Build the list of items and options for each axis
                //NOTE: For the cross-tab there is no item at position 1,1, so the items start
                //      Position 2,1 for columns and 1,2 for rows, which means the max row option position is (1, RowCount +1)
                //      and max column option position is (ColumnCount + 1, 1)
                int columnOptionPosition = 2;
                foreach (Int32 itemID in XAxisItemIDs)
                {
                    List<Int32> optionIDs = GetItemOptionIdsForPreview(itemID);

                    if (optionIDs.Count <= 0)
                    {
                        continue;
                    }

                    answerAggregator.AddItem(
                        itemID,
                        GetItemText(itemID),
                        GetSourceItemTypeName(itemID));

                    foreach (Int32 optionID in optionIDs)
                    {
                        answerAggregator.AddItemOption(itemID, optionID, GetOptionText(itemID, optionID));
                        calculator.SetOptionResultsPosition(optionID, new Coordinate(columnOptionPosition, 1));
                        columnOptionPosition++;
                    }
                }

                Int32 rowOptionPosition = 2;
                foreach (Int32 itemID in YAxisItemIDs)
                {
                    List<Int32> optionIDs = GetItemOptionIdsForPreview(itemID);

                    if (optionIDs.Count <= 0) 
                    {
                        continue;
                    }
                
                    answerAggregator.AddItem(
                        itemID,
                        GetItemText(itemID),
                        GetSourceItemTypeName(itemID));

                    foreach (Int32 optionID in optionIDs)
                    {
                        answerAggregator.AddItemOption(itemID, optionID, GetOptionText(itemID, optionID));

                        answerAggregator.AddItemOption(itemID, optionID, GetOptionText(itemID, optionID));
                        calculator.SetOptionResultsPosition(optionID, new Coordinate(1, rowOptionPosition));
                        rowOptionPosition++;
                    }
                }
                long answerIdSeed = 1000;

                //Add the answer data now
                foreach (Int32 itemID in YAxisItemIDs)
                {
                    List<ItemAnswer> answers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);

                    foreach (ItemAnswer answer in answers)
                    {
                        if (answer.AnswerId > answerIdSeed)
                        {
                            answerIdSeed = answer.AnswerId;
                        }

                        if (answer.OptionId.HasValue)
                        {
                            answerAggregator.AddAnswer(
                                answer.AnswerId,
                                answer.ResponseId,
                                itemID,
                                answer.OptionId);
                        }

                        answerIdSeed++;
                    }

                    //Store response count
                    ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);
                }

                foreach (Int32 itemID in XAxisItemIDs)
                {
                    List<ItemAnswer> answers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);

                    foreach (ItemAnswer answer in answers)
                    {
                        if (answer.AnswerId > answerIdSeed)
                        {
                            answerIdSeed = answer.AnswerId;
                        }

                        if (answer.OptionId.HasValue)
                        {
                            answerAggregator.AddAnswer(
                                answer.AnswerId,
                                answer.ResponseId,
                                itemID,
                                answer.OptionId);
                        }

                        answerIdSeed++;
                    }

                    //Store response count
                    ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);
                }

                return calculator.GetData(answerAggregator);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
                return null;
            }
        }

        /// <summary>
        /// Concat item IDs into the string
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        private string ConcatIds(IEnumerable<int> IDs)
        {
            string result = IDs.Aggregate(string.Empty, (current, id) => current + (id + ","));
            return result.Remove(result.Length - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult AggregatedData()
        {
            if (XAxisItemIDs.Count == 0 || YAxisItemIDs.Count == 0)
                return ProcessPreviewData();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_CrossTabItem");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, SourceResponseTemplateId);
            command.AddInParameter("HorizontalItemID", DbType.String, ConcatIds(XAxisItemIDs));
            command.AddInParameter("VerticalItemID", DbType.String, ConcatIds(YAxisItemIDs));
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

            var results = new List<AggregateResult>();
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    List<int> optionIds = new List<int>();
                    foreach (var optionId in XAxisItemIDs)
                    {
                        optionIds.AddRange(GetItemOptionIdsForReport(optionId));
                    }

                    //fill total answers data
                    reader.Read();
                    Dictionary<int, int> totalAnswersData = new Dictionary<int, int>();
                    foreach (var optionIdX in optionIds)
                    {
                        int count = DbUtility.GetValueFromDataReader(reader, "col_" + optionIdX, 1);
                        totalAnswersData.Add(optionIdX, count <= 0 ? 1 : count);
                    }

                    //fill cross data
                    while (reader.Read())
                    {
                        results.AddRange(from optionIdX in optionIds
                                         let count = DbUtility.GetValueFromDataReader(reader, "col_" + optionIdX, 0)
                                         let optionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1)
                                         select new AggregateResult
                                                    {
                                                        ResultKey = string.Format("{0}_{1}", optionIdX, optionId),
                                                        AnswerCount = count, 
                                                        AnswerPercent = count*100d/totalAnswersData[optionIdX]
                                                    });
                    }

                    return new AnalysisItemResult
                    {
                        AggregateResults = results.ToArray()
                    };
                }
                finally
                {
                    //Close the reader and rethrow the exception
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values =  base.GetInstanceDataValuesForSerialization();

            values["XAxisItemIds"] = string.Join("|", (from xAxisItemId in XAxisItemIDs select xAxisItemId.ToString()).ToArray());
            values["YAxisItemIds"] = string.Join("|", (from yAxisItemId in YAxisItemIDs select yAxisItemId.ToString()).ToArray());

            return values;
        }
    }
}
