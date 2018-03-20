//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Security.Principal;

namespace Checkbox.Users
{
	/// <summary>
	/// An implicit group that contains all users in the system.
	/// </summary>
    [Serializable]
	public class EveryoneGroup : Group
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EveryoneGroup(string name, string description, int aclId, int defaultPolicyId) 
            : base(name, description, 1, new string[]{})
		{
            SetAclID(aclId);
            SetDefaultPolicyID(defaultPolicyId);
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="principal"></param>
		new public void AddUser(IPrincipal principal)
		{
			throw new NotSupportedException("Users cannot be added to the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="identity"></param>
		new public void AddUser(IIdentity identity)
		{
			throw new NotSupportedException("Users cannot be added to the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="uniqueName"></param>
		new public void AddUser(string uniqueName)
		{
			throw new NotSupportedException("Users cannot be added to the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="principal"></param>
		new public void RemoveUser(IPrincipal principal)
		{
			throw new NotSupportedException("Users cannot be removed from the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="identity"></param>
		new public void RemoveUser(IIdentity identity)
		{
			throw new NotSupportedException("Users cannot be removed from the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <param name="uniqueName"></param>
		new public void RemoveUser(string uniqueName)
		{
			throw new NotSupportedException("Users cannot be removed from the Everyone group.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <returns></returns>
		new public IPrincipal[] GetUsers()
		{
			throw new NotSupportedException("Listing of users in the Everyone group is not supported.");
		}

		/// <summary>
		/// Throws exception.
		/// </summary>
		/// <returns></returns>
		new public IIdentity[] GetUserIdentities()
		{
			throw new NotSupportedException("Listing of users in the Everyone group is not supported.");
		}
	}
}
