using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for Rank order item
    /// </summary>
    public class RankOrderValidator : Validator<RankOrder>
    {
        /// <summary>
        /// This is an error message
        /// </summary>
        protected string ErrorMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return ErrorMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(RankOrder input)
        {
            ErrorMessage = String.Empty;

            //If none is selected - the item is valid.
            if (!input.Required && input.SelectedOptions.Count == 0)
                return true;

            if (input.RankOrderType == RankOrderType.Numeric)
            {
                if (input.SelectedOptions.Count < input.ShownOptionsCount)
                {
                    string validationErrorFormat = TextManager.GetText(input.ShownOptionsCount == input.Options.Count 
                        ? "/validationMessages/rankOrder/numeric/answerIsRequired" 
                        : "/validationMessages/rankOrder/numeric/notEnoughOptionsSelected", input.LanguageCode);

                    ErrorMessage = string.Format(validationErrorFormat, input.ShownOptionsCount - input.SelectedOptions.Count);
                    return false;
                }

                if (input.SelectedOptions.Any(p => p.Points < 1 || p.Points > input.ShownOptionsCount))
                {
                    ErrorMessage =
                        String.Format(
                            TextManager.GetText("/validationMessages/rankOrder/numeric/answerIsInvalid",
                                                input.LanguageCode), input.ShownOptionsCount);
                    return false;
                }


                //Check all the points are different
                double[] points = input.SelectedOptions.Select(p => p.Points).OrderBy(p => p).ToArray();
               
                for (int i = 0; i < points.Length - 1; i++)
                {
                    if (points[i + 1] - points[i] < 0.1)
                    {
                        ErrorMessage = TextManager.GetText(
                            "/validationMessages/rankOrder/numeric/pointsMustBeDifferent", input.LanguageCode);
                        return false;
                    }
                }
            }

            if (input.SelectedOptions.Count < input.ShownOptionsCount)
            {
                if (input.RankOrderType == RankOrderType.TopN)
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/rankOrder/topN/answerIsRequired",
                                                       input.LanguageCode);
                    return false;
                }
                if (input.RankOrderType == RankOrderType.SelectableDragnDrop)
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/rankOrder/selectable/answerIsRequired",
                                                       input.LanguageCode);
                    return false;
                }
            }

            return true;
        }
    }
}
