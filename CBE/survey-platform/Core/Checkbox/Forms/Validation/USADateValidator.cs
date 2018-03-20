using System;
using System.Text;
using System.Text.RegularExpressions;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;
using System.Globalization;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that a given value is a valid date in MM/DD/YYYY format
    /// </summary>
    public class USADateValidator : Validator<string>
    {
        /// <summary>
        /// Constructor.  Set the regular expression
        /// </summary>
        public USADateValidator()
        {
            //_regex = @"((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))";
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            DateTime result = new DateTime();
            return DateTime.TryParse(input, CultureInfo.GetCultureInfo("en-US").DateTimeFormat, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Get the date error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/dateUS", languageCode);
        }
    }
}
