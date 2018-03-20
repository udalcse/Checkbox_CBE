using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Common;

namespace Checkbox.Analytics.Computation
{
    /// <summary>
    /// Answer aggregator specific to the details item that provides specific handling for
    /// checkboxes.
    /// </summary>
    public class DetailsItemAnswerAggregator : ItemAnswerAggregator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preferAlias"></param>
        public DetailsItemAnswerAggregator(bool preferAlias)
            : base(preferAlias)
        {
        }

        /// <summary>
        /// Add an answer
        /// </summary>
        /// <param name="answerId">ID of answer to add</param>
        /// <param name="responseID">Response ID</param>
        /// <param name="responseGuid">Response Guid</param>
        /// <param name="itemID">Item ID</param>
        /// <param name="optionID">Option ID</param>
        /// <param name="answer">AnswerText</param>
        /*public override void AddAnswer(long answerId, long responseID, Guid? responseGuid, int itemID, int? optionID, string answer)
        {
            //See if there is already an answer for this response & item for select items.  If there are,
            // then this must be a select many and we want to comma separate the answers for the item rather
            // than have each selected option on it's own row.
            if (optionID.HasValue)
            {
                //Find an existing answer for the item/response
                if (Utilities.IsNotNullOrEmpty(answer)
                    && ResponseAnswerDictionary.ContainsKey(responseID))
                {
                    List<ItemAnswer> answerList = ListResponseAnswers(responseID);
                    
                    ItemAnswer itemAnswer = answerList.Find(a => a.ItemId == itemID);
                    
                    if (itemAnswer != null)
                    {
                        if (Utilities.IsNotNullOrEmpty(itemAnswer.AnswerText))
                        {
                            itemAnswer.AnswerText = itemAnswer.AnswerText + ", " + answer;
                        }
                        else
                        {
                            itemAnswer.AnswerText = answer;
                        }
                    }
                }
            }

            //Item is not a checkbox or there is no answer already so just call the base implementation
            base.AddAnswer(answerId, responseID, responseGuid, itemID, optionID, answer);
        }
        */
    }
}
