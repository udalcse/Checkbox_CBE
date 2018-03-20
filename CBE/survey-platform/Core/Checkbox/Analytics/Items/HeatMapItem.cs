using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Items
{
    public class HeatMapItem : AnalysisItem
    {
        private static Random _random = new Random();
      
        private bool UseMean { get; set; }
      
        private bool RandomizeResponses { get; set; }

        /// <summary>
        /// The default points value if there no answer
        /// </summary>
        private const int DefaultPointsValue = 0;

        /// <summary>
        /// The default points value if there no answer
        /// </summary>
        private const int DefaultRaitingRange = 10;

        /// <summary>
        /// Configure the item with it's meta-data, language code for displaying text, and no filters.
        /// </summary>
        /// <param name="itemData">AnalysisItem's configuration information.</param>
        /// <param name="languageCode">Language code to use for fetching text.</param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);
            var heatMapData = itemData as HeatMapData;

            if (heatMapData != null)
            {
                UseMean = heatMapData.UseMeanValues;
                RandomizeResponses = heatMapData.RandomizeResponses;
            }
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
            var result =  AggregatedData();
            result.IsPreview = true;

            return result;
        }

        protected override AnalysisItemResult AggregatedData()
        {

            AnalysisItemResult result = new AnalysisItemResult
            {
                AggregateResults = AggregateResult(new List<ItemAnswer>()),
                HeatMapAnalysisResult = new HeatMapAnalysisResult()
            };
          
            if (this.TemplateID.HasValue && this.SourceItemIds.Any())
            {
                var analysisData = this.GetAnalysisData(false, false);
                var responseProperties = analysisData.GetAllResponseProperties();

                var responsePropertiesList = responseProperties.Select(pair => pair.Value).ToList();
                var filteredResponses =
                    responsePropertiesList.GroupBy(item => item.GetStringValue("UniqueIdentifier"))
                        .ToList()
                        .Select(respondent => respondent.OrderBy(item => item.GetStringValue("UniqueIdentifier")).Last());

                var templateSections = ResponseTemplateManager.GetSurveySections(this.TemplateID.Value);
                var template = ResponseTemplateManager.GetResponseTemplate(this.TemplateID.Value);
                var sectionsESigmaValues = ResponseTemplateManager.GetSectionsESigmaValues(this.TemplateID.Value, this.ID);

                //take only radio button scale type items
                //FilterSectionItemsByType("RadioButtonScale", template, templateSections);
                FilterSectionsBySourceItems(this.SourceItemIds, templateSections);

                foreach (var response in filteredResponses)
                {
                    HeatMapResult heatMapResult = new HeatMapResult
                    {
                        ResponseId = int.Parse(response["ResponseID"].ToString()),
                        Respondent = response["UniqueIdentifier"].ToString()
                    };

                    var responseAnswers = analysisData.GetResponseAnswers(heatMapResult.ResponseId);

                    foreach (var reportableSection in templateSections)
                    {
                        SurveySection surveySection = new SurveySection {Title = reportableSection.Title, Id = reportableSection.Id};

                        foreach (var itemId in reportableSection.Items)
                        {
                            var itemAnswer = responseAnswers.FirstOrDefault(item => item.ItemId.Equals(itemId));

                            surveySection.Answers.Add(itemAnswer ?? new ItemAnswer() { Points = DefaultPointsValue });
                        }

                        heatMapResult.Sections.Add(surveySection);
                    }

                    result.HeatMapAnalysisResult.Responses.Add(heatMapResult);
                }

                //calculate mean values for sections 
                FillSectionMeanValues(result.HeatMapAnalysisResult, templateSections, sectionsESigmaValues);

                //sort response answers between respondents
                if (this.RandomizeResponses)
                {
                    SortResponseAnswers(result.HeatMapAnalysisResult.Responses);
                }


                result.HeatMapAnalysisResult.MaxRatingRange = GetFirstAnswerRatingScale(result.HeatMapAnalysisResult,
                    template);

            }

            return result;
        }

        /// <summary>
        /// Gets the first answer raiting scale.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public int GetFirstAnswerRatingScale(HeatMapAnalysisResult result, ResponseTemplate template)
        {
            var firstElement = result.Responses.FirstOrDefault();
            if (firstElement != null)
            {
                var section = firstElement.Sections.FirstOrDefault();
                var answer = section?.Answers.FirstOrDefault();

                if (answer != null)
                {
                    var itemData = ((RatingScaleItemData)template.GetItem(answer.ItemId));
                    return itemData.EndValue;
                }
            }

            return DefaultRaitingRange;
        }

        /// <summary>
        /// Filters the sections by source items.
        /// </summary>
        /// <param name="sourceItems">The source items.</param>
        /// <param name="reportableSections">The reportable sections.</param>
        private void FilterSectionsBySourceItems(List<int> sourceItems, List<ReportableSection> reportableSections)
        {
            foreach (var section in reportableSections.ToList())
            {
                var hasMatch = section.Items.Any(x => sourceItems.Any(y => y == x));

                if (!hasMatch)
                {
                    reportableSections.Remove(section);
                }

                section.Items.RemoveAll(sectionItem => !sourceItems.Contains(sectionItem));
            }
        }

        /// <summary>
        /// Sorts the response answers.
        /// </summary>
        /// <param name="responses">The responses.</param>
        private void SortResponseAnswers(List<HeatMapResult> responses)
        {
            foreach (HeatMapResult response in responses)
            {
                for (int j = 0; j < response.Sections.Count; j++)
                {
                    for (int k = 0; k < response.Sections[j].Answers.Count; k++)
                    {
                        var randomValue = _random.Next(0, responses.Count);

                        var tempValue = responses[randomValue].Sections[j].Answers[k];
                        responses[randomValue].Sections[j].Answers[k] = response.Sections[j].Answers[k];
                        response.Sections[j].Answers[k] = tempValue;
                    }
                }
            }
        }

        /// <summary>
        /// Filters the type of the section items types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="template">The template.</param>
        /// <param name="reportableSections">The reportable sections.</param>
        private void FilterSectionItemsByType(string type, ResponseTemplate template, List<ReportableSection> reportableSections )
        {
            foreach (var reportableSection in reportableSections)
            {
                foreach (var item in reportableSection.Items.ToList())
                {
                    var typeName = template.GetItem(item).ItemTypeName;
                    if (!typeName.Equals(type))
                        reportableSection.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Generate mean values for sections or use Sigma dictionary if it is passed
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="reportableSections">The reportable sections.</param>
        /// <param name="eSigma">The e sigma.</param>
        private void FillSectionMeanValues(HeatMapAnalysisResult result, List<ReportableSection> reportableSections, Dictionary<int, double> eSigma)
        {
            foreach (var reportableSection in reportableSections)
            {
                if (!this.UseMean && eSigma.ContainsKey(reportableSection.Id))
                {
                    result.SectionsMeanValues[reportableSection.Id.ToString()] = eSigma[reportableSection.Id];
                }
                else
                {

                    List<ItemAnswer> sectionsAnswers = new List<ItemAnswer>();

                    foreach (var response in result.Responses)
                    {
                        var section = response.Sections.FirstOrDefault(item => item.Id.Equals(reportableSection.Id));

                        if (section != null && section.Answers.Any())
                            sectionsAnswers.AddRange(section.Answers);
                    }

                    result.SectionsMeanValues[reportableSection.Id.ToString()] =
                        (double) (sectionsAnswers.Sum(item => item.Points) / sectionsAnswers.Count);
                }

            }
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
