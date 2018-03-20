//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

namespace Checkbox.Security
{
	/// <summary>
	/// Method to join permissions when doing permissions queries.
	/// </summary>
	public enum PermissionJoin
	{
		/// <summary>
		/// All permissions must match.
		/// </summary>
		All,

		/// <summary>
		/// Any permission must match.
		/// </summary>
		Any
	}
}
