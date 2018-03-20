using System.Web.UI.WebControls;
using Checkbox.Wcf.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls
{
    public partial class LanguageSelect : Checkbox.Web.Forms.TakeSurvey.LanguageSelectBase
    {
        protected override Button SelectLanguageBtn
        {
            get { return _selectLanguageBtn; }
        }

        protected override DropDownList SurveyLanguageList
        {
            get { return _surveyLanguageList; }
        }

        protected override Label SelectLanguageLbl
        {
            get { return _selectLanguageLbl; }
        }

        protected override string GetSelectLanguageText(string language)
        {
            return SurveyEditorServiceImplementation.GetSurveyText("LOGIN_SELECTLANGUAGE", ResponseTemplateId, language, _availableLanguages);
        }

        protected override string GetMiscContinueText(string language)
        {
            return SurveyEditorServiceImplementation.GetSurveyText("MISC_CONTINUE", ResponseTemplateId, language, _availableLanguages);
        }
    }
}