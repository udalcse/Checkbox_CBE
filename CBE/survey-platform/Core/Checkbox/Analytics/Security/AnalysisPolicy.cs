using System;
using Prezza.Framework.Security;

namespace Checkbox.Analytics.Security
{
	/// <summary>
	/// Summary description for AnalysisPolicy.
	/// </summary>
	[Serializable]
	internal class AnalysisPolicy : Policy
	{
		private static string[] allowablePermissions = { "Analysis.Edit", "Analysis.Delete", "Analysis.Run" };

		/// <summary>
		/// Constructor
		/// </summary>
		public AnalysisPolicy() : base(allowablePermissions)
		{
		}

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissions">permissions set for this instance</param>
		public AnalysisPolicy(string[] permissions) : base(permissions)
		{
		}
	}
}
