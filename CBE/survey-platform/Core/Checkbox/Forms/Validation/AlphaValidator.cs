using System;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// A regular expression validator which determines if a value contains only alpha numeric characters.
    /// </summary>
    public class AlphaValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor, set the regular expression
        /// </summary>
        public AlphaValidator()
        {
            _regex = @"^[a-zA-Z\s]+$";
        }

        /// <summary>
        /// Get the validation error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/alpha", languageCode);
        }
    }
}