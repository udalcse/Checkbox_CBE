//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Describes the interface implemented by objects which can be granted permissions in the system
	/// </summary>
	public interface IAccessPermissible
	{
		/// <summary>
		/// A string identifier of type for this 
		/// </summary>
		string AclTypeIdentifier { get; }

		/// <summary>
		/// A string identifier of an instance
		/// </summary>
		string AclEntryIdentifier { get; }
	}
}
