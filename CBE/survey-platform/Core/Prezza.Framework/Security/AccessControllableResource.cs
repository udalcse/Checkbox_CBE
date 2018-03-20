using System;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// A lightweight object which implements the IAccessControllable which is 
    /// intended to be used in place of larger Domain objects when authorizing 
    /// large datasets.
    /// </summary>
    public class AccessControllableResource : IAccessControllable
    {
        /// <summary>
        /// Member access list object.
        /// </summary>
        protected AccessControlList _acl;

        /// <summary>
        /// Member ID of the acl
        /// </summary>
        protected Nullable<Int32> _aclID;

        /// <summary>
        /// Member default policy
        /// </summary>
        protected Policy _defaultPolicy;

        /// <summary>
        /// Member default policy id
        /// </summary>
        protected Nullable<Int32> _defaultPolicyID;

        /// <summary>
        /// Get/Set the AclID.
        /// This value should correspond to the AclID of the Domain object which this 
        /// object is standing in for
        /// </summary>
        public Nullable<Int32> AclID
        {
            get { return _aclID; }
            set { _aclID = value; }
        }

        /// <summary>
        /// Get/Set the DefaultPolicyID
        /// This value should correspond to the DefaultPolicyID of the Domain object which 
        /// this object is standing in for
        /// </summary>
        public Nullable<Int32> DefaultPolicyID
        {
            get { return _defaultPolicyID; }
            set { _defaultPolicyID = value; }
        }

        /// <summary>
        /// Construct a AccessControllableResource and set the AclID and DefaultPolicyID
        /// </summary>
        /// <param name="aclID"></param>
        /// <param name="defaultPolicyID"></param>
        public AccessControllableResource(int aclID, int defaultPolicyID)
        {
            AclID = aclID;
            DefaultPolicyID = defaultPolicyID;
        }

        /// <summary>
        /// Get the default <see cref="Policy"/> for this resource
        /// </summary>
        public virtual Policy DefaultPolicy
        {
            get
            {
                if (_defaultPolicy != null)
                {
                    return _defaultPolicy;
                }

                else if (_defaultPolicyID != null && _defaultPolicyID > 0)
                {
                    _defaultPolicy = Policy.GetPolicy(_defaultPolicyID.Value);

                    return _defaultPolicy;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the <see cref="AccessControlList"/> for this resource
        /// </summary>
        public AccessControlList ACL
        {
            get
            {
                if (_acl != null)
                {
                    return _acl;
                }
                else if (_aclID != null && _aclID > 0)
                {
                    _acl = AccessControlList.Find(_aclID.Value);

                    return _acl;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Factory method creates <see cref="Policy"/> of Type appropriate to IAccessControllable implementation
        /// </summary>
        /// <param name="permissions">the permissions for the <see cref="Policy"/></param>
        /// <returns></returns>
        public virtual Policy CreatePolicy(string[] permissions)
        {
            return new Policy(permissions);
        }

        /// <summary>
        /// Unimplemented method stub, it has been included in order to comply IAccessControllable interface
        /// </summary>
        public virtual string[] SupportedPermissions
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Unimplemented method stub, it has been included in order to comply IAccessControllable interface
        /// </summary>
        public string[] SupportedPermissionMasks
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Unimplemented method stub, it has been included in order to comply IAccessControllable interface
        /// </summary>
        /// <returns></returns>
        public SecurityEditor GetEditor()
        {
            throw new NotImplementedException();
        }
    }
}