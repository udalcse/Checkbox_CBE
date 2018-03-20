//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===========================================================================
using System;
using Prezza.Framework.Security;

namespace Checkbox.Panels.Security
{
	/// <summary>
	/// Email List security policy.
	/// </summary>
	[Serializable]
	internal class EmailListPolicy : Policy
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EmailListPolicy() : base()
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="permissions">Permissions to initialize policy with.</param>
		public EmailListPolicy(string[] permissions) : base(permissions)
		{			
		}
	}
}
