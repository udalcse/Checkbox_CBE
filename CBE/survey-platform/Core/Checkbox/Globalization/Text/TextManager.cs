/****************************************************************************
 * TextManager.cs                                                           *
 * Provides routines for accessing strings used by Checkbox.                *
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Checkbox.Common;
using Checkbox.Globalization.Configuration;
using Checkbox.Management;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Caching;

namespace Checkbox.Globalization.Text
{
	/// <summary>
	/// Provides static routines for accessing strings used by Checkbox.
	/// </summary>
	public static class TextManager
	{
        /// <summary>
        /// Cache Manager
        /// </summary>
        static CacheManager _cacheManager;

        /// <summary>
        /// Static constructor
        /// </summary>
        static TextManager()
        {
            _cacheManager = CacheFactory.GetCacheManager();
        }

        /// <summary>
        /// Get an array of <see cref="ISOCode"/> objects representing available app. languages
        /// </summary>
        public static string[] ApplicationLanguages
        {
            get
            {
                if (MultiLanguageEnabled)
                {
                    return TextFactory.ApplicationLanguages;
                }
                
                return new[] { DefaultLanguage };
            }
        }

        /// <summary>
        /// Get an array of <see cref="ISOCode"/> objects representing available survey languages
        /// </summary>
        public static string[] SurveyLanguages
        {
            get
            {
                if (MultiLanguageEnabled)
                {
                    return TextFactory.SurveyLanguages;
                }
                
                return new[] { DefaultLanguage };
            }
        }

        /// <summary>
        /// Get the default application language
        /// </summary>
        public static string DefaultLanguage
        {
            get { return TextFactory.DefaultLanguage; }
        }

        /// <summary>
        /// Get whether multi-language modules is installed
        /// </summary>
        public static bool MultiLanguageEnabled
        {
            get
            {
                return (TextFactory.GetTextProvider() is IImportExportTextProvider) && ApplicationManager.AppSettings.AllowMultiLanguage;
            }
        }

		/// <summary>
		/// Retrieve the specified text.
		/// </summary>
		/// <param name="textIdentifier">String identifying the text to retrieve</param>
		/// <param name="languageCode">Language code for the string.  Format is 2 letter ISO 639 language code concatenated with the 2 letter ISO 3166 region code.  e.g. en-us</param>
		/// <returns></returns>
		public static string GetText(string textIdentifier, string languageCode)
		{
			//ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");
            if (Utilities.IsNullOrEmpty(textIdentifier) || Utilities.IsNullOrEmpty(languageCode))
            {
                return string.Empty;
            }

			return TextFactory.GetTextProvider().GetText(textIdentifier, languageCode);
		}

		/// <summary>
		/// Retrieve the specified text for default language.
		/// </summary>
		/// <param name="textIdentifier">String identifying the text to retrieve</param>
		/// <returns></returns>
		public static string GetText(string textIdentifier)
		{
			//ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");
			if (Utilities.IsNullOrEmpty(textIdentifier))
			{
				return string.Empty;
			}

			return TextFactory.GetTextProvider().GetText(textIdentifier, DefaultLanguage);
		}

        /// <summary>
        /// Retrieve specified text, falling back on alternate languages and finally default text if necessary.
        /// </summary>
        /// <param name="textIdentifier">Text ID</param>
        /// <param name="languageCode">Language to try.</param>
        /// <param name="defaultText">Default text to return in case of no text found.</param>
        /// <param name="alternateLanguages">Alternate languages to check.</param>
        /// <returns>String</returns>
        public static string GetText(string textIdentifier, string languageCode, string defaultText, params string[] alternateLanguages)
        {
            if (Utilities.IsNullOrEmpty(textIdentifier) || Utilities.IsNullOrEmpty(languageCode))
            {
                return defaultText;
            }

            string text = TextFactory.GetTextProvider().GetText(textIdentifier, languageCode);

            if (Utilities.IsNullOrEmpty(text))
            {
                foreach (string altLanguage in alternateLanguages)
                {
                    text = TextFactory.GetTextProvider().GetText(textIdentifier, altLanguage);

                    if (Utilities.IsNotNullOrEmpty(text))
                    {
                        return text;
                    }
                }

                return defaultText;
            }

            return text;
        }

        /// <summary>
        /// Get a data table containing all texts with the given text identifier
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public static DataTable GetTextData(string textIdentifier)
        {
            return TextFactory.GetTextProvider().GetTextData(textIdentifier);
        }

        /// <summary>
        /// Get a dictionary containing all texts with the given text identifier
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllTexts(string textIdentifier)
        {
            return TextFactory.GetTextProvider().GetAllTexts(textIdentifier);
        }

        /// <summary>
        /// Get a data table containing all matching texts.  Return table contains three columns: TextId, LanguageCode, and TextValue
        /// </summary>
        /// <param name="matchExpressions">Parameter array of regular expressions to match</param>
        /// <returns></returns>
        public static DataTable GetAllMatchingTexts(params string[] matchExpressions)
        {
            return TextFactory.GetTextProvider().GetMatchingTextData(matchExpressions);
        }
        
		/// <summary>
		/// Set the specified text string
		/// </summary>
		/// <param name="textIdentifier"></param>
		/// <param name="languageCode"></param>
		/// <param name="textValue"></param>
		public static void SetText(string textIdentifier, string languageCode, string textValue)
		{
			ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");
			ArgumentValidation.CheckForEmptyString(languageCode, "languageCode");

            if (textValue == null)
            {
                textValue = string.Empty;
            }

			TextFactory.GetTextProvider().SetText(textIdentifier, languageCode, textValue);			
		}

		/// <summary>
		/// Set the specified text string for default language
		/// </summary>
		/// <param name="textIdentifier"></param>
		/// <param name="textValue"></param>
		public static void SetText(string textIdentifier, string textValue)
		{
			ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

			if (textValue == null)
			{
				textValue = string.Empty;
			}

			TextFactory.GetTextProvider().SetText(textIdentifier, DefaultLanguage, textValue);
		}

	    /// <summary>
	    /// Logic common to all ExportText scenarios.
	    /// </summary>
	    /// <param name="writer"></param>
	    /// <returns></returns>
	    private static ITextProvider ExportTextBase(TextWriter writer) 
	    {
	        ArgumentValidation.CheckForNullReference(writer, "reader");

	        ITextProvider provider = TextFactory.GetTextProvider();

	        if (!MultiLanguageEnabled)
	        {
	            throw new Exception("Unable to export.  Text provider does not support exporting data.");
	        }
	        return provider;
	    }

	    /// <summary>
        /// Write all texts to the text writer.
        /// </summary>
        /// <param name="writer"></param>
        public static void ExportAllTexts(TextWriter writer)
        {
            ITextProvider provider = ExportTextBase(writer);
            ((IImportExportTextProvider)provider).ExportAllTexts(writer);
        }

        /// <summary>
        /// Write all matching texts to the text writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="languageCodes">Language codes to export texts for.  If null or length == 0, list of application languages will be used.</param>
        /// <param name="partialTextId">A list of partial text ids used to filter the exported text. </param>
        public static void ExportFilteredTexts(TextWriter writer, string[] languageCodes, params string[] partialTextId)
        {
            ITextProvider provider = ExportTextBase(writer);
            ((IImportExportTextProvider)provider).ExportFilteredTexts(writer, languageCodes, partialTextId);
        }

        /// <summary>
        /// Write all specified texts to the text writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="languageCodes">Language codes to export texts for.  If null or length == 0, list of application languages will be used.</param>
        /// <param name="textIds">The list of full text ids to export.</param>
        public static void ExportTextsById(TextWriter writer, string[] languageCodes, params string[] textIds)
        {
            ITextProvider provider = ExportTextBase(writer);
            ((IImportExportTextProvider)provider).ExportTextsById(writer, languageCodes, textIds);
        }

	    /// <summary>
        /// Import texts from the specified reader.
        /// </summary>
        /// <param name="reader"></param>
		public static void ImportTexts(TextReader reader)
		{
			ArgumentValidation.CheckForNullReference(reader, "reader");

			ITextProvider provider = TextFactory.GetTextProvider();

            if (!MultiLanguageEnabled)
			{
				throw new Exception("Unable to import.  Text provider does not support exporting data.");
			}

			((IImportExportTextProvider)provider).ImportTexts(reader);

            clearPageTextIDsCache();
		}


        /// <summary>
        /// Get localized values for enumerated types
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumLocalizedValues(Type enumType, string languageCode)
        {
            List<string> enumStringValues = Utilities.ListEnumValues(enumType);
            var localizedNames = new Dictionary<string, string>();

            foreach (string enumStringValue in enumStringValues)
            {
                localizedNames[enumStringValue] = GetText("/enum/" + enumType.Name + "/" + enumStringValue, languageCode);
            }

            return localizedNames;
        }



	    /// <summary>
	    /// List texts associated with an application page.
	    /// </summary>
	    /// <param name="pagePath"></param>
	    /// <param name="languageCode"></param>
	    /// <returns></returns>
	    public static Dictionary<string, string> GetPageTexts(string pagePath, string languageCode)
        {
            //try to locate prepared texts from the cache
            var key = string.Format("PageTexts_{0}_{1}", pagePath, languageCode);

            var pageTexts = _cacheManager.GetData(key) as Dictionary<string, string>;
            if (pageTexts != null)
                return pageTexts;
            
            pageTexts = new Dictionary<string, string>();

            var pageId = ApplicationManager.GetPageId(pagePath);

            if (pageId <= 0)
            {
                return pageTexts;
            }

            //Load all text ids
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_PageTextIds_List");
            command.AddInParameter("PageId", DbType.Int32, pageId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string textId = DbUtility.GetValueFromDataReader(reader, "TextId", string.Empty);

                        if (!string.IsNullOrEmpty(textId))
                        {
                            pageTexts[textId] = string.Empty;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

	        var keyList = new List<string>(pageTexts.Keys);

            //Get text values
            foreach (var textId in keyList)
            {
                pageTexts[textId] = GetText(textId, languageCode);
            }

            _cacheManager.Add(key, pageTexts);
            addCachedKey(key);

            return pageTexts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        private static void addCachedKey(string key)
        {
            var keys = _cacheManager.GetData("PageKeys") as List<string>;
            if (keys == null)
            {
                keys = new List<string>();
            }
            keys.Add(key);
            _cacheManager.Add("PageKeys", keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        private static void clearPageTextIDsCache()
        {
            var keys = _cacheManager.GetData("PageKeys") as List<string>;
            if (keys == null)
            {
                return;
            }
            foreach (var k in keys)
            {
                _cacheManager.Remove(k);
            }
        }
        /// <summary>
        /// Clears the configuration cache
        /// </summary>
        public static void RefreshGlobalizationConfiguration()
        {
            TextFactory.RefreshGlobalizationConfiguration();
        }

        /// <summary>
        ///  Adds an application language to the cache
        /// </summary>
        /// <param name="language"></param>
        public static void AddApplicationLanguage(string language)
        {
            TextFactory.AddApplicationLanguage(language);
        }

        /// <summary>
        /// Removes an application language from the cache
        /// </summary>
        /// <param name="language"></param>
        public static void RemoveApplicationLanguage(string language)
        {
            TextFactory.RemoveApplicationLanguage(language);
        }

        /// <summary>
        ///  Adds an survey language to the cache
        /// </summary>
        /// <param name="language"></param>
        public static void AddSurveyLanguage(string language)
        {
            TextFactory.AddSurveyLanguage(language);
        }

        /// <summary>
        /// Removes an survey language from the cache
        /// </summary>
        /// <param name="language"></param>
        public static void RemoveSurveyLanguage(string language)
        {
            TextFactory.RemoveSurveyLanguage(language);
        }

    }
}
