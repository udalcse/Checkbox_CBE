//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;

namespace Prezza.Framework.Logging
{
	/// <summary>
	/// Severity of a log entry.
	/// </summary>
	public enum Severity
	{
		/// <summary>
		/// No severity was specified.
		/// </summary>
		Unspecified,

		/// <summary>
		/// The entry contains informational content only.
		/// </summary>
		Information,

		/// <summary>
		/// The entry contains a warning of a condition that may result in an error.
		/// </summary>
		Warning,

		/// <summary>
		/// The entry contains a description of an error condition.
		/// </summary>
		Error
	}

	/// <summary>
	/// (NOT IMPLEMENTED) Map log severity levels to EventLogEntryType
	/// </summary>
	internal class SeverityMap
	{
		/// <summary>
		/// (NOT IMPLEMENTED)
		/// </summary>
		public SeverityMap()
		{
		}
	}
}
