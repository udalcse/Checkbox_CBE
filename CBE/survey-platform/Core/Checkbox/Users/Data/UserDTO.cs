using System;
using System.Security.Principal;

namespace Checkbox.Users.Data
{
    /// <summary>
    /// User data transfer object. 
    /// This class is intended to be a light weight container used for short term storage
    /// and the transfering of user data. By design it contains no business logic.
    /// </summary>
    public class UserDto
    {
        #region Member Variables

        private IIdentity _identity;

        #endregion

        #region Properties

        public string UniqueIdentifier { get; set; }

        public string EmailAddress { get; set; }

        public string Domain { get; set; }

        public string Password { get; set; }

        #endregion

        #region Constructor(s)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="emailAddress"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        public UserDto(string uniqueIdentifier, string emailAddress, string domain, string password)
        {
            UniqueIdentifier = uniqueIdentifier;
            EmailAddress = emailAddress;
            Domain = domain;
            Password = password;
        }
        #endregion
    }
}