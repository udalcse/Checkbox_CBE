using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Forms.Data;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Analytics.Computation;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Common;
using Checkbox.Management;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Business logic for collecting and displaying analysis details
    /// </summary>
    [Serializable]
    public class DetailsItem : AnalysisItem
    {
        /// <summary>v
        /// Configure the item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            GroupAnswers = ((DetailsItemData)itemData).GroupAnswers;
            LinkToResponseDetails = ((DetailsItemData)itemData).LinkToResponseDetails;
        }

        /// <summary>
        /// Get whether to link to response details
        /// </summary>
        public bool LinkToResponseDetails { get; private set; }

        /// <summary>
        /// Get whether to group answers
        /// </summary>
        public bool GroupAnswers { get; private set; }

        /// <summary>
        /// Process data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return ProcessData(false);
        }

        /// <summary>
        /// Generate preview data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return ProcessData(true);
        }

        /// <summary>
        /// Aggregate the data and return the result
        /// </summary>
        /// <returns>DataSet containing answer data.</returns>
        protected AnalysisItemResult ProcessData(bool isPreview)
        {
            List<DetailResult> aggregatedAnswers = AggregateAnswers(isPreview);

            //Set response counts
            foreach (var sourceItemId in SourceItemIds)
            {
                ResponseCounts[sourceItemId] =
                    aggregatedAnswers
                        .Where(result => result.ItemId == sourceItemId)
                        .Select(result => result.ResponseId)
                        .Distinct()
                        .Count();
            }

            var analysisItemResult = GroupAnswers
                                            ? new AnalysisItemResult
                                                {
                                                    GroupedDetailResults =
                                                        GroupAggregatedAnswers(aggregatedAnswers).ToArray()
                                                }
                                            : new AnalysisItemResult {DetailResults = aggregatedAnswers.ToArray()};

            foreach (int sourceItemId in SourceItemIds)
            {
                analysisItemResult.ItemResponseCounts[sourceItemId] = ResponseCounts[sourceItemId];
            }

            return analysisItemResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            const string SEPARATOR = ", #!%";
            const string SPECIAL_SYMBOL = "#!%";

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_Details");
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
            command.AddInParameter("Separator", DbType.String, SEPARATOR);
            command.AddInParameter("LanguageCode", DbType.String, LanguageCode);
            command.AddInParameter("UseAliases", DbType.Byte, UseAliases);

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
                        int responseId = DbUtility.GetValueFromDataReader(reader, "responseid", -1);
                        string details = DbUtility.GetValueFromDataReader(reader, "details", string.Empty);
                        var parts = details.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Replace(SPECIAL_SYMBOL, string.Empty)).Where(p => !string.IsNullOrEmpty(p));

                        answers.Add(
                            new ItemAnswer
                            {
                                ResponseId = responseId,
                                AnswerText = string.Join(", ", parts)
                            });

                        if (!GroupAnswers && parts.Count() > 1)
                        {
                            for (int i = 1; i < parts.Count(); i++)
                            {
                                answers.Add(
                                    new ItemAnswer
                                    {
                                        ResponseId = responseId,
                                        ItemId = itemId,
                                        AnswerText = parts.ElementAt(i)
                                    });
                            }
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

            //process answers
            return new AnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                DetailResults = answers.Select(answer => new DetailResult
                                                             {
                                                                 ItemId = answer.ItemId, 
                                                                 ResponseId = answer.ResponseId, 
                                                                 ResultText = answer.AnswerText
                                                             
                                                             }).ToArray()
            };
        }

        /// <summary>
        /// Now, group the answers by response id.  A column will be created for each item in the response.
        /// </summary>
        /// <param name="aggregatedAnswers">Aggregated answer data.</param>
        /// <returns>Datatable with information.</returns>
        protected virtual List<GroupedResult<DetailResult>> GroupAggregatedAnswers(List<DetailResult> aggregatedAnswers)
        {
            //Group by response id
            var groupedDetails = aggregatedAnswers.GroupBy(result => result.ResponseId);

            return groupedDetails.Select(groupedDetail => new GroupedResult<DetailResult>
            {
                GroupKey = groupedDetail.Key.ToString(),
                GroupResults = groupedDetail.OrderBy(detail => detail.ResultIndex).ToArray()
            }).ToList();
        }

        /// <summary>
        /// Aggregate answer data and return a data table containing the results
        /// </summary>
        /// <param name="isPreview"></param>
        /// <returns>DataTable with aggregated answer data</returns>
        protected virtual List<DetailResult> AggregateAnswers(bool isPreview)
        {
            var answerAggregator = new DetailsItemAnswerAggregator(UseAliases);
            
            long answerIdSeed = 1000;

            foreach (int itemID in SourceItemIds)
            {
                //Add the item and it's options (if any) to the aggregator
                answerAggregator.AddItem(
                   itemID,
                   GetItemText(itemID),
                   GetSourceItemTypeName(itemID));

                List<int> itemOptionIDs = isPreview
                    ? GetItemOptionIdsForPreview(itemID)
                    : GetItemOptionIdsForReport(itemID);

                foreach (int itemOptionID in itemOptionIDs)
                {
                    answerAggregator.AddItemOption(itemID, itemOptionID, GetOptionText(itemID, itemOptionID));
                }

                //Now add the response answers for the item
                List<ItemAnswer> answers = isPreview ? GetItemPreviewAnswers(itemID, answerIdSeed, 1000) : GetItemAnswers(itemID);
                var groupedByResponse = answers.GroupBy(a => a.ResponseId);

                foreach (var responseAnswers in groupedByResponse)
                {
                    if (!responseAnswers.Any())
                        continue;

                    string answerText = string.Empty;
                    ItemAnswer answer;
                    
                    for (int i=0; ; )
                    {
                        answer = responseAnswers.ElementAt(i);

                        if (answer.AnswerId > answerIdSeed)
                            answerIdSeed = answer.AnswerId;
                        
                        if (answer.OptionId.HasValue && !answer.IsOther)
                            answerText += GetOptionText(itemID, answer.OptionId.Value);
                        else
                            answerText += answer.AnswerText;
                        
                        answerIdSeed++;

                        if (++i < responseAnswers.Count())
                            answerText += ", ";
                        else
                            break;
                    }

                    if (Utilities.IsNotNullOrEmpty(answerText))
                    {
                        answerAggregator.AddAnswer(
                            answer.AnswerId,
                            answer.ResponseId,
                            answer.ResponseGuid,
                            itemID,
                            answer.OptionId,
                            answerText);
                    }
                }

                //Store response count
                ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);
            }
         
            return answerAggregator.GetAggregatedAnswerData();
        }

        /// <summary>
        /// Get preview answers for the item
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="answerIdSeed"></param>
        /// <param name="responseIdSeed"></param>
        /// <returns></returns>
        protected override List<ItemAnswer> GetItemPreviewAnswers(int itemID, long? answerIdSeed, long? responseIdSeed)
        {
            var answers = new List<ItemAnswer>();

            List<int> optionIds = GetItemOptionIdsForPreview(itemID);

            long answerId = answerIdSeed ?? 1000;
            long responseId = responseIdSeed ?? 1000;

            if (optionIds.Count > 0)
            {
                //Generate a random answer for the option
                var random = new Random();

                int optionIndex = random.Next(0, optionIds.Count - 1);

                LightweightOptionMetaData optionMetaData = SurveyMetaDataProxy.GetOptionData(optionIds[optionIndex], itemID, true);

                if(optionMetaData != null)
                {
                    string answerText = optionMetaData.IsOther
                        ? TextManager.GetText("/controlText/analysisItem/other", LanguageCode, "Other") +" 1"
                        : optionMetaData.GetText(Utilities.IsNotNullOrEmpty(optionMetaData.Alias), LanguageCode);

                    answers.Add(new ItemAnswer
                    {
                        AnswerId = answerId,
                        ResponseId = responseId,
                        ItemId = itemID,
                        OptionId = optionIds[optionIndex],
                        IsOther = optionMetaData.IsOther,
                        AnswerText = answerText
                    });
                }
            }
            else
            {
                answers.Add(new ItemAnswer
                {
                    AnswerId = answerId,
                    ResponseId = responseId,
                    ItemId = itemID,
                    AnswerText = TextManager.GetText("/controlText/analysisItem/sample1", LanguageCode, "Sample 1")
                });
            }

            return answers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var values = base.GetMetaDataValuesForSerialization();

            values["LinkToResponseDetails"] = LinkToResponseDetails.ToString();
            values["GroupAnswers"] = GroupAnswers.ToString();
            
            return values;
        }
    }
}
