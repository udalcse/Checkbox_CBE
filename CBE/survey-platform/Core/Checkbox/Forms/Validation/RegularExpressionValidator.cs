using System;
using System.Text.RegularExpressions;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator of strings based on regular expressions
    /// </summary>
    public class RegularExpressionValidator : Validator<string>
    {
        /// <summary>
        /// Regular expression used to validate input
        /// </summary>
        protected string _regex;

        /// <summary>
        /// Validate the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool  Validate(string input)
        {
            if (_regex == null || _regex == string.Empty)
                return true;

            try
            {
                return RegularExpression.IsMatch(input);
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
        /// Get the associated regular expression for this validator
        /// </summary>
        public virtual Regex RegularExpression
        {
            get
            {
                return new Regex(_regex, RegexOptions.None);
            }
        }

        /// <summary>
        /// Get the validation message for this error
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return _regex;
        }
    }
}
