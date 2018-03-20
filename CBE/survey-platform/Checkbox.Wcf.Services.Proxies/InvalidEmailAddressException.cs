using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// The issue occurs when the provided email address is not valid.
    /// </summary>
    [Serializable]
    public class InvalidEmailAddressException : Exception
    {
        /// <summary>
        /// Exception to indicate invalid email address.
        /// </summary>
        public InvalidEmailAddressException(string email)
            : base(string.Format("The provided email address \"{0}\" is not valid.", email))
        {
        }
    }
}