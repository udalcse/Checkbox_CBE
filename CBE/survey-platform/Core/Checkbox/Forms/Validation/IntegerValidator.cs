using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that a given value is an integer.
    /// </summary>
    public class IntegerValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Construct validator.
        /// </summary>
        public IntegerValidator()
        {
            _regex = @"^-[0-9]+$|^[0-9]+$";
        }

        /// <summary>
        /// Get the validation error message.
        /// </summary>
        /// <param name="languageCode">Language code for message.</param>
        /// <returns>Error message.</returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/integer", languageCode);
        }
    }
}
