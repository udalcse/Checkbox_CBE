using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for phone numbers (NANP)
    /// </summary>
    public class USPhoneValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor.  Sets the regular expression
        /// </summary>
        public USPhoneValidator()
        {
            _regex = @"^1?[-\. ]?(\(\d{3}\)?[-\. ]?|\d{3}?[-\. ]?)?\d{3}?[-\. ]?\d{4}$";
            //_regex = @"^([\(]{1}[0-9]{3}[\)]{1}[\.| |\-]{0,1}|^[0-9]{3}[\.|\-| ]?)?[0-9]{3}(\.|\-| )?[0-9]{4}$";
        }

        /// <summary>
        /// Get the error to display when validation fails
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/phoneNumber", languageCode);
        }

        /// <summary>
        /// Validate the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            System.Globalization.CompareInfo cmpUrl = System.Globalization.CultureInfo.InvariantCulture.CompareInfo;
            if (cmpUrl.IsPrefix(input, "+"))
                input = input.TrimStart('+');

            return base.Validate(input);
        }
    }
}
