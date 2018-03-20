using System;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validator for numeric inputs that supports min/max validation as well.
    /// </summary>
    public class CalloutNumericValidator : WarningCalloutValidator
    {
        private Nullable<double> _minValue;
        private Nullable<double> _maxValue;

        /// <summary>
        /// Get/set the min value
        /// </summary>
        public Nullable<double> MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        /// <summary>
        /// Get/set the max value
        /// </summary>
        public Nullable<double> MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

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
            else
            {
                double numericValue;

                if (double.TryParse(value, out numericValue))
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
                else
                {
                    return false;
                }
            }
        }
    }
}
