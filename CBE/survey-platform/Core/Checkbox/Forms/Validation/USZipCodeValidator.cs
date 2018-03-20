using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for US Zip Codes #####-####
    /// </summary>
    public class USZipCodeValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public USZipCodeValidator()
        {
            _regex = @"^\d{5}-\d{4}$|^\d{5}$";
        }

        /// <summary>
        /// Get the error message for invalid input
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/usPostal", languageCode);
        }
    }
}
