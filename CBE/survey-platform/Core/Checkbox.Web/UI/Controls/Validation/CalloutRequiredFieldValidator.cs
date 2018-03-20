using System;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validator using callouts for required field validation
    /// </summary>
    public class CalloutRequiredFieldValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Get/set initial value to use for comparison
        /// </summary>
        public string InitialValue { get; set; }

        /// <summary>
        /// Validate the input as being answered and not equal to the initial value, if specified
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (Utilities.IsNotNullOrEmpty(InitialValue))
            {
                return Utilities.IsNotNullOrEmpty(value) && (value != InitialValue);
            }
            
            return Utilities.IsNotNullOrEmpty(value);
        }
    }
}
