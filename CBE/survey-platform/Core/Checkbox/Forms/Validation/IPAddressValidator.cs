using System;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate dot separated IP Address
    /// </summary>
    public class IPAddressValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IPAddressValidator()
        {
            _regex = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
        }

        /// <summary>
        /// Get the error message for invalid IP Adress
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/ipAddress", languageCode);
        }
    }
}
