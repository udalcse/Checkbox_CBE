using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate answers to a select many question
    /// </summary>
    public class SelectManyValidator : SelectItemValidator
    {
        /// <summary>
        /// Validate that the required number of items have been selected
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override bool ValidateRequired(SelectItem input)
        {
            int selectedOptionsCount = input.SelectedOptions.Count;
            
            var minToSelect = ((SelectMany)input).MinToSelect;
            var maxToSelect = ((SelectMany)input).MaxToSelect;
            bool allowNoneOfAbove = ((SelectMany)input).AllowNoneOfAbove;

            if (minToSelect.HasValue && minToSelect.Value > selectedOptionsCount)
            {
                if (minToSelect.Value == 1)
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/selectMany/minErrorSingular", input.LanguageCode);
                }
                else
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/selectMany/minError", input.LanguageCode).Replace("{min}", minToSelect.ToString());
                }
                return false;
            }
            if (maxToSelect.HasValue && maxToSelect.Value < selectedOptionsCount)
            {
                ErrorMessage = TextManager.GetText("/validationMessages/selectMany/maxError", input.LanguageCode).Replace("{max}", maxToSelect.ToString());
                return false;
            }

            if (allowNoneOfAbove && input.SelectedOptions.Any(o => o.IsNoneOfAbove) && selectedOptionsCount > 1)
            {
                var optionText = Utilities.StripHtml(input.SelectedOptions.First(o => o.IsNoneOfAbove).Text);
                ErrorMessage = TextManager.GetText("/validationMessages/selectMany/noneOfAbove", input.LanguageCode).Replace("{noneOfAbove}", optionText);
                return false;
            }

            return true;
        }
    }
}
