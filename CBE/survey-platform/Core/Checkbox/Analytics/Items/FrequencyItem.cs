using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Frequency analysis
    /// </summary>
    [Serializable]
    public class FrequencyItem : AnalysisItem
    {
        /// <summary>
        /// Get/set how to handle "other" options when reporting on select items.
        /// </summary>
        public virtual OtherOption OtherOption { get; set; }

        /// <summary>
        /// Get/set primary source item id
        /// </summary>
        public virtual int? PrimarySourceItemID { get; set; }

        /// <summary>
        /// Show statistics table below the chart or not
        /// </summary>
        public virtual bool DisplayStatistics { get; set; }

        /// <summary>
        /// Show summary answers table below the chart or not
        /// </summary>
        public virtual bool DisplayAnswers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            if (itemData is FrequencyItemData)
            {
                OtherOption = (int)((FrequencyItemData)itemData).OtherOption < 1 ? OtherOption.Aggregate : ((FrequencyItemData)itemData).OtherOption;
                PrimarySourceItemID = ((FrequencyItemData)itemData).PrimarySourceItemID;
                DisplayAnswers = ((FrequencyItemData)itemData).DisplayAnswers;
                DisplayStatistics = ((FrequencyItemData)itemData).DisplayStatistics;
            }
        }

        /// <summary>
        /// Process the answer data and calculate frequencies
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
        /// Process data
        /// </summary>
        /// <returns></returns>
        protected virtual AnalysisItemResult ProcessPreviewData()
        {
            //Build the list of items & options
            var answerAggregator = new ItemAnswerAggregator(UseAliases);
            var calculator = new FrequencyDataCalculator();

            var otherAnswers = new List<DetailResult>();

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
                //List<ItemAnswer> itemAnswers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);
                List<ItemAnswer> itemAnswers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);

                foreach (ItemAnswer itemAnswer in itemAnswers)
                {
                    if (itemAnswer.AnswerId > answerIdSeed)
                    {
                        answerIdSeed = itemAnswer.AnswerId;
                    }

                    if (itemAnswer.OptionId.HasValue)
                    {
                        //Depending on the "OtherOption" we'll want to add a generic "other" text or
                        //the actual text entered by the respondent
                        if (itemAnswer.IsOther)
                        {
                            if (OtherOption == OtherOption.AggregateAndDisplay
                                || OtherOption == OtherOption.Aggregate)
                            {
                                answerAggregator.AddAnswer(
                                    itemAnswer.AnswerId,
                                    itemAnswer.ResponseId,
                                    itemID,
                                    itemAnswer.OptionId,
                                    OtherOption == OtherOption.Display
                                        ? itemAnswer.AnswerText
                                        : GetText("/controlText/analysisItem/other"));
                            }

                            if (OtherOption == OtherOption.AggregateAndDisplay
                                || OtherOption == OtherOption.Display)
                            {
                                otherAnswers.Add(new DetailResult
                                                        {
                                                            ResponseId = itemAnswer.ResponseId,
                                                            ItemId = itemAnswer.ItemId,
                                                            OptionId = itemAnswer.OptionId,
                                                            ResultText = itemAnswer.AnswerText
                                                        });
                            }
                        }
                        else
                        {
                            //Get the option text from the existing dictionary if it is in it
                            string optionText = optionTexts.ContainsKey(itemAnswer.OptionId.Value)
                                                    ? optionTexts[itemAnswer.OptionId.Value]
                                                    : GetOptionText(itemID, itemAnswer.OptionId.Value);

                            answerAggregator.AddAnswer(
                                itemAnswer.AnswerId,
                                itemAnswer.ResponseId,
                                itemID,
                                itemAnswer.OptionId,
                                optionText);
                        }
                    }
                    else
                    {
                        if (Utilities.IsNotNullOrEmpty(itemAnswer.AnswerText))
                        {
                            answerAggregator.AddAnswer(
                                itemAnswer.AnswerId,
                                itemAnswer.ResponseId,
                                itemID,
                                itemAnswer.AnswerText);
                        }
                    }
                }

                //Store response count
                ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);

                answerIdSeed++;
            }

            var result = calculator.GetData(answerAggregator);
            result.DetailResults = otherAnswers.ToArray();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ItemAnswer> RetrieveData(int itemId, out int responseCount)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_Frequency");
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
                Select(o => new ItemAnswer {OptionId = o, ItemId = itemId, Count = -1}).ToList();

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
                        bool isother = DbUtility.GetValueFromDataReader(reader, "isother", false);
                        int optionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1);
                        int count = DbUtility.GetValueFromDataReader(reader, "answercount", 0);

                        if(optionId != -1 && (!isother || OtherOption == OtherOption.Aggregate || OtherOption == OtherOption.AggregateAndDisplay))
                        {
                            var answer = answers.FirstOrDefault(a => a.OptionId == optionId);
                            if(answer != null)
                                answer.Count = count;
                        }
                    }

                    //remove unused options
                    for (int i = 0; i < answers.Count; i++)
                    {
                        if (answers[i].Count == -1 || (!showDataLabelZeroValues && answers[i].Count == 0))
                            answers.RemoveAt(i--);
                    }

                    //read other options data
                    reader.NextResult();
                    if (OtherOption == OtherOption.AggregateAndDisplay || OtherOption == OtherOption.Display)
                    {
                        while (reader.Read())
                        {
                            answers.Add(new ItemAnswer
                                            {
                                                ItemId = itemId,
                                                OptionId = DbUtility.GetValueFromDataReader(reader, "optionid", -1),
                                                IsOther = true,
                                                AnswerText = DbUtility.GetValueFromDataReader(reader, "answertext", string.Empty)
                                            });
                        }
                    }

                    //if no option data was retrieved, there are open-ended item 
                    reader.NextResult();
                    if (answers.Count == 0)
                    {
                        while (reader.Read())
                        {
                            string text = DbUtility.GetValueFromDataReader(reader, "answertext", string.Empty);
                            if(string.IsNullOrEmpty(text))
                                continue;

                            ItemAnswer ItemAnswer;
                            if ((ItemAnswer = answers.FirstOrDefault(a => a.ItemId == itemId &&
                                a.AnswerText.Equals(text, StringComparison.InvariantCultureIgnoreCase))) != null)
                                ItemAnswer.Count++;
                            else
                                answers.Add(new ItemAnswer { ItemId = itemId, AnswerText = text, Count = 1, OptionId = -1 });
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
            List<DetailResult> details = new List<DetailResult>();
            foreach (var answer in answers)
            {
                if (!itemAnswerCounts.ContainsKey(answer.ItemId))
                    itemAnswerCounts.Add(answer.ItemId, answers.Where(o => o.ItemId == answer.ItemId).Sum(o => o.Count));

                //add "other" options if not exist
                if (answer.IsOther && (OtherOption == OtherOption.AggregateAndDisplay || OtherOption == OtherOption.Display))
                {
                    details.Add(new DetailResult
                    {
                        IsAnswerOther = true,
                        ItemId = answer.ItemId,
                        OptionId = answer.OptionId,
                        ResultText = answer.AnswerText
                    });
                }
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
        /// <param name="answers"></param>
        /// <returns></returns>
        protected override AggregateResult[] AggregateResult(IEnumerable<ItemAnswer> answers)
        {
            return (from answer in answers
                    where !answer.IsOther
                    let totalAnswerCount = answers.Where(o => o.ItemId == answer.ItemId).Sum(o => o.Count)
                    select new AggregateResult
                               {
                                   ResultKey = answer.ItemId + "_" + answer.OptionId + answer.AnswerText,
                                   AnswerCount = answer.Count,
                                   AnswerPercent = answer.Count*100d/totalAnswerCount,
                                   ResultText = answer.OptionId.HasValue && answer.OptionId != -1 ? 
                                       Utilities.AdvancedHtmlDecode(GetOptionText(answer.ItemId, answer.OptionId.Value)) : answer.AnswerText
                               
                               }).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override NameValueCollection  GetMetaDataValuesForSerialization()
        {
            var values = base.GetMetaDataValuesForSerialization();

            values["OtherOption"] = OtherOption.ToString();
            values["PrimarySourceItemID"] = PrimarySourceItemID == null ? "" : PrimarySourceItemID.ToString();
            values["DisplayStatistics"] = DisplayStatistics.ToString();
            values["DisplayAnswers"] = DisplayAnswers.ToString();

            return values;
        }
    }
}
