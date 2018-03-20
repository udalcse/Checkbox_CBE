using System;
using System.Collections.Generic;
using Checkbox.Common;

namespace Checkbox.Forms.Data
{

    /// <summary>
    /// Lightweight container to store meta data information for item options contained in a survey being reported on.
    /// </summary>
    /// <remarks><see cref="Checkbox.Forms.Items.Configuration.ItemData"/> and <see cref="Checkbox.Forms.Items.Configuration.ListData" /> objects are fairly
    /// heavyweight and often contain internal <see cref="System.Data.DataSet"/> to store their data.  Since report data may be cached, and therefore serialized in
    /// multi-machine/process environments, this serves as a more efficient container class.</remarks>
    [Serializable]
    public class LightweightOptionMetaData
    {
        private Dictionary<string, string> _textDictionary;

        /// <summary>
        /// Text dictionary
        /// </summary>
        private Dictionary<string, string> TextDictionary
        {
            get { return _textDictionary ?? (_textDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)); }
        }

        /// <summary>
        /// Get/set the database Id of the item the option is associated with.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Get/set the id of the option.
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// Option position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Get/set the text of the option.
        /// </summary>
        public string GetText(string languageCode)
        {
            if (TextDictionary.ContainsKey(languageCode))
            {
                return TextDictionary[languageCode];
            }

            return string.Empty;
        }

        /// <summary>
        /// Set option text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetText(string languageCode, string text)
        {
            TextDictionary[languageCode] = text;
        }

        /// <summary>
        /// Get/set the point value of the option.
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Get/set whether the option is an "other" option or not.
        /// </summary>
        public bool IsOther { get; set; }

        /// <summary>
        /// Get/set the option alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Get the text or alias of the option, depending in the <paramref name="preferAlias"/> parameter value.  If the text is requested, but not found
        /// the method will fall back to returning the alias and vice versa.
        /// </summary>
        /// <param name="preferAlias">Indicate whether the alias is the prefered value to return.</param>
        /// <param name="languageCode">Language for text.</param>
        /// <returns>Option text or alias as indicated by <paramref name="preferAlias"/> parameter value.</returns>
        public string GetText(bool preferAlias, string languageCode)
        {
            if (preferAlias && Utilities.IsNotNullOrEmpty(Alias))
            {
                return Alias;
            }

            String text = GetText(languageCode);

            return String.IsNullOrEmpty(text) ? (Alias ?? String.Empty) : text;
        }
    }
}
