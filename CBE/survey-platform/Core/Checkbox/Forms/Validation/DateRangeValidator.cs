using System;
using System.Text;
using System.Globalization;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for date ranges
    /// </summary>
    public class DateRangeValidator : RangeValidator<DateTime>
    {
        /// <summary>
        /// Create a new date range validator
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="answerFormat"></param>
        public DateRangeValidator(DateTime? minDate, DateTime? maxDate, AnswerFormat answerFormat)
        {
            if (minDate.HasValue)
            {
                MinValue = minDate.Value;
            }

            if (maxDate.HasValue)
            {
                MaxValue = maxDate.Value;
            }

            AnswerFormat = answerFormat;
        }


        /// <summary>
        /// Get the date answer format
        /// </summary>
        public AnswerFormat AnswerFormat { get; private set; }

        /// <summary>
        /// Validate the date
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(DateTime input)
        {
            if (MinValueSet && MinValue > input)
            {
                return false;
            }
            if (MaxValueSet && MaxValue < input)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the valudation message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            CultureInfo culture;
            StringBuilder sb = new StringBuilder();
            
            if (AnswerFormat == AnswerFormat.Date_USA)
            {
                culture = Utilities.GetUsCulture();
            }
            else if (AnswerFormat == AnswerFormat.Date_ROTW)
            {
                culture = Utilities.GetRotwCulture();
            }
            else
            {
                culture = CultureInfo.CurrentCulture;
            }
            
            if (MinValueSet && MaxValueSet)
            {
                sb.Append(TextManager.GetText("/validationMessages/date/dateBetween", languageCode));
                sb.Replace("{start}", MinValue.ToString("d", culture));
                sb.Replace("{end}", MaxValue.ToString("d", culture));
            }
            else if (MinValueSet)
            {
                sb.Append(TextManager.GetText("/validationMessages/date/dateAfter", languageCode));
                sb.Replace("{date}", MinValue.ToString("d", culture));
            }
            else if (MaxValueSet)
            {
                sb.Append(TextManager.GetText("/validationMessages/date/dateBefore", languageCode));
                sb.Replace("{date}", MaxValue.ToString("d", culture));
            }

            return sb.ToString();
        }
    }
}
