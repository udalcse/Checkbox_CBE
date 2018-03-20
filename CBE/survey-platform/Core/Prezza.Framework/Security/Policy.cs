//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// A type of <see cref="Role"/> that includes additional constraints or rules governing the set of permissions.
    /// </summary>
    /// <remarks>
    /// A Policy is used to define access rules within the context of a given implementer of <see cref="IAccessControllable"/> 
    /// </remarks>
    [Serializable]
    public class Policy : Role
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Policy()
            : base(new string[0])
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permissions">permissions belonging to this Policy</param>
        public Policy(params string[] permissions)
            : base(permissions)
        {
        }

        /// <summary>
        /// Property defines the permissions supported by a given Policy subclass
        /// </summary>
        public virtual string[] SupportedPermissions
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// Persist the policy, should only be used when creating policies that are not part
        /// of access control lists...i.e. default policies, etc.
        /// </summary>
        /// <returns></returns>
        public virtual int Persist()
        {
            return PolicyMapper.Insert(this);
        }

        /// <summary>
        /// Checks if the rules of this Policy have been met
        /// </summary>
        /// <returns>true, if Policy met; otherwise false</returns>
        public virtual bool VerifyPolicy()
        {
            if (SupportedPermissions == null)
            {
                return false;
            }

            ArrayList allowables = new ArrayList(SupportedPermissions);

            foreach (string permission in Permissions)
            {
                if (!allowables.Contains(permission))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a Policy given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Policy GetPolicy(int id)
        {
            var policy = PolicyMapper.Find(id);

            if(policy != null)
            {
                policy.Id = id;
            }

            return policy;
        }

        /// <summary>
        /// Remove policy from cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void UpdatePolicy(int id)
        {
            PolicyMapper.CleanupPolicyCaches(id);
        }

    }
}
