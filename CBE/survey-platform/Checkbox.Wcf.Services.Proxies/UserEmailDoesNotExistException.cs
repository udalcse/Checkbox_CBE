using System;
using System.Collections.Generic;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// The issue occurs when the user's email does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class UserEmailDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception to verify the existence of the user's email.
        /// </summary>
        public UserEmailDoesNotExistException(IEnumerable<string> uniqueIdentifiers)
            : base("Following users do not have an email: " + string.Join(", ", uniqueIdentifiers))
        {
        }
    }
}