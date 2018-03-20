/****************************************************************************
 * ITextProvider.cs                                                         *
 * Interface definition for text providers                                  *
 ****************************************************************************/

using System.Data;
using System.Collections.Generic;

using Prezza.Framework.Configuration;

namespace Checkbox.Globalization.Text
{
	/// <summary>
	/// Text provider supports getting/setting localizable texts
	/// </summary>
	public interface ITextProvider : IConfigurationProvider
	{
		/// <summary>
		/// Get the specified text string.
		/// </summary>
		/// <param name="textIdentifier">Identifier of the text string.</param>
		/// <param name="languageCode">Language code for the text string.  This is the concatenation of the two letter ISO 639-1 language code with the
		/// two letter ISO 3166 region code.  e.g.  en-US.</param>
		/// <returns>Specified string. NULL if the string is not found.</returns>
		string GetText(string textIdentifier, string languageCode);

        /// <summary>
        /// Gets all text strings for a specified textID in a key/value dictionary where the languagecode is the key and the text is the value
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        Dictionary<string, string> GetAllTexts(string textIdentifier);

        /// <summary>
        /// Gets a DataTable of strings selected from the Text table
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        DataTable GetTextData(string textIdentifier);

        /// <summary>
        /// Get a datatable of strings selected from the text table with textids that
        /// match the supplied regular expression.
        /// The DataTable has three columns: TextId, LanguageCode, TextValue
        /// </summary>
        /// <param name="matchExpressions"></param>
        /// <returns></returns>
        DataTable GetMatchingTextData(params string[] matchExpressions);

		/// <summary>
		/// Set the value of the specified string.
		/// </summary>
		/// <param name="textIdentifier">Identifier of the text string.</param>
		/// <param name="languageCode">Language code for the text string.  This is the concatenation of the two letter ISO 639-1 language code with the
		/// two letter ISO 3166 region code.  e.g.  en-US.</param>
		/// <param name="textValue">Text string to store</param>
		void SetText(string textIdentifier, string languageCode, string textValue);
	}
}
