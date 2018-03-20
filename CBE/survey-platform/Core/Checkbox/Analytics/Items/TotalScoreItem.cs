using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TotalScoreItem : AnalysisItem
    {
        /// <summary>
        /// Process the analysis data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult ProcessData()
        {
            return ProcessData(false);
        }

        /// <summary>
        /// Generate report preview data
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult GeneratePreviewData()
        {
            return ProcessData(true);
        }

        /// <summary>
        /// Process data
        /// </summary>
        /// <param name="isPreview"></param>
        /// <returns></returns>
        protected virtual AnalysisItemResult ProcessData(bool isPreview)
        {
            var processedData = new FrequencyAnalysisData();

            long answerIdSeed = 1000;

            foreach (Int32 itemID in SourceItemIds)
            {
                if (!isPreview)
                {
                    List<ItemAnswer> answers = GetItemAnswers(itemID);

                    foreach (ItemAnswer answer in answers)
                    {
                        if (answer.AnswerId > answerIdSeed)
                        {
                            answerIdSeed = answer.AnswerId;
                        }

                        if (answer.OptionId.HasValue)
                        {
                            processedData.Increment(GetOptionPoints(itemID, answer.OptionId.Value).ToString());
                        }
                    }
                }
                else
                {
                    //Make a nice curve
                    for (int i = 1; i < 100; i = i + 20)
                    {
                        double angle = (((2 * Math.PI) / 100) * i) - (Math.PI / 2);
                        double sineResult = Math.Sin(angle);

                        Int32 numResponses = Convert.ToInt32((100 * sineResult) + 100);

                        for (int j = 0; j < numResponses; j++)
                        {
                            processedData.Increment(i.ToString());
                        }
                    }

                }

                //Store response count
                ResponseCounts[itemID] = processedData.ResponseCount;

                answerIdSeed++;
            }

            //Set responseCounts array
            var result = processedData.ResultsData;
            foreach (Int32 itemID in SourceItemIds)
            {
                result.ItemResponseCounts[itemID] = ResponseCounts[itemID];
            }

            return result;
        }
    }
}
