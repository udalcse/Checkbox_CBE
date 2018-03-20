//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Logging
{
	/// <summary>
	/// Specify how log messages will be filtered by category.
	/// </summary>
	public enum CategoryFilterMode
	{
		/// <summary>
		/// Allow all categories except those listed in the configuration.  Listed categories will be denied.
		/// </summary>
		AllowAllExceptDenied,

		/// <summary>
		/// Deny all categories except those listed in the configuration.  Listed categories will be denied.
		/// </summary>
		DenyAllExceptAllowed
	}
}
