//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Logging.Filters
{
	/// <summary>
	/// Interface definition for log filter objects.
	/// </summary>
	public interface ILogFilter
	{
		/// <summary>
		/// Evaluate a filter on the specified log entry.
		/// </summary>
		/// <param name="log">Log entry to base filter evaluation on.</param>
		/// <returns>True if the log entry satisfies the constraints of the filter.</returns>
		bool Filter(LogEntry log);
	}
}
