using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using System.Text.RegularExpressions;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Provides validation for email addresses
    /// </summary>
    public class EmailValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EmailValidator()
        {
            _regex = @"^[\p{L}0-9!$'*+\-_]+(\.[\p{L}0-9!$'*+\-_]+)*@[\p{L}0-9\-]+(\.[\p{L}0-9\-]+)*(\.[\p{L}]{2,})$";
        }

        /// <summary>
        /// Validates optional email, returns true for blanc strings
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool ValidateOptional(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            return Validate(input);
        }

        /// <summary>
        /// Validate the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            //Check to see if email address validation is enabled. If it has been disabled always return true.
            if (!ApplicationManager.AppSettings.EnableEmailAddressValidation) { return true; }

            // The local half of an email address can contain the following:
            // Uppercase and lowercase English letters (a-z, A-Z)
            // Digits 0 through 9
            // Characters ! # $ % & ' * + - / = ? ^ _ ` { | } ~ .
            // Character . provided that it is not the first nor last character, nor may it appear two or more times consecutively.
            if (Utilities.IsNullOrEmpty(input))
                return false;

            string[] address = input.Split('@');

            if (address.Length != 2)
                return false;

            string local = address[0] ?? string.Empty;
            string domain = address[1] ?? string.Empty;

            //Loop through the local part of the email address and remove the infrequently used, but valid characters, so that
            //the regular expression used to validate is not overly complex.
            string[] optional = ApplicationManager.AppSettings.EmailAddressOptionalCharacters.Split(',');
            foreach (string value in optional)
            {
                if (".".Equals(value))
                {
                    if (local.StartsWith(".") || local.EndsWith(".") || ContainsConsecutive(local, '.'))
                    {
                        return false;
                    }
                }

                local = local.Replace(value, string.Empty);
            }

            //Clean up the domain
            if (domain.StartsWith("["))
			{
				if (!domain.EndsWith("]"))
					return false;
			}
			else 
				if(domain.Contains("[") || domain.Contains("]"))
					return false;

            domain = domain.Replace("[", string.Empty);
            domain = domain.Replace("]", string.Empty);

            return RegularExpression.IsMatch(string.Format("{0}@{1}", local, domain));
        }

        /// <summary>
        /// Override property to ignore cultures and case
        /// </summary>
        public override Regex RegularExpression
        {
            get
            {
                return new Regex(_regex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }
        }

        private bool ContainsConsecutive(IEnumerable<char> source, char pattern)
        {
            int count = 0;

            foreach (char c in source)
            {
                if (count > 1) { return true; }

                if (c == pattern)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the error message for malformed email address
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/email", languageCode);
        }
    }
}
