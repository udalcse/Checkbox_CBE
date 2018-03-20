
namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that a value represents a percentage
    /// </summary>
    public class PercentageValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor for percentage validation.
        /// </summary>
        public PercentageValidator()
        {
            _regex = @"^\d{0,2}(\.\d{1,4})? *%?$";
        }
    }
}
