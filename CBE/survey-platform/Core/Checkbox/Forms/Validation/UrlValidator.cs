using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate that an expression is a URL
    /// </summary>
    public class UrlValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Constructor.  Set the regular expression
        /// </summary>
        public UrlValidator()
        {
            _regex = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            //_regex = @"(((ht|f)tp(s?):\/\/)|(www\.[^ \[\]\(\)\n\r\t]+)|(([012]?[0-9]{1,2}\.){3}[012]?[0-9]{1,2})\/)([^ \[\]\(\),;""'<>\n\r\t]+)([^\. \[\]\(\),;""'<>\n\r\t])|(([012]?[0-9]{1,2}\.){3}[012]?[0-9]{1,2})";
        }

        /// <summary>
        /// Get the error message to display on validation failure
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/regex/url", languageCode);
        }

        /// <summary>
        /// Validate the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            System.Globalization.CompareInfo cmpUrl = System.Globalization.CultureInfo.InvariantCulture.CompareInfo;
            if (!cmpUrl.IsPrefix(input, "http://") && !cmpUrl.IsPrefix(input, "https://"))
                input = "http://" + input;

            return base.Validate(input);
        }
    }
}
