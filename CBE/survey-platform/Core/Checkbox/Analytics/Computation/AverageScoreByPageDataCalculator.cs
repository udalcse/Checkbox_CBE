using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// 
    /// </summary>
    public class AverageScoreByPageDataCalculator : ReportDataCalculator
    {
        private readonly ResponseTemplate _responseTemplate;
        private readonly bool _includeTotalSurveyScore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseTemplate"></param>
        /// <param name="includeTotalSurveyScore"></param>
        public AverageScoreByPageDataCalculator(ResponseTemplate responseTemplate, bool includeTotalSurveyScore)
        {
            _responseTemplate = responseTemplate;
            _includeTotalSurveyScore = includeTotalSurveyScore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerAggregator"></param>
        /// <returns></returns>
        protected override AggregateResult[] Aggregate(ItemAnswerAggregator answerAggregator)
        {
            var result = new List<AggregateResult>();

            var totalScore = 0d;
            var groupedByPage = answerAggregator.GetAggregatedAnswerData().GroupBy(answer => answer.PageId);
            int responseCount = answerAggregator.GetResponseCount(null);
            foreach (var pageGroup in groupedByPage)
            {
                var groupedByItem = pageGroup.GroupBy(answer => answer.ItemId);
                var pageScoreSum = groupedByItem.Sum(groupedAnswer => groupedAnswer.Where(detail => !detail.IsAnswerOther).Sum(detail => detail.AnswerScore));
                totalScore += pageScoreSum;

                if (pageGroup.Key == null)
                    continue;

                var page = _responseTemplate.GetPage(pageGroup.Key.Value);
                result.Add(new AggregateResult
                {
                    ResultKey = pageGroup.Key.ToString(),
                    ResultText = TextManager.GetText("/controlText/averageScoreByPageItem/pageSource") + " " + (page.Position - 1),
                    AnswerPercent = responseCount > 0 ? pageScoreSum / responseCount : 0
                });
            }

            if (_includeTotalSurveyScore)
            {
                result.Add(new AggregateResult
                {
                    ResultKey = "-1",
                    ResultText = TextManager.GetText("/controlText/averageScoreByPageItem/totalSurveySource"),
                    AnswerPercent = responseCount > 0 ? totalScore / responseCount : 0
                });
            }

            return result.ToArray();
        }
    }
}
