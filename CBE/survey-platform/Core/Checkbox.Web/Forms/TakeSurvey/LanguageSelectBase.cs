using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;

namespace Checkbox.Web.Forms.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LanguageSelectBase : Common.UserControlBase
    {
        #region Abstract Members

        protected abstract Button SelectLanguageBtn { get; }
        protected abstract DropDownList SurveyLanguageList { get; }
        protected abstract Label SelectLanguageLbl { get; }

        protected abstract string GetSelectLanguageText(string language);
        protected abstract string GetMiscContinueText(string language);

        #endregion

        protected string _defaultLanguage;
        protected string[] _availableLanguages;

        /// <summary>
        /// 
        /// </summary>
        public string SelectedLanguage
        {
            get
            {
                return Request[SurveyLanguageList.UniqueID] ?? _defaultLanguage;
            }
        }

        public override string UniqueID { get { return ID; } }

        /// <summary>
        /// 
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableLanguages"></param>
        /// <param name="defaultLanguage"></param>
        /// <param name="responseTemplateId"></param>
        public void Initialize(List<string> availableLanguages, string defaultLanguage, int responseTemplateId)
        {
            ResponseTemplateId = responseTemplateId;
            _availableLanguages = availableLanguages != null ? availableLanguages.ToArray() : new string[]{};
            _defaultLanguage = defaultLanguage;

            SurveyLanguageList.Items.Add(new ListItem(WebTextManager.GetText("/languageText/" + defaultLanguage, TextManager.DefaultLanguage, defaultLanguage), defaultLanguage));

            foreach (var availableLanguage in _availableLanguages.Where(language => !language.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase)))
            {
                SurveyLanguageList.Items.Add(new ListItem(WebTextManager.GetText("/languageText/" + availableLanguage, TextManager.DefaultLanguage, availableLanguage), availableLanguage));
            }

            SurveyLanguageList.SelectedValue = SelectedLanguage;

            if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
                SelectLanguageBtn.OnClientClick = "return false;";

            SurveyLanguageList.Attributes["data-action"] = "none";
            SelectLanguageBtn.Attributes["data-action"] = "language";

            UpdateText(SelectedLanguage);
        }

        protected void UpdateText(string language)
        {
            string buttonText = GetMiscContinueText(language);
            if (string.IsNullOrEmpty(buttonText))
                buttonText = TextManager.GetText("/pageText/takeSurvey.aspx/continue");

            SelectLanguageBtn.Text = buttonText;
            SelectLanguageLbl.Text = GetSelectLanguageText(language);
        }
    }
}
