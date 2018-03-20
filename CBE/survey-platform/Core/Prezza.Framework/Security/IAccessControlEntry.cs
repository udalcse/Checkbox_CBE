using System;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Interface for container objects for access control entriers.
    /// </summary>
    public interface IAccessControlEntry : IEquatable<IAccessControlEntry>
    {
        /// <summary>
        /// Get the entry _policy.
        /// </summary>
        Policy Policy { get; }

        /// <summary>
        /// Get/set id of acl entry policy
        /// </summary>
        int PolicyId { get; }

        /// <summary>
        /// Get/set acl entry type identifier
        /// </summary>
        string AclEntryTypeIdentifier { get; }

        /// <summary>
        /// Get/set entry identifier
        /// </summary>
        string AclEntryIdentifier { get; }

        /// <summary>
        /// Uses to check entry availability
        /// </summary>
        bool IsInList { get; set; }
    }
}