using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for a social security number
    /// </summary>
    public class SocialSecurityValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SocialSecurityValidator()
        {
            _regex = @"(^|\s)(?!219-09-9999|078-05-1120)(?!666|000|9\d{2})\d{3}(-?|[\.])(?!00)\d{2}(-?|[\.])(?!0{4})\d{4}($|\s|[;:,!\.\?])";
            // _regex = @"(^|\s)(00[1-9]|0[1-9]0|0[1-9][1-9]|[1-6]\d{2}|7[0-6]\d|77[0-2])(-?|[\. ])([1-9]0|0[1-9]|[1-9][1-9])\3(\d{3}[1-9]|[1-9]\d{3}|\d[1-9]\d{2}|\d{2}[1-9]\d)($|\s|[;:,!\.\?])";
            //_regex = @"(^)(00[1-9]|0[1-9]0|0[1-9][1-9]|[1-6]\d{2}|7[0-6]\d|77[0-2])(-?|[\. ])([1-9]0|0[1-9]|[1-9][1-9])\3(\d{3}[1-9]|[1-9]\d{3}|\d[1-9]\d{2}|\d{2}[1-9]\d)($)";
        }

        /// <summary>
        /// Get error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/ssn", languageCode);
        }
    }
}
