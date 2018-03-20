using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Callout validator for entering colors.
    /// </summary>
    public class CalloutColorValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (Utilities.IsNullOrEmpty(value))
            {
                return true;
            }
            
            try
            {
                Utilities.GetColor(value, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
