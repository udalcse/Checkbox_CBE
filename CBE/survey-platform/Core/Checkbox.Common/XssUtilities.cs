using System.Text;

namespace Checkbox.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class XssUtilities
    {
        ///<summary>
        /// Sanitizes untrusted text which will be displayed to a client.
        ///</summary>
        ///<param name="input">The text to encode.</param>
        ///<returns></returns>
        public static string EncodeHtml(string input)
        {
            if (Utilities.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder output = new StringBuilder(string.Empty, input.Length * 2);

            foreach (char value in input)
            {
                if ((((value > '`') && (value < '{')) || ((value > '@') && (value < '['))) || (((value == ' ') || ((value > '/') && (value < ':'))) || (((value == '.') || (value == ',')) || ((value == '-') || (value == '_')))))
                {
                    output.Append(value);
                }
                else
                {
                    output.Append("&#" + ((int)value) + ";");
                }
            }

            return output.ToString();
        }
    }
}
