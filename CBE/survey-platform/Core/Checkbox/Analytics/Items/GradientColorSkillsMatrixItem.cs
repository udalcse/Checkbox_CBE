using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics.Data;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items;
using Checkbox.Security;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Items
{
    public class GradientColorSkillsMatrixItem : AnalysisItem
    {

        /// <summary>
        /// The not answered question value
        /// </summary>
        private const int NotAnsweredQuestionValue = 1;

        /// <summary>
        /// The not answered question value
        /// </summary>
        private const int DefaultRatingScaleColumn = 2;

        /// <summary>
        /// The maximum source item elements in source item data
        /// </summary>
        private const int MaxSourceItemElements = 1;
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
            AnalysisItemResult result = AggregatedData();
            result.IsPreview = true;
            return result;
        }

        protected override AnalysisItemResult AggregatedData()
        {
            AnalysisItemResult result = new AnalysisItemResult
            {
                AggregateResults = AggregateResult(new List<ItemAnswer>())
            };

            var gradientMatrixResult = new GradientColorDirectorMatrixResult();

            if (this.TemplateID.HasValue && this.SourceItemIds.Any())
            {
                if (this.SourceItemIds.Count > MaxSourceItemElements )
                    throw new ArgumentException("Source item elements can not be greater than 1");

                var matrixId = this.SourceItemIds.FirstOrDefault();

                LightweightItemMetaData matrixItemData =
                    SurveyMetaDataProxy.GetItemData(matrixId, TemplatesValidated);

                this.SourceItemIds.Clear();

                foreach (var id in matrixItemData.Children)
                    this.SourceItemIds.Add(id);

                var analysisData = this.GetAnalysisData(false, false, true);
                var responseProperties = analysisData.GetAllResponseProperties();
                var responseTemplate = ResponseTemplateManager.GetResponseTemplate(this.TemplateID.Value);
            
                var responsePropertiesList = responseProperties.Select(pair => pair.Value).ToList();
                var filteredResponses =
                    responsePropertiesList.GroupBy(item => item.GetStringValue("UniqueIdentifier"))
                        .ToList()
                        .Select(respondent => respondent.OrderBy(item => item.GetStringValue("UniqueIdentifier")).Last());

                foreach (var response in filteredResponses)
                {
                    GradientColorMatrixRespondent respondent = new GradientColorMatrixRespondent()
                    {
                        RespondentName = response["UniqueIdentifier"].ToString(),
                        ResponseId = int.Parse(response["ResponseID"].ToString())
                    };

                    var responseAnswers = analysisData.GetResponseAnswers(respondent.ResponseId);

                    List<int> optionIds = GetMatrixOptionIds(matrixItemData);

                    List<double> answers = new List<double>();

                    foreach (var optionId in optionIds)
                    {

                        //var responseAnswerValue = analysisData.GetOptionAnswer(respondent.ResponseId, )
                        var answer = responseAnswers.FirstOrDefault(item => item.ItemId.Equals(optionId));
                        if (answer != null)
                            answers.Add(answer.Points ?? NotAnsweredQuestionValue);
                        else
                        {
                            answers.Add(NotAnsweredQuestionValue);
                        }
                    }

                    respondent.AnswerScore.AddRange(answers.Select(item => item));
                    respondent.AverageByDirector = Math.Round(respondent.AnswerScore.Sum(item => item)/respondent.AnswerScore.Count,2);

                    gradientMatrixResult.Respondents.Add(respondent);
                }

                var columnPrototypeId = matrixItemData.GetColumnPrototypeID(DefaultRatingScaleColumn);
                var responseItem = SurveyMetaDataProxy.GetItemData(columnPrototypeId.Value, TemplatesValidated);
                gradientMatrixResult.MaxScaleRatingRange = this.GetOptionPoints(matrixId, responseItem.Options.Last());

                foreach (var key in matrixItemData.RowKeyItems.Keys)
                {
                    var itemData = SurveyMetaDataProxy.GetItemData(matrixItemData.RowKeyItems[key], TemplatesValidated);
                    gradientMatrixResult.Options.Add(new GradientColorMatrixOption()
                    {
                        OptionTitle = itemData.GetText(this.LanguageCode)
                    });
                }

                for (int i = 0; i < gradientMatrixResult.Options.Count; i++)
                {
                    double sum = gradientMatrixResult.Respondents.Sum(respondent => respondent.AnswerScore[i]);

                    gradientMatrixResult.Options[i].AverageScore = Math.Round(sum /gradientMatrixResult.Respondents.Count,2);
                }

                gradientMatrixResult.OptionAverage = Math.Round(gradientMatrixResult.Options.Sum(item => item.AverageScore)/gradientMatrixResult.Options.Count,2);

            }

            result.GradientColorDirectorMatrixResult = gradientMatrixResult;
            result.IsPreview = false;

            return result;
        }

       
        private List<int> GetMatrixOptionIds(LightweightItemMetaData matrix)
        {
            List<int> result = new List<int>();
            foreach (var id in matrix.Children)
            {
                var coordinate = matrix.GetChildCoordinate(id);
                if (coordinate.X == DefaultRatingScaleColumn)
                    result.Add(id);

            }

            return result;
        }

        protected override AggregateResult[] AggregateResult(IEnumerable<ItemAnswer> answers)
        {
            return answers.Select(answer => new AggregateResult
            {
                ResultKey = answer.ItemId.ToString(),
                ResultText = answer.AnswerText
            }).ToArray();
        }
    }
}
