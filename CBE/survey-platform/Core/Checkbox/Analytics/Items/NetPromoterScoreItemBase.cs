using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class NetPromoterScoreItemBase : AnalysisItem
    {
        private long _idSeed = 1000;    //Counter for answers id

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return AggregatedData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return AggregateAndCompute(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract ReportDataCalculator CreateCalculator();

        /// <summary>
        /// Aggregate survey response data for items to report
        /// </summary>
        /// <param name="isPreview">Indicate if preview data should be generated
        /// or if actual answer data should be used.</param>
        /// <returns>DataSet that the report item data can be bound to.</returns>
        private AnalysisItemResult AggregateAndCompute(bool isPreview)
        {
            //Get an answer aggregator populated with preview results.
            //ItemAnswerAggreator is a useful object for packaging item/option texts
            // as well as answers to questions.  The aggregator is used by a 
            // ReportDataCalculator which performs the actual data calculation.
            ItemAnswerAggregator aggregator = AggregateResults(isPreview);

            //Create the calculator and initialize it with the compute checked ReportOption
            var calculator = CreateCalculator();

            //Get computed result
            return calculator.GetData(aggregator);
        }

        protected override IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_StatisticsItem");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, SourceResponseTemplateId);
            command.AddInParameter("ItemIDString", DbType.String, itemId);
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

            var answers = new List<ItemAnswer>();

            //execute the function
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //read global values
                    reader.Read();
                    responseCount = DbUtility.GetValueFromDataReader(reader, "responsecount", 0);

                    //read options data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        answers.Add(
                            new ItemAnswer
                            {
                                ItemId = itemId,
                                Count = DbUtility.GetValueFromDataReader(reader, "answercount", 0),
                                Points = DbUtility.GetValueFromDataReader(reader, "points", 0d),
                                OptionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1)
                            });
                    }
                }
                finally
                {
                    //Close the reader and rethrow the exception
                    reader.Close();
                }
            }

            return answers;
        }

        protected override AnalysisItemResult AggregatedData()
        {
            List<ItemAnswer> answers = new List<ItemAnswer>();

            var itemResponseCounts = new Dictionary<int, int>();
            var itemAnswerCounts = new Dictionary<int, int>();

            int responseCount = 0;
            foreach (var itemId in SourceItemIds)
            {
                answers.AddRange(RetrieveData(itemId, out responseCount));
                itemResponseCounts.Add(itemId, responseCount);
            }

            var calculator = CreateCalculator();
            var statisticsCalc = calculator as NetPromoterScoreStatisticsItemDataCalculator;
            if (statisticsCalc != null)
            {
                statisticsCalc.TotalResponsesCount = responseCount;
            }

            //process answers
            var result = calculator.GetData(answers);
            result.ItemResponseCounts = itemResponseCounts;
            result.ItemAnswerCounts = itemAnswerCounts;

            return result;
        }


        /// <summary>
        /// Aggregate response data for source items or generate sample data 
        /// to be used for generating a report preview. The data will include 
        /// only a limited number of answers to avoid causing any performance 
        /// issues. 
        /// </summary>
        /// <returns>ItemAnswerAggregator containining preview data.</returns>
        private ItemAnswerAggregator AggregateResults(bool isPreview)
        {
            var aggregator = new ItemAnswerAggregator(false);

            //Now add data to the aggregator for the survey responses or generate preview data.            

            //Add meta data information, such as question texts, option lists, etc.
            foreach (var sourceItemId in SourceItemIds)
            {
                //Add the item and text to aggregator.
                aggregator.AddItem(sourceItemId, GetItemText(sourceItemId), GetSourceItemTypeName(sourceItemId));

                //Now get options for source items
                List<int> itemOptionIds = isPreview
                                              ? GetItemOptionIdsForPreview(sourceItemId)
                                              : GetItemOptionIdsForReport(sourceItemId);

                //Add item options to aggregator
                foreach (int itemOptionId in itemOptionIds)
                {
                    aggregator.AddItemOption(sourceItemId, itemOptionId, GetOptionText(sourceItemId, itemOptionId),
                        GetOptionPoints(sourceItemId, itemOptionId), GetOptionIsOther(sourceItemId, itemOptionId));
                }

                List<ItemAnswer> itemAnswers = isPreview ? GeneratePreviewAnswers(sourceItemId) : GetItemAnswers(sourceItemId);

                //Add item answers to aggregator
                foreach (ItemAnswer answerData in itemAnswers)
                {
                    aggregator.AddAnswer(answerData.AnswerId, answerData.ResponseId, null, answerData.ItemId,
                        answerData.OptionId, answerData.AnswerText, answerData.Points, null, answerData.Count);
                }
            }
            return aggregator;
        }

        /// <summary>
        /// Generate a list of answers for the report preview.
        /// </summary>
        private List<ItemAnswer> GeneratePreviewAnswers(int sourceItemId)
        {
            var answerDataList = new List<ItemAnswer>();

            //add fake values
            var selectedValues = new [] {10,9,10,9,10,9,10,7,8,3,4}; 

            //Add some fake answers to be added to the aggregator
            for (int i = 0; i < selectedValues.Length; i++)
            {
                //Create an answer data object to add to the list.
                var answerData = new ItemAnswer
                {
                    AnswerId = ++_idSeed,
                    ResponseId = _idSeed,
                    ItemId = sourceItemId,
                    OptionId = 1000,
                    Points = selectedValues[i],
                    Count = 1
                };

                answerDataList.Add(answerData);
            }

            return answerDataList;
        }

    }
}
