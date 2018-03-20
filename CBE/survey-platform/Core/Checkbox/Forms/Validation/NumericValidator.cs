using System;

using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for numerics
    /// </summary>
    public class NumericValidator : Validator<string>
    {
        /// <summary>
        /// Validate the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            return Utilities.GetDouble(input).HasValue;
        }

        /// <summary>
        /// Get the error message associated with a validation failure for this item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/numeric", languageCode);
        }
    }
}
