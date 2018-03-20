namespace Checkbox.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Adds the double quotes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string AddDoubleQuotes(this string  value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return string.Format("\"{0}\"",value);
        }
    }
}
