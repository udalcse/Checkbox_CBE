using System.Linq;
using System.Collections.Generic;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    public partial class SPSSKey : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get/set survey id
        /// </summary>
        [QueryParameter("s", IsRequired = true)]
        public int? SurveyId { get; set; }

        [QueryParameter]
        public bool IncludeOpenEnded { get; set; }

        protected string LanguageCode { get; private set; }
        protected int ItemCount { get; set; }

        /// <summary>
        /// Get survey
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null
                    && SurveyId.HasValue)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId.Value);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Get the controllable entity
        /// </summary>
        protected override IAccessControllable GetControllableEntity() { return ResponseTemplate; }

        /// <summary>
        /// Get the required permission
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Analysis.Responses.Export"; } }


        /// <summary>
        /// Initialize the data
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Master.HideDialogButtons();

            ItemCount = 0;

            var languages = new List<string>(ResponseTemplate.LanguageSettings.SupportedLanguages);

            LanguageCode = languages.Contains(WebTextManager.GetUserLanguage()) 
                ? WebTextManager.GetUserLanguage() 
                : ResponseTemplate.LanguageSettings.DefaultLanguage;
        }

        /// <summary>
        /// Get the text for an item
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        protected string GetItemText(ItemData itemData)
        {
            ItemTextDecorator decorator = itemData.CreateTextDecorator(LanguageCode);
            string text = string.Empty;

            if (decorator is LabelledItemTextDecorator)
            {
                text = ((LabelledItemTextDecorator)decorator).Text;

                if (text == null || text.Trim() == string.Empty)
                {
                    text = ((LabelledItemTextDecorator)decorator).SubText;
                }
            }

            if (string.IsNullOrEmpty(text))
                text = itemData.ID.ToString();

            return text;
        }

        /// <summary>
        /// Get the text for an item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GetOptionText(SelectItemData itemData, ListOptionData option)
        {
            var decorator = (SelectItemTextDecorator)itemData.CreateTextDecorator(LanguageCode);

            string text;
            if (option.IsOther)
                text = decorator.OtherText;
            else if (option.IsNoneOfAbove)
                text = decorator.NoneOfAboveText;
            else
                text = decorator.GetOptionText(option.Position);

            if (string.IsNullOrEmpty(text))
                text = option.OptionID.ToString();

            return text;
        }
    }
}