using System;
using System.Text.RegularExpressions;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate a password validator
    /// </summary>
    public class PasswordValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Error text id.
        /// </summary>
        private string ErrorTextId { get; set; }

        /// <summary>
        /// Additional parameter
        /// </summary>
        private string AdditionalParameter { get; set; }

        /// <summary>
        /// Constructor, set the regex expression
        /// </summary>
        public PasswordValidator()
        {
            String rawRegex = @"[\d\w\s]";
            _regex = rawRegex.Normalize();
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
                if (ApplicationManager.AppSettings.EnforcePasswordLimitsGlobally)
                {
                    if (ApplicationManager.AppSettings.MinPasswordLength > input.Length)
                    {
                        ErrorTextId = "/validationMessages/regex/password/minLength";
                        AdditionalParameter = ApplicationManager.AppSettings.MinPasswordLength.ToString();
                        return false;
                    }

                    var matchedExpressions = RegularExpression.Matches(input.Normalize());

                    if ((input.Length - matchedExpressions.Count) < ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric)
                    {
                        ErrorTextId = "/validationMessages/regex/password/minNonAlphaNumericCount";
                        AdditionalParameter = ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric.ToString();
                        return false;
                    }
                }

                return true;
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
            return String.Format(TextManager.GetText(ErrorTextId, languageCode), AdditionalParameter);
        }
    }
}
