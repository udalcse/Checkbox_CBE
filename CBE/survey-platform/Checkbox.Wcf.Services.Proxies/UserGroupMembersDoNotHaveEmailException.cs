using System;
using System.Collections.Generic;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UserGroupMembersDoNotHaveEmailException : Exception
    {
        /// <summary>
        /// Exception to verify the existence of the user's email.
        /// </summary>
        public UserGroupMembersDoNotHaveEmailException(string groupName, IEnumerable<string> uniqueIdentifiers)
            : base(string.Format("Following members of '{0}' group do not have an email: {1}", groupName, string.Join(", ", uniqueIdentifiers)))
        {
        }
    }
}
