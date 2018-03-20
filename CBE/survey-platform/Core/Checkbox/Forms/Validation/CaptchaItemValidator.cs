
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate captcha item answers
    /// </summary>
    public class CaptchaItemValidator : Validator<CaptchaItem>
    {
        private string _errorMessageTextId;

        /// <summary>
        /// Get the captcha error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText(_errorMessageTextId, languageCode);
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(CaptchaItem input)
        {
            if (!input.HasAnswer)
            {
                _errorMessageTextId = "/validationMessages/captchaRequired";
                return false;
            }
            
            if (!input.Code.Equals(input.GetAnswer(), System.StringComparison.InvariantCultureIgnoreCase))
            {
                _errorMessageTextId = "/validationMessages/captchaAnswerDoesNotMatch";
                return false;
            }
            
            _errorMessageTextId = string.Empty;
            return true;
        }
    }
}

