using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Globalization.Text
{
    /// <summary>
    /// Comparer to compare two language codes based on their localized texts
    /// </summary>
    public class LanguageTextComparer : IComparer<string>
    {
        private string _languageCode;

        #region IComparer<string> Members

        /// <summary>
        /// Constructor that accepts the language code to use to retrieve the language text
        /// </summary>
        /// <param name="languageCode"></param>
        public LanguageTextComparer(string languageCode)
        {
            _languageCode = languageCode;
        }

        /// <summary>
        /// Compare the two strings
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            string xLocalized = TextManager.GetText("/languageText/" + x, _languageCode);
            string yLocalized = TextManager.GetText("/languageText/" + y, _languageCode);

            if (xLocalized == null || xLocalized.Trim() == string.Empty)
            {
                xLocalized = x;
            }

            if (yLocalized == null || yLocalized.Trim() == string.Empty)
            {
                yLocalized = y;
            }

            return string.Compare(xLocalized, yLocalized);
        }

        #endregion
    }
}
