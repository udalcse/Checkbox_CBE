using System;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validator for numeric inputs that supports min/max validation as well.
    /// </summary>
    public class CalloutIntegerValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Get/set the min value
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Get/set the max value
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (Utilities.IsNullOrEmpty(value))
            {
                return false;
            }
            
            int numericValue;

            if (int.TryParse(value, out numericValue))
            {
                if (MinValue.HasValue && numericValue < MinValue)
                {
                    return false;
                }

                if (MaxValue.HasValue && numericValue > MaxValue)
                {
                    return false;
                }

                return true;
            }
            
            return false;
        }
    }
}
