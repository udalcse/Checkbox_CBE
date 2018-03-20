//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Specify which action to take after handling an exception.
	/// </summary>
	public enum PostHandlingAction
	{
		/// <summary>
		/// Do nothing after handling.
		/// </summary>
		None = 0,

		/// <summary>
		/// Log the exception and rethrow it.
		/// </summary>
		NotifyRethrow = 1,

		/// <summary>
		/// Generate a new exception and throw it.
		/// </summary>
		ThrowNewException = 2
	}
}
