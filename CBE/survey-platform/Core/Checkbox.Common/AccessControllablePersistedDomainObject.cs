using System;

using Prezza.Framework.Security;

namespace Checkbox.Common
{
    /// <summary>
    /// Base class for persisted domain objects requiring access control.
    /// </summary>
    [Serializable]
    public abstract class AccessControllablePersistedDomainObject : PersistedDomainObject, IAccessControllable
    {
        /// <summary>
        /// The default <see cref="Policy"/> for this resource
        /// </summary>
        private Policy _defaultPolicy;

        /// <summary>
        /// The <see cref="AccessControlList"/> for this resource.
        /// </summary>
        private IAccessControlList _acl;

        /// <summary>
        /// This array of supported permissions for this resource.
        /// </summary>
        private readonly string[] _supportedPermissions;

        /// <summary>
        /// The array of supported permissions masks for this resource.
        /// </summary>
        private readonly string[] _supportedPermissionMasks;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="supportedPermissionMasks"></param>
        /// <param name="supportedPermissions"></param>
        protected AccessControllablePersistedDomainObject(string[] supportedPermissionMasks, string[] supportedPermissions)
        {
            _supportedPermissionMasks = supportedPermissionMasks;
            _supportedPermissions = supportedPermissions;
        }

        /// <summary>
        /// Set access to the object
        /// </summary>
        /// <param name="defaultPolicy"></param>
        /// <param name="acl"></param>
        protected void SetAccess(Policy defaultPolicy, AccessControlList acl)
        {
            if (defaultPolicy != null)
            {
                DefaultPolicyID = defaultPolicy.Persist();
                DefaultPolicy = defaultPolicy;
            }

            if (acl != null)
            {
                AclID = Convert.ToInt32(acl.ID);
                ACL = acl;
            }
        }

        /// <summary>
        /// Get the default policy id
        /// </summary>
        public int? DefaultPolicyID { get; set; }

        /// <summary>
        /// Get the AclID for the item
        /// </summary>
        public int? AclID { get; set; }

        /// <summary>
        /// Get the name of the DB table containing PDO domain object data.  Value is used for updating security information
        /// </summary>
        public abstract string DomainDBTableName { get; }

        ///<summary>
        ///</summary>
        public abstract string DomainDBIdentityColumnName { get; }


        #region IAccessControllable Members
        /// <summary>
        /// Get the default policy for the item
        /// </summary>
        public virtual Policy DefaultPolicy
        {
            get
            {
                if (_defaultPolicy != null)
                {
                    return _defaultPolicy;
                }

                if (DefaultPolicyID != null && DefaultPolicyID > 0)
                {
                    _defaultPolicy = Policy.GetPolicy(DefaultPolicyID.Value);

                    return _defaultPolicy;
                }

                return null;
            }

            protected set { _defaultPolicy = value; }
        }

        /// <summary>
        /// Cause the default policy to be reloaded on next access
        /// </summary>
        public virtual void ReloadDefaultPolicy()
        {
            DefaultPolicy = null;
        }

        /// <summary>
        /// Cause the acl to be reloaded on next access
        /// </summary>
        public virtual void ReloadACL()
        {
            ACL = null;
        }

        /// <summary>
        /// Get the access control list for the item
        /// </summary>
        public IAccessControlList ACL
        {
            get
            {
                if (_acl == null
                    && AclID.HasValue
                    && AclID > 0)
                {
                    _acl = new AccessControlList(AclID.Value);
                }

                return _acl;
            }

            protected set { _acl = value; }
        }

        /// <summary>
        /// Create a policy for the item
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual Policy CreatePolicy(string[] permissions)
        {
            return new Policy(permissions);
        }

        /// <summary>
        /// Get the item's supported permissions
        /// </summary>
        public virtual string[] SupportedPermissions
        {
            get { return _supportedPermissions; }
        }

        /// <summary>
        /// Get the item's supported permission masks
        /// </summary>
        public virtual string[] SupportedPermissionMasks
        {
            get { return _supportedPermissionMasks; }
        }

        /// <summary>
        /// Get a security editor for the item
        /// </summary>
        /// <returns></returns>
        public abstract SecurityEditor GetEditor();

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        #endregion
    }
}