using System;

using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
	/// <summary>
	/// Summary description for FormPolicy.
	/// </summary>
	[Serializable]
	public class FormPolicy : Policy
	{
		private static string[] allowablePermissions = { "Form.Administer", "Form.Edit", "Form.Fill", "Form.Delete", "Analysis.Create", "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.ManageFilters" };

		/// <summary>
		/// Constructor
		/// </summary>
		public FormPolicy() : base(allowablePermissions)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissions"></param>
		public FormPolicy(string[] permissions) : base(permissions)
		{
		}
	}
}
