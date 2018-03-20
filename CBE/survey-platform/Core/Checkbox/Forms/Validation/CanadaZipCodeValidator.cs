using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate Canadian zip codes
    /// </summary>
    public class CanadaZipCodeValidator : RegularExpressionValidator 
    {
        /// <summary>
        /// Validate Canadian Zip Codes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            //Canadian zip codes have format ANA NAN where A = alpha and N = numeric
            //D, F, I, O, Q, and U are not used
            string alpha = "[a-ceghj-npr-tv-zA-CEGHJ-NPR-TV-Z]";
            string number = "[0-9]";

            //Remove the space
            _regex = @"^" + alpha + number + alpha + number + alpha + number + "$";

            return base.Validate(input.Replace(" ", ""));
        }

        /// <summary>
        /// Get the validation message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/usPostal", languageCode);
        }
    }
}
