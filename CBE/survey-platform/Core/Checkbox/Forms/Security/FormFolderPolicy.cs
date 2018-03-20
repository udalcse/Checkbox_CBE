//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
	/// <summary>
	/// Summary description for FormFolderPolicy.
	/// </summary>
	[Serializable]
	internal class FormFolderPolicy : Policy
	{
		private static string[] allowablePermissions = { "FormFolder.Read",
														   "FormFolder.FullControl" };

		/// <summary>
		/// Constructor
		/// </summary>
		public FormFolderPolicy() : base(allowablePermissions)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissions"></param>
		public FormFolderPolicy(string[] permissions) : base(permissions)
		{
		}
	}
}
