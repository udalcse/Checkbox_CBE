using System;
using System.Text;
using System.Text.RegularExpressions;

using Checkbox.Globalization.Text;

using Prezza.Framework.ExceptionHandling;
using System.Globalization;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that a given value is a date of the format DD/MM/YYYY
    /// </summary>
    public class ROTWDateValidator : Validator<string>
    {
        /// <summary>
        /// Constructor.  Set the regular expression
        /// </summary>
        public ROTWDateValidator()
        {
            //_regex = @"^(?:(?:(?:0?[1-9]|1\d|2[0-8])\/(?:0?[1-9]|1[0-2]))\/(?:(?:1[6-9]|[2-9]\d)\d{2}))$|^(?:(?:(?:31\/(0?[13578]|1?[02]))|(?:(?:29|30)\/(?:0?[1,3-9]|1[0-2])))\/(?:(?:1[6-9]|[2-9]\d)\d{2}))$|^(?:29\/0?2\/(?:(?:(?:1[6-9]|[2-9]\d)(?:0[048]|[2468][048]|[13579][26]))))$";
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            DateTime result = new DateTime();
            return DateTime.TryParse(input, CultureInfo.GetCultureInfo("en-GB").DateTimeFormat, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Get the date error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/dateROTW", languageCode);
        }
    }
}
