using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Indicates that an attempt to retrieve a user group failed.
    /// The issue occurs when the user group does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class UserGroupDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public UserGroupDoesNotExistException(int groupId)
            : base(string.Format("Unable to locate the user group with id {0}. Either the group does not exist or you do not have sufficient permissions to manage the group.", groupId))
        {
        }

        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public UserGroupDoesNotExistException(string groupName)
            : base(string.Format("Unable to locate the {0} user group. Either the group does not exist or you do not have sufficient permissions to manage the group.", groupName))
        {
        }
    }
}
