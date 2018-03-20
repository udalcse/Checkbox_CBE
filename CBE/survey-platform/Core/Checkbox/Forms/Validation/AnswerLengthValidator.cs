using System;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator to validate answer length
    /// </summary>
    public class AnswerLengthValidator : Validator<TextItem>
    {
        Nullable<int> _maxLength;
        Nullable<int> _minLength;

        private bool _minValidationIssue;
        private bool _maxValidationIssue;
        
        /// <summary>
        /// Get the max length, if any
        /// </summary>
        public Nullable<int> MaxLength
        {
            get { return _maxLength; }
        }

        public Nullable<int> MinLength
        {
            get { return _minLength; }
        }

        /// <summary>
        /// Get/set the validation text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            string message = null;

            if (_maxValidationIssue)
            {
                message = TextManager.GetText("/validationMessages/text/length", languageCode);
                if (!string.IsNullOrEmpty(message))
                    message = message.Replace("{max}", MaxLength.ToString());
            }
            else if (_minValidationIssue)
            {
                message = TextManager.GetText("/validationMessages/text/lengthMin", languageCode);
                if (!string.IsNullOrEmpty(message))
                    message = message.Replace("{min}", MinLength.ToString());
            }

            return message;
        }

        /// <summary>
        /// Validate the items length
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(TextItem input)
        {
            if (input.HasAnswer)
            {
                string answer = input.GetAnswer();

                if (answer != null)
                {
                    int? length = null;
                    if (input.MaxLength.HasValue)
                    {
                        length = GetAnswerLength(input, answer);
                        _maxLength = input.MaxLength.Value;
                        _maxValidationIssue = length > _maxLength;
                        if (_maxValidationIssue)
                            return false;
                    }
                    
                    if (input.MinLength.HasValue)
                    {
                        length = length ?? GetAnswerLength(input, answer);
                        _minLength = input.MinLength.Value;
                        _minValidationIssue = length < _minLength;
                        if (_minValidationIssue)
                            return false;
                    }
                }
            }

            return true;
        }

        private int? GetAnswerLength(TextItem input, string answer)
        {
            if (answer != null)
            {
                var multilineData = input as MultiLineTextBoxItem;
                if (multilineData != null && multilineData.IsHtmlFormattedData)
                {
                    var text = Utilities.StripHtml(answer);
                    return text.Length;
                }

                return answer.Length;
            }

            return null;
        }
    }
}
