using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Data;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Business logic implementation of report item that handles aggregation and
    /// calculation of data to display for report item.
    /// </summary>
    [Serializable]
    public class StatisticsItem : AnalysisItem
    {
        private long _idSeed = 1000;    //Counter for answers id

        public StatisticsItemReportingOption ReportOption { get; private set; }
        public double MinPossibleAnswerValue { get; private set; }
        public double MaxPossibleAnswerValue { get; private set; }

        /// <summary>
        /// Configure the report item with its configuration data.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            ReportOption = ((StatisticsItemData)itemData).ReportOption;
        }

        /// <summary>
        /// Override this method to increase InstanceData
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection collection = base.GetInstanceDataValuesForSerialization();
            collection.Add("ReportOption", ReportOption.ToString());

            return collection;
        }

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
            var calculator = new StatisticsItemDataCalculator(ReportOption);

            //Get computed result
            AnalysisItemResult ds = calculator.GetData(aggregator);

            //Store min/max values
            MinPossibleAnswerValue = calculator.MinPossibleAnswerValue;
            MaxPossibleAnswerValue = calculator.MaxPossibleAnswerValue;

            return ds;            
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
                                Points = DbUtility.GetValueFromDataReader(reader, "points", 0),
                                Count = DbUtility.GetValueFromDataReader(reader, "answercount", 0),
                                OptionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1),
                                IsOther = DbUtility.GetValueFromDataReader(reader, "isother", false)
                            });
                    }

                    //if no option data was retrieved, there are open-ended item 
                    reader.NextResult();
                    if (answers.Count == 0)
                    {
                        while (reader.Read())
                        {
                            string text = DbUtility.GetValueFromDataReader(reader, "answertext", string.Empty);
                            if (!string.IsNullOrEmpty(text))
                                answers.Add(new ItemAnswer { ItemId = itemId, Count = 1, AnswerText = text, OptionId = -1 });
                        }
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

            foreach (var itemId in SourceItemIds)
            {
                int responseCount;
                answers.AddRange(RetrieveData(itemId, out responseCount));
                itemResponseCounts.Add(itemId, responseCount);
            }

            double? min, max;
            //process answers
            var result = new AnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                CalculateResults = StatisticsItemDataCalculator.Calculate(answers, ReportOption, out min, out max)
            };

            MinPossibleAnswerValue = min.Value;
            MaxPossibleAnswerValue = max.Value;

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
                    aggregator.AddAnswer(answerData.AnswerId, answerData.ResponseId, answerData.ItemId,
                        answerData.OptionId, answerData.AnswerText);
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

            //Get list of option ids for the item
            List<int> itemOptionIds = GetItemOptionIdsForPreview(sourceItemId);

            //Add some fake answers to be added to the aggregator
            for (int optionCount = 0; optionCount < itemOptionIds.Count; optionCount++)
            {
                //Add answers in a answerDataList
                for (int answerCount = 0; answerCount <= optionCount; answerCount++)
                {
                    //Create an answer data object to add to the list.
                    var answerData = new ItemAnswer
                    {
                        AnswerId = ++_idSeed,
                        ResponseId = _idSeed,
                        ItemId = sourceItemId,
                        OptionId = itemOptionIds[optionCount]
                    };

                    answerDataList.Add(answerData);
                }
            }

            return answerDataList;
        }
    }
}
