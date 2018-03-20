using System;
using System.Text;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Encapsulate emailing a response to a survey
    /// </summary>
    [Serializable]
    public class EmailResponseItem : EmailItem
    {
        //Flag to prevent infinite recursion when serializing response, which includes self
        private bool _gettingBodyText;

        private bool _includeMessageItems;
        private bool _showPageNumbers;
        private bool _showQuestionNumbers;
        private bool _showHiddenItems;
        private bool _includeResponseDetails;

        /// <summary>
        /// Get the body text
        /// </summary>
        /// <returns></returns>
        protected override string GetBodyText()
        {
            var availableTerms = ResponseTemplateManager.GetResponseTemplate(Response.ResponseTemplateID);

            if (_gettingBodyText)
            {
                return string.Empty;
            }
            
            _gettingBodyText = true;

            StringBuilder sb = new StringBuilder();
            sb.Append(base.GetBodyText());
            sb.Append(GetPipedText("MessageBody", ResponseFormatter.Format(Response, MessageFormat,
                _includeResponseDetails, _showPageNumbers, _showQuestionNumbers, _includeMessageItems, _showHiddenItems, true, availableTerms.SurveyTerms)));

            _gettingBodyText = false;

            var text = sb.ToString();

            switch (MessageFormat)
            {
                case "Html":
                    text = Utilities.EncodeTagsInHtmlContent(text);
                    break;
                case "Text":
                    text = Utilities.DecodeAndStripHtml(text);
                    break;
            }

            return text;
        }

        /// <summary>
        /// Generate the message body
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string GetdMessageBody(string format)
        {
            if (!_gettingBodyText)
            {
                return GetPipedText(
                    "Body",
                    ResponseFormatter.Format(Response, format, _includeResponseDetails, _showPageNumbers, _showQuestionNumbers, _includeMessageItems, _showHiddenItems));
            }
            
            return string.Empty;
        }

        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Visible = ExportMode != ExportMode.Pdf;
            ArgumentValidation.CheckExpectedType(configuration, typeof(EmailResponseItemData));
            var config = (EmailResponseItemData)configuration;

            _includeMessageItems = config.IncludeMessageItems;
            _showPageNumbers = config.ShowPageNumbers;
            _showQuestionNumbers = config.ShowQuestionNumbers;
            _includeResponseDetails = config.IncludeResponseDetails;
            _showHiddenItems = config.ShowHiddenItems;

            base.Configure(configuration, languageCode, templateId);
        }
    }
}
