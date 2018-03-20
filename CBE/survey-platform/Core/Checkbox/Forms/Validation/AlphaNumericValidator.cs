using System;
using System.Text.RegularExpressions;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate an alphanumeric string
    /// </summary>
    public class AlphaNumericValidator : RegularExpressionValidator
    {
        private readonly string _regex2 = @"^[0-9]\d*(\.\d+)?$".Normalize();

        /// <summary>
        /// Constructor, set the regex expression
        /// </summary>
        public AlphaNumericValidator()
        {
            _regex = @"^[a-zA-Z0-9 ]*$".Normalize();
        }

        /// <summary>
        /// Validate the expression
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            if (_regex == null || _regex == string.Empty)
                return true;

            try
            {
                return Regex.IsMatch(input.Normalize(), _regex2) || RegularExpression.IsMatch(input.Normalize());
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the validation error
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/alphaNumeric", languageCode);
        }
    }
}
