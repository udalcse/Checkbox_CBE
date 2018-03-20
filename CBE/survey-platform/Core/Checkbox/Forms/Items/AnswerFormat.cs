using System;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// AnswerFormat describes the required format of an inputed answer
    /// </summary>
    [Serializable]
    public enum AnswerFormat
    {
        /// <summary>
        /// No formatting applied
        /// </summary>
        None = 1,
        /// <summary>
        /// Must be a valid email address
        /// </summary>
        Email,
        /// <summary>
        /// Must be an Integer
        /// </summary>
        Integer,
        /// <summary>
        /// Must be any valid number
        /// </summary>
        Numeric,
        /// <summary>
        /// Must contain a decimal place
        /// </summary>
        Decimal,
        /// <summary>
        /// Must be in currency format
        /// </summary>
        Money,
        /// <summary>
        /// Must be formatted as a US phone number
        /// </summary>
        Phone,
        /// <summary>
        /// Must be formatted as a social security number
        /// </summary>
        SSN,
        /// <summary>
        /// Must be formatted as a url
        /// </summary>
        URL,
        /// <summary>
        /// Must be a US or Canadian zip code
        /// </summary>
        Postal,
        /// <summary>
        /// Must be comprised of only the letters a through z or A through Z
        /// </summary>
        Alpha,
        /// <summary>
        /// Must be comprised of only the letters a through z
        /// </summary>
        Lowercase,
        /// <summary>
        /// Must be comprised of only the letters A through Z
        /// </summary>
        Uppercase,
        /// <summary>
        /// Must be formatted as either MM/DD/YYYY or DD/MM/YYYY
        /// </summary>
        Date,
        /// <summary>
        /// Must be formatted as either MM/DD/YYYY
        /// </summary>
        Date_USA,
        /// <summary>
        /// Must be formatted as either DD/MM/YYYY
        /// </summary>
        Date_ROTW,
        /// <summary>
        /// Must be comprised of only a through, A through Z, or 0 through 9
        /// </summary>
        AlphaNumeric,
        /// <summary>
        /// A cutom validator created by users
        /// </summary>
        Custom
    }
}
