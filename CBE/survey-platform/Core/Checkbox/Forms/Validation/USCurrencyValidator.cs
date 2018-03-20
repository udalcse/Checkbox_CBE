using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for matching US currency values
    /// </summary>
    public class USCurrencyValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public USCurrencyValidator()
        {
            _regex = @"^\$?(\d{1,3},?(\d{3},?)*\d{3}(\.\d{1,3})?|\d{1,3}(\.\d{2})?)$";
            //_regex = @"^\$?((([1-9][0-9]{0,2})(,\d{3})*)|([1-9][0-9]*)|(0))?(\.\d{2})?$";
        }

        /// <summary>
        /// Get error message for invalid input
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/usCurrency", languageCode);
        }
    }
}
