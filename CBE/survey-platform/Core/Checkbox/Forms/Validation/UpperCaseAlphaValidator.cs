using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate upper-case string
    /// </summary>
    public class UpperCaseAlphaValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor.  Sets the regular expression
        /// </summary>
        public UpperCaseAlphaValidator()
        {
            _regex = @"^[A-Z\s]+$";
        }

        /// <summary>
        /// Get the message for validation failure
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/uppercase", languageCode);
        }
    }
}
