using Checkbox.Forms.Items;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate answers to a rating scale question.
    /// </summary>
    public class RatingScaleValidator : SelectItemValidator
    {
        /// <summary>
        /// Validate the select input's answer(s)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(SelectItem input)
        {
            ErrorMessage = string.Empty;

            if (!ValidateRequired(input))
            {
                return false;
            }
            
            return true;            
        }

    }
}
