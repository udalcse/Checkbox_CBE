//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Security token interface.  A token supports retrieving its value as a string and is used to access
	/// items in a security cache.
	/// </summary>
	public interface IToken
	{
		/// <summary>
		/// Get the token contents as a string
		/// </summary>
		string Value
		{
			get;
		}
	}
}
