using System;
using System.Text;
using System.Text.RegularExpressions;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that a given value is a valid date in either MM/DD/YYYY or DD/MM/YYYY format
    /// </summary>
    public class DateValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Returns the Cultural Identifier associated with the Globalized date format being validated.
        /// </summary>
        /// <returns></returns>
        public virtual string CulturalIdentifier
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Constructor.  Set the regular expression
        /// </summary>
        public DateValidator()
        {
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            try
            {
                Regex regexUS = new Regex(@"((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))");
                Regex regexROTW = new Regex(@"^(?:(?:(?:0?[1-9]|1\d|2[0-8])\/(?:0?[1-9]|1[0-2]))\/(?:(?:1[6-9]|[2-9]\d)\d{2}))$|^(?:(?:(?:31\/(0?[13578]|1?[02]))|(?:(?:29|30)\/(?:0?[1,3-9]|1[0-2])))\/(?:(?:1[6-9]|[2-9]\d)\d{2}))$|^(?:29\/0?2\/(?:(?:(?:1[6-9]|[2-9]\d)(?:0[048]|[2468][048]|[13579][26]))))$");
                return regexUS.IsMatch(input) || regexROTW.IsMatch(input);
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
        /// Get the date error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/date", languageCode);
        }
    }
}
