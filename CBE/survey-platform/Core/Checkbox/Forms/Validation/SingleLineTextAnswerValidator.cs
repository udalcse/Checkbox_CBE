using System;
using System.Globalization;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate the answer to a single line text input, which extends text validation
    /// to include min/max values for certain types.
    /// </summary>
    public class SingleLineTextAnswerValidator : TextAnswerValidator
    {
        /// <summary>
        /// Perform base validation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(TextItem input)
        {
            if (!base.Validate(input))
            {
                return false;
            }

            //Convert values to date time
            if (input.HasAnswer)
            {
                SingleLineTextBoxItem slTextItem = (SingleLineTextBoxItem)input;

                if (slTextItem.Format == AnswerFormat.Date
                    || slTextItem.Format == AnswerFormat.Date_ROTW
                    || slTextItem.Format == AnswerFormat.Date_USA)
                {
                    CultureInfo culture;

                    if (slTextItem.Format == AnswerFormat.Date_USA)
                    {
                        culture = Utilities.GetUsCulture();
                    }
                    else if (slTextItem.Format == AnswerFormat.Date_ROTW)
                    {
                        culture = Utilities.GetRotwCulture();
                    }
                    else
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    //Validate the date
                    DateTime answerDate = DateTime.Parse(slTextItem.GetAnswer(), culture);

                    DateTime? minValue = slTextItem.MinNumericValue.HasValue ? (DateTime?)(new DateTime((long)slTextItem.MinNumericValue.Value)) : null;
                    DateTime? maxValue = slTextItem.MaxNumericValue.HasValue ? (DateTime?)(new DateTime((long)slTextItem.MaxNumericValue.Value)) : null;

                    DateRangeValidator validator = new DateRangeValidator(
                        minValue,
                        maxValue,
                        input.Format);

                    if (!validator.Validate(answerDate))
                    {
                        ErrorMessage = validator.GetMessage(input.LanguageCode);
                        return false;
                    }
                }
                else if (slTextItem.Format == AnswerFormat.Money)
                {
                    double? answerDouble = Utilities.GetCurrencyNumericValue(input.GetAnswer());

                    if (answerDouble.HasValue)
                    {
                        DoubleRangeValidator doubleValidator = new DoubleRangeValidator(slTextItem.MinNumericValue, slTextItem.MaxNumericValue);

                        if (!doubleValidator.Validate(answerDouble.Value))
                        {
                            ErrorMessage = doubleValidator.GetMessage(slTextItem.LanguageCode);
                            return false;
                        }
                    }
                }
                else
                {
                    double? answerDouble = Utilities.GetDouble(input.GetAnswer());

                    if (answerDouble.HasValue)
                    {
                        DoubleRangeValidator doubleValidator = new DoubleRangeValidator(slTextItem.MinNumericValue, slTextItem.MaxNumericValue);

                        if (!doubleValidator.Validate(answerDouble.Value))
                        {
                            ErrorMessage = doubleValidator.GetMessage(slTextItem.LanguageCode);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
