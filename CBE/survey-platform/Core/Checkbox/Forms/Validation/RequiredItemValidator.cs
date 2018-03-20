using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for ensuring items have answers.
    /// </summary>
    public class RequiredItemValidator : Validator<AnswerableItem>
    {
        /// <summary>
        /// Get the validation message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/required", languageCode);
        }

        /// <summary>
        /// Validate the value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(AnswerableItem input)
        {
            return input.HasAnswer;
        }
    }
}
