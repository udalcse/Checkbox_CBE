using System;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// A regular expression validator which determines if a value contains only alpha numeric characters.
    /// </summary>
    public class AlternateURLValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Maximal allowed length for alternate URL
        /// </summary>
        public const int MaxLenght = 200;

        /// <summary>
        /// Constructor, set the regular expression
        /// </summary>
        public AlternateURLValidator()
        {
            _regex = @"^[a-zA-Z0-9_-]+$";
        }

        /// <summary>
        /// Get the validation error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/alphaNumeric", languageCode);
        }

        public override bool Validate(string input)
        {
            return base.Validate(input);
        }
    }
}