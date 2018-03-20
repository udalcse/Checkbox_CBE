//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Represents an entry in and <see cref="AccessControlList"/>
    /// </summary>
    [Serializable]
    public class AccessControlEntry : IAccessControlEntry
    {
        private Policy _policy;

        /// <summary>
        /// Initialze access control entry with entry identifier and policy id for
        /// lazy loading.
        /// </summary>
        /// <param name="aclEntryTypeIdentifier">ACL Entry type id.</param>
        /// <param name="aclEntryIdentifier">Entry identifier unique to the entry type.</param>
        /// <param name="policyId">Entry policy id.</param>
        public AccessControlEntry(string aclEntryTypeIdentifier, string aclEntryIdentifier, int policyId)
        {
            AclEntryTypeIdentifier = aclEntryTypeIdentifier;
            AclEntryIdentifier = aclEntryIdentifier;
            PolicyId = policyId;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="permissible">Permissible object to create _policy entry for.</param>
        /// <param name="policy">Policy for entry.</param>
        public AccessControlEntry(IAccessPermissible permissible, Policy policy)
        {
            AclEntryIdentifier = permissible.AclEntryIdentifier;
            AclEntryTypeIdentifier = permissible.AclTypeIdentifier;
            Policy = policy;
        }

        /// <summary>
        /// Get the entry _policy.
        /// </summary>
        public Policy Policy
        {
            get
            {
                if (_policy == null && PolicyId > 0)
                {
                    _policy = Policy.GetPolicy(PolicyId);
                }

                return _policy;
            }

            private set
            {
                _policy = value;
            }
        }

        /// <summary>
        /// Get/set id of acl entry policy
        /// </summary>
        public int PolicyId { get; private set; }

        /// <summary>
        /// Get/set acl entry type identifier
        /// </summary>
        public string AclEntryTypeIdentifier { get; private set; }

        /// <summary>
        /// Get/set entry identifier
        /// </summary>
        public string AclEntryIdentifier { get; private set; }

        /// <summary>
        /// Uses to check entry availability
        /// </summary>
        public bool IsInList{ get; set; }

        #region IEquatable<IAccessControlEntry> Members

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IAccessControlEntry other)
        {
            return
                other.AclEntryIdentifier.Equals(AclEntryIdentifier, StringComparison.InvariantCultureIgnoreCase)
                && other.AclEntryTypeIdentifier.Equals(AclEntryTypeIdentifier, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
