using System;
using System.Collections.Generic;

using Checkbox.Forms.Items;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for text answers
    /// </summary>
    public class TextAnswerValidator : Validator<TextItem>
    {
        private string _errorMessage;

        /// <summary>
        /// Get/set error message
        /// </summary>
        protected string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        /// <summary>
        /// Get the error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return _errorMessage;
        }

      
        /// <summary>
        /// Validate a given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(TextItem input)
        {
            //First, validate required
            if (!ValidateRequired(input))
            {
                return false;
            }

            //If there is no answer, then return true
            if (!input.HasAnswer)
            {
                return true;
            }

            //Otherwise, validate that the answer is in the correct format.
            List<Validator<string>> validators = GetFormatValidatorList(input.Format, input.CustomFormatId);

            //Consider valid if any are valid
            bool tempValid = false;

            string answer = input.GetAnswer();
            
            foreach (Validator<string> validator in validators)
            {
                if (answer == input.DefaultText || validator.Validate(answer))
                {
                    tempValid = true;
                    break;
                }
                
                ErrorMessage = validator.GetMessage(input.LanguageCode);
            }

            //If format is not valid, return false
            if (validators.Count > 0 && !tempValid)
            {
                return false;
            }

            if (!ValidateLength(input))
            {
                return false;
            }

            //Everything looks good
            return true;
        }
        
        /// <summary>
        /// Validate required value
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ValidateRequired(TextItem item)
        {
            if (item.Required)
            {
                RequiredItemValidator validator = new RequiredItemValidator();

                if (!validator.Validate(item))
                {
                    ErrorMessage = validator.GetMessage(item.LanguageCode);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validate answer length
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ValidateLength(TextItem item)
        {

            //Get an answer length validator, if necessary
            bool lengthValidationRequired = (
                item.Format == AnswerFormat.None
                || item.Format == AnswerFormat.Alpha
                || item.Format == AnswerFormat.AlphaNumeric
                || item.Format == AnswerFormat.Email
                || item.Format == AnswerFormat.Uppercase
                || item.Format == AnswerFormat.Lowercase
                || item.Format == AnswerFormat.URL);

            if (lengthValidationRequired)
            {
                AnswerLengthValidator validator = new AnswerLengthValidator();

                if(!validator.Validate(item))
                {
                    ErrorMessage = validator.GetMessage(item.LanguageCode);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get a list of applicable validators depending on item format
        /// </summary>
        /// <returns></returns>
        public static List<Validator<string>> GetFormatValidatorList(AnswerFormat answerFormat, string customFormatId)
        {
            var validators = new List<Validator<string>>();

            //Get a format-specific answer validator
            // validate format
            switch (answerFormat)
            {
                case AnswerFormat.Alpha:
                    validators.Add(new AlphaValidator());
                    break;
                case AnswerFormat.AlphaNumeric:
                    validators.Add(new AlphaNumericValidator());
                    break;
                case AnswerFormat.Decimal:
                    validators.Add(new DecimalValidator());
                    break;
                case AnswerFormat.Email:
                    validators.Add(new EmailValidator());
                    break;
                case AnswerFormat.Numeric:
                    validators.Add(new NumericValidator());
                    break;
                case AnswerFormat.Postal:
                    validators.Add(new USZipCodeValidator());
                    validators.Add(new CanadaZipCodeValidator());
                    break;
                case AnswerFormat.Integer:
                    validators.Add(new IntegerValidator());
                    break;
                case AnswerFormat.Lowercase:
                    validators.Add(new LowerCaseAlphaValidator());
                    break;
                case AnswerFormat.Uppercase:
                    validators.Add(new UpperCaseAlphaValidator());
                    break;
                case AnswerFormat.SSN:
                    validators.Add(new SocialSecurityValidator());
                    break;
                case AnswerFormat.Money:
                    validators.Add(new USCurrencyValidator());
                    break;
                case AnswerFormat.Phone:
                    validators.Add(new USPhoneValidator());
                    break;
                case AnswerFormat.URL:
                    validators.Add(new UrlValidator());
                    break;
                case AnswerFormat.Date:
                    validators.Add(new DateValidator());
                    break;
                case AnswerFormat.Date_USA:
                    validators.Add(new USADateValidator());
                    break;
                case AnswerFormat.Date_ROTW:
                    validators.Add(new ROTWDateValidator());
                    break;
                case AnswerFormat.Custom:
                     validators.Add(new CustomFormatValidator(customFormatId));
                    break;
                default:
                    break;
            }

            return validators;
        }
    }
}
