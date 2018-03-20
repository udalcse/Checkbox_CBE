using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AverageScoreByPageItem : AverageScoreItem
    {
        /// <summary>
        /// Get list of source page ids
        /// </summary>
        public List<int> SourcePageIds { get; protected set; }

        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            SourcePageIds = new List<int>(((AverageScoreByPageItemData)itemData).SourcePageIds);
        }

        protected override void PopulateSourceItems(ReportItemInstanceData itemDto)
        {
            base.PopulateSourceItems(itemDto);

            var sourceData = new List<ReportItemSourcePageData>();

            var rt = ResponseTemplateManager.GetResponseTemplate(SourceResponseTemplateId);
            if (IncludeSourceDataInDto && rt != null)
            {
                foreach (var pageId in SourcePageIds)
                {
                    var sourcePageData = GetSourcePage(pageId, rt);

                    if (sourcePageData != null)
                        sourceData.Add(sourcePageData);
                }
            }

            if (ScoreOption == AverageScoreCalculation.PageAveragesWithTotalScore)
            {
                sourceData.Add(new ReportItemSourcePageData
                               {
                                   ReportingText = TextManager.GetText("/controlText/averageScoreByPageItem/totalSurveySource")
                               });
            }

            itemDto.SourcePages = sourceData.ToArray();
        }

        /// <summary>
        /// Get source page data for report item
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        protected virtual ReportItemSourcePageData GetSourcePage(int pageId, ResponseTemplate rt)
        {
            var page = rt.GetPage(pageId);

            if (page == null)
                return null;

            return new ReportItemSourcePageData
            {
                PageId = page.ID,
                Position = page.Position,
                ReportingText = TextManager.GetText("/controlText/averageScoreByPageItem/pageSource") + " " + (page.Position - 1)
            };
        }

        /// <summary>
        /// Process the data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessPreviewData()
        {
            var answerAggregator = new ItemAnswerAggregator(UseAliases);
            var rt = ResponseTemplateManager.GetResponseTemplate(SourceResponseTemplateId);
            var calculator = new AverageScoreByPageDataCalculator(rt, ScoreOption == AverageScoreCalculation.PageAveragesWithTotalScore);

            long answerIdSeed = 1000;
            //skip the first page as it is hidden
            for (int i = 2; i < rt.PageCount; i++)
            {
                var page = rt.GetPageAtPosition(i);
                if (page != null)
                {
                    //if the page is not a source page, include as NULL for response count calculation and total survey score
                    int? groupPageId = SourcePageIds.Contains(page.ID.Value) ? page.ID : null;

                    foreach (int itemID in page.ListItemIds())
                    {
                        var item = rt.GetItem(itemID, false);
                        if (item.ItemIsIScored)
                        {
                            var matrixItemData = item as MatrixItemData;
                            if (matrixItemData != null)
                            {
                                foreach (var childItemDataID in matrixItemData.GetChildItemDataIDs())
                                {
                                    item = rt.GetItem(childItemDataID, false);
                                    if (item.ItemIsIScored)
                                        answerIdSeed = HandleItem(answerAggregator, childItemDataID, groupPageId, answerIdSeed);
                                }
                            }
                            else
                            {
                                answerIdSeed = HandleItem(answerAggregator, itemID, groupPageId, answerIdSeed);
                            }
                        }
                    }
                }

            }
            return calculator.GetData(answerAggregator);
        }

        /// <summary>
        /// 
        /// </summary>
        private long HandleItem(ItemAnswerAggregator answerAggregator, int itemID, int? pageId, long answerIdSeed)
        {
            //Add the possibles
            var optionIDs = GetItemOptionIdsForPreview(itemID);

            answerAggregator.AddItem(
                itemID,
                GetItemText(itemID),
                GetSourceItemTypeName(itemID));

            foreach (int optionID in optionIDs)
            {
                answerAggregator.AddItemOption(itemID, optionID, GetOptionText(itemID, optionID), GetOptionPoints(itemID, optionID), GetOptionIsOther(itemID, optionID));
            }

            //Add the answers
            var answers = GetItemPreviewAnswers(itemID, answerIdSeed, 1000);
            foreach (ItemAnswer answer in answers)
            {
                if (answer.AnswerId > answerIdSeed)
                    answerIdSeed = answer.AnswerId;

                answerAggregator.AddAnswerWithPointsAndPage(answer.AnswerId, answer.ResponseId, itemID, answer.OptionId, answer.Points, pageId);
            }

            //Store response count
            ResponseCounts[itemID] = answerAggregator.GetResponseCount(itemID);
            answerIdSeed++;

            return answerIdSeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<ItemAnswer> RetrieveData(List<int> itemIds)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetItemAnswerData_AverageScore");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, SourceResponseTemplateId);
            command.AddInParameter("ItemIDString", DbType.String, string.Join(",", itemIds));
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

                    //read options data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        answers.Add(
                            new ItemAnswer
                            {
                                ItemId = DbUtility.GetValueFromDataReader(reader, "itemid", 0),
                                Points = DbUtility.GetValueFromDataReader(reader, "points", 0)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult AggregatedData()
        {
            List<ItemAnswer> answers = new List<ItemAnswer>();

            var itemResponseCounts = new Dictionary<int, int>();
            var itemAnswerCounts = new Dictionary<int, int>();

            var includeTotalSurvey = ScoreOption == AverageScoreCalculation.PageAveragesWithTotalScore;
            var rt = ResponseTemplateManager.GetResponseTemplate(SourceResponseTemplateId);
            var totalResponseCount = ResponseManager.GetResponseCount(SourceResponseTemplateId, Report.IncludeIncompleteResponses, Report.IncludeTestResponses);
            var pageItems = new Dictionary<int, List<int>>();

            var scorableItemIds = new List<int>();
            //skip the first page as it is hidden
            for (int i = 2; i < rt.PageCount; i++)
            {
                var page = rt.GetPageAtPosition(i);
                if (page == null || (!includeTotalSurvey && !SourcePageIds.Contains(page.ID.Value)))
                    continue;

                pageItems.Add(page.ID.Value, page.ListItemIds().ToList());

                foreach (int itemID in page.ListItemIds())
                {
                    var item = rt.GetItem(itemID, false);
                    if (item.ItemIsIScored)
                    {
                        var matrixItemData = item as MatrixItemData;
                        if (matrixItemData != null)
                        {
                            foreach (var childItemDataID in matrixItemData.GetChildItemDataIDs())
                            {
                                item = rt.GetItem(childItemDataID, false);
                                if (item.ItemIsIScored)
                                {
                                    scorableItemIds.Add(childItemDataID);
                                    pageItems[page.ID.Value].Add(childItemDataID);
                                }
                            }
                        }
                        else
                        {
                            scorableItemIds.Add(itemID);
                        }
                    }
                }
            }

            //fill pageId data and totalResponseCounts
            answers.AddRange(RetrieveData(scorableItemIds));
            foreach (var answer in answers)
            {
                answer.PageId = pageItems.Where(p => p.Value.Contains(answer.ItemId)).Select(p => p.Key).FirstOrDefault();
            }

            //group by page
            var groupedByPage = answers.GroupBy(a => a.PageId).ToList();
            answers.Clear();
            var totalSum = 0d;
            foreach (var pageGroup in groupedByPage)
            {
                var sum = pageGroup.Sum(a => a.Points);
                if (SourcePageIds.Contains(pageGroup.Key.Value))
                {
                    answers.Add(new ItemAnswer
                                {
                                    PageId = pageGroup.Key,
                                    Points = sum,
                                    Count = totalResponseCount
                                });
                }
                totalSum += sum.HasValue ? sum.Value : 0d;
            }
            if (includeTotalSurvey)
            {
                answers.Add(new ItemAnswer
                {
                    PageId = null,
                    Points = totalSum,
                    Count = totalResponseCount
                });
            }

            return new AnalysisItemResult
            {
                ItemResponseCounts = itemResponseCounts,
                ItemAnswerCounts = itemAnswerCounts,
                AggregateResults = answers.Select(answer => new AggregateResult
                                        {
                                            ResultKey = answer.PageId.HasValue ? answer.PageId.Value.ToString() : "-1",
                                            ResultText = answer.PageId.HasValue 
                                                ? TextManager.GetText("/controlText/averageScoreByPageItem/pageSource") + " " + (rt.GetPage(answer.PageId.Value).Position - 1)
                                                : TextManager.GetText("/controlText/averageScoreByPageItem/totalSurveySource"),
                                            AnswerCount = answer.Count,
                                            AnswerPercent = answer.Count > 0 && answer.Points.HasValue ? answer.Points.Value / answer.Count : 0
                                        }).ToArray()
            };
        }
    }
}
