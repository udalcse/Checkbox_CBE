using System;

using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
	/// <summary>
	/// Summary description for ItemLibraryPolicy.
	/// </summary>
	[Serializable]
    internal class LibraryPolicy : Policy
	{
		private static string[] allowablePermissions = { "Library.Edit", "Library.View", "Library.Delete" };

		public LibraryPolicy() : base(allowablePermissions)
		{
		}

		public LibraryPolicy(string[] permissions) : base(permissions)
		{
		}

        public static string[] AllowablePermissions
        {
            get { return allowablePermissions; }
        }
	}
}
