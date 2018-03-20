/****************************************************************************
 * Supports getting localized text in the specified language or in the      *
 * logged-in user's language.                                               *
 * **************************************************************************/

using System;
using System.Collections.Generic;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Web
{
    /// <summary>
    /// Static class for accessing text specific to a user's language
    /// </summary>
    public static class WebTextManager
    {
        /// <summary>
        /// Set the language for the current session
        /// </summary>
        /// <param name="languageCode"></param>
        public static void SetUserSessionLanguage(string languageCode)
        {
            System.Web.HttpContext.Current.Session["languageCode"] = languageCode;
        }

        /// <summary>
        /// Get the current user's language setting
        /// </summary>
        /// <returns></returns>
        public static string GetUserLanguage()
        {
            if (System.Web.HttpContext.Current.Session["languageCode"] != null)
            {
                return (string)System.Web.HttpContext.Current.Session["languageCode"];
            }

            CheckboxPrincipal p = UserManager.GetCurrentPrincipal();

            if (p != null) 
            {
                string language = p["Language"];

                if (!string.IsNullOrEmpty(language))
                {
                    return language;
                }
            }
            return TextManager.DefaultLanguage;
        }

        /// <summary>
        /// Get the specified string using the following values, which are checked in order...
        /// --Session Value
        /// --Logged-in user profile language
        /// --App default language
        /// </summary>
        /// <param name="textId">ID of the string to get.</param>
        /// <returns></returns>
        public static string GetText(string textId)
        {
            return GetText(textId, GetUserLanguage(), string.Empty);
        }

        /// <summary>
        /// Get the specified text in the specified language
        /// </summary>
        /// <param name="textId">Identifier of text to retrieve</param>
        /// <param name="languageCode">Language to get text string in.</param>
        /// <returns>Localized string</returns>
        public static string GetText(string textId, string languageCode)
        {
            return GetText(textId, languageCode, string.Empty);
        }

        /// <summary>
        /// Get the specified text in the specified language
        /// </summary>
        /// <param name="textId">Identifier of text to retrieve</param>
        /// <param name="languageCode">Language to get text string in. If it is null - current user`s language will be used.</param>
        /// <param name="defaultText">Default text to return when text is not found.</param>
        /// <returns>Localized string</returns>
        /// <remarks>If language debug mode is enabled when get text is called, textid is returned instead of default text
        /// for missing text.</remarks>
        public static string GetText(string textId, string languageCode, string defaultText)
        {
            if (ApplicationManager.AppSettings.LanguageDebugMode)
            {
                defaultText = textId;
            }

            if (Utilities.IsNullOrEmpty(ref textId))
            {
                return defaultText;
            }
            if (Utilities.IsNullOrEmpty(ref languageCode))
            {
                languageCode = GetUserLanguage();
            }

            return TextManager.GetText(textId, languageCode, defaultText);
        }

        /// <summary>
        /// Get an array containing the available langauges for the application
        /// </summary>
        public static string[] ApplicationLanguages
        {
            get { return TextManager.ApplicationLanguages; }
        }

        /// <summary>
        /// Get an array containing the available survey langauges for the application
        /// </summary>
        public static string[] SurveyLanguages
        {
            get { return TextManager.SurveyLanguages; }
        }

        /// <summary>
        /// Get a dictionary of survey language codes/texts
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetSurveyLanguagesDictionary()
        {
            Dictionary<string, string> languageDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string language in SurveyLanguages)
            {
                languageDictionary[language] = GetText("/languageText/surveyEditor/" + language, GetUserLanguage(), language);
            }

            return languageDictionary;
        }

        /// <summary>
        /// Get a sorted list of survey languages
        /// </summary>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetSortedSurveyLanguages()
        {
            return GetSortedSurveyLanguages(GetUserLanguage());
        }

        /// <summary>
        /// Get a sorted list of survey languages
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetSortedSurveyLanguages(string languageCode)
        {
            return SortLanguages(SurveyLanguages, languageCode);
        }

        /// <summary>
        /// Get a sorted list of survey languages
        /// </summary>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetSortedApplicationLanguages()
        {
            return GetSortedApplicationLanguages(null);
        }

        /// <summary>
        /// Get a sorted list of survey languages
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetSortedApplicationLanguages(string languageCode)
        {
            return SortLanguages(ApplicationLanguages, languageCode);
        }


        /// <summary>
        /// Sort languages
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> SortLanguages(string[] languages, string languageCode)
        {
            SortedDictionary<string, string> sortedList = new SortedDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string language in languages)
            {
                string languageText;

                if (languageCode != null && languageCode.Trim() != string.Empty)
                {
                    languageText = TextManager.GetText("/languageText/" + language, languageCode);
                }
                else
                {
                    languageText = TextManager.GetText("/languageText/" + language, language);
                }

                if (languageText == null || languageText.Trim() == string.Empty)
                {
                    languageText = language;
                }

                sortedList[languageText] = language;
            }

            return sortedList;
        }
    }
}