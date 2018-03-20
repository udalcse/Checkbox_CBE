using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Checkbox.Analytics.Computation;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.SystemMode;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// GovernancePrioritySummaryItem
    /// </summary>
    /// <seealso cref="Checkbox.Analytics.Items.AnalysisItem" />
    [Serializable]
    public class GovernancePrioritySummaryItem : AnalysisItem
    {
   
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
            AnalysisItemResult result = new AnalysisItemResult {CalculateResults = new CalculateResult[10], AggregateResults = AggregateResult(new List<ItemAnswer>())};
           
            result.CalculateResults[0] = new CalculateResult() {ResultKey = "Mission and Values", MaxValue = 10, MinValue = 8,ResultValue = 9.06};
            result.CalculateResults[1] = new CalculateResult() { ResultKey = "Enhics and Accountability", MaxValue = 10, MinValue = 8, ResultValue = 8.39 };
            result.CalculateResults[2] = new CalculateResult() { ResultKey = "Board Composition and Culture", MaxValue = 10, MinValue = 6, ResultValue = 8.52 };
            result.CalculateResults[3] = new CalculateResult() { ResultKey = "Board Meetings and Administration", MaxValue = 10, MinValue = 7, ResultValue = 8.74 };
            result.CalculateResults[4] = new CalculateResult() { ResultKey = "Strategy and Perfomance Measures", MaxValue = 10, MinValue = 8, ResultValue = 9.06 };
            result.CalculateResults[5] = new CalculateResult() { ResultKey = "Board's Relationship to Management", MaxValue = 8, MinValue = 3, ResultValue = 6.30 };
            result.CalculateResults[6] = new CalculateResult() { ResultKey = "Risk, Financial Monitoring and Crisis Control", MaxValue = 10, MinValue = 6, ResultValue = 8.43};
            result.CalculateResults[7] = new CalculateResult() { ResultKey = "Succession Planning and Human Resources", MaxValue = 10, MinValue = 8, ResultValue = 9.03 };
            result.CalculateResults[8] = new CalculateResult() { ResultKey = "Shareholder : Stakeholder Involment", MaxValue = 10, MinValue = 9, ResultValue = 9.63 };
            result.CalculateResults[9] = new CalculateResult() { ResultKey = "Industry-Specific Section", MaxValue = 10, MinValue = 8, ResultValue = 9.30 };

            foreach (CalculateResult calculation in result.CalculateResults)
                calculation.Points = 10;

            return result;
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
        /// Filters the type of the section items types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="template">The template.</param>
        /// <param name="reportableSections">The reportable sections.</param>
        private void FilterSectionItemsByType(string type, ResponseTemplate template, List<ReportableSection> reportableSections)
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
        /// Aggregateds the data2.
        /// </summary>
        /// <returns></returns>
        protected override AnalysisItemResult AggregatedData()
        {
            var calculationResult = new List<CalculateResult>();
            List<ItemAnswer> answers = new List<ItemAnswer>();

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

                //take only radio button scale type items
                //FilterSectionItemsByType("RadioButtonScale", template, templateSections);
                FilterSectionsBySourceItems(this.SourceItemIds, templateSections);


                List<ItemAnswer> allAnswers = new List<ItemAnswer>();

                foreach (var response in filteredResponses)
                    allAnswers.AddRange(analysisData.GetResponseAnswers(int.Parse(response["ResponseID"].ToString())));

                var allItemIds = allAnswers.Select(item => item.ItemId);


                if (allAnswers.Any())
                {
                    foreach (var section in templateSections)
                    {
                        CalculateResult sectionCalculateResult = new CalculateResult { ResultKey = section.Title };

                        var sectionAnswers = allAnswers.Where(item => section.Items.Contains(item.ItemId)).ToList();
                     
                        //remove all not valid answers for governance priority chart 
                        sectionAnswers.RemoveAll(item => item.ResponseId == -1 || !item.Points.HasValue || item.IsOther);

                        if (sectionAnswers.Any())
                        {
                            sectionCalculateResult.MinValue = sectionAnswers.Min(item => item.Points.Value);
                            sectionCalculateResult.MaxValue = sectionAnswers.Max(item => item.Points.Value);

                            var avarageValue = sectionAnswers.Sum(item => item.Points.Value) /
                                                                  sectionAnswers.Count;

                            sectionCalculateResult.ResultValue = Math.Round(avarageValue, 2);

                            if ((int)avarageValue != 0)
                            {
                                calculationResult.Add(sectionCalculateResult);
                            }
                        }

                    }

                    //get first section and identify type of the element to set up range of the bar graph (1-5 , 1-10 etc.)
                    if (templateSections.Any() && calculationResult.Any())
                    {
                        var firstSection =
                            templateSections.FirstOrDefault(
                                item => item.Title.Equals(calculationResult.FirstOrDefault().ResultKey));
                        if (firstSection != null && firstSection.Items.Any())
                        {
                            var itemId = firstSection.Items.FirstOrDefault();
                            var itemType = template.GetItem(itemId);
                            var maxRange = GetMaxRange(itemType);

                            foreach (var calculation in calculationResult)
                                calculation.Points = maxRange;
                        }
                    }
                }
            }

            return new AnalysisItemResult
            {
                AggregateResults = AggregateResult(answers),
                CalculateResults = calculationResult.ToArray()
            };
        }

        //protected override AnalysisItemResult AggregatedData()
        //{
        //    List<ItemAnswer> answers = new List<ItemAnswer>();
        //    var calculationResult = new List<CalculateResult>();
        //    var itemPages = new Dictionary<int, List<int>>();
            
        //    if (this.TemplateID != null)
        //    {
        //        var sectionHeaders = ResponseTemplateManager.GetResponseTemplateSectionsIds(this.TemplateID.Value);
        //        var template = ResponseTemplateManager.GetResponseTemplate(this.TemplateID.Value);
                
        //        // filter pages with section items
        //        foreach (var key in template.TemplatePages.Keys)
        //        {
        //            var page = template.TemplatePages[key];

        //            // if page has secton item , add this page to filtered list of data
        //            foreach (var sectionHeader in sectionHeaders.Keys)
        //            {
        //                if (page.ContainsItem(sectionHeader) && page.PageType == TemplatePageType.ContentPage)
        //                {
                            
        //                    itemPages.Add(page.ID.Value, page.ListItemIds().ToList());
        //                    break;
        //                }
        //            }
        //        }

        //        //remove item that are before section header
        //        foreach (var key in itemPages.Keys)
        //        {
        //            foreach (var itemId in itemPages[key].ToList())
        //            {
        //                if (!sectionHeaders.Keys.Contains(itemId))
        //                    itemPages[key].Remove(itemId);
        //                else
        //                    break;
        //            }
        //        }

        //        var sectionPages = GetPagesWithSections(itemPages, sectionHeaders);


        //        foreach (var section in sectionPages)
        //        {
        //            CalculateResult sectionCalculateResult = new CalculateResult {ResultKey = section.Title};

        //            List<ItemAnswer> allAnswers = new List<ItemAnswer>();
        //            foreach (var item in section.ItemIds)
        //                allAnswers.AddRange(item.Answers);
                 
        //            if (allAnswers.Any())
        //            {   //clear answers with without response id 
        //                allAnswers.RemoveAll(item => item.ResponseId == -1);
        //            }
                  

        //            if (allAnswers.Any())
        //            {
        //                sectionCalculateResult.MinValue = allAnswers.Min(item => int.Parse(item.AnswerText));
        //                sectionCalculateResult.MaxValue = allAnswers.Max(item => int.Parse(item.AnswerText));

        //                var avarageValue =  (double)allAnswers.Sum(item => int.Parse(item.AnswerText))/
        //                                                      allAnswers.Count;

        //                sectionCalculateResult.ResultValue = Math.Round(avarageValue, 2);

        //                calculationResult.Add(sectionCalculateResult);
        //            }
        //        }

        //        //get first section and identify type of the element to set up range of the bar graph (1-5 , 1-10 etc.)
        //        if (sectionPages.Any() && calculationResult.Any())
        //        {
        //            var firstSection =
        //                sectionPages.FirstOrDefault(
        //                    item => item.Title.Equals(calculationResult.FirstOrDefault().ResultKey));
        //            if (firstSection != null && firstSection.ItemIds.Any())
        //            {
        //                var item = firstSection.ItemIds.FirstOrDefault();
        //                var itemType = template.GetItem(item.Id);
        //                var maxRange = GetMaxRange(itemType);

        //                foreach (var calculation in calculationResult)
        //                    calculation.Points = maxRange;
        //            }
        //        }

        //    }

        //    return new AnalysisItemResult
        //    {
        //        AggregateResults = AggregateResult(answers),
        //        CalculateResults = calculationResult.ToArray()
        //    };
        //}

        private int GetMaxRange(ItemData item)
        {
            if (item == null)
                throw new ArgumentNullException();

            int result = 0;

            var type = item.GetType();

            switch (type.Name)
            {
                case "RatingScaleItemData":
                    result = ((RatingScaleItemData) item).EndValue;
                    break;
                case "NetPromoterScoreItemData":
                    result = 10;
                    break;
                default:
                    throw new InvalidCastException("cant cast item type data to exect type");

            }

            return result;
        }


        private List<GovernancePriorityGraphSection> GetPagesWithSections(Dictionary<int, List<int>> pages, Dictionary<int, string> sectionHeaders)
        {
            List<GovernancePriorityGraphSection> result = new List<GovernancePriorityGraphSection>();
         
            foreach (var page in pages)
            {
                foreach (var itemId in page.Value)
                {
                    //if it is section create section item and set up title property
                    if (sectionHeaders.Keys.Contains(itemId))
                    {
                        var section = new GovernancePriorityGraphSection
                        {
                            Title = sectionHeaders.FirstOrDefault(item => item.Key.Equals(itemId)).Value,
                            ItemIds = new List<SectionItem>()
                        };

                        result.Add(section);
                    }

                    //if it is not a section add this id to the list 
                    if (!sectionHeaders.Keys.Contains(itemId))
                    {
                        int responseCount;
                        result.Last().ItemIds.Add(new SectionItem() {Id = itemId, Answers = RetrieveData(itemId, out responseCount) });
                    }
                }
            }

            //leave only source items 
            foreach (var section in result)
            {
                foreach (var item in section.ItemIds.ToList())
                {
                    if (!SourceItemIds.Contains(item.Id))
                        section.ItemIds.Remove(item);
                }
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

    /// <summary>
    /// GovernancePriorityGraphSection
    /// </summary>
    public class GovernancePriorityGraphSection
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the item ids.
        /// </summary>
        /// <value>
        /// The item ids.
        /// </value>
        public List<SectionItem> ItemIds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GovernancePriorityGraphSection" /> class.
        /// </summary>
        public GovernancePriorityGraphSection()
        {
            ItemIds = new List<SectionItem>();
        }
    }

    /// <summary>
    /// SectionItem
    /// </summary>
    public class SectionItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        /// <value>
        /// The answers.
        /// </value>
        public IEnumerable<ItemAnswer> Answers { get; set; }
    }
}
