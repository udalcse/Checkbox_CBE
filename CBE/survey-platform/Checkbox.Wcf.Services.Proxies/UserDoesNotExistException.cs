using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Indicates that an attempt to retrieve a user failed.
    /// The issue occurs when the user does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public UserDoesNotExistException(string uniqueIdentifier)
            : base(string.Format("Unable to locate the user named {0}. Either {0} does not exist or you do not have sufficient permissions to manage the user.", uniqueIdentifier))
        {
        }
    }
}
