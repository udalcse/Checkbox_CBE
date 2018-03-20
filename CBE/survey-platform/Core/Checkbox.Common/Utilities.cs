using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml;
using Ionic.Zip;
using Ionic.Zlib;
using CompressionMode = System.IO.Compression.CompressionMode;
using System.Web;
using System.Web.UI.WebControls;

namespace Checkbox.Common
{
    /// <summary>
    /// Summary description for Utilities.
    /// </summary>
    public static class Utilities
    {
        private static readonly Random _random;
        static Random random = new Random();
        /// <summary>
        /// Constructor to initialize random number generator
        /// </summary>
        static Utilities()
        {
            lock (typeof(Utilities))
            {
                _random = new Random();
            }
        }

        /// <summary>
        /// Get a random number
        /// </summary>
        /// <param name="minValue">INCLUSIVE min value</param>
        /// <param name="maxValue">EXCLUSIVE max value</param>
        /// <returns></returns>
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Randomizes the values in a generic list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The randomized List</returns>
        public static List<T> RandomizeList<T>(List<T> list)
        {
            var possibleIndexes = new List<int>();
            var newList = new List<T>();

            int count = list.Count;

            //Build the list of possible indexes
            for (int i = 0; i < count; i++)
            {
                possibleIndexes.Add(i);
            }

            //Now randomly choose them
            for (int i = 0; i < count; i++)
            {
                int nextIndex = GetRandomNumber(0, possibleIndexes.Count);
                newList.Add(list[possibleIndexes[nextIndex]]);

                possibleIndexes.RemoveAt(nextIndex);
            }

            return newList;
        }

        /// <summary>
        /// Converts a string to a <see cref="Color"/>
        /// </summary>
        /// <param name="colorString">the string to convert</param>
        /// <param name="throwException">Allow avoidance of exception rethrowing</param>
        /// <returns>Color if input color is valid or White if invalid and throwException == false.</returns>
        public static Color GetColor(string colorString, bool throwException)
        {
            return GetColor(colorString, 100, throwException);
        }

        /// <summary>
        /// Converts a string to a <see cref="Color"/>
        /// </summary>
        /// <param name="alphaValue">Alpha value for color as a percentage, where 100 = opaque.</param>
        /// <param name="colorString">the string to convert</param>
        /// <param name="throwException">Allow avoidance of exception rethrowing</param>
        /// <returns>Color if input color is valid or White if invalid and throwException == false.</returns>
        public static Color GetColor(string colorString, int alphaValue, bool throwException)
        {
            try
            {
                //Sanitize the alpha value
                if (alphaValue < 0)
                {
                    alphaValue = 0;
                }

                if (alphaValue > 100)
                {
                    alphaValue = 100;
                }

                double newAlpha = alphaValue * 2.55;

                //Get color w/out alpha
                Color c = ColorTranslator.FromHtml(colorString);

                //Apply alpha
                return Color.FromArgb(Convert.ToInt32(newAlpha), c);
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }

                return Color.Black;
            }
        }

        /// <summary>
        /// Encodes a string for use in an SQL statement
        /// </summary>
        /// <param name="s">The string to encode</param>
        /// <returns>The encoded string</returns>
        public static string SqlEncode(string s)
        {
            return s.Replace("'", "''");
        }

