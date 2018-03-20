using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate lower-case text
    /// </summary>
    public class LowerCaseAlphaValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor.  Set the validation text
        /// </summary>
        public LowerCaseAlphaValidator()
        {
            _regex = @"^[a-z\s]+$";
        }

        /// <summary>
        /// Get the validation failure message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/lowerCase", languageCode);
        }
    }
}
