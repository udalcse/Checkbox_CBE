using System.Collections.Generic;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Container for language/text related survey settings
    /// </summary>
    [System.Serializable]
    public class SurveyLanguageSettings
    {
        private List<string> _supportedLanguages;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        public int SurveyId{get;set;}

        /// <summary>
        /// Get/set text id prefix for associated template
        /// </summary>
        public string TextIdPrefix{get;set;}

        /// <summary>
        /// Gets or sets a string array of the languages supported by this <see cref="ResponseTemplate"/>
        /// </summary>
        public List<string> SupportedLanguages
        {
            get { return _supportedLanguages ?? (_supportedLanguages = new List<string>()); }

            set { _supportedLanguages = value; }
        }

        /// <summary>
        /// Gets or sets a string indicating the default language for this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <remarks>
        /// Expects the format ISO 639-1 language code with the corresponding ISO 3166 region code, e.g., en-US
        /// </remarks>
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Gets or sets the source from which to set the Respondent language
        /// </summary>
        public string LanguageSource { get; set; }

        /// <summary>
        /// Gets or sets the variable name for the language setting within the <see cref="LanguageSource"/>
        /// </summary>
        public string LanguageSourceToken { get; set; }

        /// <summary>
        /// Get localized name of survey
        /// </summary>
        public string GetLocalizedName(string languageCode)
        {
            return TextManager.GetText(NameTextId, languageCode, string.Empty, SupportedLanguages.ToArray());
        }

        /// <summary>
        /// Set localized version of survey name
        /// </summary>
        /// <param name="nameText"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public void SetLocalizedName(string nameText, string languageCode)
        {
            TextManager.SetText(NameTextId, languageCode, nameText);
        }

        /// <summary>
        /// Get page number
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public string GetPageNumber(string languageCode, int pageNumber, int pageCount)
        {
            if (pageNumber > pageCount)
                return TextManager.GetText("/pageText/survey.aspx/page", languageCode, "Page") + " " + pageNumber.ToString();

            return string.Format(TextManager.GetText("/pageText/survey.aspx/pageNumber", languageCode, "Page {0} of {1}.", SupportedLanguages.ToArray()), pageNumber, pageCount);
        }



        /// <summary>
        /// Gets the TextID for this ResponseTemplate's Name
        /// </summary>
        public string NameTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/name";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Title
        /// </summary>
        public string TitleTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/title";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Description
        /// </summary>
        public string DescriptionTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/description";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Continue button
        /// </summary>
        public string ContinueButtonTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/continue";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Back button
        /// </summary>
        public string BackButtonTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/back";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Finish button
        /// </summary>
        public string FinishButtonTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/finish";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get the text id for the save progress button
        /// </summary>
        public string SaveProgressButtonTextId
        {
            get
            {
                if (SurveyId > 0)
                {
                    return "/" + TextIdPrefix + "/" + SurveyId + "/saveProgress";
                }

                return string.Empty;
            }
        }
    }
}
