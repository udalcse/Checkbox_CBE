using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter to compare item values against filter value
    /// </summary>
    [Serializable]
    public class ItemFilter : AnswerDataObjectFilter
    {
        private int _itemID;

        /// <summary>
        /// Get/set the id of the filter item source
        /// </summary>
        public virtual Int32 ItemID
        {
            get { return _itemID; }
        }

        /// <summary>
        /// Configure the filter
        /// </summary>
        /// <param name="filterData"></param>
        /// <param name="languageCode"></param>
        public override void Configure(FilterData filterData, string languageCode)
        {
            base.Configure(filterData, languageCode);

            if (filterData is ItemFilterData)
            {
                _itemID = ((ItemFilterData)filterData).ItemId;
            }
        }


        /// <summary>
        /// Evaluate the filter for an answer row
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties"></param>
        /// <param name="answerHasValue"></param>
        /// <returns></returns>
        public override bool EvaluateFilter(ItemAnswer answer, AnalysisAnswerData answerData, ResponseProperties responseProperties, out bool answerHasValue)
        {
            if (answer.ItemId != ItemID)
            {
                var surveyId = ResponseManager.GetSurveyIdFromResponseId(answer.ResponseId);
                var filteringItemAnswers = answerData.ListItemResponseAnswers(answer.ResponseId, ItemID, surveyId.Value);
                Dictionary<long, ResponseProperties> responsePros = new Dictionary<long, ResponseProperties>
                                                                        {{answer.ResponseId, responseProperties}};
                answerHasValue = EvaluateFilter(filteringItemAnswers, answerData, responsePros);
                return answerHasValue;
            }
            
            answerHasValue = answer.ItemId == ItemID;

            if (answerHasValue)
            {
                //Handle option & no option case
                if (!answer.OptionId.HasValue)
                {
                    return CompareValue(answer.AnswerText);
                }

                return CompareValue(answer.OptionId.Value);
            }

            //If there is no answer, return false if this filter must have a value to evaluate to true.
            return !ValueRequired;
        }
    }
}
