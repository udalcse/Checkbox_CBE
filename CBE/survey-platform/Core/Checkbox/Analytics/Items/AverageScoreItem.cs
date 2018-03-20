using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class AverageScoreItem : AnalysisItem 
	{
        private AverageScoreCalculation _scoreOption;

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            _scoreOption = ((AverageScoreItemData)itemData).AverageScoreCalculation;
        }

        /// <summary>
        /// Get the calculation option
        /// </summary>
        public virtual AverageScoreCalculation ScoreOption
        {
            get { return _scoreOption; }
        }

        /// <summary>
        /// Process data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return AggregatedData();
        }

        /// <summary>
        /// Generate preview data for the report
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return ProcessPreviewData();
        }


        /// <summary>
        /// Process the data
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult ProcessPreviewData()
        {
            var answerAggregator = new ItemAnswerAggregator(UseAliases);
            var calculator = new AverageScoreDataCalculator();

            //Add the possibles
            foreach (int itemID in SourceItemIds)
            {
                List<int> optionIDs = GetItemOptionIdsForPreview(itemID);

               answerAggregator.AddItem(
                   itemID,
                   GetItemText(itemID),
                   GetSourceItemTypeName(itemID));

                foreach (int optionID in optionIDs)
                {
                    answerAggregator.AddItemOption(itemID, optionID, GetOptionText(itemID, optionID), GetOptionPoints(itemID, optionID), GetOptionIsOther(itemID, optionID));
                }
            }

            //Add the answers
            long answerIdSeed = 1000;

            foreach (int itemID in SourceItemIds)
            {
                List<ItemAnswer> answers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);

                foreach (ItemAnswer answer in answers)
                {
                    if (answer.AnswerId > answerIdSeed)
                    {
                        answerIdSeed = answer.AnswerId;
                    }

                    answerAggregator.AddAnswerWithPoints(answer.AnswerId, answer.ResponseId, itemID, answer.OptionId, answer.Points);
                }

                //Store response count
                ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);

                answerIdSeed++;
            }

            return calculator.GetData(answerAggregator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_AverageScore");
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

            List<ItemAnswer> answers = new List<ItemAnswer>();

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
                                Count = responseCount
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

            foreach (var itemId in SourceItemIds)
            {
                int responseCount;
                answers.AddRange(RetrieveData(itemId, out responseCount));
                itemResponseCounts.Add(itemId, responseCount);
            }

            return new AnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                AggregateResults = AggregateResult(answers)
            };
        }

        protected override AggregateResult[] AggregateResult(IEnumerable<ItemAnswer> answers)
        {
            return answers.Select(answer => new AggregateResult
                                                {
                                                    ResultKey = answer.ItemId.ToString(), 
                                                    ResultText = GetItemText(answer.ItemId),
                                                    AnswerCount = answer.Count,
                                                    AnswerPercent = answer.Count > 0 && answer.Points.HasValue ?
                                                        answer.Points.Value / answer.Count : 0
                                                
                                                }).ToArray();
        }

	}
}
