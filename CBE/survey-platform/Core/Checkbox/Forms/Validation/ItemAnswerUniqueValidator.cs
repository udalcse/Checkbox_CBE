using System;
using System.Collections.Generic;

using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator to validate unique answers
    /// </summary>
    public class ItemAnswerUniqueValidator : Validator<List<IAnswerable>>
    {
        /// <summary>
        /// Validate the items
        /// </summary>
        /// <returns></returns>
        public override bool Validate(List<IAnswerable> itemsToValidate)
        {
            List<string> itemAnswers = new List<string>();

            foreach (IAnswerable item in itemsToValidate)
            {
                if (item.HasAnswer)
                {
                    string answer = item.GetAnswer().ToLower();

                    if (itemAnswers.Contains(answer))
                    {
                        return false;
                    }

                    itemAnswers.Add(answer);
                }
            }

            return true;
        }

        /// <summary>
        /// Get the validation message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/answerUniqueValidator/validationError", languageCode);
        }
    }
}
