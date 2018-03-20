//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Runtime.Serialization;

namespace Prezza.Framework.Security.Principal
{
    /// <summary>
    /// Extended principal class that implements the <see cref="IPrincipal"/> and <see cref="IAccessPermissible" /> interfaces.
    /// </summary>
    [Serializable]
    public class ExtendedPrincipal : IAccessPermissible, IPrincipal
    {
        private List<string> _roles;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identity">Identity that identifies this principal.</param>
        /// <param name="roles">Roles this principal is a member of.</param>
        public ExtendedPrincipal(IIdentity identity, string[] roles)
        {
            InitializePrincipal(identity, roles);
        }

        /// <summary>
        /// Initialize a principal with the specified identity and _roles
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="roles"></param>
        protected virtual void InitializePrincipal(IIdentity identity, string[] roles)
        {
            Identity = identity;

            if (roles != null)
            {
                _roles = new List<string>(roles);
            }
            else
            {
                _roles = new List<string>();
            }

            //Sort the list, so the binary search can work correctly.
            _roles.Sort(StringComparer.InvariantCultureIgnoreCase);
        }

        #region IPrincipal Members

        /// <summary>
        /// Get the identity that identifies this principal.
        /// </summary>
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Determine if this principal is in the given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return _roles.BinarySearch(role) >= 0;
        }

        /// <summary>
        /// Determines whether [is role contains] [the specified substring].
        /// </summary>
        /// <param name="substring">The substring.</param>
        /// <returns>
        ///   <c>true</c> if [is role contains] [the specified substring]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRoleContains(string substring)
        {
            if (string.IsNullOrEmpty(substring))
                throw new ArgumentNullException(nameof(substring));

            return _roles.Any(item => item.Contains(substring));
        }

        /// <summary>
        /// Get the _roles list
        /// </summary>
        /// <returns></returns>
        public string[] GetRoles()
        {
            return _roles.ToArray();
        }


        #endregion

        #region IAccessPermissible Members

        /// <summary>
        /// Get the string representation of the ACL type for this principal
        /// </summary>
        public const string ExtendedPrincipalAclTypeIdentifier = "Prezza.Framework.Security.ExtendedPrincipal";

        /// <summary>
        /// 
        /// </summary>
        public string AclTypeIdentifier { get { return ExtendedPrincipalAclTypeIdentifier; } }

        /// <summary>
        /// Get the string representation of the ACL entry for this item.
        /// </summary>
        public string AclEntryIdentifier
        {
            get
            {
                return Identity.Name;
            }
        }

        #endregion
    }
}
