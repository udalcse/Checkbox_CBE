//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Summary description for AuthorizationException.
	/// </summary>
	public class AuthorizationException : System.Security.SecurityException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AuthorizationException() : base ("You do not have permission to perform this operation.")
		{
		}
	}
}
