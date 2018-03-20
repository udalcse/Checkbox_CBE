using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using System.Data;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Business login implementation of the rank order summary item, which shows the sum of points
    /// across all responses for each answer.
    /// </summary>
    [Serializable]
    public class RankOrderSummary : AnalysisItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
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

            //process answers
            List<DetailResult> details = new List<DetailResult>();
            foreach (var answer in answers)
            {
                if (!itemAnswerCounts.ContainsKey(answer.ItemId))
                    itemAnswerCounts.Add(answer.ItemId, answers.Where(o => o.ItemId == answer.ItemId).Sum(o => o.Count));
            }

            return new AnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                AggregateResults = AggregateResult(answers),
                DetailResults = details.ToArray()
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            //Build the list of items & options
            var answerAggregator = new ItemAnswerAggregator(UseAliases);
            var calculator = new PointsDataCalculator();

            //Add the answers to the aggregator
            //For rating scales, the options have no text, but the points are used instead
            //Locally cache them for efficiency and only if no text is found for an option
            var optionTexts = new Dictionary<int, string>();

            //Add items, item text, options, and option texts to the answer aggregator
            foreach (int itemID in SourceItemIds)
            {
                List<int> optionIDs = GetItemOptionIdsForPreview(itemID);

                answerAggregator.AddItem(
                    itemID,
                    GetItemText(itemID),
                    GetSourceItemTypeName(itemID));

                foreach (int optionID in optionIDs)
                {
                    string optionText;

                    if (optionTexts.ContainsKey(optionID))
                    {
                        optionText = optionTexts[optionID];
                    }
                    else
                    {
                        optionText = GetOptionText(itemID, optionID);

                        //If no text, attempt to get the points for rating scale as value
                        if (optionText == null || optionText.Trim() == string.Empty)
                        {
                            optionText = GetOptionPoints(itemID, optionID).ToString();
                        }
                    }

                    optionTexts[optionID] = optionText;

                    answerAggregator.AddItemOption(itemID, optionID, optionTexts[optionID]);
                }
            }

            long answerIdSeed = 1000;

            //Now load the answer data into the aggregator
            foreach (int itemID in SourceItemIds)
            {
                List<ItemAnswer> itemAnswers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);

                foreach (ItemAnswer itemAnswer in itemAnswers)
                {
                    if (itemAnswer.AnswerId > answerIdSeed)
                    {
                        answerIdSeed = itemAnswer.AnswerId;
                    }

                    if (itemAnswer.OptionId.HasValue)
                    {
                        //Get the option text from the existing dictionary if it is in it
                        string optionText = optionTexts.ContainsKey(itemAnswer.OptionId.Value)
                                                ? optionTexts[itemAnswer.OptionId.Value]
                                                : GetOptionText(itemID, itemAnswer.OptionId.Value);

                        answerAggregator.AddAnswer(
                            itemAnswer.AnswerId,
                            itemAnswer.ResponseId,
                            null,
                            itemID,
                            itemAnswer.OptionId,
                            optionText,
                            itemAnswer.Points,
                            itemAnswer.PageId);
                    }

                }

                //Store response count
                ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);

                answerIdSeed++;
            }

            var result = calculator.GetData(answerAggregator);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_RankOrderSummary");
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
            command.AddInParameter("StartDate", DbType.DateTime, SourceAnalysisTemplateId.HasValue ? Report.FilterStartDate : null);
            command.AddInParameter("EndDate", DbType.DateTime, SourceAnalysisTemplateId.HasValue ? Report.FilterEndDate : null);

            //get options in the right order
            List<ItemAnswer> answers =
                GetItemOptionIdsForReport(itemId).
                Select(o => new ItemAnswer { OptionId = o, ItemId = itemId, Count = -1 }).ToList();

            bool showDataLabelZeroValues =
                ((AnalysisItemAppearanceData)AppearanceDataManager.GetAppearanceDataForItem(ID)).ShowDataLabelZeroValues;

            //execute the function))
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
                        int optionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1);
                        int count = DbUtility.GetValueFromDataReader(reader, "answercount", 0);
                        double points = DbUtility.GetValueFromDataReader<double>(reader, "points", 0.0);

                        var answer = answers.FirstOrDefault(a => a.OptionId == optionId);
                        if (answer != null)
                        {
                            answer.Count = count;
                            answer.Points = points;
                        }
                    }

                    //remove unused options
                    for (int i = 0; i < answers.Count; i++)
                    {
                        if (answers[i].Count == -1 || (!showDataLabelZeroValues && answers[i].Count == 0))
                            answers.RemoveAt(i--);
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        protected override AggregateResult[] AggregateResult(IEnumerable<ItemAnswer> answers)
        {
            return (from answer in answers
                    where !answer.IsOther
                    let totalPoints = answers.Where(o => o.ItemId == answer.ItemId).Sum(o => o.Points ?? 0)
                    select new AggregateResult
                    {
                        ResultKey = answer.ItemId + "_" + answer.OptionId + answer.AnswerText,
                        AnswerCount = answer.Count,
                        AnswerPercent = (answer.Points ?? 0) * 100d / totalPoints,
                        ResultText = answer.OptionId.HasValue && answer.OptionId != -1 ?
                            GetOptionText(answer.ItemId, answer.OptionId.Value) : answer.AnswerText,
                        Points = answer.Points ?? 0

                    }).ToArray();
        }

    }
}
