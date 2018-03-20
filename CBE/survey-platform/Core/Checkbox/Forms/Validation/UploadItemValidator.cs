using System;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that the answer to an upload item is valid.
    /// </summary>
    public class UploadItemValidator : Validator<UploadItem>
    {
        /// <summary>
        /// Get/set the error message
        /// </summary>
        protected string ErrorMessage { get; set; }

        /// <summary>
        /// Get a language specific validation message.
        /// </summary>
        /// <param name="languageCode">Language code</param>
        /// <returns>Validation message</returns>
        public override string GetMessage(string languageCode)
        {
            return ErrorMessage;
        }

        /// <summary>
        /// Validate the upload item
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(UploadItem input)
        {
            ErrorMessage = string.Empty;

            if (!ValidateHasAnswer(input)) { return false; }

            return ValidateFileType(input);
        }

        /// <summary>
        /// Validates that the question answer is an allowed file type.
        /// No answer is considered a valid file type.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool ValidateFileType(UploadItem input)
        {
            if (!input.HasAnswer)
                return true;

            var fileType = input.FileType;
            
            if(string.IsNullOrWhiteSpace(fileType))
            {
                // this validation method requires that a fileType be set, however,
                // fileName and fileType are not set or retrieved from the database
                // when editing a response. Therefore, if this item has an answer
                // we need to parse the answer text for the file name and type
                var text = input.AnswerData.GetTextAnswerForItem(input.ID);

                var answerParts = text.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                var indexOfDot = answerParts[answerParts.Count() - 2].Trim().LastIndexOf('.'); ;

                fileType = answerParts[answerParts.Count() - 2].Substring(indexOfDot);
            }

            if (Utilities.IsNotNullOrEmpty(fileType))
            {
                if (input.AllowedFileTypes.Any(allowed => allowed.Equals(fileType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            ErrorMessage = TextManager.GetText("/validationMessages/text/uploadItemInvalidFileType", input.LanguageCode);
            return false;
        }

        /// <summary>
        /// Determines if the question has been answered and displays the appropriate error if it has not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual bool ValidateHasAnswer(UploadItem input)
        {
            if (! input.Required) { return true; }

            RequiredItemValidator validator = new RequiredItemValidator();

            if (!validator.Validate(input))
            {
                ErrorMessage = validator.GetMessage(input.LanguageCode);
                return false;
            }

            return true;
        }
    }
}