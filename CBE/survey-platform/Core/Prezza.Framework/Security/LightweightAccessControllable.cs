using System;
using System.Data;
using Prezza.Framework.Data.Sprocs;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Lightweight access controllable object that can be used for authentication purposes only. It does
    /// not support any access editing functionality, but is suitable as a base for other access controllable
    /// resources.
    /// </summary>
    [Serializable]
    public class LightweightAccessControllable : IAccessControllable
    {
        private Policy _defaultPolicy;
        private AbstractAccessControlList _acl;

        /// <summary>
        /// Get/set the acl id
        /// </summary>
        [FetchParameter(Name = "AclID", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        public int AclID { get; set; }

        /// <summary>
        /// Get/set default policy id
        /// </summary>
        [FetchParameter(Name = "DefaultPolicy", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        public int DefaultPolicyID { get; set; }

        /// <summary>
        /// Get/set id of access controllable entity
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// Get/set name of access controllable entity
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Get/set type of access controllable entity
        /// </summary>
        public virtual string EntityType { get; set; }

        /// <summary>
        /// Get/set "Owner" of entity.  Depending on security mode, owner may have
        /// different meaning.
        /// </summary>
        public virtual string Owner { get; set; }

        /// <summary>
        /// Get/set count of children entities. For the folder it's a count of surveys stored in this folder.
        /// </summary>
        public virtual int ChildrenCount { get; set; }

        /// <summary>
        /// Get string version of the item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}] [{1}] {2}", EntityType, ID, Name);
        }

        #region IAccessControllable Members

        /// <summary>
        /// Get the default <see cref="Policy"/> for this resource
        /// </summary>
        public virtual Policy DefaultPolicy
        {
            get
            {
                if (_defaultPolicy == null && DefaultPolicyID > 0)
                {
                    _defaultPolicy = Policy.GetPolicy(DefaultPolicyID);
                }

                return _defaultPolicy;
            }
        }

        /// <summary>
        /// Get the <see cref="AccessControlList"/> for this resource
        /// </summary>
        public IAccessControlList ACL
        {
            get
            {
                if (_acl == null && AclID > 0)
                {
                    _acl = new AccessControlList(AclID);
                }

                return _acl;
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual Policy CreatePolicy(string[] permissions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public virtual string[] SupportedPermissions
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public virtual string[] SupportedPermissionMasks
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public virtual SecurityEditor GetEditor()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
