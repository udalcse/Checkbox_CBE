using System;
using System.Text;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for ensuring a numeric value is in a given range
    /// </summary>
    public class DoubleRangeValidator : RangeValidator<double>
    {
        /// <summary>
        /// Constructor that accepts a max and min value.
        /// </summary>
        /// <param name="min">Minimum allowed value. NULL indicates no minimum.</param>
        /// <param name="max">Maximum allowed value. NULL indicates no maximum.</param>
        public DoubleRangeValidator(Nullable<Double> min, Nullable<Double> max)
        {
            if (min.HasValue)
            {
                MinValue = min.Value;
            }

            if (max.HasValue)
            {
                MaxValue = max.Value;
            }
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(double input)
        {
            if (MinValueSet && MinValue > input)
            {
                return false;
            }
            else if (MaxValueSet && MaxValue < input)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
