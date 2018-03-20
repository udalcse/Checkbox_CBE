using System;

using Prezza.Framework.Common;
using Prezza.Framework.Security;

namespace Checkbox.Users.Security
{
	/// <summary>
	/// Summary description for GroupPolicy.
	/// </summary>
	[Serializable]
	internal     class GroupPolicy : Policy
	{
		private static string[] allowablePermissions = { "Group.View", "Group.Delete", "Group.Edit", "Group.ManageUsers"};

		/// <summary>
		/// Constructor
		/// </summary>
		public GroupPolicy() : base(allowablePermissions)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissions"></param>
		public GroupPolicy(string[] permissions) : base(permissions)
		{
		}
	}
}
