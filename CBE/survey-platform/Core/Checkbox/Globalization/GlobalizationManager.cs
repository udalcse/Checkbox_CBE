using System;
using System.Globalization;
using Checkbox.Globalization.Configuration;
using Prezza.Framework.Configuration;

namespace Checkbox.Globalization
{
    ///<summary>
    ///</summary>
    public static class GlobalizationManager
    {
        private static readonly GlobalizationConfiguration _config;

        static GlobalizationManager()
        {
            _config = (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration");
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static string GetDatePickerLocalizationFile()
        {
            string culture = CultureInfo.CurrentCulture.ToString();
            if (_config.DatePickerLocalizationFiles.ContainsKey(culture))
                return _config.DatePickerLocalizationFiles[culture];

            return _config.DatePickerLocalizationFiles[_config.DatePickerDefaultLocalization];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatTheDate(DateTime date)
        {
            string format = "{0:" + GetDateFormat() + " " + GetTimeFormat() + "}";
            if (!format.Contains("HH") && format.Contains("H"))
                format = format.Replace("H", "HH");
            if (!format.Contains("MM") && format.Contains("M"))
                format = format.Replace("M", "MM");
            if (!format.Contains("dd") && format.Contains("d"))
                format = format.Replace("d", "dd");

            return string.Format(format, date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDateFormat()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetTimeFormat()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
        }
    }
}
