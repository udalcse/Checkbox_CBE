using System;

using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator that uses regex to validate that a number is a decimal.
    /// </summary>
    public class DecimalValidator : Validator<string>
    {
        /// <summary>
        /// Validate the input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            //Verify that a decimal exists and that there is at least one character
            // after the decimal.
            if (!input.Contains(".") && !input.Contains(","))
            {
                return false;
            }

            //Verify at least one character after the decimal
            int lastCharIndex = input.Length - 1;
            int lastPeriodIndex = input.LastIndexOf('.');
            int lastCommaIndex = input.LastIndexOf(',');

            //If a decimal is the last character, then not valid
            if (lastPeriodIndex == lastCharIndex
                || lastCommaIndex == lastCharIndex)
            {
                return false;
            }

            return Utilities.GetDouble(input).HasValue;
        }
        
        /// <summary>
        /// Get the validation error message.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/decimal", languageCode);
        }
    }
}
