//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Defines a set of _permissions
    /// </summary>
    [Serializable]
    public class Role : IAccessPermissible
    {
        /// <summary>
        /// A container for the _permissions associated with this Role
        /// </summary>
        private List<string> _permissions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param _name="_permissions">a string array of Permissions to initialize this Role with</param>
        public Role(params string[] permissions)
        {
            _permissions = new List<string>(permissions);
            _permissions.Sort();
        }

        /// <summary>
        /// Get the permissions of the role
        /// </summary>
        public List<string> Permissions
        {
            get { return _permissions ?? (_permissions = new List<string>()); }
        }

        /// <summary>
        /// Get/set id of the role
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the string Name of this Role
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the string Description of this Role
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the AccessControl Entry Identifier 
        /// </summary>
        public string AclEntryIdentifier
        {
            get { return Name; }
        }

        /// <summary>
        /// Gets the string type idenifier for use in Access Control Entries
        /// </summary>
        public string AclTypeIdentifier
        {
            get { return "Prezza.Framework.Security.Role"; }
        }


        /// <summary>
        /// Checks whether this Role contains a given Permission
        /// </summary>
        /// <param _name="permission">the permission to check for</param>
        /// <returns>true, if found; otherwise false</returns>
        public virtual bool HasPermission(string permission)
        {
            if ("System Administrator".Equals(Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            //Use binary search for case-insensitive searching
            return Permissions.BinarySearch(permission, StringComparer.InvariantCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Return a boolean indicating if the role object contains all permissions.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual bool HasAllPermissions(string[] permissions)
        {
            return permissions.All(HasPermission);
        }

        /// <summary>
        /// Return a boolean indicating if the role object contains at least
        /// one of the specified permissions.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual bool HasAtLeastOnePermission(string[] permissions)
        {
            return permissions.Any(HasPermission);
        }
    }
}