        ///<summary>
        /// Converts a string into an HTML-encoded string.
        ///</summary>
        ///<param name="s">The string to encode</param>
        ///<returns>The Html encoded string</returns>
        public static string SimpleHtmlEncode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Contains("&"))
                s = s.Replace("&", "&amp;");
            if (s.Contains("<"))
                s = s.Replace("<", "&lt;");
            if (s.Contains(">"))
                s = s.Replace(">", "&gt;");
            return s;
        }

        public static string CustomDecode(string s)
        {
            string decodedStr = s;
            int beforeDecodeLength;
            int afterDecodeLenght;

            do
            {
                beforeDecodeLength = decodedStr.Length;
                decodedStr = AdvancedHtmlDecode(decodedStr);
                afterDecodeLenght = decodedStr.Length;
            } while (beforeDecodeLength != afterDecodeLenght);

            return FixAttributesInTag(decodedStr);
        }

        ///<summary>
        /// Converts a string into an HTML-encoded string.
        ///</summary>
        ///<param name="s">The string to encode</param>
        ///<returns>The Html encoded string</returns>
        public static string AdvancedHtmlEncode(string s)
        {
            /*
            s = SimpleHtmlEncode(s);

            if (string.IsNullOrEmpty(s))
                return s;

            s = s.Replace("\"", "&quot;");
            s = s.Replace("'", "&#39;");
            s = s.Replace("¶", "&para;");
            s = s.Replace("§", "&sect;");
            */

            return WebUtility.HtmlEncode(WebUtility.HtmlEncode(s));
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string
        /// </summary>
        /// <param name="s">The string to decode</param>
        /// <returns>The decoded string</returns>
        public static string SimpleHtmlDecode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            
            if (s.Contains("&lt;"))
                s = s.Replace("&lt;", "<");
            if (s.Contains("&gt;"))
                s = s.Replace("&gt;", ">");
            if (s.Contains("&amp;"))
                s = s.Replace("&amp;", "&");
            return s;
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string
        /// </summary>
        /// <param name="s">The string to decode</param>
        /// <returns>The decoded string</returns>
        public static string AdvancedHtmlDecode(string s)
        {
            /*
            if (string.IsNullOrEmpty(s))
                return s;
            
            s = s.Replace("&lt;", "<");
            s = s.Replace("&gt;", ">");
            s = s.Replace("&quot;", "\"");
            s = s.Replace("&#39;", "'");
            s = s.Replace("&rsquo;", "’");
            s = s.Replace("&amp;", "&");
            s = s.Replace("&para;", "¶");
            s = s.Replace("&sect;", "§");
            */

            return WebUtility.HtmlDecode(s);
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string
        /// and strips tags
        /// </summary>
        /// <param name="s"></param>
        /// <param name="maxLength"> </param>
        /// <returns></returns>
        public static string DecodeAndStripHtml(string s, int? maxLength = null)
        {
            return StripHtml(AdvancedHtmlDecode(s), maxLength);
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string
        /// and strips tags
        /// </summary>
        /// <param name="s"></param>
        /// <param name="maxLength"> </param>
        /// <returns></returns>
        public static string StripHtmlAndEncode(string s, int? maxLength = null)
        {
            return AdvancedHtmlEncode(StripHtml(AdvancedHtmlDecode(s), maxLength));
        }

        /// <summary>
        /// Converts a double to a hex string representation
        /// </summary>
        /// <param name="i">the double to convert</param>
        /// <returns>the hex string</returns>
        public static string DoubleToHex(double i)
        {
            // To hold our converted unsigned integer32 value
            uint uiDecimal = 0;

            try
            {
                // Convert text string to unsigned integer
                uiDecimal = checked(Convert.ToUInt32(i));
            }
            catch (OverflowException)
            {
                return String.Empty;
            }

            // Format unsigned integer value to hex and show in another textbox
            return String.Format("{0:x2}", uiDecimal);
        }

        /// <summary>
        /// Converts a common string into hex format
        /// </summary>
        /// <param name="i">the string to convert</param>
        /// <returns>the hex formatted string</returns>
        public static string StringToHex(string i)
        {
            // To hold our converted unsigned integer32 value
            uint uiDecimal = 0;

            try
            {
                // Convert text string to unsigned integer
                uiDecimal = checked(Convert.ToUInt32(i));
            }
            catch (OverflowException)
            {
                return String.Empty;
            }

            // Format unsigned integer value to hex and show in another textbox
            return String.Format("{0:x2}", uiDecimal);
        }

        /// <summary>
        /// Gets a substring cut from the right side of a given string
        /// </summary>
        /// <param name="s">the string to cut</param>
        /// <param name="length">the length from the right side to cut it</param>
        /// <returns>The substring from the right side</returns>
        public static string Right(string s, int length)
        {
            return s.Substring(s.Length - length);
        }

        /// <summary>
        /// Gets a substring cut from the left side of a given string
        /// </summary>
        /// <param name="s">the string to cut</param>
        /// <param name="length">the length from the left side to cut it</param>
        /// <returns>The substring from the left side</returns>
        public static string Left(string s, int length)
        {
            return s.Substring(0, length);
        }

        /// <summary>
        /// Gets a substring cut from the middle of a given string
        /// </summary>
        /// <param name="s">the string to cut</param>
        /// <param name="start">position to start the cut.</param>
        /// <param name="length">the length to cut it</param>
        /// <returns>The substring from middle</returns>
        public static string Mid(string s, int start, int length)
        {
            return s.Substring(start, length);
        }

        /// <summary>
        /// Determines if the given string is a number
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns>true if the string is numeric, otherwise false</returns>
        public static bool IsNumeric(string str)
        {
            if (str.Length == 0)
                return false;

            // test for negative
            if (str.Substring(0, 1) == "-")
            {
                // if the only character is a minus, return
                if (str.Length == 1)
                {
                    return false;
                }

                str = str.Substring(1, str.Length - 1);
            }

            return !str.Where((t, i) => !Char.IsNumber(str, i)).Any();
        }

        /// <summary>
        /// Determines if the given string is a positive number
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns>true if positive number; otherwise false</returns>
        public static bool IsPositiveNumber(string str)
        {
            if (str == null)
                return false;

            if (!IsNumeric(str))
                return false;

            if ((Int64.Parse(str)) > 0)
                return true;

            return false;
        }


        /// <summary>
        /// Returns the reverse of a given string
        /// </summary>
        /// <param name="inStr">The string to reverse</param>
        /// <returns>The reversed string</returns>
        public static String ReverseString(String inStr)
        {
            //  Helper Method that reverses a String.
            int counter;
            string outStr = "";

            for (counter = inStr.Length - 1; counter >= 0; counter--)
            {
                outStr = outStr + inStr[counter];
            }
            return outStr;
        }

        /// <summary>
        /// Split a delimited string into a List of strings.
        /// </summary>
        /// <param name="source">The delimited string to split.</param>
        /// <param name="delimiter">The token to split the source string on.</param>
        /// <returns></returns>
        public static List<string> StringToList(string source, char delimiter)
        {
            var list = new List<string>();

            if (IsNotNullOrEmpty(source))
            {
                list.AddRange(source.Trim().Split(delimiter).Select(value => value.ToLower()));
            }

            return list;
        }

        #region Hex
        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Byte[] HexStringToByteArray(string hex)
        {
            int stringLength = hex.Length;

            if (stringLength % 2 < 0 || stringLength % 2 > 0)
                throw new ArgumentException("Invalid hex string");

            int byteArrayLength = stringLength / 2;
            var results = new byte[byteArrayLength];

            for (int i = 0; i < byteArrayLength; i++)
            {
                results[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return results;
        }

        /// <summary>
        /// This method converts a hexvalues string as 80FF into a integer.
        ///	Note that you may not put a '#' at the beginning of string! 
        ///	If the string does not represent a valid hexadecimal value it returns 0.
        /// </summary>
        /// <param name="hexstr">The hex string to convert</param>
        /// <returns>An integer representation of the hex</returns>
        public static int HexToInt(String hexstr)
        {
            int counter;
            int hexint = 0;
            hexstr = hexstr.ToUpper();
            char[] hexarr = hexstr.ToCharArray();
            for (counter = hexarr.Length - 1; counter >= 0; counter--)
            {
                if ((hexarr[counter] >= '0') && (hexarr[counter] <= '9'))
                {
                    hexint += (hexarr[counter] - 48) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                }
                else
                {
                    if ((hexarr[counter] >= 'A') && (hexarr[counter] <= 'F'))
                    {
                        hexint += (hexarr[counter] - 55) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                    }
                    else
                    {
                        hexint = 0;
                        break;
                    }
                }
            }
            return hexint;
        }

        /// <summary>
        /// This method converts a integer into a hexadecimal string representing the int value. 
        /// The returned string will look like this: 55FF. 
        /// Note that there is no leading '#' in the returned string! 
        /// </summary>
        /// <param name="hexint">The integer representation of a hex</param>
        /// <returns>The string representation of a hex</returns>
        public static String IntToHex(int hexint)
        {
            int counter = 1;
            string hexstr = "";

            while (hexint + 15 > Math.Pow(16, counter - 1))
            {
                var remainder = (int)(hexint % Math.Pow(16, counter));
                remainder = (int)(remainder / Math.Pow(16, counter - 1));
                if (remainder <= 9)
                {
                    hexstr = hexstr + (char)(remainder + 48);
                }
                else
                {
                    hexstr = hexstr + (char)(remainder + 55);
                }
                hexint -= remainder;
                counter++;
            }
            return ReverseString(hexstr);
        }

        /// <summary>
        /// This version of the IntToHex method returns a hexadecimal string representing the 
        /// int value in the given minimum length. If the hexadecimal string is shorter then the 
        /// length parameter the missing characters will be filled up with leading zeroes.																																										
        /// Note that the returned string is not truncated if the value exeeds the length!
        /// </summary>
        /// <param name="hexint">The integer value</param>
        /// <param name="length">The minimum length</param>
        /// <returns>hexadecimal string representing the int value</returns>
        public static String IntToHex(int hexint, int length)
        {
            string hexstr = IntToHex(hexint);
            string ret = "";

            if (hexstr.Length < length)
            {
                int counter;
                for (counter = 0; counter < (length - hexstr.Length); counter++)
                {
                    ret = ret + "0";
                }
            }
            return ret + hexstr;
        }

        /// <summary>
        /// Translates a html hexadecimal definition of a color into a .NET Framework Color.
        ///  The input string must start with a '#' character and be followed by 6 hexadecimal
        ///  digits. The digits A-F are not case sensitive. If the conversion was not successfull
        ///  the color white will be returned.
        /// </summary>
        /// <param name="hexString">The html hex definition</param>
        /// <returns>A <see cref="Color"/> object</returns>
        public static Color HexToColor(String hexString)
        {
            Color actColor;

            if (hexString.StartsWith("#") && hexString.Length == 7)
            {
                int r = HexToInt(hexString.Substring(1, 2));
                int g = HexToInt(hexString.Substring(3, 2));
                int b = HexToInt(hexString.Substring(5, 2));
                actColor = Color.FromArgb(r, g, b);
            }
            else if (hexString.StartsWith("#") && hexString.Length == 9)
            {
                actColor = HexToAlphaColor(hexString);
            }
            else
            {
                actColor = Color.White;
            }

            return actColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static Color HexToAlphaColor(string hexString)
        {
            // 255 is the default opacity (100% opaque)
            int a = HexToInt(hexString.Substring(1, 2));
            int r = HexToInt(hexString.Substring(3, 2));
            int g = HexToInt(hexString.Substring(5, 2));
            int b = HexToInt(hexString.Substring(7, 2));
            Color actColor = Color.FromArgb(a, r, g, b);

            return actColor;
        }

        /// <summary>
        /// Translates a .NET Framework Color into a string containing the html hexadecimal 
        ///  representation of a color. The string has a leading '#' character that is followed 
        ///  by 8 hexadecimal digits. 
        /// </summary>
        /// <param name="actColor">The <see cref="Color"/> to translate</param>
        /// <returns>The hex string</returns>
        public static String ColorToHex(Color actColor)
        {
            return "#" + IntToHex(actColor.A, 2) + IntToHex(actColor.R, 2) + IntToHex(actColor.G, 2) + IntToHex(actColor.B, 2);
        }

        /// <summary>
        /// Translates a .NET Framework Color into a string containing the html hexadecimal 
        ///  representation of a color. The string has a leading '#' character that is followed 
        ///  by 6 or 8 hexadecimal digits. 
        /// </summary>
        /// <param name="actColor">The <see cref="Color" /> to translate</param>
        /// <param name="includeAlpha">Include alpha channel or not</param>
        /// <returns>The hex string</returns>
        public static String ColorToHex(Color actColor, bool includeAlpha)
        {
            if (includeAlpha)
            {
                return ColorToHex(actColor);
            }

            return "#" + IntToHex(actColor.R, 2) + IntToHex(actColor.G, 2) + IntToHex(actColor.B, 2);
        }

        /// <summary>
        /// Translates a percentage value to it's appropriate hexadecimal color value
        /// </summary>
        /// <param name="percent">The percent value to translate</param>
        /// <returns>The hexadecimal color value</returns>
        public static string PercentToHexColor(int percent)
        {
            float alpha = ((float)percent / 100) * 255;
            return IntToHex((int)alpha);
        }

        #endregion

        /// <summary>
        /// Determines if a string is contains a value other than null or the String.Empty. 
        /// Both leading and trailing whitespace are removed before comparisons are made.
        /// </summary>
        /// <param name="value">The string being evaluated</param>
        /// <returns>True is returned if the value is not null or String.Empty; False is returned in all other cases.</returns>
        public static bool IsNotNullOrEmpty(string value)
        {
            if (value != null && value.Trim().Length != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a string is either null or equal to String.Empty. 
        /// Both leading and trailing whitespace are removed before comparisons are made.
        /// </summary>
        /// <param name="value">The string being evaluated</param>
        /// <returns>True is returned if the value is either null or String.Empty. False is returned in all other cases; </returns>
        public static bool IsNullOrEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            // string is not empty if it has at least one non white space character
            for (int i = 0; i < value.Length; i++)
                if (!Char.IsWhiteSpace(value[i]))
                    return false;

            // all characters are white space
            return true;

            //return !IsNotNullOrEmpty(value);
        }

        /// <summary>
        /// Determines if a string is either null or equal to String.Empty. 
        /// Both leading and trailing whitespace are removed before comparisons are made.
        /// </summary>
        /// <param name="value">The string being evaluated</param>
        /// <returns>True is returned if the value is either null or String.Empty. False is returned in all other cases; </returns>
        public static bool IsNullOrEmpty(ref string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            // string is not empty if it has at least one non white space character
            for (int i = 0; i < value.Length; i++)
                if (!Char.IsWhiteSpace(value[i]))
                    return false;

            // all characters are white space
            return true;
        }


        /// <summary>
        /// Return the intersection of two lists
        /// </summary>
        /// <typeparam name="T">Type of data stored in the list.</typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> ListIntersection<T>(List<T> list1, List<T> list2)
        {
            return list1.Intersect(list2).ToList();

        }

        /// <summary>
        /// Iterates through a List of strings and constructs a delimited list from 
        /// the contents of the list and the specified delimiter.
        /// </summary>
        /// <param name="delimitValue">The character used to delimit.</param>
        /// <param name="values">The data to be converted to a delimited string.</param>
        /// <returns>A delimited string. If an empty list is passed in the empty string is returned.</returns>
        public static string ListToDelimitedString(char delimitValue, List<string> values)
        {
            var delimitedList = new StringBuilder();

            if (values == null || values.Count == 0)
            {
                return String.Empty;
            }

            for (int i = 0; i < values.Count; i++)
            {
                delimitedList.Append(values[i]);
                if (i != values.Count - 1)
                {
                    delimitedList.Append(delimitValue);
                }
            }

            return delimitedList.ToString();
        }

        ///<summary>
        /// Converts a list of objects to a list of lowercase strings.
        ///</summary>
        ///<remarks>This method is useful for deterring if values are already in a dictionary.</remarks>
        ///<param name="objects">The list of objects to iterate through.</param>
        public static List<string> ListToLower(List<object> objects)
        {
            var lowercaseKeys = new List<string>();

            if (objects != null)
            {
                lowercaseKeys.AddRange(objects.Select(key => key.ToString().ToLower()));
            }

            return lowercaseKeys;
        }

        /// <summary>
        /// Returns the int representation of a string. 
        /// Null is returned if the string does not represent a valid integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? AsInt(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            int result;

            if (Int32.TryParse(value, out result))
                return result;

            return null;

            //try
            //{
            //    return Convert.ToInt32(value);
            //}
            //catch
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// Returns the bool representation of an object.
        /// Default value is returned if the string does not represent a valid bool.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool AsBool(object value, bool defaultValue)
        {
            try
            {
                if (value == null || value == string.Empty)
                {
                    return defaultValue;
                }

                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns the int representation of an object.
        /// Default value is returned if the string does not represent a valid int.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int AsInt(object value, int defaultValue)
        {
            try
            {
                if (value is String && String.IsNullOrEmpty((String)value))
                    return defaultValue;

                if (value == null)
                {
                    return defaultValue;
                }

                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns the double representation of a string.
        /// Null is returned if the string does not represent a double.
        /// </summary>
        /// <param name="value">The string representation of the double to convert.</param>
        /// <returns></returns>
        public static double? AsDouble(string value)
        {
            if (IsNullOrEmpty(value)) return null;

            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the double representation of a string.
        /// Null is returned if the string does not represent a double.
        /// </summary>
        /// <param name="value">The string representation of the double to convert.</param>
        /// <returns></returns>
        public static long? AsLong(object value)
        {
            if (value == null
                || value == DBNull.Value)
            {
                return null;
            }

            if (IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the double representation of a date. Cultural differences are considered when converting the date.
        /// Null is returned if the date is not valid.
        /// </summary>
        /// <param name="dateString">The string representation of the date to convert.</param>
        /// <param name="cultureInfo">The format of the date string.</param>
        /// <returns></returns>
        public static double? AsDouble(string dateString, CultureInfo cultureInfo)
        {
            if (IsNullOrEmpty(dateString)) return null;

            DateTime date;
            dateString = dateString.Trim();

            try
            {
                //Store the ticks so they will fit in the double field
                date = cultureInfo != null ? DateTime.Parse(dateString, cultureInfo) : DateTime.Parse(dateString);

                return Convert.ToDouble(date.Ticks);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Make a guid-like string conform to a guid format with respect to positioning
        /// of dash characters.  No validation is performed for invalid characters or 
        /// length.
        /// </summary>
        /// <param name="guidString"></param>
        /// <returns></returns>
        public static string AsPartialGuid(string guidString)
        {
            //Remove any existing dashes
            //Sample (not technically valid guid) ABCDEFGH-1234-5678-9012-XYZ123ABCDEF
            // ABCDEFGH123456789012XYZ123ABCDEF
            guidString = guidString.Replace("-", "");

            if (IsNullOrEmpty(guidString) || guidString.Length < 8)
            {
                return guidString;
            }

            //Guid string has format of (8 Chars)-(4 Chars)-(4 Chars)-(4 Chars)-(12-Chars)
            //ABCDEFGH-123456789012XYZ123ABCDEF
            guidString = guidString.Insert(8, "-");

            //ABCDEFGH-1234-56789012XYZ123ABCDEF
            if (guidString.Length > 13)
            {
                guidString = guidString.Insert(13, "-");
            }

            //ABCDEFGH-1234-5678-9012XYZ123ABCDEF
            if (guidString.Length > 18)
            {
                guidString = guidString.Insert(18, "-");
            }

            //ABCDEFGH-1234-5678-9012-XYZ123ABCDEF
            if (guidString.Length > 23)
            {
                guidString = guidString.Insert(23, "-");
            }

            return guidString;
        }

        /// <summary>
        /// List values of an enum type in string form
        /// </summary>
        /// <returns></returns>
        public static List<string> ListEnumValues(Type enumType)
        {
            if (enumType.BaseType != typeof(Enum))
            {
                throw new Exception("Type must be an enum.");
            }

            Array enumValues = Enum.GetValues(enumType);

            return (from object enumValue in enumValues select enumValue.ToString()).ToList();
        }

        #region More Numeric Conversion

        /// <summary>
        /// Check if value is an int32
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <returns></returns>
        public static bool IsInt32(string value)
        {
            return GetInt32(value).HasValue;
        }

        /// <summary>
        /// Get an int value
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <returns></returns>
        public static int? GetInt32(string value)
        {
            int dummyInt;

            if (IsNullOrEmpty(value))
            {
                return null;
            }

            if (Int32.TryParse(value, out dummyInt))
            {
                return dummyInt;
            }

            return null;
        }

        /// <summary>
        /// Check if value is an int32
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <returns></returns>
        public static bool IsLongInt(string value)
        {
            return GetLongInt(value).HasValue;
        }

        /// <summary>
        /// Get an int value
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <returns></returns>
        public static long? GetLongInt(string value)
        {
            long dummyInt;

            if (IsNullOrEmpty(value))
            {
                return null;
            }

            if (Int64.TryParse(value, out dummyInt))
            {
                return dummyInt;
            }

            return null;
        }

        /// <summary>
        /// Get cultures for conversions
        /// </summary>
        /// <returns></returns>
        private static List<CultureInfo> GetCultures()
        {
            //Attempt to use current thread culture by default.  This can be changed via machine configuration
            // or via the globalization element in the system.web portion of the web.config.
            var list = new List<CultureInfo> { CultureInfo.CurrentCulture, GetUsCulture(), GetRotwCulture() };

            //French grouping separator is a non-breaking space, ascii 160, which
            // is not possible to enter in UTF-8, so we'll add a special french
            // culture clone that looks for a space, ascii 32, as the grouping separator
            var customFrench = new CultureInfo("fr-FR") { NumberFormat = { NumberGroupSeparator = " " } };

            list.Add(customFrench);

            var result = list.Distinct().ToList();

            return result;
        }

        /// <summary>
        /// Get culture for us date/number
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetUsCulture()
        {
            return new CultureInfo("en-US");
        }

        /// <summary>
        /// Get culture for rest of world date/culture
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetRotwCulture()
        {
            return new CultureInfo("fr-FR");
        }

        /// <summary>
        /// Get if text contains html
        /// </summary>
        /// <returns></returns>
        public static bool IsHtmlFormattedText(string text)
        {
            return !string.IsNullOrEmpty(text) && (text.IndexOf("html-wrapper") >= 0 ||
                (text.StartsWith("<p>") && text.EndsWith("</p>")) ||
                (text.StartsWith(AdvancedHtmlEncode("<p>")) &&  text.EndsWith(AdvancedHtmlEncode("</p>"))));
        }

        /// <summary>
        /// Get if text is encoded
        /// </summary>
        /// <returns></returns>
        public static bool IsTextEncoded(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            Regex r = new Regex(@"&lt;(.|\n)*?&gt;");

            return r.IsMatch(text) && text.IndexOfAny(new []{'>','<'}) < 0;
        }

        /// <summary>
        /// Check if value is a double
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>
        /// <returns></returns>
        public static bool IsDouble(string value, params CultureInfo[] cultures)
        {
            return GetDouble(value, cultures).HasValue;
        }

        /// <summary>
        /// Get a double value. Null
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>
        /// <returns></returns>
        public static double? GetDouble(string value, params CultureInfo[] cultures)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }

            List<CultureInfo> cultureList = cultures.Length == 0 ? GetCultures() : new List<CultureInfo>(cultures);

            //Double try parse allows numbers like this:
            //  78,59,50.34 = 785950.34
            // or 3,40      = 340 or 3.40 depending on the culture
            // The latter case is especially confusing for a French-style number where
            // 3,40 is actually 3.40.  TryParse with culture of en-US results in a value of
            // 340.
            //As a result, some manual validation needs to be done on the numbers, rather than just a try parse
            foreach (CultureInfo culture in cultureList)
            {
                //Perform first check to validate number, which prevents
                // 3,40 from evaluating to 340 or to prevent
                // 3,4,5,433 from evaulating sucessfully, both of which will
                // happen with double.TryParse for en-us culture.
                if (IsValidDouble(value, culture.NumberFormat))
                {
                    double dummyDouble;

                    if (Double.TryParse(value, NumberStyles.Number | NumberStyles.Float, culture, out dummyDouble))
                    {
                        return dummyDouble;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Return a boolean if a number is valid for the specified number format.
        /// </summary>
        /// <param name="stringValue">Value as a string.</param>
        /// <param name="numberFormat">Number format.</param>
        /// <returns></returns>
        private static bool IsValidDouble(string stringValue, NumberFormatInfo numberFormat)
        {
            if (IsNullOrEmpty(stringValue))
            {
                return false;
            }

            //Check if the string has group or decimal values

            //Base regexp with some tokens to replace for the current format
            string basePattern = "^[-]{0,1}(\\d{1,__GROUP_SIZE__}(__GROUP_SEPARATOR__\\d{__GROUP_SIZE__})*|\\d*)(__DECIMAL_SEPARATOR__\\d*){0,1}$";

            //See if we have the group sizes to plug into the regexp
            int[] numberGroupSizes = numberFormat.NumberGroupSizes;

            if (numberGroupSizes.Length > 0)
            {
                basePattern = basePattern.Replace("__GROUP_SIZE__", numberGroupSizes[0].ToString());
                basePattern = basePattern.Replace("__GROUP_SEPARATOR__", "\\" + numberFormat.NumberGroupSeparator);
                basePattern = basePattern.Replace("__DECIMAL_SEPARATOR__", "\\" + numberFormat.NumberDecimalSeparator);

                var regEx = new Regex(basePattern);

                return regEx.Match(stringValue).Success;
            }

            return true;
        }

        /// <summary>
        /// Check if value is a date
        /// </summary>
        /// <param name="value">Value of operand as string</param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>        
        /// <returns></returns>
        public static bool IsDate(string value, params CultureInfo[] cultures)
        {
            return GetDate(value, cultures).HasValue;
        }

        /// <summary>
        /// Get a date value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>        
        /// <returns></returns>
        public static DateTime? GetDate(string value, params CultureInfo[] cultures)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }

            List<CultureInfo> cultureList = cultures.Length == 0 ? GetCultures() : new List<CultureInfo>(cultures);

            foreach (CultureInfo culture in cultureList)
            {
                DateTime dummyDate;

                if (DateTime.TryParse(value, culture, DateTimeStyles.AllowWhiteSpaces, out dummyDate))
                {
                    return dummyDate;
                }
            }

            return null;
        }

        /// <summary>
        /// Return a boolean indicating if the value is currency
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>        
        /// <returns></returns>
        public static bool IsCurrency(string value, params CultureInfo[] cultures)
        {
            return GetCurrencyNumericValue(value, cultures).HasValue;
        }

        /// <summary>
        /// Get the numeric portion of a currency string. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>        
        /// <returns></returns>
        public static double? GetCurrencyNumericValue(string value, params CultureInfo[] cultures)
        {
            if (IsNullOrEmpty(value))
            {
                return null;
            }

            //Consider a double or int valid for currency
            if (IsDouble(value, cultures))
            {
                return GetDouble(value, cultures);
            }

            //See if the first character is the only non numeric or separator value
            if (value.Length > 1)
            {
                string firstChar = value.Substring(0, 1);
                string trimmedValue = value.Substring(1, value.Length - 1);

                //If first char is a number then it's not a currency symbol, otherwise
                // see if rest of number is
                if (!IsInt32(firstChar))
                {
                    //Consider a double or int valid for currency
                    if (IsDouble(value, cultures))
                    {
                        return GetDouble(trimmedValue, cultures);
                    }
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Return a string with HTML values stripped out.
        /// </summary>
        /// <param name="stringToStrip">The string to strip.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="removeNewLineCharacters">if set to <c>true</c> [remove new line characters].</param>
        /// <returns></returns>
        public static string StripHtml(string stringToStrip, int? maxLength = null, bool removeNewLineCharacters = true)
        {
            if (IsNullOrEmpty(stringToStrip))
            {
                return stringToStrip;
            }

            stringToStrip = HttpUtility.HtmlDecode(stringToStrip);

            //Remove white space
            stringToStrip = stringToStrip.Replace("&nbsp;", " ");

            stringToStrip = stringToStrip.Replace("<br />", removeNewLineCharacters ? " " : "\n");
            stringToStrip = stringToStrip.Replace("<br>", removeNewLineCharacters ? " " : "\n");
             
            //Remove html and non-printable whitespace characters
            stringToStrip = Regex.Replace(stringToStrip, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
            stringToStrip = Regex.Replace(stringToStrip, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty);
            stringToStrip = Regex.Replace(stringToStrip, @"<iframe.*?>", string.Empty);
            stringToStrip = stringToStrip.Replace("&ldquo;", "“");
            stringToStrip = stringToStrip.Replace("&rdquo;", "”");
            return TruncateText(Regex.Replace(stringToStrip, @"</([a-z/.]+)*?>", string.Empty), maxLength);

        }


        /// <summary>
        /// Removes the new line characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string RemoveNewLineCharacters(string text)
        {
            return text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// Determines if text is the appropriate length. 
        /// If the text is too long the middle is truncated in order to maintain readability.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="maxLength">The maximum allowed length of the text.</param>
        /// <returns></returns>
        public static string TruncateText(string text, int? maxLength)
        {
            if (maxLength.HasValue)
            {
                if (text.Length > maxLength)
                {
                    //Add addtional 4 to account for ellipsis inserted into middle of text and add one in case rounding of numToRemove/2
                    // would cause an additional character to be included.
                    int numToRemove = text.Length - maxLength.Value + 4;
                    int firstSegmentEnd = (text.Length / 2) - (numToRemove / 2);
                    int secondSegmentStart = (text.Length / 2) + (numToRemove / 2);

                    text = text.Substring(0, firstSegmentEnd) + "..." + text.Substring(secondSegmentStart);
                }
            }

            return text;
        }

        /// <summary>
        /// Validate that the specified file name ends in .csv or .txt
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ValidateCsvFileName(string fileName)
        {
            return IsNotNullOrEmpty(fileName)
                    && (fileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase)
                    || fileName.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get "page" of data from a list.
        /// </summary>
        /// <param name="listToPage"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <returns></returns>
        public static List<T> GetListDataPage<T>(List<T> listToPage, int pageNumber, int resultsPerPage)
        {
            if (pageNumber <= 0 || resultsPerPage <= 0)
            {
                return listToPage;
            }

            int startIndex = (pageNumber - 1) * resultsPerPage;
            int itemCount = resultsPerPage;

            //Make sure start index isn't higher than total item count
            if (startIndex >= listToPage.Count)
            {
                itemCount = 0;
            }

            //Ensure there are enough items to get a full page.  If not adjust item count accordingly
            if ((startIndex + resultsPerPage) > listToPage.Count)
            {
                itemCount = listToPage.Count - startIndex;
            }

            //Return items or empty list depending on count
            if (itemCount > 0)
            {
                return listToPage.GetRange(startIndex, itemCount);
            }

            return new List<T>();
        }

        /// <summary>
        /// Move a list element to a new position.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="currentElementPosition"></param>
        /// <param name="newElementPosition"></param>
        public static void MoveListElement<T>(List<T> list, int currentElementPosition, int newElementPosition)
        {
            if (currentElementPosition < 0
                || newElementPosition < 0
                || currentElementPosition >= list.Count
                || newElementPosition >= list.Count)
            {
                return;
            }

            //Copy item to new location


            if (newElementPosition < currentElementPosition)
            {
                list.Insert(newElementPosition, list[currentElementPosition]);

                //If moving item earlier in the list, the current index of item to remove
                // will be incremented.););

                list.RemoveAt(currentElementPosition + 1);
            }
            else
            {
                list.Insert(newElementPosition + 1, list[currentElementPosition]);
                list.RemoveAt(currentElementPosition);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSaltedMd5Hash(string input)
        {
            //salt the salt is the md5 hash of "hello world"
            input = input + "5eb63bbbe01eeed093cb22bb8f5acdc3";

            var results = new StringBuilder();
            var cryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] inputStream = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(input));

            foreach (byte b in inputStream)
            {
                results.Append(b.ToString("x2").ToLower());
            }

            return results.ToString();
        }


        /// <summary>
        /// Removes script tag and it's content from the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveScript(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                string res = Regex.Replace(value, @"(?s)<\s?script.*?(/\s?>|<\s?/\s?script\s?>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                res = Regex.Replace(res, @"on([a-zA-Z0-9]+)(\s*)=(\s*)""[^""]*""", "on$1$2=$3\"\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                res = Regex.Replace(res, @"on([a-zA-Z0-9]+)(\s*)=(\s*)'[^']*'", "on$1$2=$3''", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                
                //this stuff removes href from user-entered links in TinyMCE
                //res = Regex.Replace(res, @"href=(\s*)""[^""]*""", "href = \"#\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //res = Regex.Replace(res, @"href=(\s*)'[^']*'", "href = '#'", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                
                return res;
            }
            return value;
        }

        private static string FixAttributesInTag(string tagHtml)
        {
            if (string.IsNullOrEmpty(tagHtml))
                return tagHtml;

            try
            {
                int index = 0;
                //lets find first attribute="value" construction
                while ((index = tagHtml.IndexOf("=\"", index)) >= 0)
                {
                    //check if there is any following attribute="value"
                    int indexOfNext = tagHtml.IndexOf("=\"", index + 1);
                    //if not, find closing tag
                    if (indexOfNext < 0)
                        indexOfNext = tagHtml.IndexOf(">", index + 1);
                    //otherway find closing quotes
                    else
                    {
                        string substr = tagHtml.Substring(index + 2, indexOfNext - index - 2);

                        //html in selected substring may contain quoutes inside element text
                        //trying to find closing tag, and its index
                        if (substr.IndexOf(">") >= 0)
                        {
                            substr = substr.Substring(0, substr.IndexOf(">"));
                            indexOfNext = index + 2 + substr.LastIndexOf("\"");
                        }
                        else
                        {
                            indexOfNext = index + 2 + substr.LastIndexOf("\"");
                        }
                    }

                    //get attribute value
                    string attributeValue = tagHtml.Substring(index + 2, indexOfNext - index - 1);
                    if (!string.IsNullOrEmpty(attributeValue))
                    {
                        int lastClosingQuoteIndex = attributeValue.LastIndexOf("\"");
                        attributeValue = attributeValue.Remove(lastClosingQuoteIndex);

                        //check for tag name. It its a title - do not replace quotes inside
                        var tagStartIndex = tagHtml.LastIndexOf(" ", index);
                        if (tagStartIndex > 0 && tagHtml.Substring(tagStartIndex, index - tagStartIndex).Trim().ToLower() == "title")
                        {
                            var newAttributeValue = WebUtility.HtmlEncode(attributeValue);
                            tagHtml = tagHtml.Replace(attributeValue, newAttributeValue);
                            index += newAttributeValue.Length - 1;
                            continue;
                        }

                        //there is possible a situation with a couple of quotes in attribute value, remove unnecessary
                        tagHtml = tagHtml.Replace(attributeValue, attributeValue.Replace("\"", string.Empty));
                    }
                    index++;
                }
            }
            catch
            {
            }

            return tagHtml;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripIframes(string text)
        {
            bool encoded = IsTextEncoded(text);

            if (encoded)
                text = AdvancedHtmlDecode(text);

            const string expression = @"<iframe.*?/iframe>";
//            const string youtubeExpr = @"^(http://youtu\.be/([a-zA-Z0-9]|_)+($|\?.*)|https?://www\.youtube\.com/watch\?v=([a-zA-Z0-9]|_)+&.*)";
  //          const string vimeoExpr = @"^http://(vimeo\.com/[0-9]+($|\?.*)|player\.vimeo\.com/video/[0-9]*($|\?.+))";

            const string prefix = "src=\"//";
            const string httpPrefix = "src=\"http://";
            const string securePrefix = "src=\"https://";

            List<string> allowedDomains = new List<string>
                                                  {
                                                      "www.youtube.com/",
                                                      "youtube.com/",
                                                      "www.youtu.be/",
                                                      "youtu.be/",
                                                      "www.vimeo.com",
                                                      "vimeo.com",
                                                      "player.vimeo.com",
                                                      "app.box.com"
                                                  };

            Regex regex = new Regex(expression);
            foreach (Match match in regex.Matches(text))
            {
                bool contains = false;
                foreach (var domain in allowedDomains)
                {
                    string d = prefix + domain;
                    string hd = httpPrefix + domain;
                    string sd = securePrefix + domain;
                    if (match.Value.Contains(d) || match.Value.Contains(hd) || match.Value.Contains(sd))
                    {
                        contains = true;
                        break;
                    }
                }

                if(!contains)
                    text = text.Replace(match.Value, string.Empty);
            }

            if (encoded)
            {
                text = AdvancedHtmlEncode(text);
            
                regex = new Regex(@"&lt;iframe.*?/iframe&gt;");
                foreach (Match match in regex.Matches(text))
                {
                    text = text.Replace(match.Value, AdvancedHtmlDecode(match.Value));
                }
            }

            return text;
        }

        /// <summary>
        /// Encodes ">, <" sings in html content
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string EncodeTagsInHtmlContent(string html)
        {
            var res = EncodeTagsInHtmlContentWorker(html);

            res = StripDataAttributes(res);

            return StripMSWordTags(res);
        }

        private static string EncodeTagsInHtmlContentWorker(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            int lastOpenTagIndex = 0;
            while (lastOpenTagIndex >= 0)
            {
                int openTagInd = html.IndexOf("<", lastOpenTagIndex);
                if (openTagInd > -1) // if it contains any tags lets parse them
                {
                    //find closing tag index or whitespace if it's tag with attributes
                    int commentEndIndex = -1;
                    int tagNameEndSpaceIndex = html.IndexOf(" ", openTagInd);
                    int tagNameEndClosingIndex = html.IndexOf(">", openTagInd);
                    int commentStartIndex = html.IndexOf("<!--", openTagInd);
                    if (commentStartIndex > -1)
                        commentEndIndex = html.IndexOf("-->", commentStartIndex);
                    int tagNameEnd;

                    //what we have found
                    //1. there is a comment
                    if (commentStartIndex > -1 && commentEndIndex > -1)
                    {
                        int commentContentStartIndex = commentStartIndex + "<!--".Length;
                        string comment = html.Substring(commentContentStartIndex, commentEndIndex - commentContentStartIndex);
                        //if there are MS Word tags
                        bool msTags = comment.IndexOf("[") >= 0 && comment.IndexOf("]") > 0 && comment.IndexOf("if ") > 0;
                        int closingCommentIndex = html.IndexOf("<!--[endif]-->", commentContentStartIndex);
                        if (msTags && closingCommentIndex > -1)
                            comment = html.Substring(commentStartIndex, closingCommentIndex + "<!--[endif]-->".Length - commentStartIndex);
                        else
                            comment = html.Substring(commentStartIndex, commentEndIndex + "-->".Length - commentStartIndex);

                        //strip comment
                        html = html.Replace(comment, string.Empty);
                        continue;
                    }
                    //2. there are only <, > signs, not tags; then just replace those
                    if (openTagInd > -1 && tagNameEndClosingIndex < 0)
                    {
                        string inner = html.Substring(lastOpenTagIndex);
                        html = html.Replace(inner, inner.Replace("<", "&lt;").Replace(">", "&gt;"));
                        continue;
                    }
                    //3. possibly here is a tag with attributes
                    if (tagNameEndSpaceIndex > -1 && tagNameEndSpaceIndex < tagNameEndClosingIndex)
                    {
                        tagNameEnd = tagNameEndSpaceIndex;

                        //fix attributes in tag
                        string tag = html.Substring(tagNameEndSpaceIndex, tagNameEndClosingIndex - tagNameEndSpaceIndex + 1);
                        if (tag.Length > 0)
                            html = html.Remove(tagNameEndSpaceIndex, tagNameEndClosingIndex - tagNameEndSpaceIndex + 1)
                                .Insert(tagNameEndSpaceIndex, FixAttributesInTag(tag));

                    }
                    //4. simple tag
                    else if (tagNameEndClosingIndex > -1 && (tagNameEndClosingIndex < tagNameEndSpaceIndex || tagNameEndSpaceIndex < 0))
                        tagNameEnd = tagNameEndClosingIndex;
                    //5. there's no any closing tag, then the first part can be encoded
                    else
                    {
                        html = html.Remove(openTagInd, 1).Insert(openTagInd, "&lt;");
                        lastOpenTagIndex = openTagInd + 4;
                        continue;
                    }

                    //get tag name
                    string tagName = html.Substring(openTagInd + 1, tagNameEnd - openTagInd - 1).Trim();

                    //find closing tag or /> 
                    int closingTagIndex1 = html.IndexOf("</" + tagName + ">", openTagInd);
                    int closingTagIndex2 = html.IndexOf("/>", openTagInd);
                    int nextOpeningTagIndex = html.IndexOf("<", openTagInd + 1);
                    int closingTagInd;

                    //what we have found
                    if (closingTagIndex1 > -1 && (closingTagIndex1 < closingTagIndex2 || closingTagIndex2 < 0 || nextOpeningTagIndex < closingTagIndex2))
                    {
                        closingTagInd = closingTagIndex1;

                        //find inner similar tags
                        int lastInnerTagIndex = tagNameEnd;
                        while (lastInnerTagIndex < closingTagInd)
                        {
                            lastInnerTagIndex = html.IndexOf("<" + tagName, lastInnerTagIndex);
                            if (lastInnerTagIndex < 0 || lastInnerTagIndex >= closingTagInd)
                                break;

                            //this hack is needed there to prevent situations
                            //with similar-spelled tags confusion (as example: <b and <br)
                            char nextChar = html[lastInnerTagIndex + tagName.Length + 1];
                            bool findNextClosingTag = nextChar == '/' || nextChar == '>';
                            if (!findNextClosingTag && nextChar == ' ')
                            {
                                //if current tag is self-closed skip it
                                var indexOfOpeningNextTag = html.IndexOf("<", lastInnerTagIndex + tagName.Length + 1);
                                var indexOfSelfClosing = html.IndexOf("/>", lastInnerTagIndex + tagName.Length + 1);
                                findNextClosingTag = indexOfSelfClosing < 0 || indexOfSelfClosing > indexOfOpeningNextTag;
                            }
                            if (findNextClosingTag)
                                closingTagInd = html.IndexOf("</" + tagName + ">", closingTagInd + 1);

                            lastInnerTagIndex++;
                        }
                    }
                    else if (closingTagIndex2 > -1 && (nextOpeningTagIndex < 0 || nextOpeningTagIndex > closingTagIndex2)
                        && (closingTagIndex2 < closingTagIndex1 || closingTagIndex1 < 0))
                    {
                        //this is tag witout content, skip it
                        lastOpenTagIndex = closingTagIndex2 + 2;
                        continue;
                    }
                    else //there's no any closing tag, then the first part can be encoded
                    {
                        //<img> and <br> tags could be unclosed
                        if (tagName != "img" && tagName != "br")
                        {
                            if (nextOpeningTagIndex < 0)
                                nextOpeningTagIndex = html.Length;
                            var inner = html.Substring(openTagInd, nextOpeningTagIndex - openTagInd);
                            if (inner.Length > 0)
                            {
                                html = html.Remove(openTagInd, nextOpeningTagIndex - openTagInd)
                                    .Insert(openTagInd, inner.Replace("<", "&lt;").Replace(">", "&gt;"));
                            }

                            lastOpenTagIndex = html.IndexOf("<", openTagInd);
                        }
                        else
                            lastOpenTagIndex = tagNameEnd + 1;

                        continue;
                    }

                    //correct closing tag index
                    tagNameEnd = html.IndexOf(">", tagNameEnd);

                    //look if there's content not in tag
                    var firstPartHtml = html.Substring(lastOpenTagIndex, openTagInd - lastOpenTagIndex);
                    if (firstPartHtml.Length > 0)
                    {
                        html = html.Replace(firstPartHtml, firstPartHtml.Replace("<", "&lt;").Replace(">", "&gt;"));
                        lastOpenTagIndex = html.IndexOf("<", lastOpenTagIndex);
                        continue;
                    }

                    //get inner html
                    var innerHtml = html.Substring(tagNameEnd + 1, closingTagInd - tagNameEnd - 1);
                    int lenBefore = 0, lenAfter = 0;
                    if (innerHtml.Length > 0)
                    {
                        lenBefore = innerHtml.Length;
                        string encoded = EncodeTagsInHtmlContentWorker(innerHtml);
                        html = html.Remove(tagNameEnd + 1, closingTagInd - tagNameEnd - 1).Insert(tagNameEnd + 1, encoded);
                        lenAfter = encoded.Length;
                    }

                    //update tag index
                    lastOpenTagIndex = closingTagInd + ("</" + tagName + ">").Length + lenAfter - lenBefore;
                }
                else //current part can be encoded
                {
                    var substr = html.Substring(lastOpenTagIndex);
                    if (substr.Length > 0)
                        html = html.Replace(substr, substr.Replace("<", "&lt;").Replace(">", "&gt;"));
                    break;
                }
            }

            return html;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static string ReplaceHtmlAttributes(string html, bool validate)
        {
            try
            {
                StringBuilder output = new StringBuilder();

                //create root element to avoid xml-parsing mistake
                html = string.Format("<root>{0}</root>", html);
                // NOTE: you cannot use <,>,",',& characterts in xml value. Need to use escaped equivalents,
                // but AdvancedHtmlDecode method perform convertion from html encoded value &amp; to &
                //using (XmlTextReader reader = new XmlTextReader(new StringReader(AdvancedHtmlDecode(html))))
                using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                {
                    XmlWriterSettings ws = new XmlWriterSettings();
                    ws.Indent = false;
                    ws.ConformanceLevel = ConformanceLevel.Auto;
                    ws.NewLineOnAttributes = false;
                    ws.OmitXmlDeclaration = true;
                    ws.NewLineHandling = NewLineHandling.None;
                    ws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                    
                    using (XmlWriter writer = XmlWriter.Create(output, ws))
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    
                                    writer.WriteStartElement(reader.Name);
                                    if (reader.Name == "br")
                                    {
                                        writer.WriteEndElement();
                                    }
                                    string colorValue = null;
                                    string styleValue = null;
                                    while (reader.MoveToNextAttribute())
                                    {
                                        string name = reader.Name;
                                        string value = reader.Value;

                                        //skip attributes with dash as they break renderer
                                        if (name.Contains('-'))
                                            continue;

                                        switch (name)
                                        {
                                            case "bgcolor":
                                                colorValue = value;
                                                break;
                                            case "style":
                                                styleValue = value;
                                                break;
                                            default:
                                                writer.WriteAttributeString(name, value);
                                                break;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(colorValue) && !string.IsNullOrEmpty(styleValue))
                                    {
                                        if (styleValue.Last() != ';')
                                            styleValue += ';';
                                        
                                        styleValue += String.Format("background-color: {0};", colorValue);
                                    }

                                    if (!String.IsNullOrEmpty(styleValue))
                                        writer.WriteAttributeString("style", styleValue);
                                    break;

                                case XmlNodeType.Text:
                                    writer.WriteString(reader.Value);
                                    break;

                                case XmlNodeType.EntityReference:
                                    writer.WriteRaw("&");
                                    writer.WriteRaw(reader.Name);
                                    writer.WriteRaw(";");
                                    break;

                                case XmlNodeType.EndElement:
                                    if (reader.Name != "br")
                                    {
                                        writer.WriteEndElement();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                var result = output.ToString();
                //remove root element
                result = Regex.Replace(result, "^<root>", String.Empty);
                result = Regex.Replace(result, "</root>$", String.Empty);

                return result;
            }
            catch (Exception)
            {
                return validate 
                    ? "<div class=\"error\">We have detected invalid or corrupted HTML. Please correct the issue with your HTML</div>"
                    : html;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanInvalidXmlCharacters(string text)
        {
            //this thing strips some non-unicode characters like "č, ž, š"
            //const string PATTERN = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            //return Regex.Replace(text, PATTERN, "");

            if (string.IsNullOrEmpty(text))
                return string.Empty; // vacancy test.

            var textOut = new StringBuilder(); 

            foreach (char c in text.Where(XmlConvert.IsXmlChar))
            {
                textOut.Append(c);
            }

            return textOut.ToString();
        }

        /// <summary>
        /// Strips Html Tags from given string
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static string StripHtmlTags(string answer)
        {
            const string HTML_TAG_PATTERN = "<.*?>";

            return Regex.Replace
              (answer, HTML_TAG_PATTERN, string.Empty);
        }

        /// <summary>
        /// Strips data-* attributes from given string
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static string StripDataAttributes(string answer)
        {
            const string DATA_ATTRIBUTE_PATTERN = @"(data-.+?="".*?"")|(data-.+?='.*?')";

            return Regex.Replace
              (answer, DATA_ATTRIBUTE_PATTERN, string.Empty, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Removes tags and attributes from MS Word
        /// 
        /// Taken from http://tim.mackey.ie/2005/11/23/CleanWordHTMLUsingRegularExpressions.aspx
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string StripMSWordTags(string p)
        {
            if (!string.IsNullOrEmpty(p))
            {
                p = Regex.Replace(p, @"<[/]?([ovwxp]:\w+)[^>]*?>", "", RegexOptions.IgnoreCase);
                p = Regex.Replace(p, @"<([^>]*)(?:lang|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
                p = Regex.Replace(p, @"<([^>]*)(?:lang|[ovwxp]:\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            }

            return p;
        }

        public static string ProtectFilterFieldFromSQLInjections(string src)
        {
 
            if (!string.IsNullOrEmpty(src))
                return System.Text.RegularExpressions.Regex.Replace(src, "[^a-zA-Z]+", "", RegexOptions.Compiled);
            return src;
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new ZlibStream(output, Ionic.Zlib.CompressionMode.Compress, CompressionLevel.BestCompression, true))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream())
            {
                input.Write(data, 0, data.Length);
                input.Position = 0;

                using (var gzip = new ZlibStream(input, Ionic.Zlib.CompressionMode.Decompress, CompressionLevel.BestCompression, true))
                {
                    using (var output = new MemoryStream())
                    {
                        var buff = new byte[64];
                        var read = gzip.Read(buff, 0, buff.Length);
                        while (read > 0)
                        {
                            output.Write(buff, 0, read);
                            read = gzip.Read(buff, 0, buff.Length);
                        }
                        return output.ToArray();
                    }
                }
            }
        }

        public static List<int> GenerateRandom(int count, int min, int max)
        {

            //  initialize set S to empty
            //  for J := N-M + 1 to N do
            //    T := RandInt(1, J)
            //    if T is not in S then
            //      insert T in S
            //    else
            //      insert J in S
            //
            // adapted for C# which does not have an inclusive Next(..)
            // and to make it from configurable range not just 1.

            if (max <= min || count < 0 ||
                    // max - min > 0 required to avoid overflow
                    (count > max - min && max - min > 0))
            {
                // need to use 64-bit to support big ranges (negative min, positive max)
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                        " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
            }

            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();

            // start count values before max, and end at max
            for (int top = max - count; top < max; top++)
            {
                // May strike a duplicate.
                // Need to add +1 to make inclusive generator
                // +1 is safe even for MaxVal max value because top < max
                if (!candidates.Add(random.Next(min, top + 1)))
                {
                    // collision, add inclusive max.
                    // which could not possibly have been added before.
                    candidates.Add(top);
                }
            }

            // load them in to a list, to sort
            List<int> result = candidates.ToList();

            // shuffle the results because HashSet has messed
            // with the order, and the algorithm does not produce
            // random-ordered results (e.g. max-1 will never be the first value)
            for (int i = result.Count - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }

        public static List<int> GenerateRandom(int count)
        {
            return GenerateRandom(count, 0, Int32.MaxValue);
        }

        public static List<ListItem> GetFontFamilyNameListItems(params FontStyle[] requiredStyles)
        {
            List<ListItem> familyNames = new List<ListItem>();

            foreach (FontFamily family in FontFamily.Families)
            {
                bool supported = true;

                foreach (FontStyle style in requiredStyles)
                {
                    if (!family.IsStyleAvailable(style))
                    {
                        supported = false;
                        break;
                    }
                }

                if (supported)
                {
                    familyNames.Add(new ListItem(family.Name, family.Name));
                }
            }

            return familyNames;
        }

        public static List<ListItem> GetFontSizeListItems(int smallest, int largest, int step, string sizeSuffix)
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = smallest; i <= largest; i = i + step)
            {
                items.Add(new ListItem(i + " " + sizeSuffix, i.ToString()));
            }

            return items;
        }

        public static void BindList(ListControl list, IEnumerable<ListItem> listItems, string selectedValue)
        {
            foreach (ListItem item in listItems)
            {
                list.Items.Add(item);
            }

            if (selectedValue != null && list.Items.FindByValue(selectedValue) != null)
            {
                list.SelectedValue = selectedValue;
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
